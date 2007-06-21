using System;
using System.Collections;
using System.Reflection;

using DOL.Events;
using DOL.Database;
using DOL.GS.PacketHandler;

using log4net;

namespace DOL.GS
{
	public class LootGeneratorUniqueObject : LootGeneratorBase
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		[ScriptLoadedEvent]
		public static void OnScriptLoaded(DOLEvent e, object sender, EventArgs args)
		{
			LootMgr.RegisterLootGenerator(new LootGeneratorUniqueObject(), "", "", "", 0);
			log.Info("ROG System Loaded!");
		}

		/// <summary>
		/// Generate loot for given mob
		/// </summary>
		/// <param name="mob"></param>
		/// <returns></returns>
		public override LootList GenerateLoot(GameNPC mob, GameObject killer)
		{
			LootList loot = base.GenerateLoot(mob, killer);

			int baseChance = 30;
			if (killer.Level < 10)
				baseChance = 100;
			else if (killer.Level < 20)
				baseChance = 50;
			else if (killer.Level < 30)
				baseChance = 40;

			int chance = baseChance + (int)killer.GetConLevel(mob) * 10;
			if (killer.Realm != 0 && Util.Chance(chance))
				loot.AddFixed(GenerateUniqueItem(mob, killer));

			return loot;
		}

		public static InventoryItem GenerateUniqueItem(GameNPC mob, GameObject killer)
		{
			if (killer is GamePlayer)
			{
				GamePlayer player = killer as GamePlayer;
				if (player.PlayerGroup != null)
				{
					GamePlayer usePlayer = player;
					foreach (GamePlayer member in player.PlayerGroup.GetPlayersInTheGroup())
						if (member.Level > usePlayer.Level)
							usePlayer = member;

					killer = usePlayer;
				}
			}


			InventoryItem item = new InventoryItem();
			//item realm
			item.Realm = killer.Realm;
			//item level
			if (mob.Level >= 71)
				item.Level = 51;
			else if (mob.Level >= 65)
				item.Level = 50;
			else
			{
				item.Level = mob.Level;
				if (item.Level > 49)
					item.Level = 49;
				if (item.Level < 1)
					item.Level = 1;
			}

			//object type
			item.Object_Type = (int)GenerateObjectType(item.Realm);

			//item slot
			item.Item_Type = (int)GenerateItemType((eObjectType)item.Object_Type);

			//damage type
			item.Type_Damage = (int)GenerateDamageType((eObjectType)item.Object_Type);

			//item stats
			//instrument dps_af needs to be known before name generation
			GenerateItemStats(item);

			//item name and model
			GenerateItemNameModel(item);

			//item magical bonuses
			//if staff and magic..... focus
			GenerateMagicalBonuses(item);

			item.IsDropable = true;
			item.IsPickable = true;
			item.IsTradable = true;

			//item quality / maxquality
			item.Quality = GenerateItemQuality(killer.GetConLevel(mob));

			//item bonus
			int temp = item.Level - 15;
			temp -= temp % 5;
			item.Bonus = temp;
			if (item.Bonus < 0)
				item.Bonus = 0;

			//constants
			int condition = item.Level * 2000;
			item.Condition = condition;
			item.MaxCondition = condition;
			item.Durability = condition;
			item.MaxDurability = condition;

			item.Weight = GenerateItemWeight((eObjectType)item.Object_Type, (eInventorySlot)item.Item_Type);

			item.CrafterName = "Unique Object";
			/*
			item.Gold = (short)(item.Level / 8);
			if (item.Gold == 0)
				item.Silver = (byte)(item.Level * 10 / 8);
			if (item.Silver == 0)
				item.Copper = (byte)(item.Level * 100 / 8);
			if (item.Copper < 1)
				item.Copper = 1;
			 */
			item.Id_nb = "Unique";

			return item;
		}

		public static InventoryItem GenerateFreeEquipment(eInventorySlot slot, eObjectType type, byte level, GamePlayer player, eDamageType damage)
		{
			InventoryItem item = new InventoryItem();
			//item realm
			item.Realm = player.Realm;
			//item level
			item.Level = level;

			//object type
			item.Object_Type = (int)type;

			//item slot
			item.Item_Type = (int)slot;

			//damage type
			item.Type_Damage = (int)damage;

			//item stats
			//instrument dps_af needs to be known before name generation
			GenerateItemStats(item);

			//item name and model
			GenerateItemNameModel(item);

			if (item.Object_Type == (int)eObjectType.Staff)
			{
				item.Bonus1 = item.Level;
				item.Bonus1Type = (int)eProperty.AllFocusLevels;
			}

			item.IsDropable = false;
			item.IsPickable = true;
			item.IsTradable = false;

			//item quality / maxquality
			item.Quality = 85;

			//item bonus
			int temp = item.Level - 15;
			temp -= temp % 5;
			item.Bonus = temp;
			if (item.Bonus < 0)
				item.Bonus = 0;

			//constants
			int condition = item.Level * 2000;
			item.Condition = condition;
			item.MaxCondition = condition;
			item.Durability = condition;
			item.MaxDurability = condition;

			item.Weight = GenerateItemWeight((eObjectType)item.Object_Type, (eInventorySlot)item.Item_Type);

			item.CrafterName = "Free Equipment";
			item.Copper = 1;
			item.Id_nb = "Free";

			return item;
		}

		private static eObjectType GenerateObjectType(int realm)
		{
			eGenerateType type = eGenerateType.None;
			if (Util.Chance(50)) type = eGenerateType.Armor;
			else if (Util.Chance(20)) type = eGenerateType.Magical;
			else type = eGenerateType.Weapon;

			switch ((eRealm)realm)
			{
				case eRealm.Albion:
					{
						switch (type)
						{
							case eGenerateType.Armor: return AlbionArmor[Util.Random(0, AlbionArmor.Length - 1)];
							case eGenerateType.Weapon: return AlbionWeapons[Util.Random(0, AlbionWeapons.Length - 1)];
							case eGenerateType.Magical: return eObjectType.Magical;
						}
						break;
					}
				case eRealm.Midgard:
					{
						switch (type)
						{
							case eGenerateType.Armor: return MidgardArmor[Util.Random(0, MidgardArmor.Length - 1)];
							case eGenerateType.Weapon: return MidgardWeapons[Util.Random(0, MidgardWeapons.Length - 1)];
							case eGenerateType.Magical: return eObjectType.Magical;
						}
						break;
					}
				case eRealm.Hibernia:
					{
						switch (type)
						{
							case eGenerateType.Armor: return HiberniaArmor[Util.Random(0, HiberniaArmor.Length - 1)];
							case eGenerateType.Weapon: return HiberniaWeapons[Util.Random(0, HiberniaWeapons.Length - 1)];
							case eGenerateType.Magical: return eObjectType.Magical;
						}
						break;
					}
			}
			return eObjectType.GenericItem;
		}

		private static eInventorySlot GenerateItemType(eObjectType type)
		{
			if ((int)type >= (int)eObjectType._FirstArmor && (int)type <= (int)eObjectType._LastArmor)
				return (eInventorySlot)ArmorSlots[Util.Random(0, ArmorSlots.Length - 1)];
			switch (type)
			{
				//left or right standard
				case eObjectType.HandToHand:
				case eObjectType.Piercing:
				case eObjectType.Blades:
				case eObjectType.Blunt:
				case eObjectType.SlashingWeapon:
				case eObjectType.CrushingWeapon:
				case eObjectType.ThrustWeapon:
					return (eInventorySlot)Util.Random(Slot.RIGHTHAND, Slot.LEFTHAND);
				//left or right or twohand
				case eObjectType.Sword:
				case eObjectType.Hammer:
					return (eInventorySlot)Util.Random(Slot.RIGHTHAND, Slot.TWOHAND);
				//right
				case eObjectType.Flexible:
					return (eInventorySlot)Slot.RIGHTHAND;
				//left
				case eObjectType.LeftAxe:
				case eObjectType.Shield:
					return (eInventorySlot)Slot.LEFTHAND;
				//twohanded
				case eObjectType.LargeWeapons:
				case eObjectType.CelticSpear:
				case eObjectType.PolearmWeapon:
				case eObjectType.Spear:
				case eObjectType.Staff:
				case eObjectType.Scythe:
				case eObjectType.TwoHandedWeapon:
					return (eInventorySlot)Slot.TWOHAND;
				//right or twohand
				case eObjectType.Axe:
					{
						if (Util.Chance(50))
							return eInventorySlot.RightHandWeapon;
						else return eInventorySlot.TwoHandWeapon;
					}
				//ranged
				case eObjectType.CompositeBow:
				case eObjectType.Fired:
				case eObjectType.Longbow:
				case eObjectType.RecurvedBow:
				case eObjectType.Crossbow:
					return (eInventorySlot)Slot.RANGED;
				case eObjectType.Magical:
					return (eInventorySlot)MagicalSlots[Util.Random(0, MagicalSlots.Length - 1)];
				case eObjectType.Instrument:
					return (eInventorySlot)Util.Random(Slot.TWOHAND, Slot.RANGED);
			}
			return eInventorySlot.FirstEmptyBackpack;
		}

