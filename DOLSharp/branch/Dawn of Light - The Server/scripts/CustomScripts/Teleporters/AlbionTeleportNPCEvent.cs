/*
 * Author:		Kristopher Gilbert <ogre@fallenrealms.net>
 * Rev:			$Id: AlbionTeleportNPCEvent.cs 456 2005-05-09 12:33:51Z shagz $
 * Copyright:	2004 by Hired Goons, LLC
 * License:		http://www.gnu.org/licenses/gpl.txt
 * 
 */

using System;
using System.Collections;
using DOL.Events;

namespace DOL.GS.GameEvents
{
	public class AlbionTeleportNPCEvent
	{
		protected static ArrayList m_locs = new ArrayList();
		protected static ArrayList m_npcs = new ArrayList();

		[ScriptLoadedEvent]
		public static void OnScriptCompiled(DOLEvent e, object sender, EventArgs args)
		{
			//create locations for npcs
			m_locs.Add(new Location("Adribard's Retreat", 1, 470325, 630354, 1712, 795));
			m_locs.Add(new Location("Caer Ulfwych", 1, 520423, 617027, 1807, 49067));
			m_locs.Add(new Location("Caer Witrin", 1, 436302, 651214, 2448, 2910));
			m_locs.Add(new Location("Camelot", 10, 35504, 24790, 8751, 1529));
			m_locs.Add(new Location("Church of Albion", 1, 505162, 494342, 2495, 2865));
			m_locs.Add(new Location("Cotswold", 1, 560427, 511757, 2280, 2213));
			m_locs.Add(new Location("Cornwall Station", 1, 408939, 652597, 4944, 1430));
			m_locs.Add(new Location("Camelot Hills", 1, 585313, 531656, 2072, 3698));
			m_locs.Add(new Location("Humberton", 1, 508719, 477345, 2284, 3467));
			m_locs.Add(new Location("Lethantis Association", 1, 500282, 590150, 1872, 1402));
			m_locs.Add(new Location("Ludlow", 1, 531454, 479586, 2200, 5979));
			m_locs.Add(new Location("Llyn Barfog", 1, 478362, 392883, 4857, 3384));
			m_locs.Add(new Location("Prydwen Keep", 1, 574292, 529669, 2906, 3703));
			m_locs.Add(new Location("Snowdonia Station", 1, 522958, 407959, 4184, 3810));
			m_locs.Add(new Location("Swanton Keep", 1, 511616, 382418, 7992, 6517));
			m_locs.Add(new Location("West Downs", 1, 577661, 557185, 2188, 5364));
			m_locs.Add(new Location("Yarley's Farm", 1, 370129, 678916, 5593, 8086));
			//m_locs.Add(new Location("Albion Housing", 2, 572048, 569356, 3520, 736));

			TeleportNPCUtility.CreateTeleporters(1, m_locs, "AlbionTeleportNPCEvent", m_npcs);
		}
		[ScriptUnloadedEvent]
		public static void OnScriptUnloaded(DOLEvent e, object sender, EventArgs args)
		{
			//utility to remove them from the world
			TeleportNPCUtility.Stop(m_npcs, m_locs);
		}
	}
}
