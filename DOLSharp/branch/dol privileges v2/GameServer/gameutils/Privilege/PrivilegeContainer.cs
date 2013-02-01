using System.Collections.Generic;
using System.Linq;
using DOL.Database;

namespace DOL.GS.Privilege
{
    public abstract class PrivilegeContainer
    {
        protected readonly DataObject DBEntity;

        private readonly IList<PrivilegeGroup> m_groups = new List<PrivilegeGroup>();
        private readonly IList<string> m_privileges = new List<string>();
        private readonly IDictionary<string, IList<string>> m_parameterizedPrivileges = new Dictionary<string, IList<string>>(); 


        protected PrivilegeContainer(DataObject databaseEntry)
        {
            DBEntity = databaseEntry;
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
        public IDictionary<string, IList<string>> ParameterizedPrivileges
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
            Privileges.Add(privilegeKey);

            DBPrivileges = string.Join(";", Privileges.Where(s => !s.StartsWith(PrivilegeDefaults.CommandPrefix)));
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
            Privileges.Remove(privilegeKey);

            DBPrivileges = string.Join(";", Privileges.Where(s => !s.StartsWith(PrivilegeDefaults.CommandPrefix)));
            if (!SaveEntry())
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
    }
}