		private static eDamageType GenerateDamageType(eObjectType type)
		{
			switch (type)
			{
				//all
				case eObjectType.TwoHandedWeapon:
				case eObjectType.PolearmWeapon:
					return (eDamageType)Util.Random(1, 3);
				//slash
				case eObjectType.Axe:
				case eObjectType.Blades:
				case eObjectType.SlashingWeapon:
				case eObjectType.LeftAxe:
				case eObjectType.Sword:
				case eObjectType.Scythe:
					return eDamageType.Slash;
				//thrust
				case eObjectType.ThrustWeapon:
				case eObjectType.Piercing:
				case eObjectType.CelticSpear:
					return eDamageType.Thrust;
				//crush
				case eObjectType.Hammer:
				case eObjectType.CrushingWeapon:
				case eObjectType.Blunt:
					return eDamageType.Crush;
				//specifics
				case eObjectType.Longbow:
				case eObjectType.RecurvedBow:
				case eObjectType.CompositeBow:
				case eObjectType.Fired:
				case eObjectType.Crossbow:
					return eDamageType.Thrust;
				case eObjectType.HandToHand:
					return (eDamageType)Util.Random(2, 3);
				case eObjectType.Spear:
					return (eDamageType)Util.Random(2, 3);
				case eObjectType.Flexible:
					return (eDamageType)Util.Random(1, 2);
				case eObjectType.Staff:
					return eDamageType.Crush;
				case eObjectType.LargeWeapons:
					return (eDamageType)Util.Random(1, 2);
				//do shields return the shield size?
				case eObjectType.Shield:
					return (eDamageType)Util.Random(1, 3);
				//instruments too?
				case eObjectType.Instrument:
					return (eDamageType)Util.Random(1, 3);

			}
			return eDamageType.Natural;
		}

		private static void GenerateItemNameModel(InventoryItem item)
		{
			eInventorySlot slot = (eInventorySlot)item.Item_Type;
			eDamageType damage = (eDamageType)item.Type_Damage;
			eRealm realm = (eRealm)item.Realm;
			eObjectType type = (eObjectType)item.Object_Type;

			//TODO LEFT HAND NAMES
			//TODO TWO HANDED NAMES (midgard)

			string name = "No Name";
			int model = 488;

			switch (type)
			{
				//armor
				case eObjectType.Cloth:
					{
						name = "Cloth " + ArmorSlotToName(slot);
						switch (slot)
						{
							case eInventorySlot.ArmsArmor: model = 141; break;
							case eInventorySlot.LegsArmor: model = 140; break;
							case eInventorySlot.FeetArmor: model = 143; break;
							case eInventorySlot.HeadArmor: model = 822; break;
							case eInventorySlot.TorsoArmor: model = 139; break;
							case eInventorySlot.HandsArmor: model = 142; break;
						}
						break;
					}
				case eObjectType.Leather:
					{
						name = "Leather " + ArmorSlotToName(slot);
						switch (slot)
						{
							case eInventorySlot.ArmsArmor: model = 38; break;
							case eInventorySlot.LegsArmor: model = 37; break;
							case eInventorySlot.FeetArmor: model = 40; break;
							case eInventorySlot.HeadArmor: model = 62; break;
							case eInventorySlot.TorsoArmor: model = 36; break;
							case eInventorySlot.HandsArmor: model = 39; break;
						}
						break;
					}
				case eObjectType.Studded:
					{
						name = "Studded " + ArmorSlotToName(slot);
						switch (slot)
						{
							case eInventorySlot.ArmsArmor: model = 83; break;
							case eInventorySlot.LegsArmor: model = 82; break;
							case eInventorySlot.FeetArmor: model = 84; break;
							case eInventorySlot.HeadArmor: model = 824; break;
							case eInventorySlot.TorsoArmor: model = 81; break;
							case eInventorySlot.HandsArmor: model = 85; break;
						}
						break;
					}
				case eObjectType.Plate:
					{
						name = "Plate " + ArmorSlotToName(slot);
						switch (slot)
						{
							case eInventorySlot.ArmsArmor: model = 48; break;
							case eInventorySlot.LegsArmor: model = 47; break;
							case eInventorySlot.FeetArmor: model = 50; break;
							case eInventorySlot.HeadArmor: model = 64; break;
							case eInventorySlot.TorsoArmor: model = 46; break;
							case eInventorySlot.HandsArmor: model = 49; break;
						}
						break;
					}
				case eObjectType.Chain:
					{
						name = "Chain " + ArmorSlotToName(slot);
						switch (slot)
						{
							case eInventorySlot.ArmsArmor: model = 43; break;
							case eInventorySlot.LegsArmor: model = 42; break;
							case eInventorySlot.FeetArmor: model = 45; break;
							case eInventorySlot.HeadArmor: model = 63; break;
							case eInventorySlot.TorsoArmor: model = 41; break;
							case eInventorySlot.HandsArmor: model = 44; break;
						}
						break;
					}
				case eObjectType.Reinforced:
					{
						name = "Reinforced " + ArmorSlotToName(slot);
						switch (slot)
						{
							case eInventorySlot.ArmsArmor: model = 385; break;
							case eInventorySlot.LegsArmor: model = 384; break;
							case eInventorySlot.FeetArmor: model = 387; break;
							case eInventorySlot.HeadArmor: model = 835; break;
							case eInventorySlot.TorsoArmor: model = 383; break;
							case eInventorySlot.HandsArmor: model = 386; break;
						}
						break;
					}
				case eObjectType.Scale:
					{
						name = "Scale " + ArmorSlotToName(slot);
						switch (slot)
						{
							case eInventorySlot.ArmsArmor: model = 390; break;
							case eInventorySlot.LegsArmor: model = 389; break;
							case eInventorySlot.FeetArmor: model = 392; break;
							case eInventorySlot.HeadArmor: model = 838; break;
							case eInventorySlot.TorsoArmor: model = 388; break;
							case eInventorySlot.HandsArmor: model = 391; break;
						}
						break;
					}

				//weapons
				case eObjectType.Axe:
					{
						if (item.Hand == 1)
						{
							name = "Two Handed Axe";
							model = 577;
						}
						else
						{
							name = "Axe";
							model = 315;
						}
						break;
					}
				case eObjectType.Blades:
					{
						name = "Sword";
						model = 446;
						break;
					}
				case eObjectType.Blunt:
					{
						name = "Hammer";
						model = 461;
						break;
					}
				case eObjectType.CelticSpear:
					{
						name = "Celtic Spear";
						model = 469;
						break;
					}
				case eObjectType.CompositeBow:
					{
						name = "Composite Bow";
						model = 564;
						break;
					}
				case eObjectType.Crossbow:
					{
						name = "Crossbow";
						model = 226;
						break;
					}
				case eObjectType.CrushingWeapon:
					{
						name = "Hammer";
						model = 12;
						break;
					}
				case eObjectType.Fired:
					{
						name = "Shortbow";
						model = 569;
						break;
					}
				case eObjectType.Flexible:
					{
						switch (damage)
						{
							case eDamageType.Crush:
								{
									name = "Chain";
									model = 857;
									break;
								}
							case eDamageType.Slash:
								{
									name = "Whip";
									model = 867;
									break;
								}
						}
						break;

					}
				case eObjectType.Hammer:
					{
						if (item.Hand == 1)
						{
							name = "Two Handed Hammer";
							model = 574;
						}
						else
						{
							name = "Hammer";
							model = 320;
						}
						break;
					}
				case eObjectType.HandToHand:
					{
						switch (damage)
						{
							case eDamageType.Slash:
								{
									name = "Claw";
									model = 961;
									break;
								}
							case eDamageType.Thrust:
								{
									name = "Fang";
									model = 960;
									break;
								}
						}
						break;
					}
				case eObjectType.Instrument:
					{
						switch (item.DPS_AF)
						{
							case 1:
								{
									name = "Drum Instrument";
									model = 228;
									break;
								}
							case 2:
								{
									name = "Lute Instrument";
									model = 227;
									break;
								}
							case 3:
								{
									name = "Flute Instrument";
									model = 325;
									break;
								}
						}
						break;
					}
				case eObjectType.LargeWeapons:
					{
						switch (damage)
						{
							case eDamageType.Slash:
								{
									name = "Great Sword";
									model = 459;
									break;
								}
							case eDamageType.Crush:
								{
									name = "Great Hammer";
									model = 640;
									break;
								}
						}
						break;
					}
				case eObjectType.LeftAxe:
					{
						name = "Axe";
						model = 315;
						break;
					}
				case eObjectType.Longbow:
					{
						name = "Longbow";
						model = 132;
						break;
					}
				case eObjectType.Magical:
					{
						switch (slot)
						{
							case eInventorySlot.Cloak:
								{
									name = "Cloak";
									model = 57;
									break;
								}
							case eInventorySlot.Waist:
								{
									name = "Belt";
									model = 597;
									break;
								}
							case eInventorySlot.Neck:
								{
									name = "Necklace";
									model = 101;
									break;
								}
							case eInventorySlot.Jewellery:
								{
									name = "Gem";
									model = Util.Random(110, 119);
									break;
								}
							case eInventorySlot.LeftBracer:
							case eInventorySlot.RightBracer:
								{
									name = "Bracer";
									model = 598;
									break;
								}
							case eInventorySlot.LeftRing:
							case eInventorySlot.RightRing:
								{
									name = "Ring";
									model = 103;
									break;
								}
						}
						break;
					}
				case eObjectType.Piercing:
					{
						name = "Adze";
						model = 940;
						break;
					}
				case eObjectType.PolearmWeapon:
					{
						switch (damage)
						{
							case eDamageType.Slash:
								{
									name = "Halberd";
									model = 67;
									break;
								}
							case eDamageType.Thrust:
								{
									name = "Pike";
									model = 69;
									break;
								}
							case eDamageType.Crush:
								{
									name = "Mattock";
									model = 70;
									break;
								}
						}
						break;
					}
				case eObjectType.RecurvedBow:
					{
						name = "Recurved Bow";
						model = 925;
						break;
					}
				case eObjectType.Scythe:
					{
						name = "Scythe";
						model = 931;
						break;
					}
				case eObjectType.Shield:
					{
						switch ((int)damage)
						{
							case 1:
								{
									name = "Small Shield";
									model = 59;
									break;
								}
							case 2:
								{
									name = "Medium Shield";
									model = 61;
									break;
								}
							case 3:
								{
									name = "Large Shield";
									model = 60;
									break;
								}
						}
						break;
					}
				case eObjectType.SlashingWeapon:
					{
						name = "Sword";
						model = 4;
						break;
					}
				case eObjectType.Spear:
					{
						name = "Spear";
						model = 328;
						break;
					}
				case eObjectType.Staff:
					{
						name = "Staff";
						model = 442;
						break;
					}
				case eObjectType.Sword:
					{
						if (item.Hand == 1)
						{
							name = "Two Handed Sword";
							model = 314;
						}
						else
						{
							name = "Sword";
							model = 310;
						}
						break;
					}
				case eObjectType.ThrustWeapon:
					{
						name = "Rapier";
						model = 22;
						break;
					}
				case eObjectType.TwoHandedWeapon:
					{
						switch (damage)
						{
							case eDamageType.Slash:
								{
									name = "Two Handed Sword";
									model = 6;
									break;
								}
							case eDamageType.Crush:
								{
									name = "Two Handed Hammer";
									model = 17;
									break;
								}
							case eDamageType.Thrust:
								{
									name = "Two Handed Pick";
									model = 16;
									break;
								}
						}
						break;
					}
			}
			item.Name = name;
			item.Model = model;
		}

