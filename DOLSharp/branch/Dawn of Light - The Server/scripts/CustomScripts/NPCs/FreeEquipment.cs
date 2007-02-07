using System;
using System.Reflection;

using DOL.Events;
using DOL.GS.PacketHandler;

using log4net;

namespace DOL.GS.Scripts
{
	public class FreeEquipment
	{
		public const int FREE_LEVEL = 19;
		private static GameNPC Albion_FreeEquipment = null;
		private static GameNPC Midgard_FreeEquipment = null;
		private static GameNPC Hibernia_FreeEquipment = null;

		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		[ScriptLoadedEvent]
		public static void ScriptLoaded(DOLEvent e, object sender, EventArgs args)
		{
			GameNpcInventoryTemplate template = new GameNpcInventoryTemplate();
			template.AddNPCEquipment(eInventorySlot.TwoHandWeapon, 1658);
			template.AddNPCEquipment(eInventorySlot.Cloak, 1720);
			template.AddNPCEquipment(eInventorySlot.TorsoArmor, 2245);
			template = template.CloseTemplate();

			string name = "Equipment Master";
			string guildname = "Free Equipment";
			byte size = 60;
			byte level = 50;

			Albion_FreeEquipment = new FreeEquipmentNPC();
			Albion_FreeEquipment.Model = 280;
			Albion_FreeEquipment.Name = name;
			Albion_FreeEquipment.GuildName = guildname;
			Albion_FreeEquipment.Realm = (byte)eRealm.Albion;
			Albion_FreeEquipment.CurrentRegionID = 10;
			Albion_FreeEquipment.Size = size;
			Albion_FreeEquipment.Level = level;
			Albion_FreeEquipment.X = 34650;
			Albion_FreeEquipment.Y = 31590;
			Albion_FreeEquipment.Z = 7877;
			Albion_FreeEquipment.Heading = 1807;
			Albion_FreeEquipment.Inventory = template;
			Albion_FreeEquipment.AddToWorld();
			Albion_FreeEquipment.SwitchWeapon(GameLiving.eActiveWeaponSlot.TwoHanded);

			Midgard_FreeEquipment = new FreeEquipmentNPC();
			Midgard_FreeEquipment.Model = 160;
			Midgard_FreeEquipment.Name = name;
			Midgard_FreeEquipment.GuildName = guildname;
			Midgard_FreeEquipment.Realm = (byte)eRealm.Midgard;
			Midgard_FreeEquipment.CurrentRegionID = 101;
			Midgard_FreeEquipment.Size = size;
			Midgard_FreeEquipment.Level = level;
			Midgard_FreeEquipment.X = 34621;
			Midgard_FreeEquipment.Y = 33893;
			Midgard_FreeEquipment.Z = 8003;
			Midgard_FreeEquipment.Heading = 3955;
			Midgard_FreeEquipment.Inventory = template;
			Midgard_FreeEquipment.AddToWorld();
			Midgard_FreeEquipment.SwitchWeapon(GameLiving.eActiveWeaponSlot.TwoHanded);

			Hibernia_FreeEquipment = new FreeEquipmentNPC();
			Hibernia_FreeEquipment.Model = 340;
			Hibernia_FreeEquipment.Name = name;
			Hibernia_FreeEquipment.GuildName = guildname;
			Hibernia_FreeEquipment.Realm = (byte)eRealm.Hibernia;
			Hibernia_FreeEquipment.CurrentRegionID = 201;
			Hibernia_FreeEquipment.Size = size;
			Hibernia_FreeEquipment.Level = level;
			Hibernia_FreeEquipment.X = 24004;
			Hibernia_FreeEquipment.Y = 30386;
			Hibernia_FreeEquipment.Z = 7137;
			Hibernia_FreeEquipment.Heading = 4360;
			Hibernia_FreeEquipment.Inventory = template;
			Hibernia_FreeEquipment.AddToWorld();
			Hibernia_FreeEquipment.SwitchWeapon(GameLiving.eActiveWeaponSlot.TwoHanded);

			if (log.IsInfoEnabled)
				log.Info("Free Equipment Loaded");
		}
	}

	public class FreeEquipmentNPC : GameNPC
	{
		public override bool Interact(GamePlayer player)
		{
			if (!base.Interact(player))
				return false;

			if (player.Level < 20)
			{
				SayTo(player, "You need to be level 20!");
				return false;
			}

			SayTo(player, "Would you like some [equipment] suitable for level " + FreeEquipment.FREE_LEVEL + "?");
			return true;
		}

