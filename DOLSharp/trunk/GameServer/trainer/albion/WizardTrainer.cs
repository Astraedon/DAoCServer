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
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Trainer
{
	/// <summary>
	/// Wizard Trainer
	/// </summary>	
	[NPCGuildScript("Wizard Trainer", eRealm.Albion)]		// this attribute instructs DOL to use this script for all "Wizard Trainer" NPC's in Albion (multiple guilds are possible for one script)
	public class WizardTrainer : GameTrainer
	{
		public override eCharacterClass TrainedClass
		{
			get { return eCharacterClass.Wizard; }
		}

		public const string WEAPON_ID = "wizard_item";

		public WizardTrainer() : base()
		{
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
			if (player.CharacterClass.ID == (int) eCharacterClass.Wizard) 
			{

				// popup the training window
				player.Out.SendTrainerWindow();
				//player.Out.SendMessage(this.Name + " says, \"Select what you like to train.\"", eChatType.CT_Say, eChatLoc.CL_PopupWindow);												

			}
			else
			{
				// perhaps player can be promoted
				if (CanPromotePlayer(player)) 
				{
					player.Out.SendMessage(this.Name + " says, \"Do you desire to [join the Academy] and summon the power of the elements as a Wizard?\"",eChatType.CT_Say,eChatLoc.CL_PopupWindow);
				} 
				else
				{
					DismissPlayer(player);
				}
			}
			return true;
 		}

		/// <summary>
		/// checks wether a player can be promoted or not
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static bool CanPromotePlayer(GamePlayer player)
		{
			return (player.Level>=5 && player.CharacterClass.ID == (int) eCharacterClass.Elementalist && (player.Race == (int) eRace.Briton
				|| player.Race == (int) eRace.Avalonian || player.Race == (int) eRace.HalfOgre));
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
	
			switch (text) {
			case "join the Academy":
				// promote player to other class
				if (CanPromotePlayer(player)) {
					PromotePlayer(player, (int)eCharacterClass.Wizard, "Welcome to our guild! You have much to learn, but I see greatness in your future! Here too is your guild weapon, a Staff of Focus!", null);
					player.ReceiveItem(this,WEAPON_ID);
				}
				break;
			}
			return true;		
		}
	}
}
