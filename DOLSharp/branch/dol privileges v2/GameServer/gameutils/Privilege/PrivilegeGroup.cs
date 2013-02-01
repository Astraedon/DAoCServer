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

using System.Linq;
using DOL.Database;

namespace DOL.GS.Privilege
{
    public class PrivilegeGroup : PrivilegeContainer
    {
        public PrivilegeGroup(DBPrivilegeGroup entry) : base(entry) { }

        #region Creation / Setup

        public void Initialize()
        {
            if (!string.IsNullOrEmpty(DBEntry.Privileges))
            {
                Privileges.AddRange(DBEntry.Privileges.Split(';').
                    Where(s => s != "" && !PrivilegeDefaults.ParameterizedRegex.IsMatch(s)).ToList());
            }
                

            if (!string.IsNullOrEmpty(DBEntry.Commands))
                foreach (string str in DBEntry.Commands.Split(';'))
                    Privileges.Add(PrivilegeDefaults.CommandPrefix + str);
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
                    PrivilegeGroup toAdd = PrivilegeManager.GetGroup(privilegeGroup);

                    if (toAdd != null)
                        Groups.Add(toAdd);
                }
            }
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Database entry backing up this PrivilegeBinding.
        /// </summary>
        public DBPrivilegeGroup DBEntry
        {
            get { return DBEntity as DBPrivilegeGroup; }
        }


        protected override string DBPrivileges
        {
            get { return DBEntry.Privileges; }
            set { DBEntry.Privileges = value; }
        }

        protected override string DBCommands
        {
            get { return DBEntry.Commands; }
            set { DBEntry.Commands = value; }
        }

        protected override string DBGroups
        {
            get { return DBEntry.InheritedGroups; }
            set { DBEntry.InheritedGroups = value; }
        }

        #endregion

        public override ModificationStatus AddGroup(PrivilegeGroup grp)
        {
            if (HasInherited(grp.DBEntry.GroupIndex)) return ModificationStatus.Circular;

            return base.AddGroup(grp);
        }


        #region Overflow Protection

        /// <summary>
        /// Check if this group has a circular inheritance chain, ie does this group
        /// inherit from itself via a sub group or otherwise? Used to prevent it from
        /// causing stack overflows in case someone has done something stupid.
        /// </summary>
        /// <returns>Whether or not there's a circular inheritance chain.</returns>
        public bool HasCircularInheritanceChain()
        {
            return Groups.Any(sg => sg.HasInherited(DBEntry.GroupIndex) ||
                Groups.Any(ig => ig.HasInherited(DBEntry.GroupIndex)));
        }

        /// <summary>
        /// Has this group inherited a specific group ID? Used to check for circular
        /// inheritance to prevent stack overflows.
        /// </summary>
        /// <param name="groupID">ID to check for.</param>
        /// <returns></returns>
        public bool HasInherited(int groupID)
        {
            return DBEntry.GroupIndex == groupID ||
                Groups.Any(sg => sg.DBEntry.GroupIndex == groupID || 
                    sg.HasInherited(groupID));
        }

        #endregion
    }
}
