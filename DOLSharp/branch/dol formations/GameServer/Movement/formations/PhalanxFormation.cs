using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOL.GS;

namespace DOL.Movement.formations
{
	public class PhalanxFormation : Formation
	{
		protected PhalanxFormation(List<GameNPC> formationMembers, float spacing)
			: base(formationMembers, spacing)
		{

		}

		override protected void calculateShaping()
		{
			List<GameNPC> leader = getMembersByRole(eFormationMemberRole.Leader);
			List<GameNPC> melee = getMembersByRole(eFormationMemberRole.Melee);
			List<GameNPC> ranged = getMembersByRole(eFormationMemberRole.Ranged);

			int totalLines = 0;
			if ((leader.Count > 0) || (melee.Count > 0)) totalLines++;
			if (ranged.Count > 0) totalLines++;

			float zStep = this.m_spacing / totalLines;
			float halfStep = this.m_spacing / 2;

			int firstLineCount = leader.Count() + melee.Count();
			if (firstLineCount > 0)
			{
				List<GameNPC> firstLine = new List<GameNPC>();
				firstLine.AddRange(melee);
				firstLine.InsertRange(melee.Count() / 2, leader);
				foreach (GameNPC npc in firstLine)
				{
					//TODO: CONTINUE HERE
					//npc.FormationPosition = new Point3D(
				}

			}
		}
	}
}
