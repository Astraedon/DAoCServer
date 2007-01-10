using System;
using System.Net;
using System.Collections;
using System.Reflection;
using IRC;
using log4net;

using DOL;
using DOL.Database;
using DOL.GS;
using DOL.GS.Keeps;
using DOL.GS.PacketHandler;
using DOL.Events;

namespace DOL.GS.Scripts
{
	public class ServerIRC
	{
		public static IRCClient IRCBot = null;
		private const string CHANNEL = "#dolplay";

		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		[GameServerStartedEvent]
		public static void OnScriptLoaded(DOLEvent e, object sender, EventArgs args)
		{
			if (GameServer.Instance.Configuration.ServerNameShort == "Etaew")
				return;
			log.Info("Starting IRC Bot");

			IRCBot = new IRCClient();
			IRCBot.Nick = "DOLBot";
			IRCBot.Ident = IRCBot.Nick.ToLower();
			IRCBot.RealName = IRCBot.Nick;

			IRCBot.OnConnect += new IRCClient.Handler(IRCBot_OnConnect);
			IRCBot.OnConnectFailed += new IRCClient.ErrorHandler(IRCBot_OnConnectFailed);
			IRCBot.OnMessage += new IRCClient.MessageHandler(IRCBot_OnMessage);
			IRCBot.OnJoin += new IRCClient.JoinHandler(IRCBot_OnJoin);

			GameEventMgr.AddHandler(KeepEvent.KeepTaken, new DOLEventHandler(KeepTaken));
			GameEventMgr.AddHandler(KeepEvent.TowerRaized, new DOLEventHandler(TowerRaized));

			IRCBot.Connect("irc.quakenet.org", 6667);
		}

		static void IRCBot_OnJoin(IRCClient sender, Client clt, Channel chan)
		{
			if (clt.Nick == IRCBot.Nick)
				IRCBot.SendMessage(CHANNEL, GameServer.Instance.Configuration.ServerName + " started!");
			else IRCBot.SendNotice(clt.Nick, "Welcome to " + GameServer.Instance.Configuration.ServerName + " IRC you can look at my command list with !list");
		}

