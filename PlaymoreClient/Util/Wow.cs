using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace PlaymoreClient.Util
{
	public static class Wow
	{
		public static bool Is64BitOperatingSystem
		{
			get
			{
				bool flag;
				if (Wow.Is64BitProcess)
				{
					return true;
				}
				if (Wow.ModuleContainsFunction("kernel32.dll", "IsWow64Process") && Wow.IsWow64Process(Wow.GetCurrentProcess(), out flag))
				{
					return flag;
				}
				return false;
			}
		}

		public static bool Is64BitProcess
		{
			get
			{
				return IntPtr.Size == 8;
			}
		}

		public static bool IsAdministrator
		{
			get
			{
				return (new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator);
			}
		}

		[DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr GetCurrentProcess();

		[DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		private static extern IntPtr GetModuleHandle(string moduleName);

		[DllImport("kernel32.dll", CharSet=CharSet.Ansi, ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string methodName);

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool IsWow64Process(IntPtr hProcess, out bool isWow64);

		private static bool ModuleContainsFunction(string moduleName, string methodName)
		{
			IntPtr moduleHandle = Wow.GetModuleHandle(moduleName);
			if (moduleHandle == IntPtr.Zero)
			{
				return false;
			}
			return Wow.GetProcAddress(moduleHandle, methodName) != IntPtr.Zero;
		}
	}
}