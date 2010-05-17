using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using DOL.GS;

namespace DOL.Movement
{
	/// <summary>
	/// This abstract class represents fundamental system for all formation types
	/// </summary>
	public abstract class Formation
	{
		#region Formation properties
		/// <summary>
		/// protected list of all members of a formation
		/// </summary>
		protected List<GameNPC> m_formationMembers = new List<GameNPC>();

		/// <summary>
		/// represents space between two formation members
		/// </summary>
		protected float m_spacing;

		#endregion

		#region get/set

		public IList<GameNPC> FormationMembers
		{
			get { return new ReadOnlyCollection<GameNPC>(m_formationMembers); }
			set
			{
				this.m_formationMembers = (List<GameNPC>)value;
				calculateShaping();
			}
		}

		public float Spacing
		{
			get { return this.m_spacing; }
			set
			{
				this.m_spacing = value;
				calculateShaping();
			}
		}
		#endregion

		#region constructors
		/// <summary>
		/// Basic constructor of the formation
		/// </summary>
		/// <param name="formationMembers">List of all formation members</param>
		protected Formation(List<GameNPC> formationMembers, float spacing)
		{
			this.m_formationMembers = formationMembers;
			this.m_spacing = spacing;
			calculateShaping();
		}

		#endregion

		#region shaping

		/// <summary>
		/// This method updates internal position of each member within a squad
		/// </summary>
		protected abstract void calculateShaping();

		#endregion

		#region getRoles
		protected List<GameNPC> getMembersByRole(eFormationMemberRole role)
		{
			List<GameNPC> roleList = new List<GameNPC>();
			foreach (GameNPC npc in m_formationMembers)
				if (npc.FormationRole == role)
					roleList.Add(npc);

			return roleList;
		}

		#endregion
	}
}
