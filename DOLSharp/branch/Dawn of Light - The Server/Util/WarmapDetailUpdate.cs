/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */
using System;
using System.Collections.Generic;
using System.Text;
using DOL.Events;
using System.Collections;
using DOL.GS.PacketHandler;
using System.Threading;
using System.Reflection;
using log4net;

namespace DOL.GS.Scripts
{
	public class WarmapDetailUpdate
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static byte[, ,] group = new byte[4, 4, 4];
		private static byte[, ,] fight = new byte[4, 4, 4];
		public const ushort REGIONID = 163;
		public const ushort UPDATEINTERVAL = 90; // secs
		private static Timer m_collecttimer;
		private static Timer m_checktimer;


		[ScriptUnloadedEvent]
		public static void OnScriptUnloaded(DOLEvent e, object sender, EventArgs args)
		{
			m_collecttimer.Change(Timeout.Infinite, Timeout.Infinite);
		}

		[ScriptLoadedEvent]
		public static void OnScriptLoaded(DOLEvent e, object sender, EventArgs args)
		{
			m_collecttimer = new Timer(new TimerCallback(CollectData), null, UPDATEINTERVAL * 1000, UPDATEINTERVAL * 1000);
			m_checktimer = new Timer(new TimerCallback(Check), null, Timeout.Infinite, Timeout.Infinite);
			Start();
		}

		public static void CollectData(object state)
		{
			int callbackStart = Environment.TickCount;
			fillRegionData(REGIONID);
			if (Environment.TickCount - callbackStart > 100)
			{
				if (log.IsWarnEnabled)
				{
					string warning = "callback took " + (Environment.TickCount - callbackStart) + "ms! WarmapDetailUpdate.CollectData()";
					log.Warn(warning);
				}
			}
		}

		public static void Check(object state)
		{
			int callbackStart = Environment.TickCount;
			scanRegion();
			if (Environment.TickCount - callbackStart > 500)
			{
				if (log.IsWarnEnabled)
				{
					string warning = "callback took " + (Environment.TickCount - callbackStart) + "ms! WarmapDetailUpdate.Check()";
					log.Warn(warning);
				}
			}
		}

		public static void Start()
		{
			m_checktimer.Change(UPDATEINTERVAL * 1000, UPDATEINTERVAL * 1000);
		}

		public static void Stop()
		{
			m_checktimer.Change(Timeout.Infinite, Timeout.Infinite);
		}

		/// <summary>
		/// get data from one zoneid 
		/// </summary>
		/// <param name="zoneid"></param>
		public static void fillRegionData(ushort regionID)
		{
			try
			{
				int xdiff = (492000 - 420000) / 4;
				int ydiff = (556000 - 492000) / 4;

				group = new byte[4, 4, 4];
				fight = new byte[4, 4, 4];

				foreach (GameClient client in WorldMgr.GetClientsOfRegion(REGIONID))
				{
					byte index_x = (byte)Math.Max(0, Math.Min(3, (client.Player.X - 420000) / xdiff));
					byte index_y = (byte)Math.Max(0, Math.Min(3, (client.Player.Y - 492000) / ydiff));

					if ((client.Account.PrivLevel >= 1) &&
						(client.Player.Realm <= (byte)eRealm._LastPlayerRealm) &&
						(client.Player.Realm >= (byte)eRealm._FirstPlayerRealm))
					{
						if (client.Player.LastAttackTick + 30000 >= client.Player.CurrentRegion.Time ||
							client.Player.LastAttackedByEnemyTick + 30000 >= client.Player.CurrentRegion.Time)
						{
							fight[index_x, index_y, client.Player.Realm] += 1;
						}
						group[index_x, index_y, client.Player.Realm] += 1;
					}
				}

				for (byte x = 0; x < 4; x++)
					for (byte y = 0; y < 4; y++)
					{
						if (getRealmWarFlag(fight[x, y, (byte)eRealm.Albion],
											fight[x, y, (byte)eRealm.Midgard],
											fight[x, y, (byte)eRealm.Hibernia])
							!= -1)
						{
							// if war is on sector erase group data from this sector
							group[x, y, (byte)eRealm.Albion] = 0;
							group[x, y, (byte)eRealm.Midgard] = 0;
							group[x, y, (byte)eRealm.Hibernia] = 0;
						}
					}
			}
			catch (Exception e)
			{
				log.Error(e);
			}
		}

		/// <summary>
		/// 0x00 all 3 realms fights in sector,
		/// 0x01 redblue in sector,
		/// 0x02 redgreen in sector,
		/// 0x03 bluegren in sector, 
		/// -1 realm no fights
		/// </summary>
		/// <param name="realm1"></param>
		/// <param name="realm2"></param>
		/// <param name="realm3"></param>
		/// <returns></returns>
		private static int getRealmWarFlag(byte realm1, byte realm2, byte realm3)
		{
			if (realm1 != 0)
			{
				if ((realm2 != 0) && (realm3 == 0))
					return 0x01;
				if ((realm2 == 0) && (realm3 != 0))
					return 0x02;
				if ((realm2 != 0) && (realm3 != 0))
					return 0x00;
			}
			else
			{
				if ((realm2 != 0) && (realm3 != 0))
					return 0x03;
			}
			return -1;
		}

