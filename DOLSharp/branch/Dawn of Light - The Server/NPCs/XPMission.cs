using System;
using DOL.Events;
using DOL.GS.Quests;
using DOL.GS.PacketHandler;
using DOL.GS.Keeps;

namespace DOL.GS.Scripts
{
	public class XPMissionMaster : GameNPC
	{
		public override bool AddToWorld()
		{
			Name = "Guard Commander";
			GuildName = "Task Master";
			Level = 60;
			switch (Realm)
			{
				case 0:
				case 1: Inventory = ClothingMgr.Albion_Lord.CloneTemplate(); break;
				case 2: Inventory = ClothingMgr.Midgard_Lord.CloneTemplate(); break;
				case 3: Inventory = ClothingMgr.Hibernia_Lord.CloneTemplate(); break;
			}
			switch (Realm)
			{
				case 0:
				case 1:
					{
						switch (Util.Random(0, 5))
						{
							case 0: Model = TemplateMgr.HighlanderMale; break;//Highlander Male
							case 1: Model = TemplateMgr.BritonMale; break;//Briton Male
							case 2: Model = TemplateMgr.AvalonianMale; break;//Avalonian Male
							case 3: Model = TemplateMgr.HighlanderFemale; break;//Highlander Female
							case 4: Model = TemplateMgr.BritonFemale; break;//Briton Female
							case 5: Model = TemplateMgr.AvalonianFemale; break;//Avalonian Female
						}
						break;
					}
				case 2:
					{
						switch (Util.Random(0, 7))
						{
							case 0: Model = TemplateMgr.DwarfMale; break;//Dwarf Male
							case 1: Model = TemplateMgr.NorseMale; break;//Norse Male
							case 2: Model = TemplateMgr.TrollMale; break;//Troll Male
							case 3: Model = TemplateMgr.KoboldMale; break;//Kobold Male
							case 4: Model = TemplateMgr.DwarfFemale; break;//Dwarf Female
							case 5: Model = TemplateMgr.NorseFemale; break;//Norse Female
							case 6: Model = TemplateMgr.TrollFemale; break;//Troll Female
							case 7: Model = TemplateMgr.KoboldFemale; break;//Kobold Female
						}
						break;
					}
				case 3:
					{
						switch (Util.Random(0, 7))
						{
							case 0: Model = TemplateMgr.CeltMale; break;//Celt Male
							case 1: Model = TemplateMgr.FirbolgMale; break;//Firbolg Male
							case 2: Model = TemplateMgr.LurikeenMale; break;//Lurikeen Male
							case 3: Model = TemplateMgr.ElfMale; break;//Elf Male
							case 4: Model = TemplateMgr.CeltFemale; break;//Celt Female
							case 5: Model = TemplateMgr.FirbolgFemale; break;//Firbolg Female
							case 6: Model = TemplateMgr.LurikeenFemale; break;//Lurikeen Female
							case 7: Model = TemplateMgr.ElfFemale; break;//Elf Female
						}
						break;
					}
			}
			return base.AddToWorld();
		}

		public override bool Interact(GamePlayer player)
		{
			if (!base.Interact(player))
				return false;

			SayTo(player, "Greetings " + player.Name + ", it's good to see adventurers willing to help the realm in such times, my job is to assign adventurers such as you with [personal] or [group] missions for which you will be compensated with experience.");

			return true;
		}

		public override bool WhisperReceive(GameLiving source, string str)
		{
			if (!base.WhisperReceive(source, str))
				return false;

			GamePlayer player = source as GamePlayer;
			if (player == null)
				return false;

			switch (str.ToLower())
			{
				case "personal":
					{
						if (player.Mission != null)
							player.Mission.ExpireMission();

						//find a npc within a con level
						GameNPC npc = player.CurrentZone.GetRandomNPCByCon(eRealm.None, player.Level, 0);
						if (npc != null)
						{
							//generate the mission
							player.Mission = new XPKillMission(npc.Name, player.Level, "monsters of the name " + npc.Name, player);
						}
						else
							SayTo(player, "Sorry, I don't have anything for your level!");
						break;
					}
				case "group":
					{
						SayTo(player, "This option is not yet ready!");
						return false;

						if (player.PlayerGroup == null)
						{
							SayTo(player, "You are not in a group!");
							break;
						}

						if (player.PlayerGroup.Leader != player)
						{
							SayTo(player, "You are not the leader of your group!");
							break;
						}

						if (player.PlayerGroup.Mission != null)
							player.PlayerGroup.Mission.ExpireMission();

						//find a npc within a con level
						int targetCon = 0;
						if (player.PlayerGroup.PlayerCount <= 4)
							targetCon = 1;
						else targetCon = 2;
						GameNPC npc = player.CurrentZone.GetRandomNPCByCon(eRealm.None, player.Level, targetCon);
						//generate the mission
						player.PlayerGroup.Mission = new XPKillMission(npc.Name, player.Level, "monsters of the name " + npc.Name, player);

						break;
					}
			}
			if (player.Mission != null)
				SayTo(player, player.Mission.Description);

			if (player.PlayerGroup != null && player.PlayerGroup.Mission != null)
				SayTo(player, player.PlayerGroup.Mission.Description);
			return true;
		}
	}

	public class XPKillMission : AbstractMission
	{
		private string m_targetName = "";
		private int m_total = 0;
		private int m_current = 0;
		private string m_desc = "";
		public XPKillMission(string targetName, int total, string desc, object owner)
			: base(owner)
		{
			m_targetName = targetName;
			m_total = total;
			m_desc = desc;
		}

		public override void Notify(DOLEvent e, object sender, EventArgs args)
		{
			if (e != GameLivingEvent.EnemyKilled)
				return;

			EnemyKilledEventArgs eargs = args as EnemyKilledEventArgs;

			//we don't want insta spawn npcs to count
			if (eargs.Target is GameNPC && (eargs.Target as GameNPC).RespawnInterval < 1000)
				return;

			if (eargs.Target.Name != m_targetName)
				return;

			//we dont allow events triggered by non group leaders
			if (MissionType == eMissionType.Group && sender is GamePlayer)
			{
				GamePlayer player = sender as GamePlayer;

				if (player.PlayerGroup == null)
					return;

				if (player.PlayerGroup.Leader != player)
					return;
			}

			//we don't want group events to trigger personal mission updates
			if (MissionType == eMissionType.Personal && sender is GamePlayer)
			{
				GamePlayer player = sender as GamePlayer;

				if (player.PlayerGroup != null)
					return;
			}

			m_current++;
			UpdateMission();
			if (m_current == m_total)
				FinishMission();

		}

		public override string Description
		{
			get
			{
				return "Kill " + m_total + " " + m_desc + ", you have killed " + m_current + ".";
			}
		}

		public override long RewardXP
		{
			get 
			{
				if (m_owner is GamePlayer)
				{
					return GameServer.ServerRules.GetExperienceForLiving((m_owner as GamePlayer).Level) * m_total;
				}
				else
				{
					return GameServer.ServerRules.GetExperienceForLiving((m_owner as GamePlayer).Level) * m_total;
				}
				return base.RewardXP; 
			}
		}

		public override long RewardMoney
		{
			get { return 0; }
		}

		public override long RewardRealmPoints
		{
			get { return 0; }
		}
	}
}