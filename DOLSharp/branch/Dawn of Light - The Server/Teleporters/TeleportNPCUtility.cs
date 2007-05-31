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
			try
			{
				foreach (TeleportNPC npc in npcs)
					npc.AddToWorld();
				if (log.IsInfoEnabled)
					log.Info(eventname + " initialized");
			}
			catch
			{
				if (log.IsErrorEnabled)
					log.Error(eventname + " failed to initialize");
				return false;
			}
			return true;
		}
   
		public static bool Stop(ArrayList npcs, ArrayList locs)
		{
			//remove npcs
			foreach (TeleportNPC npc in npcs)
			{
				npc.m_locs.Clear();
				npc.Delete();
			}
			npcs.Clear();
			//remove locs
			foreach (ITeleportLocation location in locs)
			{
				if (location is GameLocation == false)
					continue;
				(location as Location).Delete();
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

			foreach (ITeleportLocation location in locs)
			{
				if (location is Location == false)
					continue;
				//add npcs to the npc collection
				Location loc = (Location)location;
				m_npcs.Add(new TeleportNPC(loc.CurrentRegionID, loc.X, loc.Y, loc.Z, loc.Heading,
					model, realm, name, guild, locs, locs.IndexOf(location), message));
			}
			Start(m_npcs, locs, eventName);
		}
	}
}
