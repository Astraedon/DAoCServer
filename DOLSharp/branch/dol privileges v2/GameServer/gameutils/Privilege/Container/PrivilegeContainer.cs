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
using System.Text.RegularExpressions;
using DOL.Database;
using DOL.GS;
using DOL.GS.Privilege;
using DOL.GS.Privilege.ParameterizedBindings;

namespace DOL.gameutils.Privilege.Container
{
    public abstract class PrivilegeContainer
    {
        protected DataObject DBEntity;

        private readonly IList<PrivilegeGroup> m_groups = new List<PrivilegeGroup>();
        private readonly IList<string> m_privileges = new List<string>();
        private readonly IDictionary<string, ParameterizedPrivilegeBinding> m_parameterizedPrivileges = 
                new Dictionary<string, ParameterizedPrivilegeBinding>(StringComparer.OrdinalIgnoreCase); 


        protected PrivilegeContainer(DataObject databaseEntry)
        {
            DBEntity = databaseEntry;
        }

        /// <summary>
        /// Initializes the privileges by pulling straight from the supplied string
        /// Optionally will clear the existing privileges from the cache that aren't
        /// related to the commands.
        /// </summary>
        /// <param name="fromString">Semicolon delimited string of privileges.</param>
        /// <param name="clearExisting">Clear existing privileges that are not command related?</param>
        public void InitializePrivileges(string fromString, bool clearExisting)
        {
            if (clearExisting)
            {
                foreach (string currentPriv in Privileges.Where(s => !s.StartsWith(PrivilegeDefaults.CommandPrefix)).ToList())
                {
                    Privileges.Remove(currentPriv);
                }
            }

            InitializePrivileges(fromString);
        }

        /// <summary>
        /// Initializes the privileges by pulling straight from the supplied string.
        /// </summary>
        /// <param name="fromString">Semicolon delimited string of privileges.</param>
        protected void InitializePrivileges(string fromString)
        {
            if (!string.IsNullOrEmpty(fromString))
            {
                string[] splitPrivileges = fromString.Split(';').Where(s => !string.IsNullOrEmpty(s)).ToArray();

                Privileges.AddRange(splitPrivileges.Where(s => !PrivilegeDefaults.ParameterizedRegex.IsMatch(s)).ToList());

                foreach (string currentPrivilege in splitPrivileges.Where(s => PrivilegeDefaults.ParameterizedRegex.IsMatch(s)).ToList())
                {
                    Match m = PrivilegeDefaults.ParameterizedRegex.Match(currentPrivilege);

                    ParameterizedPrivilegeBinding binding = PrivilegeManager.GetParameterizedPrivilege(m.Groups[1].Value, m.Groups[2].Value.Split('|'));

                    if (binding != null)
                    {
                        ParameterizedPrivileges.Add(m.Groups[1].Value, binding);
                        Privileges.Add(m.Groups[1].Value);
                    }
                    else
                    {
                        PrivilegeManager.Log.Error(String.Format
                            ("[Privilege Container] Error Initializing Parameterized Privilege {0} dropping privilege from container.", m.Groups[1].Value));
                    }
                    
                }
            }
        }


        /// <summary>
        /// Initializes the privileges by pulling straight from the supplied string
        /// Optionally will clear the existing privileges from the cache that aren't
        /// related to the commands.
        /// </summary>
        /// <param name="fromString">Semicolon delimited string of privileges.</param>
        /// <param name="clearExisting">Clear existing privileges that are not command related?</param>
        public void InitializeCommands(string fromString, bool clearExisting)
        {
            if (clearExisting)
            {
                foreach (string currentPriv in Privileges.Where(s => s.StartsWith(PrivilegeDefaults.CommandPrefix)).ToList())
                {
                    Privileges.Remove(currentPriv);
                }
            }

            InitializeCommands(fromString);
        }

        /// <summary>
        /// Initializes the commands by pulling straight from the supplied string.
        /// </summary>
        /// <param name="fromString">Semicolon delimited string of commands.</param>
        protected void InitializeCommands(string fromString)
        {
            if (!string.IsNullOrEmpty(fromString))
            {
                foreach (string str in fromString.Split(';'))
                {
                    if (str != "")
                        Privileges.Add(PrivilegeDefaults.CommandPrefix + str);
                }
            }
        }


        protected virtual string DBPrivileges { get; set; }
        protected virtual string DBCommands { get; set; }
        protected virtual string DBGroups { get; set; }

        /// <summary>
        /// Additional privileges specified to this binding on top of any in the groups.
        /// </summary>
        public IList<string> Privileges
        {
            get { return m_privileges; }
        }

        /// <summary>
        /// Dictionary of Parameterized Privileges.
        /// </summary>
        public IDictionary<string, ParameterizedPrivilegeBinding> ParameterizedPrivileges
        {
            get { return m_parameterizedPrivileges; }
        }

        /// <summary>
        /// Groups that this binding belongs to.
        /// </summary>
        public IList<PrivilegeGroup> Groups
        {
            get { return m_groups; }
        }

        /// <summary>
        /// Saves the underlying DB object.
        /// </summary>
        /// <returns>Success if its failed or not.</returns>
        private bool SaveEntry()
        {
            return GameServer.Database.SaveObject(DBEntity);
        }

