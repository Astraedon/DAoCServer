using System;
using System.Collections.Generic;
using System.Reflection;

using DOL.Events;
using DOL.GS.Behaviour;
using DOL.GS.Behaviour.Actions;
using DOL.GS.Behaviour.Triggers;
using DOL.GS.Movement;
using DOL.Database;

using log4net;

namespace DOL.GS.Scripts
{
	public class CityCrierEvent
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		// list of all registered behaviours
		private static List<BaseBehaviour> behaviours = new List<BaseBehaviour>();

		private const int SPEED = 50;
		private const int SHOUT_INTERVAL = 20000;

		private const string TIP_PROPERTY = "TIP_PROPERTY";

		private const bool LOAD_STARTER_CRIERS = true;
		private const bool LOAD_CAPITAL_CRIERS = false;

		private static List<GameNPC> criersList = new List<GameNPC>();
		private static RegionTimer timer;

		private static string[] TIPS = new string[] 
{
	"Welcome to the Official Dawn of Light XP RvR server, we hope you enjoy your stay!",
	"The Experience Rate is currently set to " + ServerProperties.Properties.XP_RATE + "x of standard!",
	"You are allowed to create characters in all realms using the same account!",
	"You can change the client language with /language command!",
	"Hackers are automatically banned!",
	"Every " + ServerProperties.Properties.FREELEVEL_DAYS + " days, provided you have gained a normal level, you can gain a free level !",
	"All damage vs NPC's is increased by " + ServerProperties.Properties.PVE_DAMAGE + "x of standard!",
	"All new players have 2 free full respecs and realm respecs credited to their account!",
	"A Unique Object Generator (ROG) is applied to all mobs across the server, this gives the chance for any mob to drop randomly generated equipment!",
	"A Utility Scroll system is applied to all mobs across the server, this gives the chance for any mob to drop a scroll which has various functions from summoning a trainer to creating a teleporter!",
	"There are respec merchants in the capital cities!",
	"There are item restorers in the capital cities, if an item is changed in a patch, hand the item to the NPC and he will give you a new one!",
	"There are teleporters placed in most major towns, these teleporters can instantly teleport you around the realm!",
	"RvR is in Thidranki for sub 50, and Cathal Valley and the entire New Frontiers for post 50, use teleporters in the capital cities to go there!",
	"A Parchment system is applied to all mobs in Darkness Falls, these pieces of parchment are special currency for a merchant in the capital cities!",
	"Generic Trainers are special NPC's which allow's all classes to use them, these can be summoned from scrolls!",
	"Extension NPC's are availiable in the border keeps of each realm, for bounty points they will add armor extensions to your equipment!",
	"A Horse Merchant is availiable in the border keeps of each realm, for bounty points players can buy a player controlled horse and armor!",
	"A special Keep Balance System is in effect for the frontiers, this means that the more you expand your realms keeps, the weaker they will get, and the stronger the enemies will get!",
	"You can use /ircon to set your in game IRC status to on, to allow you to send and receive IRC messages from the servers channel!",
	"Keep an eye on our forums and herald on http://www.dolserver.net!",

};

