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
        private int _groupIndex;
        private string _name;
        private string _displayName;
        private string _commands;
        private string _privileges;
        private string _inheritedGroups;

        [ReadOnly]
        [DataElement(Index = true)]
        [PrimaryKey(AutoIncrement = true)]
        public int GroupIndex
        {
            get { return _groupIndex; }
            set 
            { 
                Dirty = true;
                _groupIndex = value;
            }
        }

        [DataElement]
        public string Name
        {
            get { return _name; }
            set
            {
                Dirty = true;
                _name = value;
            }
        }

        [DataElement]
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                Dirty = true;
                _displayName = value;
            }
        }

        [DataElement]
        public string Commands
        {
            get { return _commands; }
            set
            {
                Dirty = true;
                _commands = value;
            }
        }

        [DataElement]
        public string Privileges
        {
            get { return _privileges; }
            set
            {
                Dirty = true;
                _privileges = value;
            }
        }

        [DataElement]
        public string InheritedGroups
        {
            get { return _inheritedGroups; }
            set
            {
                Dirty = true;
                _inheritedGroups = value;
            }
        }
    }
}
