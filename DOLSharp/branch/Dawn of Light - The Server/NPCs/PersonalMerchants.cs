namespace DOL.GS.Scripts
{
	public class MarketExplorer : GameNPC
	{
		public override bool Interact(GamePlayer player)
		{
			if (!base.Interact(player))
				return false;

			player.Out.SendMarketExplorerWindow();

			return true;
		}
	}

	public class ConsignmentMerchant : GameNPC
	{
		public override bool Interact(GamePlayer player)
		{
			if (!base.Interact(player))
				return false;

			//0:05:27.359 S=>C 0x02 inventory update v182 (slots:0 bits:0x00 visibleSlots:0x00 preAction:0x05)
			player.Out.SendInventoryItemsUpdate(0x05, null);
			//0:05:27.375 S=>C 0x1E consignment merchant money (copper:0  silver:0  gold:0   mithril:0   platinum:0)

			return true;
		}
	}
}

namespace DOL.Database
{
	public class PlayerMerchantItem : DataObject
	{
		private string m_inventoryitemid;
		private long m_price;

		public override bool AutoSave
		{
			get { return true; }
			set { }
		}
	}
}
