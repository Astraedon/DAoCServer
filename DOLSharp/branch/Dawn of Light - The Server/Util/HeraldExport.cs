/*
 * 
 * This script was created to export any data from characters/guilds/alliances
 * from the GS to the website database, to increase performance of herald pages
 * and decrease the bandwidth from the GS.
 * 
 * Author: ZioRed
 * Date: 05/28/2007
 * Credits: Cisien for his DAoC Portal ServerListUpdate script as source for some informations
 * 
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using DOL.Database;
using DOL.Events;
using DOL.GS.PacketHandler;
using log4net;

namespace DOL.GS.GameEvents
{
	public class HeraldExport
	{
		private static readonly string URL_PAGE = "http://www.dolserver.net/portal/play/herald/server_update.php";

		#region Tick intervals/retries configuration
		/// <summary>
		/// First run tick interval
		/// </summary>
		private static readonly long FIRSTRUN = 15 * 1000;
		/// <summary>
		/// Export tick interval
		/// </summary>
		private static readonly long INTERVAL = 3 * 60 * 60 * 1000;
		/// <summary>
		/// Retry interval if too many clients are connected
		/// </summary>
		private static readonly long INTERVAL_RETRY = 5 * 60 * 1000;
		/// <summary>
		/// Export player queue tick interval
		/// </summary>
		private static readonly long INTERVAL_PLAYERS_QUEUE = 1000;
		/// <summary>
		/// Max amount of connected clients to begin export
		/// </summary>
		private static readonly byte MAX_CLIENTS_TO_EXPORT = 5;
		/// <summary>
		/// Max amount of retries before forcing the export
		/// </summary>
		private static readonly byte MAX_RETRIES = 3;
		#endregion

		/// <summary>
		// Holds the current number of retry
		/// </summary>
		private static byte m_total_retries;
		
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Defines the timer which holds the export tick.
		/// </summary>
		private static volatile Timer m_timer = null;

		/// <summary>
		/// Creates the thread which is used to export the herald.
		/// </summary>
		protected static Thread m_thread;

		private static volatile Timer m_timerPlayers = null;
		protected static Thread m_threadPlayers;
		protected static Queue<Character> m_players = new Queue<Character>();
		
		protected static Character m_player = null;
		
		[ScriptLoadedEvent]
		public static void OnScriptCompiled(DOLEvent e, object sender, EventArgs args)
		{
			if (log.IsDebugEnabled)
				log.Debug("Loading HeraldExport");
			m_total_retries = 0;
			m_timer = new Timer(new TimerCallback(StartExportThread), null, FIRSTRUN, 0);
			if (m_timer == null)
			{
				if (log.IsErrorEnabled)
					log.Error("HeraldExport timer failed to start. Stopping!");
			}
			else
			{
				log.Info("HeraldExport initialized");
				m_timerPlayers = new Timer(new TimerCallback(StartPlayersExportThread), null, 1000, 0);
				GameEventMgr.AddHandler(GamePlayerEvent.Quit, new DOLEventHandler(PlayerQuit));
			}
		}

		[ScriptUnloadedEvent]
		public static void OnScriptUnloaded(DOLEvent e, object sender, EventArgs args)
		{
			GameEventMgr.RemoveHandler(GamePlayerEvent.Quit, new DOLEventHandler(PlayerQuit));
			try
			{
				if (m_timer != null)
					m_timer = null;
			}
			catch (Exception ex)
			{
				if (log.IsErrorEnabled)
					log.Error("Herald Export error occured stopping timer: \r\n" + ex.ToString());
			}
			try
			{
				if (m_thread != null)
				{
					m_thread.Abort();
					m_thread = null;
				}
			}
			catch (Exception ex)
			{
				if (log.IsErrorEnabled)
					log.Error("Herald Export error occured stopping guilds thread: \r\n" + ex.ToString());
			}
			try
			{
				if (m_threadPlayers != null)
				{
					m_threadPlayers.Abort();
					m_threadPlayers = null;
				}
			}
			catch (Exception ex)
			{
				if (log.IsErrorEnabled)
					log.Error("Herald Export error occured stopping players thread: \r\n" + ex.ToString());
			}
		}
		
		/// <summary>
		/// Starts a new thread for export operations
		/// </summary>
		/// <param name="timer"></param>
		private static void StartExportThread(object timer)
		{
			m_thread = new Thread(new ThreadStart(ExportHerald));
			m_thread.Start();
		}
		
		/// <summary>
		/// Callback for the export operation thread
		/// </summary>
		private static void ExportHerald()
		{
			if (WorldMgr.GetAllPlayingClientsCount() > MAX_CLIENTS_TO_EXPORT)
			{
				// if MAX_RETRIES has been reached, then force the export
				if (m_total_retries == MAX_RETRIES)
					m_total_retries = 0;
				else
				{
					m_total_retries++;
					// change the interval to retry sooner
					m_timer.Change(INTERVAL_RETRY, Timeout.Infinite);
					if (log.IsDebugEnabled)
						log.Debug("Herald Export scheduled");
					return;
				}
			}
			try
			{
				bool error = false;
				int guildsCount = 0, alliancesCount = 0;
				List<string> guilds = new List<string>();
				List<string> alliances = new List<string>();
				DataObject[] exportData;
				
				// Export the guilds
				exportData = GameServer.Database.SelectAllObjects(typeof(DBGuild));
				foreach (DBGuild guild in exportData)
				{
					//Set up our URI to be passed to the WebClient.
					string Updater = UrlEncode(URL_PAGE +
						"?guild_id=" + guild.ObjectId + "&guildid=" + guild.GuildID + "&guildname=" + guild.GuildName +
						"&realmpoints=" + guild.RealmPoints + "&bountypoints=" + guild.BountyPoints +
						"&webpage=" + guild.Webpage + "&email=" + guild.Email + "&allianceid=" + guild.AllianceID);
					//log.Debug("Export guild: " + Updater);
					if (!HeraldUpdater(Updater))
					{
						if (log.IsErrorEnabled)
							log.Error("HeraldExport: error during export guild operation");
						break;
					}
					else
						guildsCount++;
					if (guild.AllianceID != "" && guild.AllianceID != null && !ExistsInList(alliances, guild.AllianceID))
						alliances.Add(guild.AllianceID);
				}

				// Export the alliances
				exportData = GameServer.Database.SelectObjects(typeof(DBAlliance), "GuildAlliance_ID IN ('" + string.Join("','", alliances.ToArray()) + "')");
				foreach (DBAlliance alliance in exportData)
				{
					//Set up our URI to be passed to the WebClient.
					string Updater = UrlEncode(URL_PAGE +
						"?allianceid=" + alliance.ObjectId + "&alliancename=" + alliance.AllianceName);
					//log.Debug("Export alliance: " + Updater);
					if (!HeraldUpdater(Updater))
					{
						if (log.IsErrorEnabled)
							log.Error("HeraldExport: error during export guild alliance operation");
						break;
					}
					else
						alliancesCount++;
				}
				
				if (!error)
					log.WarnFormat("HeraldExport: exported {0} guilds, {1} alliances", guildsCount, alliancesCount);
			}
			catch (Exception ex)
			{
				log.Warn("Herald export exited with the following error: " + ex.Message + "\nTracktrace: " + ex.StackTrace);
			}
			finally
			{
				m_timer.Change(INTERVAL, Timeout.Infinite);
			}
		}

		/// <summary>
		/// Event handler fired when player leaves the game
		/// </summary>
		/// <param name="e"></param>
		/// <param name="sender"></param>
		/// <param name="arguments"></param>
		private static void PlayerQuit(DOLEvent e, object sender, EventArgs arguments)
		{
			GamePlayer player = sender as GamePlayer;
			if (player == null) return;
			m_players.Enqueue(player.PlayerCharacter);
			if (m_players.Count == 1)
				m_timerPlayers.Change(INTERVAL_PLAYERS_QUEUE, 0);
		}

		/// <summary>
		/// Starts a new thread for export operations
		/// </summary>
		/// <param name="timer"></param>
		private static void StartPlayersExportThread(object timer)
		{
			if (m_players.Count == 0)
				return;
			m_threadPlayers = new Thread(new ThreadStart(ExportNextPlayer));
			m_threadPlayers.Start();
		}
		
		/// <summary>
		/// Export a single character to the website herald
		/// </summary>
		/// <param name="player">The player to export</param>
		/// <returns></returns>
		private static void ExportNextPlayer()
		{
			m_player = m_players.Dequeue();
			Character player = m_player;
			//Set up our URI to be passed to the WebClient.
			string Updater = UrlEncode(URL_PAGE +
				"?dolcharacters_id=" + player.ObjectId + "&charname=" + player.Name + "&lastname=" + player.LastName +
				"&realm=" + player.Realm + "&race=" + player.Race + "&gender=" + player.Gender +
				"&level=" + player.Level + "&class=" + player.Class +
				"&guildid=" + player.GuildID +
				"&realmpoints=" + player.RealmPoints + "&realmrank=" + player.RealmLevel + "&bountypoints=" + player.BountyPoints +
				"&creationdate=" + player.CreationDate.ToString("yyyy-MM-dd HH:mm") + "&lastplayed=" + player.LastPlayed.ToString("yyyy-MM-dd HH:mm") + "&playedtime=" + player.PlayedTime +
				"&serializedcraftingskills=" + player.SerializedCraftingSkills + "&killsdragon=" + player.KillsDragon +
				"&killsalbionplayers=" + player.KillsAlbionPlayers + "&killsmidgardplayers=" + player.KillsMidgardPlayers + "&killshiberniaplayers=" + player.KillsHiberniaPlayers +
				"&killsalbiondeathblows=" + player.KillsAlbionDeathBlows + "&killsmidgarddeathblows=" + player.KillsMidgardDeathBlows + "&killshiberniadeathblows=" + player.KillsHiberniaDeathBlows +
				"&killsalbionsolo=" + player.KillsAlbionSolo + "&killsmidgardsolo=" + player.KillsMidgardSolo + "&killshiberniasolo=" + player.KillsHiberniaSolo +
				"&capturedkeeps=" + player.CapturedKeeps + "&capturedtowers=" + player.CapturedTowers + "&deathspvp=" + player.DeathsPvP);
			//log.Debug("Export: " + Updater);
			if (!HeraldUpdater(Updater))
			{
				if (log.IsErrorEnabled)
					log.Error("HeraldExport: error during character export - char:" + player.Name);
			}
			if (m_players.Count > 0)
				m_timerPlayers.Change(INTERVAL_PLAYERS_QUEUE, 0);
		}

		/// <summary>
		/// This Method creates the web client and updates the herald using the values
		/// provided in the updateurl
		/// </summary>
		/// <param name="updateurl">A pre-formatted URI used to send params to the web server</param>
		/// <returns>returns true if successful</returns>
		private static bool HeraldUpdater(string updateurl)
		{
			try
			{
				WebClient webclient = new WebClient();
				Byte[] contentBuffer = webclient.DownloadData(updateurl);
				string result = Encoding.ASCII.GetString(contentBuffer).ToLower();
				if (result.IndexOf("success") != -1)
				{
					return true;
				}
				else
				{
					if (log.IsErrorEnabled && result != "")
						log.Error("HeraldExport error on export: " + updateurl);
					return false;
				}
			}
			catch (Exception ex)
			{
				if (log.IsErrorEnabled)
					log.Error("HeraldExport Error Occured: " + ex.Message);
				return false;
			}
		}

		/// <summary>
		/// This method makes the URL friendly (replaces spaces and other html characters
		/// with their url friendly counterparts.
		/// </summary>
		/// <param name="Url">url to be formatted</param>
		/// <returns>friendly url</returns>
		public static string UrlEncode(string Url)
		{
			string newUrl = String.Empty;
			newUrl = Url.Replace(" ", "%20");
			return newUrl;
		}
		
		/// <summary>
		/// Check if the new item already exists in the list
		/// </summary>
		/// <param name="list">List of items to check</param>
		/// <param name="item">New item to add</param>
		/// <returns></returns>
		private static bool ExistsInList (List<string> list, string item)
		{
			foreach (string s in list)
			{
				if (s.ToLower() == item.ToLower())
					return true;
			}
			return false;
		}
	}
}
