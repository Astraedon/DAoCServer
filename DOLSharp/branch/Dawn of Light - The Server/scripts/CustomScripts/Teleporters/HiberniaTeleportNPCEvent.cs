/*
 * Author:		Kristopher Gilbert <ogre@fallenrealms.net>
 * Rev:			$Id: HiberniaTeleportNPCEvent.cs 456 2005-05-09 12:33:51Z shagz $
 * Copyright:	2004 by Hired Goons, LLC
 * License:		http://www.gnu.org/licenses/gpl.txt
 * 
 */

using System;
using System.Collections;
using DOL.Events;

namespace DOL.GS.GameEvents
{
	public class HiberniaTeleportNPCEvent
	{
		protected static ArrayList m_locs = new ArrayList();
		protected static ArrayList m_npcs = new ArrayList();

		[ScriptLoadedEvent]
		public static void OnScriptCompiled(DOLEvent e, object sender, EventArgs args)
		{
			//create locations for npcs
			m_locs.Add(new Location("Ardagh", 200, 351238, 553743, 5104, 2610));
			m_locs.Add(new Location("Ardee", 200, 339763, 467363, 5200, 3981));
			m_locs.Add(new Location("Brynach", 200, 409831, 527727, 2112, 132));
			m_locs.Add(new Location("Connla", 200, 295723, 642545, 4848, 2026));
			m_locs.Add(new Location("Culraid", 200, 298606, 688040, 4949, 3916));
			m_locs.Add(new Location("Howth", 200, 342582, 592316, 5456, 4946));
			m_locs.Add(new Location("Mag Mel", 200, 345736, 490868, 5200, 4262));
			m_locs.Add(new Location("Murdagh", 200, 353051, 517098, 5077, 640));
			m_locs.Add(new Location("Tir na mBeo", 200, 345038, 528249, 5448, 923));
			m_locs.Add(new Location("Tir na Nog", 201, 33672, 32305, 7999, 1497));

			TeleportNPCUtility.CreateTeleporters(3, m_locs, "HiberniaTeleportNPCEvent", m_npcs);
		}

		[ScriptUnloadedEvent]
		public static void OnScriptUnloaded(DOLEvent e, object sender, EventArgs args)
		{
			//utility to remove them from the world
			TeleportNPCUtility.Stop(m_npcs, m_locs);
		}
	}
}
