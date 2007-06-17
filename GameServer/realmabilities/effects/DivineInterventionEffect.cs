using System;
using System.Collections;
using DOL.GS.PacketHandler;
using DOL.GS.SkillHandler;
using DOL.Events;
using DOL.GS.Effects;
//using DOL.GS.GameEvents;

namespace DOL.GS.RealmAbilities
{
	/// <summary>
	/// 
	/// </summary>
	public class DivineInterventionEffect : TimedEffect, IGameEffect
	{
		public DivineInterventionEffect(int value)
			: base(DivineInterventionAbility.poolDuration)
		{
			poolValue = value;
		}

		Group m_group;
		public int poolValue = 0;
		ArrayList affected = new ArrayList();

		/// <summary>
		/// Start the effect on a living target
		/// </summary>
		/// <param name="living"></param>
		public override void Start(GameLiving living)
		{
			base.Start(living);
			if (living is GamePlayer)
				m_group = (living as GamePlayer).PlayerGroup;

			if (m_group == null)
				return;
			if (living is GamePlayer)
			{
				(living as GamePlayer).Out.SendMessage("Your group is protected by a pool of healing!", eChatType.CT_Spell, eChatLoc.CL_SystemWindow);
				GameEventMgr.AddHandler(m_group, PlayerGroupEvent.PlayerJoined, new DOLEventHandler(PlayerJoinedGroup));
				GameEventMgr.AddHandler(m_group, PlayerGroupEvent.PlayerDisbanded, new DOLEventHandler(PlayerDisbandedGroup));
			}

			foreach (GamePlayer g_player in m_group.GetPlayersInTheGroup())
			{
				foreach (GamePlayer t_player in living.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
				{
					t_player.Out.SendSpellEffectAnimation(living, g_player, 7036, 0, false, 1);
				}
				if (g_player == living) continue;
				g_player.Out.SendMessage("Your are protected by a pool of healing!", eChatType.CT_Spell, eChatLoc.CL_SystemWindow);
				affected.Add(g_player);
				GameEventMgr.AddHandler(g_player, GamePlayerEvent.TakeDamage, new DOLEventHandler(TakeDamage));
			}
		}
		protected void PlayerJoinedGroup(DOLEvent e, object sender, EventArgs args)
		{
			PlayerJoinedEventArgs pjargs = args as PlayerJoinedEventArgs;
			if (pjargs == null) return;
			affected.Add(pjargs.Player);
			GameEventMgr.AddHandler(pjargs.Player, GamePlayerEvent.TakeDamage, new DOLEventHandler(TakeDamage));
			pjargs.Player.Out.SendMessage("Your are protected by a pool of healing!", eChatType.CT_Spell, eChatLoc.CL_SystemWindow);
		}
		protected void PlayerDisbandedGroup(DOLEvent e, object sender, EventArgs args)
		{
			PlayerDisbandedEventArgs pdargs = args as PlayerDisbandedEventArgs;
			if (pdargs == null) return;
			affected.Remove(pdargs.Player);
			GameEventMgr.RemoveHandler(pdargs.Player, GamePlayerEvent.TakeDamage, new DOLEventHandler(TakeDamage));
			pdargs.Player.Out.SendMessage("Your are not longer protected by a pool of healing!", eChatType.CT_SpellExpires, eChatLoc.CL_SystemWindow);
			if (m_group == null)
				Cancel(false);
		}
		protected void TakeDamage(DOLEvent e, object sender, EventArgs args)
		{
			TakeDamageEventArgs targs = args as TakeDamageEventArgs;
			GamePlayer player = sender as GamePlayer;
			if (!WorldMgr.CheckDistance(player, m_owner, 2300))
				return;
			if (targs.DamageType == eDamageType.Falling) return;
			if (!player.IsAlive) return;
			if (player.HealthPercent >= 75) return;
			int dmgamount = player.MaxHealth - player.Health;
			if (dmgamount <= 0) return;
			int healamount = 0;
			if (poolValue <= 0)
				Cancel(false);
			if (!(player.IsAlive))
				return;
			if (poolValue - dmgamount > 0)
			{
				healamount = dmgamount;
			}
			else
			{
				healamount = poolValue;
			}
			foreach (GamePlayer t_player in player.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
			{
				t_player.Out.SendSpellEffectAnimation(m_owner, player, 8051, 0, false, 1);
			}
			player.Out.SendMessage("You are healed by the pool of healing for " + healamount + "!", eChatType.CT_Spell, eChatLoc.CL_SystemWindow);
			player.ChangeHealth(m_owner, GameLiving.eHealthChangeType.Spell, healamount);
			poolValue -= dmgamount;
			if (poolValue <= 0)
				Cancel(false);
		}

		public override void Stop()
		{
			base.Stop();
			if (m_group != null)
			{
				GameEventMgr.RemoveHandler(m_group, PlayerGroupEvent.PlayerDisbanded, new DOLEventHandler(PlayerDisbandedGroup));
				GameEventMgr.RemoveHandler(m_group, PlayerGroupEvent.PlayerJoined, new DOLEventHandler(PlayerJoinedGroup));
			}
			foreach (GamePlayer pl in affected)
			{
				pl.Out.SendMessage("Your are no longer protected by a pool of healing!", eChatType.CT_SpellExpires, eChatLoc.CL_SystemWindow);
				GameEventMgr.RemoveHandler(pl, GamePlayerEvent.TakeDamage, new DOLEventHandler(TakeDamage));
			}
			affected.Clear();
			m_group = null;
		}
		
		/// <summary>
		/// Name of the effect
		/// </summary>
		public override string Name { get { return "Divine Intervention"; } }

		/// <summary>
		/// Icon to show on players, can be id
		/// </summary>
		public override ushort Icon { get { return 3035; } }
		
		/// <summary>
		/// Delve Info
		/// </summary>
		public override IList DelveInfo
		{
			get
			{
				IList delveInfoList = new ArrayList(10);
				delveInfoList.Add("This ability creates a pool of healing on the user, instantly healing any member of the caster's group when they go below 75% hp, until it is used up.");
				delveInfoList.Add(" ");
				delveInfoList.Add("Target: Group");
				delveInfoList.Add("Duration: 15:00 min");
				delveInfoList.Add(" ");
				delveInfoList.Add("Value: " + poolValue);
				int seconds = (int)(RemainingTime / 1000);
				if (seconds > 0)
				{
					delveInfoList.Add(" ");
					if (seconds > 60)
						delveInfoList.Add("- " + seconds / 60 + ":" + (seconds % 60).ToString("00") + " minutes remaining.");
					else
						delveInfoList.Add("- " + seconds + " seconds remaining.");
				}
				return delveInfoList;
			}
		}
	}
}