		private static void GenerateItemStats(InventoryItem item)
		{
			eObjectType type = (eObjectType)item.Object_Type;

			//special property for instrument
			if (type == eObjectType.Instrument)
				item.DPS_AF = Util.Random(1, 3);

			//set hand
			switch (type)
			{
				//two handed weapons
				case eObjectType.CelticSpear:
				case eObjectType.CompositeBow:
				case eObjectType.Crossbow:
				case eObjectType.Fired:
				case eObjectType.Instrument:
				case eObjectType.LargeWeapons:
				case eObjectType.Longbow:
				case eObjectType.PolearmWeapon:
				case eObjectType.RecurvedBow:
				case eObjectType.Scythe:
				case eObjectType.Spear:
				case eObjectType.Staff:
				case eObjectType.TwoHandedWeapon:
					{
						item.Hand = 1;
						break;
					}
				//right or left handed weapons
				case eObjectType.Blades:
				case eObjectType.Blunt:
				case eObjectType.CrushingWeapon:
				case eObjectType.HandToHand:
				case eObjectType.Piercing:
				case eObjectType.SlashingWeapon:
				case eObjectType.ThrustWeapon:
					{
						if ((eInventorySlot)item.Item_Type == eInventorySlot.LeftHandWeapon)
							item.Hand = 2;
						break;
					}
				//left handed weapons
				case eObjectType.LeftAxe:
				case eObjectType.Shield:
					{
						item.Hand = 2;
						break;
					}
				//right or two handed weapons
				case eObjectType.Sword:
				case eObjectType.Hammer:
				case eObjectType.Axe:
					{
						if ((eInventorySlot)item.Item_Type == eInventorySlot.TwoHandWeapon)
							item.Hand = 1;
						break;
					}
			}

			//set dps_af and spd_abs
			if ((int)type >= (int)eObjectType._FirstArmor && (int)type <= (int)eObjectType._LastArmor)
			{
				if (type == eObjectType.Cloth)
					item.DPS_AF = item.Level;
				else item.DPS_AF = item.Level * 2;
				item.SPD_ABS = GetAbsorb(type);
			}
			switch (type)
			{
				case eObjectType.Axe:
				case eObjectType.Blades:
				case eObjectType.Blunt:
				case eObjectType.CelticSpear:
				case eObjectType.CompositeBow:
				case eObjectType.Crossbow:
				case eObjectType.CrushingWeapon:
				case eObjectType.Fired:
				case eObjectType.Flexible:
				case eObjectType.Hammer:
				case eObjectType.HandToHand:
				case eObjectType.LargeWeapons:
				case eObjectType.LeftAxe:
				case eObjectType.Longbow:
				case eObjectType.Piercing:
				case eObjectType.PolearmWeapon:
				case eObjectType.RecurvedBow:
				case eObjectType.Scythe:
				case eObjectType.Shield:
				case eObjectType.SlashingWeapon:
				case eObjectType.Spear:
				case eObjectType.Staff:
				case eObjectType.Sword:
				case eObjectType.ThrustWeapon:
				case eObjectType.TwoHandedWeapon:
					{
						item.DPS_AF = (int)(((item.Level * 0.3) + 1.2) * 10);
						item.SPD_ABS = GetWeaponSpeed(item);
						break;
					}
			}
		}

		public enum eBonusType
		{
			Stat,
			Resist,
			Skill,
			Focus,
		}

		private static void GenerateMagicalBonuses(InventoryItem item)
		{
			int min = 0;
			if (item.Object_Type == (int)eObjectType.Magical)
				min = 1;
			//int number = Util.Random(min, 4);

			int number = min;

			if (Util.Chance(40))
				number++;
			if (Util.Chance(30))
				number++;
			if (Util.Chance(20))
				number++;

			if ((number != 4) && Util.Chance(10))
			{
				number++;
			}

			for (int i = 0; i < number; i++)
			{
				eBonusType type = GetPropertyType(item);
				eProperty property = GetProperty(type, item);
				int amount = GetBonusAmount(type, property, item.Level);
				WriteBonus(item, property, amount);
			}
		}

		private static int GenerateItemQuality(double conlevel)
		{
			int minQuality = 89;
			int maxQuality = (int)(1.310 * conlevel + 94.29);
			maxQuality = Math.Min(maxQuality, 100);
			maxQuality = Math.Max(maxQuality, minQuality);

			int quality = Util.Random(minQuality, maxQuality);

			return quality;
		}

		private static eBonusType GetPropertyType(InventoryItem item)
		{
			//focus... staves only
			if (CanAddFocus(item))
				return eBonusType.Focus;
			//stats
			if (Util.Chance(50))
				return eBonusType.Stat;
			//resists
			if (Util.Chance(40))
				return eBonusType.Resist;
			//skills
			return eBonusType.Skill;
		}

		private static bool CanAddFocus(InventoryItem item)
		{
			if (item.Object_Type == (int)eObjectType.Staff)
			{
				if (item.Realm == (int)eRealm.Albion && Util.Chance(15))
					return false;

				if (item.Bonus1Type == (int)eProperty.AllFocusLevels)
					return false;

				if (item.Bonus2Type == (int)eProperty.AllFocusLevels)
					return false;

				if (item.Bonus3Type == (int)eProperty.AllFocusLevels)
					return false;

				if (item.Bonus4Type == (int)eProperty.AllFocusLevels)
					return false;

				return true;
			}
			return false;
		}

		private static eProperty GetProperty(eBonusType type, InventoryItem item)
		{
			switch (type)
			{
				case eBonusType.Focus:
					{
						return eProperty.AllFocusLevels;
					}
				case eBonusType.Resist:
					{
						return (eProperty)Util.Random((int)eProperty.Resist_First, (int)eProperty.Resist_Last);
					}
				case eBonusType.Skill:
					{
						//fill valid skills
						ArrayList validSkills = new ArrayList();
						foreach (eProperty property in SkillBonus)
						{
							if (GlobalConstants.GetBonusRealm(property) == (eRealm)item.Realm && !BonusExists(item, property) && SkillIsValidForObjectType(item.Level, (eRealm)item.Realm, (eObjectType)item.Object_Type, property))
								validSkills.Add(property);
						}

						if (!BonusExists(item, eProperty.AllMagicSkills) && SkillIsValidForObjectType(item.Level, (eRealm)item.Realm, (eObjectType)item.Object_Type, eProperty.AllMagicSkills))
							validSkills.Add(eProperty.AllMagicSkills);

						if (!BonusExists(item, eProperty.AllMeleeWeaponSkills) && SkillIsValidForObjectType(item.Level, (eRealm)item.Realm, (eObjectType)item.Object_Type, eProperty.AllMeleeWeaponSkills))
							validSkills.Add(eProperty.AllMeleeWeaponSkills);

						if (!BonusExists(item, eProperty.AllSkills) && SkillIsValidForObjectType(item.Level, (eRealm)item.Realm, (eObjectType)item.Object_Type, eProperty.AllSkills))
							validSkills.Add(eProperty.AllSkills);

						if (!BonusExists(item, eProperty.AllDualWieldingSkills) && SkillIsValidForObjectType(item.Level, (eRealm)item.Realm, (eObjectType)item.Object_Type, eProperty.AllDualWieldingSkills))
							validSkills.Add(eProperty.AllDualWieldingSkills);

						if (!BonusExists(item, eProperty.AllArcherySkills) && SkillIsValidForObjectType(item.Level, (eRealm)item.Realm, (eObjectType)item.Object_Type, eProperty.AllArcherySkills))
							validSkills.Add(eProperty.AllArcherySkills);

						int index = 0;
						index = validSkills.Count - 1;
						if (validSkills.Count < 1)
							return eProperty.MaxHealth;
						return (eProperty)validSkills[Util.Random(0, index)];
					}
				case eBonusType.Stat:
					{
						//fill valid stats
						ArrayList validStats = new ArrayList();
						foreach (eProperty property in StatBonus)
						{
							if (!BonusExists(item, property) && StatIsValidForObjectType((eRealm)item.Realm, (eObjectType)item.Object_Type, property) && StatIsValidForRealm((eRealm)item.Realm, property))
								validStats.Add(property);
						}
						return (eProperty)validStats[Util.Random(0, validStats.Count - 1)];
					}
			}
			return eProperty.MaxHealth;
		}

