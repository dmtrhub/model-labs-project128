using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using FTN.Common;
using FTN.ServiceContracts;

namespace NMSTestClientGUI
{
    public partial class MainWindow : Window
    {
        private readonly ModelResourcesDesc modelResourcesDesc = new ModelResourcesDesc();
        private readonly EnumDescs enumDescs = new EnumDescs();

        private NetworkModelGDAProxy gdaProxy;
        private NetworkModelGDAProxy GdaProxy
        {
            get
            {
                if (gdaProxy != null)
                {
                    try { gdaProxy.Abort(); } catch { }
                    gdaProxy = null;
                }
                gdaProxy = new NetworkModelGDAProxy("NetworkModelGDAEndpoint");
                gdaProxy.Open();
                SetConnected(true);
                return gdaProxy;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            EnsureResultsDirectory();
            Log("NMS GUI Test Client started.");
        }

        #region Button handlers

        private void BtnGetValues_Click(object sender, RoutedEventArgs e)
        {
            string raw = TxtGVId.Text.Trim();
            if (!TryParseGid(raw, out long gid))
            {
                Log("ERROR: Invalid GID format. Use hex, e.g. 0x0000000700000001");
                return;
            }
            ExecuteGetValues(gid);
        }

        private void BtnGetExtentValues_Click(object sender, RoutedEventArgs e)
        {
            if (CmbEVType.SelectedItem == null) return;
            string typeName = ((ComboBoxItem)CmbEVType.SelectedItem).Content.ToString();
            if (!Enum.TryParse(typeName, out ModelCode modelCode))
            {
                Log($"ERROR: Unknown type '{typeName}'");
                return;
            }
            ExecuteGetExtentValues(modelCode);
        }

        private void BtnGetRelatedValues_Click(object sender, RoutedEventArgs e)
        {
            string raw = TxtRVSourceId.Text.Trim();
            if (!TryParseGid(raw, out long sourceGid))
            {
                Log("ERROR: Invalid Source GID format.");
                return;
            }
            if (CmbRVProp.SelectedItem == null || CmbRVType.SelectedItem == null) return;

            string propName = ((ComboBoxItem)CmbRVProp.SelectedItem).Content.ToString();
            string typeName = ((ComboBoxItem)CmbRVType.SelectedItem).Content.ToString();

            if (!Enum.TryParse(propName, out ModelCode propCode))
            {
                Log($"ERROR: Unknown property '{propName}'");
                return;
            }
            if (!Enum.TryParse(typeName, out ModelCode typeCode))
            {
                Log($"ERROR: Unknown type '{typeName}'");
                return;
            }
            Association assoc = new Association(propCode, typeCode);
            ExecuteGetRelatedValues(sourceGid, assoc);
        }

        private void BtnGetAllTypes_Click(object sender, RoutedEventArgs e)
        {
            ExecuteGetAllTypes();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ResultTree.Items.Clear();
            TxtLog.Text = string.Empty;
            TxtResultCount.Text = string.Empty;
            SetStatus("Cleared.");
        }

        #endregion

        #region GDA operations

        private void ExecuteGetValues(long gid)
        {
            Log($"GetValues → GID=0x{gid:x16}");
            SetStatus("Calling GetValues...");
            try
            {
                short typeCode = ModelCodeHelper.ExtractTypeFromGlobalId(gid);
                List<ModelCode> props = modelResourcesDesc.GetAllPropertyIds((DMSType)typeCode);

                ResourceDescription rd = GdaProxy.GetValues(gid, props);

                if (rd == null)
                {
                    Log("ERROR: No resource found for this GID.");
                    SetStatus("GetValues failed - no resource found.");
                    return;
                }

                SaveXml("GetValues_Results.xml", w =>
                {
                    rd.ExportToXml(w);
                });

                TreeViewItem root = MakeNode($"GetValues  [0x{gid:x16}]  — {(DMSType)typeCode}", "#2980B9");
                root.IsExpanded = true;
                PopulateRd(root, rd);
                InsertIntoTree(root);

                TxtResultCount.Text = $"{rd.Properties.Count} properties";
                Log($"GetValues OK → {rd.Properties.Count} props, type={(DMSType)typeCode}");
                SetStatus("GetValues completed.");
            }
            catch (Exception ex)
            {
                Log($"GetValues FAILED: {ex.Message}");
                SetStatus("GetValues failed.");
            }
        }

        private void ExecuteGetExtentValues(ModelCode modelCode)
        {
            Log($"GetExtentValues → {modelCode}");
            SetStatus($"Calling GetExtentValues({modelCode})...");
            int iteratorId = 0;
            try
            {
                const int batchSize = 2;
                List<ModelCode> props = modelResourcesDesc.GetAllPropertyIds(modelCode);
                iteratorId = GdaProxy.GetExtentValues(modelCode, props);
                int total = GdaProxy.IteratorResourcesTotal(iteratorId);
                int left  = GdaProxy.IteratorResourcesLeft(iteratorId);

                TreeViewItem root = MakeNode($"GetExtentValues  [{modelCode}]  — {total} resources", "#8E44AD");
                root.IsExpanded = true;

                List<ResourceDescription> allRds = new List<ResourceDescription>();

                SaveXml("GetExtentValues_Results.xml", w =>
                {
                    w.WriteStartElement("ExtentValues");
                    while (left > 0)
                    {
                        List<ResourceDescription> batch = GdaProxy.IteratorNext(batchSize, iteratorId);
                        foreach (ResourceDescription rd in batch)
                        {
                            allRds.Add(rd);
                            rd.ExportToXml(w);
                        }
                        left = GdaProxy.IteratorResourcesLeft(iteratorId);
                    }
                    w.WriteEndElement();
                });

                foreach (ResourceDescription rd in allRds)
                {
                    short t = ModelCodeHelper.ExtractTypeFromGlobalId(rd.Id);
                    TreeViewItem node = MakeNode($"[0x{rd.Id:x16}]  ({(DMSType)t})", "#2C3E50");
                    PopulateRd(node, rd);
                    root.Items.Add(node);
                }

                GdaProxy.IteratorClose(iteratorId);
                InsertIntoTree(root);

                TxtResultCount.Text = $"{allRds.Count} resources";
                Log($"GetExtentValues OK → {allRds.Count} resources of type {modelCode}");
                SetStatus($"GetExtentValues completed — {allRds.Count} resources.");
            }
            catch (Exception ex)
            {
                try { if (iteratorId != 0) GdaProxy.IteratorClose(iteratorId); } catch { }
                Log($"GetExtentValues FAILED: {ex.Message}");
                SetStatus("GetExtentValues failed.");
            }
        }

        private void ExecuteGetRelatedValues(long sourceGid, Association association)
        {
            Log($"GetRelatedValues → source=0x{sourceGid:x16}, prop={association.PropertyId}, type={association.Type}");
            SetStatus("Calling GetRelatedValues...");
            int iteratorId = 0;
            try
            {
                const int batchSize = 2;

                // Uzmi sve propertije target tipa (ne samo IDOBJ propertije)
                List<ModelCode> props;
                if (association.Type != 0)
                {
                    DMSType targetType = ModelResourcesDesc.GetTypeFromModelCode(association.Type);
                    props = modelResourcesDesc.GetAllPropertyIds(targetType);
                }
                else
                {
                    props = new List<ModelCode>
                    {
                        ModelCode.IDOBJ_MRID,
                        ModelCode.IDOBJ_NAME,
                        ModelCode.IDOBJ_DESCRIPTION
                    };
                }

                iteratorId = GdaProxy.GetRelatedValues(sourceGid, props, association);
                int total = GdaProxy.IteratorResourcesTotal(iteratorId);
                int left  = GdaProxy.IteratorResourcesLeft(iteratorId);

                TreeViewItem root = MakeNode(
                    $"GetRelatedValues  [0x{sourceGid:x16}]  prop={association.PropertyId}  — {total} related",
                    "#16A085");
                root.IsExpanded = true;

                List<ResourceDescription> allRds = new List<ResourceDescription>();

                SaveXml("GetRelatedValues_Results.xml", w =>
                {
                    w.WriteStartElement("RelatedValues");
                    while (left > 0)
                    {
                        List<ResourceDescription> batch = GdaProxy.IteratorNext(batchSize, iteratorId);
                        foreach (ResourceDescription rd in batch)
                        {
                            allRds.Add(rd);
                            rd.ExportToXml(w);
                        }
                        left = GdaProxy.IteratorResourcesLeft(iteratorId);
                    }
                    w.WriteEndElement();
                });

                foreach (ResourceDescription rd in allRds)
                {
                    short t = ModelCodeHelper.ExtractTypeFromGlobalId(rd.Id);
                    TreeViewItem node = MakeNode($"[0x{rd.Id:x16}]  ({(DMSType)t})", "#2C3E50");
                    PopulateRd(node, rd);
                    root.Items.Add(node);
                }

                GdaProxy.IteratorClose(iteratorId);
                InsertIntoTree(root);

                TxtResultCount.Text = $"{allRds.Count} related";
                Log($"GetRelatedValues OK → {allRds.Count} related resources");
                SetStatus($"GetRelatedValues completed — {allRds.Count} resources.");
            }
            catch (Exception ex)
            {
                try { if (iteratorId != 0) GdaProxy.IteratorClose(iteratorId); } catch { }
                Log($"GetRelatedValues FAILED: {ex.Message}");
                SetStatus("GetRelatedValues failed.");
            }
        }

        private void ExecuteGetAllTypes()
        {
            Log("GetExtentValues ALL types...");
            SetStatus("Loading all types...");
            TreeViewItem root = MakeNode("All Types — Extent Values", "#D35400");
            root.IsExpanded = true;
            int grandTotal = 0;
            try
            {
                foreach (DMSType dmsType in Enum.GetValues(typeof(DMSType)))
                {
                    if (dmsType == DMSType.MASK_TYPE) continue;
                    try
                    {
                        ModelCode mc = modelResourcesDesc.GetModelCodeFromType(dmsType);
                        List<ModelCode> props = modelResourcesDesc.GetAllPropertyIds(dmsType);
                        int itId = GdaProxy.GetExtentValues(mc, props);
                        int left = GdaProxy.IteratorResourcesLeft(itId);
                        int cnt  = 0;
                        List<long> ids = new List<long>();
                        while (left > 0)
                        {
                            List<ResourceDescription> batch = GdaProxy.IteratorNext(1000, itId);
                            foreach (ResourceDescription rd in batch) ids.Add(rd.Id);
                            left = GdaProxy.IteratorResourcesLeft(itId);
                        }
                        GdaProxy.IteratorClose(itId);
                        cnt = ids.Count;
                        grandTotal += cnt;

                        TreeViewItem typeNode = MakeNode($"{dmsType}  ({cnt} resources)", "#2C3E50");
                        foreach (long id in ids)
                            typeNode.Items.Add(MakeNode($"0x{id:x16}", "#7F8C8D"));
                        root.Items.Add(typeNode);
                        Log($"  {dmsType}: {cnt}");
                    }
                    catch { }
                }
                InsertIntoTree(root);
                TxtResultCount.Text = $"Total: {grandTotal} resources";
                Log($"All types done. Grand total: {grandTotal}");
                SetStatus($"All types loaded. Total: {grandTotal} resources.");
            }
            catch (Exception ex)
            {
                Log($"GetAllTypes FAILED: {ex.Message}");
                SetStatus("Failed.");
            }
        }

        #endregion

        #region Helpers

        private void PopulateRd(TreeViewItem parent, ResourceDescription rd)
        {
            foreach (Property prop in rd.Properties)
            {
                string val = GetPropertyString(prop);
                parent.Items.Add(MakeNode($"{prop.Id}  =  {val}", "#566573"));
            }
        }

        private string GetPropertyString(Property prop)
        {
            try
            {
                switch (Property.GetPropertyType(prop.Id))
                {
                    case PropertyType.Float:
                        return prop.AsFloat().ToString("G");
                    case PropertyType.String:
                        return $"\"{prop.AsString()}\"";
                    case PropertyType.Int32:
                        return prop.AsInt().ToString();
                    case PropertyType.Int64:
                        return $"0x{prop.AsLong():x16}";
                    case PropertyType.Bool:
                        return prop.AsBool().ToString();
                    case PropertyType.Enum:
                        return enumDescs.GetStringFromEnum(prop.Id, prop.AsEnum());
                    case PropertyType.Reference:
                        long refVal = prop.AsReference();
                        return refVal == 0 ? "(none)" : $"0x{refVal:x16}";
                    case PropertyType.DateTime:
                        long ticks = prop.AsLong();
                        return ticks == 0 ? "(none)" : new DateTime(ticks).ToString("yyyy-MM-dd HH:mm:ss");
                    case PropertyType.TimeSpan:
                        return TimeSpan.FromSeconds(prop.AsLong()).ToString();
                    case PropertyType.ReferenceVector:
                        List<long> refs = prop.AsReferences();
                        if (refs == null || refs.Count == 0) return "(empty)";
                        return string.Join(" | ", refs.ConvertAll(r => $"0x{r:x16}"));
                    case PropertyType.FloatVector:
                        List<float> floats = prop.AsFloats();
                        if (floats == null || floats.Count == 0) return "(empty)";
                        return string.Join(", ", floats.ConvertAll(f => f.ToString("G")));
                    case PropertyType.Int32Vector:
                        List<int> ints = prop.AsInts();
                        if (ints == null || ints.Count == 0) return "(empty)";
                        return string.Join(", ", ints.ConvertAll(i => i.ToString()));
                    case PropertyType.Int64Vector:
                        List<long> longs = prop.AsLongs();
                        if (longs == null || longs.Count == 0) return "(empty)";
                        return string.Join(", ", longs.ConvertAll(l => $"0x{l:x16}"));
                    case PropertyType.StringVector:
                        List<string> strings = prop.AsStrings();
                        if (strings == null || strings.Count == 0) return "(empty)";
                        return string.Join(", ", strings.ConvertAll(s => $"\"{s}\""));
                    case PropertyType.EnumVector:
                        List<short> enums = prop.AsEnums();
                        if (enums == null || enums.Count == 0) return "(empty)";
                        return string.Join(", ", enums.ConvertAll(e => e.ToString()));
                    default:
                        return prop.PropertyValue?.ToString() ?? "null";
                }
            }
            catch { return "(read error)"; }
        }

        private TreeViewItem MakeNode(string header, string colorHex)
        {
            return new TreeViewItem
            {
                Header = header,
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(colorHex),
                Padding = new Thickness(2)
            };
        }

        private void InsertIntoTree(TreeViewItem item)
        {
            ResultTree.Items.Insert(0, item);
        }

        private void SaveXml(string filename, Action<XmlTextWriter> writeAction)
        {
            try
            {
                string dir = System.Configuration.ConfigurationManager.AppSettings["ResultDirecotry"] ?? "../Results";
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string path = Path.Combine(dir, filename);
                using (XmlTextWriter w = new XmlTextWriter(path, Encoding.Unicode))
                {
                    w.Formatting = Formatting.Indented;
                    writeAction(w);
                    w.Flush();
                }
            }
            catch (Exception ex) { Log($"Warning: XML save failed: {ex.Message}"); }
        }

        private void EnsureResultsDirectory()
        {
            try
            {
                string dir = System.Configuration.ConfigurationManager.AppSettings["ResultDirecotry"] ?? "../Results";
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            }
            catch { }
        }

        private static bool TryParseGid(string raw, out long gid)
        {
            gid = 0;
            if (string.IsNullOrWhiteSpace(raw)) return false;
            try
            {
                string s = raw.Trim();
                if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    gid = Convert.ToInt64(s.Substring(2), 16);
                else
                    gid = long.Parse(s);
                return true;
            }
            catch { return false; }
        }

        private void Log(string message)
        {
            TxtLog.Text = $"[{DateTime.Now:HH:mm:ss}]  {message}\n" + TxtLog.Text;
        }

        private void SetStatus(string msg)
        {
            TxtStatus.Text = $"[{DateTime.Now:HH:mm:ss}]  {msg}";
        }

        private void SetConnected(bool connected)
        {
            StatusDot.Fill = connected
                ? (SolidColorBrush)new BrushConverter().ConvertFrom("#27AE60")
                : (SolidColorBrush)new BrushConverter().ConvertFrom("#E74C3C");
            TxtConnectionStatus.Text = connected ? "Connected" : "Not connected";
        }

        #endregion
    }
}
