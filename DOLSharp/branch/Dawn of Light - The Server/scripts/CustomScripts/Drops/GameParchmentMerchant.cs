using System;
using System.Collections;
using System.Reflection;
using System.Threading;
using DOL.AI.Brain;
using DOL.Database;
using DOL.GS.PacketHandler;
using log4net;
using DOL.GS.Scripts;

namespace DOL.GS.Scripts
{
	/// <summary>
	/// Represents an in-game merchant
	/// </summary>
	public class GameParchmentMerchant : GameCountMerchant
	{
        public override bool Interact(GamePlayer player)
        {
            if(!base.Interact(player))
                return false;

            player.Out.SendMessage("Hail " + player.RealmTitle + ", I have been assigned to aid warriors with " +
                                    "magical scrolls, all I require are some pieces of [parchment] in to imbue.", eChatType.CT_System, eChatLoc.CL_PopupWindow);
            return true;
        }

        public override bool WhisperReceive(GameLiving source, string str)
        {
            GamePlayer player = source as GamePlayer;

            if (!base.WhisperReceive(source, str) || player == null)
                return false;

            switch (str)
            {
                case "parchment":
                    player.Out.SendMessage("Parchment is often being carried around by humanoid-type " +
                                            "foes which carry any type of items.",eChatType.CT_System,eChatLoc.CL_PopupWindow);
                    break;
            }

            return true;
        }
        
        public GameParchmentMerchant()
	        : base()
		{
			m_moneyItem = new GameInventoryItem(new InventoryItem(LootGeneratorParchment.Parchment));
			m_countText = "pieces of parchment";
		}
	}
}