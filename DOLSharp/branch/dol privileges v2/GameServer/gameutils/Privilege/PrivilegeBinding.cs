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
    public class PrivilegeBinding
    {
        private readonly DBPrivilegeBinding m_dbEntry;

        private readonly IList<PrivilegeGroup> m_groups = new List<PrivilegeGroup>();
        private readonly IList<string> m_additionalPrivileges = new List<string>(); 

        public PrivilegeBinding(DBPrivilegeBinding entry)
        {
            m_dbEntry = entry;

            if (!string.IsNullOrEmpty(DBEntry.Groups))
            {
                foreach (string privilegeGroup in DBEntry.Groups.Split(';'))
                {
                    int key;
                    PrivilegeGroup toAdd = int.TryParse(privilegeGroup, out key) ?
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
                        AdditionalPrivileges.Add(DefaultPrivileges.CommandPrefix + str);
                }
            }
        }

        #region Accessors

        /// <summary>
        /// Database entry backing up this PrivilegeBinding.
        /// </summary>
        public DBPrivilegeBinding DBEntry
        {
            get { return m_dbEntry; }
        }

        /// <summary>
        /// Additional privileges specified to this binding on top of any in the groups.
        /// </summary>
        public IList<string> AdditionalPrivileges
        {
            get { return m_additionalPrivileges; }
        }

        /// <summary>
        /// Groups that this binding belongs to.
        /// </summary>
        public IList<PrivilegeGroup> Groups
        {
            get { return m_groups; }
        }

        #endregion

        #region Add / Remove

        #region Privileges

        /// <summary>
        /// Adds a privilege to the Group's privileges and syncs to the database.
        /// </summary>
        /// <param name="privilegeKey">Privilege to add.</param>
        public ModificationStatus AddPrivilege(string privilegeKey)
        {
            if(AdditionalPrivileges.Contains(privilegeKey)) return ModificationStatus.AlreadyExists;

            AdditionalPrivileges.Add(privilegeKey);

            DBEntry.AdditionalPrivileges = string.Join(";", AdditionalPrivileges.Where(s => !s.StartsWith(DefaultPrivileges.CommandPrefix)));
            
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
            if (!AdditionalPrivileges.Contains(privilegeKey)) return ModificationStatus.DoesNotExist;

            AdditionalPrivileges.Remove(privilegeKey);

            DBEntry.AdditionalPrivileges = string.Join(";", AdditionalPrivileges.Where(s => !s.StartsWith(DefaultPrivileges.CommandPrefix)));
           
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
            if (AdditionalPrivileges.Contains(DefaultPrivileges.CommandPrefix + commandString)) 
                return ModificationStatus.AlreadyExists;

            AdditionalPrivileges.Add(DefaultPrivileges.CommandPrefix + commandString);

            string[] cmds = AdditionalPrivileges.Where(s => s.StartsWith(DefaultPrivileges.CommandPrefix)).ToArray();

            for (int index = 0; index < cmds.Length; index++)
            {
                if (cmds[index].StartsWith(DefaultPrivileges.CommandPrefix))
                    cmds[index] = cmds[index].Replace(DefaultPrivileges.CommandPrefix, "");
            }

            DBEntry.AdditionalCommands = string.Join(";", cmds);
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
            if (!AdditionalPrivileges.Contains(DefaultPrivileges.CommandPrefix + commandString))
                return ModificationStatus.DoesNotExist;

            AdditionalPrivileges.Remove(DefaultPrivileges.CommandPrefix + commandString);

            string[] cmds = AdditionalPrivileges.Where(s => s.StartsWith(DefaultPrivileges.CommandPrefix)).ToArray();

            for (int index = 0; index < cmds.Length; index++)
            {
                if (cmds[index].StartsWith(DefaultPrivileges.CommandPrefix))
                    cmds[index] = cmds[index].Replace(DefaultPrivileges.CommandPrefix, "");
            }

            DBEntry.AdditionalCommands = string.Join(";", cmds);
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
            if (Groups.Contains(grp)) return ModificationStatus.AlreadyExists;

            Groups.Add(grp);

            DBEntry.Groups = string.Join(";", Groups.Select(g => g.DBEntry.Name));
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
            if(!Groups.Contains(grp)) return ModificationStatus.DoesNotExist;

            Groups.Remove(grp);

            DBEntry.Groups = string.Join(";", Groups.Select(g => g.DBEntry.Name));
            if (!GameServer.Database.SaveObject(DBEntry))
                return ModificationStatus.FailedToSave;

            return ModificationStatus.Success;
        }

        /// <summary>
        /// Removes several groups from this binding and syncs to database
        /// </summary>
        /// <param name="grps"></param>
        /// <returns></returns>
        public bool RemoveGroups(IEnumerable<PrivilegeGroup> grps)
        {
            return grps.Aggregate(true, (current, privilegeGroup) => current & RemoveGroup(privilegeGroup) == ModificationStatus.Success);
        }

        #endregion

        #endregion

        /// <summary>
        /// Check if this binding contains the rights to the specified privilege key.
        /// 
        /// Search Order is 
        /// Wildcard > Additional Privileges > Group Checks.
        /// </summary>
        /// <param name="privilegeKey">Key to search on.</param>
        /// <returns>Allowed?</returns>
        public bool HasPrivilege(string privilegeKey)
        {
            return AdditionalPrivileges.Any(s => s == DefaultPrivileges.Wildcard || s == privilegeKey) || Groups.Any(g => g.HasPrivilege(privilegeKey));
        }
    }
}
