/*
 * Written by Supgee
 * For: [Official] Dawn of Light
 */

using System;
using System.Reflection;
using DOL.Events;
using log4net;
using DOL;
using DOL.GS;
using DOL.Database;
using System.Collections;
using DOL.GS.PacketHandler;

namespace DOL.GS.GameEvents
{
	public class OfficialTeleporterAlbion : GameNPC
	{
		private static ArrayList NPCS = new ArrayList();
		public Queue m_animTeleportTimerQueue = new Queue();
		public Queue m_portTeleportTimerQueue = new Queue();
		public Queue m_animPlayerQueue = new Queue();
		public Queue m_portPlayerQueue = new Queue();
		public Queue m_portDestinationQueue = new Queue();

		public OfficialTeleporterAlbion(int x, int y, int z, ushort heading, ushort region, string name, ushort model)
			: base()
		{
			X = x;
			Y = y;
			Z = z;
			Heading = heading;
			CurrentRegionID = region;
			Name = name;
			Model = model;
			Flags |= (uint)GameNPC.eFlags.PEACE;
			GameNpcInventoryTemplate template = new GameNpcInventoryTemplate();
			template.AddNPCEquipment(eInventorySlot.TwoHandWeapon, 1166, 0);
			template.AddNPCEquipment(eInventorySlot.Cloak, 57, 27);
			template.AddNPCEquipment(eInventorySlot.TorsoArmor, 58, 36);
			template = template.CloseTemplate();
			Inventory = template;

			if ((template.GetItem(eInventorySlot.TwoHandWeapon) != null) && (template.GetItem(eInventorySlot.RightHandWeapon) == null))
				SwitchWeapon(eActiveWeaponSlot.TwoHanded);
		}
		private ushort spell = 13509; //The spell effect which is launched
		private Queue m_timer = new Queue();//Gametimer for casting some spell at the end of the process
		private Queue castplayer = new Queue();//Used to hold the player who the spell gets cast on
		protected GameNPC sfx; //The definition for the effect mob

		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		[ScriptLoadedEvent]
		public static void ScriptLoaded(DOLEvent e, object sender, EventArgs args)
		{
			NPCS.Add(new OfficialTeleporterAlbion(584775, 486868, 2207, 978, 1, "Master Omerus", 61));
			NPCS.Add(new OfficialTeleporterAlbion(517051, 372900, 8208, 471, 1, "Master Ilius", 61));
			NPCS.Add(new OfficialTeleporterAlbion(461889, 632726, 1705, 6, 1, "Apprentice Ni'tul", 716));
			NPCS.Add(new OfficialTeleporterAlbion(585144, 561107, 3576, 429, 2, "Channeler Bal'zam", 716));
			NPCS.Add(new OfficialTeleporterAlbion(270871, 540682, 8344, 2498, 73, "Master Jo'an", 61));
			NPCS.Add(new OfficialTeleporterAlbion(525716, 542469, 3168, 76, 51, "Master Aslin", 716));
			NPCS.Add(new OfficialTeleporterAlbion(434244, 493301, 3088, 2638, 51, "Master O'ak", 716));
			NPCS.Add(new OfficialTeleporterAlbion(426686, 416359, 5712, 3525, 51, "Master Auk", 716));
			NPCS.Add(new OfficialTeleporterAlbion(403459, 502918, 4680, 631, 51, "Apprentice Aur'utk", 716));
			NPCS.Add(new OfficialTeleporterAlbion(36041, 30875, 8001, 2494, 10, "Channeler Deng'ani", 716));
			NPCS.Add(new OfficialTeleporterAlbion(562661, 574245, 5408, 2535, 238, "Channeler Thidranki", 716));
			NPCS.Add(new OfficialTeleporterAlbion(583772, 584520, 4896, 381, 165, "Channeler Cathal", 716));

			foreach (GameNPC npc in NPCS)
			{
				GameNPC[] npcs = WorldMgr.GetNPCsByName(npc.Name, eRealm.Albion);
				if (npcs.Length > 1)
					npcs[0].RemoveFromWorld();
				npc.GuildName = "Teleporter";
				npc.Realm = 1;
				npc.Level = 59;
				npc.Size = 50;
				npc.AddToWorld();
			}

			if (log.IsInfoEnabled)
				log.Info("Supgee's Teleporter(Albion) initialized");
		}