		public override bool WhisperReceive(GameLiving source, string str)
		{
			if (!base.WhisperReceive(source, str))
				return false;

			GamePlayer player = source as GamePlayer;
			if (player == null)
				return false;

			if (player.Level < 20)
				return false;

			switch (str)
			{
				case "equipment":
					{
						switch ((eCharacterClass)player.CharacterClass.ID)
						{
							case eCharacterClass.Animist:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Armsman:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Plate, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Plate, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Plate, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Plate, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Plate, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Plate, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.PolearmWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.PolearmWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.PolearmWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.TwoHandedWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.TwoHandedWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.TwoHandedWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.SlashingWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.ThrustWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.CrushingWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Large));
									break;
								}
							case eCharacterClass.Bainshee:
								break;
							case eCharacterClass.Bard:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Instrument, FreeEquipment.FREE_LEVEL, player, (eDamageType)1));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Instrument, FreeEquipment.FREE_LEVEL, player, (eDamageType)2));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Instrument, FreeEquipment.FREE_LEVEL, player, (eDamageType)3));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blunt, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									break;
								}
							case eCharacterClass.Berserker:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Hammer, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Hammer, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Hammer, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Sword, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Sword, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Sword, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Axe, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Axe, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.LeftAxe, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									break;
								}
							case eCharacterClass.Blademaster:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blades, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Blades, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blunt, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Blunt, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Piercing, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Piercing, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									break;
								}
							case eCharacterClass.Bonedancer:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Cabalist:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Champion:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Scale, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Scale, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Scale, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Scale, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Scale, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Scale, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blades, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blunt, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Piercing, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.LargeWeapons, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.LargeWeapons, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Cleric:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.CrushingWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Druid:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Scale, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Scale, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Scale, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Scale, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Scale, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Scale, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blades, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blunt, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Eldritch:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Enchanter:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Friar:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.CrushingWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Healer:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Hammer, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Hammer, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Hammer, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Hammer, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									break;
								}
							case eCharacterClass.Heretic:
								break;
							case eCharacterClass.Hero:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Scale, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Scale, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Scale, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Scale, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Scale, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Scale, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Large));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blades, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blunt, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Piercing, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.LargeWeapons, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.LargeWeapons, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.CelticSpear, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									break;
								}
							case eCharacterClass.Hunter:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Spear, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Spear, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.DistanceWeapon, eObjectType.CompositeBow, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));

									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Sword, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Sword, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Infiltrator:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.SlashingWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.ThrustWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.SlashingWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.ThrustWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									break;
								}
							case eCharacterClass.Mentalist:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Mercenary:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.SlashingWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.ThrustWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.SlashingWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.ThrustWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.CrushingWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.CrushingWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Minstrel:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.SlashingWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.ThrustWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									break;
								}
							case eCharacterClass.Necromancer:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Nightshade:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blades, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Blades, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));

									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Piercing, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Piercing, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									break;
								}
							case eCharacterClass.Paladin:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Plate, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Plate, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Plate, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Plate, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Plate, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Plate, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Large));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.TwoHandedWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.TwoHandedWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.TwoHandedWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.SlashingWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.ThrustWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.CrushingWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Ranger:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.DistanceWeapon, eObjectType.RecurvedBow, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));

									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blades, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Blades, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));

									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Piercing, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Piercing, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									break;
								}
							case eCharacterClass.Reaver:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.SlashingWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.ThrustWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.CrushingWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Flexible, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Large));
									break;
								}
							case eCharacterClass.Runemaster:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Savage:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.HandToHand, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.HandToHand, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Hammer, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Hammer, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Sword, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Sword, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Axe, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Axe, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									break;
								}
							case eCharacterClass.Scout:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Studded, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.SlashingWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.ThrustWeapon, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.DistanceWeapon, eObjectType.Longbow, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									break;
								}
							case eCharacterClass.Shadowblade:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Leather, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Sword, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Sword, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Sword, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Axe, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Axe, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.LeftAxe, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									break;
								}
							case eCharacterClass.Shaman:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Hammer, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Hammer, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Skald:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Hammer, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Hammer, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Sword, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Sword, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Axe, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Axe, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									break;
								}
							case eCharacterClass.Sorcerer:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Spiritmaster:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Thane:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Hammer, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Hammer, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Sword, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Sword, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Axe, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Axe, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									break;
								}
							case eCharacterClass.Theurgist:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Valewalker:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Scythe, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Scythe, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Scythe, FreeEquipment.FREE_LEVEL, player, eDamageType.Thrust));
									break;
								}
							case eCharacterClass.Valkyrie:
								break;
							case eCharacterClass.Vampiir:
								break;
							case eCharacterClass.Warden:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Reinforced, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blades, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blunt, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Warlock:
								break;
							case eCharacterClass.Warrior:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Chain, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, FreeEquipment.FREE_LEVEL, player, (eDamageType)ShieldLevel.Large));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Hammer, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Hammer, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Sword, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Sword, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Axe, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Axe, FreeEquipment.FREE_LEVEL, player, eDamageType.Slash));
									break;
								}
							case eCharacterClass.Wizard:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, FreeEquipment.FREE_LEVEL, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, FreeEquipment.FREE_LEVEL, player, eDamageType.Crush));
									break;
								}
						}
						break;
					}
			}
			return true;
		}
	}
}