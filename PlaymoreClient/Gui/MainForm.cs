using PlaymoreClient.DTO;
using PlaymoreClient.Messages.Account;
using PlaymoreClient.Messages.Champion;
using PlaymoreClient.Messages.GameLobby;
using PlaymoreClient.Messages.GameLobby.Participants;
using PlaymoreClient.Messages.GameStats;
using PlaymoreClient.Messages.Readers;
using PlaymoreClient.Messaging;
using PlaymoreClient.Properties;
using PlaymoreClient.Proxy;
using PlaymoreClient.Util;
using FluorineFx;
using FluorineFx.Messaging.Messages;
using FluorineFx.Messaging.Rtmp.Event;
using Microsoft.Win32;
using NotMissing.Logging;
using ServiceStack.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.Net.Sockets;
using System.Text;

namespace PlaymoreClient.Gui
{
	public class MainForm : Form
	{
        public static log4net.ILog LOGGER = log4net.LogManager.GetLogger("playmore_logger");
        public static log4net.ILog UPDATE_LOGGER = log4net.LogManager.GetLogger("game_update_logger");
		private const string SettingsFile = "settings.json";

		private const string LogFile = "Log.txt";

		private IContainer components;

		private System.Windows.Forms.ContextMenuStrip PlayerEditStrip;

		private ToolStripMenuItem editToolStripMenuItem;

		private ToolStripMenuItem clearToolStripMenuItem;

		private System.Windows.Forms.ContextMenuStrip CallEditStrip;

		private ToolStripMenuItem clearToolStripMenuItem1;

		private ToolStripMenuItem dumpToolStripMenuItem;

		private GroupBox ModuleGroupBox;

		private RadioButton MirrorRadio;

		private RadioButton ToolHelpRadio;

		private RadioButton ProcessRadio;

		private ComboBox RegionList;

		private Label RegionLabel;

		private System.Windows.Forms.StatusStrip StatusStrip;

		private ToolStripStatusLabel StatusLabel;

		private Label label5;

		private Button OpenTeamStats;

		private Label VersionLabel;

		private Button SaveOwnedSkins;

		private PictureBox pictureBox1;

		private Label StepsLabel;

		private Label StepTwoLabel;

		private Label label1;

		private Label label2;

		private Label label3;

		private LinkLabel FurtherHelpLink;

		public readonly static string Version;

		private readonly Dictionary<LeagueRegion, CertificateHolder> Certificates;

		private readonly Dictionary<ProcessInjector.GetModuleFrom, RadioButton> ModuleResolvers;

		private readonly ProcessQueue<string> TrackingQueue = new ProcessQueue<string>();

        private readonly ProcessMonitor Launcher = new ProcessMonitor(new string[] { "LoLLauncher" });
		private RtmpsProxyHost Connection;

		private MessageReader Reader;

		private CertificateInstaller Installer;

		private ProcessInjector Injector;

		private GameDTO CurrentGame;

		private List<ChampionDTO> Champions;

		private SummonerData SelfSummoner;

		private Dictionary<LeagueRegion, string> RegionsFullText;

		private List<string> Teammates;

		private List<string> EnemyTeam;

		private List<int> EnemyChampionIds;

		private HashSet<int> Bans;

		private HashSet<int> LockedInChampions;

		private HashSet<string> LockedInSummoners;

		private string InstanceId;

		private int TeamId;

		private readonly object settingslock = new object();

		private readonly object LogLock = new object();

		private MainSettings Settings
		{
			get
			{
				return MainSettings.Instance;
			}
		}

		static MainForm()
		{
			MainForm.Version = string.Concat(AssemblyAttributes.FileVersion, AssemblyAttributes.Configuration);
		}

		public MainForm()
		{
           this.InitializeComponent();
			Logger.Instance.Register(new DefaultListener(Levels.All, new LogHandler(this.OnLog)));
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);
			Application.ThreadException += new ThreadExceptionEventHandler(this.Application_ThreadException);
			PlaymoreClient.Gui.MainForm.LOGGER.Info(string.Format("Version {0}", MainForm.Version));
			this.Settings.Load("settings.json");
			Dictionary<LeagueRegion, CertificateHolder> leagueRegions = new Dictionary<LeagueRegion, CertificateHolder>();
			leagueRegions.Add(LeagueRegion.NA, new CertificateHolder("prod.na1.lol.riotgames.com", Resources.prod_na1_lol_riotgames_com));
			leagueRegions.Add(LeagueRegion.EUW, new CertificateHolder("prod.eu.lol.riotgames.com", Resources.prod_eu_lol_riotgames_com));
			leagueRegions.Add(LeagueRegion.EUNE, new CertificateHolder("prod.eun1.lol.riotgames.com", Resources.prod_eun1_lol_riotgames_com));
			this.Certificates = leagueRegions;
            //Dictionary<ProcessInjector.GetModuleFrom, RadioButton> getModuleFroms = new Dictionary<ProcessInjector.GetModuleFrom, RadioButton>();
            //getModuleFroms.Add(ProcessInjector.GetModuleFrom.Toolhelp32Snapshot, this.ToolHelpRadio);
            //getModuleFroms.Add(ProcessInjector.GetModuleFrom.ProcessClass, this.ProcessRadio);
            //getModuleFroms.Add(ProcessInjector.GetModuleFrom.Mirroring, this.MirrorRadio);
            //this.ModuleResolvers = getModuleFroms;
            //foreach (KeyValuePair<ProcessInjector.GetModuleFrom, RadioButton> moduleResolver in this.ModuleResolvers)
            //{
            //    moduleResolver.Value.Click += new EventHandler(this.moduleresolvers_Click);
            //}
			Dictionary<LeagueRegion, string> leagueRegions1 = new Dictionary<LeagueRegion, string>();
			leagueRegions1.Add(LeagueRegion.NA, "North America");
			leagueRegions1.Add(LeagueRegion.EUW, "Europe West");
			leagueRegions1.Add(LeagueRegion.EUNE, "Europe Nordic & East");
			this.RegionsFullText = leagueRegions1;
			KeyValuePair<LeagueRegion, CertificateHolder> keyValuePair = this.Certificates.FirstOrDefault<KeyValuePair<LeagueRegion, CertificateHolder>>((KeyValuePair<LeagueRegion, CertificateHolder> kv) => kv.Key == this.Settings.Region);
			CertificateHolder value = keyValuePair.Value ?? this.Certificates.First<KeyValuePair<LeagueRegion, CertificateHolder>>().Value;
			this.Injector = new ProcessInjector("lolclient");
			this.Connection = new RtmpsProxyHost(2099, value.Domain, 2099, value.Certificate);
			this.Reader = new MessageReader(this.Connection);
			this.Connection.Connected += new EventHandler(this.Connection_Connected);
			this.Injector.Injected += new EventHandler(this.Injector_Injected);
			this.Reader.ObjectRead += new ObjectReadD(this.Reader_ObjectRead);
			this.Connection.CallResult += new CallHandler(this.Connection_Call);
			this.Connection.Notify += new NotifyHandler(this.Connection_Notify);
			foreach (KeyValuePair<LeagueRegion, CertificateHolder> certificate in this.Certificates)
			{
				this.RegionList.Items.Add(this.RegionsFullText[certificate.Key]);
			}
			int num = this.RegionList.Items.IndexOf(this.RegionsFullText[this.Settings.Region]);
			this.RegionList.SelectedIndex = (num != -1 ? num : 0);
			this.Installer = new CertificateInstaller((
				from c in this.Certificates
				select c.Value.Certificate).ToArray<X509Certificate2>());
			this.TrackingQueue.Process += new EventHandler<ProcessQueueEventArgs<string>>(this.TrackingQueue_Process);
            this.Launcher.ProcessFound += new EventHandler<ProcessMonitor.ProcessEventArgs>(this.launcher_ProcessFound);
            
