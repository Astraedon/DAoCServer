/*
 * Author:		Etaew  originally written for Fallen Realms, and used on Dracis
 * Version:		2
 */

using System;
using System.Collections ;
using DOL;
using DOL.GS;
using DOL.Database;
using DOL.GS.PacketHandler;
using DOL.Events;
using log4net;

namespace DOL.GS.Scripts
{
	public class RespecMerchant : GameNPC
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private static GameNPC Albion_RespecMerchant = null;
		private static GameNPC Midgard_RespecMerchant = null;
		private static GameNPC Hibernia_RespecMerchant = null;

		[ScriptLoadedEvent]
		public static void ScriptLoaded(DOLEvent e, object sender, EventArgs args)
		{
			GameNpcInventoryTemplate template = new GameNpcInventoryTemplate();
			template.AddNPCEquipment(eInventorySlot.TwoHandWeapon, 1658);
			template.AddNPCEquipment(eInventorySlot.Cloak, 1720);
			template.AddNPCEquipment(eInventorySlot.TorsoArmor, 2245);
			template = template.CloseTemplate();

			string name = "Skills Master";
			string guildname = "Respec Merchant";
			byte size = 60;
			byte level = 50;

			Albion_RespecMerchant = new RespecMerchant();
			Albion_RespecMerchant.Model = 280;
			Albion_RespecMerchant.Name = name;
			Albion_RespecMerchant.GuildName = guildname;
			Albion_RespecMerchant.Realm = (byte)eRealm.Albion;
			Albion_RespecMerchant.CurrentRegionID = 10;
			Albion_RespecMerchant.Size = size;
			Albion_RespecMerchant.Level = level;
			Albion_RespecMerchant.X = 33096;
			Albion_RespecMerchant.Y = 22847;
			Albion_RespecMerchant.Z = 8480;
			Albion_RespecMerchant.Heading = 15;
			Albion_RespecMerchant.Inventory = template;
			Albion_RespecMerchant.AddToWorld();
			Albion_RespecMerchant.SwitchWeapon(eActiveWeaponSlot.TwoHanded);

			Midgard_RespecMerchant = new RespecMerchant();
			Midgard_RespecMerchant.Model = 160;
			Midgard_RespecMerchant.Name = name;
			Midgard_RespecMerchant.GuildName = guildname;
			Midgard_RespecMerchant.Realm = (byte)eRealm.Midgard;
			Midgard_RespecMerchant.CurrentRegionID = 101;
			Midgard_RespecMerchant.Size = size;
			Midgard_RespecMerchant.Level = level;
			Midgard_RespecMerchant.X = 32059;
			Midgard_RespecMerchant.Y = 27616;
			Midgard_RespecMerchant.Z = 8800;
			Midgard_RespecMerchant.Heading = 1015;
			Midgard_RespecMerchant.Inventory = template;
			Midgard_RespecMerchant.AddToWorld();
			Midgard_RespecMerchant.SwitchWeapon(eActiveWeaponSlot.TwoHanded);

			Hibernia_RespecMerchant = new RespecMerchant();
			Hibernia_RespecMerchant.Model = 340;
			Hibernia_RespecMerchant.Name = name;
			Hibernia_RespecMerchant.GuildName = guildname;
			Hibernia_RespecMerchant.Realm = (byte)eRealm.Hibernia;
			Hibernia_RespecMerchant.CurrentRegionID = 201;
			Hibernia_RespecMerchant.Size = size;
			Hibernia_RespecMerchant.Level = level;
			Hibernia_RespecMerchant.X = 32815;
			Hibernia_RespecMerchant.Y = 31784;
			Hibernia_RespecMerchant.Z = 8000;
			Hibernia_RespecMerchant.Heading = 25;
			Hibernia_RespecMerchant.Inventory = template;
			Hibernia_RespecMerchant.AddToWorld();
			Hibernia_RespecMerchant.SwitchWeapon(eActiveWeaponSlot.TwoHanded);

			if (log.IsInfoEnabled)
				log.Info("Respec Merchants Loaded");
		}

