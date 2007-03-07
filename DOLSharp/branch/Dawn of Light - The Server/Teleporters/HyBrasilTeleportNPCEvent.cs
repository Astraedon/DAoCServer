/*
 * Author:		Kristopher Gilbert <ogre@fallenrealms.net>
 * Rev:			$Id: HyBrasilTeleportNPCEvent.cs 456 2005-05-09 12:33:51Z shagz $
 * Copyright:	2004 by Hired Goons, LLC
 * License:		http://www.gnu.org/licenses/gpl.txt
 * 
 */

using System;
using System.Collections;
using DOL.Events;

namespace DOL.GS.GameEvents
{
	public class HyBrasilTeleportNPCEvent
	{
		protected static string m_eventname = "HyBrasilTeleportNPCEvent";

		protected static ArrayList m_locs = new ArrayList();
		protected static ArrayList m_npcs = new ArrayList();
		protected static TeleportNPCUtility m_utility = new TeleportNPCUtility();

		[ScriptLoadedEvent]
		public static void OnScriptCompiled(DOLEvent e, object sender, EventArgs args)
		{
			//create locations for npcs
			m_locs.Add ( new Location ( "Bann Didein" , 181 , 426815 , 404528 , 4610 , 2186 ) ) ;
			m_locs.Add ( new Location ( "Droighaid" , 181 , 379854 , 421873 , 5528 , 1968 ) ) ;
			m_locs.Add ( new Location ( "Grove of Aalid Feie" , 181 , 311593 , 353389 , 3626 , 38848 ) ) ;
			m_locs.Add ( new Location ( "Grove of Domnann" , 181 , 423688 , 440063 , 5968 , 1170 ) ) ;
			m_locs.Add ( new Location ( "Outlander Town" , 181 , 336934 , 315749 , 2339 , 6239 ) ) ;
			m_locs.Add ( new Location ( "Krrzck" , 181 , 365228 , 264042 , 3458 , 6948 ) ) ;
			m_locs.Add ( new Location ( "Necht" , 181 , 429654 , 318695 , 3472 , 5098 ) ) ;

			m_npcs.Clear();

            for (int i = 0; i != m_locs.Count; i++)
            {
                //some npc stuff
                ushort model = 302;
                byte realm = 3;
                string name = "HyBrasil Translocater";
                string guild = "Ethereal Translocation Services";
                string message = "I can teleport you to various towns around HyBrasil. " +
                    "Which town would you like me to teleport you to?\n\n";

                //add npcs to the npc collection
                Location loc = (Location)m_locs[i];
                m_npcs.Add(new TeleportNPC(loc.CurrentRegionID, loc.X, loc.Y, loc.Z, loc.Heading,
                    model, realm, name, guild, m_locs, i, message));

            }
			//utility to add them to the world
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
