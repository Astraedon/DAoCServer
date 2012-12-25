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
using System.Collections.Generic;
using System.Linq;
using DOL.Database;

namespace DOL.GS.Privilege
{
    public class PrivilegeBinding
    {
        private readonly DBPrivilegeBinding _dbEntry;

        private readonly IList<PrivilegeGroup> _groups = new List<PrivilegeGroup>();
        private readonly IList<string> _additionalPrivileges = new List<string>(); 

        public PrivilegeBinding(DBPrivilegeBinding entry)
        {
            _dbEntry = entry;

            if (!string.IsNullOrEmpty(DBEntry.Groups))
            {
                foreach (string privilegeGroup in DBEntry.Groups.Split(';'))
                {
                    ushort key;
                    PrivilegeGroup toAdd = ushort.TryParse(privilegeGroup, out key) ?
                        PrivilegeManager.GetGroupFromID(key) : PrivilegeManager.GetGroupFromName(privilegeGroup);

                    if (toAdd != null)
                        Groups.Add(toAdd);
                }
            }

            if (!string.IsNullOrEmpty(DBEntry.AdditionalPrivileges))
                AdditionalPrivileges.AddRange(DBEntry.AdditionalPrivileges.Split(';').Where(s => s != "").ToList());

            if (!string.IsNullOrEmpty(DBEntry.AdditionalCommands))
            {
                foreach (string str in DBEntry.AdditionalCommands.Split(';'))
                {
                    if (str != "")
                        AdditionalPrivileges.Add("cmd_" + str);
                }
            }
        }

        public bool HasPrivilege(string privilegeKey)
        {
            return AdditionalPrivileges.Any(s => s == "*" || s == privilegeKey) || Groups.Any(g => g.HasPrivilege(privilegeKey));
        }

        public DBPrivilegeBinding DBEntry
        {
            get { return _dbEntry; }
        }

        public IList<string> AdditionalPrivileges
        {
            get { return _additionalPrivileges; }
        }

        public IList<PrivilegeGroup> Groups
        {
            get { return _groups; }
        }
    }
}
