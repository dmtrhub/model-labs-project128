using CIM.Model;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter.Manager;
using System;
using System.Collections.Generic;

namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
    /// <summary>
    /// SwitchingImporter - konvertuje CIM Switch/SwitchingOperation u NMS Delta
    /// </summary>
    public class SwitchingImporter
    {
        /// <summary> Singleton </summary>
        private static SwitchingImporter switchingImporter = null;
        private static object singletonLock = new object();

        private ConcreteModel concreteModel;
        private Delta delta;
        private ImportHelper importHelper;
        private TransformAndLoadReport report;

        #region Properties
        public static SwitchingImporter Instance
        {
            get
            {
                if (switchingImporter == null)
                {
                    lock (singletonLock)
                    {
                        if (switchingImporter == null)
                        {
                            switchingImporter = new SwitchingImporter();
                            switchingImporter.Reset();
                        }
                    }
                }
                return switchingImporter;
            }
        }

        public Delta NMSDelta
        {
            get
            {
                return delta;
            }
        }
        #endregion Properties

        public void Reset()
        {
            concreteModel = null;
            delta = new Delta();
            importHelper = new ImportHelper();
            report = null;
        }

        public TransformAndLoadReport CreateNMSDelta(ConcreteModel cimConcreteModel)
        {
            LogManager.Log("Importing Switching Elements...", LogLevel.Info);
            report = new TransformAndLoadReport();
            concreteModel = cimConcreteModel;
            delta.ClearDeltaOperations();

            if ((concreteModel != null) && (concreteModel.ModelMap != null))
            {
                try
                {
                    ConvertModelAndPopulateDelta();
                }
                catch (Exception ex)
                {
                    string message = string.Format("{0} - ERROR in data import - {1}", DateTime.Now, ex.Message);
                    LogManager.Log(message);
                    report.Report.AppendLine(ex.Message);
                    report.Success = false;
                }
            }
            LogManager.Log("Importing Switching Elements - END.", LogLevel.Info);
            return report;
        }

        /// <summary>
        /// Konvertuje sve elemente iz konkretnog modela u DMS model
        /// </summary>
        private void ConvertModelAndPopulateDelta()
        {
            LogManager.Log("Loading elements and creating delta...", LogLevel.Info);

            if (concreteModel.ModelMap != null)
            {
                LogManager.Log($"DEBUG: concreteModel.ModelMap.Count = {concreteModel.ModelMap.Count}", LogLevel.Info);

                // Provjeri koja imena tipova su u ModelMap
                foreach (var kvp in concreteModel.ModelMap)
                {
                    LogManager.Log($"DEBUG: Found type in ModelMap: Key={kvp.Key}, Type={kvp.Value?.GetType().Name}", LogLevel.Info);
                }
            }
            else
            {
                LogManager.Log("DEBUG: concreteModel.ModelMap je NULL!", LogLevel.Info);
                return;
            }

            // Provjeri sa različitim imenima
            LogManager.Log("DEBUG: Pokušavam GetAllObjectsOfType(\"FTN.Curve\")...", LogLevel.Info);
            SortedDictionary<string, object> cimCurves = concreteModel.GetAllObjectsOfType("FTN.Curve");
            LogManager.Log($"DEBUG: Rezultat: {cimCurves?.Count ?? 0} objekata", LogLevel.Info);

            if (cimCurves == null)
            {
                LogManager.Log("DEBUG: GetAllObjectsOfType vratilo NULL - pokušavam sa \"Curve\"", LogLevel.Info);
                cimCurves = concreteModel.GetAllObjectsOfType("Curve");
                LogManager.Log($"DEBUG: Sa \"Curve\": {cimCurves?.Count ?? 0} objekata", LogLevel.Info);
            }

            // Import redosled je važan - prvo reference, zatim objekti koji ih koriste
            ImportCurves();
            ImportCurveData();
            ImportRegularIntervalSchedules();
            ImportRegularTimePoints();
            ImportIrregularIntervalSchedules();
            ImportIrregularTimePoints();
            ImportOutageSchedules();
            ImportSwitchingOperations();  // SWOP mora biti pre Switch-a (Switch referencira SWOP)
            ImportSwitches();

            LogManager.Log("Loading elements and creating delta completed.", LogLevel.Info);
        }

        #region Import Methods

        private void ImportCurves()
        {
            SortedDictionary<string, object> cimCurves = concreteModel.GetAllObjectsOfType("FTN.Curve");
            if (cimCurves != null)
            {
                foreach (KeyValuePair<string, object> cimCurvePair in cimCurves)
                {
                    FTN.Curve cimCurve = cimCurvePair.Value as FTN.Curve;
                    ResourceDescription rd = CreateCurveResourceDescription(cimCurve);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("Curve ID = ").Append(cimCurve.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("Curve ID = ").Append(cimCurve.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateCurveResourceDescription(FTN.Curve cimCurve)
        {
            ResourceDescription rd = null;
            if (cimCurve != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.CURVE, importHelper.CheckOutIndexForDMSType(DMSType.CURVE));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimCurve.ID, gid);

                SwitchingConverter.PopulateCurveProperties(cimCurve, rd);
            }
            return rd;
        }

        private void ImportCurveData()
        {
            SortedDictionary<string, object> cimCurveDataObjects = concreteModel.GetAllObjectsOfType("FTN.CurveData");
            if (cimCurveDataObjects != null)
            {
                foreach (KeyValuePair<string, object> cimCurveDataPair in cimCurveDataObjects)
                {
                    FTN.CurveData cimCurveData = cimCurveDataPair.Value as FTN.CurveData;
                    ResourceDescription rd = CreateCurveDataResourceDescription(cimCurveData);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("CurveData ID = ").Append(cimCurveData.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("CurveData ID = ").Append(cimCurveData.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateCurveDataResourceDescription(FTN.CurveData cimCurveData)
        {
            ResourceDescription rd = null;
            if (cimCurveData != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.CURVEDATA, importHelper.CheckOutIndexForDMSType(DMSType.CURVEDATA));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimCurveData.ID, gid);

                SwitchingConverter.PopulateCurveDataProperties(cimCurveData, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportRegularIntervalSchedules()
        {
            SortedDictionary<string, object> cimRegularIntervalSchedules = concreteModel.GetAllObjectsOfType("FTN.RegularIntervalSchedule");
            if (cimRegularIntervalSchedules != null)
            {
                foreach (KeyValuePair<string, object> cimSchedulePair in cimRegularIntervalSchedules)
                {
                    FTN.RegularIntervalSchedule cimSchedule = cimSchedulePair.Value as FTN.RegularIntervalSchedule;
                    ResourceDescription rd = CreateRegularIntervalScheduleResourceDescription(cimSchedule);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("RegularIntervalSchedule ID = ").Append(cimSchedule.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("RegularIntervalSchedule ID = ").Append(cimSchedule.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateRegularIntervalScheduleResourceDescription(FTN.RegularIntervalSchedule cimSchedule)
        {
            ResourceDescription rd = null;
            if (cimSchedule != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.REGULARINTSCHEDULE, importHelper.CheckOutIndexForDMSType(DMSType.REGULARINTSCHEDULE));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimSchedule.ID, gid);

                SwitchingConverter.PopulateRegularIntervalScheduleProperties(cimSchedule, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportRegularTimePoints()
        {
            SortedDictionary<string, object> cimRegularTimePoints = concreteModel.GetAllObjectsOfType("FTN.RegularTimePoint");
            if (cimRegularTimePoints != null)
            {
                foreach (KeyValuePair<string, object> cimTimePointPair in cimRegularTimePoints)
                {
                    FTN.RegularTimePoint cimTimePoint = cimTimePointPair.Value as FTN.RegularTimePoint;
                    ResourceDescription rd = CreateRegularTimePointResourceDescription(cimTimePoint);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("RegularTimePoint ID = ").Append(cimTimePoint.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("RegularTimePoint ID = ").Append(cimTimePoint.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateRegularTimePointResourceDescription(FTN.RegularTimePoint cimTimePoint)
        {
            ResourceDescription rd = null;
            if (cimTimePoint != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.REGULARTIMEPOINT, importHelper.CheckOutIndexForDMSType(DMSType.REGULARTIMEPOINT));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimTimePoint.ID, gid);

                SwitchingConverter.PopulateRegularTimePointProperties(cimTimePoint, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportIrregularIntervalSchedules()
        {
            SortedDictionary<string, object> cimIrregularIntervalSchedules = concreteModel.GetAllObjectsOfType("FTN.IrregularIntervalSchedule");
            if (cimIrregularIntervalSchedules != null)
            {
                foreach (KeyValuePair<string, object> cimSchedulePair in cimIrregularIntervalSchedules)
                {
                    FTN.IrregularIntervalSchedule cimSchedule = cimSchedulePair.Value as FTN.IrregularIntervalSchedule;
                    ResourceDescription rd = CreateIrregularIntervalScheduleResourceDescription(cimSchedule);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("IrregularIntervalSchedule ID = ").Append(cimSchedule.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("IrregularIntervalSchedule ID = ").Append(cimSchedule.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateIrregularIntervalScheduleResourceDescription(FTN.IrregularIntervalSchedule cimSchedule)
        {
            ResourceDescription rd = null;
            if (cimSchedule != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.IRREGULARINTSCHEDULE, importHelper.CheckOutIndexForDMSType(DMSType.IRREGULARINTSCHEDULE));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimSchedule.ID, gid);

                SwitchingConverter.PopulateIrregularIntervalScheduleProperties(cimSchedule, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportIrregularTimePoints()
        {
            SortedDictionary<string, object> cimIrregularTimePoints = concreteModel.GetAllObjectsOfType("FTN.IrregularTimePoint");
            if (cimIrregularTimePoints != null)
            {
                foreach (KeyValuePair<string, object> cimTimePointPair in cimIrregularTimePoints)
                {
                    FTN.IrregularTimePoint cimTimePoint = cimTimePointPair.Value as FTN.IrregularTimePoint;
                    ResourceDescription rd = CreateIrregularTimePointResourceDescription(cimTimePoint);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("IrregularTimePoint ID = ").Append(cimTimePoint.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("IrregularTimePoint ID = ").Append(cimTimePoint.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateIrregularTimePointResourceDescription(FTN.IrregularTimePoint cimTimePoint)
        {
            ResourceDescription rd = null;
            if (cimTimePoint != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.IRREGULARTIMEPOINT, importHelper.CheckOutIndexForDMSType(DMSType.IRREGULARTIMEPOINT));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimTimePoint.ID, gid);

                SwitchingConverter.PopulateIrregularTimePointProperties(cimTimePoint, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportOutageSchedules()
        {
            SortedDictionary<string, object> cimOutageSchedules = concreteModel.GetAllObjectsOfType("FTN.OutageSchedule");
            if (cimOutageSchedules != null)
            {
                foreach (KeyValuePair<string, object> cimSchedulePair in cimOutageSchedules)
                {
                    FTN.OutageSchedule cimSchedule = cimSchedulePair.Value as FTN.OutageSchedule;
                    ResourceDescription rd = CreateOutageScheduleResourceDescription(cimSchedule);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("OutageSchedule ID = ").Append(cimSchedule.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("OutageSchedule ID = ").Append(cimSchedule.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateOutageScheduleResourceDescription(FTN.OutageSchedule cimSchedule)
        {
            ResourceDescription rd = null;
            if (cimSchedule != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.OUTAGESCHEDULE, importHelper.CheckOutIndexForDMSType(DMSType.OUTAGESCHEDULE));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimSchedule.ID, gid);

                SwitchingConverter.PopulateOutageScheduleProperties(cimSchedule, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportSwitches()
        {
            SortedDictionary<string, object> cimSwitches = concreteModel.GetAllObjectsOfType("FTN.Switch");
            if (cimSwitches != null)
            {
                foreach (KeyValuePair<string, object> cimSwitchPair in cimSwitches)
                {
                    FTN.Switch cimSwitch = cimSwitchPair.Value as FTN.Switch;
                    ResourceDescription rd = CreateSwitchResourceDescription(cimSwitch);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("Switch ID = ").Append(cimSwitch.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("Switch ID = ").Append(cimSwitch.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateSwitchResourceDescription(FTN.Switch cimSwitch)
        {
            ResourceDescription rd = null;
            if (cimSwitch != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.SWITCH, importHelper.CheckOutIndexForDMSType(DMSType.SWITCH));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimSwitch.ID, gid);

                SwitchingConverter.PopulateSwitchProperties(cimSwitch, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportSwitchingOperations()
        {
            SortedDictionary<string, object> cimSwitchingOperations = concreteModel.GetAllObjectsOfType("FTN.SwitchingOperation");
            if (cimSwitchingOperations != null)
            {
                foreach (KeyValuePair<string, object> cimSwitchingOperationPair in cimSwitchingOperations)
                {
                    FTN.SwitchingOperation cimSwitchingOperation = cimSwitchingOperationPair.Value as FTN.SwitchingOperation;
                    ResourceDescription rd = CreateSwitchingOperationResourceDescription(cimSwitchingOperation);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("SwitchingOperation ID = ").Append(cimSwitchingOperation.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("SwitchingOperation ID = ").Append(cimSwitchingOperation.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateSwitchingOperationResourceDescription(FTN.SwitchingOperation cimSwitchingOperation)
        {
            ResourceDescription rd = null;
            if (cimSwitchingOperation != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.SWITCHINGOPERATION, importHelper.CheckOutIndexForDMSType(DMSType.SWITCHINGOPERATION));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimSwitchingOperation.ID, gid);

                SwitchingConverter.PopulateSwitchingOperationProperties(cimSwitchingOperation, rd, importHelper, report);
            }
            return rd;
        }

        #endregion Import Methods
    }
}