		static void IRCBot_OnMessage(IRCClient sender, Client source, object target, string message)
		{
			string[] data = message.Split(' ');
			switch (data[0])
			{
				case "!rw":
				case "!realmwar":
				case "!realm":
					{
						string msg = "Realm Status: ";

						AbstractGameKeep thidranki = KeepMgr.getKeepByID(129);
						AbstractGameKeep orseo = KeepMgr.getKeepByID(5);

						int alb = 0, mid = 0, hib = 0;
						if (thidranki != null)
						{
							msg += thidranki.Name + ": " + GlobalConstants.RealmToName((eRealm)thidranki.Realm);
							alb = WorldMgr.GetClientsOfRegionCount((ushort)thidranki.Region, 1);
							mid = WorldMgr.GetClientsOfRegionCount((ushort)thidranki.Region, 2);
							hib = WorldMgr.GetClientsOfRegionCount((ushort)thidranki.Region, 3);
							msg += " Players: Alb (" + alb + ") Mid (" + mid + ") Hib (" + hib + ") Total (" + (alb + mid + hib) + ")";
							IRCBot.SendMessage(CHANNEL, msg);
						}

						if (orseo != null)
						{
							msg = orseo.Name + ": " + GlobalConstants.RealmToName((eRealm)orseo.Realm);
							alb = WorldMgr.GetClientsOfRegionCount((ushort)orseo.Region, 1);
							mid = WorldMgr.GetClientsOfRegionCount((ushort)orseo.Region, 2);
							hib = WorldMgr.GetClientsOfRegionCount((ushort)orseo.Region, 3);
							msg += " Players: Alb (" + alb + ") Mid (" + mid + ") Hib (" + hib + ") Total (" + (alb + mid + hib) + ")";
							IRCBot.SendMessage(CHANNEL, msg);
						}

						if (orseo != null)
						{
							msg = ("Darkness Falls: " + GlobalConstants.RealmToName((eRealm)orseo.Realm));
							alb = WorldMgr.GetClientsOfRegionCount(249, 1);
							mid = WorldMgr.GetClientsOfRegionCount(249, 2);
							hib = WorldMgr.GetClientsOfRegionCount(249, 3);
							msg += " Players: Alb (" + alb + ") Mid (" + mid + ") Hib (" + hib + ") Total (" + (alb + mid + hib) + ")";
						}

						break;
					}
				case "!stats":
				case "!status":
					{
						long uptime = GameServer.Instance.TickCount;
						long sec = uptime / 1000;
						long min = sec / 60;
						long hours = min / 60;
						long days = hours / 24;
						int all = WorldMgr.GetAllClients().Count;
						int alb = WorldMgr.GetClientsOfRealmCount(1);
						int mid = WorldMgr.GetClientsOfRealmCount(2);
						int hib = WorldMgr.GetClientsOfRealmCount(3);

						string clientstr = "Clients: " + all;
						string albstr = "Albion: " + alb;
						if (all > 0)
							albstr = albstr + " (" + (alb * 100 / all) + "%)";
						else albstr = albstr + " (100%)";
						string midstr = "Midgard: " + mid;
						if (all > 0)
							midstr = midstr + " (" + (mid * 100 / all) + "%)";
						else midstr = midstr + " (100%)";
						string hibstr = "Hibernia: " + hib;
						if (all > 0)
							hibstr = hibstr + " (" + (hib * 100 / all) + "%)";
						else hibstr = hibstr + " (100%)";

						int loading = all - alb - mid - hib;
						string loadingstr = "Loading: " + loading;
						if (all > 0)
							loadingstr = loadingstr + " (" + (loading * 100 / all) + "%)";
						else loadingstr = loadingstr + " (100%)";

						int staff = 0;

						foreach (GameClient client in WorldMgr.GetAllPlayingClients())
						{
							if (client.Account.PrivLevel > 1)
								staff++;
						}

						string staffstr = "Staff: " + staff;

						clientstr = clientstr + " " + albstr + " " + midstr + " " + hibstr + " " + loadingstr + " " + staffstr;

						IRCBot.SendMessage(CHANNEL, clientstr + " Uptime: " + string.Format("{0}d {1}h {2}m {3:00}s", days, hours % 24, min % 60, sec % 60));
						break;
					}
				case "!ip":
					{
						//String strHostName = Dns.GetHostName();

						// Find host by name
						//IPHostEntry iphostentry = Dns.GetHostByName(strHostName);

						// Enumerate IP addresses
						/*
						foreach (IPAddress ipaddress in iphostentry.AddressList)
						{
							IRCBot.SendNotice(source.Nick, "IP: " + ipaddress.ToString() + " PORT: 10300");
							break;
						}*/
						IRCBot.SendNotice(source.Nick, "IP: server.dolserver.net");
						//daoc://ip.or.hostname.com:10300
						break;
					}
				case "!lookup":
					{
						if (data.Length < 2)
							break;

						string name = data[1];
						Character c = (Character)GameServer.Database.SelectObject(typeof(Character), "Name = '" + GameServer.Database.Escape(name) + "'");
						string msg = "";
						if (c == null)
							msg = name + " not found in the database!";
						else
						{
							string namestr = c.Name;
							if (c.LastName != "")
								namestr = namestr + " " + c.LastName;

							string guildstr = "";
							if (c.GuildID == "")
								guildstr = "With no guild alliegance ";
							else
							{
								guildstr = "in " + GuildMgr.GetGuildByGuildID(c.GuildID).Name +" ";
							}

							string rrstr = "" + (((float)c.RealmLevel) / 10 + 1);
							string[] rrstrTwo = new string[1];
							rrstrTwo = rrstr.Trim().Split('.');

							if (rrstrTwo.GetUpperBound(0) == 1)
								rrstr = rrstrTwo[0] + "L" + rrstrTwo[1];
							else
								rrstr = rrstrTwo[0] + "L0";

							rrstrTwo = null;

							string killsstr = (c.KillsAlbionPlayers + c.KillsHiberniaPlayers + c.KillsMidgardPlayers) + " Total kill(s), " + (c.KillsAlbionSolo + c.KillsHiberniaSolo + c.KillsMidgardSolo) + " Solo kill(s)";

							string classstr = ((eCharacterClass)c.Class).ToString();

							msg = string.Format("{0}, the RR {1} {2}, {3}with {4} and {5} RPs", c.Name, rrstr, classstr, guildstr, killsstr, c.RealmPoints);
						}
						//<firstname> <lastname>, the realm rank <rank> <classname>, in <guild> with <totalkills> kills and <playing status>
						IRCBot.SendMessage(CHANNEL, msg);
						break;
					}
				case "!list":
					{
						IRCBot.SendNotice(source.Nick, "!realm, !stats, !ip, !lookup, !list");
						break;
					}
				case "!auth":
					{
						IRCBot.SendWHOIS(source);
						break;
					}
				case "!announce":
					{
						if (source.Auth == "")
						{
							IRCBot.SendNotice(source.Nick, "You need to be authed to use this command");
							break;
						}

						Account acc = (Account)GameServer.Database.FindObjectByKey(typeof(Account), source.Auth);
						if (acc == null)
						{
							IRCBot.SendNotice(source.Nick, "Account " + source.Auth + " not found in the DB");
							break;
						}
						if (acc.PrivLevel < 3)
							break;

						message = message.Replace(data[0] + " ", "");
						foreach (GameClient client in WorldMgr.GetAllPlayingClients())
						{
							IList textList = new ArrayList();
							textList.Add(source.Nick + " Broadcasts: ");
							textList.Add("");
							textList.Add(message);
							client.Player.Out.SendCustomTextWindow("Broadcast", textList);
						}
						IRCBot.SendNotice(source.Nick, "\"" + message + "\" sent");
						break;
					}
			}

			/*
			if (source.Nick != IRCBot.Nick && message.StartsWith("!") == false)
			{
				foreach (GameClient client in WorldMgr.GetAllPlayingClients().Clone() as ArrayList)
				{
					bool ircon = client.Player.TempProperties.getProperty("IRC", false);
					if (ircon)
					{
						client.Out.SendMessage("[IRC] " + source.Nick + ": " + message, eChatType.CT_Alliance, eChatLoc.CL_ChatWindow);
					}
				}
			}
			 */
		}

