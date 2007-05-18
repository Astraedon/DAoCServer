using System;
using System.Reflection;

using DOL.Events;
using DOL.GS.PacketHandler;

using log4net;

namespace DOL.GS.Scripts
{
	public class FreeEquipment
	{
		public static byte[] FreeEquipmentLevels = new byte[] { 5, 10, 20, 30, 40, 45, 49 };
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

			if (player.Level < 5)
			{
				SayTo(player, "I'm sorry " + player.Name + ", but I only give equipment out to people above level 5.");
				return false;
			}

			byte chosenlevel = 0;
			foreach (byte level in FreeEquipment.FreeEquipmentLevels)
			{
				if (player.Level > level && level > chosenlevel)
					chosenlevel = level;
			}

			SayTo(player, "Would you like some [equipment] suitable for level " + chosenlevel + "?");
			return true;
		}

		public override bool WhisperReceive(GameLiving source, string str)
		{
			if (!base.WhisperReceive(source, str))
				return false;

			GamePlayer player = source as GamePlayer;
			if (player == null)
				return false;

			if (player.Level < 5)
				return false;

			byte chosenlevel = 0;
			foreach (byte level in FreeEquipment.FreeEquipmentLevels)
			{
				if (player.Level > level && level > chosenlevel)
					chosenlevel = level;
			}

			chosenlevel -= 1;

			switch (str)
			{
				case "equipment":
					{
						switch ((eCharacterClass)player.CharacterClass.ID)
						{
							case eCharacterClass.Animist:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Armsman:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Plate, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Plate, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Plate, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Plate, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Plate, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Plate, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.PolearmWeapon, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.PolearmWeapon, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.PolearmWeapon, chosenlevel, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.TwoHandedWeapon, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.TwoHandedWeapon, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.TwoHandedWeapon, chosenlevel, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.SlashingWeapon, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.ThrustWeapon, chosenlevel, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.CrushingWeapon, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Large));
									break;
								}
							case eCharacterClass.Bainshee:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Bard:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Instrument, chosenlevel, player, (eDamageType)1));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Instrument, chosenlevel, player, (eDamageType)2));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Instrument, chosenlevel, player, (eDamageType)3));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blunt, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									break;
								}
							case eCharacterClass.Berserker:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Hammer, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Hammer, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Hammer, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Sword, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Sword, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Sword, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Axe, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Axe, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.LeftAxe, chosenlevel, player, eDamageType.Slash));
									break;
								}
							case eCharacterClass.Blademaster:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blades, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Blades, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blunt, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Blunt, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Piercing, chosenlevel, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Piercing, chosenlevel, player, eDamageType.Thrust));
									break;
								}
							case eCharacterClass.Bonedancer:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Cabalist:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Champion:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Scale, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Scale, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Scale, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Scale, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Scale, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Scale, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blades, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blunt, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Piercing, chosenlevel, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.LargeWeapons, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.LargeWeapons, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Cleric:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.CrushingWeapon, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Druid:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Scale, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Scale, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Scale, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Scale, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Scale, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Scale, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blades, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blunt, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Eldritch:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Enchanter:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Friar:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.CrushingWeapon, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Healer:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Hammer, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Hammer, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Hammer, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Hammer, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Heretic:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Hammer, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Flexible, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Flexible, chosenlevel, player, eDamageType.Slash));
									break;
								}
							case eCharacterClass.Hero:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Scale, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Scale, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Scale, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Scale, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Scale, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Scale, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Large));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blades, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blunt, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Piercing, chosenlevel, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.LargeWeapons, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.LargeWeapons, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.CelticSpear, chosenlevel, player, eDamageType.Thrust));
									break;
								}
							case eCharacterClass.Hunter:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Spear, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Spear, chosenlevel, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.DistanceWeapon, eObjectType.CompositeBow, chosenlevel, player, eDamageType.Thrust));

									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Sword, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Sword, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Infiltrator:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.SlashingWeapon, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.ThrustWeapon, chosenlevel, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.SlashingWeapon, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.ThrustWeapon, chosenlevel, player, eDamageType.Thrust));
									break;
								}
							case eCharacterClass.Mentalist:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Mercenary:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.SlashingWeapon, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.ThrustWeapon, chosenlevel, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.SlashingWeapon, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.ThrustWeapon, chosenlevel, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.CrushingWeapon, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.CrushingWeapon, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Minstrel:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.SlashingWeapon, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.ThrustWeapon, chosenlevel, player, eDamageType.Thrust));
									break;
								}
							case eCharacterClass.Necromancer:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Nightshade:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blades, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Blades, chosenlevel, player, eDamageType.Slash));

									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Piercing, chosenlevel, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Piercing, chosenlevel, player, eDamageType.Thrust));
									break;
								}
							case eCharacterClass.Paladin:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Plate, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Plate, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Plate, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Plate, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Plate, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Plate, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Large));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.TwoHandedWeapon, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.TwoHandedWeapon, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.TwoHandedWeapon, chosenlevel, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.SlashingWeapon, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.ThrustWeapon, chosenlevel, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.CrushingWeapon, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Ranger:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.DistanceWeapon, eObjectType.RecurvedBow, chosenlevel, player, eDamageType.Thrust));

									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blades, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Blades, chosenlevel, player, eDamageType.Slash));

									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Piercing, chosenlevel, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Piercing, chosenlevel, player, eDamageType.Thrust));
									break;
								}
							case eCharacterClass.Reaver:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.SlashingWeapon, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.ThrustWeapon, chosenlevel, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.CrushingWeapon, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Flexible, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Large));
									break;
								}
							case eCharacterClass.Runemaster:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Savage:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.HandToHand, chosenlevel, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.HandToHand, chosenlevel, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Hammer, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Hammer, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Sword, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Sword, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Axe, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Axe, chosenlevel, player, eDamageType.Slash));
									break;
								}
							case eCharacterClass.Scout:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Studded, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.SlashingWeapon, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.ThrustWeapon, chosenlevel, player, eDamageType.Thrust));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.DistanceWeapon, eObjectType.Longbow, chosenlevel, player, eDamageType.Thrust));
									break;
								}
							case eCharacterClass.Shadowblade:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Sword, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Sword, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Sword, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Axe, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Axe, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.LeftAxe, chosenlevel, player, eDamageType.Slash));
									break;
								}
							case eCharacterClass.Shaman:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Hammer, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Hammer, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Skald:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Hammer, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Hammer, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Sword, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Sword, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Axe, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Axe, chosenlevel, player, eDamageType.Slash));
									break;
								}
							case eCharacterClass.Sorcerer:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Spiritmaster:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Thane:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Hammer, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Hammer, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Sword, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Sword, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Axe, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Axe, chosenlevel, player, eDamageType.Slash));
									break;
								}
							case eCharacterClass.Theurgist:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Valewalker:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Scythe, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Scythe, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Scythe, chosenlevel, player, eDamageType.Thrust));
									break;
								}
							case eCharacterClass.Valkyrie:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Spear, chosenlevel, player, eDamageType.Thrust));
									break;
								}
							case eCharacterClass.Vampiir:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Leather, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Piercing, chosenlevel, player, eDamageType.Thrust));
									break;
								}
							case eCharacterClass.Warden:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Reinforced, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blades, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Blunt, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Warlock:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, chosenlevel, player, eDamageType.Crush));
									break;
								}
							case eCharacterClass.Warrior:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Chain, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Small));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Medium));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LeftHandWeapon, eObjectType.Shield, chosenlevel, player, (eDamageType)ShieldLevel.Large));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Hammer, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Hammer, chosenlevel, player, eDamageType.Crush));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Sword, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Sword, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Axe, chosenlevel, player, eDamageType.Slash));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.RightHandWeapon, eObjectType.Axe, chosenlevel, player, eDamageType.Slash));
									break;
								}
							case eCharacterClass.Wizard:
								{
									//armor
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.ArmsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.FeetArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HandsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.HeadArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.LegsArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TorsoArmor, eObjectType.Cloth, chosenlevel, player, eDamageType.Natural));
									//weapons
									player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, LootGeneratorUniqueObject.GenerateFreeEquipment(eInventorySlot.TwoHandWeapon, eObjectType.Staff, chosenlevel, player, eDamageType.Crush));
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