using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using DOL.Events;
using DOL.Database;
using DOL.GS.Utils;
using log4net;

namespace DOL.GS.Scripts
{
	public class LootGeneratorParchment : LootGeneratorBase
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		[ScriptLoadedEvent]
		public static void OnScriptLoaded(DOLEvent e, object sender, EventArgs args)
		{
			LootMgr.RegisterLootGenerator(new LootGeneratorParchment(), "", "", "", 249);
			log.Info("Parchment Generator Loaded!");
		}

		private static ItemTemplate m_parchment = null;
		public static ItemTemplate Parchment
		{
			get
			{
				if (m_parchment == null)
				{
					m_parchment = new ItemTemplate();
					m_parchment.Id_nb = "piece_of_parchment";
					m_parchment.Name = "pieces of parchment";
					m_parchment.Level = 0;
					m_parchment.Durability = 100;
					m_parchment.MaxDurability = 100;
					m_parchment.Condition = 100;
					m_parchment.MaxCondition = 100;
					m_parchment.Quality = 0;
					m_parchment.Object_Type = (int)eObjectType.Magical;
					m_parchment.Weight = 1;
					m_parchment.Copper = 50;
					m_parchment.MaxCount = 100;
					m_parchment.PackSize = 1;
					m_parchment.Realm = 0;
					m_parchment.Model = 603;
				}
				return m_parchment;
			}
		}

		public override LootList GenerateLoot(GameNPC mob, GameObject killer)
		{
			LootList loot = base.GenerateLoot(mob, killer);

			int chance = mob.Level;

			if (Util.Chance(chance))
				loot.AddFixed(Parchment);

			/*if (mob.Inventory != null) //if the mob has items he is humanoid in most cases
			{
				//int chance = (int)(100 / (GameServer.ServerRules.GetExperienceForLevel(killer.Level) / mob.ExperienceValue));

			}*/

			return loot;
		}
	}
}