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

            foreach (string privilegeGroup in entry.InheritedGroups.Split(';'))
            {
                ushort key;
                PrivilegeGroup toAdd = ushort.TryParse(privilegeGroup, out key) ? 
                    PrivilegeManager.GetGroupFromID(key) : PrivilegeManager.GetGroupFromName(privilegeGroup);

                if(toAdd != null)
                    InheritedGroups.Add(toAdd);
            }

            Privileges.AddRange(DBEntry.Privileges.Split(';'));

            foreach (string str in DBEntry.Commands.Split(';'))
                Privileges.Add("cmd_" + str);
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
    }
}
