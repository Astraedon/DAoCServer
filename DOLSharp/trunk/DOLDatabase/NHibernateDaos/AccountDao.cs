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
using DOL.Database.IDaos;
using DOL.Database.TransferObjects;
using NHibernate.Expression;

namespace DOL.Database.NHibernateDaos
{
	public class AccountDao : IAccountDao
	{
		public Account SelectByAccountName(string accountName)
		{
			return (Account) Database.Instance.SelectObject(typeof (Account), Expression.Eq("AccountName", accountName));
		}

		public Account SelectByAccountName(int accountID)
		{
			return (Account) Database.Instance.SelectObject(typeof (Account), Expression.Eq("AccountID", accountID));
		}

		public void Save(Account account)
		{
			Database.Instance.SaveObject(account);
		}

		public int SaveAll()
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}