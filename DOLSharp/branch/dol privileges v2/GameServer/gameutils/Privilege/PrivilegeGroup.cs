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
    public class PrivilegeGroup
    {
        private readonly DBPrivilegeGroup _dbEntry;
        private readonly IList<string> _privileges = new List<string>(); 
        private readonly IList<PrivilegeGroup> _inheritedGroups = new List<PrivilegeGroup>();

        public PrivilegeGroup(DBPrivilegeGroup entry)
        {
            _dbEntry = entry;
        }

        public void Initialize()
        {
            if (!string.IsNullOrEmpty(DBEntry.Privileges))
                Privileges.AddRange(DBEntry.Privileges.Split(';'));

            if (!string.IsNullOrEmpty(DBEntry.Commands))
                foreach (string str in DBEntry.Commands.Split(';'))
                    Privileges.Add("cmd_" + str);
        }

        /// <summary>
        /// Resolve all the groups stored in the DB Entry to actual objects, to be used
        /// after the central group cache has been built.
        /// </summary>
        public void ResolveGroups()
        {
            if (!string.IsNullOrEmpty(DBEntry.InheritedGroups))
            {
                foreach (string privilegeGroup in DBEntry.InheritedGroups.Split(';'))
                {
                    ushort key;
                    PrivilegeGroup toAdd = ushort.TryParse(privilegeGroup, out key) ?
                        PrivilegeManager.GetGroupFromID(key) : PrivilegeManager.GetGroupFromName(privilegeGroup);

                    if (toAdd != null)
                        InheritedGroups.Add(toAdd);
                }
            }
        }

        public bool HasPrivilege(string privilegeKey)
        {
            return Privileges.Any(s => s == "*" || s == privilegeKey) || _inheritedGroups.Any(sg => sg.HasPrivilege(privilegeKey));
        }

        public DBPrivilegeGroup DBEntry
        {
            get { return _dbEntry; }
        }

        public IList<PrivilegeGroup> InheritedGroups
        {
            get { return _inheritedGroups; }
        }

        public IList<string> Privileges
        {
            get { return _privileges; }
        }


        /// <summary>
        /// Check if this group has a circular inheritance chain, ie does this group
        /// inherit from itself via a sub group or otherwise? Used to prevent it from
        /// causing stack overflows in case someone has done something stupid.
        /// </summary>
        /// <returns>Whether or not there's a circular inheritance chain.</returns>
        public bool HasCircularInheritanceChain()
        {
            foreach (PrivilegeGroup sg in InheritedGroups)
            {
                if (sg.HasInherited(DBEntry.GroupIndex) || InheritedGroups.Any(ig => ig.HasInherited(DBEntry.GroupIndex)))
                    return true;
            }
            return false;
        }

        private bool HasInherited(int groupID)
        {
            return DBEntry.GroupIndex == groupID || InheritedGroups.Any(sg => sg.DBEntry.GroupIndex == groupID || sg.HasInherited(groupID));
        }
    }
}