		[ScriptLoadedEvent]
		public static void ScriptLoaded(DOLEvent e, object sender, EventArgs args)
		{
			GameNpcInventoryTemplate crierTemplate = new GameNpcInventoryTemplate();
			crierTemplate.AddNPCEquipment(eInventorySlot.TwoHandWeapon, 228);
			crierTemplate.AddNPCEquipment(eInventorySlot.HandsArmor, 137, 0, 0, 1);
			crierTemplate.AddNPCEquipment(eInventorySlot.TorsoArmor, 51, 0, 0, 2);
			crierTemplate.AddNPCEquipment(eInventorySlot.Cloak, 669, 0, 0, 0);
			crierTemplate.AddNPCEquipment(eInventorySlot.LegsArmor, 135, 0, 0, 1);
			crierTemplate.CloseTemplate();

			#region NPC's
			//camelot crier
			GameNPC camelotCrier = null;
			if (LOAD_CAPITAL_CRIERS)
			{
				camelotCrier = new GameNPC();
				camelotCrier.Model = 40;
				camelotCrier.Name = "Camelot Crier";
				camelotCrier.GuildName = "Crier";
				camelotCrier.Realm = (byte)1;
				camelotCrier.CurrentRegionID = 10;
				camelotCrier.Size = 50;
				camelotCrier.Level = 50;
				camelotCrier.MaxSpeedBase = SPEED;
				camelotCrier.X = 34623;
				camelotCrier.Y = 24308;
				camelotCrier.Z = 8512;
				camelotCrier.Heading = 2989;
				camelotCrier.Inventory = crierTemplate.CloneTemplate();
				camelotCrier.AddToWorld();
				camelotCrier.SwitchWeapon(GameLiving.eActiveWeaponSlot.TwoHanded);
				criersList.Add(camelotCrier);
			}

			if (LOAD_STARTER_CRIERS)
			{
				//cotswold crier
				GameNPC cotswoldCrier = new GameNPC();
				cotswoldCrier.Model = 40;
				cotswoldCrier.Name = "Cotswold Crier";
				cotswoldCrier.GuildName = "Crier";
				cotswoldCrier.Realm = (byte)1;
				cotswoldCrier.CurrentRegionID = 1;
				cotswoldCrier.Size = 50;
				cotswoldCrier.Level = 50;
				cotswoldCrier.MaxSpeedBase = SPEED;
				cotswoldCrier.X = 561875;
				cotswoldCrier.Y = 512488;
				cotswoldCrier.Z = 2597;
				cotswoldCrier.Heading = 2613;
				cotswoldCrier.Inventory = crierTemplate.CloneTemplate();
				cotswoldCrier.AddToWorld();
				cotswoldCrier.SwitchWeapon(GameLiving.eActiveWeaponSlot.TwoHanded);
				criersList.Add(cotswoldCrier);

				//mularn crier
				GameNPC mularnCrier = new GameNPC();
				mularnCrier.Model = 157;
				mularnCrier.Name = "Mularn Crier";
				mularnCrier.GuildName = "Crier";
				mularnCrier.Realm = (byte)2;
				mularnCrier.CurrentRegionID = 100;
				mularnCrier.Size = 50;
				mularnCrier.Level = 50;
				mularnCrier.MaxSpeedBase = SPEED;
				mularnCrier.X = 802457;
				mularnCrier.Y = 726307;
				mularnCrier.Z = 4764;
				mularnCrier.Heading = 3283;
				mularnCrier.Inventory = crierTemplate.CloneTemplate();
				mularnCrier.AddToWorld();
				mularnCrier.SwitchWeapon(GameLiving.eActiveWeaponSlot.TwoHanded);
				criersList.Add(mularnCrier);

				//magmell crier
				GameNPC magmellCrier = new GameNPC();
				magmellCrier.Model = 157;
				magmellCrier.Name = "Mag Mell Crier";
				magmellCrier.GuildName = "Crier";
				magmellCrier.Realm = (byte)3;
				magmellCrier.CurrentRegionID = 200;
				magmellCrier.Size = 50;
				magmellCrier.Level = 50;
				magmellCrier.MaxSpeedBase = SPEED;
				magmellCrier.X = 347938;
				magmellCrier.Y = 490700;
				magmellCrier.Z = 5220;
				magmellCrier.Heading = 3224;
				magmellCrier.Inventory = crierTemplate.CloneTemplate();
				magmellCrier.AddToWorld();
				magmellCrier.SwitchWeapon(GameLiving.eActiveWeaponSlot.TwoHanded);
				criersList.Add(magmellCrier);
			}
			#endregion

			#region pathing
			if (LOAD_CAPITAL_CRIERS)
			{
				//camelot crier
				List<PathPoint> camelotCrierPoints = new List<PathPoint>();
				camelotCrierPoints.Add(new PathPoint(34623, 24308, 8512, SPEED, ePathType.Loop));
				camelotCrierPoints.Add(new PathPoint(34412, 24318, 8464, SPEED, ePathType.Loop));
				camelotCrierPoints.Add(new PathPoint(33511, 24355, 8464, SPEED, ePathType.Loop));

				PathPoint lastPoint = null;
				foreach (PathPoint point in camelotCrierPoints)
				{
					//associate next and previous
					if (lastPoint != null)
					{
						point.Prev = lastPoint;
						lastPoint.Next = point;
					}
					lastPoint = point;
				}
				camelotCrierPoints[camelotCrierPoints.Count - 1].Next = camelotCrierPoints[0];
				camelotCrierPoints[0].Prev = camelotCrierPoints[camelotCrierPoints.Count - 1];

				camelotCrier.CurrentWayPoint = camelotCrierPoints[0];
				camelotCrier.MoveOnPath(100);

			}
			#endregion

			timer = new RegionTimer(WorldMgr.GetRegion(1).TimeManager);
			timer.Callback = new RegionTimerCallback(ShoutCallback);
			timer.Interval = SHOUT_INTERVAL;
			timer.Start(SHOUT_INTERVAL);

			log.Info("City criers started...");
		}

		[ScriptUnloadedEvent]
		public static void ScriptUnloaded(DOLEvent e, object sender, EventArgs args) 
		{
			timer.Stop();
			timer = null;
			foreach (GameNPC npc in criersList)
				npc.Delete();
		}

		public static int ShoutCallback(RegionTimer timer)
		{
			foreach (GameNPC crier in criersList)
			{
				if (crier.CurrentRegionID == 10
					|| crier.CurrentRegionID == 101
					|| crier.CurrentRegionID == 201)
				{
					Character c = (Character)GameServer.Database.SelectObject(typeof(Character), "`Realm` = '" + crier.Realm + "' ORDER BY `RealmPoints` DESC");
					if (c != null)
					{
						crier.Say("Regent Etaew sends his regards and wishes to congratulate " + c.Name + " on being the leader of the realm with " + c.RealmPoints + " realm points!");
					}
				}
				else
				{
					int tipNo = timer.Properties.getIntProperty(TIP_PROPERTY, 0);
					crier.Say(TIPS[tipNo]);
					tipNo++;
					if (tipNo > TIPS.Length - 1)
						tipNo = 0;
					timer.Properties.setProperty(TIP_PROPERTY, tipNo);
				}
			}
			return SHOUT_INTERVAL;
		}
	}
}