		public override IList GetExamineMessages(GamePlayer player)
		{
			IList list = new ArrayList();
			list.Add("You examine " + GetName(0, false) + ".  " + GetPronoun(0, true) + " is " + GetAggroLevelString(player, false) + " and is an Respec Merchant.");
			return list;
		}
		private static long plat = 1000 * 100 * 100;
		private static long AllCash = 3 * plat;
		private static long SingleCash = 1 * plat;
		private static long RealmCash = 1 * plat;
		private static long CraftingCash = 1 * plat;

		public override bool Interact(GamePlayer player)
		{
			if (!base.Interact(player))
				return false;

			TurnTo(player.X, player.Y);
			SayTo(player, "Hi " + player.Name + ",\n Would you like to purchase a [Full Respec] for " + Money.GetString(AllCash) + ", a [Single Respec] for " + Money.GetString(SingleCash) + " or a [Realm Respec] for " + Money.GetString(RealmCash) + "\n"
				+ "You currently have " + player.RespecAmountAllSkill + " Full respecs, " + player.RespecAmountSingleSkill + " single respecs, and " + player.RespecAmountRealmSkill + " realm respecs!");

			SayTo(player, "The goverment has received petitions regarding the ability to [Respec Crafting] abilities, we now offer this service.");

			return true;
		}

		public override bool WhisperReceive(GameLiving source, string str)
		{
			if (!base.WhisperReceive(source, str)) return false;
			GamePlayer player = source as GamePlayer;
			if (player == null) return false;
			switch (str.ToLower())
			{
				case "full respec":
					{
						player.TempProperties.setProperty("stone", "full");
						player.Out.SendCustomDialog("Buy a Full Respec for " + Money.GetString(AllCash), new CustomDialogResponse(BuyStoneCallback));
						break;
					}
				case "single respec":
					{
						player.TempProperties.setProperty("stone", "single");
						player.Out.SendCustomDialog("Buy a Single Respec for " + Money.GetString(SingleCash), new CustomDialogResponse(BuyStoneCallback));
						break;
					}
				case "realm respec":
					{
						player.TempProperties.setProperty("stone", "realm");
						player.Out.SendCustomDialog("Buy a Realm Respec for " + Money.GetString(SingleCash), new CustomDialogResponse(BuyStoneCallback));
						break;
					}
				case "respec crafting":
					{
						player.TempProperties.setProperty("stone", "crafting");
						player.Out.SendCustomDialog("Buy a Crafting Respec for " + Money.GetString(CraftingCash), new CustomDialogResponse(BuyStoneCallback));
						break;
					}
			}
			return true;
		}

		private void BuyStoneCallback(GamePlayer player, byte response)
		{
			if (response == 0x00) return;
			if (player == null) return;
			string type = (string)player.TempProperties.getObjectProperty("stone", null);
			if (type == null)
				return;
			switch (type)
			{
				case "full":
					{
						if (player.RemoveMoney(AllCash))
						{
							player.RespecAmountAllSkill++;
							player.Out.SendMessage("You have gained a full respec!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						}
						else player.Out.SendMessage("You do not have enough money!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						break;
					}
				case "single":
					{
						if (player.RemoveMoney(SingleCash))
						{
							player.RespecAmountSingleSkill++;
							player.Out.SendMessage("You have gained a single respec!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						}
						else player.Out.SendMessage("You do not have enough money!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						break;
					}
				case "realm":
					{
						if (player.RemoveMoney(RealmCash))
						{
							player.RespecAmountRealmSkill++;
							player.Out.SendMessage("You have gained a realm respec!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						}
						else player.Out.SendMessage("You do not have enough money!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						break;
					}
				case "crafting":
					{
						if (player.RemoveMoney(CraftingCash))
						{
							player.CraftingPrimarySkill = eCraftingSkill.NoCrafting;
							player.CraftingSkills.Clear();
							player.Out.SendUpdateCraftingSkills();
							player.SaveIntoDatabase();
							player.Out.SendMessage("You have had your crafting skills cleared!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						}
						else player.Out.SendMessage("You do not have enough money!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						break;
					}
			}
		}
	}
}
