using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOL.Movement
{
	/// <summary>
	/// This enumarator contains list of all available formations
	/// </summary>
	public enum eFormation
	{
		/// <summary>
		/// squared formation, Leader in first line, followed by melee 
		/// (tanks) in next line and on the edges, keeping ranged in the 
		/// middle/back
		/// </summary>
		Square,

		/// <summary>
		/// triangle formation, leader goes first, edges of triangle being
		/// occupied by melee, inside of the triangle containing ranged NPCs
		/// </summary>
		Triangle,

		/// <summary>
		/// two rows formation. Leader in the middle of first one, left wing
		/// and right wing with melee, second row wings with ranged
		/// </summary>
		Phalanx
	}

	/// <summary>
	/// This enumerator servers as a setting for each formation member
	/// Specific formations use this to determine position of GameLiving
	/// within the position
	/// </summary>
	public enum eFormationMemberRole
	{
		/// <summary>
		/// Leading NPC for the formation
		/// </summary>
		Leader,

		/// <summary>
		/// Tank like close range NPC in the formation
		/// </summary>
		Melee,

		/// <summary>
		/// Ranged class for the formation
		/// </summary>
		Ranged,

		/// <summary>
		/// NPC with this role won't be counted in a formation
		/// </summary>
		None
	}

	/// <summary>
	/// This class represents factory for creation of formations
	/// </summary>
	public class FactoryFormations
	{
		//private Formation
	}
}
