using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Threading;

namespace PlaymoreClient.Util
{
	public class ProcessMemory : IDisposable
	{
		private const uint PROCESS_ALL_ACCESS = 2097151;

		public IntPtr Handle
		{
			get;
			protected set;
		}

		public int ProcessId
		{
			get;
			protected set;
		}

		public ProcessMemory(int id)
		{
			this.ProcessId = id;
			this.Handle = ProcessMemory.OpenProcess(2097151, 0, id);
			if (this.Handle == IntPtr.Zero)
			{
				throw new Win32Exception();
			}
		}

		public int Alloc(int len)
		{
			int num = ProcessMemory.VirtualAllocEx(this.Handle, 0, len, ProcessMemory.AllocationType.Commit | ProcessMemory.AllocationType.Reserve, ProcessMemory.MemoryProtection.ExecuteReadWrite);
			if (num == 0)
			{
				throw new Win32Exception();
			}
			return num;
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=true, SetLastError=true)]
		private static extern bool CloseHandle(HandleRef handle);

		[DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=false, SetLastError=true)]
		private static extern ProcessMemory.ToolHelpHandle CreateToolhelp32Snapshot(int flags, int processId);

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool dispose)
		{
			if (this.Handle != IntPtr.Zero)
			{
				ProcessMemory.CloseHandle(this.Handle);
				this.Handle = IntPtr.Zero;
			}
		}

		[DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=false, SetLastError=true)]
		private static extern bool DuplicateHandle([In] IntPtr hSourceProcessHandle, [In] IntPtr hSourceHandle, [In] IntPtr hTargetProcessHandle, [In][Out] ref IntPtr lpTargetHandle, [In] uint dwDesiredAccess, [In] bool bInheritHandle, [In] uint dwOptions);

		public void DuplicateMutex(Mutex mutex)
		{
			IntPtr zero = IntPtr.Zero;
			if (!ProcessMemory.DuplicateHandle(ProcessMemory.GetCurrentProcess(), mutex.SafeWaitHandle.DangerousGetHandle(), this.Handle, ref zero, 0, false, 2))
			{
				throw new Win32Exception();
			}
		}

		~ProcessMemory()
		{
			this.Dispose(false);
		}

		public int GetAddress(int mod, string name)
		{
			int procAddress = ProcessMemory.GetProcAddress(mod, name);
			if (procAddress == 0)
			{
				throw new Win32Exception();
			}
			return procAddress;
		}

		public int GetAddress(string modname, string name)
		{
			return this.GetAddress(ProcessMemory.GetModule(modname), name);
		}

		[DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr GetCurrentProcess();

		public static int GetModule(string modname)
		{
			int moduleHandle = ProcessMemory.GetModuleHandle(modname);
			if (moduleHandle == 0)
			{
				throw new Win32Exception();
			}
			return moduleHandle;
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern int GetModuleHandle(string lpModuleName);

        public IEnumerable<ProcessMemory.ModuleInfo> GetModuleInfos()
        {
            ProcessMemory.ToolHelpHandle hSnapshot = (ProcessMemory.ToolHelpHandle)null;
            List<ProcessMemory.ModuleInfo> list = new List<ProcessMemory.ModuleInfo>();
            try
            {
                hSnapshot = ProcessMemory.CreateToolhelp32Snapshot(8, this.ProcessId);
                if (hSnapshot.IsInvalid)
                    throw new Win32Exception();
                ProcessMemory.MODULEENTRY32 lpme = new ProcessMemory.MODULEENTRY32();
                lpme.dwSize = (uint)Marshal.SizeOf((object)lpme);
                if (ProcessMemory.Module32First(hSnapshot, ref lpme))
                {
                    do
                    {
                        ProcessMemory.ModuleInfo moduleInfo = new ProcessMemory.ModuleInfo()
                        {
                            baseName = lpme.szModule,
                            fileName = lpme.szExePath,
                            baseOfDll = lpme.modBaseAddr,
                            sizeOfImage = (int)lpme.modBaseSize,
                            Id = (int)lpme.th32ModuleID
                        };
                        list.Add(moduleInfo);
                        lpme.dwSize = (uint)Marshal.SizeOf((object)lpme);
                    }
                    while (ProcessMemory.Module32Next(hSnapshot, ref lpme));
                }
                if (Marshal.GetLastWin32Error() != 18)
                    throw new Win32Exception();
            }
            finally
            {
                if (hSnapshot != null && !hSnapshot.IsInvalid)
                    hSnapshot.Close();
            }
            return (IEnumerable<ProcessMemory.ModuleInfo>)list;
        }

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern int GetProcAddress(int hModule, string procedureName);

		public bool Is64Bit()
		{
			bool flag;
			if ((Environment.OSVersion.Version.Major != 5 || Environment.OSVersion.Version.Minor < 1) && Environment.OSVersion.Version.Major <= 5 || !Wow.Is64BitOperatingSystem)
			{
				return false;
			}
			if (!ProcessMemory.IsWow64Process(this.Handle, out flag))
			{
				throw new Win32Exception();
			}
			return !flag;
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool IsWow64Process(IntPtr processHandle, out bool wow64Process);

		[DllImport("kernel32.dll", CharSet=CharSet.None, EntryPoint="LoadLibraryA", ExactSpelling=false, SetLastError=true)]
		private static extern int LoadLibrary(string dllToLoad);

		public int LoadModule(string name)
		{
			int num = ProcessMemory.LoadLibrary(name);
			if (num == 0)
			{
				throw new Win32Exception();
			}
			return num;
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool Module32First(ProcessMemory.ToolHelpHandle hSnapshot, ref ProcessMemory.MODULEENTRY32 lpme);

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool Module32Next(ProcessMemory.ToolHelpHandle hSnapshot, ref ProcessMemory.MODULEENTRY32 lpme);

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, int dwProcessId);

		public byte[] Read(int addr, int len)
		{
			IntPtr intPtr;
			byte[] numArray = new byte[len];
			if (!ProcessMemory.ReadProcessMemory(this.Handle, (IntPtr)addr, numArray, (int)numArray.Length, out intPtr))
			{
				throw new Win32Exception();
			}
			return numArray;
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, int size, out IntPtr lpNumberOfBytesRead);

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=true, SetLastError=true)]
		private static extern int VirtualAllocEx(IntPtr hProcess, int lpAddress, int dwSize, ProcessMemory.AllocationType flAllocationType, ProcessMemory.MemoryProtection flProtect);

		public ProcessMemory.MEMORY_BASIC_INFORMATION VirtualQuery(int address)
		{
			ProcessMemory.MEMORY_BASIC_INFORMATION mEMORYBASICINFORMATION = new ProcessMemory.MEMORY_BASIC_INFORMATION();
			if (ProcessMemory.VirtualQueryEx(this.Handle, address, ref mEMORYBASICINFORMATION, Marshal.SizeOf(typeof(ProcessMemory.MEMORY_BASIC_INFORMATION))) == 0)
			{
				throw new Win32Exception();
			}
			return mEMORYBASICINFORMATION;
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern int VirtualQueryEx(IntPtr handle, int address, ref ProcessMemory.MEMORY_BASIC_INFORMATION buffer, int sizeOfBuffer);

		public void Write(int addr, byte[] bytes)
		{
			IntPtr intPtr;
			if (!ProcessMemory.WriteProcessMemory(this.Handle, (IntPtr)addr, bytes, (int)bytes.Length, out intPtr))
			{
				throw new Win32Exception();
			}
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);

		[Flags]
		private enum AllocationType
		{
			Commit = 4096,
			Reserve = 8192,
			Decommit = 16384,
			Release = 32768,
			Reset = 524288,
			TopDown = 1048576,
			WriteWatch = 2097152,
			Physical = 4194304,
			LargePages = 536870912
		}

		public struct MEMORY_BASIC_INFORMATION
		{
			public IntPtr BaseAddress;

			public IntPtr AllocationBase;

			public uint AllocationProtect;

			public UIntPtr RegionSize;

			public uint State;

			public uint Protect;

			public uint Type;
		}

		[Flags]
		private enum MemoryProtection
		{
			NoAccess = 1,
			ReadOnly = 2,
			ReadWrite = 4,
			WriteCopy = 8,
			Execute = 16,
			ExecuteRead = 32,
			ExecuteReadWrite = 64,
			ExecuteWriteCopy = 128,
			GuardModifierflag = 256,
			NoCacheModifierflag = 512,
			WriteCombineModifierflag = 1024
		}

		public static class MemoryState
		{
			public static uint Commit;

			public static uint Free;

			public static uint Reserve;

			static MemoryState()
			{
				ProcessMemory.MemoryState.Commit = 4096;
				ProcessMemory.MemoryState.Free = 65536;
				ProcessMemory.MemoryState.Reserve = 8192;
			}
		}

		public struct MODULEENTRY32
		{
			public uint dwSize;

			public uint th32ModuleID;

			public uint th32ProcessID;

			public uint GlblcntUsage;

			public uint ProccntUsage;

			public IntPtr modBaseAddr;

			public uint modBaseSize;

			public IntPtr hModule;

			public string szModule;

			public string szExePath;
		}

		public class ModuleInfo
		{
			public string baseName;

			public IntPtr baseOfDll;

			public IntPtr entryPoint;

			public string fileName;

			public int Id;

			public int sizeOfImage;

			public ModuleInfo()
			{
			}
		}

		public class ToolHelpHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
			private ToolHelpHandle() : base(true)
			{
			}

			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			protected override bool ReleaseHandle()
			{
				return ProcessMemory.CloseHandle(this.handle);
			}
		}
	}
}