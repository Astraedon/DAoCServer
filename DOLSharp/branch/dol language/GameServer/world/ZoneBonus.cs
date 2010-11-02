﻿/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */
using System;
using System.Collections.Generic;
using System.Text;
using DOL.Database;
using DOL.GS.PacketHandler;
using log4net;
using System.Reflection;
using DOL.Language;

namespace DOL.GS
{
    public class ZoneBonus
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region eZoneBonusType
        public enum eZoneBonusType
        {
            XP = 0,
            RP = 1,
            BP = 2,
            COIN = 3,
        } 
        #endregion
        #region Get Bonuses Methods
        public static int GetXPBonus(GamePlayer player)
        {
            return player.CurrentZone.BonusExperience;
        }
        public static int GetRPBonus(GamePlayer player)
        {
            return player.CurrentZone.BonusRealmpoints;
        }
        public static int GetBPBonus(GamePlayer player)
        {
            return player.CurrentZone.BonusBountypoints;
        }
        public static int GetCoinBonus(GamePlayer player)
        {
            return player.CurrentZone.BonusCoin;
        } 
        #endregion
        #region Get Bonus Message
        public static string GetBonusMessage(GamePlayer player, int bonusAmount, eZoneBonusType type)
        {
            switch (type)
            {
                case eZoneBonusType.XP:
                    return LanguageMgr.GetTranslation(player.Client, eTranslationKey.SystemText, "You gain an additional {points} experience for adventuring in this zone!",
                        "").Replace("{points}", bonusAmount.ToString());
                case eZoneBonusType.RP:
                    return LanguageMgr.GetTranslation(player.Client, eTranslationKey.SystemText, "You gain an additional {points} realmpoints for adventuring in this zone!",
                        "").Replace("{points}", bonusAmount.ToString());
                case eZoneBonusType.BP:
                    return LanguageMgr.GetTranslation(player.Client, eTranslationKey.SystemText, "You gain an additional {points} bountypoints for adventuring in this zone!",
                        "").Replace("{points}", bonusAmount.ToString());
                case eZoneBonusType.COIN:
                    return LanguageMgr.GetTranslation(player.Client, eTranslationKey.SystemText, "You gain additional coins for adventuring in this zone!", "");
                default: return "No Bonus Type Found";
            }
        } 
        #endregion

    }
}
