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
    [DataTable(TableName = "Privilege_Group")]
    public class DBPrivilegeGroup : DataObject
    {   
        private int m_groupIndex;
        private string m_name;
        private string m__displayName;
        private string m_commands;
        private string m_privileges;
        private string m_inheritedGroups;

        [DataElement(Index = true, Unique = true, AllowDbNull = false)]
        [PrimaryKey(AutoIncrement = true)]
        public int GroupIndex
        {
            get { return m_groupIndex; }
            set 
            { 
                Dirty = true;
                m_groupIndex = value;
            }
        }

        [DataElement]
        public string Name
        {
            get { return m_name; }
            set
            {
                Dirty = true;
                m_name = value;
            }
        }

        [DataElement]
        public string DisplayName
        {
            get { return m__displayName; }
            set
            {
                Dirty = true;
                m__displayName = value;
            }
        }

        [DataElement]
        public string Commands
        {
            get { return m_commands; }
            set
            {
                Dirty = true;
                m_commands = value;
            }
        }

        [DataElement]
        public string Privileges
        {
            get { return m_privileges; }
            set
            {
                Dirty = true;
                m_privileges = value;
            }
        }

        [DataElement]
        public string InheritedGroups
        {
            get { return m_inheritedGroups; }
            set
            {
                Dirty = true;
                m_inheritedGroups = value;
            }
        }
    }
}
