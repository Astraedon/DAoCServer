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
using System.Reflection;
using System.Collections;
using System.Threading;
using DOL.AI;
using DOL.AI.Brain;
using DOL.Database;
using DOL.Events;
using DOL.GS.Movement;
using DOL.GS.Quests;
using DOL.GS.PacketHandler;
using DOL.GS.Spells;
using DOL.GS.Utils;
using log4net;

namespace DOL.GS
{
	/// <summary>
	/// This class is the baseclass for all Non Player Characters like
	/// Monsters, Merchants, Guards, Steeds ...
	/// </summary>
	public abstract class GameNPC : GameLiving
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		#region Sizes/Properties
		/// <summary>
		/// Holds the size of the NPC
		/// </summary>
		protected byte m_size;
		/// <summary>
		/// Gets or sets the size of the npc
		/// </summary>
		public byte Size
		{
			get { return m_size; }
			set
			{
				m_size = value;
				if (ObjectState == eObjectState.Active)
				{
					foreach (GamePlayer player in GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
					{
						player.Out.SendNPCCreate(this);
						if (m_inventory != null)
							player.Out.SendLivingEquipmentUpdate(this);
					}
					BroadcastUpdate();
				}
			}
		}

		/// <summary>
		/// Gets or sets the model of this npc
		/// </summary>
		public override ushort Model
		{
			get { return base.Model; }
			set
			{
				base.Model = value;
				if (ObjectState == eObjectState.Active)
				{
					foreach (GamePlayer player in GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
						player.Out.SendModelChange(this, Model);
				}
			}
		}

		/// <summary>
		/// Gets or sets the heading of this NPC
		/// </summary>
		public override ushort Heading
		{
			get { return base.Heading; }
			set
			{
				if (IsTurningDisabled)
					return;
				base.Heading = value;
				BroadcastUpdate();
			}
		}

		/// <summary>
		/// Gets or sets the level of this NPC
		/// </summary>
		public override byte Level
		{
			get { return base.Level; }
			set
			{
				base.Level = value;
				//MaxHealth = (ushort)(value * 20 + 20);	// MaxHealth depends from mob level
				m_health = MaxHealth;
				if (ObjectState == eObjectState.Active)
				{
					foreach (GamePlayer player in GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
					{
						player.Out.SendNPCCreate(this);
						if (m_inventory != null)
							player.Out.SendLivingEquipmentUpdate(this);
					}
					BroadcastUpdate();
				}
			}
		}

		/// <summary>
		/// Gets or Sets the effective level of the Object
		/// </summary>
		public override int EffectiveLevel
		{
			get
			{
				IControlledBrain brain = Brain as IControlledBrain;
				if (brain != null)
					return brain.Owner.Level;
				return base.Level;
			}
		}

		/// <summary>
		/// Gets or sets the Realm of this NPC
		/// </summary>
		public override byte Realm
		{
			get
			{
				IControlledBrain brain = Brain as IControlledBrain;
				if (brain != null)
					return brain.Owner.Realm; // always realm of the owner
				return base.Realm;
			}
			set
			{
				base.Realm = value;
				if (ObjectState == eObjectState.Active)
				{
					foreach (GamePlayer player in GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
					{
						player.Out.SendNPCCreate(this);
						if (m_inventory != null)
							player.Out.SendLivingEquipmentUpdate(this);
					}
					BroadcastUpdate();
				}
			}
		}

		/// <summary>
		/// Gets or sets the name of this npc
		/// </summary>
		public override string Name
		{
			get { return base.Name; }
			set
			{
				base.Name = value;
				if (ObjectState == eObjectState.Active)
				{
					foreach (GamePlayer player in GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
					{
						player.Out.SendNPCCreate(this);
						if (m_inventory != null)
							player.Out.SendLivingEquipmentUpdate(this);
					}
					BroadcastUpdate();
				}
			}
		}

		/// <summary>
		/// Gets or sets the guild name
		/// </summary>
		public override string GuildName
		{
			get { return base.GuildName; }
			set
			{
				base.GuildName = value;
				if (ObjectState == eObjectState.Active)
				{
					foreach (GamePlayer player in GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
					{
						player.Out.SendNPCCreate(this);
						if (m_inventory != null)
							player.Out.SendLivingEquipmentUpdate(this);
					}
					BroadcastUpdate();
				}
			}
		}
		/// <summary>
		/// Holds the Faction of the NPC
		/// </summary>
		protected Faction m_faction;

		/// <summary>
		/// Gets the Faction of the NPC
		/// </summary>
		public Faction Faction
		{
			get { return m_faction; }
			set
			{
				m_faction = value;
			}
		}

		protected ArrayList m_linkedFactions;

		public ArrayList LinkedFactions
		{
			get { return m_linkedFactions; }
			set { m_linkedFactions = value; }
		}

		#endregion
		#region Flags/Position/SpawnPosition/UpdateTick
		/// <summary>
		/// Various flags for this npc
		/// </summary>
		[Flags]
		public enum eFlags : byte
		{
			/// <summary>
			/// The npc is a ghost, see through, transparent
			/// </summary>
			TRANSPARENT = 0x01,
			/// <summary>
			/// The npc is stealth (new since 1.71)
			/// </summary>
			STEALTH = 0x02,
			/// <summary>
			/// The npc doesn't show a name above its head but can be targeted
			/// </summary>
			DONTSHOWNAME = 0x04,
			/// <summary>
			/// The npc doesn't show a name above its head and can't be targeted
			/// </summary>
			CANTTARGET = 0x08,
			/// <summary>
			/// Not in nearest enemyes if different vs player realm, but can be targeted if model support this
			/// </summary>
			PEACE = 0x10,
			/// <summary>
			/// The npc is flying (z above ground permitted)
			/// </summary>
			FLYING = 0x20,
		}
		/// <summary>
		/// Holds various flags of this npc
		/// </summary>
		protected uint m_flags;
		/// <summary>
		/// Spawn X coordinate
		/// </summary>
		protected int m_spawnX;
		/// <summary>
		/// Spawn Y coordinate
		/// </summary>
		protected int m_spawnY;
		/// <summary>
		/// Spawn Z coordinate
		/// </summary>
		protected int m_spawnZ;
		/// <summary>
		/// Spawn Heading
		/// </summary>
		protected ushort m_spawnHeading;
		/// <summary>
		/// The last time this NPC sent the 0x09 update packet
		/// </summary>
		protected volatile uint m_lastUpdateTickCount = uint.MinValue;
		/// <summary>
		/// The last time this NPC was actually updated to at least one player
		/// </summary>
		protected volatile uint m_lastVisibleToPlayerTick = uint.MinValue;
		/// <summary>
		/// Gets or Sets the flags of this npc
		/// </summary>
		public virtual uint Flags
		{
			get { return m_flags; }
			set
			{
				uint oldflags = m_flags;
				m_flags = value;
				if (ObjectState == eObjectState.Active)
				{
					bool ghostChanged = (oldflags & (byte)eFlags.TRANSPARENT) != (value & (byte)eFlags.TRANSPARENT);
					bool stealthChanged = (oldflags & (byte)eFlags.STEALTH) != (value & (byte)eFlags.STEALTH);
					bool cantTargetChanged = (oldflags & (byte)eFlags.CANTTARGET) != (value & (byte)eFlags.CANTTARGET);
					bool dontShowNameChanged = (oldflags & (byte)eFlags.DONTSHOWNAME) != (value & (byte)eFlags.DONTSHOWNAME);
					bool peaceChanged = (oldflags & (byte)eFlags.PEACE) != (value & (byte)eFlags.PEACE);
					bool flyingChanged = (oldflags & (byte)eFlags.FLYING) != (value & (byte)eFlags.FLYING);

					if (ghostChanged || stealthChanged || cantTargetChanged || dontShowNameChanged || peaceChanged || flyingChanged)
					{
						foreach (GamePlayer player in GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
						{
							player.Out.SendNPCCreate(this);
							if (m_inventory != null)
								player.Out.SendLivingEquipmentUpdate(this);
						}
					}
					BroadcastUpdate();
				}
			}
		}

		/// <summary>
		/// Shows wether any player sees that mob
		/// we dont need to calculate things like AI if mob is in no way
		/// visible to at least one player
		/// </summary>
		public virtual bool IsVisibleToPlayers
		{
			get { return (uint)Environment.TickCount - m_lastVisibleToPlayerTick < 60000; }
		}
		/// <summary>
		/// Gets or sets the spawnposition of this npc
		/// </summary>
		public virtual int SpawnX
		{
			get { return m_spawnX; }
			set { m_spawnX = value; }
		}
		/// <summary>
		/// Gets or sets the spawnposition of this npc
		/// </summary>
		public virtual int SpawnY
		{
			get { return m_spawnY; }
			set { m_spawnY = value; }
		}
		/// <summary>
		/// Gets or sets the spawnposition of this npc
		/// </summary>
		public virtual int SpawnZ
		{
			get { return m_spawnZ; }
			set { m_spawnZ = value; }
		}

		/// <summary>
		/// Gets or sets the spawnheading of this npc
		/// </summary>
		public virtual ushort SpawnHeading
		{
			get { return m_spawnHeading; }
			set { m_spawnHeading = value; }
		}

		/// <summary>
		/// Gets or sets the current speed of the npc
		/// </summary>
		public override int CurrentSpeed
		{
			set
			{
				//Update the position before changing speed!
				m_X = X;
				m_Y = Y;
				m_Z = Z;
				base.CurrentSpeed = value;
				BroadcastUpdate();
			}
		}

		/// <summary>
		/// Gets sets the currentwaypoint that npc has to wander to
		/// </summary>
		public PathPoint CurrentWayPoint
		{
			get { return m_currentWayPoint; }
			set { m_currentWayPoint = value; }
		}
		/// <summary>
		/// Stores the currentwaypoint that npc has to wander to
		/// </summary>
		protected PathPoint m_currentWayPoint = null;

		/// <summary>
		/// Gets sets the speed for traveling on path
		/// </summary>
		public int PathingNormalSpeed
		{
			get { return m_pathingNormalSpeed; }
			set { m_pathingNormalSpeed = value; }
		}
		/// <summary>
		/// Stores the speed for traveling on path
		/// </summary>
		protected int m_pathingNormalSpeed;

		/// <summary>
		/// Gets the current X of this living. Don't modify this property
		/// to try to change position of the mob while active. Use the
		/// MoveTo function instead
		/// </summary>
		public override int X
		{
			get
			{
				if (!IsMoving)
					return base.X;

				if (TargetX != 0 || TargetY != 0 || TargetZ != 0)
				{
					long diffnow = (long)FastMath.Abs((Environment.TickCount - MovementStartTick) * m_xAddition);
					long diffshould = FastMath.Abs((long)m_targetX - m_X);
					if (diffshould == 0) return m_targetX;

					if (diffshould - diffnow < 0)
					{
						return TargetX;
					}
				}
				return base.X;
			}
		}

		/// <summary>
		/// Gets the current Y of this NPC. Don't modify this property
		/// to try to change position of the mob while active. Use the
		/// MoveTo function instead
		/// </summary>
		public override int Y
		{
			get
			{
				if (!IsMoving)
					return base.Y;

				if (TargetX != 0 || TargetY != 0 || TargetZ != 0)
				{
					long diffnow = (long)FastMath.Abs((Environment.TickCount - MovementStartTick) * m_yAddition);
					long diffshould = FastMath.Abs((long)TargetY - m_Y);
					if (diffshould == 0) return TargetY;

					if (diffshould - diffnow < 0)
					{
						return TargetY;
					}
				}
				return base.Y;
			}
		}

		/// <summary>
		/// Gets the current Z of this NPC. Don't modify this property
		/// to try to change position of the mob while active. Use the
		/// MoveTo function instead
		/// </summary>
		public override int Z
		{
			get
			{
				if (!IsMoving)
					return base.Z;

				if (TargetX != 0 || TargetY != 0 || TargetZ != 0)
				{
					long diffnow = (long)FastMath.Abs((Environment.TickCount - MovementStartTick) * m_zAddition);
					long diffshould = FastMath.Abs((long)TargetZ - m_Z);
					if (diffshould == 0) return TargetZ;

					if (diffshould - diffnow < 0)
					{
						return TargetZ;
					}
				}
				return base.Z;
			}
		}

		/// <summary>
		/// The stealth state of this NPC
		/// </summary>
		public override bool IsStealthed
		{
			get
			{
				return (Flags & (uint)eFlags.TRANSPARENT) != 0;//TODO
			}
		}

		#endregion
		#region Movement
		/// <summary>
		/// Target X coordinate to walk to
		/// </summary>
		protected int m_targetX;
		/// <summary>
		/// Target Y coordinate to walk to
		/// </summary>
		protected int m_targetY;
		/// <summary>
		/// Target Z coordinate to walk to
		/// </summary>
		protected int m_targetZ;
		/// <summary>
		/// Timer to be set if an OnArriveAtTarget
		/// handler is set before calling the WalkTo function
		/// </summary>
		protected ArriveAtTargetAction m_arriveAtTargetAction;
		/// <summary>
		/// The interval between follow checks, in milliseconds
		/// </summary>
		protected const int FOLLOWCHECKTICKS = 500;
		/// <summary>
		/// Timer to be set if an OnCloseToTarget
		/// handler is set before calling the WalkTo function
		/// </summary>
		protected CloseToTargetAction m_closeToTargetAction;
		/// <summary>
		/// Object that this npc is following as weakreference
		/// </summary>
		protected WeakReference m_followTarget;
		/// <summary>
		/// Max range to keep following
		/// </summary>
		protected int m_followMaxDist;
		/// <summary>
		/// Min range to keep to the target
		/// </summary>
		protected int m_followMinDist;
		/// <summary>
		/// Timer with purpose of follow updating
		/// </summary>
		protected RegionTimer m_followTimer;
		/// <summary>
		/// Property entry on follow timer, wether the follow target is in range
		/// </summary>
		protected static readonly string FOLLOW_TARGET_IN_RANGE = "FollowTargetInRange";

		/// <summary>
		/// Recalculates position addition values of this living
		/// </summary>
		protected override void RecalculatePostionAddition()
		{
			if (!IsMoving)
			{
				//				log.ErrorFormat("{0} is not moving\n{1}", Name, Environment.StackTrace);
				m_xAddition = m_yAddition = m_zAddition = 0f;
				return;
			}

			if (TargetX != 0 || TargetY != 0 || TargetZ != 0)
			{
				float dist = WorldMgr.GetDistance(m_X, m_Y, m_Z, m_targetX, m_targetY, m_targetZ);
				if (dist <= 0)
				{
					m_xAddition = m_yAddition = m_zAddition = 0f;
					return;
				}
				float speed = CurrentSpeed;
				float diffx = (long)TargetX - m_X;
				float diffy = (long)TargetY - m_Y;
				float diffz = (long)TargetZ - m_Z;
				m_xAddition = (diffx * speed / dist) * 0.001f;
				m_yAddition = (diffy * speed / dist) * 0.001f;
				m_zAddition = (diffz * speed / dist) * 0.001f;
				//				log.WarnFormat("{0} is moving to target, dist = {1}, add = ({2} {3} {4})", Name, dist, m_xAddition, m_yAddition, m_zAddition);
			}
			else
			{
				//				log.WarnFormat("{0} is moving but target is 0", Name);
				base.RecalculatePostionAddition();
			}
		}

		/// <summary>
		/// Returns if the mob has arrived on its target
		/// </summary>
		/// <returns>true if on target</returns>
		public bool IsOnTarget()
		{
			if (X == TargetX && Y == TargetY && Z == TargetZ) return true;
			return false;
		}
		/// <summary>
		/// Gets or sets the TargetX
		/// </summary>
		public virtual int TargetX
		{
			get
			{
				return m_targetX;
			}
		}
		/// <summary>
		/// Gets or sets the TargetY
		/// </summary>
		public virtual int TargetY
		{
			get
			{
				return m_targetY;
			}
		}
		/// <summary>
		/// Gets or sets the TargetZ
		/// </summary>
		public virtual int TargetZ
		{
			get
			{
				return m_targetZ;
			}
		}

		/// <summary>
		/// Turns the npc towards a specific spot
		/// </summary>
		/// <param name="tx">Target X</param>
		/// <param name="ty">Target Y</param>
		public virtual void TurnTo(int tx, int ty)
		{
			if (this.Stun || this.Mez) return;
			Notify(GameNPCEvent.TurnTo, this, new TurnToEventArgs(tx, ty));
			Heading = GetHeadingToSpot(tx, ty);
		}

		/// <summary>
		/// Turns the npc towards a specific heading
		/// </summary>
		/// <param name="newHeading">the new heading</param>
		public virtual void TurnTo(ushort newHeading)
		{
			if (this.Stun || this.Mez) return;
			Notify(GameNPCEvent.TurnToHeading, this, new TurnToHeadingEventArgs(newHeading));
			Heading = newHeading;
		}
		/// <summary>
		/// Turns the NPC towards a specific gameObject
		/// which can be anything ... a player, item, mob, npc ...
		/// </summary>
		/// <param name="target">GameObject to turn towards</param>
		public virtual void TurnTo(GameObject target)
		{
			if (target == null) return;
			if (target.CurrentRegion != CurrentRegion) return;
			TurnTo(target.X, target.Y);
		}
		/// <summary>
		/// Turns the NPC towards a specific gameObject
		/// which can be anything ... a player, item, mob, npc ...
		/// and turn back after specified duration
		/// </summary>
		/// <param name="target">GameObject to turn towards</param>
		/// <param name="duration">restore heading after this duration</param>
		public virtual void TurnTo(GameObject target, int duration)
		{
			if (target == null) return;
			if (target.CurrentRegion != CurrentRegion) return;
			// store original heading if not set already
			RestoreHeadingAction restore = (RestoreHeadingAction)TempProperties.getObjectProperty(RESTORE_HEADING_ACTION_PROP, null);
			if (restore == null)
			{
				restore = new RestoreHeadingAction(this);
				TempProperties.setProperty(RESTORE_HEADING_ACTION_PROP, restore);
			}
			TurnTo(target);
			restore.Start(duration);
		}

		/// <summary>
		/// The property used to store the NPC heading restore action
		/// </summary>
		protected const string RESTORE_HEADING_ACTION_PROP = "NpcRestoreHeadingAction";

		/// <summary>
		/// Restores the NPC heading after some time
		/// </summary>
		protected class RestoreHeadingAction : RegionAction
		{
			/// <summary>
			/// The NPCs old heading
			/// </summary>
			protected readonly ushort m_oldHeading;

			/// <summary>
			/// The NPCs old position
			/// </summary>
			protected readonly Point3D m_oldPosition;

			/// <summary>
			/// Creates a new TurnBackAction
			/// </summary>
			/// <param name="actionSource">The source of action</param>
			public RestoreHeadingAction(GameNPC actionSource)
				: base(actionSource)
			{
				m_oldHeading = actionSource.Heading;
				m_oldPosition = new Point3D(actionSource);
			}

			/// <summary>
			/// Called on every timer tick
			/// </summary>
			protected override void OnTick()
			{
				GameNPC npc = (GameNPC)m_actionSource;

				npc.TempProperties.removeProperty(RESTORE_HEADING_ACTION_PROP);

				if (npc.ObjectState != eObjectState.Active) return;
				if (!npc.Alive) return;
				if (npc.AttackState) return;
				if (npc.IsMoving) return;
				if (npc.Equals(m_oldPosition)) return;
				if (npc.Heading == m_oldHeading) return; // already set? oO

				npc.TurnTo(m_oldHeading);
			}
		}

		/// <summary>
		/// Gets the last time this mob was updated
		/// </summary>
		public uint LastUpdateTickCount
		{
			get { return m_lastUpdateTickCount; }
		}

		/// <summary>
		/// Gets the last this this NPC was actually update to at least one player.
		/// </summary>
		public uint LastVisibleToPlayersTickCount
		{
			get { return m_lastVisibleToPlayerTick; }
		}

		/// <summary>
		/// Delayed action that fires an event when an NPC arrives at its target
		/// </summary>
		protected class ArriveAtTargetAction : RegionAction
		{
			/// <summary>
			/// Constructs a new ArriveAtTargetAction
			/// </summary>
			/// <param name="actionSource">The action source</param>
			public ArriveAtTargetAction(GameNPC actionSource)
				: base(actionSource)
			{
			}

			/// <summary>
			/// This function is called when the Mob arrives at its target spot
			/// It fires the ArriveAtTarget event
			/// </summary>
			protected override void OnTick()
			{
				GameNPC npc = (GameNPC)m_actionSource;
				npc.m_arriveAtTargetAction = null;
				npc.Notify(GameNPCEvent.ArriveAtTarget, npc);
			}
		}

		/// <summary>
		/// Delayed action that fires an event when an NPC is 200ms away from its target
		/// </summary>
		protected class CloseToTargetAction : RegionAction
		{
			/// <summary>
			/// Constructs a new CloseToTargetAction
			/// </summary>
			/// <param name="actionSource">The action source</param>
			public CloseToTargetAction(GameNPC actionSource)
				: base(actionSource)
			{
			}

			/// <summary>
			/// This function is called when the npc is close to its target
			/// It will fire the CloseToTarget event
			/// </summary>
			protected override void OnTick()
			{
				GameNPC npc = (GameNPC)m_actionSource;
				npc.m_closeToTargetAction = null;
				npc.Notify(GameNPCEvent.CloseToTarget, npc);
			}
		}

		/// <summary>
		/// This function is used to make the npc move towards
		/// a certain target spot within this region. The target
		/// spot should be in the same or an adjacent Zone of the
		/// npc
		/// </summary>
		/// <param name="tx">target x</param>
		/// <param name="ty">target y</param>
		/// <param name="tz">target z (or 0 to put the mob on the ground)</param>
		/// <param name="speed">walk speed</param>
		public virtual void WalkTo(int tx, int ty, int tz, int speed)
		{
			if (IsTurningDisabled)
				return; // can't walk when turning is disabled
			//Slow mobs down when they are hurt!
			int maxSpeed = MaxSpeed;
			if (speed > maxSpeed)
				speed = maxSpeed;

			Notify(GameNPCEvent.WalkTo, this, new WalkToEventArgs(tx, ty, tz, speed));
			//Set our current position
			m_X = X;
			m_Y = Y;
			m_Z = Z;
			m_Heading = GetHeadingToSpot(tx, ty);
			m_currentSpeed = speed;
			m_targetX = tx;
			m_targetY = ty;
			m_targetZ = tz;
			RecalculatePostionAddition();

			//ARGHL!!!! the following took me 2 days to find out!
			//The mobs in the database have a Z value set ... but normally when
			//we call WalkTo, we set TargetZ to Zero, this results in a HUGE
			//distance bug, makeing the mobs move totally weird!
			//So we have to test if our targetZ == 0
			//duff answer : ever test in get distance so do not need it!
			double dist = WorldMgr.GetDistance(m_X, m_Y, m_Z, m_targetX, m_targetY, m_targetZ);

			//DOLConsole.WriteLine("DX="+diffx+" DY="+diffy+" DIST="+dist);
			int timeToTarget = 0;
			if (speed > 0)
			{
				timeToTarget = (int)(dist * 1000 / speed);
			}

			if (m_arriveAtTargetAction != null)
			{
				m_arriveAtTargetAction.Stop();
			}

			m_arriveAtTargetAction = new ArriveAtTargetAction(this);
			if (timeToTarget > 1)
			{
				m_arriveAtTargetAction.Start(timeToTarget);
			}
			else
			{
				m_arriveAtTargetAction.Start(1);
			}

			if (m_closeToTargetAction != null)
			{
				m_closeToTargetAction.Stop();
			}

			m_closeToTargetAction = new CloseToTargetAction(this);
			if (timeToTarget > 200)
			{
				m_closeToTargetAction.Start(timeToTarget - 200); //200ms before target is close
			}
			else
			{
				m_closeToTargetAction.Start(1);
			}

			MovementStartTick = Environment.TickCount;
			BroadcastUpdate();
		}

		/// <summary>
		/// Walk to a certain point with given speed
		/// </summary>
		/// <param name="p"></param>
		/// <param name="speed"></param>
		public virtual void WalkTo(IPoint3D p, int speed)
		{
			WalkTo(p.X, p.Y, p.Z, speed);
		}

		/// <summary>
		/// Walk to the spawn point
		/// </summary>
		public virtual void WalkToSpawn()
		{
			StopAttack();
			StopFollow();
			//			WalkTo(SpawnX+Random(750)-350, SpawnY+Random(750)-350, SpawnZ, MaxSpeed/3);
			WalkTo(SpawnX, SpawnY, SpawnZ, (int)(MaxSpeed / 2.5));
		}

		/// <summary>
		/// This function is used to start the mob walking. It will
		/// walk in the heading direction until the StopMovement function
		/// is called
		/// </summary>
		/// <param name="speed">walk speed</param>
		public virtual void Walk(int speed)
		{
			Notify(GameNPCEvent.Walk, this, new WalkEventArgs(speed));

			if (m_arriveAtTargetAction != null)
			{
				m_arriveAtTargetAction.Stop();
				m_arriveAtTargetAction = null;
			}
			if (m_closeToTargetAction != null)
			{
				m_closeToTargetAction.Stop();
				m_closeToTargetAction = null;
			}

			m_X = X;
			m_Y = Y;
			m_Z = Z;

			m_targetX = 0;
			m_targetY = 0;
			m_targetZ = 0;

			m_currentSpeed = speed;

			MovementStartTick = Environment.TickCount;
			RecalculatePostionAddition();
			BroadcastUpdate();
		}

		/// <summary>
		/// Gets the NPC current follow target
		/// </summary>
		public GameObject CurrentFollowTarget
		{
			get { return m_followTarget.Target as GameObject; }
		}

		/// <summary>
		/// Stops the movement of the mob
		/// </summary>
		public virtual void StopMoving()
		{
			//if(!IsMoving) return;
			if (m_arriveAtTargetAction != null)
			{
				m_arriveAtTargetAction.Stop();
				m_arriveAtTargetAction = null;
			}
			if (m_closeToTargetAction != null)
			{
				m_closeToTargetAction.Stop();
				m_closeToTargetAction = null;
			}
			//This broadcasts an update!
			CurrentSpeed = 0;
		}
		/// <summary>
		/// Follow given object
		/// </summary>
		/// <param name="followTarget">target to follow</param>
		/// <param name="minDistance">min distance to keep to the target</param>
		/// <param name="maxDistance">max distance to keep following</param>
		public virtual void Follow(GameObject followTarget, int minDistance, int maxDistance)
		{
			//			string targName = followTarget==null ? "(null)" : followTarget.Name;
			//			log.Debug(this.Name+": Follow("+targName+","+minDistance+","+maxDistance+")");
			//First stop the active timer
			if (m_followTimer.IsAlive)
				m_followTimer.Stop();
			if (followTarget == null || followTarget.ObjectState != eObjectState.Active) return;
			//Set the new values
			m_followMaxDist = maxDistance;
			m_followMinDist = minDistance;
			m_followTarget.Target = followTarget;
			//Start the timer again
			m_followTimer.Start(1);
			//TODO fire event OnFollow
		}

		/// <summary>
		/// Stop following
		/// </summary>
		public virtual void StopFollow()
		{
			lock (m_followTimer)
			{
				if (m_followTimer.IsAlive)
					m_followTimer.Stop();
				m_followTarget.Target = null;
				StopMoving();
				//TODO fire event OnStopFollow
			}
		}

		/// <summary>
		/// Will be called if follow mode is active
		/// and we reached the follow target
		/// </summary>
		public virtual void FollowTargetInRange()
		{
			if (AttackState)
			{
				// if in last attack the enemy was out of range, we can attack him now immediately
				AttackData ad = (AttackData)TempProperties.getObjectProperty(LAST_ATTACK_DATA, null);
				if (ad != null && ad.AttackResult == eAttackResult.OutOfRange)
				{
					m_attackAction.Start(1);// schedule for next tick
				}
			}
		}

		/// <summary>
		/// Keep following a specific object at a max distance
		/// </summary>
		protected virtual int FollowTimerCallback(RegionTimer callingTimer)
		{
			bool wasInRange = m_followTimer.Properties.getProperty(FOLLOW_TARGET_IN_RANGE, false);
			m_followTimer.Properties.removeProperty(FOLLOW_TARGET_IN_RANGE);

			GameObject followTarget = (GameObject)m_followTarget.Target;
			GameLiving followLiving = followTarget as GameLiving;
			if (followLiving != null && !followLiving.Alive)
			{
				StopFollow();
				Notify(GameNPCEvent.FollowLostTarget, this, new FollowLostTargetEventArgs(followTarget));
				return 0;
			}

			//Stop following if we have no target
			if (followTarget == null || followTarget.ObjectState != eObjectState.Active || CurrentRegionID != followTarget.CurrentRegionID)
			{
				StopFollow();
				Notify(GameNPCEvent.FollowLostTarget, this, new FollowLostTargetEventArgs(followTarget));
				return 0;
			}

			//Calculate the difference between our position and the players position
			float diffx = (long)followTarget.X - X;
			float diffy = (long)followTarget.Y - Y;
			float diffz = (long)followTarget.Z - Z;

			//SH: Removed Z checks when one of the two Z values is zero(on ground)
			float distance = 0;
			if (followTarget.Z == 0 || Z == 0)
				distance = (float)Math.Sqrt(diffx * diffx + diffy * diffy);
			else
				distance = (float)Math.Sqrt(diffx * diffx + diffy * diffy + diffz * diffz);

			//if distance is greater then the max follow distance, stop following and return home
			if (distance > m_followMaxDist)
			{
				StopFollow();
				Notify(GameNPCEvent.FollowLostTarget, this, new FollowLostTargetEventArgs(followTarget));
				this.WalkToSpawn();
			}

			//if the npc hasn't hit or been hit in a while, stop following and return home
			StandardMobBrain brain = this.Brain as StandardMobBrain;
			if (AttackState && brain != null && followLiving != null)
			{
				long seconds = 10 + ((brain.GetAggroAmountForLiving(followLiving) / (MaxHealth + 1)) * 100);
				long lastattacked = m_lastAttackTick;
				long lasthit = m_lastAttackedByEnemyTick;
				if (CurrentRegion.Time - lastattacked > seconds * 1000 && CurrentRegion.Time - lasthit > seconds * 1000)
				{
					StopFollow();
					Notify(GameNPCEvent.FollowLostTarget, this, new FollowLostTargetEventArgs(followTarget));
					this.WalkToSpawn();
					return 0;
				}
			}

			//Are we in range yet?
			if (distance <= m_followMinDist)
			{
				//StopMoving();
				TurnTo(followTarget);
				if (!wasInRange)
				{
					m_followTimer.Properties.setProperty(FOLLOW_TARGET_IN_RANGE, true);
					FollowTargetInRange();
				}
				return FOLLOWCHECKTICKS;
			}

			// follow on distance
			diffx = (diffx / distance) * m_followMinDist;
			diffy = (diffy / distance) * m_followMinDist;
			diffz = (diffz / distance) * m_followMinDist;

			//Subtract the offset from the target's position to get
			//our target position
			int newX = (int)(followTarget.X - diffx);
			int newY = (int)(followTarget.Y - diffy);
			int newZ = (int)(followTarget.Z - diffz);
			WalkTo(newX, newY, (ushort)newZ, MaxSpeed);
			return FOLLOWCHECKTICKS;
		}

		/// <summary>
		/// Disables the turning for this living
		/// </summary>
		/// <param name="add"></param>
		public override void DisableTurning(bool add)
		{
			bool old = IsTurningDisabled;
			base.DisableTurning(add);
			if (old != IsTurningDisabled)
				BroadcastUpdate();
		}

		#endregion
		#region Inventory/LoadfromDB

		/// <summary>
		/// Loads the equipment template of this npc
		/// </summary>
		/// <param name="equipmentTemplateID">The template id</param>
		public virtual void LoadEquipmentTemplateFromDatabase(string equipmentTemplateID)
		{
			EquipmentTemplateID = equipmentTemplateID;
			if (EquipmentTemplateID != null && EquipmentTemplateID.Length > 0)
			{
				GameNpcInventoryTemplate template = new GameNpcInventoryTemplate();
				if (template.LoadFromDatabase(EquipmentTemplateID))
				{
					m_inventory = template.CloseTemplate();
				}
				else
				{
					if (log.IsDebugEnabled)
					{
						//log.Warn("Error loading NPC inventory: InventoryID="+EquipmentTemplateID+", NPC name="+Name+".");
					}
				}
				if (Inventory != null)
				{
					// if the two handed slot isnt empty we use that
					// or if the distance slot isnt empty we use that
					if (Inventory.GetItem(eInventorySlot.TwoHandWeapon) != null)
						SwitchWeapon(eActiveWeaponSlot.TwoHanded);
					else if (Inventory.GetItem(eInventorySlot.DistanceWeapon) != null)
						SwitchWeapon(eActiveWeaponSlot.Distance);
					else SwitchWeapon(eActiveWeaponSlot.Standard); // sets visible left and right hand slots
				}
			}
		}


		/// <summary>
		/// Load a npc from the npc template
		/// </summary>
		/// <param name="obj">template to load from</param>
		public override void LoadFromDatabase(DataObject obj)
		{
			base.LoadFromDatabase(obj);
			if (!(obj is Mob)) return;
			Mob npc = (Mob)obj;
			Name = npc.Name;
			GuildName = npc.Guild;
			m_X = npc.X;
			m_Y = npc.Y;
			m_Z = npc.Z;
			m_Heading = (ushort)(npc.Heading & 0xFFF);
			m_maxSpeedBase = npc.Speed;	// TODO db has currntly senseless information here, mob type db required
			if (m_maxSpeedBase == 0)
				m_maxSpeedBase = 191;
			m_currentSpeed = 0;
			CurrentRegionID = npc.Region;
			Realm = npc.Realm;
			Model = npc.Model;
			Size = npc.Size;
			Level = npc.Level;	// health changes when GameNPC.Level changes
			Flags = npc.Flags;
			MeleeDamageType = (eDamageType)npc.MeleeDamageType;
			if (MeleeDamageType == 0)
			{
				MeleeDamageType = eDamageType.Slash;
			}
			IAggressiveBrain aggroBrain = Brain as IAggressiveBrain;
			if (aggroBrain != null)
			{
				if (npc.AggroRange == 0)
				{
					if (Char.IsLower(Name[0]))
					{ // make some default on basic mobs
						aggroBrain.AggroRange = 450;
						aggroBrain.AggroLevel = (Level > 3) ? 30 : 0;
					}
					else if (Realm != 0)
					{
						aggroBrain.AggroRange = 500;
						aggroBrain.AggroLevel = 60;
					}
				}
				else
				{
					aggroBrain.AggroLevel = npc.AggroLevel;
					aggroBrain.AggroRange = npc.AggroRange;
				}
			}
			m_activeWeaponSlot = eActiveWeaponSlot.Standard;
			ActiveQuiverSlot = eActiveQuiverSlot.None;

			m_faction = FactionMgr.GetFactionByID(npc.FactionID);
			LoadEquipmentTemplateFromDatabase(npc.EquipmentTemplateID);
		}

		/// <summary>
		/// Switches the active weapon to another one
		/// </summary>
		/// <param name="slot">the new eActiveWeaponSlot</param>
		public override void SwitchWeapon(eActiveWeaponSlot slot)
		{
			base.SwitchWeapon(slot);
			if (ObjectState == eObjectState.Active)
			{
				// Update active weapon appearence
				UpdateNPCEquipmentAppearance();
			}
		}
		/// <summary>
		/// Equipment templateID
		/// </summary>
		protected string m_equipmentTemplateID;
		/// <summary>
		/// The equipment template id of this npc
		/// </summary>
		public string EquipmentTemplateID
		{
			get { return m_equipmentTemplateID; }
			set { m_equipmentTemplateID = value; }
		}
		/// <summary>
		/// Updates the items on a character
		/// </summary>
		public void UpdateNPCEquipmentAppearance()
		{
			if (ObjectState == eObjectState.Active)
			{
				foreach (GamePlayer player in GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
					player.Out.SendLivingEquipmentUpdate(this);
			}
		}

		#endregion
		#region Quest
		/// <summary>
		/// Holds all the quests this npc can give to players
		/// </summary>
		protected readonly ArrayList m_questListToGive = new ArrayList(1);

		/// <summary>
		/// Gets the questlist of this player
		/// </summary>
		public IList QuestListToGive
		{
			get { return m_questListToGive; }
		}

		/// <summary>
		/// Adds a quest type to the npc questlist
		/// </summary>
		/// <param name="questType">The quest type to add</param>
		/// <returns>true if added, false if the npc has already the quest!</returns>
		public void AddQuestToGive(Type questType)
		{
			lock (m_questListToGive.SyncRoot)
			{
				if (HasQuest(questType) == null)
				{
					AbstractQuest newQuest = (AbstractQuest)Activator.CreateInstance(questType);
					if (newQuest != null) m_questListToGive.Add(newQuest);
				}
			}
		}

		/// <summary>
		/// Adds a quest to the npc questlist
		/// </summary>
		/// <param name="questType">The questType to remove</param>
		/// <returns>true if added, false if the npc has already the quest!</returns>
		public bool RemoveQuestToGive(Type questType)
		{
			lock (m_questListToGive.SyncRoot)
			{
				foreach (AbstractQuest q in m_questListToGive)
				{
					if (q.GetType().Equals(questType))
					{
						m_questListToGive.Remove(q);
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Check if the npc can give the specified quest to a player
		/// </summary>
		/// <param name="questType">The type of the quest</param>
		/// <param name="player">The player who search a quest</param>
		/// <returns>the number of time the quest can be done again</returns>
		public int CanGiveQuest(Type questType, GamePlayer player)
		{
			lock (m_questListToGive.SyncRoot)
			{
				foreach (AbstractQuest q in m_questListToGive)
				{
					if (q.GetType().Equals(questType) && q.CheckQuestQualification(player) && player.HasFinishedQuest(questType) < q.MaxQuestCount)
					{
						return q.MaxQuestCount - player.HasFinishedQuest(questType);
					}
				}
			}
			return 0;
		}

		/// <summary>
		/// Check if the npc can give one quest to a player
		/// </summary>
		/// <param name="player">The player to check</param>
		/// <returns>true if yes, false if the npc can give any quest</returns>
		public bool CanGiveOneQuest(GamePlayer player)
		{
			lock (m_questListToGive.SyncRoot)
			{
				foreach (AbstractQuest q in m_questListToGive)
				{
					Type questType = q.GetType();
					int doingQuest = (player.IsDoingQuest(questType) != null ? 1 : 0);
					if (q.CheckQuestQualification(player) && player.HasFinishedQuest(questType) + doingQuest < q.MaxQuestCount)
						return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Give a quest a to specific player
		/// </summary>
		/// <param name="questType">The quest type</param>
		/// <param name="player">The player that gets the quest</param>
		/// <param name="startStep">The starting quest step</param>
		/// <returns>true if added, false if the player do already the quest!</returns>
		public bool GiveQuest(Type questType, GamePlayer player, int startStep)
		{
			AbstractQuest quest = HasQuest(questType);
			if (quest != null)
			{
				AbstractQuest newQuest = (AbstractQuest)Activator.CreateInstance(questType, new object[] { player, startStep });
				if (newQuest != null && player.AddQuest(newQuest))
				{
					if (!CanGiveOneQuest(player))
						player.Out.SendNPCsQuestEffect(this, false);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Checks if this npc already have a specified quest
		/// </summary>
		/// <param name="questType">The quest type</param>
		/// <returns>the quest if the npc have the quest or null if not</returns>
		private AbstractQuest HasQuest(Type questType)
		{
			lock (m_questListToGive.SyncRoot)
			{
				foreach (AbstractQuest q in m_questListToGive)
				{
					if (q.GetType().Equals(questType))
						return q;
				}
			}
			return null;
		}

		#endregion
		#region Riding
		//NPC's can have riders :-)
		/// <summary>
		/// Holds the rider of this NPC as weak reference
		/// </summary>
		protected WeakReference m_rider;
		/// <summary>
		/// Gets the rider of this mob
		/// </summary>
		public GamePlayer Rider
		{
			get
			{
				return m_rider.Target as GamePlayer;
			}
		}

		/// <summary>
		/// This function is called when a rider mounts this npc
		/// Since only players can ride NPC's you should use the
		/// GamePlayer.MountSteed function instead to make sure all
		/// callbacks are called correctly
		/// </summary>
		/// <param name="rider">GamePlayer that is the rider</param>
		/// <param name="forced">if true, mounting can't be prevented by handlers</param>
		/// <returns>true if mounted successfully</returns>
		public virtual bool RiderMount(GamePlayer rider, bool forced)
		{
			if (Rider != null) return false;
			Notify(GameNPCEvent.RiderMount, this, new RiderMountEventArgs(rider, this));
			m_rider.Target = rider;
			Rider.Steed = this;
			return true;
		}

		/// <summary>
		/// Called to dismount a rider from this npc.
		/// Since only players can ride NPC's you should use the
		/// GamePlayer.MountSteed function instead to make sure all
		/// callbacks are called correctly
		/// </summary>
		/// <param name="forced">if true, the dismounting can't be prevented by handlers</param>
		/// <returns>true if dismounted successfully</returns>
		public virtual bool RiderDismount(bool forced)
		{
			if (Rider == null) return false;
			Notify(GameNPCEvent.RiderDismount, this, new RiderDismountEventArgs(Rider, this));
			Rider.Steed = null;
			m_rider.Target = null;
			return true;
		}
		#endregion
		#region Add/Remove/Create/Remove/Update
		/// <summary>
		/// Broadcasts the npc to all players around
		/// </summary>
		public virtual void BroadcastUpdate()
		{
			if (ObjectState != eObjectState.Active) return;
			foreach (GamePlayer player in GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
			{
				if (player == null) continue;
				player.Out.SendNPCUpdate(this);
				player.CurrentUpdateArray[ObjectID - 1] = true;
			}
			m_lastUpdateTickCount = (uint)Environment.TickCount;
		}

		/// <summary>
		/// callback that npc was updated to the world
		/// so it must be visible to at least one player
		/// </summary>
		public void NPCUpdatedCallback()
		{
			m_lastVisibleToPlayerTick = (uint)Environment.TickCount;
			lock (BrainSync)
			{
				ABrain brain = Brain;
				if (brain != null)
					brain.Start();
			}
		}
		/// <summary>
		/// Adds the npc to the world
		/// </summary>
		/// <returns>true if the npc has been successfully added</returns>
		public override bool AddToWorld()
		{
			if (!base.AddToWorld()) return false;
			foreach (GamePlayer player in GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
			{
				if (player == null) continue;
				player.Out.SendNPCCreate(this);
				if (m_inventory != null)
					player.Out.SendLivingEquipmentUpdate(this);
			}
			BroadcastUpdate();
			m_spawnX = X;
			m_spawnY = Y;
			m_spawnZ = Z;
			m_spawnHeading = Heading;
			lock (BrainSync)
			{
				ABrain brain = Brain;
				if (brain != null)
					brain.Start();
			}

			if (Mana <= 0 && MaxMana > 0)
				Mana = MaxMana;
			else if (Mana > 0 && MaxMana > 0)
				StartPowerRegeneration();
			return true;
		}

		/// <summary>
		/// Removes the npc from the world
		/// </summary>
		/// <returns>true if the npc has been successfully removed</returns>
		public override bool RemoveFromWorld()
		{
			if (ObjectState == eObjectState.Active)
			{
				foreach (GamePlayer player in GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
					player.Out.SendRemoveObject(this);
			}
			if (!base.RemoveFromWorld()) return false;
			lock (BrainSync)
			{
				ABrain brain = Brain;
				brain.Stop();
			}
			return true;
		}

		/// <summary>
		/// Gets or Sets the current Region of the Object
		/// </summary>
		public override Region CurrentRegion
		{
			get { return base.CurrentRegion; }
			set
			{
				Region oldRegion = CurrentRegion;
				base.CurrentRegion = value;
				Region newRegion = CurrentRegion;
				if (oldRegion != newRegion && newRegion != null)
				{
					if (m_followTimer != null) m_followTimer.Stop();
					m_followTimer = new RegionTimer(this);
					m_followTimer.Callback = new RegionTimerCallback(FollowTimerCallback);
				}
			}
		}

		/// <summary>
		/// Marks this object as deleted!
		/// </summary>
		public override void Delete()
		{
			lock (BrainSync)
			{
				ABrain brain = Brain;
				brain.Stop();
			}
			StopFollow();
			TempProperties.removeProperty(CHARMED_TICK_PROP);
			base.Delete();
		}

		#endregion
		#region AI

		/// <summary>
		/// Holds the own NPC brain
		/// </summary>
		protected ABrain m_ownBrain;

		/// <summary>
		/// Holds the all added to this npc brains
		/// </summary>
		private ArrayList m_brains = new ArrayList(1);

		/// <summary>
		/// The sync object for brain changes
		/// </summary>
		private readonly object m_brainSync = new object();

		/// <summary>
		/// Gets the brain sync object
		/// </summary>
		public object BrainSync
		{
			get { return m_brainSync; }
		}

		/// <summary>
		/// Gets the current brain of this NPC
		/// </summary>
		public ABrain Brain
		{
			get
			{
				ArrayList brains = m_brains;
				if (brains.Count > 0)
					return (ABrain)brains[brains.Count - 1];
				return m_ownBrain;
			}
		}

		/// <summary>
		/// Sets the NPC own brain
		/// </summary>
		/// <param name="brain">The new brain</param>
		/// <returns>The old own brain</returns>
		public virtual ABrain SetOwnBrain(ABrain brain)
		{
			if (brain == null)
				throw new ArgumentNullException("brain");
			if (brain.IsActive)
				throw new ArgumentException("The new brain is already active.", "brain");

			lock (BrainSync)
			{
				ABrain oldBrain = m_ownBrain;
				bool activate = oldBrain.IsActive;
				if (activate)
					oldBrain.Stop();
				m_ownBrain = brain;
				m_ownBrain.Body = this;
				if (activate)
					m_ownBrain.Start();

				return oldBrain;
			}
		}

		/// <summary>
		/// Adds a temporary brain to Npc, last added brain is active
		/// </summary>
		/// <param name="newBrain"></param>
		public virtual void AddBrain(ABrain newBrain)
		{
			if (newBrain == null)
				throw new ArgumentNullException("newBrain");
			if (newBrain.IsActive)
				throw new ArgumentException("The new brain is already active.", "newBrain");

			lock (BrainSync)
			{
				Brain.Stop();
				ArrayList brains = new ArrayList(m_brains);
				brains.Add(newBrain);
				m_brains = brains; // make new array list to avoid locks in the Brain property
				newBrain.Body = this;
				newBrain.Start();
			}
		}

		/// <summary>
		/// Removes a temporary brain from Npc
		/// </summary>
		/// <param name="removeBrain">The brain to remove</param>
		/// <returns>True if brain was found</returns>
		public virtual bool RemoveBrain(ABrain removeBrain)
		{
			if (removeBrain == null) return false;

			lock (BrainSync)
			{
				ArrayList brains = new ArrayList(m_brains);
				int index = brains.IndexOf(removeBrain);
				if (index < 0) return false;
				bool active = brains[index] == Brain;
				if (active)
					removeBrain.Stop();
				brains.RemoveAt(index);
				m_brains = brains;
				if (active)
					Brain.Start();

				return true;
			}
		}
		#endregion
		#region GetAggroLevelString

		/// <summary>
		/// How friendly this NPC is to player
		/// </summary>
		/// <param name="player">GamePlayer that is examining this object</param>
		/// <param name="firstLetterUppercase"></param>
		/// <returns>aggro state as string</returns>
		public virtual string GetAggroLevelString(GamePlayer player, bool firstLetterUppercase)
		{
			// "aggressive", "hostile", "neutral", "friendly"
			// TODO: correct aggro strings
			// TODO: some merchants can be aggressive to players even in same realm
			// TODO: findout if trainers can be aggro at all

			//int aggro = CalculateAggroLevelToTarget(player);

			IAggressiveBrain aggroBrain = Brain as IAggressiveBrain;
			if (GameServer.ServerRules.IsSameRealm(this, player, true))
			{
				if (firstLetterUppercase) return "Friendly";
				else return "friendly";
			}
			if (aggroBrain != null && aggroBrain.AggroLevel > 0)
			{
				if (firstLetterUppercase) return "Aggressive";
				else return "aggressive";
			}

			if (firstLetterUppercase) return "Neutral";
			else return "neutral";

		}

		/*		/// <summary>
				/// Pronoun of this NPC in case you need to refer it in 3rd person
				/// http://webster.commnet.edu/grammar/cases.htm
				/// </summary>
				/// <param name="firstLetterUppercase"></param>
				/// <param name="form">0=Subjective, 1=Possessive, 2=Objective</param>
				/// <returns>pronoun of this object</returns>
				public override string GetPronoun(bool firstLetterUppercase, int form)
				{
					// TODO: when mobs will get gender
					if(PlayerCharacter.Gender == 0)
						// male
						switch(form)
						{
							default: // Subjective
								if(firstLetterUppercase) return "He"; else return "he";
							case 1:	// Possessive
								if(firstLetterUppercase) return "His"; else return "his";
							case 2:	// Objective
								if(firstLetterUppercase) return "Him"; else return "him";
						}
					else
						// female
						switch(form)
						{
							default: // Subjective
								if(firstLetterUppercase) return "She"; else return "she";
							case 1:	// Possessive
								if(firstLetterUppercase) return "Her"; else return "her";
							case 2:	// Objective
								if(firstLetterUppercase) return "Her"; else return "her";
						}

					// it
					switch(form)
					{
						// Subjective
						default: if(firstLetterUppercase) return "It"; else return "it";
						// Possessive
						case 1:	if(firstLetterUppercase) return "Its"; else return "its";
						// Objective
						case 2: if(firstLetterUppercase) return "It"; else return "it";
					}
				}*/
		#endregion
		#region Interact/SayTo
		/// <summary>
		/// This function is called from the ObjectInteractRequestHandler
		/// </summary>
		/// <param name="player">GamePlayer that interacts with this object</param>
		/// <returns>false if interaction is prevented</returns>
		public override bool Interact(GamePlayer player)
		{
			if (!base.Interact(player)) return false;
			if (!GameServer.ServerRules.IsSameRealm(this, player, true))
			{
				player.Out.SendMessage(GetName(0, true) + " gives you a dirty look.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Format "say" message and send it to target in popup window
		/// </summary>
		/// <param name="target"></param>
		/// <param name="message"></param>
		public virtual void SayTo(GamePlayer target, string message)
		{
			SayTo(target, eChatLoc.CL_PopupWindow, message);
		}

		/// <summary>
		/// Format "say" message and send it to target
		/// </summary>
		/// <param name="target"></param>
		/// <param name="loc">chat location of the message</param>
		/// <param name="message"></param>
		public virtual void SayTo(GamePlayer target, eChatLoc loc, string message)
		{
			string resultText = GetName(0, true) + " says, \"" + message + "\"";

			switch (loc)
			{
				case eChatLoc.CL_PopupWindow:
					target.Out.SendMessage(resultText, eChatType.CT_System, eChatLoc.CL_PopupWindow);
					Message.ChatToArea(this, GetName(0, true) + " speaks to " + target.GetName(0, false), eChatType.CT_System, WorldMgr.SAY_DISTANCE, target);
					break;
				case eChatLoc.CL_ChatWindow:
					target.Out.SendMessage(resultText, eChatType.CT_Say, eChatLoc.CL_ChatWindow);
					break;
				case eChatLoc.CL_SystemWindow:
					target.Out.SendMessage(resultText, eChatType.CT_System, eChatLoc.CL_SystemWindow);
					break;
			}
		}
		#endregion
		#region Combat

		/// <summary>
		/// The property that holds charmed tick if any
		/// </summary>
		public const string CHARMED_TICK_PROP = "CharmedTick";

		/// <summary>
		/// The duration of no exp after charmed, in game ticks
		/// </summary>
		public const int CHARMED_NOEXP_TIMEOUT = 60000;

		/// <summary>
		/// Starts a melee attack on a target
		/// </summary>
		/// <param name="attackTarget">The object to attack</param>
		public override void StartAttack(GameObject attackTarget)
		{
			TargetObject = attackTarget;
			m_lastAttackTick = m_CurrentRegion.Time;
			base.StartAttack(attackTarget);
		}

		/// <summary>
		/// Gets/sets the object health
		/// </summary>
		public override int Health
		{
			get
			{
				return base.Health;
			}
			set
			{
				base.Health = value;
				//Slow mobs down when they are hurt!
				int maxSpeed = MaxSpeed;
				if (CurrentSpeed > maxSpeed)
					CurrentSpeed = maxSpeed;
			}
		}

		/// <summary>
		/// Tests if this MOB should give XP and loot based on the XPGainers
		/// </summary>
		/// <returns>true if it should deal XP and give loot</returns>
		public virtual bool IsWorthReward
		{
			get
			{
				if (CurrentRegion.Time - CHARMED_NOEXP_TIMEOUT < TempProperties.getLongProperty(CHARMED_TICK_PROP, 0L))
					return false;
				lock (m_xpGainers.SyncRoot)
				{
					if (m_xpGainers.Keys.Count == 0) return false;
					foreach (DictionaryEntry de in m_xpGainers)
					{
						GameObject obj = (GameObject)de.Key;
						if (obj is GamePlayer)
						{
							//If a gameplayer with privlevel > 1 attacked the
							//mob, then the players won't gain xp ...
							if (((GamePlayer)obj).Client.Account.PrivLevel > 1)
								return false;
							//If a player to which we are gray killed up we
							//aren't worth anything either
							if (((GamePlayer)obj).IsObjectGreyCon(this))
								return false;
						}
						else
						{
							//If object is no gameplayer and realm is != none
							//then it means that a npc has hit this living and
							//it is not worth any xp ...
							//if(obj.Realm != (byte)eRealm.None)
							//If grey to at least one living then no exp
							if (obj is GameLiving && ((GameLiving)obj).IsObjectGreyCon(this))
								return false;
						}
					}
					return true;
				}
			}
			set
			{
			}
		}

		/// <summary>
		/// Called when this living dies
		/// </summary>
		public override void Die(GameObject killer)
		{
			Message.SystemToArea(this, GetName(0, true) + " dies!", eChatType.CT_PlayerDied, killer);
			if (killer is GamePlayer)
				((GamePlayer)killer).Out.SendMessage(GetName(0, true) + " dies!", eChatType.CT_PlayerDied, eChatLoc.CL_SystemWindow);
			StopFollow();
			base.Die(killer);

			// deal out exp and realm points based on server rules
			GameServer.ServerRules.OnNPCKilled(this, killer);

			Delete();
		}

		/// <summary>
		/// Stores the melee damage type of this NPC
		/// </summary>
		protected eDamageType m_meleeDamageType = eDamageType.Slash;

		/// <summary>
		/// Gets or sets the melee damage type of this NPC
		/// </summary>
		public virtual eDamageType MeleeDamageType
		{
			get { return m_meleeDamageType; }
			set { m_meleeDamageType = value; }
		}

		/// <summary>
		/// Returns the damage type of the current attack
		/// </summary>
		/// <param name="weapon">attack weapon</param>
		public override eDamageType AttackDamageType(InventoryItem weapon)
		{
			return m_meleeDamageType;
		}

		/// <summary>
		/// Stores the NPC evade chance
		/// </summary>
		protected byte m_evadeChance;
		/// <summary>
		/// Stores the NPC block chance
		/// </summary>
		protected byte m_blockChance;
		/// <summary>
		/// Stores the NPC parry chance
		/// </summary>
		protected byte m_parryChance;
		/// <summary>
		/// Stores the NPC left hand swing chance
		/// </summary>
		protected byte m_leftHandSwingChance;

		/// <summary>
		/// Gets or sets the NPC evade chance
		/// </summary>
		public virtual byte EvadeChance
		{
			get { return m_evadeChance; }
			set { m_evadeChance = value; }
		}

		/// <summary>
		/// Gets or sets the NPC block chance
		/// </summary>
		public virtual byte BlockChance
		{
			get { return m_blockChance; }
			set { m_blockChance = value; }
		}

		/// <summary>
		/// Gets or sets the NPC parry chance
		/// </summary>
		public virtual byte ParryChance
		{
			get { return m_parryChance; }
			set { m_parryChance = value; }
		}

		/// <summary>
		/// Gets or sets the NPC left hand swing chance
		/// </summary>
		public byte LeftHandSwingChance
		{
			get { return m_leftHandSwingChance; }
			set { m_leftHandSwingChance = value; }
		}

		/// <summary>
		/// Calculates how many times left hand swings
		/// </summary>
		/// <returns></returns>
		public override int CalculateLeftHandSwingCount()
		{
			if (Util.Chance(m_leftHandSwingChance))
				return 1;
			return 0;
		}

		/// <summary>
		/// Checks whether Living has ability to use lefthanded weapons
		/// </summary>
		public override bool CanUseLefthandedWeapon
		{
			get { return m_leftHandSwingChance > 0; }
		}

		/// <summary>
		/// Method to switch the npc to Melee attacks
		/// </summary>
		/// <param name="target"></param>
		public void SwitchToMelee(GameObject target)
		{
			InventoryItem twohand = Inventory.GetItem(eInventorySlot.TwoHandWeapon);
			InventoryItem righthand = Inventory.GetItem(eInventorySlot.RightHandWeapon);

			if (twohand != null && righthand == null)
				SwitchWeapon(eActiveWeaponSlot.TwoHanded);
			else if (twohand != null && righthand != null)
			{
				if (Util.Chance(50))
					SwitchWeapon(eActiveWeaponSlot.TwoHanded);
				else SwitchWeapon(eActiveWeaponSlot.Standard);
			}
			else SwitchWeapon(eActiveWeaponSlot.Standard);
			Follow(target, 90, 5000);	// follow at stickrange
			StartAttack(target);
		}

		#endregion
		#region Spell
		/// <summary>
		/// array of spell
		/// </summary>
		private IList m_spells = new ArrayList(1);

		/// <summary>
		/// property of spell array of NPC
		/// </summary>
		public IList Spells
		{
			get { return m_spells; }
			set { m_spells = value; }
		}
		/// <summary>
		/// property of style array of NPC
		/// </summary>
		private IList m_styles = new ArrayList(1);
		public IList Styles
		{
			get { return m_styles; }
			set { m_styles = value; }
		}

		/// <summary>
		/// start to cast spell attack in continue until takken melee damage
		/// </summary>
		/// <param name="attackTarget"></param>
		/// <returns></returns>
		public virtual bool StartSpellAttack(GameObject attackTarget)
		{
			if (!WorldMgr.CheckDistance(attackTarget, this, AttackRange))
			{
				if (this.Spells != null)
				{
					foreach (Spell spell in this.Spells)
					{
						if (spell.SpellType == "DirectDamage")
						{
							SpellLine spellline = SkillBase.GetSpellLine(GlobalSpellsLines.Mob_Spells);
							this.CastSpell(spell, spellline);
							return true;
						}
					}
				}
			}
			return false;
		}

		/// <summary>
		/// stop the spell attack
		/// </summary>
		public virtual void StopSpellAttack()
		{
			if (m_runningSpellHandler != null && m_runningSpellHandler.Spell.SpellType == "DirectDamage")
			{
				//prevent from relaunch
				m_runningSpellHandler.CastingCompleteEvent -= new CastingCompleteCallback(OnAfterSpellCastSequence);
				m_runningSpellHandler = null;
			}
		}

		/// <summary>
		/// Callback after spell execution finished and next spell can be processed
		/// </summary>
		/// <param name="handler"></param>
		//	public override void OnAfterSpellCastSequence(ISpellHandler handler)
		//	{
		//		if (handler.Spell.SpellType == "DirectDamage")
		//		{
		//			m_runningSpellHandler = handler;
		//			handler.CastSpell();
		//		}
		//	}

		/// <summary>
		///
		/// </summary>
		/// <param name="body"></param>
		public void Buff(GameNPC body)
		{
			if (this.Spells != null)
			{
				foreach (Spell spell in this.Spells)
				{
					//todo find a way to get it by inherit of PropertyChangingSpell or not
					switch (spell.SpellType)
					{
						case "StrengthConstitutionBuff":
						case "DexterityQuicknessBuff":
						case "StrengthBuff":
						case "DexterityBuff":
						case "ConstitutionBuff":
						case "ArmorFactorBuff":
						case "ArmorAbsorbtionBuff":
						case "CombatSpeedBuff":
						case "MeleeDamageBuff":
						case "AcuityBuff":
						case "BodyResistBuff":
						case "ColdResistBuff":
						case "EnergyResistBuff":
						case "HeatResistBuff":
						case "MatterResistBuff":
						case "SpiritResistBuff":
						case "BodySpiritEnergyBuff":
						case "HeatColdMatterBuff":
						case "CrushSlashThrustBuff":
							{
								SpellLine spellline = SkillBase.GetSpellLine(GlobalSpellsLines.Mob_Spells);
								this.CastSpell(spell, spellline);
								return;
							}
					}
				}
			}
		}
		#endregion
		#region Notify

		public override void Notify(DOLEvent e, object sender, EventArgs args)
		{
			base.Notify(e, sender, args);

			ABrain brain = Brain;
			if (brain != null)
				brain.Notify(e, sender, args);
		}

		#endregion

		/// <summary>
		/// Constructs a NPC
		/// </summary>
		public GameNPC()
			: base()
		{
			m_brainSync = m_brains.SyncRoot;
			m_followTarget = new WeakRef(null);
			m_rider = new WeakRef(null);

			m_size = 50; //Default size
			m_targetX = 0;
			m_targetY = 0;
			m_followMinDist = 100;
			m_followMaxDist = 3000;
			m_flags = 0;
			//m_factionName = "";
			LinkedFactions = new ArrayList(1);
			m_ownBrain = new StandardMobBrain();
			m_ownBrain.Body = this;
		}
	}
}
