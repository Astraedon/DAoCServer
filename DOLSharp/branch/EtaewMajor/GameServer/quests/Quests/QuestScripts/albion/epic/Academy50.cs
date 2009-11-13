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
/*
*Author         : Etaew - Fallen Realms
*Editor         : Gandulf
*Source         : http://camelot.allakhazam.com
*Date           : 22 November 2004
*Quest Name     : Symbol of the Broken (level 50)
*Quest Classes  : Sorceror, Minstrel, Wizard(Academy)
*Quest Version  : v1
*
*ToDo:
*   Add correct Text - Added custom text as workaround
*   Find Helm ModelID for epics..
*/

using System;
using System.Reflection;
using DOL.AI.Brain;
using DOL.Database;
using DOL.Events;
using DOL.GS.PacketHandler;
using log4net;

namespace DOL.GS.Quests.Albion
{
	public class Academy_50 : BaseQuest
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		protected const string questTitle = "Symbol of the Broken";
		protected const int minimumLevel = 50;
		protected const int maximumLevel = 50;

		private static GameNPC Ferowl = null; // Start NPC
		private static GameNPC Morgana = null; // Mob
		private static GameNPC Bechard = null; // Mob to kill
		private static GameNPC Silcharde = null; // Mob to kill

		private static IArea morganaArea = null;

		private static ItemTemplate sealed_pouch = null; //sealed pouch
		private static ItemTemplate WizardEpicBoots = null; //Bernor's Numinous Boots 
		private static ItemTemplate WizardEpicHelm = null; //Bernor's Numinous Coif 
		private static ItemTemplate WizardEpicGloves = null; //Bernor's Numinous Gloves 
		private static ItemTemplate WizardEpicVest = null; //Bernor's Numinous Hauberk 
		private static ItemTemplate WizardEpicLegs = null; //Bernor's Numinous Legs 
		private static ItemTemplate WizardEpicArms = null; //Bernor's Numinous Sleeves 
		private static ItemTemplate MinstrelEpicBoots = null; //Shadow Shrouded Boots 
		private static ItemTemplate MinstrelEpicHelm = null; //Shadow Shrouded Coif 
		private static ItemTemplate MinstrelEpicGloves = null; //Shadow Shrouded Gloves 
		private static ItemTemplate MinstrelEpicVest = null; //Shadow Shrouded Hauberk 
		private static ItemTemplate MinstrelEpicLegs = null; //Shadow Shrouded Legs 
		private static ItemTemplate MinstrelEpicArms = null; //Shadow Shrouded Sleeves 
		private static ItemTemplate SorcerorEpicBoots = null; //Valhalla Touched Boots 
		private static ItemTemplate SorcerorEpicHelm = null; //Valhalla Touched Coif 
		private static ItemTemplate SorcerorEpicGloves = null; //Valhalla Touched Gloves 
		private static ItemTemplate SorcerorEpicVest = null; //Valhalla Touched Hauberk 
		private static ItemTemplate SorcerorEpicLegs = null; //Valhalla Touched Legs 
		private static ItemTemplate SorcerorEpicArms = null; //Valhalla Touched Sleeves                 

		// Constructors
		public Academy_50() : base()
		{
		}

		public Academy_50(GamePlayer questingPlayer) : base(questingPlayer)
		{
		}

		public Academy_50(GamePlayer questingPlayer, int step) : base(questingPlayer, step)
		{
		}

		public Academy_50(GamePlayer questingPlayer, DBQuest dbQuest) : base(questingPlayer, dbQuest)
		{
		}