        #region Add / Remove

        #region Privileges

        /// <summary>
        /// Adds a privilege to the Group's privileges and syncs to the database.
        /// </summary>
        /// <param name="privilegeKey">Privilege to add.</param>
        public ModificationStatus AddPrivilege(string privilegeKey)
        {
            bool isParameterized = privilegeKey.Contains("=");

            string pName = null;
            string[] pParams = null;

            if (isParameterized)
            {
                pName = privilegeKey.Split('=')[0];
                pParams = privilegeKey.Split('=')[1].Split(',');
            }

            if(isParameterized && HasParameterizedBinding(pName)) return ModificationStatus.AlreadyExists; 
            if (HasPrivilege(privilegeKey)) return ModificationStatus.AlreadyExists;

            // TODO: Add a sort of pre-processor for the privilege key to allow you to add a param'ed privilege to someone.

            if (!isParameterized)
                Privileges.Add(privilegeKey);
            else
            {
                ParameterizedPrivilegeBinding ppb = PrivilegeManager.GetParameterizedPrivilege(pName, pParams);

                if(ppb != null)
                    ParameterizedPrivileges.Add(pName, ppb);
                else return ModificationStatus.InvalidArguments;
            }

            DBPrivileges = FormatPrivileges();
            if (!SaveEntry())
                return ModificationStatus.FailedToSave;

            return ModificationStatus.Success;
        }

        /// <summary>
        /// Removes a privilege to the Group's privileges and syncs to the database.
        /// </summary>
        /// <param name="privilegeKey">Privilege to remove.</param>
        public ModificationStatus RemovePrivilege(string privilegeKey)
        {
            if (!HasPrivilege(privilegeKey) && !ParameterizedPrivileges.ContainsKey(privilegeKey)) return ModificationStatus.DoesNotExist;

            Privileges.Remove(privilegeKey);

            if (ParameterizedPrivileges.ContainsKey(privilegeKey)) ParameterizedPrivileges.Remove(privilegeKey);

            DBPrivileges = FormatPrivileges();
            if (!SaveEntry())
                return ModificationStatus.FailedToSave;

            return ModificationStatus.Success;
        }

        private string FormatPrivileges()
        {
            IList<string> rawPrivileges = new List<string>();
            rawPrivileges.AddRange(Privileges.Where(s => !s.StartsWith(PrivilegeDefaults.CommandPrefix)).ToList());
            rawPrivileges.AddRange(ParameterizedPrivileges.Select(pp => string.Format("{0}({1})", pp.Key, string.Join("|", pp.Value.RawArguments))).ToList());

            return string.Join(";", rawPrivileges);
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

            DBCommands = string.Join(";", cmds);
            if (!SaveEntry())
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

            DBCommands = string.Join(";", cmds);
            if (!SaveEntry())
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
        public virtual ModificationStatus AddGroup(PrivilegeGroup grp)
        {
            if (Groups.Contains(grp)) return ModificationStatus.AlreadyExists;

            Groups.Add(grp);

            DBGroups = string.Join(";", Groups.Select(g => g.DBEntry.Name));
            if (!SaveEntry())
                return ModificationStatus.FailedToSave;

            return ModificationStatus.Success;
        }

        /// <summary>
        /// Removes a group from this binding and syncs to database.
        /// </summary>
        /// <param name="grp">Group to remove.</param>
        /// <returns></returns>
        public virtual ModificationStatus RemoveGroup(PrivilegeGroup grp)
        {
            if (!Groups.Contains(grp)) return ModificationStatus.DoesNotExist;

            Groups.Remove(grp);

            DBGroups = string.Join(";", Groups.Select(g => g.DBEntry.Name));
            if (!SaveEntry())
                return ModificationStatus.FailedToSave;

            return ModificationStatus.Success;
        }

        /// <summary>
        /// Removes several groups from this binding and syncs to database
        /// </summary>
        /// <param name="grps"></param>
        /// <returns></returns>
        public virtual bool RemoveGroups(IEnumerable<PrivilegeGroup> grps)
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
            return Privileges.Any(s => s == PrivilegeDefaults.Wildcard || s == privilegeKey) || Groups.Any(g => g.HasPrivilege(privilegeKey));
        }

        /// <summary>
        /// Retrieves a parameter-bound privilege's associated object if it exists, or just null.
        /// </summary>g
        /// <typeparam name="T">Type of the parameter bound privilege object</typeparam>
        /// <param name="privilege">Key found under</param>
        /// <returns></returns>
        public T GetParameterBinding<T>(string privilege) where T : ParameterizedPrivilegeBinding
        {
            if (!HasParameterizedBinding(privilege)) return null;

            T retVal = null;

            if (ParameterizedPrivileges.ContainsKey(privilege) && ParameterizedPrivileges[privilege] is T)
                retVal = (ParameterizedPrivileges[privilege] as T);
            else
            {
                foreach (T temp in Groups.Select(binding => binding.GetParameterBinding<T>(privilege)).Where(temp => temp != null))
                {
                    retVal = temp;
                    break;
                }
            }

            return retVal;
        }

        public bool HasParameterizedBinding(string name)
        {
            return ParameterizedPrivileges.ContainsKey(name) || Groups.Any(g => g.HasParameterizedBinding(name));
        }
    }
}
