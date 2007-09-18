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
using DOL.Events;

namespace DOL.GS.Effects
{

    public class SelectiveBlindnessEffect : TimedEffect
    {
        private GamePlayer EffectOwner;

        public SelectiveBlindnessEffect()
            : base(RealmAbilities.SelectiveBlindnessAbility.DURATION)
        { }

        public override void Start(GameLiving target)
        {
            base.Start(target);
            if (target is GamePlayer)
            {
                EffectOwner = target as GamePlayer;
                foreach (GamePlayer p in target.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
                {
                    p.Out.SendSpellEffectAnimation(EffectOwner, EffectOwner, 7059, 0, false, 1);
                }
                GameEventMgr.AddHandler(EffectOwner, GamePlayerEvent.Quit, new DOLEventHandler(PlayerLeftWorld));
            }
        }
        public override void Stop()
        {
            if (EffectOwner != null)
            {
                EffectOwner.TempProperties.removeProperty("SelectiveBlindnessOwner");
                GameEventMgr.RemoveHandler(EffectOwner, GamePlayerEvent.Quit, new DOLEventHandler(PlayerLeftWorld));
            }

            base.Stop();
        }

        /// <summary>
        /// Called when a player leaves the game
        /// </summary>
        /// <param name="e">The event which was raised</param>
        /// <param name="sender">Sender of the event</param>
        /// <param name="args">EventArgs associated with the event</param>
        private static void PlayerLeftWorld(DOLEvent e, object sender, EventArgs args)
        {
            GamePlayer player = sender as GamePlayer;

            SelectiveBlindnessEffect SelectiveBlindness = (SelectiveBlindnessEffect)player.EffectList.GetOfType(typeof(SelectiveBlindnessEffect));
            if (SelectiveBlindness != null)
                SelectiveBlindness.Cancel(false);
        }

        public override string Name { get { return "Selective Blindness"; } }
        public override ushort Icon { get { return 7059; } }

        // Delve Info
        public override IList DelveInfo
        {
            get
            {
                ArrayList list = new ArrayList();
                list.Add("You can't attack an ennemy.");
                return list;
            }
        }
    }
}