		[ScriptLoadedEvent]
		public static void ScriptLoaded(DOLEvent e, object sender, EventArgs args)
		{
			if (!ServerProperties.Properties.LOAD_QUESTS)
				return;
			if (log.IsInfoEnabled)
				log.Info("Quest \"" + questTitle + "\" initializing ...");

			#region defineNPCs

            GameNPC[] npcs = WorldMgr.GetNPCsByName("Master Ferowl", eRealm.Albion);
			if (npcs.Length == 0)
			{
				Ferowl = new GameNPC();
				Ferowl.Model = 61;
				Ferowl.Name = "Master Ferowl";
				if (log.IsWarnEnabled)
					log.Warn("Could not find " + Ferowl.Name + " , creating it ...");
				Ferowl.GuildName = "";
				Ferowl.Realm = eRealm.Albion;
				Ferowl.CurrentRegionID = 1;
				Ferowl.Size = 51;
				Ferowl.Level = 40;
				Ferowl.X = 559716;
				Ferowl.Y = 510733;
				Ferowl.Z = 2720;
				Ferowl.Heading = 703;
				Ferowl.AddToWorld();

				if (SAVE_INTO_DATABASE)
					Ferowl.SaveIntoDatabase();
			}
			else
				Ferowl = npcs[0];
			// end npc

			npcs = WorldMgr.GetNPCsByName("Morgana", eRealm.None);
			if (npcs.Length == 0)
			{
				Morgana = new GameNPC();
				Morgana.Model = 283;
				Morgana.Name = "Morgana";
				if (log.IsWarnEnabled)
					log.Warn("Could not find " + Morgana.Name + " , creating it ...");
				Morgana.GuildName = "";
				Morgana.Realm = eRealm.None;
				Morgana.CurrentRegionID = 1;
				Morgana.Size = 51;
				Morgana.Level = 90;
				Morgana.X = 306056;
				Morgana.Y = 670106;
				Morgana.Z = 3095;
				Morgana.Heading = 3261;

				StandardMobBrain brain = new StandardMobBrain();
				brain.AggroLevel = 0;
				brain.AggroRange = 0;
				Morgana.SetOwnBrain(brain);

				GameNpcInventoryTemplate template = new GameNpcInventoryTemplate();
				template.AddNPCEquipment(eInventorySlot.TorsoArmor, 98, 43);
				template.AddNPCEquipment(eInventorySlot.FeetArmor, 133, 61);
				Morgana.Inventory = template.CloseTemplate();

//				Morgana.AddNPCEquipment((byte) eVisibleItems.TORSO, 98, 43, 0, 0);
//				Morgana.AddNPCEquipment((byte) eVisibleItems.BOOT, 133, 61, 0, 0);

				//Morgana.AddToWorld(); will be added later during quest

				if (SAVE_INTO_DATABASE)
					Morgana.SaveIntoDatabase();
			}
			else
				Morgana = npcs[0];

			npcs = WorldMgr.GetNPCsByName("Bechard", eRealm.None);
			if (npcs.Length == 0)
			{
				if (log.IsWarnEnabled)
					log.Warn("Could not find Bechard , creating it ...");
				Bechard = new GameNPC();
				Bechard.Model = 606;
				Bechard.Name = "Bechard";
				Bechard.GuildName = "";
				Bechard.Realm = eRealm.None;
				Bechard.CurrentRegionID = 1;
				Bechard.Size = 50;
				Bechard.Level = 63;
				Bechard.X = 306025;
				Bechard.Y = 670473;
				Bechard.Z = 2863;
				Bechard.Heading = 3754;
				Bechard.AddToWorld();

				if (SAVE_INTO_DATABASE)
					Bechard.SaveIntoDatabase();
			}
			else
				Bechard = npcs[0];
			// end npc

			npcs = WorldMgr.GetNPCsByName("Silcharde", eRealm.None);
			if (npcs.Length == 0)
			{
				if (log.IsWarnEnabled)
					log.Warn("Could not find Silcharde , creating it ...");
				Silcharde = new GameNPC();
				Silcharde.Model = 606;
				Silcharde.Name = "Silcharde";
				Silcharde.GuildName = "";
				Silcharde.Realm = eRealm.None;
				Silcharde.CurrentRegionID = 1;
				Silcharde.Size = 50;
				Silcharde.Level = 63;
				Silcharde.X = 306252;
				Silcharde.Y = 670274;
				Silcharde.Z = 2857;
				Silcharde.Heading = 3299;
				Silcharde.AddToWorld();

				if (SAVE_INTO_DATABASE)
					Silcharde.SaveIntoDatabase();

			}
			else
				Silcharde = npcs[0];
			// end npc

			#endregion

			#region Item Declarations
			sealed_pouch = (ItemTemplate) GameServer.Database.FindObjectByKey(typeof (ItemTemplate), "sealed_pouch");
			WizardEpicBoots = (ItemTemplate) GameServer.Database.FindObjectByKey(typeof (ItemTemplate), "WizardEpicBoots");
			WizardEpicHelm = (ItemTemplate) GameServer.Database.FindObjectByKey(typeof (ItemTemplate), "WizardEpicHelm");
			WizardEpicGloves = (ItemTemplate) GameServer.Database.FindObjectByKey(typeof (ItemTemplate), "WizardEpicGloves");
			WizardEpicVest = (ItemTemplate) GameServer.Database.FindObjectByKey(typeof (ItemTemplate), "WizardEpicVest");
			WizardEpicLegs = (ItemTemplate) GameServer.Database.FindObjectByKey(typeof (ItemTemplate), "WizardEpicLegs");
			WizardEpicArms = (ItemTemplate) GameServer.Database.FindObjectByKey(typeof (ItemTemplate), "WizardEpicArms");
			MinstrelEpicBoots = (ItemTemplate) GameServer.Database.FindObjectByKey(typeof (ItemTemplate), "MinstrelEpicBoots");
			MinstrelEpicHelm = (ItemTemplate) GameServer.Database.FindObjectByKey(typeof (ItemTemplate), "MinstrelEpicHelm");
			MinstrelEpicGloves = (ItemTemplate) GameServer.Database.FindObjectByKey(typeof (ItemTemplate), "MinstrelEpicGloves");
			MinstrelEpicVest = (ItemTemplate) GameServer.Database.FindObjectByKey(typeof (ItemTemplate), "MinstrelEpicVest");
			MinstrelEpicLegs = (ItemTemplate) GameServer.Database.FindObjectByKey(typeof (ItemTemplate), "MinstrelEpicLegs");
			MinstrelEpicArms = (ItemTemplate) GameServer.Database.FindObjectByKey(typeof (ItemTemplate), "MinstrelEpicArms");
			SorcerorEpicBoots = (ItemTemplate) GameServer.Database.FindObjectByKey(typeof (ItemTemplate), "SorcerorEpicBoots");
			SorcerorEpicHelm = (ItemTemplate) GameServer.Database.FindObjectByKey(typeof (ItemTemplate), "SorcerorEpicHelm");
			SorcerorEpicGloves = (ItemTemplate) GameServer.Database.FindObjectByKey(typeof (ItemTemplate), "SorcerorEpicGloves");
			SorcerorEpicVest = (ItemTemplate) GameServer.Database.FindObjectByKey(typeof (ItemTemplate), "SorcerorEpicVest");
			SorcerorEpicLegs = (ItemTemplate) GameServer.Database.FindObjectByKey(typeof (ItemTemplate), "SorcerorEpicLegs");
			SorcerorEpicArms = (ItemTemplate) GameServer.Database.FindObjectByKey(typeof (ItemTemplate), "SorcerorEpicArms");
			#endregion

			morganaArea = WorldMgr.GetRegion(Morgana.CurrentRegionID).AddArea(new Area.Circle(null, Morgana.X, Morgana.Y, 0, 1000));
			morganaArea.RegisterPlayerEnter(new DOLEventHandler(PlayerEnterMorganaArea));

			GameEventMgr.AddHandler(GamePlayerEvent.AcceptQuest, new DOLEventHandler(SubscribeQuest));
			GameEventMgr.AddHandler(GamePlayerEvent.DeclineQuest, new DOLEventHandler(SubscribeQuest));

			GameEventMgr.AddHandler(Ferowl, GameObjectEvent.Interact, new DOLEventHandler(TalkToFerowl));
			GameEventMgr.AddHandler(Ferowl, GameLivingEvent.WhisperReceive, new DOLEventHandler(TalkToFerowl));

			/* Now we bring to Ferowl the possibility to give this quest to players */
			Ferowl.AddQuestToGive(typeof (Academy_50));

			if (log.IsInfoEnabled)
				log.Info("Quest \"" + questTitle + "\" initialized");
		}

