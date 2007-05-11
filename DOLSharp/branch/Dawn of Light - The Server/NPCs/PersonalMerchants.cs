using System;
using System.Reflection;
using DOL.Database;
using DOL.GS.Housing;
using log4net;
using DOL.Database.Attributes;

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

		[DataElement(AllowDbNull = false)]
		public string InventoryItemID
		{
			get { return m_inventoryitemid; }
			set
			{
				Dirty = true;
				m_inventoryitemid = value;
			}
		}

		[DataElement(AllowDbNull = false)]
		public long Price
		{
			get { return m_price; }
			set
			{
				Dirty = true;
				m_price = value;
			}
		}

		public override bool AutoSave
		{
			get { return true; }
			set { }
		}
	}
}

namespace DOL.GS.PacketHandler.Client.v168
{
	[PacketHandler(PacketHandlerType.TCP, 0x1A, "Set market price")]
	public class PlayerSetMarketPriceHandler : IPacketHandler
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player == null)
				return 0;

			int slot = packet.ReadByte();
			int unk1 = packet.ReadByte();
			ushort unk2 = packet.ReadShort();
			uint price = packet.ReadInt();

			System.Text.StringBuilder str = new System.Text.StringBuilder();
			str.AppendFormat("PlayerSetMarketPrice: slot:{0,2} price:{1,-7} unk1:0x{2:X2} unk2:0x{3:X4}", slot, price, unk1, unk2);
			log.Debug(str.ToString());
			return 1;
		}
	}
}
