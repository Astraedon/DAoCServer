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
using System.Collections;

namespace DOL.GS.Effects
{
	/// <summary>
	/// Player shade effect.
	/// </summary>
	public class ShadeEffect : StaticEffect, IGameEffect
	{
		/// <summary>
		/// The effect owner
		/// </summary>
		GamePlayer m_player;

		/// <summary>
		/// Creates a new shade effect.
		/// </summary>
		public ShadeEffect() { }

		/// <summary>
		/// Start the shade effect on a player.
		/// </summary>
		/// <param name="living">The effect target</param>
		public override void Start(GameLiving living)
		{
			GamePlayer player = living as GamePlayer;
			if (player != null)
			{
				m_player = player;
				player.EffectList.Add(this);
			}
		}

		/// <summary>
		/// Stop the effect.
		/// </summary>
		public override void Stop()
		{
			m_player.EffectList.Remove(this);
		}

		/// <summary>
		/// Cancel the effect.
		/// </summary>
		public override void Cancel(bool playerCancel) 
		{
			m_player.Shade(false);
		}

		/// <summary>
		/// Name of the effect.
		/// </summary>
		public override string Name { get { return "Shade"; } }	


		/// <summary>
		/// Icon to show on players, can be id
		/// </summary>
		public override ushort Icon { get { return 0x193; } }
		
		/// <summary>
		/// Delve Info
		/// </summary>
		public override IList DelveInfo { get { return new ArrayList(0); } }
	}
}
