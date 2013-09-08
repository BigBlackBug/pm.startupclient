using Newtonsoft.Json;
using NotMissing.Logging;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace PlaymoreClient.Gui
{
	public class MainSettings
	{
		protected static MainSettings _instance;

        private readonly object settingslock = new object();

		private LeagueRegion _region;

		private string _moduleresolver;

		public static MainSettings Instance
		{
			get
			{
				MainSettings mainSetting = MainSettings._instance;
				if (mainSetting == null)
				{
					mainSetting = new MainSettings();
					MainSettings._instance = mainSetting;
				}
				return mainSetting;
			}
		}

		public string ModuleResolver
		{
			get
			{
				return this._moduleresolver;
			}
			set
			{
				this._moduleresolver = value;
				this.OnPropertyChanged("ModuleResolver");
			}
		}

		public LeagueRegion Region
		{
			get
			{
				return this._region;
			}
			set
			{
				this._region = value;
				this.OnPropertyChanged("Region");
			}
		}

		public MainSettings()
		{
			this._region = LeagueRegion.NA;
			this._moduleresolver = "";
		}

		public void Load(string file)
		{
            lock (this.settingslock){
			try
			{
				if (File.Exists(file))
				{
					using (StreamReader streamReader = new StreamReader(File.Open(file, FileMode.Open, FileAccess.Read)))
					{
						JsonConvert.PopulateObject(streamReader.ReadToEnd(), this);
					}
					this.OnLoad();
				}
			}
			catch (IOException oException)
			{
				PlaymoreClient.Gui.MainForm.LOGGER.Debug(oException);
			}
            }
		}

		protected void OnLoad()
		{
			if (this.Loaded != null)
			{
				this.Loaded(this, new EventArgs());
			}
		}

		protected void OnPropertyChanged(string name)
		{
            //PropertyChangedEventHandler propertyChangedEventHandler = this.PropertyChanged;
            //if (propertyChangedEventHandler != null)
            //{
			    //	propertyChangedEventHandler(this, new PropertyChangedEventArgs(name));
                Save("settings.json");
            //}
		}

		public bool Save(string file)
		{
            
            Console.WriteLine("saving settings");
			bool flag;
            lock (this.settingslock){
			try
			{
				using (StreamWriter streamWriter = new StreamWriter(File.Open(file, FileMode.Create, FileAccess.Write)))
				{
					streamWriter.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
					flag = true;
				}
			}
			catch (IOException oException)
			{
				PlaymoreClient.Gui.MainForm.LOGGER.Debug(oException);
				flag = false;
			}
            }
			return flag;
		}

		public event EventHandler Loaded;

		public event PropertyChangedEventHandler PropertyChanged;
	}
}