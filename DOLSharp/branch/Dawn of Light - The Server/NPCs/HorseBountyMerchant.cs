using System.Collections;
using System;
using DOL.Database;
using DOL.Events;

namespace DOL.GS.Scripts
{
	/// <summary>
	/// Represents an in-game merchant
	/// </summary>
	public class GameBountyHorseMerchant : GameBountyMerchant
	{
		public override bool AddToWorld()
		{
			GameNpcInventoryTemplate template = new GameNpcInventoryTemplate();
			template.AddNPCEquipment(eInventorySlot.Cloak, 1727);
			template.AddNPCEquipment(eInventorySlot.TorsoArmor, 2121);
			template.AddNPCEquipment(eInventorySlot.LegsArmor, 2158);
			template.AddNPCEquipment(eInventorySlot.ArmsArmor, 2873);
			template.AddNPCEquipment(eInventorySlot.HandsArmor, 2492);
			template.AddNPCEquipment(eInventorySlot.FeetArmor, 2875);
			Inventory = template.CloseTemplate();
			return base.AddToWorld();
		}
		private static ArrayList m_items = new ArrayList();

		[ScriptLoadedEvent]
		public static void OnScriptCompiled(DOLEvent e, object sender, EventArgs args)
		{
			ItemTemplate chestnutHorse = (ItemTemplate)GameServer.Database.FindObjectByKey(typeof(ItemTemplate), "brownstandardmount");
			if (chestnutHorse != null)
				m_items.Add(chestnutHorse);

			ItemTemplate greyHorse = (ItemTemplate)GameServer.Database.FindObjectByKey(typeof(ItemTemplate), "greystandardmount");
			if (greyHorse != null)
				m_items.Add(greyHorse);

			ItemTemplate spottedHorse = (ItemTemplate)GameServer.Database.FindObjectByKey(typeof(ItemTemplate), "spottedstandardmount");
			if (spottedHorse != null)
				m_items.Add(spottedHorse);

			ItemTemplate chocolateHorse = (ItemTemplate)GameServer.Database.FindObjectByKey(typeof(ItemTemplate), "chocolatestandardmount");
			if (chocolateHorse != null)
				m_items.Add(chocolateHorse);

			ItemTemplate blackHorse = (ItemTemplate)GameServer.Database.FindObjectByKey(typeof(ItemTemplate), "blackstandardmount");
			if (blackHorse != null)
				m_items.Add(blackHorse);

			ItemTemplate silverHorse = (ItemTemplate)GameServer.Database.FindObjectByKey(typeof(ItemTemplate), "whitestandardmount");
			if (silverHorse != null)
				m_items.Add(silverHorse);

			ItemTemplate palominoHorse = (ItemTemplate)GameServer.Database.FindObjectByKey(typeof(ItemTemplate), "palominostandardmount");
			if (palominoHorse != null)
				m_items.Add(palominoHorse);

			/*ItemTemplate chestnutWarHorse = (ItemTemplate)GameServer.Database.FindObjectByKey(typeof(ItemTemplate), "brownheavymount");
			if (chestnutWarHorse != null)
				m_items.Add(chestnutWarHorse);

			ItemTemplate greyWarHorse = (ItemTemplate)GameServer.Database.FindObjectByKey(typeof(ItemTemplate), "greyheavymount");
			if (greyWarHorse != null)
				m_items.Add(greyWarHorse);

			ItemTemplate spottedWarHorse = (ItemTemplate)GameServer.Database.FindObjectByKey(typeof(ItemTemplate), "spottedheavymount");
			if (spottedWarHorse != null)
				m_items.Add(spottedWarHorse);

			ItemTemplate bayWarHorse = (ItemTemplate)GameServer.Database.FindObjectByKey(typeof(ItemTemplate), "spottedheavymount");
			if (bayWarHorse != null)
				m_items.Add(bayWarHorse);*/
		}

		public override bool Interact(GamePlayer player)
		{
			if (m_tradeItems == null || m_tradeItems.GetAllItems().Count == 0)
			{
				m_tradeItems = new MerchantTradeItems(null);
				foreach (ItemTemplate item in m_items)
					m_tradeItems.AddTradeItem(0, eMerchantWindowSlot.FirstEmptyInPage, item);
			}
			return base.Interact(player);
		}
	}
}