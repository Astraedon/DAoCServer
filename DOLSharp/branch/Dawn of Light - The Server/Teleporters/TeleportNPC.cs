/*
 * Author:		Kristopher Gilbert <ogre@fallenrealms.net>
 * Rev:			$Id: TeleportNPC.cs 361 2005-04-07 02:02:53Z Rudal $
 * Copyright:	2004 by Hired Goons, LLC
 * License:		http://www.gnu.org/licenses/gpl.txt
 * 
 * Base class for Teleporter NPC's.
 * 
 */

using System.Collections;

using DOL.Database;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.GS.ServerRules;

using log4net;

namespace DOL.GS.GameEvents
{
	public class TeleportNPC : GameNPC
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public int m_index = -1;
		public ArrayList m_locs;
		public string m_message;
		public Queue m_animTeleportTimerQueue = new Queue();
		public Queue m_portTeleportTimerQueue = new Queue();
		public Queue m_animPlayerQueue = new Queue();
		public Queue m_portPlayerQueue = new Queue();
		public Queue m_portDestinationQueue = new Queue();
		public bool IsSummoned = false;
		protected GameNPC sfx;

		public TeleportNPC()
			: base()
		{
			m_message = "I can teleport you to various major towns around your realm. Which town would you like me to teleport you to?\n\n";

			Level = 60;
			Size = 55;
			Name = "Teleporter";
			GuildName = "Teleporter";
			GameNpcInventoryTemplate template = new GameNpcInventoryTemplate();
			template.AddNPCEquipment(eInventorySlot.TwoHandWeapon, 1658);
			template.AddNPCEquipment(eInventorySlot.Cloak, 1720);
			template.AddNPCEquipment(eInventorySlot.TorsoArmor, 2245);
			template = template.CloseTemplate();
			Inventory = template;

			//Rudal - Needed for Teleporters with Staff in Two Handed Slot
			if ((template.GetItem(eInventorySlot.TwoHandWeapon) != null) && (template.GetItem(eInventorySlot.RightHandWeapon) == null))
				SwitchWeapon(eActiveWeaponSlot.TwoHanded);

			Flags ^= (uint)GameNPC.eFlags.PEACE;
		}

		//constructor
        public TeleportNPC(ushort region, int x, int y, int z, ushort heading,
                             ushort model, byte realm, string name, string guild,
                             ArrayList locations, int index, string message)
            : base()
        {
            m_locs = locations;
            m_index = index;
            m_message = message;

            CurrentRegionID = region;
            X = x;
            Y = y;
            Z = z;
            Heading = heading;

            Realm = realm;
            Model = model;
            Name = name;
            GuildName = guild;

            Level = 60;
            Size = 55;
            GameNpcInventoryTemplate template = new GameNpcInventoryTemplate();
            template.AddNPCEquipment(eInventorySlot.TwoHandWeapon, 1658);
            template.AddNPCEquipment(eInventorySlot.Cloak, 1720);
            template.AddNPCEquipment(eInventorySlot.TorsoArmor, 2245);
            template = template.CloseTemplate();
            Inventory = template;

            //Rudal - Needed for Teleporters with Staff in Two Handed Slot
            if ((template.GetItem(eInventorySlot.TwoHandWeapon) != null) && (template.GetItem(eInventorySlot.RightHandWeapon) == null))
                SwitchWeapon(eActiveWeaponSlot.TwoHanded);

			Flags ^= (uint)GameNPC.eFlags.PEACE;

        }
		//callbacks
		public override bool Interact(GamePlayer player)
		{
			if (!base.Interact(player)) return false;

			if (player.Client.Account.PrivLevel == 1 && Realm != player.Realm)
				return false;

			TurnTo(player.X, player.Y);

			if (m_locs == null)
			{
				switch (player.Realm)
				{
					case 1: m_locs = RvRTeleportNPCEvent.AlbLocs; break;
					case 2: m_locs = RvRTeleportNPCEvent.MidLocs; break;
					case 3: m_locs = RvRTeleportNPCEvent.HibLocs; break;
				}
			}

			string message = player.Name + " the " + player.CharacterClass.Name + ", " + m_message;

			foreach (ITeleportLocation location in m_locs)
			{
				if (m_locs.IndexOf(location) == m_index)
					continue;
				message += "[" + location.Name + "]\n";
			}

			SayTo(player, message);

			return true;
		}

