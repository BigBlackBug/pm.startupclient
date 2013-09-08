using PlaymoreClient.Properties;
using NotMissing.Logging;
using System;
using System.Collections.Generic;
using System.Net;

namespace PlaymoreClient
{
	public static class ElophantAPI
	{
		public static void AddBan(int championId, string instanceId)
		{
			using (WebClient webClient = new WebClient())
			{
				webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
				try
				{
					webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(ElophantAPI.AddBanCompleted);
					Uri uri = new Uri(string.Concat(Settings.Default.TeamStatsAPIBase, "add/ban"));
					object[] objArray = new object[] { "championId=", championId, "&instanceId=", instanceId };
					webClient.UploadStringAsync(uri, string.Concat(objArray));
				}
				catch (WebException webException)
				{
					PlaymoreClient.Gui.MainForm.LOGGER.Error(webException.Message);
				}
			}
		}

		private static void AddBanCompleted(object sender, UploadStringCompletedEventArgs e)
		{
		}

		public static void AddEnemyTeam(List<string> summonerNames, List<int> selectedChampionIds, string instanceId)
		{
			using (WebClient webClient = new WebClient())
			{
				webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
				try
				{
					webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(ElophantAPI.AddEnemyTeamCompleted);
					Uri uri = new Uri(string.Concat(Settings.Default.TeamStatsAPIBase, "add/enemy_team"));
					string[] strArrays = new string[] { "summonerNames=", string.Join(",", summonerNames), "&selectedChampionIds=", string.Join<int>(",", selectedChampionIds), "&instanceId=", instanceId };
					webClient.UploadStringAsync(uri, string.Concat(strArrays));
				}
				catch (WebException webException)
				{
					PlaymoreClient.Gui.MainForm.LOGGER.Error(webException.Message);
				}
			}
		}

		private static void AddEnemyTeamCompleted(object sender, UploadStringCompletedEventArgs e)
		{
		}

		public static void AddLockedInPlayers(HashSet<string> summonerNames, HashSet<int> championIds, string instanceId)
		{
			using (WebClient webClient = new WebClient())
			{
				webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
				try
				{
					webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(ElophantAPI.AddLockedInPlayersCompleted);
					Uri uri = new Uri(string.Concat(Settings.Default.TeamStatsAPIBase, "add/locked_in_players"));
					string[] strArrays = new string[] { "summonerNames=", string.Join(",", summonerNames), "&championIds=", string.Join<int>(",", championIds), "&instanceId=", instanceId };
					webClient.UploadStringAsync(uri, string.Concat(strArrays));
				}
				catch (WebException webException)
				{
					PlaymoreClient.Gui.MainForm.LOGGER.Error(webException.Message);
				}
			}
		}

		private static void AddLockedInPlayersCompleted(object sender, UploadStringCompletedEventArgs e)
		{
		}

		public static void CreateTeamStatisticsInstance(List<string> players, string gameType, string region, UploadStringCompletedEventHandler result)
		{
			using (WebClient webClient = new WebClient())
			{
				webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
				try
				{
					webClient.UploadStringCompleted += result;
					Uri uri = new Uri(string.Concat(Settings.Default.TeamStatsAPIBase, "create"));
					string[] strArrays = new string[] { "summonerNames=", string.Join(",", players), "&gameType=", gameType, "&region=", region };
					webClient.UploadStringAsync(uri, string.Concat(strArrays));
				}
				catch (WebException webException)
				{
					PlaymoreClient.Gui.MainForm.LOGGER.Error(webException.Message);
				}
			}
		}

		public static void SaveOwnedSkins(HashSet<int> skinIds, string summonerName, long summonerId, long accountId, string region, UploadStringCompletedEventHandler result)
		{
			using (WebClient webClient = new WebClient())
			{
				webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
				try
				{
					webClient.UploadStringCompleted += result;
					Uri uri = new Uri(Settings.Default.SkinsAPIPath);
					object[] objArray = new object[] { "skinIds=", string.Join<int>(",", skinIds), "&summonerName=", summonerName, "&summonerId=", summonerId, "&accountId=", accountId, "&region=", region };
					webClient.UploadStringAsync(uri, string.Concat(objArray));
				}
				catch (WebException webException)
				{
					PlaymoreClient.Gui.MainForm.LOGGER.Error(webException.Message);
				}
			}
		}
	}
}