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

using DOL.Database.Attributes;

namespace DOL.Database
{
    [DataTable(TableName = "Privilege_Binding")]
    public class DBPrivilegeBinding : DataObject
    {
        private string m_identifier;
        private string m_groups;
        private string m_additionalCommands;
        private string m_additionalPrivileges;

        /// <summary>
        /// Sets/Gets Player ID or Account Name
        /// </summary>
        [ReadOnly]
        [DataElement(Index = true, AllowDbNull = false, Unique = true)]
        [PrimaryKey]
        public string Identifier
        {
            get
            {
                return m_identifier;
            }
            set
            {
                Dirty = true;
                m_identifier = value;
            }
        }

        [DataElement]
        public string Groups
        {
            get { return m_groups; }
            set
            {
                Dirty = true;

                m_groups = value;
            }
        }

        [DataElement]
        public string AdditionalCommands
        {
            get
            {
                return m_additionalCommands;
            }
            set
            {
                Dirty = true;
                m_additionalCommands = value;
            }
        }

        [DataElement]
        public string AdditionalPrivileges
        {
            get
            {
                return m_additionalPrivileges;
            }
            set
            {
                Dirty = true;
                m_additionalPrivileges = value;
            }
        }
    }
}
