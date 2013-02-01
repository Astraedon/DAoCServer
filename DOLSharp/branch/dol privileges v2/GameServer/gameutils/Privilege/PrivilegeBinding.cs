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
using System.Text.RegularExpressions;
using DOL.Database;

namespace DOL.GS.Privilege
{
    /// <summary>
    /// Binding that holds Privileges in memory cached from the database for either an account or user.
    /// </summary>
    public class PrivilegeBinding : PrivilegeContainer
    {
        public PrivilegeBinding(DBPrivilegeBinding entry) : base(entry)
        {
            if (!string.IsNullOrEmpty(DBEntry.Groups))
            {
                foreach (string privilegeGroup in DBEntry.Groups.Split(';'))
                {
                    PrivilegeGroup toAdd = PrivilegeManager.GetGroup(privilegeGroup);

                    if (toAdd != null)
                        Groups.Add(toAdd);
                }
            }

            if (!string.IsNullOrEmpty(DBEntry.AdditionalPrivileges))
            {
                string[] splitPrivileges = DBEntry.AdditionalPrivileges.Split(';').Where(s => !string.IsNullOrEmpty(s)).ToArray();

                Privileges.AddRange(splitPrivileges.Where(s => !PrivilegeDefaults.ParameterizedRegex.IsMatch(s)).ToList());

                foreach (string currentPrivilege in splitPrivileges.Where(s => PrivilegeDefaults.ParameterizedRegex.IsMatch(s)).ToList())
                {
                    Match m = PrivilegeDefaults.ParameterizedRegex.Match(currentPrivilege);

                    ParameterizedPrivileges.Add(m.Groups[1].Value, m.Groups[2].Value.Split('|'));
                    Privileges.Add(m.Groups[1].Value);
                }
            }
                

            if (!string.IsNullOrEmpty(DBEntry.AdditionalCommands))
            {
                foreach (string str in DBEntry.AdditionalCommands.Split(';'))
                {
                    if (str != "")
                        Privileges.Add(PrivilegeDefaults.CommandPrefix + str);
                }
            }
        }

        #region Accessors

        /// <summary>
        /// Database entry backing up this PrivilegeBinding.
        /// </summary>
        public DBPrivilegeBinding DBEntry
        {
            get { return DBEntity as DBPrivilegeBinding; }
        }

        protected override string DBPrivileges
        {
            get { return DBEntry.AdditionalPrivileges; }
            set { DBEntry.AdditionalPrivileges = value; }
        }

        protected override string DBCommands
        {
            get { return DBEntry.AdditionalCommands; }
            set { DBEntry.AdditionalCommands = value; }
        }

        protected override string DBGroups
        {
            get { return DBEntry.Groups; }
            set { DBEntry.Groups = value; }
        }

        #endregion
    }
}