		private static bool StatIsValidForObjectType(eRealm realm, eObjectType type, eProperty property)
		{
			switch (type)
			{
				case eObjectType.Magical: return StatIsValidForRealm(realm, property);
				case eObjectType.Cloth:
				case eObjectType.Leather:
				case eObjectType.Studded:
				case eObjectType.Reinforced:
				case eObjectType.Chain:
				case eObjectType.Scale:
				case eObjectType.Plate: return StatIsValidForArmor(realm, type, property);
				case eObjectType.Axe:
				case eObjectType.Blades:
				case eObjectType.Blunt:
				case eObjectType.CelticSpear:
				case eObjectType.CompositeBow:
				case eObjectType.Crossbow:
				case eObjectType.CrushingWeapon:
				case eObjectType.Fired:
				case eObjectType.Flexible:
				case eObjectType.Hammer:
				case eObjectType.HandToHand:
				case eObjectType.Instrument:
				case eObjectType.LargeWeapons:
				case eObjectType.LeftAxe:
				case eObjectType.Longbow:
				case eObjectType.Piercing:
				case eObjectType.PolearmWeapon:
				case eObjectType.RecurvedBow:
				case eObjectType.Scythe:
				case eObjectType.Shield:
				case eObjectType.SlashingWeapon:
				case eObjectType.Spear:
				case eObjectType.Staff:
				case eObjectType.Sword:
				case eObjectType.ThrustWeapon:
				case eObjectType.TwoHandedWeapon: return StatIsValidForWeapon(realm, type, property);
			}
			return true;
		}

		private static bool SkillIsValidForObjectType(int level, eRealm realm, eObjectType type, eProperty property)
		{
			switch (type)
			{
				case eObjectType.Magical: return true;
				case eObjectType.Cloth:
				case eObjectType.Leather:
				case eObjectType.Studded:
				case eObjectType.Reinforced:
				case eObjectType.Chain:
				case eObjectType.Scale:
				case eObjectType.Plate: return SkillIsValidForArmor(level, realm, type, property);
				case eObjectType.Axe:
				case eObjectType.Blades:
				case eObjectType.Blunt:
				case eObjectType.CelticSpear:
				case eObjectType.CompositeBow:
				case eObjectType.Crossbow:
				case eObjectType.CrushingWeapon:
				case eObjectType.Fired:
				case eObjectType.Flexible:
				case eObjectType.Hammer:
				case eObjectType.HandToHand:
				case eObjectType.Instrument:
				case eObjectType.LargeWeapons:
				case eObjectType.LeftAxe:
				case eObjectType.Longbow:
				case eObjectType.Piercing:
				case eObjectType.PolearmWeapon:
				case eObjectType.RecurvedBow:
				case eObjectType.Scythe:
				case eObjectType.Shield:
				case eObjectType.SlashingWeapon:
				case eObjectType.Spear:
				case eObjectType.Staff:
				case eObjectType.Sword:
				case eObjectType.ThrustWeapon:
				case eObjectType.TwoHandedWeapon: return SkillIsValidForWeapon(level, realm, type, property);
			}
			return true;
		}