			try
			{
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4");
				if (registryKey == null || registryKey != null && !registryKey.GetSubKeyNames().Contains<string>("Full"))
				{
					if (MessageBox.Show("The Elophant Client requires the .NET Framework 4.0 Full version. Would you like to download it?", ".NET Framework 4.0 Full Not Found", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
					{
						Process.Start("http://www.microsoft.com/en-us/download/details.aspx?id=17718");
					}
					MessageBox.Show("The Elophant Client will now close.");
					Process.GetCurrentProcess().Kill();
					return;
				}
			}
			catch (Exception exception)
			{
				PlaymoreClient.Gui.MainForm.LOGGER.Error(exception.ToString());
				MessageBox.Show("An unknown exception has occurred. Check the log for more information.");
				Process.GetCurrentProcess().Kill();
				return;
			}
			try
			{
				string str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "lolbans", "LoLLoader.dll");
				if (File.Exists(str))
				{
					PlaymoreClient.Gui.MainForm.LOGGER.Info("Uninstalling old loader.");
					string shortPath = AppInit.GetShortPath(str);
					List<string> appInitDlls32 = AppInit.AppInitDlls32;
					if (appInitDlls32.Contains(shortPath))
					{
						appInitDlls32.Remove(AppInit.GetShortPath(shortPath));
						AppInit.AppInitDlls32 = appInitDlls32;
					}
					if (File.Exists(str))
					{
						File.Delete(str);
					}
				}
			}
			catch (SecurityException securityException)
			{
				PlaymoreClient.Gui.MainForm.LOGGER.Warn(securityException);
			}
			catch (Exception exception1)
			{
				PlaymoreClient.Gui.MainForm.LOGGER.Error(string.Concat("Failed to uninstall. Message: ", exception1));
			}
			try
			{
				if (!this.Installer.IsInstalled)
				{
					if (Wow.IsAdministrator)
					{
						try
						{
							this.Installer.Install();
							MessageBox.Show(string.Concat("The Elophant Client was successfully installed!", Environment.NewLine, "Please relaunch without administrator privileges."), "Install Complete", MessageBoxButtons.OK);
							Process.GetCurrentProcess().Kill();
							return;
						}
						catch (UnauthorizedAccessException unauthorizedAccessException1)
						{
							UnauthorizedAccessException unauthorizedAccessException = unauthorizedAccessException1;
							MessageBox.Show("Unable to fully install. Please make sure that the LoL Client is not running.", "Unable to Install", MessageBoxButtons.OK);
							PlaymoreClient.Gui.MainForm.LOGGER.Warn(unauthorizedAccessException);
						}
						this.UpdateStatus();
					}
					else
					{
						MessageBox.Show(string.Concat("Please run the Elophant Client once as the administrator to install it.", Environment.NewLine, "Right click ElophantClient.exe, then click \"Run as administrator.\""), "Admin Privileges Required", MessageBoxButtons.OK);
						Process.GetCurrentProcess().Kill();
						return;
					}
				}
			}
			catch
			{
			}
			PlaymoreClient.Gui.MainForm.LOGGER.Info("Startup Completed");
		}

        //private void AddMissingTypeNames(object obj)
        //{
        //    if (obj == null)
        //    {
        //        return;
        //    }
        //    if (obj is ASObject)
        //    {
        //        ASObject typeName = (ASObject)obj;
        //        typeName["TypeName"] = typeName.TypeName;
        //        foreach (KeyValuePair<string, object> keyValuePair in typeName)
        //        {
        //            this.AddMissingTypeNames(keyValuePair.Value);
        //        }
        //    }
        //    else if (obj is IList)
        //    {
        //        foreach (object obj1 in (IList)obj)
        //        {
        //            this.AddMissingTypeNames(obj1);
        //        }
        //    }
        //}

		private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			this.LogException(e.Exception, true);
		}

        //not actually used for anything but logging
		private static string CallArgToString(object arg)
		{
			if (arg is RemotingMessage)
			{
				return ((RemotingMessage)arg).operation;
			}
			if (arg is DSK)
			{
				ASObject body = ((DSK)arg).Body as ASObject;
				if (body != null)
				{
					return body.TypeName;
				}
			}
			if (!(arg is FluorineFx.Messaging.Messages.CommandMessage))
			{
				return arg.ToString();
			}
			return FluorineFx.Messaging.Messages.CommandMessage.OperationToString(((FluorineFx.Messaging.Messages.CommandMessage)arg).operation);
		}

