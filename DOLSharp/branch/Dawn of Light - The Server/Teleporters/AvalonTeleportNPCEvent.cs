/*
 * Author:		Kristopher Gilbert <ogre@fallenrealms.net>
 * Rev:			$Id: AvalonTeleportNPCEvent.cs 456 2005-05-09 12:33:51Z shagz $
 * Copyright:	2004 by Hired Goons, LLC
 * License:		http://www.gnu.org/licenses/gpl.txt
 * 
 */

using System;
using System.Collections;
using DOL.Events;

namespace DOL.GS.GameEvents
{
	public class AvalonTeleportNPCEvent
	{
		protected static string m_eventname = "AvalonTeleportNPCEvent";

		protected static ArrayList m_locs = new ArrayList();
		protected static ArrayList m_npcs = new ArrayList();

		[ScriptLoadedEvent]
		public static void OnScriptCompiled(DOLEvent e, object sender, EventArgs args)
		{
			//create locations for npcs
			m_locs.Add ( new Location ( "Anniogel" , 51 , 529132 , 512677 , 3648 , 4497 ) ) ;
			m_locs.Add ( new Location ( "Caer Diogel" , 51 , 402769 , 504565 , 4680 , 2346 ) ) ;
			m_locs.Add ( new Location ( "Caifelle" , 51 , 545381 , 512413 , 3168 , 6903 ) ) ;
			m_locs.Add ( new Location ( "Fort Gwyntel" , 51 , 427098 , 416568 , 5712 , 4659 ) ) ;
			m_locs.Add ( new Location ( "Gothwaite Harbor" , 51 , 527133 , 542782 , 3168 , 2012 ) ) ;
			m_locs.Add ( new Location ( "Wearyall Village" , 51 , 435997 , 493646 , 3088 , 846 ) ) ;

			m_npcs.Clear();

            for (int i = 0; i != m_locs.Count; i++)
            {
                //some npc stuff
                ushort model = 14;
                byte realm = 1;
                string name = "Avalon Translocater";
                string guild = "Ethereal Translocation Services";
                string message = "I can teleport you to various towns around Avalon. " +
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
