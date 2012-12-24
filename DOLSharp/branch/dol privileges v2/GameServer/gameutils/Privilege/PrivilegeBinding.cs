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

            foreach (string privilegeGroup in DBEntry.Groups.Split(';'))
            {
                ushort key;
                PrivilegeGroup toAdd = ushort.TryParse(privilegeGroup, out key) ?
                    PrivilegeManager.GetGroupFromID(key) : PrivilegeManager.GetGroupFromName(privilegeGroup);

                if (toAdd != null)
                    Groups.Add(toAdd);
            }

            AdditionalPrivileges.AddRange(DBEntry.AdditionalPrivileges.Split(';'));

            foreach (string str in DBEntry.AdditionalCommands.Split(';'))
                AdditionalPrivileges.Add("cmd_" + str);
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