		[ScriptUnloadedEvent]
		public static void ScriptUnloaded(DOLEvent e, object sender, EventArgs args)
		{
			if (!ServerProperties.Properties.LOAD_QUESTS)
				return;
			//if not loaded, don't worry
			if (Ferowl == null)
				return;

			morganaArea.UnRegisterPlayerEnter(new DOLEventHandler(PlayerEnterMorganaArea));
			WorldMgr.GetRegion(Morgana.CurrentRegionID).RemoveArea(morganaArea);
			// remove handlers
			GameEventMgr.RemoveHandler(GamePlayerEvent.AcceptQuest, new DOLEventHandler(SubscribeQuest));
			GameEventMgr.RemoveHandler(GamePlayerEvent.DeclineQuest, new DOLEventHandler(SubscribeQuest));

			GameEventMgr.RemoveHandler(Ferowl, GameObjectEvent.Interact, new DOLEventHandler(TalkToFerowl));
			GameEventMgr.RemoveHandler(Ferowl, GameLivingEvent.WhisperReceive, new DOLEventHandler(TalkToFerowl));

			/* Now we remove to Ferowl the possibility to give this quest to players */
			Ferowl.RemoveQuestToGive(typeof (Academy_50));
		}

		protected static void PlayerEnterMorganaArea(DOLEvent e, object sender, EventArgs args)
		{
			AreaEventArgs aargs = args as AreaEventArgs;
			GamePlayer player = aargs.GameObject as GamePlayer;
			Academy_50 quest = player.IsDoingQuest(typeof (Academy_50)) as Academy_50;

			if (quest != null && Morgana.ObjectState != GameObject.eObjectState.Active)
			{
				// player near grove
				SendSystemMessage(player, "As you approach the fallen tower you see Morgana standing on top of the tower.");
				quest.CreateMorgana();

				if (player.Group != null)
					Morgana.Yell("Ha, is this all the forces of Albion have to offer? I expected a whole army leaded by my brother Arthur, but what do they send a little group of adventurers lead by a poor " + player.CharacterClass.Name + "?");
				else
					Morgana.Yell("Ha, is this all the forces of Albion have to offer? I expected a whole army leaded by my brother Arthur, but what do they send a poor " + player.CharacterClass.Name + "?");

				foreach (GamePlayer visPlayer in Morgana.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
				{
					visPlayer.Out.SendSpellCastAnimation(Morgana, 1, 20);
				}
			}
		}

		protected virtual void CreateMorgana()
		{
			if (Morgana == null)
			{
				Morgana = new GameNPC();
				Morgana.Model = 283;
				Morgana.Name = "Morgana";
				if (log.IsWarnEnabled)
					log.Warn("Could not find " + Morgana.Name + " , creating it ...");
				Morgana.GuildName = "";
				Morgana.Realm = eRealm.None;
				Morgana.CurrentRegionID = 1;
				Morgana.Size = 51;
				Morgana.Level = 90;
				Morgana.X = 306056;
				Morgana.Y = 670106;
				Morgana.Z = 3095;
				Morgana.Heading = 3261;

				
				StandardMobBrain brain = new StandardMobBrain();
				brain.AggroLevel = 0;
				brain.AggroRange = 0;
				Morgana.SetOwnBrain(brain);

				GameNpcInventoryTemplate template = new GameNpcInventoryTemplate();
				template.AddNPCEquipment(eInventorySlot.TorsoArmor, 98, 43);
				template.AddNPCEquipment(eInventorySlot.FeetArmor, 133, 61);
				Morgana.Inventory = template.CloseTemplate();

//				Morgana.AddNPCEquipment((byte) eVisibleItems.TORSO, 98, 43, 0, 0);
//				Morgana.AddNPCEquipment((byte) eVisibleItems.BOOT, 133, 61, 0, 0);
			}

			Morgana.AddToWorld();
		}

		protected virtual void DeleteMorgana()
		{
			if (Morgana != null)
				Morgana.Delete();
		}

		protected static void TalkToFerowl(DOLEvent e, object sender, EventArgs args)
		{
			//We get the player from the event arguments and check if he qualifies		
			GamePlayer player = ((SourceEventArgs) args).Source as GamePlayer;
			if (player == null)
				return;

			if(Ferowl.CanGiveQuest(typeof (Academy_50), player)  <= 0)
				return;

			//We also check if the player is already doing the quest
			Academy_50 quest = player.IsDoingQuest(typeof (Academy_50)) as Academy_50;

			if (e == GameObjectEvent.Interact)
			{
				// Nag to finish quest
				if (quest != null)
				{
					Ferowl.SayTo(player, "Were you able to [fulfill] your given task? Albions fate lies in you hands. ");
				}
				else
				{
					Ferowl.SayTo(player, "Ah good to see you, there are rumors about your tasks all over Albion, yet we are in need of your [services] once again!");
				}
				return;
			}
				// The player whispered to the NPC
			else if (e == GameLivingEvent.WhisperReceive)
			{
				WhisperReceiveEventArgs wArgs = (WhisperReceiveEventArgs) args;
				if (quest == null)
				{
					switch (wArgs.Text)
					{
						case "services":
							player.Out.SendQuestSubscribeCommand(Ferowl, QuestMgr.GetIDForQuestType(typeof(Academy_50)), "Will you help Ferowl [Academy Level 50 Epic]");
							break;
					}
				}
				else
				{
					switch (wArgs.Text)
					{
						case "Morgana":
							Ferowl.SayTo(player, "You must have heard about her, she's the evil sister of King Arthur, tried to take his throne. She must be [stopped]!");
							break;
						case "stopped":
							Ferowl.SayTo(player, "Once Morgana has summoned her army everything is lost. So hurry and stop her unholy rituals. With the help of two mighty demons Silcharde and Bechard she can summon as many minions as she wants. Killing one of them should be enough to stop here [ritual].");
							break;
						case "ritual":
							Ferowl.SayTo(player, "Morgana is probably performing her rital at the fallen tower in Lyonesse. To get there follow the Telamon road past the majority of the Danaoian Farmers, until you see the [fallen tower].");
							break;
						case "fallen tower":
							Ferowl.SayTo(player, "Be wise and don't take any unneccessary risks by going directly on Morgana , you might be a strong " + player.CharacterClass.Name + ", but you are no match for Morgana herself. Kill her demons and return to me, we will then try to take care of the rest, once her time has come.");
							break;

							// once the deomns are dead:
						case "fulfill":
							Ferowl.SayTo(player, "Did you find anything near the fallen tower? If yes give it to me, we could need any hints we can get on our crusade against Morgana.");
							break;

						case "abort":
							player.Out.SendCustomDialog("Do you really want to abort this quest, \nall items gained during quest will be lost?", new CustomDialogResponse(CheckPlayerAbortQuest));
							break;
					}
				}
			}
		}

		public override bool CheckQuestQualification(GamePlayer player)
		{
			// if the player is already doing the quest his level is no longer of relevance
			if (player.IsDoingQuest(typeof (Academy_50)) != null)
				return true;

			if (player.CharacterClass.ID != (byte) eCharacterClass.Minstrel &&
				player.CharacterClass.ID != (byte) eCharacterClass.Wizard &&
				player.CharacterClass.ID != (byte) eCharacterClass.Sorcerer)
				return false;

			// This checks below are only performed is player isn't doing quest already

			//if (player.HasFinishedQuest(typeof(Academy_47)) == 0) return false;

			//if (!CheckPartAccessible(player,typeof(CityOfCamelot)))
			//	return false;

			if (player.Level < minimumLevel || player.Level > maximumLevel)
				return false;

			return true;
		}

		/* This is our callback hook that will be called when the player clicks
		 * on any button in the quest offer dialog. We check if he accepts or
		 * declines here...
		 */

		protected static void SubscribeQuest(DOLEvent e, object sender, EventArgs args)
		{
			QuestEventArgs qargs = args as QuestEventArgs;
			if (qargs == null)
				return;

			if (qargs.QuestID != QuestMgr.GetIDForQuestType(typeof(Academy_50)))
				return;

			if (e == GamePlayerEvent.AcceptQuest)
				CheckPlayerAcceptQuest(qargs.Player, 0x01);
			else if (e == GamePlayerEvent.DeclineQuest)
				CheckPlayerAcceptQuest(qargs.Player, 0x00);
		}

		private static void CheckPlayerAbortQuest(GamePlayer player, byte response)
		{
			Academy_50 quest = player.IsDoingQuest(typeof (Academy_50)) as Academy_50;

			if (quest == null)
				return;

			if (response == 0x00)
			{
				SendSystemMessage(player, "Good, now go out there and finish your work!");
			}
			else
			{
				SendSystemMessage(player, "Aborting Quest " + questTitle + ". You can start over again if you want.");
				quest.AbortQuest();
			}
		}

		private static void CheckPlayerAcceptQuest(GamePlayer player, byte response)
		{
			if(Ferowl.CanGiveQuest(typeof (Academy_50), player)  <= 0)
				return;

			if (player.IsDoingQuest(typeof (Academy_50)) != null)
				return;

			if (response == 0x00)
			{
				player.Out.SendMessage("Our God forgives your laziness, just look out for stray lightning bolts.", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
			}
			else
			{
				// Check to see if we can add quest
				if (!Ferowl.GiveQuest(typeof (Academy_50), player, 1))
					return;

				Ferowl.SayTo(player, "I have heard rumors about the witch [Morgana] trying to summon an army of demons to crush the mighty city of Camelot!");
			}
		}

		//Set quest name
		public override string Name
		{
			get { return "Symbol of the Broken (Level 50 Academy Epic)"; }
		}

		// Define Steps
		public override string Description
		{
			get
			{
				switch (Step)
				{
					case 1:
						return "[Step #1] Seek out Bechard or Silcharde at the fallen tower in Lyonesse and kill one!";
					case 2:
						return "[Step #2] Return to Ferowl for your reward!";
				}
				return base.Description;
			}
		}

		public override void Notify(DOLEvent e, object sender, EventArgs args)
		{
			GamePlayer player = sender as GamePlayer;

			if (player==null || player.IsDoingQuest(typeof (Academy_50)) == null)
				return;

			if (Step == 1 && e == GameLivingEvent.EnemyKilled)
			{
				EnemyKilledEventArgs gArgs = (EnemyKilledEventArgs) args;
				{
					if (gArgs.Target.Name == Bechard.Name || gArgs.Target.Name == Silcharde.Name)
					{
						Morgana.Yell("You may have stopped me here, but I'll come back! Albion will be mine!");
						DeleteMorgana();

						m_questPlayer.Out.SendMessage("Take the pouch to " + Ferowl.GetName(0, true), eChatType.CT_System, eChatLoc.CL_SystemWindow);
						GiveItem(m_questPlayer, sealed_pouch);
						Step = 2;
						return;
					}
				}
			}

			if (Step == 2 && e == GamePlayerEvent.GiveItem)
			{
				GiveItemEventArgs gArgs = (GiveItemEventArgs) args;
				if (gArgs.Target.Name == Ferowl.Name && gArgs.Item.TemplateID == sealed_pouch.TemplateID)
				{
					RemoveItem(Ferowl, player, sealed_pouch);
					Ferowl.SayTo(player, "You have earned this Epic Armour, wear it with honor!");
					FinishQuest();
					return;
				}
			}
		}

		public override void AbortQuest()
		{
			base.AbortQuest(); //Defined in Quest, changes the state, stores in DB etc ...

			RemoveItem(m_questPlayer, sealed_pouch, false);
		}

		public override void FinishQuest()
		{
			base.FinishQuest(); //Defined in Quest, changes the state, stores in DB etc ...

			if (m_questPlayer.CharacterClass.ID == (byte) eCharacterClass.Minstrel)
			{
				GiveItem(m_questPlayer, MinstrelEpicBoots);
				GiveItem(m_questPlayer, MinstrelEpicHelm);
				GiveItem(m_questPlayer, MinstrelEpicGloves);
				GiveItem(m_questPlayer, MinstrelEpicArms);
				GiveItem(m_questPlayer, MinstrelEpicVest);
				GiveItem(m_questPlayer, MinstrelEpicLegs);
			}
			else if (m_questPlayer.CharacterClass.ID == (byte) eCharacterClass.Wizard)
			{
				GiveItem(m_questPlayer, WizardEpicBoots);
				GiveItem(m_questPlayer, WizardEpicHelm);
				GiveItem(m_questPlayer, WizardEpicGloves);
				GiveItem(m_questPlayer, WizardEpicVest);
				GiveItem(m_questPlayer, WizardEpicArms);
				GiveItem(m_questPlayer, WizardEpicLegs);
			}
			else if (m_questPlayer.CharacterClass.ID == (byte) eCharacterClass.Sorcerer)
			{
				GiveItem(m_questPlayer, SorcerorEpicBoots);
				GiveItem(m_questPlayer, SorcerorEpicHelm);
				GiveItem(m_questPlayer, SorcerorEpicGloves);
				GiveItem(m_questPlayer, SorcerorEpicVest);
				GiveItem(m_questPlayer, SorcerorEpicArms);
				GiveItem(m_questPlayer, SorcerorEpicLegs);
			}

			m_questPlayer.GainExperience(1937768448, true);
			//m_questPlayer.AddMoney(Money.GetMoney(0,0,0,2,Util.Random(50)), "You recieve {0} as a reward.");		
		}

		#region Allakhazam Epic Source

		/*
        * Return to Esmond in Cornwall Station once you reach level 50. If you had given him the dagger at the end of the level 48 epic, he will ask you if you want it back. If he asks, accept the knife back and continue. If not, make sure you have the ritual dagger with you. 
		* Go to Lyonesse and find the tower, which is located at 20k, 39k. You can simply follow the Telamon road past the majority of the Danaoian Farmers, until you see a fallen tower with two large named demons (purple to 50) and Morgana sitting on top of the tower. 
		* To defeat them is quite easy and can take as little as 6 people. As long as you have at least one tank, a healer, and someone who can root or mez, you should be ok. 
		* Do not attack Morgana. She will not do anything during this attack. Have someone root or mez one of the named demons, while the tank(s) hold aggro on the second one. When the aggroed one is defeated, a large group of tiny demons will appear and fly around the tower (they were all green to a 50). Take care of the previously rooted/mezed demon and another group of tiny demons will appear. Morgana will spout off something that can be heard across the zone, then leave. Kill all the tiny demons that remain. 
		* Once all the aggro has been cleared, stand next to the tower. There will be a message that says, "You sense the tower is clear of necromantic ties!" about 5 or so times. Your dagger should dissapear from your inventory, followed by a message that says, "A sense of calm settles about you!" When you recieve that message, your journal will update and tell you go to meet Master Ferowl again. 
		* Master Ferowl congratulates you on a job well done and asks you to go meet your trainer in Camelot for your reward. Also, Ferowl gives you 1,937,768,448 experience for some reason. 
		* Your trainer in Camelot should give you your epic armor, with another congratulations. 
		* The description of this quest was done by a Wizard. Other Academy classes might be slightly different. Also, this quest takes into consideration that you gave the knife to Esmond at the end of the 48 epic quest, which may or may not be a big deal.
        */

		#endregion
	}
}