		private static bool SkillIsValidForArmor(int level, eRealm realm, eObjectType type, eProperty property)
		{
			switch (property)
			{
				case eProperty.Skill_Augmentation:
					{
						if (level < 10)
						{
							if (type == eObjectType.Leather)
								return true;
							return false;
						}
						else if (level < 20)
						{
							if (type == eObjectType.Studded)
								return true;
							return false;
						}
						else
						{
							if (type == eObjectType.Chain)
								return true;
							return false;
						}
					}
				case eProperty.Skill_Axe:
					{
						if (type == eObjectType.Leather || type == eObjectType.Studded || type == eObjectType.Chain)
							return true;
						return false;
					}
				case eProperty.Skill_Battlesongs:
					{
						if (level < 20)
						{
							if (type == eObjectType.Studded)
								return true;
							return false;
						}
						else
						{
							if (type == eObjectType.Chain)
								return true;
							return false;
						}
					}
				case eProperty.Skill_Pathfinding:
				case eProperty.Skill_BeastCraft:
					{
						if (level < 10)
						{
							if (type == eObjectType.Leather)
								return true;
							return false;
						}
						else
						{
							if (type == eObjectType.Studded)
								return true;
							return false;
						}
					}
				case eProperty.Skill_Blades:
					{
						if (type == eObjectType.Leather || type == eObjectType.Reinforced || type == eObjectType.Scale)
							return true;
						return false;
					}
				case eProperty.Skill_Blunt:
					{
						if (type == eObjectType.Reinforced || type == eObjectType.Scale)
							return true;
						return false;
					}
				//Cloth skills
				case eProperty.Skill_Arboreal:
				case eProperty.Skill_Body:
				case eProperty.Skill_BoneArmy:
				case eProperty.Skill_Cold:
				case eProperty.Skill_Creeping:
				case eProperty.Skill_Cursing:
				case eProperty.Skill_Darkness:
				case eProperty.Skill_Death_Servant:
				case eProperty.Skill_DeathSight:
				case eProperty.Skill_Earth:
				case eProperty.Skill_Enchantments:
				case eProperty.Skill_EtherealShriek:
				case eProperty.Skill_Fire:
				case eProperty.Skill_Hexing:
				case eProperty.Skill_Light:
				case eProperty.Skill_Mana:
				case eProperty.Skill_Matter:
				case eProperty.Skill_Mentalism:
				case eProperty.Skill_Mind:
				case eProperty.Skill_Pain_working:
				case eProperty.Skill_PhantasmalWail:
				case eProperty.Skill_Runecarving:
				case eProperty.Skill_SpectralForce:
				case eProperty.Skill_Spirit:
				case eProperty.Skill_Summoning:
				case eProperty.Skill_Suppression:
				case eProperty.Skill_Verdant:
				case eProperty.Skill_Void:
				case eProperty.Skill_Wind:
				case eProperty.Skill_Witchcraft:
					{
						if (type == eObjectType.Cloth)
							return true;
						return false;
					}
				case eProperty.Skill_Celtic_Dual:
					{
						if (type == eObjectType.Leather ||
							type == eObjectType.Reinforced)
							return true;
						return false;
					}
				case eProperty.Skill_Celtic_Spear:
					{
						if (level < 15)
						{
							if (type == eObjectType.Reinforced)
								return true;
							return false;
						}
						else
						{
							if (type == eObjectType.Scale)
								return true;
							return false;
						}
					}
				case eProperty.Skill_Chants:
					{
						if (level < 10)
						{
							if (type == eObjectType.Studded)
								return true;
							return false;
						}
						else if (level < 20)
						{
							if (type == eObjectType.Chain)
								return true;
							return false;
						}
						else
						{
							if (type == eObjectType.Plate)
								return true;
							return false;
						}
					}
				case eProperty.Skill_Composite:
				case eProperty.Skill_RecurvedBow:
				case eProperty.Skill_Long_bows:
					{
						if (level < 10)
						{
							if (type == eObjectType.Leather)
								return true;
							return false;
						}
						else
						{
							if (type == eObjectType.Studded)
								return true;
							return false;
						}
					}
				case eProperty.Skill_Critical_Strike:
				case eProperty.Skill_Envenom:
				case eProperty.Skill_Dementia:
				case eProperty.Skill_Nightshade:
				case eProperty.Skill_ShadowMastery:
				case eProperty.Skill_VampiiricEmbrace:
					{
						if (type == eObjectType.Leather)
							return true;
						return false;
					}
				case eProperty.Skill_Cross_Bows:
					{
						if (type == eObjectType.Leather)
							return true;
						if (level < 15)
						{
							if (type == eObjectType.Chain)
								return true;
							return false;
						}
						else
						{
							if (type == eObjectType.Plate)
								return true;
							return false;
						}
					}
				case eProperty.Skill_Crushing:
					{
						if (level < 15)
						{
							if (type == eObjectType.Studded)
								return true;
							return false;
						}
						else
						{
							if (type == eObjectType.Studded || type == eObjectType.Chain || type == eObjectType.Plate)
								return true;
							return false;
						}
					}
				case eProperty.Skill_Dual_Wield:
					{
						if (level < 20)
						{
							if (type == eObjectType.Leather || type == eObjectType.Studded)
								return true;
							return false;
						}
						else
						{
							if (type == eObjectType.Leather || type == eObjectType.Chain)
								return true;
							return false;
						}
					}
				case eProperty.Skill_Enhancement:
					{
						if (level < 10)
						{
							if (type == eObjectType.Leather)
								return true;
							return false;
						}
						else if (level < 20)
						{
							if (type == eObjectType.Studded)
								return true;
							return false;
						}
						else
						{
							if (type == eObjectType.Chain)
								return true;
							return false;
						}
					}
				case eProperty.Skill_Flexible_Weapon:
				case eProperty.Skill_Hammer:
					{
						if (level < 10)
						{
							if (type == eObjectType.Studded)
								return true;
							return false;
						}
						else
						{
							if (type == eObjectType.Chain)
								return true;
							return false;
						}
					}
				case eProperty.Skill_HandToHand:
					{
						if (type == eObjectType.Studded)
							return true;
						return false;
					}
				case eProperty.Skill_Instruments:
					{
						if (level < 10)
						{
							if (type == eObjectType.Leather)
								return true;
							return false;
						}
						else if (level < 20)
						{
							if (type == eObjectType.Studded)
								return true;
							return false;
						}
						else
						{
							if (type == eObjectType.Chain)
								return true;
							return false;
						}
					}
				case eProperty.Skill_Large_Weapon:
					{
						if (level < 10)
							return false;
						if (level < 15)
						{
							if (type == eObjectType.Reinforced)
								return true;
							return false;
						}
						else
						{
							if (type == eObjectType.Scale)
								return true;
							return false;
						}
					}
				case eProperty.Skill_Left_Axe:
					{
						if (level < 5)
							return false;
						if (type == eObjectType.Leather || type == eObjectType.Studded)
							return true;
						break;
					}
				case eProperty.Skill_Music:
					{
						if (level < 5)
							return false;
						if (level < 15)
						{
							if (type == eObjectType.Leather)
								return true;
							return false;
						}
						else
						{
							if (type == eObjectType.Reinforced)
								return true;
							return false;
						}
					}
				case eProperty.Skill_Nature:
					{
						if (level < 5)
							return false;
						else if (level < 10)
						{
							if (type == eObjectType.Leather)
								return true;
							return false;
						}
						else if (level < 20)
						{
							if (type == eObjectType.Reinforced)
								return true;
							return false;
						}
						else
						{
							if (type == eObjectType.Scale)
								return true;
							return false;
						}
					}
				case eProperty.Skill_Nurture:
				case eProperty.Skill_Regrowth:
					{
						if (level < 5)
							return false;
						else if (level < 10)
						{
							if (type == eObjectType.Leather)
								return true;
							return false;
						}
						else
						{
							if (type == eObjectType.Reinforced || type == eObjectType.Scale)
								return true;
							return false;
						}
					}
				case eProperty.Skill_OdinsWill:
					{
						if (level < 5)
							return false;
						else if (level < 12)
						{
							if (type == eObjectType.Studded)
								return true;
							return false;
						}
						else
						{
							if (type == eObjectType.Chain)
								return true;
							return false;
						}
					}
				case eProperty.Skill_Pacification:
					{
						if (level < 5)
							return false;
						else if (level < 10)
						{
							if (type == eObjectType.Leather)
								return true;
							return false;
						}
						else if (level < 20)
						{
							if (type == eObjectType.Studded)
								return true;
							return false;
						}
						else
						{
							if (type == eObjectType.Chain)
								return true;
							return false;
						}
					}
				case eProperty.Skill_Parry:
					{
						if (type == eObjectType.Cloth && realm == eRealm.Hibernia && level > 5)
							return true;
						else if (type == eObjectType.Leather && realm == eRealm.Albion && level > 10)
							return true;
						if (level < 10)
							return false;
						return false;
					}
				case eProperty.Skill_Piercing:
					{
						if (type == eObjectType.Leather || type == eObjectType.Reinforced)
							return true;
						return false;
					}
				case eProperty.Skill_Polearms:
					{
						if (level < 5)
							return false;
						else if (level < 15)
						{
							if (type == eObjectType.Chain)
								return true;
							return false;
						}
						else
						{
							if (type == eObjectType.Plate)
								return true;
							return false;
						}
					}
				case eProperty.Skill_Rejuvenation:
					{
						if (level < 5)
							return false;
						else if (type == eObjectType.Cloth)
							return true;
						else if (type == eObjectType.Leather)
							return true;
						else if (type == eObjectType.Studded && level < 20)
							return true;
						else if (type == eObjectType.Chain && level >= 20)
							return true;
						break;
					}
				case eProperty.Skill_Savagery:
					{
						if (level < 5)
							return false;
						else if (type == eObjectType.Studded)
							return true;
						break;
					}
				case eProperty.Skill_Scythe:
					{
						if (level < 5)
							return false;
						else if (type == eObjectType.Cloth)
							return true;
						break;
					}
				case eProperty.Skill_Shields:
					{
						if (type == eObjectType.Cloth && realm == eRealm.Albion)
							return true;
						else if (type == eObjectType.Studded || type == eObjectType.Chain || type == eObjectType.Reinforced || type == eObjectType.Scale || type == eObjectType.Plate)
							return true;
						break;
					}
				case eProperty.Skill_ShortBow:
					{
						if (realm == eRealm.Hibernia && level < 7)
							return false;
						else if (realm == eRealm.Albion && level < 10)
							return false;
						else if (type == eObjectType.Reinforced)
							return true;
						else if (type == eObjectType.Scale && level >= 15)
							return true;
						else if (type == eObjectType.Chain)
							return true;
						break;
					}
				case eProperty.Skill_Smiting:
					{
						if (level < 5)
							return false;
						else if (type == eObjectType.Leather && level < 10)
							return true;
						else if (type == eObjectType.Studded && level < 20)
							return true;
						else if (type == eObjectType.Chain && level >= 20)
							return true;
						break;
					}
				case eProperty.Skill_SoulRending:
					{
						if (level < 5)
							return false;
						else if (type == eObjectType.Studded && level < 10)
							return true;
						else if (type == eObjectType.Chain && level > 20)
							return true;
						break;
					}
				case eProperty.Skill_Spear:
					{
						if (level < 5)
							return false;
						else if (type == eObjectType.Studded && level < 12)
							return true;
						else if (type == eObjectType.Chain && level >= 12)
							return true;
						break;
					}
				case eProperty.Skill_Staff:
					{
						if (level < 5)
							return false;
						else if (type == eObjectType.Leather && realm == eRealm.Albion)
							return true;
						break;
					}
				case eProperty.Skill_Stealth:
					{
						if (type == eObjectType.Leather || type == eObjectType.Studded || type == eObjectType.Reinforced)
							return true;
						break;
					}
				case eProperty.Skill_Stormcalling:
					{
						if (level < 5)
							return false;
						else if (type == eObjectType.Studded && level < 12)
							return true;
						else if (type == eObjectType.Chain && level >= 12)
							return true;
						break;
					}
				case eProperty.Skill_Subterranean:
					{
						if (level < 5)
							return false;
						else if (type == eObjectType.Leather && level < 10)
							return true;
						else if (type == eObjectType.Studded && level < 20)
							return true;
						else if (type == eObjectType.Chain && level >= 20)
							return true;
						break;
					}
				case eProperty.Skill_Sword:
					{
						if (type == eObjectType.Studded || type == eObjectType.Chain)
							return true;
						break;
					}
				case eProperty.Skill_Slashing:
					{
						if (type == eObjectType.Leather || type == eObjectType.Studded || type == eObjectType.Chain || type == eObjectType.Plate)
							return true;
						break;
					}
				case eProperty.Skill_Thrusting:
					{
						if (type == eObjectType.Leather && level < 10)
							return true;
						else if (type == eObjectType.Studded || type == eObjectType.Chain || type == eObjectType.Plate)
							return true;
						break;
					}
				case eProperty.Skill_Two_Handed:
					{
						if (level < 5)
							return false;
						else if (type == eObjectType.Studded && level < 15)
							return true;
						else if (type == eObjectType.Chain && level < 20)
							return true;
						else if (type == eObjectType.Plate)
							return true;
						break;
					}
				case eProperty.Skill_Valor:
					{
						if (level < 5)
							return false;
						else if (type == eObjectType.Reinforced && level < 20)
							return true;
						else if (type == eObjectType.Scale)
							return true;
						break;
					}
				case eProperty.AllArcherySkills:
					{
						//Archers are always above level 4 and can not wear chain or scale.
						if (level < 5)
							return false;
						else if (type == eObjectType.Leather && level < 10)
							return true;
						else if (type == eObjectType.Reinforced || type == eObjectType.Studded)
							return true;
						break;
					}
				case eProperty.AllDualWieldingSkills:
					{
						//Dualwielders are always above level 4 and can wear better than cloth from the start.
						if (level < 5)
							return false;
						else if (type == eObjectType.Cloth)
							return false;
						//mercs are the only dualwielder who can wear chain
						else if (realm == eRealm.Albion && type == eObjectType.Chain)
							return true;
						//all assassins wear leather, blademasters and zerks wear studded.
						else if (type == eObjectType.Leather || type == eObjectType.Reinforced || type == eObjectType.Studded)
							return true;
						break;
					}
				case eProperty.AllMagicSkills:
					{
						//There isn't a single armortype that doesn't use magical skills at all. Thus, we always allow it.
						return true;
					}
				case eProperty.AllMeleeWeaponSkills:
					{
						//Valewalkers wear cloth.
						if (realm == eRealm.Hibernia && type == eObjectType.Cloth)
							return true;
						else
							return true;
					}
				case eProperty.AllSkills:
					{
						//everyone can use this
						return true;
					}
			}

			return false;
		}

