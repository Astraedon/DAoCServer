/*
 * Author:		Kristopher Gilbert <ogre@fallenrealms.net>
 * Rev:			$Id: AegirTeleportNPCEvent.cs 456 2005-05-09 12:33:51Z shagz $
 * Copyright:	2004 by Hired Goons, LLC
 * License:		http://www.gnu.org/licenses/gpl.txt
 * 
 */

using System;
using System.Collections;
using DOL.Events;

namespace DOL.GS.GameEvents
{
	public class AegirTeleportNPCEvent
	{
		protected static string m_eventname = "AegirTeleportNPCEvent";

		protected static ArrayList m_locs = new ArrayList();
		protected static ArrayList m_npcs = new ArrayList();

		[ScriptLoadedEvent]
		public static void OnScriptCompiled(DOLEvent e, object sender, EventArgs args)
		{
			//create locations for npcs
			m_locs.Add ( new Location ( "Aegir's Landing" , 151 , 294100 , 356352 , 3488 , 4633 ) ) ;
			m_locs.Add ( new Location ( "Bjarken" , 151 , 290104 , 303007 , 4160 , 1889 ) ) ;
			m_locs.Add ( new Location ( "Dyrfjell" , 151 , 278617 , 324902 , 5128 , 1061 ) ) ;
			m_locs.Add ( new Location ( "Hagall" , 151 , 379140 , 385526 , 7752 , 5684 ) ) ;
			m_locs.Add ( new Location ( "Knarr" , 151 , 302915 , 432206 , 3184 , 1464 ) ) ;

			m_npcs.Clear();

            for (int i = 0; i != m_locs.Count; i++)
            {
                //some npc stuff
                ushort model = 153;
                byte realm = 2;
                string name = "Aegir Translocater";
                string guild = "Ethereal Translocation Services";
                string message = "I can teleport you to various towns around Aegirhamn. " +
                    "Which town would you like me to teleport you to?\n\n";

                //add npcs to the npc collection
                Location loc = (Location)m_locs[i];
                m_npcs.Add(new TeleportNPC(loc.CurrentRegionID, loc.X, loc.Y, loc.Z, loc.Heading,
                    model, realm, name, guild, m_locs, i, message));

            }

			TeleportNPCUtility.Start(m_npcs, m_locs, m_eventname);
		}

		[ScriptUnloadedEvent]
		public static void OnScriptUnloaded(DOLEvent e, object sender, EventArgs args)
		{
			//utility to remove them from the world
			TeleportNPCUtility.Stop(m_npcs, m_locs);
		}
	}
}
