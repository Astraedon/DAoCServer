using System;
using System.Reflection;
using DOL.Events;
using DOL.GS.PacketHandler;
using DOL.GS.Scripts;
using DOL.Database;
using log4net;

namespace DOL.GS.GameEvents
{
	//the event class
	public class ParchmentEvent
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		//Every entry in this array gets a Merchant
		private static GLoc[] m_locations = { new GLoc(33626, 30638, 8002, 2000, 10, 1), new GLoc(31996, 34241, 8032, 4000, 101, 2), new GLoc(33383, 32330, 8000, 2044, 201, 3) };
		//Every string in this array is added to m_tradeItems
		private static ItemTemplate[] m_items = { UtilityScrollsEvent.HealerScroll, UtilityScrollsEvent.MerchantScroll, UtilityScrollsEvent.TeleporterScroll, UtilityScrollsEvent.Tinderbox, UtilityScrollsEvent.TrainerScroll, LootGeneratorRam.Ram};

		[ScriptLoadedEvent]
		public static void OnScriptCompiled(DOLEvent e, object sender, EventArgs args)
		{
			//Stores items to be sold
			MerchantTradeItems m_tradeitems = new MerchantTradeItems("ScrollMerchant");
			//Stores the page items are being added to
			int m_page = 0;
			//Loops through the m_items array
			for (int i = 0; i <= m_items.Length - 1; i++)
			{
				//if i is 30 60 90 120 or 150 pages in being increased by 1
				if (i > MerchantTradeItems.MAX_ITEM_IN_TRADEWINDOWS * (m_page + 1))
				{
					if (m_page <= MerchantTradeItems.MAX_PAGES_IN_TRADEWINDOWS)
						m_page++;
				}

				//try to get an ItemTemplate from the string in the array if null do nothing else add to TradeItems
				ItemTemplate m_template = m_items[i] as ItemTemplate;
				log.Info("ParchmentEvent: added " + m_template.Name + " to MerchantTradeItems.");
				m_tradeitems.AddTradeItem(m_page, eMerchantWindowSlot.FirstEmptyInPage, m_template);
			}

			log.Debug("ParchmentEvent: m_locations length: " + m_locations.Length);

			GameNpcInventoryTemplate template = new GameNpcInventoryTemplate();
			template.AddNPCEquipment(eInventorySlot.TwoHandWeapon, 1658);
			template.AddNPCEquipment(eInventorySlot.Cloak, 1720);
			template.AddNPCEquipment(eInventorySlot.TorsoArmor, 2245);
			template = template.CloseTemplate();

			//loop through the m_locations array and create a new merchant everywhere
			foreach (GLoc loc in m_locations)
			{
				log.Debug("ParchmentEvent: Loc " + loc.ToString());
				GameParchmentMerchant pmerchant = new GameParchmentMerchant();
				pmerchant.X = loc.X;
				pmerchant.Y = loc.Y;
				pmerchant.Z = loc.Z;
				pmerchant.Heading = loc.Heading;
				pmerchant.CurrentRegionID = loc.Region;
				pmerchant.Level = 45;
				pmerchant.GuildName = "Scroll Merchant";
				pmerchant.CurrentSpeed = 0;
				pmerchant.MaxSpeedBase = 200;

				//support for different models depending on realm if realm is 0 default values are being used
				switch (loc.Realm)
				{
					case 1:
						pmerchant.Name = "Parchment Master";
						pmerchant.Model = 50;
						pmerchant.Size = 50;
						pmerchant.Realm = 1;
						break;
					case 2:
						pmerchant.Name = "Parchment Master";
						pmerchant.Model = 212;
						pmerchant.Size = 50;
						pmerchant.Realm = 2;
						break;
					case 3:
						pmerchant.Name = "Parchment Master";
						pmerchant.Model = 300;
						pmerchant.Size = 50;
						pmerchant.Realm = 3;
						break;
					default:
						pmerchant.Name = "Default Name";
						pmerchant.Model = 408;
						pmerchant.Size = 50;
						pmerchant.Realm = 0;
						pmerchant.Flags ^= (uint)GameNPC.eFlags.PEACE;
						break;
				}

				pmerchant.Inventory = template;

				//assign TradeItems to merchant
				pmerchant.TradeItems = m_tradeitems;

				//if theres an error let everyone know
				if (!pmerchant.AddToWorld())
				{
					log.Error("ParchmentEvent: Couldnt add " + pmerchant.ToString() + " to world, returning false.");
				}
			}
		}

		[ScriptUnloadedEvent]
		public static void OnScriptUnloaded(DOLEvent e, object sender, EventArgs args)
		{
			//I dont need to remove my npcs do I?
		}
	}

	//GLoc class to make supporting multiple GLocs that the NPC is being added to easier
	public class GLoc
	{
		protected int m_x = 0;
		protected int m_y = 0;
		protected int m_z = 0;
		protected ushort m_h = 0;
		protected ushort m_r = 0;
		protected byte m_realm = 0;

		//constructor with realm
		public GLoc(int x, int y, int z, ushort h, ushort r, byte realm)
		{
			m_x = x;
			m_y = y;
			m_z = z;
			m_h = h;
			m_r = r;
			m_realm = realm;
		}

		//constructor without realm
		public GLoc(int x, int y, int z, ushort h, ushort r)
		{
			m_x = x;
			m_y = y;
			m_z = z;
			m_h = h;
			m_r = r;
		}

		public int X
		{
			get { return m_x; }
		}
		public int Y
		{
			get { return m_y; }
		}
		public int Z
		{
			get { return m_z; }
		}
		public ushort Heading
		{
			get { return m_h; }
		}
		public ushort Region
		{
			get { return m_r; }
		}

		public byte Realm
		{
			get { return m_realm; }
		}

		public override string ToString()
		{
			return "X: " + m_x + " Y: " + m_y + " Z: " + m_z + " Heading: " + m_h + " Region: " + m_r + " Realm " + m_realm;
		}
	}
}