		private static bool SkillIsValidForWeapon(int level, eRealm realm, eObjectType type, eProperty property)
		{
			switch (property)
			{
				case eProperty.Skill_Arboreal:
				case eProperty.Skill_Body:
				case eProperty.Skill_BoneArmy:
				case eProperty.Skill_Cold:
				case eProperty.Skill_Creeping:
				case eProperty.Skill_Cursing:
				case eProperty.Skill_Darkness:
				case eProperty.Skill_Death_Servant:
				case eProperty.Skill_DeathSight:
				case eProperty.Skill_Earth:
				case eProperty.Skill_Enchantments:
				case eProperty.Skill_EtherealShriek:
				case eProperty.Skill_Fire:
				case eProperty.Skill_Hexing:
				case eProperty.Skill_Light:
				case eProperty.Skill_Mana:
				case eProperty.Skill_Matter:
				case eProperty.Skill_Mentalism:
				case eProperty.Skill_Mind:
				case eProperty.Skill_Pain_working:
				case eProperty.Skill_PhantasmalWail:
				case eProperty.Skill_Runecarving:
				case eProperty.Skill_SpectralForce:
				case eProperty.Skill_Spirit:
				case eProperty.Skill_Summoning:
				case eProperty.Skill_Suppression:
				case eProperty.Skill_Verdant:
				case eProperty.Skill_Void:
				case eProperty.Skill_Wind:
				case eProperty.Skill_Witchcraft:
					{
						if (type == eObjectType.Staff)
							return true;
						break;
					}
				//healer things
				case eProperty.Skill_Augmentation:
				case eProperty.Skill_Mending:
				case eProperty.Skill_Subterranean:
				case eProperty.Skill_Enhancement:
				case eProperty.Skill_Smiting:
				case eProperty.Skill_Rejuvenation:
				case eProperty.Skill_Nurture:
				case eProperty.Skill_Nature:
				case eProperty.Skill_Regrowth:
					{
						if (level < 5)
							return false;
						else if (type == eObjectType.Hammer || type == eObjectType.Staff || type == eObjectType.Shield || type == eObjectType.CrushingWeapon || type == eObjectType.Blunt)
							return true;
						break;
					}
				//archery things
				case eProperty.Skill_Composite:
					{
						if (level < 5)
							return false;
						else if (type == eObjectType.CompositeBow)
							return true;
						break;
					}
				case eProperty.Skill_RecurvedBow:
					{
						if (level < 5)
							return false;
						else if (type == eObjectType.RecurvedBow)
							return true;
						break;
					}
				case eProperty.Skill_Long_bows:
					{
						if (level < 5)
							return false;
						else if (type == eObjectType.Longbow)
							return true;
						break;
					}
				//other specifics
				case eProperty.Skill_Staff:
					{
						if (level < 5)
							return false;
						else if (type == eObjectType.Staff)
							return true;
						break;
					}
				case eProperty.Skill_Axe:
					{
						if (type == eObjectType.Axe || type == eObjectType.LeftAxe || type == eObjectType.Shield)
							return true;
						break;
					}
				case eProperty.Skill_Battlesongs:
					{
						if (type == eObjectType.Sword || type == eObjectType.Axe || type == eObjectType.Hammer || type == eObjectType.Shield)
							return true;
						break;
					}
				case eProperty.Skill_BeastCraft:
					{
						if (type == eObjectType.Spear)
							return true;
						break;
					}
				case eProperty.Skill_Blades:
					{
						if (type == eObjectType.Blades || type == eObjectType.Shield)
							return true;
						break;
					}
				case eProperty.Skill_Blunt:
					{
						if (type == eObjectType.Blunt || type == eObjectType.Shield)
							return true;
						break;
					}
				case eProperty.Skill_Celtic_Dual:
					{
						if (type == eObjectType.Piercing || type == eObjectType.Blades || type == eObjectType.CrushingWeapon)
							return true;
						break;
					}
				case eProperty.Skill_Celtic_Spear:
					{
						if (type == eObjectType.CelticSpear)
							return true;
						break;
					}
				case eProperty.Skill_Chants:
					{
						if (type == eObjectType.SlashingWeapon || type == eObjectType.CrushingWeapon || type == eObjectType.ThrustWeapon || type == eObjectType.TwoHandedWeapon || type == eObjectType.Shield)
							return true;
						break;
					}
				case eProperty.Skill_Critical_Strike:
					{
						if (type == eObjectType.Piercing || type == eObjectType.SlashingWeapon || type == eObjectType.ThrustWeapon || type == eObjectType.Blades || type == eObjectType.Axe || type == eObjectType.LeftAxe)
							return true;
						break;
					}
				case eProperty.Skill_Cross_Bows:
					{
						if (type == eObjectType.Crossbow)
							return true;
						break;
					}
				case eProperty.Skill_Crushing:
					{
						if (type == eObjectType.CrushingWeapon || type == eObjectType.TwoHandedWeapon || type == eObjectType.Shield)
							return true;
						break;
					}
				case eProperty.Skill_Dual_Wield:
					{
						if (type == eObjectType.SlashingWeapon || type == eObjectType.ThrustWeapon || type == eObjectType.CrushingWeapon)
							return true;
						break;
					}
				case eProperty.Skill_Envenom:
					{
						if (type == eObjectType.SlashingWeapon || type == eObjectType.ThrustWeapon)
							return true;
						break;
					}
				case eProperty.Skill_Flexible_Weapon:
					{
						if (type == eObjectType.Flexible || type == eObjectType.Shield)
							return true;
						break;
					}
				case eProperty.Skill_Hammer:
					{
						if (type == eObjectType.Hammer || type == eObjectType.Shield)
							return true;
						break;
					}
				case eProperty.Skill_HandToHand:
					{
						if (type == eObjectType.HandToHand)
							return true;
						break;
					}
				case eProperty.Skill_Instruments:
					{
						if (type == eObjectType.Instrument)
							return true;
						break;
					}
				case eProperty.Skill_Large_Weapon:
					{
						if (type == eObjectType.LargeWeapons)
							return true;
						break;
					}
				case eProperty.Skill_Left_Axe:
					{
						if (type == eObjectType.Axe || type == eObjectType.LeftAxe)
							return true;
						break;
					}
				case eProperty.Skill_Music:
					{
						if (type == eObjectType.Blades || type == eObjectType.Blunt || type == eObjectType.Shield || type == eObjectType.Instrument)
							return true;
						break;
					}
				case eProperty.Skill_Nightshade:
					{
						if (type == eObjectType.Blades || type == eObjectType.Piercing || type == eObjectType.Shield)
							return true;
						break;
					}
				case eProperty.Skill_OdinsWill:
					{
						if (type == eObjectType.Sword || type == eObjectType.Spear || type == eObjectType.Shield)
							return true;
						break;
					}
				case eProperty.Skill_Pathfinding:
					{
						if (type == eObjectType.RecurvedBow || type == eObjectType.Piercing || type == eObjectType.Blades)
							return true;
						break;
					}
				case eProperty.Skill_Piercing:
					{
						if (type == eObjectType.Piercing || type == eObjectType.Shield)
							return true;
						break;
					}
				case eProperty.Skill_Polearms:
					{
						if (type == eObjectType.PolearmWeapon)
							return true;
						break;
					}
				case eProperty.Skill_Savagery:
					{
						if (type == eObjectType.Sword || type == eObjectType.Axe || type == eObjectType.Hammer || type == eObjectType.HandToHand || type == eObjectType.Shield)
							return true;
						break;
					}
				case eProperty.Skill_Scythe:
					{
						if (type == eObjectType.Scythe)
							return true;
						break;
					}

				case eProperty.Skill_VampiiricEmbrace:
				case eProperty.Skill_ShadowMastery:
					{
						if (type == eObjectType.Piercing)
							return true;
						break;
					}
				case eProperty.Skill_Shields:
					{
						if (type == eObjectType.SlashingWeapon || type == eObjectType.CrushingWeapon || type == eObjectType.ThrustWeapon || type == eObjectType.Blunt || type == eObjectType.Blades || type == eObjectType.Piercing || type == eObjectType.Shield || type == eObjectType.Axe || type == eObjectType.Sword || type == eObjectType.Hammer)
							return true;
						break;
					}
				case eProperty.Skill_ShortBow:
					{
						if (type == eObjectType.Fired)
							return true;
						break;
					}
				case eProperty.Skill_Slashing:
					{
						if (type == eObjectType.SlashingWeapon || type == eObjectType.TwoHandedWeapon || type == eObjectType.Shield)
							return true;
						break;
					}
				case eProperty.Skill_SoulRending:
					{
						if (type == eObjectType.SlashingWeapon || type == eObjectType.CrushingWeapon || type == eObjectType.ThrustWeapon || type == eObjectType.Flexible || type == eObjectType.Shield)
							return true;
						break;
					}
				case eProperty.Skill_Spear:
					{
						if (type == eObjectType.Spear)
							return true;
						break;
					}
				case eProperty.Skill_Stealth:
					{
						if (type == eObjectType.Longbow || type == eObjectType.RecurvedBow || type == eObjectType.CompositeBow || type == eObjectType.Shield || type == eObjectType.Spear || type == eObjectType.Sword || type == eObjectType.Axe || type == eObjectType.LeftAxe || type == eObjectType.SlashingWeapon || type == eObjectType.ThrustWeapon || type == eObjectType.Piercing || type == eObjectType.Blades || type == eObjectType.Instrument)
							return true;
						break;
					}
				case eProperty.Skill_Stormcalling:
					{
						if (type == eObjectType.Sword || type == eObjectType.Axe || type == eObjectType.Hammer || type == eObjectType.Shield)
							return true;
						break;
					}
				case eProperty.Skill_Sword:
					{
						if (type == eObjectType.Sword || type == eObjectType.Shield)
							return true;
						break;
					}
				case eProperty.Skill_Thrusting:
					{
						if (type == eObjectType.ThrustWeapon || type == eObjectType.TwoHandedWeapon || type == eObjectType.Shield)
							return true;
						break;
					}
				case eProperty.Skill_Two_Handed:
					{
						if (type == eObjectType.TwoHandedWeapon)
							return true;
						break;
					}
				case eProperty.Skill_Valor:
					{
						if (type == eObjectType.Blades || type == eObjectType.Piercing || type == eObjectType.Blunt || type == eObjectType.LargeWeapons || type == eObjectType.Shield)
							return true;
						break;
					}
				case eProperty.Skill_Thrown_Weapons:
					{
						return false;
					}
				case eProperty.Skill_Pacification:
					{
						if (type == eObjectType.Hammer)
							return true;
						break;
					}
				case eProperty.Skill_Dementia:
					{
						if (type == eObjectType.Piercing)
							return true;
						break;
					}
				case eProperty.AllArcherySkills:
					{
						if (type == eObjectType.CompositeBow || type == eObjectType.Longbow || type == eObjectType.RecurvedBow)
							return true;
						break;
					}
				case eProperty.AllDualWieldingSkills:
					{
						if (type == eObjectType.Axe || type == eObjectType.Sword || type == eObjectType.Hammer || type == eObjectType.LeftAxe || type == eObjectType.SlashingWeapon || type == eObjectType.CrushingWeapon || type == eObjectType.ThrustWeapon || type == eObjectType.Piercing || type == eObjectType.Blades || type == eObjectType.Blunt)
							return true;
						break;
					}
				case eProperty.AllMagicSkills:
					{
						//scouts, armsmen, mercs, blademasters, heroes, zerks, warriors do not need this.
						if (type == eObjectType.Longbow || type == eObjectType.CelticSpear || type == eObjectType.PolearmWeapon)
							return false;
						else
							return true;
					}
				case eProperty.AllMeleeWeaponSkills:
					{

						if (type == eObjectType.Staff && realm != eRealm.Albion)
							return false;
						else if (type == eObjectType.Staff && Util.Chance(80)) //80% chance to be a caster staff
							return false;
						else if (type == eObjectType.Longbow || type == eObjectType.CompositeBow || type == eObjectType.RecurvedBow || type == eObjectType.Crossbow || type == eObjectType.Fired)
							return false;
						else
							return true;
					}
			}
			return false;
		}

