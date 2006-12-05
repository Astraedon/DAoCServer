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
	public class RvRTeleportNPCEvent
	{
		protected static ArrayList m_albLocs = new ArrayList();
		protected static ArrayList m_midLocs = new ArrayList();
		protected static ArrayList m_hibLocs = new ArrayList();
		protected static ArrayList m_albNpcs = new ArrayList();
		protected static ArrayList m_midNpcs = new ArrayList();
		protected static ArrayList m_hibNpcs = new ArrayList();

		[ScriptLoadedEvent]
		public static void OnScriptCompiled(DOLEvent e, object sender, EventArgs args)
		{
			//create locations for npcs
			m_albLocs.Add(new Location("Camelot", 10, 35253, 25005, 8751, 1549));
			m_albLocs.Add(new LocationExpansion("Thidranki [20-24]", 238, 563466, 574358, 5408, 2121, GameClient.eClientType.ShroudedIsles, GameClient.eClientAddons.NewFrontiers, GameClient.eClientVersion.Version170, 20, 24));
			//m_albLocs.Add(new LocationExpansion("Molvik [35-39]", 238, 563466, 574358, 5408, 2121, GameClient.eClientType.ShroudedIsles, GameClient.eClientAddons.NewFrontiers, GameClient.eClientVersion.Version170, 20, 24));
			//m_albLocs.Add(new LocationExpansion("Leirvik [40-44]", 165, 583159, 585478, 4896, 2417, GameClient.eClientType.ShroudedIsles, GameClient.eClientAddons.NewFrontiers, GameClient.eClientVersion.Version185, 40, 44));
			m_albLocs.Add(new LocationExpansion("Cathal Valley [45+]", 165, 583159, 585478, 4896, 2417, GameClient.eClientType.ShroudedIsles, GameClient.eClientAddons.NewFrontiers, GameClient.eClientVersion.Version185));
			TeleportNPCUtility.CreateTeleporters(1, m_albLocs, "AlbionRvRTeleportNPCEvent", m_albNpcs);

			m_midLocs.Add(new Location("Jordheim", 101, 31750, 28519, 8985, 2050));
			m_midLocs.Add(new LocationExpansion("Thidranki [20-24]", 238, 570038, 540351, 5408, 4076, GameClient.eClientType.ShroudedIsles, GameClient.eClientAddons.NewFrontiers, GameClient.eClientVersion.Version170, 20, 24));
			m_midLocs.Add(new LocationExpansion("Cathal Valley [45+]", 165, 575858, 537997, 4832, 1043, GameClient.eClientType.ShroudedIsles, GameClient.eClientAddons.NewFrontiers, GameClient.eClientVersion.Version185));
			TeleportNPCUtility.CreateTeleporters(2, m_midLocs, "MidgardRvRTeleportNPCEvent", m_midNpcs);

			m_hibLocs.Add(new Location("Tir na Nog", 201, 33680, 31800, 7999, 4684));
			m_hibLocs.Add(new LocationExpansion("Thidranki [20-24]", 238, 533649, 533990, 5408, 3503, GameClient.eClientType.ShroudedIsles, GameClient.eClientAddons.NewFrontiers, GameClient.eClientVersion.Version170, 20, 24));
			m_hibLocs.Add(new LocationExpansion("Cathal Valley [45+]", 165, 536144, 585708, 5800, 2158, GameClient.eClientType.ShroudedIsles, GameClient.eClientAddons.NewFrontiers, GameClient.eClientVersion.Version185));
			TeleportNPCUtility.CreateTeleporters(3, m_hibLocs, "HiberniaRvRTeleportNPCEvent", m_hibNpcs);
		}
		[ScriptUnloadedEvent]
		public static void OnScriptUnloaded(DOLEvent e, object sender, EventArgs args)
		{
			//utility to remove them from the world
			TeleportNPCUtility.Stop(m_albNpcs, m_albLocs);
			TeleportNPCUtility.Stop(m_midNpcs, m_midLocs);
			TeleportNPCUtility.Stop(m_hibNpcs, m_hibLocs);
		}
	}
}
