/*
 * Author:		Kristopher Gilbert <ogre@fallenrealms.net>
 * Rev:			$Id: TeleportNPCUtility.cs 456 2005-05-09 12:33:51Z shagz $
 * Copyright:	2004 by Hired Goons, LLC
 * License:		http://www.gnu.org/licenses/gpl.txt
 * 
 * Utility class for Teleporter NPC's.
 * 
 */

using System.Collections;

using log4net;

namespace DOL.GS.GameEvents
{
	public class TeleportNPCUtility
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public static bool Start(ArrayList npcs, ArrayList locs, string eventname)
		{
			//add npcs to world
			bool good = true;
			for (int i = 0; i != npcs.Count; i++) 
			{
				((TeleportNPC)npcs[i]).AddToWorld();
			}
			if (log.IsInfoEnabled)
				log.Info(eventname + " initialized");
			return good;
		}
   
		public static bool Stop(ArrayList npcs, ArrayList locs)
		{
			//remove npcs
			for (int i = 0; i != npcs.Count; i++)
			{
				TeleportNPC npc = (TeleportNPC)npcs[i];
				npc.m_locs.Clear();
				npc.Delete();
			}
			npcs.Clear();
			//remove locs
			for (int i = 0; i != locs.Count; i++)
			{
				Location location = (Location)locs[i];
				location.Delete();
			}
			locs.Clear();
			return true;
		}

		public static void CreateTeleporters(byte realm, ArrayList locs, string eventName, ArrayList m_npcs)
		{
			ushort model = 0;
			string name = "";
			string guild = "Ethereal Translocation Services";
			string message = "";

			switch (realm)
			{
				case 1:
					{
						model = 14;
						name = "Albion Translocater";
						message = "I can teleport you to various towns around Albion. " +
							"Which town would you like me to teleport you to?\n\n";
						break;
					}
				case 2:
					{
						model = 153;
						name = "Midgard Translocater";
						message = "I can teleport you to various towns around Midgard. " +
							"Which town would you like me to teleport you to?\n\n";
						break;
					}
				case 3:
					{
						model = 302;
						name = "Hibernia Translocater";
						message = "I can teleport you to various towns around Hibernia. " +
							"Which town would you like me to teleport you to?\n\n";
						break;
					}
			}

			for (int i = 0; i != locs.Count; i++)
			{
				//add npcs to the npc collection
				Location loc = (Location)locs[i];
				m_npcs.Add(new TeleportNPC(loc.CurrentRegionID, loc.X, loc.Y, loc.Z, loc.Heading,
					model, realm, name, guild, locs, i, message));
			}
			Start(m_npcs, locs, eventName);
		}
	}
}
