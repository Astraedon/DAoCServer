/*
 * Etaew: Originally based on the work I did for Fallen Realms and Dracis
 * 
 * TODO: Dragon needs to fly around its home zone
 * 
 */

using System;
using System.Collections;
using DOL.Database;
using DOL.Events;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.GS.Scripts;
using System.Threading;
using log4net;
using DOL.AI.Brain;

namespace DOL.GS.Scripts
{
	public class Dragon : GameNPC
	{
		public Dragon()
			: base()
		{
			SetOwnBrain(new DragonBrain());
		}
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		#region Variables/Properties

		GameLiving m_dragonTarget;

		public int stage = 0;

		//Glares Target
		public GameLiving DragonTarget
		{
			get
			{
				return m_dragonTarget;
			}
			set
			{
				m_dragonTarget = value;
			}
		}

		public override int MaxHealth
		{
			get
			{
				return base.MaxHealth * Level;
			}
		}

		public override double AttackDamage(InventoryItem weapon)
		{
			return base.AttackDamage(weapon) * 1.5;
		}

		/// <summary>
		/// Gets or sets the base maxspeed of this living
		/// </summary>
		public override int MaxSpeedBase
		{
			get
			{
				return 191 + (Level * 2);
			}
			set
			{
				m_maxSpeedBase = value;
			}
		}

		/// <summary>
		/// The Respawn Interval of this mob
		/// </summary>
		/// 
		public override int RespawnInterval
		{
			get
			{
				int highmod = Level + 50;
				int lowmod = Level / 3;
				int result = Util.Random(lowmod, highmod);
				return result * 60 * 1000;
			}
		}
		/// <summary>
		/// Melee Attack Range.
		/// </summary>
		public override int AttackRange
		{
			get
			{
				//Normal mob attacks have 200 ...
				return 400;
			}
			set { }
		}

		#endregion

		#region Combat

		public int DragonNukeandStun(RegionTimer timer)
		{
			//AOE Stun
			CastSpell(Dragon.Stun, SkillBase.GetSpellLine(GlobalSpellsLines.Mob_Spells));
			//AOE Nuke
			CastSpell(Dragon.Nuke,SkillBase.GetSpellLine(GlobalSpellsLines.Mob_Spells));
			return 0;
		}

		// What happens when the DragonNuke timer is filled
		public int DragonNuke(RegionTimer timer)
		{
			//AOE Nuke
			CastSpell(Dragon.Nuke, SkillBase.GetSpellLine(GlobalSpellsLines.Mob_Spells));
			return 0;
		}

		public void DragonGlare(object timer)
		{
			ActionDragonGlare(m_dragonTarget);
		}

		public override void Die(GameObject killer)
		{
			int count = 0;
			lock (this.XPGainers.SyncRoot)
			{
				foreach (System.Collections.DictionaryEntry de in this.XPGainers)
				{
					GameObject obj = (GameObject)de.Key;
					if (obj is GamePlayer)
					{
						GamePlayer player = obj as GamePlayer;
						player.KillsDragon++;
						count++;
					}
				}
			}

			string message = this.Name + " has been slain by a force of " + count + " warriors from the realm of " + GlobalConstants.RealmToName((eRealm)killer.Realm);
			NewsMgr.CreateNews(message, killer.Realm, eNewsType.PvE, true);
			ServerIRC.IRCBot.SendMessage(ServerIRC.CHANNEL, message);

			base.Die(killer);
			// dragon died message
			DragonBroadcast(Name + " sighs, \"I am defeated!\"");
			//Event dragons dont respawn
			if (RespawnInterval == -1)
			{
				Delete();
				DeleteFromDatabase();
			}
		}

		public override void TakeDamage(GameObject source, eDamageType damageType, int damageAmount, int criticalAmount)
		{
			base.TakeDamage(source, damageType, damageAmount, criticalAmount);
			if (ObjectState != eObjectState.Active) return;
			GameLiving t = (GameLiving)source;
			int range = WorldMgr.GetDistance(this, t);
			if (range >= 1500)
			{
				m_dragonTarget = t;
				PickAction();
			}
		}

		public override void EnemyHealed(GameLiving enemy, GameObject healSource, eHealthChangeType changeType, int healAmount)
		{
			base.EnemyHealed(enemy, healSource, changeType, healAmount);
			if (ObjectState != eObjectState.Active) return;
			if (healSource is GameLiving)
			{
				GameLiving t = (GameLiving)healSource;
				int range = WorldMgr.GetDistance(this, t);
				if (range >= ((StandardMobBrain)Brain).AggroRange)
				{
					m_dragonTarget = t;
					PickAction();
				}
			}
		}

		void PickAction()
		{
			if (Util.Random(1) < 1)
			{
				//Glare
				Timer timer = new Timer(new TimerCallback(DragonGlare), null, 30, 0);
			}
			else
			{
				//Throw
				ActionDragonThrow(m_dragonTarget);
			}
		}

		void DragonBroadcast(string message)
		{
			foreach (GamePlayer players in this.GetPlayersInRadius((ushort)(WorldMgr.VISIBILITY_DISTANCE + 1500)))
				players.Out.SendMessage(message, eChatType.CT_Broadcast, eChatLoc.CL_ChatWindow);
		}