		private static bool StatIsValidForRealm(eRealm realm, eProperty property)
		{
			switch (property)
			{
				case eProperty.Piety:
					{
						if (realm == eRealm.Hibernia)
							return false;
						break;
					}
				case eProperty.Empathy:
					{
						if (realm == eRealm.Midgard || realm == eRealm.Albion)
							return false;
						break;
					}
				case eProperty.Intelligence:
					{
						if (realm == eRealm.Midgard)
							return false;
						break;
					}
			}
			return true;
		}

		private static bool StatIsValidForArmor(eRealm realm, eObjectType type, eProperty property)
		{
			switch (property)
			{
				case eProperty.Intelligence:
					{
						if (type != eObjectType.Cloth)
							return false;
						break;
					}
				case eProperty.Piety:
					{
						if (realm == eRealm.Albion)
						{
							if (type == eObjectType.Studded || type == eObjectType.Leather)
								return false;
						}
						else if (realm == eRealm.Midgard)
						{
							if (type != eObjectType.Cloth && type != eObjectType.Chain)
								return false;
						}
						break;
					}
				case eProperty.Charisma:
					{
						if (realm == eRealm.Albion || realm == eRealm.Midgard)
						{
							if (type != eObjectType.Chain)
								return false;
						}
						else if (realm == eRealm.Hibernia)
						{
							if (type != eObjectType.Cloth && type != eObjectType.Reinforced)
								return false;
						}
						break;
					}
				case eProperty.Empathy:
					{
						if (type != eObjectType.Reinforced && type != eObjectType.Scale)
							return false;
						break;
					}
			}
			return true;
		}

		private static bool StatIsValidForWeapon(eRealm realm, eObjectType type, eProperty property)
		{
			switch (type)
			{
				case eObjectType.Staff:
					{
						if (property == eProperty.Piety && realm != eRealm.Albion && realm != eRealm.Midgard)
							return false;
						break;
					}

				case eObjectType.Shield:
					{
						if ((realm == eRealm.Albion || realm == eRealm.Midgard) && (property == eProperty.Intelligence || property == eProperty.Empathy))
							return false;
						else if (realm == eRealm.Hibernia && property == eProperty.Piety)
							return false;
						break;
					}

				case eObjectType.Blades:
				case eObjectType.Blunt:
					{
						if (property == eProperty.Piety)
							return false;
						break;
					}
				case eObjectType.LargeWeapons:
				case eObjectType.Piercing:
					{
						if (property == eProperty.Piety || property == eProperty.Empathy)
							return false;
						break;
					}
				case eObjectType.CrushingWeapon:
				case eObjectType.SlashingWeapon:
				case eObjectType.ThrustWeapon:
				case eObjectType.Hammer:
				case eObjectType.Sword:
				case eObjectType.TwoHandedWeapon:
				case eObjectType.Axe:
				case eObjectType.Flexible:
					{
						if (property == eProperty.Intelligence || property == eProperty.Empathy)
							return false;
						break;
					}
				case eObjectType.CelticSpear:
				case eObjectType.HandToHand:
				case eObjectType.LeftAxe:
				case eObjectType.PolearmWeapon:
				case eObjectType.Spear:
					{
						if (property == eProperty.Intelligence || property == eProperty.Empathy || property == eProperty.Piety)
							return false;
						break;
					}
			}
			return true;
		}

		private static void WriteBonus(InventoryItem item, eProperty property, int amount)
		{
			if (item.Bonus1 == 0)
			{
				item.Bonus1 = amount;
				item.Bonus1Type = (int)property;
			}
			else if (item.Bonus2 == 0)
			{
				item.Bonus2 = amount;
				item.Bonus2Type = (int)property;
			}
			else if (item.Bonus3 == 0)
			{
				item.Bonus3 = amount;
				item.Bonus3Type = (int)property;
			}
			else if (item.Bonus4 == 0)
			{
				item.Bonus4 = amount;
				item.Bonus4Type = (int)property;
			}
		}

		private static bool BonusExists(InventoryItem item, eProperty property)
		{
			if (item.Bonus1Type == (int)property ||
				item.Bonus2Type == (int)property ||
				item.Bonus3Type == (int)property ||
				item.Bonus4Type == (int)property)
				return true;

			return false;
		}

		private static int GetBonusAmount(eBonusType type, eProperty property, int level)
		{
			switch (type)
			{
				case eBonusType.Focus:
					{
						return level;
					}
				case eBonusType.Resist:
					{
						int max = (int)Math.Ceiling((((level / 2.0) + 1) / 3));
						return Util.Random(1, max);
					}
				case eBonusType.Skill:
					{
						int max = (int)Math.Ceiling(((level / 5.0) + 1) / 3);
						return Util.Random(1, max);
					}
				case eBonusType.Stat:
					{
						if (property == eProperty.MaxHealth)
						{
							int max = (int)Math.Ceiling(((double)level * 4.0) / 3);
							return Util.Random(1, max);
						}
						else if (property == eProperty.MaxMana)
						{
							int max = (int)Math.Ceiling(((double)level / 2.0 + 1) / 3);
							return Util.Random(1, max);
						}
						else
						{
							int max = (int)Math.Ceiling(((double)level * 1.5) / 3);
							return Util.Random(1, max);
						}
					}
			}
			return 1;
		}

		private static string ArmorSlotToName(eInventorySlot slot)
		{
			switch (slot)
			{
				case eInventorySlot.ArmsArmor: return "Sleeves";
				case eInventorySlot.FeetArmor: return "Boots";
				case eInventorySlot.HandsArmor: return "Gloves";
				case eInventorySlot.HeadArmor: return "Helm";
				case eInventorySlot.LegsArmor: return "Pants";
				case eInventorySlot.TorsoArmor: return "Vest";
				default: return "No armor slot to name found for " + GlobalConstants.SlotToName((int)slot);
			}
		}

		private static int GetAbsorb(eObjectType type)
		{
			switch (type)
			{
				case eObjectType.Cloth: return 0;
				case eObjectType.Leather: return 10;
				case eObjectType.Studded: return 19;
				case eObjectType.Reinforced: return 19;
				case eObjectType.Chain: return 27;
				case eObjectType.Scale: return 27;
				case eObjectType.Plate: return 34;
				default: return 0;
			}
		}

		private static int GetWeaponSpeed(InventoryItem item)
		{
			switch ((eObjectType)item.Object_Type)
			{
				case eObjectType.SlashingWeapon:
				case eObjectType.ThrustWeapon:
				case eObjectType.CrushingWeapon:
					{
						//min 2.5 max 4.5
						if (item.Hand == 2)
							return Util.Random(22, 40);
						else return Util.Random(28, 45);
					}
				case eObjectType.Fired:
					{
						//min 30 max 50
						return Util.Random(30, 50);
					}
				case eObjectType.TwoHandedWeapon:
					{
						//min 37 max 57
						return Util.Random(37, 57);
					}
				case eObjectType.PolearmWeapon:
					{
						//min 30 max 60
						return Util.Random(30, 60);
					}
				case eObjectType.Staff:
					{
						// i dont know - edit: suncheck (friar staffs are fast)
						return Util.Random(30, 60);
					}
				case eObjectType.Longbow:
					{
						//40 to 58
						return Util.Random(30, 58);
					}
				case eObjectType.Crossbow:
					{
						//38 to 55
						return Util.Random(38, 55);
					}
				case eObjectType.Flexible:
					{
						//32 to 42
						return Util.Random(32, 42);
					}
				case eObjectType.Sword:
				case eObjectType.Hammer:
				case eObjectType.Axe:
					{
						//28 to 58
						if (item.Hand == 1)
							return Util.Random(50, 58);
						else return Util.Random(28, 45);
					}
				case eObjectType.Spear:
					{
						//39 to 58
						return Util.Random(39, 58);
					}
				case eObjectType.CompositeBow:
					{
						//37 50
						return Util.Random(37, 50);
					}
				case eObjectType.LeftAxe:
					{
						//20 40
						return Util.Random(20, 40);
					}
				case eObjectType.HandToHand:
					{
						//25 44
						return Util.Random(25, 44);
					}
				case eObjectType.RecurvedBow:
					{
						//40 54
						return Util.Random(40, 54);
					}
				case eObjectType.Blades:
				case eObjectType.Blunt:
				case eObjectType.Piercing:
					{
						//22 41
						if (item.Hand == 2)
							return Util.Random(22, 40);
						else return Util.Random(28, 45);
					}
				case eObjectType.LargeWeapons:
					{
						//40 56
						return Util.Random(40, 56);
					}
				case eObjectType.CelticSpear:
					{
						//33 58
						return Util.Random(33, 58);
					}
				case eObjectType.Scythe:
					{
						//37 57
						return Util.Random(37, 57);
					}
				case eObjectType.Shield:
					{
						return 50;
					}
			}
			//temporary right hand 4.0 left hand 3.0 two hand 6.0 ranged 5.5
			if (item.Hand == 1)
				return 50;
			else if (item.Hand == 2)
				return 30;
			else return 40;
		}

