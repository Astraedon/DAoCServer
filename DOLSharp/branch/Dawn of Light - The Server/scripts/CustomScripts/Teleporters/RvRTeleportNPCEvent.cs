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
			m_albLocs.Add(new LocationExpansion("Thidranki (20-24)", 238, 563466, 574358, 5408, 2121, GameClient.eClientType.ShroudedIsles, GameClient.eClientAddons.NewFrontiers, GameClient.eClientVersion.Version170, 20, 24));
			//m_albLocs.Add(new LocationExpansion("Molvik [35-39]", 238, 563466, 574358, 5408, 2121, GameClient.eClientType.ShroudedIsles, GameClient.eClientAddons.NewFrontiers, GameClient.eClientVersion.Version170, 20, 24));
			//m_albLocs.Add(new LocationExpansion("Leirvik [40-44]", 165, 583159, 585478, 4896, 2417, GameClient.eClientType.ShroudedIsles, GameClient.eClientAddons.NewFrontiers, GameClient.eClientVersion.Version185, 40, 44));
			m_albLocs.Add(new LocationExpansion("Cathal Valley (45+)", 165, 583159, 585478, 4896, 2417, GameClient.eClientType.ShroudedIsles, GameClient.eClientAddons.NewFrontiers, GameClient.eClientVersion.Version185, 45, 50));
			m_albLocs.Add(new Location("Caerwent (Housing)", 2, 556483, 559261, 3646, 2940));
			m_albLocs.Add(new Location("Old Sarum (Housing)", 2, 559714, 620708, 3650, 2940));
			m_albLocs.Add(new Location("Rilan (Housing)", 2, 559587, 489821, 3418, 2868));
			m_albLocs.Add(new Location("Brisworthy (Housing)", 2, 490053, 490246, 3626, 552));
			m_albLocs.Add(new Location("Stoneleigh (Housing)", 2, 528654, 489658, 3650, 2871));
			m_albLocs.Add(new Location("Chiltern (Housing)", 2, 428433, 558438, 3650, 1803));
			m_albLocs.Add(new Location("Sherbourne (Housing)", 2, 428218, 620995, 3274, 7663));
			m_albLocs.Add(new Location("Aylesbury (Housing)", 2, 491863, 620061, 3249, 1675));
			m_albLocs.Add(new Location("Dalton (Housing)", 2, 488707, 557993, 3746, 776));
			m_albLocs.Add(new SpecialLocation("Personal House", SpecialLocation.eSpecialLocation.PersonalHouse));
			TeleportNPCUtility.CreateTeleporters(1, m_albLocs, "AlbionRvRTeleportNPCEvent", m_albNpcs);

			m_midLocs.Add(new Location("Jordheim", 101, 31750, 28519, 8985, 2050));
			m_midLocs.Add(new LocationExpansion("Thidranki (20-24)", 238, 570038, 540351, 5408, 4076, GameClient.eClientType.ShroudedIsles, GameClient.eClientAddons.NewFrontiers, GameClient.eClientVersion.Version170, 20, 24));
			m_midLocs.Add(new LocationExpansion("Cathal Valley (45+)", 165, 575858, 537997, 4832, 1043, GameClient.eClientType.ShroudedIsles, GameClient.eClientAddons.NewFrontiers, GameClient.eClientVersion.Version185, 45, 50));
			m_midLocs.Add(new Location("Carlingford (Housing)", 102, 623984, 557289, 3722, 1359));
			m_midLocs.Add(new Location("Arothi (Housing)", 102, 556954, 484844, 3514, 1250));
			m_midLocs.Add(new Location("Kaupang (Housing)", 102, 626252, 482408, 3618, 1539));
			m_midLocs.Add(new Location("Stavgaard (Housing)", 102, 686284, 491490, 3770, 339));
			m_midLocs.Add(new Location("Holmestrand (Housing)", 102, 686407, 557202, 3738, 4017));
			m_midLocs.Add(new Location("Nittedal (Housing)", 102, 688034, 615923, 3514, 1246));
			m_midLocs.Add(new Location("Frisia (Housing)", 102, 621515, 615911, 3730, 803));
			m_midLocs.Add(new Location("Wyndham (Housing)", 102, 555224, 622551, 3770, 326));
			m_midLocs.Add(new Location("Erikstaad (Housing)", 102, 553194, 566166, 3650, 524));
			m_midLocs.Add(new SpecialLocation("Personal House", SpecialLocation.eSpecialLocation.PersonalHouse));
			TeleportNPCUtility.CreateTeleporters(2, m_midLocs, "MidgardRvRTeleportNPCEvent", m_midNpcs);

			m_hibLocs.Add(new Location("Tir na Nog", 201, 33680, 31800, 7999, 4684));
			m_hibLocs.Add(new LocationExpansion("Thidranki (20-24)", 238, 533649, 533990, 5408, 3503, GameClient.eClientType.ShroudedIsles, GameClient.eClientAddons.NewFrontiers, GameClient.eClientVersion.Version170, 20, 24));
			m_hibLocs.Add(new LocationExpansion("Cathal Valley (45+)", 165, 536144, 585708, 5800, 2158, GameClient.eClientType.ShroudedIsles, GameClient.eClientAddons.NewFrontiers, GameClient.eClientVersion.Version185, 45, 50));
			m_hibLocs.Add(new Location("Torrylin (Housing)", 202, 565479, 618767, 3185, 3515));
			m_hibLocs.Add(new Location("Aberillan (Housing)", 202, 616881, 619453, 3185, 987));
			m_hibLocs.Add(new Location("Kilcullen (Housing)", 202, 619295, 561232, 3209, 3061));
			m_hibLocs.Add(new Location("Broughshane (Housing)", 202, 619290, 692303, 3209, 3080));
			m_hibLocs.Add(new Location("Tullamore (Housing)", 202, 560360, 692308, 3215, 1020));
			m_hibLocs.Add(new Location("Moycullen (Housing)", 202, 493620, 688573, 3137, 3123));
			m_hibLocs.Add(new Location("Saeranthal (Housing)", 202, 493562, 619855, 3129, 2503));
			m_hibLocs.Add(new Location("Dunshire (Housing)", 202, 493610, 555502, 3137, 3109));
			m_hibLocs.Add(new Location("Meath (Housing)", 202, 562625, 559998, 3076, 3109));
			m_hibLocs.Add(new SpecialLocation("Personal House", SpecialLocation.eSpecialLocation.PersonalHouse));
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