		[ScriptUnloadedEvent]
		public static void OnScriptUnloaded(DOLEvent e, object sender, EventArgs args)
		{
			//To stop this event, we simply delete
			//(remove from world completly) the npc
			foreach (GameNPC npc in NPCS)
			{
				npc.Delete();
			}
		}

		public override bool Interact(GamePlayer player)
		{
			//This function is the callback function that is called when
            //a player right clicks on the npc
            if (!WorldMgr.CheckDistance(this, player, WorldMgr.INTERACT_DISTANCE))
            {
                player.Out.SendMessage("You are too far away to speak with " + GetName(0, false) + ".", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return false;
            }
            
            //This function is the callback function that is called when
			//a player right clicks on the npc
			if (!base.Interact(player))
				return false;

			//Now we turn the npc into the direction of the person it is
			//speaking to.
			TurnTo(player.X, player.Y);

			//We send a message to player and make it appear in a popup
			//window. Text inside the [brackets] is clickable in popup
			//windows and will generate a &whis text command!
			player.Out.SendMessage(this.Name + " says, Greetings. I can channel the energies of this place to send you to far away lands. If you wish to fight in the Frontiers I can send you to [Forest Sauvage] or to the border keeps [Castle Sauvage] and [Snowdonia Fortress]. Maybe you wish to undertake the Trials of Atlantis in [Oceanus] haven or wish to visit the harbor of [Gothwaite] and the [Shrouded Isles]? You could explore the [Avalon Marsh] or prehaps you would prefer the comforts of the [housing] regions. Perhaps the fierce [Battlegrounds] are more to your liking or do you wish to meet the citizens inside the great city of [Camelot] or the [Inconnu Crypt]?", eChatType.CT_System, eChatLoc.CL_PopupWindow);
			return true;

		}

		//This function is the callback function that is called when
		//someone whispers something to this mob!
		public override bool WhisperReceive(GameLiving source, string str)
		{

			if (!base.WhisperReceive(source, str))
				return false;

			//If the source is no player, we return false.
			if (!(source is GamePlayer))
				return false;

			//We cast our source to a GamePlayer object
			GamePlayer player = (GamePlayer)source;

			//Now we turn the npc into the direction of the person it is
			//speaking to.
			TurnTo(player.X, player.Y);

			//lets make sure we should teleport the player
			if (m_animPlayerQueue.Contains(player))
			{
				SayTo(player, "You are already being translocated somewhere!");
				return false;
			}
			if (player.InCombat)
			{
				SayTo(player, "I'm sorry, translocative magic at work here doesn't work when you're in combat.");
				return false;
			}

			//We test what the player whispered to the npc and
			//send a reply.
			switch (str)
			{
				case "Forest Sauvage":
					if (WorldMgr.GetRegion(163).IsDisabled)
					{
						SendReply(player, "This zone is disabled!");
						return false;
					}
					else
						SendReply(player, "Now too the frontiers, for the glory of the realm!");

					Teleport(new Location("", 163, 654491, 617288, 9560, 1546), player);
					break;

				case "Castle Sauvage":
					if (WorldMgr.GetRegion(1).IsDisabled)
					{
						SendReply(player, "This zone is Disabled!");
						return false;
					}
					else
						SendReply(player, "Castle Sauvage is what you seek, and Castle Sauvage is what you shall find.");

					Teleport(new Location("", 1, 583131, 486577, 2184, 3247), player);
					break;

				case "Snowdonia Fortress":
					if (WorldMgr.GetRegion(1).IsDisabled)
					{
						SendReply(player, "This zone is Disabled!");
						return false;
					}
					else
						SendReply(player, "Snowdonia Fortress is what you seek, and Snowdonia Fortress is what you shall find.");

					Teleport(new Location("", 1, 515777, 372527, 8208, 3483), player);
					break;

				case "Oceanus":
					if (WorldMgr.GetRegion(73).IsDisabled)
					{
						SendReply(player, "This zone is Disabled!");
						return false;
					}
					else
						SendReply(player, "Oceanus Hesperos is what you seek, and Oceanus Hesperos is what you shall find.");

					Teleport(new Location("", 73, 271236, 540271, 8344, 616), player);
					break;

				case "Gothwaite":
					if (WorldMgr.GetRegion(51).IsDisabled)
					{
						SendReply(player, "This zone is Disabled!");
						return false;
					}
					else
						SendReply(player, "Gothwaite Harbor is what you seek, and Gothwaite Harbor is what you shall find.");

					Teleport(new Location("", 51, 526542, 541271, 3168, 17), player);
					break;

				case "Shrouded Isles":
					SendReply(player, "The isles of Avalon are an excellent choice.\n Would you prefer the harbor of [Gothwaite] or perhaps one of the outlying towns like [Wearyall] Village, Fort [Gwyntell], or Caer [Diogel]?");
					break;

				case "Wearyall":
					if (WorldMgr.GetRegion(51).IsDisabled)
					{
						SendReply(player, "This zone is Disabled!");
						return false;
					}
					else
						SendReply(player, "The Shrouded Isles awaits.");

					Teleport(new Location("", 51, 535986, 493768, 3088, 0), player);
					break;

				case "Gwyntell":
					if (WorldMgr.GetRegion(51).IsDisabled)
					{
						SendReply(player, "This zone is Disabled!");
						return false;
					}
					else
						SendReply(player, "The Shrouded Isles awaits.");

					Teleport(new Location("", 51, 426449, 416456, 5712, 3427), player);
					break;

				case "Diogel":
					if (WorldMgr.GetRegion(51).IsDisabled)
					{
						SendReply(player, "This zone is Disabled!");
						return false;
					}
					else
						SendReply(player, "The Shrouded Isles awaits.");

					Teleport(new Location("", 51, 403464, 502929, 4680, 763), player);
					break;

				case "Avalon Marsh":
					if (WorldMgr.GetRegion(1).IsDisabled)
					{
						SendReply(player, "This zone is Disabled!");
						return false;
					}
					else
						SendReply(player, "Avalon Marsh is what you seek, and Avalon Marsh is what you shall find.");
					Teleport(new Location("", 1, 462031, 632938, 1709, 3611), player);
					break;

				case "housing":
					SendReply(player, "I can send you too your [personal] house. If you do not have a personal house or wish to be sent too to the housing [entrance] then you will arrive just inside the housing area. I can also send you to your [guild] house. If your guild does not own a house then you will not be teleported. You may go to your [Hearth] bind as well if you are bound inside a house."); ;
					break;

				case "entrance":
					if (WorldMgr.GetRegion(2).IsDisabled)
					{
						SendReply(player, "This zone is Disabled!");
						return false;
					}
					else
						SendReply(player, "Enjoy the comforts of the housing region.");

					Teleport(new Location("", 2, 584935, 561385, 3576, 1938), player);
					break;

				case "personal":
					Teleport(new SpecialLocation("", SpecialLocation.eSpecialLocation.PersonalHouse), player);
					break;

				case "guild":
					SendReply(player, "This feature is not supported at this time.");
					break;

				case "Hearth":
					SendReply(player, "This feature is not supported at this time.");
					break;

				case "Battlegrounds":
					SendReply(player, "Which battleground would you like me too send you too?\n [Thidranki]\n or\n [Cathal Valley]?");
					break;

				case "Thidranki":
					if (WorldMgr.GetRegion(238).IsDisabled)
					{
						SendReply(player, "This zone is Disabled!");
						return false;
					}
					if (player.Level < 20)
					{
						SendReply(player, "You are too low level for this Battleground.");
						return false;
					}
					if (player.Level > 24)
					{
						SendReply(player, "You are too high level for this Battleground.");
						return false;
					}
					else
						SendReply(player, "Now too the fierce battleground, Thidranki!");

					Teleport(new Location("", 238, 563469, 574118, 5408, 2132), player);
					break;

				case "Cathal Valley":
					if (WorldMgr.GetRegion(165).IsDisabled)
					{
						SendReply(player, "This zone is Disabled!");
						return false;
					}
					if (player.Level != 50)
					{
						SendReply(player, "Only adventurers that are level 50, may enter this area");
						return false;
					}
					else
						SendReply(player, "Now too the fierce battleground, Cathal Valley!");

					Teleport(new Location("", 165, 583322, 585178, 4896, 2049), player);
					break;

				case "Camelot":
					if (WorldMgr.GetRegion(10).IsDisabled)
					{
						SendReply(player, "This zone is Disabled!");
						return false;
					}
					else
						SendReply(player, "Onward too the Great City.");

					Teleport(new Location("", 10, 36282, 29782, 7977, 5), player);
					break;

				case "Inconnu Crypt":
					SendReply(player, "I may only send those who know the way to this city. Seek out the path to the city and in future times I will aid you on this journey."); ;
					//You need to touch an obelisk before you can teleport here, not supported yet.
					break;
				default: break;
			}
			return true;
		}

		// This function adds the blue teleporter effect under the NPC's feet.
		public override bool AddToWorld()
		{
			if (!base.AddToWorld()) return false;
			GameNPC mob = new GameNPC();
			mob.Name = "teleport spell effect";
			mob.Flags = (uint)GameNPC.eFlags.PEACE + (uint)GameNPC.eFlags.DONTSHOWNAME;
			mob.Size = 255;
			mob.CurrentRegion = this.CurrentRegion;
			mob.X = this.X;
			mob.Y = this.Y;
			mob.Z = this.Z;
			mob.Model = 0x783;
			mob.Heading = this.Heading;
			if (mob.AddToWorld())
				sfx = mob;
			return true;
		}

		// This function removes the blue teleport effect when the server is taken down.
		public override bool RemoveFromWorld()
		{
			if (!base.RemoveFromWorld()) return false;
			if (sfx != null)
				sfx.Delete();
			return true;
		}

		//This function sends some text to a player and makes it appear
		//in a popup window! We define it here ,that way we dont need to keep calling it everytime!
		private void SendReply(GamePlayer target, string msg)
		{
			target.Out.SendMessage(msg, eChatType.CT_System, eChatLoc.CL_PopupWindow);
		}

		//This function teleports the player
		public void Teleport(ITeleportLocation location, GamePlayer player)
		{
			TurnTo(player.X, player.Y);

			m_animTeleportTimerQueue.Enqueue(new RegionTimer(this, new RegionTimerCallback(MakeAnimSequence), 2000));
			m_portTeleportTimerQueue.Enqueue(new RegionTimer(this, new RegionTimerCallback(MakePortSequence), 3000));

			m_animPlayerQueue.Enqueue(player);
			m_portPlayerQueue.Enqueue(player);
			m_portDestinationQueue.Enqueue(location);
            foreach (GamePlayer p in GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
            {
                p.Out.SendSpellCastAnimation(this, spell, 20);
            }
        }
        
        //This function is the method for the Animation Sequence
		protected virtual int MakeAnimSequence(RegionTimer timer)
		{
			m_animTeleportTimerQueue.Dequeue();
			GamePlayer animPlayer = (GamePlayer)m_animPlayerQueue.Dequeue();
			foreach (GamePlayer player in animPlayer.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
			{
				player.Out.SendSpellEffectAnimation(this, animPlayer, 4310, 0, false, 0x1);
				//player.Out.SendEmoteAnimation(animPlayer, eEmote.Bind);
			}
			return 0;
		}

		//This function is the method for the Teleportation Sequence
        protected virtual int MakePortSequence(RegionTimer timer)
		{
			m_portTeleportTimerQueue.Dequeue();
			GamePlayer player = (GamePlayer)m_portPlayerQueue.Dequeue();
			ITeleportLocation location = (ITeleportLocation)m_portDestinationQueue.Dequeue();
			if (location is Location)
			{
				Location loc = location as Location;
				int x, y;
				loc.getDestinationSpot(out x, out y);

				player.MoveTo(loc.CurrentRegionID, x, y, loc.Z, loc.Heading);
			}
			if (location is SpecialLocation)
			{
				SpecialLocation sloc = location as SpecialLocation;
				switch (sloc.Location)
				{
					case SpecialLocation.eSpecialLocation.PersonalHouse:
						{
							Housing.HouseMgr.GetHouse(Housing.HouseMgr.GetHouseNumberByPlayer(player)).Exit(player, true);
							break;
						}
					case SpecialLocation.eSpecialLocation.BindPoint:
						{
							player.MoveTo((ushort)player.PlayerCharacter.BindRegion, player.PlayerCharacter.BindXpos, player.PlayerCharacter.BindYpos, player.PlayerCharacter.BindZpos, (ushort)player.PlayerCharacter.BindHeading);
							break;
						}
				}
			}
			return 0;
		}
	}
}