		public static int GenerateItemWeight(eObjectType type, eInventorySlot slot)
		{
			switch (type)
			{
				case eObjectType.LeftAxe:
				case eObjectType.Flexible:
				case eObjectType.Axe:
				case eObjectType.Blades:
				case eObjectType.HandToHand:
					return 25;
				case eObjectType.CompositeBow:
				case eObjectType.RecurvedBow:
				case eObjectType.Longbow:
				case eObjectType.Blunt:
				case eObjectType.CrushingWeapon:
				case eObjectType.Fired:
				case eObjectType.Hammer:
				case eObjectType.Piercing:
				case eObjectType.SlashingWeapon:
				case eObjectType.Sword:
				case eObjectType.ThrustWeapon:
					return 30;
				case eObjectType.Crossbow:
				case eObjectType.Spear:
				case eObjectType.CelticSpear:
				case eObjectType.Staff:
				case eObjectType.TwoHandedWeapon:
					return 40;
				case eObjectType.Scale:
				case eObjectType.Chain:
					{
						switch (slot)
						{
							case eInventorySlot.ArmsArmor: return 48;
							case eInventorySlot.FeetArmor: return 32;
							case eInventorySlot.HandsArmor: return 32;
							case eInventorySlot.HeadArmor: return 32;
							case eInventorySlot.LegsArmor: return 56;
							case eInventorySlot.TorsoArmor: return 80;
						}
						return 0;
					}
				case eObjectType.Cloth:
					{
						switch (slot)
						{
							case eInventorySlot.ArmsArmor: return 8;
							case eInventorySlot.FeetArmor: return 8;
							case eInventorySlot.HandsArmor: return 8;
							case eInventorySlot.HeadArmor: return 32;
							case eInventorySlot.LegsArmor: return 14;
							case eInventorySlot.TorsoArmor: return 20;
						}
						return 0;
					}
				case eObjectType.Instrument:
					return 15;
				case eObjectType.LargeWeapons:
					return 50;
				case eObjectType.Leather:
					{
						switch (slot)
						{
							case eInventorySlot.ArmsArmor: return 24;
							case eInventorySlot.FeetArmor: return 16;
							case eInventorySlot.HandsArmor: return 16;
							case eInventorySlot.HeadArmor: return 16;
							case eInventorySlot.LegsArmor: return 28;
							case eInventorySlot.TorsoArmor: return 40;
						}
						return 0;
					}
				case eObjectType.Magical:
					return 5;
				case eObjectType.Plate:
					{
						switch (slot)
						{
							case eInventorySlot.ArmsArmor: return 54;
							case eInventorySlot.FeetArmor: return 36;
							case eInventorySlot.HandsArmor: return 36;
							case eInventorySlot.HeadArmor: return 40;
							case eInventorySlot.LegsArmor: return 63;
							case eInventorySlot.TorsoArmor: return 90;
						}
						return 0;
					}
				case eObjectType.PolearmWeapon:
					return 60;
				case eObjectType.Reinforced:
				case eObjectType.Studded:
					{
						switch (slot)
						{
							case eInventorySlot.ArmsArmor: return 36;
							case eInventorySlot.FeetArmor: return 24;
							case eInventorySlot.HandsArmor: return 24;
							case eInventorySlot.HeadArmor: return 24;
							case eInventorySlot.LegsArmor: return 42;
							case eInventorySlot.TorsoArmor: return 60;
						}
						return 0;
					}
				case eObjectType.Scythe:
					return 40;
				case eObjectType.Shield:
					return 31;
			}
			return 10;
		}

		public enum eGenerateType
		{
			Weapon,
			Armor,
			Magical,
			None,
		}

		private static eProperty[] StatBonus = new eProperty[] 
{
		eProperty.Strength,
		eProperty.Dexterity,
		eProperty.Constitution,
		eProperty.Quickness,
		eProperty.Intelligence,
		eProperty.Piety,
		eProperty.Empathy,
		eProperty.Charisma,
		eProperty.MaxMana,
		eProperty.MaxHealth,
};

		private static eProperty[] ResistBonus = new eProperty[] 
{
			// resists
		eProperty.Resist_Body,
		eProperty.Resist_Cold,
		eProperty.Resist_Crush,
		eProperty.Resist_Energy,
		eProperty.Resist_Heat,
		eProperty.Resist_Matter,
		eProperty.Resist_Slash,
		eProperty.Resist_Spirit,
		eProperty.Resist_Thrust,
};

		private static eProperty[] SkillBonus = new eProperty[] 
{
			// skills
		eProperty.Skill_First,
		eProperty.Skill_Two_Handed,
		eProperty.Skill_Body,
		eProperty.Skill_Chants,
		eProperty.Skill_Critical_Strike,
		eProperty.Skill_Cross_Bows,
		eProperty.Skill_Crushing,
		eProperty.Skill_Death_Servant,
		eProperty.Skill_DeathSight,
		eProperty.Skill_Dual_Wield,
		eProperty.Skill_Earth,
		eProperty.Skill_Enhancement,
		eProperty.Skill_Envenom,
		eProperty.Skill_Fire,
		eProperty.Skill_Flexible_Weapon,
		eProperty.Skill_Cold,
		eProperty.Skill_Instruments,
		eProperty.Skill_Long_bows,
		eProperty.Skill_Matter,
		eProperty.Skill_Mind,
		eProperty.Skill_Pain_working,
		eProperty.Skill_Parry,
		eProperty.Skill_Polearms,
		eProperty.Skill_Rejuvenation,
		eProperty.Skill_Shields,
		eProperty.Skill_Slashing,
		eProperty.Skill_Smiting,
		eProperty.Skill_SoulRending,
		eProperty.Skill_Spirit,
		eProperty.Skill_Staff,
		eProperty.Skill_Stealth,
		eProperty.Skill_Thrusting,
		eProperty.Skill_Wind,
		eProperty.Skill_Sword,
		eProperty.Skill_Hammer,
		eProperty.Skill_Axe,
		eProperty.Skill_Left_Axe,
		eProperty.Skill_Spear,
		eProperty.Skill_Mending,
		eProperty.Skill_Augmentation,
		//Skill_Cave_Magic = 59,
		eProperty.Skill_Darkness,
		eProperty.Skill_Suppression,
		eProperty.Skill_Runecarving,
		eProperty.Skill_Stormcalling,
		eProperty.Skill_BeastCraft,
		eProperty.Skill_Light,
		eProperty.Skill_Void,
		eProperty.Skill_Mana,
		eProperty.Skill_Composite,
		eProperty.Skill_Battlesongs,
		eProperty.Skill_Enchantments,

		eProperty.Skill_Blades,
		eProperty.Skill_Blunt,
		eProperty.Skill_Piercing,
		eProperty.Skill_Large_Weapon,
		eProperty.Skill_Mentalism,
		eProperty.Skill_Regrowth,
		eProperty.Skill_Nurture,
		eProperty.Skill_Nature,
		eProperty.Skill_Music,
		eProperty.Skill_Celtic_Dual,
		eProperty.Skill_Celtic_Spear,
		eProperty.Skill_RecurvedBow,
		eProperty.Skill_Valor,
		eProperty.Skill_Subterranean,
		eProperty.Skill_BoneArmy,
		eProperty.Skill_Verdant,
		eProperty.Skill_Creeping,
		eProperty.Skill_Arboreal,
		eProperty.Skill_Scythe,
		eProperty.Skill_Thrown_Weapons,
		eProperty.Skill_HandToHand,
		eProperty.Skill_ShortBow,
		eProperty.Skill_Pacification,
		eProperty.Skill_Savagery,
		eProperty.Skill_Nightshade,
		eProperty.Skill_Pathfinding,
		eProperty.Skill_Summoning,
		eProperty.Skill_Dementia,
		eProperty.Skill_ShadowMastery,
		eProperty.Skill_VampiiricEmbrace,
		eProperty.Skill_EtherealShriek,
		eProperty.Skill_PhantasmalWail,
		eProperty.Skill_SpectralForce,
		eProperty.Skill_OdinsWill,
		eProperty.Skill_Cursing,
		eProperty.Skill_Hexing,
		eProperty.Skill_Witchcraft,
		eProperty.Skill_Last,
};

		private static int[] ArmorSlots = new int[] { 21, 22, 23, 25, 27, 28, };
		private static int[] MagicalSlots = new int[] { 24, 26, 29, 32, 33, 34, 35, 36 };

		private static eObjectType[] AlbionWeapons = new eObjectType[] 
{
	eObjectType.CrushingWeapon,
	eObjectType.SlashingWeapon, 
	eObjectType.ThrustWeapon,
	eObjectType.Fired,
	eObjectType.TwoHandedWeapon,
	eObjectType.PolearmWeapon,
	eObjectType.Staff,
	eObjectType.Longbow,
	eObjectType.Crossbow,
	eObjectType.Flexible,
	eObjectType.Instrument,
	eObjectType.Shield,
};
		private static eObjectType[] AlbionArmor = new eObjectType[] 
{
	eObjectType.Cloth,
	eObjectType.Leather,
	eObjectType.Studded,
	eObjectType.Chain,
	eObjectType.Plate,
};
		private static eObjectType[] MidgardWeapons = new eObjectType[] 
{
	eObjectType.Sword,
	eObjectType.Hammer,
	eObjectType.Axe,
	eObjectType.Spear,
	eObjectType.CompositeBow ,
	eObjectType.LeftAxe,
	eObjectType.HandToHand,
	eObjectType.Shield,
	eObjectType.Staff,
};
		private static eObjectType[] MidgardArmor = new eObjectType[] 
{
	eObjectType.Cloth,
	eObjectType.Leather,
	eObjectType.Studded,
	eObjectType.Chain,
};
		private static eObjectType[] HiberniaWeapons = new eObjectType[] 
{
	eObjectType.RecurvedBow,
	eObjectType.Blades,
	eObjectType.Blunt,
	eObjectType.Piercing,
	eObjectType.LargeWeapons,
	eObjectType.CelticSpear,
	eObjectType.Scythe,
	eObjectType.Instrument,
	eObjectType.Shield,
	eObjectType.Staff,
};
		private static eObjectType[] HiberniaArmor = new eObjectType[] 
{
	eObjectType.Cloth,
	eObjectType.Leather,
	eObjectType.Reinforced,
	eObjectType.Scale,
};
	}

}
