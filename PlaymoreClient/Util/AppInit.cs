using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PlaymoreClient.Util
{
	public static class AppInit
	{
		private const string AppInitDef = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Windows";

		private const string AppInit32On64 = "SOFTWARE\\Wow6432Node\\Microsoft\\Windows NT\\CurrentVersion\\Windows";

		public static List<string> AppInitDlls32
		{
			get
			{
				return AppInit.GetAppInitDlls((Wow.Is64BitOperatingSystem ? "SOFTWARE\\Wow6432Node\\Microsoft\\Windows NT\\CurrentVersion\\Windows" : "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Windows"));
			}
			set
			{
				AppInit.SetAppInitDlls((Wow.Is64BitOperatingSystem ? "SOFTWARE\\Wow6432Node\\Microsoft\\Windows NT\\CurrentVersion\\Windows" : "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Windows"), value);
			}
		}

		public static List<string> GetAppInitDlls(string path)
		{
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(path);
			if (registryKey == null)
			{
				throw new NullReferenceException("AppInit key null");
			}
			string value = (string)registryKey.GetValue("AppInit_DLLs");
			if (value == null)
			{
				return new List<string>();
			}
			char[] chrArray = new char[] { ';', ' ' };
			return (
				from s in value.Split(chrArray)
				select s.Trim() into s
				where s != ""
				select s).ToList<string>();
		}

		public static string GetShortPath(string path)
		{
			char[] chrArray = new char[256];
			int shortPathName = AppInit.GetShortPathName(path, chrArray, (int)chrArray.Length);
			return new string(chrArray, 0, shortPathName);
		}

		[DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=false, SetLastError=true)]
		private static extern int GetShortPathName(string lpszLongPath, char[] lpszShortPath, int cchBuffer);

		public static void SetAppInitDlls(string path, List<string> dlls)
		{
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(path, true);
			if (registryKey == null)
			{
				throw new NullReferenceException("AppInit key null");
			}
			registryKey.SetValue("AppInit_DLLs", string.Join("; ", dlls.ToArray()));
			registryKey.SetValue("LoadAppInit_DLLs", 1);
		}
	}
}