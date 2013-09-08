using NotMissing.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PlaymoreClient.Util
{
	public class ProcessInjector : IDisposable
	{
		private readonly byte[] connectcc = new byte[] { 85, 139, 236, 96, 139, 69, 12, 102, 131, 56, 2, 117, 18, 185, 8, 51, 0, 0, 102, 57, 72, 2, 117, 7, 199, 64, 4, 127, 0, 0, 1, 97, 233, 0, 0, 0, 0 };

		private readonly byte[] safecheck = new byte[] { 139, 255, 85, 139, 236 };

		private bool isinjected;

		public Thread CheckThread
		{
			get;
			protected set;
		}

		public Process lolProcess
		{
			get;
			protected set;
		}

		private ProcessInjector.GetModuleFrom From
		{
			get;
			set;
		}

		public bool IsInjected
		{
			get
			{
				return this.isinjected;
			}
			protected set
			{
				if (this.isinjected != value)
				{
					this.isinjected = value;
					if (this.Injected != null)
					{
						this.Injected(this, new EventArgs());
					}
				}
			}
		}

		public string ProcessName
		{
			get;
			protected set;
		}

		public ProcessInjector(string process)
		{
			this.ProcessName = process;
			Thread thread = new Thread(new ThreadStart(this.CheckLoop));
			thread.IsBackground = true;
			this.CheckThread = thread;
			this.From = ProcessInjector.GetModuleFrom.ProcessClass;//-changed- from toolhelpsnapshot
		}

		protected void CheckLoop()
		{
			while (this.CheckThread != null)
			{
				if (this.lolProcess == null || this.lolProcess.HasExited)
				{
					this.IsInjected = false;
					this.lolProcess = Process.GetProcessesByName(this.ProcessName).FirstOrDefault<Process>();
					if (this.lolProcess != null)
					{
						try
						{
							this.Inject();
							this.IsInjected = true;
						}
						catch (FileNotFoundException fileNotFoundException)
						{
							PlaymoreClient.Gui.MainForm.LOGGER.Info(fileNotFoundException.Message);
							this.lolProcess = null;
							Thread.Sleep(1000);
							continue;
						}
						catch (WarningException warningException1)
						{
							WarningException warningException = warningException1;
							this.IsInjected = true;
							PlaymoreClient.Gui.MainForm.LOGGER.Info(warningException.Message);
						}
						catch (NotSupportedException notSupportedException)
						{
							PlaymoreClient.Gui.MainForm.LOGGER.Warn(notSupportedException);
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							PlaymoreClient.Gui.MainForm.LOGGER.Error(new Exception(string.Format("{0} [{1}]", exception.Message, this.From), exception));
						}
					}
				}
				Thread.Sleep(500);
			}
		}

		public void Clear()
		{
			this.lolProcess = null;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool dispose)
		{
			if (dispose)
			{
				this.CheckThread = null;
			}
		}

		~ProcessInjector()
		{
			this.Dispose(false);
		}

		private ProcessModule GetModule(ProcessModuleCollection mods, string name)
		{
			ProcessModule processModule;
			name = name.ToLower();
			IEnumerator enumerator = mods.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ProcessModule current = (ProcessModule)enumerator.Current;
					if (current.ModuleName.ToLower() != name)
					{
						continue;
					}
					processModule = current;
					return processModule;
				}
				return null;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return processModule;
		}

		private int GetModuleAddress(Process curproc, ProcessMemory curmem, string name)
		{
			if (this.From == ProcessInjector.GetModuleFrom.ProcessClass)
			{
				ProcessModule module = this.GetModule(curproc.Modules, name);
				if (module == null)
				{
					return 0;
				}
				return module.BaseAddress.ToInt32();
			}
			if (this.From == ProcessInjector.GetModuleFrom.Mirroring)
			{
				int num = ProcessMemory.GetModule("ws2_32.dll");
				if (curmem.VirtualQuery(num).State == ProcessMemory.MemoryState.Free)
				{
					return 0;
				}
				return num;
			}
			if (this.From != ProcessInjector.GetModuleFrom.Toolhelp32Snapshot)
			{
				return -1;
			}
			ProcessMemory.ModuleInfo moduleInfo = curmem.GetModuleInfos().FirstOrDefault<ProcessMemory.ModuleInfo>((ProcessMemory.ModuleInfo mi) => mi.baseName.ToLowerInvariant() == name);
			if (moduleInfo == null)
			{
				return 0;
			}
			return moduleInfo.baseOfDll.ToInt32();
		}

		private void Inject()
		{
			using (ProcessMemory lolProcessMemory = new ProcessMemory(this.lolProcess.Id))
			{
				using (ProcessMemory thisProcessMemory = new ProcessMemory(Process.GetCurrentProcess().Id))
				{
					if (lolProcessMemory.Is64Bit())
					{
						throw new NotSupportedException("lolclient is running in 64bit mode which is not supported");
					}
					byte[] numArray = new byte[(int)this.connectcc.Length];
					this.connectcc.CopyTo(numArray, 0);
					int length = (int)numArray.Length - 4;
					int module = ProcessMemory.GetModule("ws2_32.dll");
					int address = thisProcessMemory.GetAddress(module, "connect");
					address = address - module;
					int moduleAddress = this.GetModuleAddress(this.lolProcess, lolProcessMemory, "ws2_32.dll");
					if (moduleAddress == 0)
					{
						throw new FileNotFoundException("Lolclient has not yet loaded ws2_32.dll");
					}
					int num = moduleAddress + address;
					byte[] numArray1 = lolProcessMemory.Read(num, 5);
					if (numArray1[0] == 233)
					{
						throw new WarningException("Connect already redirected");
					}
					if (!numArray1.SequenceEqual<byte>(this.safecheck))
					{
						numArray1 = lolProcessMemory.Read(num, 20);
						throw new AccessViolationException(string.Format("Connect has unknown bytes [{0},{1}]", Convert.ToBase64String(numArray1), this.From));
					}
					int num1 = lolProcessMemory.Alloc((int)this.connectcc.Length);
					BitConverter.GetBytes(num + 5 - (num1 + (int)numArray.Length)).CopyTo(numArray, length);
					lolProcessMemory.Write(num1, numArray);
					byte[] numArray2 = new byte[] { 233, 0, 0, 0, 0 };
					BitConverter.GetBytes(num1 - (num + 5)).CopyTo(numArray2, 1);
					lolProcessMemory.Write(num, numArray2);
				}
			}
		}

		public void Start()
		{
			if (!this.CheckThread.IsAlive)
			{
				this.CheckThread.Start();
			}
		}

		public event EventHandler Injected;

		public enum GetModuleFrom
		{
			Toolhelp32Snapshot,
			ProcessClass,
			Mirroring
		}
	}
}