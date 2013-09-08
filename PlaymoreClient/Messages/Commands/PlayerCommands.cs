using PlaymoreClient.Messages;
using PlaymoreClient.Messages.GameLobby;
using PlaymoreClient.Messages.Statistics;
using PlaymoreClient.Messages.Summoner;
using PlaymoreClient.Messages.Translators;
using PlaymoreClient.Proxy;
using PlaymoreClient.Util;
using FluorineFx;
using FluorineFx.AMF3;
using FluorineFx.Messaging.Messages;
using FluorineFx.Messaging.Rtmp.Event;
using NotMissing.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Commands
{
	public class PlayerCommands
	{
		public RtmpsProxyHost Host
		{
			get;
			protected set;
		}

		public PlayerCommands(RtmpsProxyHost host)
		{
			this.Host = host;
		}

		public GameDTO GetGame(int num)
		{
			object[] objArray = new object[] { num };
			return this.InvokeService<GameDTO>("gameService", "getGame", objArray);
		}

		public PublicSummoner GetPlayerByName(string name)
		{
			object[] objArray = new object[] { name };
			return this.InvokeService<PublicSummoner>("summonerService", "getSummonerByName", objArray);
		}

		public RecentGames GetRecentGames(long acctid)
		{
			object[] objArray = new object[] { acctid };
			return this.InvokeService<RecentGames>("playerStatsService", "getRecentGames", objArray);
		}

		public T InvokeService<T>(string service, string operation, params object[] args)
		where T : class
		{
			T t;
			RemotingMessage remotingMessage = new RemotingMessage();
			remotingMessage.operation = operation;
			remotingMessage.destination = service;
			remotingMessage.headers["DSRequestTimeout"] = 60;
			remotingMessage.headers["DSId"] = RtmpUtil.RandomUidString();
			remotingMessage.headers["DSEndpoint"] = "my-rtmps";
			remotingMessage.body = args;
			remotingMessage.messageId = RtmpUtil.RandomUidString();
			string str = string.Concat(service, ".", operation);
			Notify notify = this.Host.Call(remotingMessage);
			if (notify == null)
			{
				PlaymoreClient.Gui.MainForm.LOGGER.Warn(string.Format("Invoking {0} returned null", str));
				return default(T);
			}
			if (RtmpUtil.IsError(notify))
			{
				ErrorMessage error = RtmpUtil.GetError(notify);
				string str1 = (error == null || error.faultDetail == null ? "" : string.Format(" [{0}]", error.faultDetail));
				PlaymoreClient.Gui.MainForm.LOGGER.Warn(string.Format("{0} returned an error{1}{2}", str, (error == null || error.faultString == null ? "" : string.Format(", {0}", error.faultString)), str1));
				return default(T);
			}
			Tuple<object, long> tuple = RtmpUtil.GetBodies(notify).FirstOrDefault<Tuple<object, long>>();
			if (tuple == null)
			{
				PlaymoreClient.Gui.MainForm.LOGGER.Debug(string.Concat(str, " RtmpUtil.GetBodies returned null"));
				return default(T);
			}
			if (tuple.Item1 == null)
			{
				PlaymoreClient.Gui.MainForm.LOGGER.Debug(string.Concat(str, " Body.Item1 returned null"));
				return default(T);
			}
			object item2 = null;
			if (!(tuple.Item1 is ASObject))
			{
				if (!(tuple.Item1 is ArrayCollection))
				{
					PlaymoreClient.Gui.MainForm.LOGGER.Debug(string.Concat(str, " unknown object ", tuple.Item1.GetType()));
					return default(T);
				}
				try
				{
					Type type = typeof(T);
					object[] item1 = new object[] { (ArrayCollection)tuple.Item1 };
					item2 = Activator.CreateInstance(type, item1);
					if (item2 is MessageObject)
					{
						((MessageObject)item2).TimeStamp = tuple.Item2;
					}
					return (T)item2;
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					PlaymoreClient.Gui.MainForm.LOGGER.Warn(string.Concat(str, " failed to construct ", typeof(T)));
					PlaymoreClient.Gui.MainForm.LOGGER.Debug(exception);
					t = default(T);
				}
				return t;
			}
			else
			{
				ASObject aSObject = (ASObject)tuple.Item1;
				item2 = MessageTranslator.Instance.GetObject<T>(aSObject);
				if (item2 == null)
				{
					object[] typeName = new object[] { str, " expected ", typeof(T), ", got ", aSObject.TypeName };
					PlaymoreClient.Gui.MainForm.LOGGER.Debug(string.Concat(typeName));
					return default(T);
				}
			}
			if (item2 is MessageObject)
			{
				((MessageObject)item2).TimeStamp = tuple.Item2;
			}
			return (T)item2;
		}

		public object InvokeServiceUnknown(string service, string operation, params object[] args)
		{
			RemotingMessage remotingMessage = new RemotingMessage();
			remotingMessage.operation = operation;
			remotingMessage.destination = service;
			remotingMessage.headers["DSRequestTimeout"] = 60;
			remotingMessage.headers["DSId"] = RtmpUtil.RandomUidString();
			remotingMessage.headers["DSEndpoint"] = "my-rtmps";
			remotingMessage.body = args;
			remotingMessage.messageId = RtmpUtil.RandomUidString();
			string str = string.Concat(service, ".", operation);
			Notify notify = this.Host.Call(remotingMessage);
			if (notify == null)
			{
				PlaymoreClient.Gui.MainForm.LOGGER.Warn(string.Format("Invoking {0} returned null", str));
				return null;
			}
			if (!RtmpUtil.IsError(notify))
			{
				Tuple<object, long> tuple = RtmpUtil.GetBodies(notify).FirstOrDefault<Tuple<object, long>>();
				if (tuple != null)
				{
					return tuple.Item1;
				}
				PlaymoreClient.Gui.MainForm.LOGGER.Debug(string.Concat(str, " RtmpUtil.GetBodies returned null"));
				return null;
			}
			ErrorMessage error = RtmpUtil.GetError(notify);
			string str1 = (error == null || error.faultDetail == null ? "" : string.Format(" [{0}]", error.faultDetail));
			PlaymoreClient.Gui.MainForm.LOGGER.Warn(string.Format("{0} returned an error{1}{2}", str, (error == null || error.faultString == null ? "" : string.Format(", {0}", error.faultString)), str1));
			return null;
		}

		public PlayerLifetimeStats RetrievePlayerStatsByAccountId(long acctid)
		{
			object[] objArray = new object[] { acctid, "CURRENT" };
			return this.InvokeService<PlayerLifetimeStats>("playerStatsService", "retrievePlayerStatsByAccountId", objArray);
		}

		public ChampionStatInfoList RetrieveTopPlayedChampions(long acctid, string gamemode)
		{
			object[] objArray = new object[] { acctid, gamemode };
			return this.InvokeService<ChampionStatInfoList>("playerStatsService", "retrieveTopPlayedChampions", objArray);
		}

		public SpellBookPage SelectDefaultSpellBookPage(SpellBookPage page)
		{
			object[] objArray = new object[] { page };
			return this.InvokeService<SpellBookPage>("spellBookService", "selectDefaultSpellBookPage", objArray);
		}
	}
}