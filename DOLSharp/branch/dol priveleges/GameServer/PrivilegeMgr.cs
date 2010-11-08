/*
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
using DOL.Database;

namespace DOL.GS
{
	/// <summary>
	/// The privilege level of the client
	/// Bibary 2^x flag
	/// </summary>
	public enum ePrivLevel : uint
	{
		/// <summary>
		/// Player rights
		/// </summary>
		Player = 1,
		/// <summary>
		/// Translation rights
		/// </summary>
		Translator = 2,
		/// <summary>
		/// GameMaster rights
		/// </summary>
		GM = 4,
		/// <summary>
		/// Admin rights
		/// </summary>
		Admin = 8,
	}
	/// <summary>
	/// Description of the view content
	/// </summary>
	public static class PrivilegeMgr
	{
		/// <summary>
		/// Allows the selected privilege level
		/// </summary>
		/// <param name="client"></param>
		/// <param name="plvl"></param>
		/// <returns></returns>
		public static bool HavePrivilege(GameClient client, ePrivLevel plvl)
		{
			return HavePrivilege(client.Account, plvl);
		}
		public static bool HavePrivilege(GamePlayer player, ePrivLevel plvl)
		{
			return HavePrivilege(player.Client.Account, plvl);
		}
		public static bool HavePrivilege(Account account, ePrivLevel plvl)
		{
			return ((account.PrivLevel & (uint)plvl) > 0);
		}
		
		/// <summary>
		/// Allows either GM or Admin
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		public static bool IsGameMaster(Account account)
		{
			return (HavePrivilege(account, ePrivLevel.GM) || HavePrivilege(account, ePrivLevel.Admin));
		}
		public static bool IsGameMaster(GameClient client)
		{
			return IsGameMaster(client.Account);
		}
		public static bool IsGameMaster(GamePlayer player)
		{
			return IsGameMaster(player.Client.Account);
		}
	}
}
