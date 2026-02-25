using System;
using System.IO;
using System.Reflection;
using System.Threading;
using CIM.Model;
using CIMParser;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter.Importer;
using FTN.ESI.SIMES.CIM.CIMAdapter.Manager;
using FTN.ServiceContracts;

namespace FTN.ESI.SIMES.CIM.CIMAdapter
{
	public class CIMAdapter
	{
        private NetworkModelGDAProxy gdaQueryProxy = null;
       
		public CIMAdapter()
		{
		}

        private NetworkModelGDAProxy GdaQueryProxy
        {
            get
            {
                if (gdaQueryProxy != null)
                {
                    gdaQueryProxy.Abort();
                    gdaQueryProxy = null;
                }

                gdaQueryProxy = new NetworkModelGDAProxy("NetworkModelGDAEndpoint");
                gdaQueryProxy.Open();

                return gdaQueryProxy;
            }
        }

		public Delta CreateDelta(Stream extract, SupportedProfiles extractType, out string log)
		{
			Delta nmsDelta = null;
			ConcreteModel concreteModel = null;
			Assembly assembly = null;
			string loadLog = string.Empty;
			string transformLog = string.Empty;

			if (LoadModelFromExtractFile(extract, extractType, ref concreteModel, ref assembly, out loadLog))
			{
				DoTransformAndLoad(assembly, concreteModel, extractType, out nmsDelta, out transformLog);
			}
			log = string.Concat("Load report:\r\n", loadLog, "\r\nTransform report:\r\n", transformLog);

			return nmsDelta;
		}

		public string ApplyUpdates(Delta delta)
		{
			string updateResult = "Apply Updates Report:\r\n";
			System.Globalization.CultureInfo culture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

			if ((delta != null) && (delta.NumberOfOperations != 0))
			{
				//// NetworkModelService->ApplyUpdates
                updateResult = GdaQueryProxy.ApplyUpdate(delta).ToString();
			}

			Thread.CurrentThread.CurrentCulture = culture;
			return updateResult;
		}


		private bool LoadModelFromExtractFile(Stream extract, SupportedProfiles extractType, ref ConcreteModel concreteModelResult, ref Assembly assembly, out string log)
		{
			bool valid = false;
			log = string.Empty;

			System.Globalization.CultureInfo culture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
			try
			{
				LogManager.Log("DEBUG: Starting LoadModelFromExtractFile...", LogLevel.Info);
				LogManager.Log($"DEBUG: extractType = {extractType}", LogLevel.Info);
				
				ProfileManager.LoadAssembly(extractType, out assembly);
				if (assembly != null)
				{
					LogManager.Log("DEBUG: Assembly loaded successfully", LogLevel.Info);
					
					CIMModel cimModel = new CIMModel();
					LogManager.Log("DEBUG: CIMModel created", LogLevel.Info);
					
					CIMModelLoaderResult modelLoadResult = CIMModelLoader.LoadCIMXMLModel(extract, ProfileManager.Namespace, out cimModel);
                    LogManager.Log($"DEBUG: CIMModelLoader result - Success: {modelLoadResult.Success}, ObjectCount: {cimModel?.CountObjectsInModelMap ?? 0}", LogLevel.Info);
                    LogManager.Log($"DEBUG: ModelLoadResult report: {modelLoadResult.Report}", LogLevel.Info);
					
					if (modelLoadResult.Success)
					{
						concreteModelResult = new ConcreteModel();
						LogManager.Log("DEBUG: ConcreteModel created", LogLevel.Info);
						
						ConcreteModelBuilder builder = new ConcreteModelBuilder();
						ConcreteModelBuildingResult modelBuildResult = builder.GenerateModel(cimModel, assembly, ProfileManager.Namespace, ref concreteModelResult);
						LogManager.Log($"DEBUG: ConcreteModelBuilder result - Success: {modelBuildResult.Success}, ModelMap count: {concreteModelResult?.ModelMap?.Count ?? 0}", LogLevel.Info);
						LogManager.Log($"DEBUG: ModelBuildResult report: {modelBuildResult.Report}", LogLevel.Info);

						if (modelBuildResult.Success)
						{
							valid = true;
						}
						log = modelBuildResult.Report.ToString();
					}
					else
					{
						LogManager.Log($"DEBUG: CIMModelLoader FAILED - {modelLoadResult.Report}", LogLevel.Info);
						log = modelLoadResult.Report.ToString();
					}
				}
				else
				{
					LogManager.Log("DEBUG: Assembly je NULL!", LogLevel.Info);
				}
			}
			catch (Exception e)
			{
				LogManager.Log($"DEBUG: Exception - {e.Message}\n{e.StackTrace}", LogLevel.Info);
				log = e.Message;
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = culture;
			}
			return valid;
		}

		private bool DoTransformAndLoad(Assembly assembly, ConcreteModel concreteModel, SupportedProfiles extractType, out Delta nmsDelta, out string log)
		{
			nmsDelta = null;
			log = string.Empty;
			bool success = false;
			try
			{
				LogManager.Log(string.Format("Importing {0} data...", extractType), LogLevel.Info);

				switch (extractType)
				{
					//case SupportedProfiles.PowerTransformer:
					//	{
					//		// transformation to DMS delta					
					//		TransformAndLoadReport report = PowerTransformerImporter.Instance.CreateNMSDelta(concreteModel);

					//		if (report.Success)
					//		{
					//			nmsDelta = PowerTransformerImporter.Instance.NMSDelta;
					//			success = true;
					//		}
					//		else
					//		{
					//			success = false;
					//		}
					//		log = report.Report.ToString();
					//		PowerTransformerImporter.Instance.Reset();

					//		break;
					//	}
			case SupportedProfiles.SwitchingModel:
				{
					TransformAndLoadReport report = SwitchingImporter.Instance.CreateNMSDelta(concreteModel);

					if (report.Success)
					{
						nmsDelta = SwitchingImporter.Instance.NMSDelta;
						success = true;
					}
					else
					{
						success = false;
					}
					log = report.Report.ToString();
					SwitchingImporter.Instance.Reset();

					break;
				}
			default:
				{
					LogManager.Log(string.Format("Import of {0} data is NOT SUPPORTED.", extractType), LogLevel.Warning);
					break;
				}
		    }

		    return success;
	    }
	    catch (Exception ex)
	    {
		    LogManager.Log(string.Format("Import unsuccessful: {0}", ex.StackTrace), LogLevel.Error);
		    return false;
	    }
    }
	}
}
