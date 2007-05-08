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