		private void Connection_Call(object sender, Notify call, Notify result)
		{

            PlaymoreClient.Gui.MainForm.LOGGER.Info("CALLING METHOD "+call.ServiceCall.ServiceMethodName+" SERVICE " + call.ServiceCall.ServiceName);
            PlaymoreClient.Gui.MainForm.LOGGER.Info("call arguments "+string.Join(", ", call.ServiceCall.Arguments.Select<object, string>(new Func<object, string>(MainForm.CallArgToString))));
            PlaymoreClient.Gui.MainForm.LOGGER.Info("result arguments " + string.Join(", ", result.ServiceCall.Arguments.Select<object, string>(new Func<object, string>(MainForm.CallArgToString))));

            //PlaymoreClient.Gui.MainForm.LOGGER.Info(string.Format("Call {0} ({1}), Return ({2})", call.ServiceCall.ServiceMethodName, string.Join(", ", call.ServiceCall.Arguments.Select<object, string>(new Func<object, string>(MainForm.CallArgToString))), string.Join(", ", result.ServiceCall.Arguments.Select<object, string>(new Func<object, string>(MainForm.CallArgToString)))));
		}

		private void Connection_Connected(object sender, EventArgs e)
		{
            PlaymoreClient.Gui.MainForm.LOGGER.Info("connection connected");
			if (base.Created)
			{
				base.BeginInvoke(new Action(this.UpdateStatus));
			}
		}

		private void Connection_Notify(object sender, Notify notify)
		{
			PlaymoreClient.Gui.MainForm.LOGGER.Info(string.Format("NOTIFIED {0}({1})", (!string.IsNullOrEmpty(notify.ServiceCall.ServiceMethodName) ? string.Concat(notify.ServiceCall.ServiceMethodName, " ") : ""), string.Join(", ", notify.ServiceCall.Arguments.Select<object, string>(new Func<object, string>(MainForm.CallArgToString)))));
		}

        //private void CreateTeamStatisticsInstanceCompleted(object sender, UploadStringCompletedEventArgs e)
        //{
        //    try
        //    {
        //        StringResponse stringResponse = JsonSerializer.DeserializeFromString<StringResponse>(e.Result);
        //        if (!stringResponse.success)
        //        {
        //            PlaymoreClient.Gui.MainForm.LOGGER.Error(stringResponse.error);
        //            MessageBox.Show("An error occurred while saving the team's stats. Check the log file for more info.", "Team Stats Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
        //        }
        //        else
        //        {
        //            PlaymoreClient.Gui.MainForm.LOGGER.Info(string.Concat("Team stats ID: ", stringResponse.data));
        //            this.InstanceId = stringResponse.data;
        //            this.OpenTeamStats.Enabled = true;
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        PlaymoreClient.Gui.MainForm.LOGGER.Error(exception.ToString());
        //        MessageBox.Show("An unknown error occurred while saving the team's stats. Check the log file for more info.", "Unknown Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
        //    }
        //}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			this.LogException((Exception)e.ExceptionObject, !e.IsTerminating);
			if (e.IsTerminating)
			{
				ProcessQueueEventArgs<string> processQueueEventArg = new ProcessQueueEventArgs<string>();
				processQueueEventArg.Item = string.Format("error/{0}", Parse.ToBase64(e.ExceptionObject.ToString()));
				this.TrackingQueue_Process(this, processQueueEventArg);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.Connection != null)
			{
				this.Connection.Dispose();
			}
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void FInvoke(MethodInvoker inv)
		{
			base.BeginInvoke(inv);
		}

