/*
 * Author:		Kristopher Gilbert <ogre@fallenrealms.net>
 * Rev:			$Id: MidgardTeleportNPCEvent.cs 456 2005-05-09 12:33:51Z shagz $
 * Copyright:	2004 by Hired Goons, LLC
 * License:		http://www.gnu.org/licenses/gpl.txt
 * 
 */

using System;
using System.Collections;
using DOL.Events;

namespace DOL.GS.GameEvents
{
	public class MidgardTeleportNPCEvent
	{

		protected static ArrayList m_locs = new ArrayList();
		protected static ArrayList m_npcs = new ArrayList();

		[ScriptLoadedEvent]
		public static void OnScriptCompiled(DOLEvent e, object sender, EventArgs args)
		{
			//create locations for npcs
			m_locs.Add(new Location("Audliten", 100, 724990, 760045, 4528, 2911));
			m_locs.Add(new Location("Dvalin", 100, 726635, 789810, 4600, 37840)); // no bind
			m_locs.Add(new Location("Fort Atla", 100, 749299, 815853, 4408, 4219));
			m_locs.Add(new Location("Fort Veldon", 100, 800159, 678494, 5304, 4862));
			m_locs.Add(new Location("Galplen", 100, 798320, 892901, 4744, 825));
			m_locs.Add(new Location("Gna Faste", 100, 787693, 903909, 4744, 4129));
			m_locs.Add(new Location("Haggerfel", 100, 805532, 700591, 4960, 4152));
			m_locs.Add(new Location("Huginfel", 100, 712753, 783853, 4672, 5034));
			m_locs.Add(new Location("Jordheim", 101, 31746, 27104, 8814, 0));
			m_locs.Add(new Location("Mularn", 100, 804508, 724289, 4680, 4097));
			m_locs.Add(new Location("Nalliten", 100, 770332, 837474, 4624, 132));
			m_locs.Add(new Location("Svasud Faste", 100, 764362, 675133, 5722, 4083));
			m_locs.Add(new Location("Vasudheim", 100, 774936, 755183, 4600, 4493));
			m_locs.Add(new Location("West Skona", 100, 711965, 924393, 5063, 1824));
			m_locs.Add(new Location("Raumarik", 100, 660811, 764919, 4614, 816));

			TeleportNPCUtility.CreateTeleporters(2, m_locs, "MidgardTeleportNPCEvent", m_npcs);
		}

		[ScriptUnloadedEvent]
		public static void OnScriptUnloaded(DOLEvent e, object sender, EventArgs args)
		{
			//utility to remove them from the world
			TeleportNPCUtility.Stop(m_npcs, m_locs);
		}
	}
}
