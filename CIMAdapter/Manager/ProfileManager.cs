using System;
using System.Reflection;

namespace FTN.ESI.SIMES.CIM.CIMAdapter.Manager
{
	public enum SupportedProfiles : byte
	{
		PowerTransformer = 0,
		VoltageRegulator,
		SwitchingEquipment,
		OverheadLines,
		UndergroundCables,
		ProtectionDevices,
        SwitchingModel = 10
    };
                

	/// <summary>
	/// ProfileManager
	/// </summary>
	public static class ProfileManager
	{
		public const string Namespace = "FTN";

		/// <summary>
		/// Method returns the name of CIM profile based on the defined enumeration.
		/// </summary>
		/// <param name="profile">supported CIM profile</param>
		/// <returns>name of profile + "CIMProfile_Labs"</returns>
		public static string GetProfileName(SupportedProfiles profile)
		{
			return string.Format("{0}CIMProfile_Labs", profile.ToString());
		}

		/// <summary>
		/// Method returns the name of the CIM profile DLL based on the defined enumeration.
		/// </summary>
		/// <param name="profile">supported CIM profile</param>
		/// <returns>name of profile + "CIMProfile_Labs.DLL"</returns>
		public static string GetProfileDLLName(SupportedProfiles profile)
		{
			return string.Format("{0}CIMProfile_Labs.DLL", profile.ToString());
		}

		public static bool LoadAssembly(SupportedProfiles profile, out Assembly assembly)
		{
			try
			{
				string dllPath = string.Format(".\\{0}", ProfileManager.GetProfileDLLName(profile));
				
				// ✅ DEBUG
				Console.WriteLine($"[DEBUG] Pokušaj učitavanja DLL iz: {dllPath}");
				Console.WriteLine($@"[DEBUG] Puni path: {System.IO.Path.GetFullPath(dllPath)}""");
				
				if (!System.IO.File.Exists(dllPath))
				{
					Console.WriteLine($"[ERROR] DLL NIJE PRONAĐEN na putanji: {dllPath}");
					assembly = null;
					return false;
				}
				
				assembly = Assembly.LoadFrom(dllPath);
				
				Console.WriteLine($"[SUCCESS] DLL uspješno učitan!");
				Console.WriteLine($"[INFO] Tip: {profile.ToString()}");
				
				return true;
			}
			catch (Exception e)
			{
				assembly = null;
				Console.WriteLine($"[ERROR] {e.Message}");
				LogManager.Log(string.Format("Error during Assembly load. Profile: {0} ; Message: {1}", 
					profile, e.Message), LogLevel.Error);
				return false;
			}
		}

		public static bool LoadAssembly(string path, out Assembly assembly)
		{
			try
			{
				assembly = Assembly.LoadFrom(path);
			}
			catch (Exception e)
			{
				assembly = null;
				LogManager.Log(string.Format("Error during Assembly load. Path: {0} ; Message: {1}", path, e.Message), LogLevel.Error);
				return false;
			}
			return true;
		}
	}
}