		private static List<List<byte>> getFightList(ushort zoneid)
		{
			List<List<byte>> list = new List<List<byte>>();


			for (byte x = 0; x < 4; x++)
				for (byte y = 0; y < 4; y++)
				{
					short status = (short)getRealmWarFlag(fight[x, y, (byte)eRealm.Albion],
												fight[x, y, (byte)eRealm.Midgard],
												fight[x, y, (byte)eRealm.Hibernia]);
					if (status != -1)
					{
						List<byte> sector = new List<byte>();
						sector.Add((byte)zoneid);
						sector.Add(x);
						sector.Add(y);
						sector.Add((byte)status);
						byte amount = (byte)((fight[x, y, (byte)eRealm.Albion] + fight[x, y, (byte)eRealm.Midgard] + fight[x, y, (byte)eRealm.Hibernia]) / 8 + 1);
						if (amount > 3)
							amount = 3;
						sector.Add(amount);
						list.Add(sector);
					}
				}

			return list;
		}
		/// <summary>
		/// create grouplist depends on player realm
		/// </summary>
		/// <param name="zoneid"></param>
		/// <param name="client"></param>
		/// <returns></returns>
		private static List<List<byte>> getGroupList(ushort zoneid, GameClient client)
		{
			List<List<byte>> list = new List<List<byte>>();
			for (byte x = 0; x < 4; x++)
				for (byte y = 0; y < 4; y++)
				{
					byte status = getBiggestGroup(group[x, y, (byte)eRealm.Albion],
												group[x, y, (byte)eRealm.Midgard],
												group[x, y, (byte)eRealm.Hibernia],
												((client.Account.PrivLevel > 1) ? (byte)0 : (byte)client.Player.Realm)
												);
					switch ((eRealm)status)
					{
						case eRealm.Albion:
							list.Add(getData(zoneid, x, y, status, group[x, y, status]));
							break;
						case eRealm.Midgard:
							list.Add(getData(zoneid, x, y, status, group[x, y, status]));
							break;
						case eRealm.Hibernia:
							list.Add(getData(zoneid, x, y, status, group[x, y, status]));
							break;
						default:
							break;
					}

				}
			return list;
		}

		/// <summary>
		/// create the datagram for grouplist
		/// </summary>
		/// <param name="zoneid"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="realm"></param>
		/// <param name="amount"></param>
		/// <returns></returns>
		private static List<byte> getData(ushort zoneid, byte x, byte y, byte realm, byte amount)
		{
			List<byte> sector = new List<byte>();
			sector.Add((byte)zoneid);
			sector.Add(x);
			sector.Add(y);
			sector.Add(realm);
			amount = (byte)((amount / 4) + 1);
			if (amount > 3)
				amount = 3;
			sector.Add(amount);

			return sector;
		}

		/// <summary>
		/// 0 - no group, 1 -Albion, 2- Midgard , 3- Hibernia
		/// </summary>
		/// <param name="realm1"></param>
		/// <param name="realm2"></param>
		/// <param name="realm3"></param>
		/// <param name="excluderealm">0- no exclude, 1- Albion,2-Midgard,3-Hibernia</param>
		/// <returns></returns>
		private static byte getBiggestGroup(byte realm1, byte realm2, byte realm3, byte excluderealm)
		{
			switch ((eRealm)excluderealm)
			{
				case eRealm.Albion:
					if (realm2 > realm3)
						return (byte)eRealm.Midgard;
					else
						if (realm3 == 0)
							return 0;
						else
							return (byte)eRealm.Hibernia;

				case eRealm.Midgard:
					if (realm1 > realm3)
						return (byte)eRealm.Albion;
					else
						if (realm3 == 0)
							return 0;
						else
							return (byte)eRealm.Hibernia;

				case eRealm.Hibernia:
					if (realm1 > realm2)
						return (byte)eRealm.Albion;
					else
						if (realm2 == 0)
							return 0;
						else
							return (byte)eRealm.Midgard;

				default:
					if (realm1 > realm2)
					{
						if (realm1 > realm3)
							return (byte)eRealm.Albion;
						else
							return (byte)eRealm.Hibernia;
					}
					else
					{
						if (realm2 > realm3)
							return (byte)eRealm.Midgard;
						else
						{
							if (realm3 == 0)
								return 0;
							else
								return (byte)eRealm.Hibernia;
						}
					}

			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="client"></param>
		/// <param name="check"></param>
		/// <returns>bit 1 fights, bit 2 groups</returns>
		public static byte DisplayMap(GameClient client)
		{
			if (client.Account.PrivLevel > 1)
				return 0x03;
			return 0x00;
		}

		/// <summary>
		/// Testmethod for collect warmapdata
		/// </summary>
		public static void scanRegion()
		{
			List<List<byte>> fights = getFightList(REGIONID);
			foreach (GameClient client in WorldMgr.GetClientsOfRegion(REGIONID))
			{

				switch (DisplayMap(client))
				{
					case 0x00:
						client.Out.SendWarmapDetailUpdate(new List<List<byte>>(), new List<List<byte>>());
						break;
					case 0x01:
						client.Out.SendWarmapDetailUpdate(fights, new List<List<byte>>());
						break;
					case 0x02:
						client.Out.SendWarmapDetailUpdate(new List<List<byte>>(), getGroupList(REGIONID, client));
						break;
					case 0x03:
						client.Out.SendWarmapDetailUpdate(fights, getGroupList(REGIONID, client));
						break;
				}
			}
		}
	}
}