		void ActionDragonGlare(GameLiving target)
		{
			// Let them know that they're about to die.
			DragonBroadcast(Name + " glares at " + target.Name);
			TargetObject = m_dragonTarget;
		}

		void ActionDragonThrow(GameLiving target)
		{
			if (!(target is GamePlayer)) return;
			if (Realm == 5) return; // I don't want event dragons causing XP deaths
			DragonBroadcast(Name + " throws " + m_dragonTarget.Name + " into the air!");
			m_dragonTarget.MoveTo(m_dragonTarget.CurrentRegionID,
				m_dragonTarget.X,
				m_dragonTarget.Y,
				m_dragonTarget.Z + 750,
				m_dragonTarget.Heading);
		}

		#endregion

		#region Spells
		protected static Spell m_glare;
		public static Spell Glare
		{
			get
			{
				if (m_glare == null)
				{
					DBSpell spell = new DBSpell();
					spell.AutoSave = false;
					spell.CastTime = 0;
					spell.ClientEffect = 360;
					spell.Description = "Glare";
					spell.Name = "Dragon Glare";
					spell.Range = 2500;
					spell.Damage = 500;
					spell.DamageType = (int)eDamageType.Heat;
					spell.SpellID = 6001;
					spell.Target = "Enemy";
					spell.Type = "DirectDamage";
					m_glare = new Spell(spell, 70);
					SkillBase.GetSpellList(GlobalSpellsLines.Mob_Spells).Add(m_glare);
				}
				return m_glare;
			}
		}

		protected static Spell m_nuke;
		public static Spell Nuke
		{
			get
			{
				if (m_nuke== null)
				{
					DBSpell spell = new DBSpell();
					spell.AutoSave = false;
					spell.CastTime = 0;
					spell.Uninterruptible = true;
					spell.ClientEffect = 2308;
					spell.Description = "Nuke";
					spell.Name = "Dragon Nuke";
					spell.Range = 700;
					spell.Radius = 700;
					spell.Damage = 500;
					spell.DamageType = (int)eDamageType.Heat;
					spell.SpellID = 6000;
					spell.Target = "Enemy";
					spell.Type = "DirectDamage";
					m_nuke = new Spell(spell, 70);
					SkillBase.GetSpellList(GlobalSpellsLines.Mob_Spells).Add(m_nuke);
				}
				return m_nuke;
			}
		}

		protected static Spell m_stun;
		public static Spell Stun
		{
			get
			{
				if (m_stun == null)
				{
					DBSpell spell = new DBSpell();
					spell.AutoSave = false;
					spell.CastTime = 0;
					spell.Uninterruptible = true;
					spell.ClientEffect = 4123;
					spell.Duration = 5;
					spell.Description = "Stun";
					spell.Name = "Dragon Stun";
					spell.Range = 700;
					spell.Radius = 700;
					spell.Damage = 500;
					spell.DamageType = 13;
					spell.SpellID = 6002;
					spell.Target = "Enemy";
					spell.Type = "Stun";
					spell.Message1 = "You cannot move!";
					spell.Message2 = "{0} cannot seem to move!";
					m_stun = new Spell(spell, 70);
					SkillBase.GetSpellList(GlobalSpellsLines.Mob_Spells).Add(m_stun);
				}
				return m_stun;
			}
		}
		#endregion
	}
}

namespace DOL.AI.Brain
{
	public class DragonBrain : StandardMobBrain
	{
		public DragonBrain()
			: base()
		{
			AggroLevel = 100;
			AggroRange = 1000;
			ThinkInterval = 2000;
		}
		public override void Think()
		{
			Dragon dragon = Body as Dragon;
			//If at full HP we reset stages
			if (dragon.HealthPercent == 100 && dragon.stage != 0)
				dragon.stage = 0;
			//Stage 1 < 50%
			else if (dragon.HealthPercent < 50 && dragon.HealthPercent > 25 && dragon.stage == 0)
			{
				dragon.DragonTarget = CalculateNextAttackTarget();
				if (dragon.DragonTarget != null)
				{
					new RegionTimer(dragon, new RegionTimerCallback(dragon.DragonNuke), 5000);
					dragon.stage = 1;
				}
			}
			//Stage 2 < 25%
			else if (dragon.HealthPercent < 25 && dragon.HealthPercent > 10 && dragon.stage == 1)
			{
				dragon.DragonTarget = CalculateNextAttackTarget();
				if (dragon.DragonTarget != null)
				{
					new RegionTimer(dragon, new RegionTimerCallback(dragon.DragonNukeandStun), 5000);
					dragon.stage = 2;
				}
			}
			//Stage 3 < 10%
			else if (dragon.HealthPercent < 10 && dragon.stage == 2)
			{
				dragon.DragonTarget = CalculateNextAttackTarget();
				if (dragon.DragonTarget != null)
				{
					new RegionTimer(dragon, new RegionTimerCallback(dragon.DragonNukeandStun), 5000);
					dragon.stage = 3;
				}
			}
			base.Think();
		}
	}
}