		public override bool WhisperReceive(GameLiving source, string str)
		{
			if (!base.WhisperReceive(source, str)) return false;

			if (!GameServer.ServerRules.IsSameRealm(this, source , true))
				return false;

			if (!(source is GamePlayer)) return false;
			GamePlayer player = (GamePlayer)source;
			TurnTo(player.X, player.Y);
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

			bool isPorting = false;
			ITeleportLocation location = null;

			foreach (ITeleportLocation l in m_locs)
			{
				if (l == null || l.Name != str)
					continue;
				if (l is SpecialLocation)
				{
					SpecialLocation sloc = (l as SpecialLocation);
					switch (sloc.Location)
					{
						case SpecialLocation.eSpecialLocation.PersonalHouse:
							{
								if (Housing.HouseMgr.GetHouseNumberByPlayer(player) == 0)
								{
									SendReply(player, "You don't have a house!");
									continue;
								}
								break;
							}
					}
				}
				location = l;
				if (l is LocationExpansion)
				{
					LocationExpansion locationex = l as LocationExpansion;
					bool good = true;
					if ((int)player.Client.ClientType < (int)locationex.Software)
					{
						SendReply(player, "You don't have the right client type. (" + locationex.Software.ToString() + ")");
						good = false;
					}
					if ((int)player.Client.ClientAddons < (int)locationex.Expansions)
					{
						SendReply(player, "You don't have the right expansion. (" + locationex.Expansions.ToString() + ")");
						good = false;
					}
					if ((int)player.Client.Version < (int)locationex.Version)
					{
						SendReply(player, "You don't have the right client version. (" + locationex.Version.ToString() + ")");
						good = false;
					}
					if (locationex.MinLevel > 0 && player.Level < locationex.MinLevel)
					{
						SendReply(player, "You don't have the minimum level. (" + locationex.MinLevel + ")");
						good = false;
					}
					if (locationex.MaxLevel > 0 && player.Level > locationex.MaxLevel)
					{
						SendReply(player, "You exceed the level requirement. (" + locationex.MaxLevel + ")");
						good = false;
					}
					isPorting = good;
				}
				else isPorting = true;
				if (isPorting)
					break;
			}

			if (!isPorting)
				SendReply(player, "I can't port you there!");
			else
			{
				Say("Enjoy the ride to " + str + ", " + player.Name + "...");

				m_animTeleportTimerQueue.Enqueue(new RegionTimer(this, new RegionTimerCallback(MakeAnimSequence), 2000));
				m_portTeleportTimerQueue.Enqueue(new RegionTimer(this, new RegionTimerCallback(MakePortSequence), 3000));

				m_animPlayerQueue.Enqueue(source);
				m_portPlayerQueue.Enqueue(player);
				m_portDestinationQueue.Enqueue(location);

				foreach (GamePlayer p in GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
				{
					p.Out.SendSpellCastAnimation(this, 1, 20);
				}
			}
			return true;
		}
		public override IList GetExamineMessages(GamePlayer player)
		{
			IList list = new ArrayList();
			list.Add("You target [" + GetName(0, false) + "] it is a Translocator");
			return list;
		}
		//timer callbacks
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

        protected static int SameRegionImmunity = 10;
        protected static int DiffRegionImmunity = 60;
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
		//utility
		public void SendReply(GamePlayer target, string msg)
		{
			target.Out.SendMessage(msg, eChatType.CT_Say, eChatLoc.CL_PopupWindow);
		}

		public override bool AddToWorld()
		{
			if (!base.AddToWorld()) return false;
			GameNPC mob = new GameNPC();
			mob.Name = "teleport spell effect";
			mob.Flags = (uint)GameNPC.eFlags.PEACE + (uint)GameNPC.eFlags.DONTSHOWNAME;
			mob.Size = 255;
			mob.Realm = Realm;
			mob.CurrentRegion = this.CurrentRegion;
			mob.X = this.X;
			mob.Y = this.Y;
			mob.Z = this.Z;
			mob.Model = 0x783;
			mob.Heading = this.Heading;
			mob.MaxSpeedBase = 0;
			if (mob.AddToWorld())
				sfx = mob;

			if (!IsSummoned && this.NPCTemplate == null)
			{
				BindPoint bp = new BindPoint();
				bp.Realm = Realm;
				bp.Region = this.CurrentRegionID;
				bp.Radius = 1000;
				bp.X = this.X;
				bp.Y = this.Y;
				bp.Z = this.Z;
				this.CurrentRegion.AddArea(new Area.BindArea("bind point", bp));
			}
			return true;
		}
		public override bool RemoveFromWorld()
		{
			if (!base.RemoveFromWorld()) return false;
			if (sfx != null)
				sfx.Delete();
			return true;
		}
	}
}