		static void IRCBot_OnConnectFailed(IRCClient sender, Exception e)
		{
			log.Info("(Connect Failed)Sender: " + sender.ToString() + " exception: " + e.ToString());
		}

		static void IRCBot_OnConnect(IRCClient sender)
		{
			IRCBot.SendJoin(CHANNEL);
		}

		static void KeepTaken(DOLEvent e, object sender, EventArgs args)
		{
			KeepEventArgs kargs = args as KeepEventArgs;
			AbstractGameKeep keep = kargs.Keep;
			IRCBot.SendMessage(CHANNEL, string.Format("The forces of {0} have captured {1}!", GlobalConstants.RealmToName((eRealm)keep.Realm), keep.Name));
		}

		static void TowerRaized(DOLEvent e, object sender, EventArgs args)
		{
			KeepEventArgs kargs = args as KeepEventArgs;
			AbstractGameKeep keep = kargs.Keep;

			IRCBot.SendMessage(CHANNEL, string.Format("{1} has been razed by the forces of {0}!", GlobalConstants.RealmToName((eRealm)kargs.Realm), keep.Name));
		}

		public static void SendMessageToChannel(string message)
		{
			IRCBot.SendMessage(CHANNEL, message);
		}
	}
/*
	[CmdAttribute(
"&ircon",
1,
"turns on irc chat")]
	public class IRCOnCommandHandler : ICommandHandler
	{
		public int OnCommand(GameClient client, string[] args)
		{
			client.Player.TempProperties.setProperty("IRC", true);
			client.Out.SendMessage("IRC ON - You will now be able to send and receive IRC messages via /irc <message> to turn off this feature use /ircoff", eChatType.CT_System, eChatLoc.CL_SystemWindow);
			return 1;
		}
	}

	[CmdAttribute(
"&ircoff",
1,
"turns off irc chat")]
	public class IRCOffCommandHandler : ICommandHandler
	{
		public int OnCommand(GameClient client, string[] args)
		{
			client.Player.TempProperties.setProperty("IRC", false);
			client.Out.SendMessage("IRC OFF - You will no longer be able to send and receive IRC messages to turn on this feature use /ircon", eChatType.CT_System, eChatLoc.CL_SystemWindow);
			return 1;
		}
	}

	[CmdAttribute(
"&irc",
1,
"sends an irc message")]
	public class IRCCommandHandler : ICommandHandler
	{
		public int OnCommand(GameClient client, string[] args)
		{
			bool ircon = client.Player.TempProperties.getProperty("IRC", false);
			if (ircon)
			{
				if (args.Length < 2)
				{
					client.Out.SendMessage("You must say something...", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					return 1;
				}

				string message = string.Join(" ", args, 1, args.Length - 1);

				AftermathIRC.SendMessageToChannel("[Game] " + client.Player.Name + ": " + message);
				foreach (GameClient client in WorldMgr.GetAllPlayingClients().Clone() as ArrayList)
				{
					bool ircon = client.Player.TempProperties.getProperty("IRC", false);
					if (ircon)
					{
						client.Out.SendMessage("[IRC] " + source.Nick + ": " + message, eChatType.CT_Alliance, eChatLoc.CL_ChatWindow);
					}
				}
			}
			else
				client.Out.SendMessage("To send an IRC message you must first turn IRC on by using /ircon", eChatType.CT_System, eChatLoc.CL_SystemWindow);
			return 1;
		}
	}*/
}