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

using DOL.Database;
using DOL.Database.Attributes;

namespace DOL.Database
{
	/// <summary>
	/// Objects as Tables, Lights, Bags non static in Game.
	/// </summary>
	[DataTable(TableName="WorldObject")]
	public class WorldObject : DataObject
	{
		private string m_internalID;
		private string	m_type;
		private string	m_name;
		private int		m_x;
		private int		m_y;
		private int		m_z;
		private ushort	m_heading;
		private ushort	m_model;
		private ushort	m_region;
		private ushort	m_zone;
		private int		m_emblem;
		private string	m_guild;


		static bool			m_autoSave;

		public WorldObject()
		{
			m_autoSave=false;
			m_type = "DOL.GS.GameItem";
			m_guild = "";
			m_internalID = "";
		}

		override public bool AutoSave
		{
			get
			{
				return m_autoSave;
			}
			set
			{
				m_autoSave = value;
			}
		}
		/// <summary>
		/// Internal index of Object
		/// </summary>
		[DataElement(AllowDbNull = true)]
		public string InternalID
		{
			get
			{
				return m_internalID;
			}
			set
			{
				Dirty = true;
				m_internalID = value;
			}
		}
		[DataElement(AllowDbNull = false)]
		public string Guild
		{
			get
			{
				return m_guild;
			}
			set
			{
				Dirty = true;
				m_guild = value;
			}
		}

		[DataElement(AllowDbNull = true)]
		public string ClassType
		{
			get
			{
				return m_type;
			}
			set
			{
				Dirty = true;
				m_type = value;
			}
		}

		[DataElement(AllowDbNull=false)]
		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				Dirty = true;
				m_name = value;
			}
		}
				
		[DataElement(AllowDbNull=false)]
		public int X
		{
			get
			{
				return m_x;
			}
			set
			{   
				Dirty = true;
				m_x = value;
			}
		}
		
		[DataElement(AllowDbNull=false)]
		public int Y
		{
			get
			{
				return m_y;
			}
			set
			{   
				Dirty = true;
				m_y = value;
			}
		}

		[DataElement(AllowDbNull=false)]
		public int Z
		{
			get
			{
				return m_z;
			}
			set
			{   
				Dirty = true;
				m_z = value;
			}
		}
		
		[DataElement(AllowDbNull=false)]
		public ushort Heading
		{
			get
			{
				return m_heading;
			}
			set
			{   
				Dirty = true;
				m_heading = value;
			}
		}

		[DataElement(AllowDbNull=false, Index=true)]
		public ushort Region
		{
			get
			{
				return m_region;
			}
			set
			{   
				Dirty = true;
				m_region = value;
			}
		}
		[DataElement(AllowDbNull = false, Index = true)]
		public ushort Zone
		{
			get
			{
				return m_zone;
			}
			set
			{
				Dirty = true;
				m_zone = value;
			}
		}
		
		[DataElement(AllowDbNull=false)]
		public ushort Model
		{
			get
			{
				return m_model;
			}
			set
			{   
				Dirty = true;
				m_model = value;
			}
		}
		[DataElement(AllowDbNull=false)]
		public int Emblem
		{
			get
			{
				return m_emblem;
			}
			set
			{   
				Dirty = true;
				m_emblem = value;
			}
		}
		
	}
}
