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
    public class PlaymoreControllerImpl : PlaymoreController
    {
        public static log4net.ILog LOGGER = log4net.LogManager.GetLogger("playmore_logger");
        public static log4net.ILog UPDATE_LOGGER = log4net.LogManager.GetLogger("game_update_logger");

        private const string SettingsFile = "settings.json";

        private readonly static string Version;

        private readonly Dictionary<LeagueRegion, CertificateHolder> Certificates;

        private RtmpsProxyHost Connection;

        private MessageReader Reader;

        private CertificateInstaller Installer;

        private ProcessInjector Injector;

        private GameDTO CurrentGame;

        private List<ChampionDTO> Champions;

        private SummonerData SelfSummoner;

        private MainSettings Settings
        {
            get
            {
                return MainSettings.Instance;
            }
        }

        private IView view;

        static PlaymoreControllerImpl()
        {
            PlaymoreControllerImpl.Version = string.Concat(AssemblyAttributes.FileVersion, AssemblyAttributes.Configuration);
        }

        public PlaymoreControllerImpl(IView view)
        {
            this.view = view;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);
            Application.ThreadException += new ThreadExceptionEventHandler(this.Application_ThreadException);
            PlaymoreClient.Gui.PlaymoreControllerImpl.LOGGER.Info(string.Format("Version {0}", PlaymoreControllerImpl.Version));
            this.Settings.Load("settings.json");
            Dictionary<LeagueRegion, CertificateHolder> leagueRegions = new Dictionary<LeagueRegion, CertificateHolder>();
            leagueRegions.Add(LeagueRegion.NA, new CertificateHolder("prod.na1.lol.riotgames.com", Resources.prod_na1_lol_riotgames_com));
            leagueRegions.Add(LeagueRegion.EUW, new CertificateHolder("prod.eu.lol.riotgames.com", Resources.prod_eu_lol_riotgames_com));
            leagueRegions.Add(LeagueRegion.EUNE, new CertificateHolder("prod.eun1.lol.riotgames.com", Resources.prod_eun1_lol_riotgames_com));
            this.Certificates = leagueRegions;
            //select region
            //Dictionary<LeagueRegion, string> leagueRegions1 = new Dictionary<LeagueRegion, string>();
            //leagueRegions1.Add(LeagueRegion.NA, "North America");
            //leagueRegions1.Add(LeagueRegion.EUW, "Europe West");
            //leagueRegions1.Add(LeagueRegion.EUNE, "Europe Nordic & East");
            //this.RegionsFullText = leagueRegions1;

            //selected region from settings
            KeyValuePair<LeagueRegion, CertificateHolder> keyValuePair = 
                this.Certificates.FirstOrDefault<KeyValuePair<LeagueRegion, CertificateHolder>>
                ((KeyValuePair<LeagueRegion, CertificateHolder> kv) => kv.Key == this.Settings.Region);
            CertificateHolder value = keyValuePair.Value 
                ?? this.Certificates.First<KeyValuePair<LeagueRegion, CertificateHolder>>().Value;
            LOGGER.Info("cert " + value.Domain);
            this.Injector = new ProcessInjector("lolclient");
            this.Connection = new RtmpsProxyHost(2099, value.Domain, 2099, value.Certificate);

            this.Reader = new MessageReader(this.Connection);
            this.Connection.Connected += new EventHandler(this.Connection_Connected);
            this.Injector.Injected += new EventHandler(this.Injector_Injected);
            this.Reader.ObjectRead += new ObjectReadD(this.ObjectRead);
            this.Connection.CallResult += new CallHandler(this.Connection_Call);
            this.Connection.Notify += new NotifyHandler(this.Connection_Notify);
            
            this.Installer = new CertificateInstaller((
                from c in this.Certificates
                select c.Value.Certificate).ToArray<X509Certificate2>());

            try
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4");
                if (registryKey == null || registryKey != null && !registryKey.GetSubKeyNames().Contains<string>("Full"))
                {
                    if (MessageBox.Show("The Playmore Client requires the .NET Framework 4.0 Full version."+
                        "Would you like to download it?", ".NET Framework 4.0 Full Not Found", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        Process.Start("http://www.microsoft.com/en-us/download/details.aspx?id=17718");
                    }
                    MessageBox.Show("The Playmore Client will now close.");
                    Process.GetCurrentProcess().Kill();
                    return;
                }
                
                
            }
            catch (Exception exception)
            {
                PlaymoreClient.Gui.PlaymoreControllerImpl.LOGGER.Error(exception.ToString());
                MessageBox.Show("An unknown exception has occurred. Check the log for more information.");
                Process.GetCurrentProcess().Kill();
                return;
            }

            string javaDirectory = null;
            RegistryKey javaKey =
                Registry.LocalMachine.OpenSubKey("SOFTWARE\\Javasoft\\Java Runtime Environment") ??
                Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Javasoft\\Java Runtime Environment");

            if (javaKey != null)
            {
                string javaVersion = javaKey.GetValue("CurrentVersion").ToString();
                try
                {
                    javaDirectory = javaKey.OpenSubKey(javaVersion).GetValue("JavaHome").ToString();
                }
                catch (NullReferenceException)
                { /* Ignore null deref, means we can't get a directory */ }
            }

            if (javaDirectory == null)
            {
                // deal with a lack of Java here.
            }

            try
            {
                string str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "lolbans", "LoLLoader.dll");
                if (File.Exists(str))
                {
                    PlaymoreClient.Gui.PlaymoreControllerImpl.LOGGER.Info("Uninstalling old loader.");
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
                PlaymoreClient.Gui.PlaymoreControllerImpl.LOGGER.Warn(securityException);
            }
            catch (Exception exception1)
            {
                PlaymoreClient.Gui.PlaymoreControllerImpl.LOGGER.Error(string.Concat("Failed to uninstall. Message: ", exception1));
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
                            MessageBox.Show(string.Concat("The Playmore Client was successfully installed!", Environment.NewLine, "Please relaunch without administrator privileges."), "Install Complete", MessageBoxButtons.OK);
                            Process.GetCurrentProcess().Kill();
                            return;
                        }
                        catch (UnauthorizedAccessException unauthorizedAccessException1)
                        {
                            UnauthorizedAccessException unauthorizedAccessException = unauthorizedAccessException1;
                            MessageBox.Show("Unable to fully install. Please make sure that the LoL Client is not running.", "Unable to Install", MessageBoxButtons.OK);
                            PlaymoreClient.Gui.PlaymoreControllerImpl.LOGGER.Warn(unauthorizedAccessException);
                        }
                        this.UpdateStatus();
                    }
                    else
                    {
                        MessageBox.Show(string.Concat("Please run the Playmore Client once as the administrator to install it.", Environment.NewLine, "Right click PlaymoreClient.exe, then click \"Run as administrator.\""), "Admin Privileges Required", MessageBoxButtons.OK);
                        Process.GetCurrentProcess().Kill();
                        return;
                    }
                }
            }
            catch
            {
            }
            PlaymoreClient.Gui.PlaymoreControllerImpl.LOGGER.Info("Startup Completed");
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
            //this.LogException(e.Exception, true);
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

            PlaymoreClient.Gui.PlaymoreControllerImpl.LOGGER.Info("CALLING METHOD " + call.ServiceCall.ServiceMethodName + " SERVICE " + call.ServiceCall.ServiceName);
            PlaymoreClient.Gui.PlaymoreControllerImpl.LOGGER.Info("call arguments " + string.Join(", ", call.ServiceCall.Arguments.Select<object, string>(new Func<object, string>(PlaymoreControllerImpl.CallArgToString))));
            PlaymoreClient.Gui.PlaymoreControllerImpl.LOGGER.Info("result arguments " + string.Join(", ", result.ServiceCall.Arguments.Select<object, string>(new Func<object, string>(PlaymoreControllerImpl.CallArgToString))));

            //PlaymoreClient.Gui.Model.LOGGER.Info(string.Format("Call {0} ({1}), Return ({2})", call.ServiceCall.ServiceMethodName, string.Join(", ", call.ServiceCall.Arguments.Select<object, string>(new Func<object, string>(Model.CallArgToString))), string.Join(", ", result.ServiceCall.Arguments.Select<object, string>(new Func<object, string>(Model.CallArgToString)))));
        }

        private void Connection_Connected(object sender, EventArgs e)
        {
            PlaymoreClient.Gui.PlaymoreControllerImpl.LOGGER.Info("connection connected");
            UpdateStatus();
        }

        private void Connection_Notify(object sender, Notify notify)
        {
            PlaymoreClient.Gui.PlaymoreControllerImpl.LOGGER.Info(string.Format("NOTIFIED {0}({1})", (!string.IsNullOrEmpty(notify.ServiceCall.ServiceMethodName) ? string.Concat(notify.ServiceCall.ServiceMethodName, " ") : ""), string.Join(", ", notify.ServiceCall.Arguments.Select<object, string>(new Func<object, string>(PlaymoreControllerImpl.CallArgToString)))));
        }

        //private void CreateTeamStatisticsInstanceCompleted(object sender, UploadStringCompletedEventArgs e)
        //{
        //    try
        //    {
        //        StringResponse stringResponse = JsonSerializer.DeserializeFromString<StringResponse>(e.Result);
        //        if (!stringResponse.success)
        //        {
        //            PlaymoreClient.Gui.Model.LOGGER.Error(stringResponse.error);
        //            MessageBox.Show("An error occurred while saving the team's stats. Check the log file for more info.", "Team Stats Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
        //        }
        //        else
        //        {
        //            PlaymoreClient.Gui.Model.LOGGER.Info(string.Concat("Team stats ID: ", stringResponse.data));
        //            this.InstanceId = stringResponse.data;
        //            this.OpenTeamStats.Enabled = true;
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        PlaymoreClient.Gui.Model.LOGGER.Error(exception.ToString());
        //        MessageBox.Show("An unknown error occurred while saving the team's stats. Check the log file for more info.", "Unknown Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
        //    }
        //}

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //this.LogException((Exception)e.ExceptionObject, !e.IsTerminating);
        }

        protected void Dispose(bool disposing)
        {
                 this.Connection.Dispose();
        }

        private void ObjectRead(object obj)
        {
            PlaymoreClient.Gui.PlaymoreControllerImpl.LOGGER.Info("object read " + obj.ToString());
            

            //string json = new JavaScriptSerializer().Serialize(obj);
            //JsonObject j = new JavaScriptSerializer().Deserialize<JsonObject>(json);
            //j.Add("type", "gamedto");
            if (obj is GameDTO)
            {
                // UPDATE GUI HERE
                string json = new JavaScriptSerializer().Serialize(obj);
                SendPacket(json);
                //this.GameLobbyUpdate((GameDTO)obj);
                return;
            }
            //if (obj is List<ChampionDTO>)
            //{
            //    //do we even need this?
            //    this.Champions = (List<ChampionDTO>)obj;
            //    if (this.Champions.Count > 0)
            //    {
            //        //this.SaveOwnedSkins.Enabled = true;
            //        //-changed- can't access gui from a different thread
            //        return;
            //    }
            //}
            //else 
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
            Socket s = null;
            IPHostEntry hostEntry = Dns.GetHostEntry("localhost"); ;

            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(address, 4545);
                Socket tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
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
            if (this.CurrentGame == null || this.CurrentGame.Id != lobby.Id)
            {
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
        }

        
        private void Injector_Injected(object sender, EventArgs e)
        {
            PlaymoreClient.Gui.PlaymoreControllerImpl.LOGGER.Info("injector injected");
            UpdateStatus();
        }

        private void launcher_ProcessFound(object sender, ProcessMonitor.ProcessEventArgs e)
        {
        }

        private void Model_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((this.Connection != null && this.Connection.IsConnected || this.CurrentGame != null) && MessageBox.Show("Closing the Playmore Client now will disconnect you from the LoL Client. Are you sure that you want to exit?", "Exit Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
            {
                e.Cancel = true;
            }
        }
        
        private void UpdateStatus()
        {
            LOGGER.Info("updatestatus started");
            if (!this.Installer.IsInstalled)
            {
               view.UpdateStatus( "Playmore Client is not installed.");
                return;
            }
            if (this.Injector.IsInjected)
            {
                if (this.Connection != null && this.Connection.IsConnected)
                {
                    view.UpdateStatus( "Successfully connected to the LoL Client.");

                }
                else
                {
                    view.UpdateStatus( "LoL Client detected! Log in now.");
                }
            }
            else
            {
                view.UpdateStatus("LoL Client not detected. Open it now.");
            }
        }

        public bool logIn(string userName, string password)
        {
            startServices();
            return true;
        }

        private void startServices()
        {
            this.UpdateStatus();
            this.Connection.Start();
            this.Injector.Start();
        }

        public void changeRegion(LeagueRegion region)
        {
            CertificateHolder value;
            Certificates.TryGetValue(region, out value);
            this.Settings.Region = region;
            Console.WriteLine("changed setting");
            this.Connection.ChangeRemote(value.Domain, value.Certificate);
        }

        public LeagueRegion getCurrentRegion()
        {
            return Settings.Region;
        }


        public void stopServices()
        {
     		if (this.Connection != null)
			{
				this.Connection.Dispose();
			}
        }

        public string getVersion()
        {
            return Version;
        }
    }
}