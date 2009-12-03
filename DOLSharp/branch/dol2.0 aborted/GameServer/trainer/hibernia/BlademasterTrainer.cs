/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */
using System;
using System.Collections;
using System.Reflection;
using DOL.Database;
using DOL.Events;
using DOL.GS.Database;
using DOL.GS.PacketHandler;
using log4net;

namespace DOL.GS.Trainer
{
	/// <summary>
	/// Blademaster Trainer
	/// </summary>	
	[NPCGuildScript("Blademaster Trainer", eRealm.Hibernia)]		// this attribute instructs DOL to use this script for all "Blademaster Trainer" NPC's in Albion (multiple guilds are possible for one script)
	public class BlademasterTrainer : GameTrainer
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// This hash constrain all item template the trainer can give
		/// </summary>	
		private static IDictionary allStartupItems = new Hashtable();

		/// <summary>
		/// This function is called at the server startup
		/// </summary>	
		[GameServerStartedEvent]
		public static void OnServerStartup(DOLEvent e, object sender, EventArgs args)
		{	
			#region Blademaster Gloves

			HandsArmorTemplate blademaster_gloves_template = new HandsArmorTemplate();
			blademaster_gloves_template.Name = "Blademaster Initiate Gloves";
			blademaster_gloves_template.Level = 5;
			blademaster_gloves_template.Durability = 100;
			blademaster_gloves_template.Condition = 100;
			blademaster_gloves_template.Quality = 90;
			blademaster_gloves_template.Bonus = 10;
			blademaster_gloves_template.ArmorLevel = eArmorLevel.Medium;
			blademaster_gloves_template.ArmorFactor = 14;
			blademaster_gloves_template.Weight = 16;
			blademaster_gloves_template.Model = 346;
			blademaster_gloves_template.Realm = eRealm.Hibernia;
			blademaster_gloves_template.IsDropable = true; 
			blademaster_gloves_template.IsTradable = false; 
			blademaster_gloves_template.IsSaleable = false;
			blademaster_gloves_template.MaterialLevel = eMaterialLevel.Bronze;
			
			blademaster_gloves_template.MagicalBonus.Add(new ItemMagicalBonus(eProperty.Quickness, 1));
			
			if(!allStartupItems.Contains("Blademaster_Initiate_Gloves"))
			{
				allStartupItems.Add("Blademaster_Initiate_Gloves", blademaster_gloves_template);
			
				if (log.IsDebugEnabled)
					log.Debug("Adding " + blademaster_gloves_template.Name + " to BlademasterTrainer gifts.");
			}
			#endregion
		}

		/// <summary>
		/// Gets trainer classname
		/// </summary>
		public override string TrainerClassName
		{
			get { return "Blademaster"; }
		}

		/// <summary>
		/// Interact with trainer
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
 		public override bool Interact(GamePlayer player)
 		{		
 			if (!base.Interact(player)) return false;
								
			// check if class matches.				
			if (player.CharacterClass.ID == (int) eCharacterClass.Blademaster)
			{
				player.Out.SendTrainerWindow();
				player.Out.SendMessage(this.Name + " says, \"Do you wish to learn some more, " + player.Name + "? Step up and receive your training!\"", eChatType.CT_Say, eChatLoc.CL_ChatWindow);
			}
			else if (CanPromotePlayer(player)) 
			{
				player.Out.SendMessage(this.Name + " says, \"" + player.Name + ", do you choose the Path of Harmony, and life as a [Blademaster]?\"", eChatType.CT_System, eChatLoc.CL_PopupWindow);
			} 
			else 
			{
				player.Out.SendMessage(this.Name + " says, \"You must seek elsewhere for your training.\"", eChatType.CT_Say, eChatLoc.CL_ChatWindow);
			}
			return true;
 		}

		/// <summary>
		/// checks whether a player can be promoted or not
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public bool CanPromotePlayer(GamePlayer player) 
		{
			return (player.Level>=5 && player.CharacterClass.ID == (int) eCharacterClass.Guardian && (player.Race == (int) eRace.Celt || player.Race == (int) eRace.Firbolg
				|| player.Race == (int) eRace.Elf || player.Race == (int) eRace.Shar));
		}

		/// <summary>
		/// Talk to trainer
		/// </summary>
		/// <param name="source"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		public override bool WhisperReceive(GameLiving source, string text)
		{				
			if (!base.WhisperReceive(source, text)) return false;			
			GamePlayer player = source as GamePlayer;			
	
			switch (text) 
			{
				case "Blademaster":
					if (CanPromotePlayer(player)) 
						PromotePlayer(player, (int)eCharacterClass.Blademaster, "Very well, " + source.GetName(0, false) + ". I gladly take your training into my hands. Congratulations, from this day forth, you are a Blademaster. Here, take this gift to aid you.", new GenericItemTemplate[] {allStartupItems["Blademaster_Initiate_Gloves"] as GenericItemTemplate});
					
				break;
			}
			return true;		
		}
	}
}