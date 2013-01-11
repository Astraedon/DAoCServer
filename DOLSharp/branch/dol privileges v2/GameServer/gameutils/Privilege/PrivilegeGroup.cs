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

using System.Collections.Generic;
using System.Linq;
using DOL.Database;

namespace DOL.GS.Privilege
{
    public class PrivilegeGroup
    {
        private readonly DBPrivilegeGroup m_dbEntry;
        private readonly IList<string> m_privileges = new List<string>(); 
        private readonly IList<PrivilegeGroup> m_inheritedGroups = new List<PrivilegeGroup>();

        public PrivilegeGroup(DBPrivilegeGroup entry)
        {
            m_dbEntry = entry;
        }

        #region Creation / Setup

        public void Initialize()
        {
            if (!string.IsNullOrEmpty(DBEntry.Privileges))
                Privileges.AddRange(DBEntry.Privileges.Split(';'));

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
                        InheritedGroups.Add(toAdd);
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
            get { return m_dbEntry; }
        }

        /// <summary>
        /// Additional privileges specified to this binding on top of any in the groups.
        /// </summary>
        public IList<PrivilegeGroup> InheritedGroups
        {
            get { return m_inheritedGroups; }
        }

        /// <summary>
        /// Privileges specified to this group on top of any in the sub-groups.
        /// </summary>
        public IList<string> Privileges
        {
            get { return m_privileges; }
        }

        #endregion

        /// <summary>
        /// Does this PrivilegeGroup have the right to use this key?
        /// </summary>
        /// <param name="privilegeKey">Key to search for rights.</param>
        /// <returns></returns>
        public bool HasPrivilege(string privilegeKey)
        {
            return Privileges.Any(s => s == PrivilegeDefaults.Wildcard || s == privilegeKey) || InheritedGroups.Any(sg => sg.HasPrivilege(privilegeKey));
        }

        #region Add / Remove

        #region Privileges

        /// <summary>
        /// Adds a privilege to the Group's privileges and syncs to the database.
        /// </summary>
        /// <param name="privilegeKey">Privilege to add.</param>
        public ModificationStatus AddPrivilege(string privilegeKey)
        {
            Privileges.Add(privilegeKey);

            DBEntry.Privileges = string.Join(";", Privileges.Where(s => !s.StartsWith(PrivilegeDefaults.CommandPrefix)));
            if (!GameServer.Database.SaveObject(DBEntry))
                return ModificationStatus.FailedToSave;

            return ModificationStatus.Success;
        }

        /// <summary>
        /// Removes a privilege to the Group's privileges and syncs to the database.
        /// </summary>
        /// <param name="privilegeKey">Privilege to remove.</param>
        public ModificationStatus RemovePrivilege(string privilegeKey)
        {
            Privileges.Remove(privilegeKey);

            DBEntry.Privileges = string.Join(";", Privileges.Where(s => !s.StartsWith(PrivilegeDefaults.CommandPrefix)));
            if (!GameServer.Database.SaveObject(DBEntry))
                return ModificationStatus.FailedToSave;

            return ModificationStatus.Success;
        }

        #endregion

        #region Command

        /// <summary>
        /// Adds a command to the Group's privileges and syncs to the database.
        /// </summary>
        /// <param name="commandString">Command to add.</param>
        public ModificationStatus AddCommand(string commandString)
        {
            Privileges.Add(PrivilegeDefaults.CommandPrefix + commandString);

            string[] cmds = Privileges.Where(s => s.StartsWith(PrivilegeDefaults.CommandPrefix)).ToArray();

            for (int index = 0; index < cmds.Length; index++)
            {
                if (cmds[index].StartsWith(PrivilegeDefaults.CommandPrefix))
                    cmds[index] = cmds[index].Replace(PrivilegeDefaults.CommandPrefix, "");
            }

            DBEntry.Commands = string.Join(";", cmds);
            if (!GameServer.Database.SaveObject(DBEntry))
                return ModificationStatus.FailedToSave;

            return ModificationStatus.Success;
        }

        /// <summary>
        /// Removes a command to the Group's privileges and syncs to the database.
        /// </summary>
        /// <param name="commandString">Command to Remove.</param>
        public ModificationStatus RemoveCommand(string commandString)
        {
            Privileges.Remove(PrivilegeDefaults.CommandPrefix + commandString);

            string[] cmds = Privileges.Where(s => s.StartsWith(PrivilegeDefaults.CommandPrefix)).ToArray();

            for (int index = 0; index < cmds.Length; index++)
            {
                if (cmds[index].StartsWith(PrivilegeDefaults.CommandPrefix))
                    cmds[index] = cmds[index].Replace(PrivilegeDefaults.CommandPrefix, "");
            }

            DBEntry.Commands = string.Join(";", cmds);
            if (!GameServer.Database.SaveObject(DBEntry))
                return ModificationStatus.FailedToSave;

            return ModificationStatus.Success;
        }

        #endregion
        
        #region Groups

        /// <summary>
        /// Adds a group to this binding and syncs to database.
        /// </summary>
        /// <param name="grp">Group to add.</param>
        /// <returns></returns>
        public ModificationStatus AddGroup(PrivilegeGroup grp)
        {
            if (InheritedGroups.Contains(grp)) return ModificationStatus.AlreadyExists;
            if (HasInherited(grp.DBEntry.GroupIndex)) return ModificationStatus.Circular;

            InheritedGroups.Add(grp);

            DBEntry.InheritedGroups = string.Join(";", InheritedGroups.Select(g => g.DBEntry.Name));
            if (!GameServer.Database.SaveObject(DBEntry))
                return ModificationStatus.FailedToSave;

            return ModificationStatus.Success;
        }

        /// <summary>
        /// Removes a group from this binding and syncs to database.
        /// </summary>
        /// <param name="grp">Group to remove.</param>
        /// <returns></returns>
        public ModificationStatus RemoveGroup(PrivilegeGroup grp)
        {
            if (!InheritedGroups.Contains(grp)) return ModificationStatus.DoesNotExist;

            InheritedGroups.Remove(grp);

            DBEntry.InheritedGroups = string.Join(";", InheritedGroups.Select(g => g.DBEntry.Name));
            if (!GameServer.Database.SaveObject(DBEntry))
                return ModificationStatus.FailedToSave;

            return ModificationStatus.Success;
        }

        #endregion

        #endregion

        #region Overflow Protection

        /// <summary>
        /// Check if this group has a circular inheritance chain, ie does this group
        /// inherit from itself via a sub group or otherwise? Used to prevent it from
        /// causing stack overflows in case someone has done something stupid.
        /// </summary>
        /// <returns>Whether or not there's a circular inheritance chain.</returns>
        public bool HasCircularInheritanceChain()
        {
            return InheritedGroups.Any(sg => sg.HasInherited(DBEntry.GroupIndex) || 
                InheritedGroups.Any(ig => ig.HasInherited(DBEntry.GroupIndex)));
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
                InheritedGroups.Any(sg => sg.DBEntry.GroupIndex == groupID || 
                    sg.HasInherited(groupID));
        }

        #endregion
    }
}