		private void FurtherHelpLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://www.elophant.com/league-of-legends/champion-select/help");
		}

        private void StartGui()
        {

        }
        private void Reader_ObjectRead(object obj)
        {
            PlaymoreClient.Gui.MainForm.LOGGER.Info("object read " + obj.ToString());
            string json = new JavaScriptSerializer().Serialize(obj);
            SendPacket(json);
            
            
            if (obj is GameDTO)
            {
                // UPDATE GUI HERE
                this.GameLobbyUpdate((GameDTO)obj);
                return;
            }
            if (obj is List<ChampionDTO>)
            {
                //do we even need this?
                this.Champions = (List<ChampionDTO>)obj;
                if (this.Champions.Count > 0)
                {
                    //this.SaveOwnedSkins.Enabled = true;
                    //-changed- can't access gui from a different thread
                    return;
                }
            }
            else if (obj is LoginDataPacket)
            {
                //do we even need this?
                this.SelfSummoner = ((LoginDataPacket)obj).AllSummonerData.Summoner;
            }
            else if (obj is EndOfGameStats)
            {
                //do something with these stats. i remember we had something in our minds about fancy resoving criterias. here it shall be
                ProcessGameStats((EndOfGameStats)obj);
            }
        }

        public void ProcessGameStats(EndOfGameStats stats)
        {

        }
        private void SendPacket(string json)
        {
        //{
        //    IPHostEntry ipHost = Dns.GetHostEntry("localhost");
        //    IPAddress ipAddr = ipHost.AddressList[0];
        //    IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 4510);  

        //    Socket sListener;
        //    SocketPermission permission = new SocketPermission(NetworkAccess.Accept,
        //           TransportType.Tcp, "", SocketPermission.AllPorts);
        //    sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        //    sListener.Bind(ipEndPoint);
        //    Byte[] bytes = Encoding.UTF8.GetBytes(json);
        //    sListener.BeginSend(bytes, 0, bytes.Length, 0, new AsyncCallback(delegate(IAsyncResult result) { Console.WriteLine(result); }), sListener);
        //    sListener.Close();
             Socket s = null;
        IPHostEntry hostEntry = null;

        // Get host related information.
        hostEntry = Dns.GetHostEntry("localhost");

        // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid 
        // an exception that occurs when the host IP Address is not compatible with the address family 
        // (typical in the IPv6 case). 
        foreach(IPAddress address in hostEntry.AddressList)
        {
            IPEndPoint ipe = new IPEndPoint(address, 4545);
            Socket tempSocket = 
                new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            tempSocket.Connect(ipe);

            if(tempSocket.Connected)
            {
                s = tempSocket;
                break;
            }
            else
            {
                continue;
            }
        }
        s.Send(Encoding.UTF8.GetBytes(json));
        s.Close();
       
        }

		public void GameLobbyUpdate(GameDTO lobby)
		{
			if (base.InvokeRequired)
			{
				Action<GameDTO> action = new Action<GameDTO>(this.GameLobbyUpdate);
				object[] objArray = new object[] { lobby };
				base.BeginInvoke(action, objArray);
				return;
			}
			if (this.CurrentGame == null || this.CurrentGame.Id != lobby.Id)
			{
                //this.Teammates = new List<string>();
                //this.EnemyTeam = new List<string>();
                //this.EnemyChampionIds = new List<int>();
                //this.Bans = new HashSet<int>();
                //this.LockedInChampions = new HashSet<int>();
                //this.LockedInSummoners = new HashSet<string>();

                //new game started
				this.CurrentGame = lobby;
			}
            UPDATE_LOGGER.Info(lobby.ToString());
            
            if (lobby.QueueTypeName.Equals("NONE") && lobby.GameMode.Equals("CLASSIC") && lobby.GameType.Equals("PRACTICE_GAME"))
            {
                string gameState = lobby.GameState;
                if (gameState.Equals("TEAM_SELECT"))
                {
                    //update team lineup
                }
                else if (gameState.Equals("CHAMP_SELECT"))
                {
                    //team lined up.update champs
                }
                else if (gameState.Equals("START_REQUESTED"))
                {
                    //print "starting" or smth
                }
                else if (gameState.Equals("TERMINATED"))
                {
                    //print "hooray terminted" or smth
                    // endgamestats should have arrived by now
                }
                else if (gameState.Equals("TERMINATED_IN_ERROR"))
                {
                    //game wasn't finished. happened when everybody left
                }
            }
            else
            {
                //unknown GameDTO
            }

            //if (!lobby.queuetypename.equals("ranked_solo_5x5") && !lobby.queuetypename.equals("normal") && !lobby.queuetypename.equals("ranked_team_5x5"))
            //{
            //    return;
            //}
            //if (lobby.gamestate.equals("pre_champ_select") || lobby.gamestate.equals("champ_select") && lobby.queuetypename.equals("normal"))
            //{//-changed- champ select
            //    if (this.teammates.count == 0)
            //    {
            //        this.teamid = 0;
            //        list<teamparticipants> teamparticipants = new list<teamparticipants>();
            //        teamparticipants.add(lobby.teamone);
            //        teamparticipants.add(lobby.teamtwo);
            //        list<teamparticipants> teamparticipants1 = teamparticipants;
            //        for (int i = 0; i < teamparticipants1.count; i++)
            //        {
            //            for (int j = 0; j < teamparticipants1[i].count; j++)
            //            {
            //                playerparticipant item = teamparticipants1[i][j] as playerparticipant;
            //                if (item != null && item.summonerid != (long)0 && item.summonerid == this.selfsummoner.summonerid)
            //                {
            //                    this.teamid = i;
            //                }
            //            }
            //        }
            //        for (int k = 0; k < teamparticipants1[this.teamid].count; k++)
            //        {
            //            playerparticipant playerparticipant = teamparticipants1[this.teamid][k] as playerparticipant;
            //            if (playerparticipant != null && !string.isnullorempty(playerparticipant.name))
            //            {
            //                this.teammates.add(playerparticipant.name);
            //            }
            //        }
            //        elophantapi.createteamstatisticsinstance(this.teammates, lobby.queuetypename, this.settings.region.tostring(), new uploadstringcompletedeventhandler(this.createteamstatisticsinstancecompleted));
            //    }
            //    if (lobby.bannedchampions != null && lobby.bannedchampions.count > 0)
            //    {
            //        foreach (bannedchampion bannedchampion in lobby.bannedchampions)
            //        {
            //            if (!this.bans.add(bannedchampion.championid))
            //            {
            //                continue;
            //            }
            //            elophantapi.addban(bannedchampion.championid, this.instanceid);
            //        }
            //    }
            //}
            //else if (!lobby.gamestate.equals("champ_select"))
            //{
            //    if (lobby.gamestate.equals("start_requested") || lobby.gamestate.equals("post_champ_select"))
            //    {//locked in info
            //        if (lobby.playerchampionselections != null && lobby.playerchampionselections.count > 0)
            //        {
            //            hashset<string> strs = new hashset<string>();
            //            hashset<int> nums = new hashset<int>();
            //            foreach (playerchampionselection playerchampionselection in lobby.playerchampionselections)
            //            {
            //                if (this.lockedinsummoners.add(playerchampionselection.summonerinternalname) && !regex.match(playerchampionselection.summonerinternalname, "summoner([0-9]+)$", regexoptions.ignorecase).success)
            //                {
            //                    strs.add(playerchampionselection.summonerinternalname);
            //                }
            //                if (!this.lockedinchampions.add(playerchampionselection.championid))
            //                {
            //                    continue;
            //                }
            //                nums.add(playerchampionselection.championid);
            //            }
            //            if (strs.count > 0 && nums.count > 0)
            //            {
            //                elophantapi.addlockedinplayers(strs, nums, this.instanceid);
            //            }
            //            int num = math.abs(this.teamid - 1);
            //            list<teamparticipants> teamparticipants2 = new list<teamparticipants>();
            //            teamparticipants2.add(lobby.teamone);
            //            teamparticipants2.add(lobby.teamtwo);
            //            foreach (participant participant in teamparticipants2[num])
            //            {
            //                playerparticipant playerparticipant1 = participant as playerparticipant;
            //                if (playerparticipant1 == null || string.isnullorempty(playerparticipant1.name))
            //                {
            //                    continue;
            //                }
            //                this.enemyteam.add(playerparticipant1.name);
            //                foreach (playerchampionselection playerchampionselection1 in lobby.playerchampionselections)
            //                {
            //                    if (!playerchampionselection1.summonerinternalname.equals(playerparticipant1.internalname))
            //                    {
            //                        continue;
            //                    }
            //                    this.enemychampionids.add(playerchampionselection1.championid);
            //                }
            //            }
            //            if (this.enemyteam.count == 5 && this.enemychampionids.count == 5)
            //            {
            //                elophantapi.addenemyteam(this.enemyteam, this.enemychampionids, this.instanceid);
            //            }
            //        }
            //        this.openteamstats.enabled = false;
            //        return;
            //    }
            //    if (lobby.gamestate.equals("terminated"))
            //    {
            //        this.openteamstats.enabled = false;
            //    }
            //}
            //else if (lobby.playerchampionselections != null && lobby.playerchampionselections.count > 0)
            //{//-changed- pick turn of whatever. it doesn't matter for now
            //    hashset<string> strs1 = new hashset<string>();
            //    hashset<int> nums1 = new hashset<int>();
            //    foreach (playerchampionselection playerchampionselection2 in lobby.playerchampionselections)
            //    {
            //        if (lobby.pickturn <= this.getplayerpickturn(lobby, playerchampionselection2.summonerinternalname) || playerchampionselection2.championid == 0)
            //        {
            //            continue;
            //        }
            //        if (this.lockedinsummoners.add(playerchampionselection2.summonerinternalname) && !regex.match(playerchampionselection2.summonerinternalname, "summoner([0-9]+)$", regexoptions.ignorecase).success)
            //        {
            //            strs1.add(playerchampionselection2.summonerinternalname);
            //        }
            //        if (!this.lockedinchampions.add(playerchampionselection2.championid))
            //        {
            //            continue;
            //        }
            //        nums1.add(playerchampionselection2.championid);
            //    }
            //    if (strs1.count > 0 || nums1.count > 0)
            //    {
            //        elophantapi.addlockedinplayers(strs1, nums1, this.instanceid);
            //        return;
            //    }
            //}
		}

        //private static T GetParent<T>(Control c)
        //where T : Control
        //{
        //    if (c == null)
        //    {
        //        return default(T);
        //    }
        //    if (c.GetType() == typeof(T))
        //    {
        //        return (T)c;
        //    }
        //    return MainForm.GetParent<T>(c.Parent);
        //}

        //private int GetPlayerPickTurn(GameDTO lobby, string internalSummonerName)
        //{
        //    List<TeamParticipants> teamParticipants = new List<TeamParticipants>();
        //    teamParticipants.Add(lobby.TeamOne);
        //    teamParticipants.Add(lobby.TeamTwo);
        //    List<TeamParticipants> teamParticipants1 = teamParticipants;
        //    for (int i = 0; i < teamParticipants1.Count; i++)
        //    {
        //        for (int j = 0; j < teamParticipants1[i].Count; j++)
        //        {
        //            PlayerParticipant item = teamParticipants1[i][j] as PlayerParticipant;
        //            if (item != null && item.InternalName != null && item.InternalName.Equals(internalSummonerName))
        //            {
        //                return item.PickTurn;
        //            }
        //        }
        //    }
        //    return 0;
        //}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MainForm));
			this.PlayerEditStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.editToolStripMenuItem = new ToolStripMenuItem();
			this.clearToolStripMenuItem = new ToolStripMenuItem();
			this.CallEditStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.dumpToolStripMenuItem = new ToolStripMenuItem();
			this.clearToolStripMenuItem1 = new ToolStripMenuItem();
			this.ModuleGroupBox = new GroupBox();
			this.MirrorRadio = new RadioButton();
			this.ToolHelpRadio = new RadioButton();
			this.ProcessRadio = new RadioButton();
			this.RegionList = new ComboBox();
			this.RegionLabel = new Label();
			this.StatusStrip = new System.Windows.Forms.StatusStrip();
			this.StatusLabel = new ToolStripStatusLabel();
			this.label5 = new Label();
			this.OpenTeamStats = new Button();
			this.VersionLabel = new Label();
			this.SaveOwnedSkins = new Button();
			this.pictureBox1 = new PictureBox();
			this.StepsLabel = new Label();
			this.StepTwoLabel = new Label();
			this.label1 = new Label();
			this.label2 = new Label();
			this.label3 = new Label();
			this.FurtherHelpLink = new LinkLabel();
			this.PlayerEditStrip.SuspendLayout();
			this.CallEditStrip.SuspendLayout();
			this.ModuleGroupBox.SuspendLayout();
			this.StatusStrip.SuspendLayout();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			ToolStripItemCollection items = this.PlayerEditStrip.Items;
			ToolStripItem[] toolStripItemArray = new ToolStripItem[] { this.editToolStripMenuItem, this.clearToolStripMenuItem };
			items.AddRange(toolStripItemArray);
			this.PlayerEditStrip.Name = "PlayerEditStrip";
			this.PlayerEditStrip.Size = new System.Drawing.Size(102, 48);
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
			this.editToolStripMenuItem.Text = "Edit";
			this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
			this.clearToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
			this.clearToolStripMenuItem.Text = "Clear";
			ToolStripItemCollection toolStripItemCollections = this.CallEditStrip.Items;
			ToolStripItem[] toolStripItemArray1 = new ToolStripItem[] { this.dumpToolStripMenuItem, this.clearToolStripMenuItem1 };
			toolStripItemCollections.AddRange(toolStripItemArray1);
			this.CallEditStrip.Name = "CallEditStrip";
			this.CallEditStrip.Size = new System.Drawing.Size(108, 48);
			this.dumpToolStripMenuItem.Name = "dumpToolStripMenuItem";
			this.dumpToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.dumpToolStripMenuItem.Text = "Dump";
			this.clearToolStripMenuItem1.Name = "clearToolStripMenuItem1";
			this.clearToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
			this.clearToolStripMenuItem1.Text = "Clear";
			this.ModuleGroupBox.BackColor = Color.Transparent;
			this.ModuleGroupBox.Controls.Add(this.MirrorRadio);
			this.ModuleGroupBox.Controls.Add(this.ToolHelpRadio);
			this.ModuleGroupBox.Controls.Add(this.ProcessRadio);
			this.ModuleGroupBox.Location = new Point(266, 8);
			this.ModuleGroupBox.Name = "ModuleGroupBox";
			this.ModuleGroupBox.Size = new System.Drawing.Size(125, 95);
			this.ModuleGroupBox.TabIndex = 11;
			this.ModuleGroupBox.TabStop = false;
			this.ModuleGroupBox.Text = "Module Resolver";
			this.ModuleGroupBox.Visible = false;
			this.MirrorRadio.AutoSize = true;
			this.MirrorRadio.Location = new Point(6, 65);
			this.MirrorRadio.Name = "MirrorRadio";
			this.MirrorRadio.Size = new System.Drawing.Size(51, 17);
			this.MirrorRadio.TabIndex = 2;
			this.MirrorRadio.Text = "Mirror";
			this.MirrorRadio.UseVisualStyleBackColor = true;
			this.ToolHelpRadio.AutoSize = true;
			this.ToolHelpRadio.Location = new Point(6, 42);
			this.ToolHelpRadio.Name = "ToolHelpRadio";
			this.ToolHelpRadio.Size = new System.Drawing.Size(78, 17);
			this.ToolHelpRadio.TabIndex = 1;
			this.ToolHelpRadio.Text = "Toolhelp32";
			this.ToolHelpRadio.UseVisualStyleBackColor = true;
			this.ProcessRadio.AutoSize = true;
			this.ProcessRadio.Checked = true;
			this.ProcessRadio.Location = new Point(6, 19);
			this.ProcessRadio.Name = "ProcessRadio";
			this.ProcessRadio.Size = new System.Drawing.Size(88, 17);
			this.ProcessRadio.TabIndex = 0;
			this.ProcessRadio.TabStop = true;
			this.ProcessRadio.Text = "ProcessClass";
			this.ProcessRadio.UseVisualStyleBackColor = true;
			this.RegionList.DropDownStyle = ComboBoxStyle.DropDownList;
			this.RegionList.Font = new System.Drawing.Font("Segoe UI", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.RegionList.FormattingEnabled = true;
			this.RegionList.Location = new Point(61, 65);
			this.RegionList.Name = "RegionList";
			this.RegionList.Size = new System.Drawing.Size(156, 25);
			this.RegionList.TabIndex = 10;
			this.RegionList.SelectedIndexChanged += new EventHandler(this.RegionList_SelectedIndexChanged);
			this.RegionLabel.AutoSize = true;
			this.RegionLabel.BackColor = Color.Transparent;
			this.RegionLabel.Font = new System.Drawing.Font("Segoe UI", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.RegionLabel.Location = new Point(3, 68);
			this.RegionLabel.Name = "RegionLabel";
			this.RegionLabel.Size = new System.Drawing.Size(52, 17);
			this.RegionLabel.TabIndex = 12;
			this.RegionLabel.Text = "Region:";
			this.StatusStrip.Items.AddRange(new ToolStripItem[] { this.StatusLabel });
			this.StatusStrip.Location = new Point(0, 324);
			this.StatusStrip.Name = "StatusStrip";
			this.StatusStrip.Size = new System.Drawing.Size(274, 22);
			this.StatusStrip.SizingGrip = false;
			this.StatusStrip.TabIndex = 16;
			this.StatusStrip.Text = "Searching for the LoL Client...";
			this.StatusLabel.BackColor = Color.Transparent;
			this.StatusLabel.Name = "StatusLabel";
			this.StatusLabel.Size = new System.Drawing.Size(162, 17);
			this.StatusLabel.Text = "Searching for the LoL Client...";
			this.label5.BorderStyle = BorderStyle.Fixed3D;
			this.label5.Location = new Point(6, 282);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(265, 2);
			this.label5.TabIndex = 20;
			this.OpenTeamStats.AutoSize = true;
			this.OpenTeamStats.Enabled = false;
			this.OpenTeamStats.Font = new System.Drawing.Font("Segoe UI", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.OpenTeamStats.Location = new Point(13, 291);
			this.OpenTeamStats.Name = "OpenTeamStats";
			this.OpenTeamStats.Size = new System.Drawing.Size(118, 27);
			this.OpenTeamStats.TabIndex = 23;
			this.OpenTeamStats.Text = "Open Team Stats";
			this.OpenTeamStats.UseVisualStyleBackColor = true;
			this.OpenTeamStats.Click += new EventHandler(this.OpenTeamStats_Click);
			this.VersionLabel.AutoSize = true;
			this.VersionLabel.BackColor = SystemColors.MenuBar;
			this.VersionLabel.Font = new System.Drawing.Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.VersionLabel.Location = new Point(233, 328);
			this.VersionLabel.Name = "VersionLabel";
			this.VersionLabel.Size = new System.Drawing.Size(0, 15);
			this.VersionLabel.TabIndex = 24;
			this.SaveOwnedSkins.AutoSize = true;
			this.SaveOwnedSkins.Enabled = false;
			this.SaveOwnedSkins.Font = new System.Drawing.Font("Segoe UI", 9.75f);
			this.SaveOwnedSkins.Location = new Point(139, 291);
			this.SaveOwnedSkins.Name = "SaveOwnedSkins";
			this.SaveOwnedSkins.Size = new System.Drawing.Size(123, 27);
			this.SaveOwnedSkins.TabIndex = 25;
			this.SaveOwnedSkins.Text = "Save Owned Skins";
			this.SaveOwnedSkins.UseVisualStyleBackColor = true;
			this.SaveOwnedSkins.Click += new EventHandler(this.ViewOwnedSkins_Click);
			this.pictureBox1.Image = Resources.ElophantDotComClientHeader;
			this.pictureBox1.Location = new Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(274, 59);
			this.pictureBox1.TabIndex = 26;
			this.pictureBox1.TabStop = false;
			this.StepsLabel.AutoSize = true;
			this.StepsLabel.BackColor = Color.Transparent;
			this.StepsLabel.Font = new System.Drawing.Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.StepsLabel.Location = new Point(2, 105);
			this.StepsLabel.Name = "StepsLabel";
			this.StepsLabel.Size = new System.Drawing.Size(51, 21);
			this.StepsLabel.TabIndex = 28;
			this.StepsLabel.Text = "Steps:";
			this.StepTwoLabel.AutoSize = true;
			this.StepTwoLabel.BackColor = Color.Transparent;
			this.StepTwoLabel.Font = new System.Drawing.Font("Segoe UI", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.StepTwoLabel.Location = new Point(11, 126);
			this.StepTwoLabel.MaximumSize = new System.Drawing.Size(260, 0);
			this.StepTwoLabel.Name = "StepTwoLabel";
			this.StepTwoLabel.Size = new System.Drawing.Size(258, 51);
			this.StepTwoLabel.TabIndex = 29;
			this.StepTwoLabel.Text = "1. Open LoL, open this client, then log in to the LoL Client. The status bar should say \"Successfully connected to the LoL Client.\"";
			this.label1.AutoSize = true;
			this.label1.BackColor = Color.Transparent;
			this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.label1.Location = new Point(11, 180);
			this.label1.MaximumSize = new System.Drawing.Size(250, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(238, 34);
			this.label1.TabIndex = 30;
			this.label1.Text = "2. Join any ranked or normal queue on Summoner's Rift.";
			this.label2.AutoSize = true;
			this.label2.BackColor = Color.Transparent;
			this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.label2.Location = new Point(11, 217);
			this.label2.MaximumSize = new System.Drawing.Size(260, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(255, 34);
			this.label2.TabIndex = 31;
			this.label2.Text = "3. In champ select, the \"Open Team Stats\" button will become enabled.";
			this.label3.BorderStyle = BorderStyle.Fixed3D;
			this.label3.Location = new Point(6, 99);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(265, 2);
			this.label3.TabIndex = 32;
			this.FurtherHelpLink.AutoSize = true;
			this.FurtherHelpLink.BackColor = Color.Transparent;
			this.FurtherHelpLink.LinkBehavior = LinkBehavior.HoverUnderline;
			this.FurtherHelpLink.Location = new Point(11, 259);
			this.FurtherHelpLink.Name = "FurtherHelpLink";
			this.FurtherHelpLink.Size = new System.Drawing.Size(128, 13);
			this.FurtherHelpLink.TabIndex = 33;
			this.FurtherHelpLink.TabStop = true;
			this.FurtherHelpLink.Text = "Click here for further help.";
			this.FurtherHelpLink.LinkClicked += new LinkLabelLinkClickedEventHandler(this.FurtherHelpLink_LinkClicked);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = SystemColors.Window;
			this.BackgroundImage = Resources.noise;
			base.ClientSize = new System.Drawing.Size(274, 346);
			base.Controls.Add(this.FurtherHelpLink);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.pictureBox1);
			base.Controls.Add(this.VersionLabel);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.StatusStrip);
			base.Controls.Add(this.ModuleGroupBox);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.StepTwoLabel);
			base.Controls.Add(this.RegionList);
			base.Controls.Add(this.StepsLabel);
			base.Controls.Add(this.RegionLabel);
			base.Controls.Add(this.OpenTeamStats);
			base.Controls.Add(this.label5);
			base.Controls.Add(this.SaveOwnedSkins);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(100, 100);
			base.Name = "MainForm";
			this.Text = "Elophant Client";
			base.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);
			base.Shown += new EventHandler(this.MainForm_Shown);
			base.ResizeBegin += new EventHandler(this.MainForm_ResizeBegin);
			base.ResizeEnd += new EventHandler(this.MainForm_ResizeEnd);
			base.Resize += new EventHandler(this.MainForm_Resize);
			this.PlayerEditStrip.ResumeLayout(false);
			this.CallEditStrip.ResumeLayout(false);
			this.ModuleGroupBox.ResumeLayout(false);
			this.ModuleGroupBox.PerformLayout();
			this.StatusStrip.ResumeLayout(false);
			this.StatusStrip.PerformLayout();
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void Injector_Injected(object sender, EventArgs e)
		{
			if (base.Created)
            {
                PlaymoreClient.Gui.MainForm.LOGGER.Info("injector injected");
				base.BeginInvoke(new Action(this.UpdateStatus));
			}
		}

		private void launcher_ProcessFound(object sender, ProcessMonitor.ProcessEventArgs e)
		{
            Console.WriteLine("proc found");
		}

		private void Log(Levels level, object obj)
		{
			object obj1 = string.Format("[{0}] {1} ({2:MM/dd/yyyy HH:mm:ss.fff})", level.ToString().ToUpper(), obj, DateTime.UtcNow);
			Task.Factory.StartNew(new Action<object>(this.LogToFile), obj1);
		}

		private void LogException(Exception ex, bool track)
		{
			this.LogToFile(string.Format("[{0}] {1} ({2:MM/dd/yyyy HH:mm:ss.fff})", Levels.Fatal.ToString().ToUpper(), string.Format("{0} [{1}]", ex.Message, Parse.ToBase64(ex.ToString())), DateTime.UtcNow));
			if (track)
			{
				this.TrackingQueue.Enqueue(string.Format("error/{0}", Parse.ToBase64(ex.ToString())));
			}
		}

		private void LogToFile(object obj)
		{
			try
			{
				lock (this.LogLock)
				{
					File.AppendAllText("Log.txt", string.Concat(obj, Environment.NewLine));
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				PlaymoreClient.Gui.MainForm.LOGGER.Error(string.Format("[{0}] {1} ({2:MM/dd/yyyy HH:mm:ss.fff})", Levels.Fatal.ToString().ToUpper(), exception.Message, DateTime.UtcNow));
			}
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if ((this.Connection != null && this.Connection.IsConnected || this.CurrentGame != null) && MessageBox.Show("Closing the Elophant Client now will disconnect you from the LoL Client. Are you sure that you want to exit?", "Exit Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
			{
				e.Cancel = true;
			}
		}

		private void MainForm_Resize(object sender, EventArgs e)
		{
		}

		private void MainForm_ResizeBegin(object sender, EventArgs e)
		{
			base.SuspendLayout();
		}

		private void MainForm_ResizeEnd(object sender, EventArgs e)
		{
			base.ResumeLayout();
			this.MainForm_Resize(sender, e);
		}

        //start all the services
		private void MainForm_Shown(object sender, EventArgs e)
		{
			this.TrackingQueue.Enqueue("startup");
			//this.Settings_Loaded(this, new EventArgs());
			this.UpdateStatus();
			this.Settings.PropertyChanged += new PropertyChangedEventHandler(this.Settings_PropertyChanged);
			this.Settings.ModuleResolver = "Toolhelp32Snapshot";
			this.VersionLabel.Text = string.Concat("v", MainForm.Version);
			this.Connection.Start();
			this.Injector.Start();
            this.Launcher.Start();
			this.MainForm_Resize(this, new EventArgs());
			this.TryToCheckForUpdates();
		}

		private void moduleresolvers_Click(object sender, EventArgs e)
		{
			KeyValuePair<ProcessInjector.GetModuleFrom, RadioButton> keyValuePair = this.ModuleResolvers.FirstOrDefault<KeyValuePair<ProcessInjector.GetModuleFrom, RadioButton>>((KeyValuePair<ProcessInjector.GetModuleFrom, RadioButton> kv) => kv.Value.Checked);
			this.Settings.ModuleResolver = keyValuePair.Key.ToString();
			this.Injector.Clear();
		}

		private void OnLog(Levels level, object obj)
		{
			if (level == Levels.Trace)
			{
				return;
			}
			if (level == Levels.Debug)
			{
				return;
			}
			if (level == Levels.Error && obj is Exception)
			{
				this.TrackingQueue.Enqueue(string.Format("error/{0}", Parse.ToBase64(obj.ToString())));
			}
			if (!(obj is Exception))
			{
				this.Log(level, obj);
				return;
			}
			this.Log(level, string.Format("{0} [{1}]", ((Exception)obj).Message, Parse.ToBase64(obj.ToString())));
		}

		private void OpenTeamStats_Click(object sender, EventArgs e)
		{
			Process.Start(string.Concat("http://www.elophant.com/league-of-legends/champion-select/", this.InstanceId));
		}

		private void RegionList_SelectedIndexChanged(object sender, EventArgs e)
		{
			LeagueRegion leagueRegion;
			if (!Enum.TryParse<LeagueRegion>(this.RegionsFullText.FirstOrDefault<KeyValuePair<LeagueRegion, string>>((KeyValuePair<LeagueRegion, string> x) => x.Value == this.RegionList.SelectedItem.ToString()).Key.ToString(), out leagueRegion))
			{
				KeyValuePair<LeagueRegion, string> keyValuePair = this.RegionsFullText.FirstOrDefault<KeyValuePair<LeagueRegion, string>>((KeyValuePair<LeagueRegion, string> x) => x.Value == this.RegionList.SelectedItem.ToString());
				PlaymoreClient.Gui.MainForm.LOGGER.Warn(string.Concat("Unknown enum ", keyValuePair.Key.ToString(), "."));
				return;
			}
			this.Settings.Region = leagueRegion;
			KeyValuePair<LeagueRegion, CertificateHolder> keyValuePair1 = this.Certificates.FirstOrDefault<KeyValuePair<LeagueRegion, CertificateHolder>>((KeyValuePair<LeagueRegion, CertificateHolder> kv) => kv.Key == this.Settings.Region);
			CertificateHolder value = keyValuePair1.Value ?? this.Certificates.First<KeyValuePair<LeagueRegion, CertificateHolder>>().Value;
			this.Connection.ChangeRemote(value.Domain, value.Certificate);
		}

		private void SavingSkinsCompleted(object sender, UploadStringCompletedEventArgs e)
		{
			try
			{
				if (!JsonSerializer.DeserializeFromString<bool>(e.Result))
				{
					this.SaveOwnedSkins.Text = "Error";
					PlaymoreClient.Gui.MainForm.LOGGER.Error("Unknown error while saving skins.");
					MessageBox.Show("An error occurred while saving the skins. Check the log file for more info.", "Save Skins Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
				}
				else
				{
					object[] lower = new object[] { "http://www.elophant.com/league-of-legends/summoner/", this.Settings.Region.ToString().ToLower(), "/", this.SelfSummoner.SummonerId, "/skins" };
					Process.Start(string.Concat(lower));
					this.SaveOwnedSkins.Text = "Saved";
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.SaveOwnedSkins.Text = "Error";
				PlaymoreClient.Gui.MainForm.LOGGER.Error(exception.ToString());
				MessageBox.Show("An error occurred while saving the skins. Check the log file for more info.", "Save Skins Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
		}

		private void Settings_Loaded(object sender, EventArgs e)
		{
			KeyValuePair<ProcessInjector.GetModuleFrom, RadioButton> keyValuePair = this.ModuleResolvers.FirstOrDefault<KeyValuePair<ProcessInjector.GetModuleFrom, RadioButton>>((KeyValuePair<ProcessInjector.GetModuleFrom, RadioButton> kv) => kv.Key.ToString() == this.Settings.ModuleResolver);
			if (keyValuePair.Value == null)
			{
				keyValuePair = this.ModuleResolvers.First<KeyValuePair<ProcessInjector.GetModuleFrom, RadioButton>>();
			}
			keyValuePair.Value.Checked = true;
		}

		private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			lock (this.settingslock)
			{
				PlaymoreClient.Gui.MainForm.LOGGER.Info("Settings saved");
				this.Settings.Save("settings.json");
			}
		}

		private void SetTitle(string title)
		{
			string version = MainForm.Version;
			this.Text = string.Format("Elophant Client v{0}{1}", version, (!string.IsNullOrEmpty(title) ? string.Concat(" - ", title) : ""));
		}

		private void TrackingQueue_Process(object sender, ProcessQueueEventArgs<string> e)
		{
		}

		private void TryToCheckForUpdates()
		{
			string version = MainForm.Version;
			using (WebClient webClient = new WebClient())
			{
				try
				{
					version = JsonSerializer.DeserializeFromString<StringResponse>(webClient.DownloadString(PlaymoreClient.Properties.Settings.Default.LatestVersionAPIPath)).data;
				}
				catch (Exception exception)
				{
					PlaymoreClient.Gui.MainForm.LOGGER.Error(exception.ToString());
					return;
				}
			}
			if (!version.Equals(MainForm.Version) && MessageBox.Show("A newer version of the client is available. Would you like to download it?", "Update Available", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
			{
				Process.Start("http://www.elophant.com/league-of-legends/champion-select");
			}
		}

		private void UpdateStatus()
		{
            PlaymoreClient.Gui.MainForm.LOGGER.Info("updatestatus started");
			if (!this.Installer.IsInstalled)
			{
				this.StatusLabel.Text = "Elophant Client is not installed.";
				return;
			}
			if (this.Injector.IsInjected)
			{
				if (this.Connection != null && this.Connection.IsConnected)
				{
					this.StatusLabel.Text = "Successfully connected to the LoL Client.";
					this.RegionList.Enabled = false;
					return;
				}
				this.StatusLabel.Text = "LoL Client detected! Log in now.";
			}
			else
			{
				this.StatusLabel.Text = "LoL Client not detected. Open it now.";
				if (!this.RegionList.Enabled)
				{
					this.RegionList.Enabled = true;
					return;
				}
			}
		}

		private void ViewOwnedSkins_Click(object sender, EventArgs e)
		{
			this.SaveOwnedSkins.Enabled = false;
			this.SaveOwnedSkins.Text = "Saving...";
			HashSet<int> nums = new HashSet<int>();
			foreach (ChampionDTO champion in this.Champions)
			{
				foreach (ChampionSkinDTO championSkin in champion.ChampionSkins)
				{
					if (!championSkin.Owned)
					{
						continue;
					}
					nums.Add(championSkin.SkinId);
				}
			}
			ElophantAPI.SaveOwnedSkins(nums, this.SelfSummoner.Username, this.SelfSummoner.SummonerId, this.SelfSummoner.AccountId, this.Settings.Region.ToString(), new UploadStringCompletedEventHandler(this.SavingSkinsCompleted));
		}
	}
}