using System;
using System.Reflection;
using DOL.Events;
using log4net;

namespace DOL.GS.ServerRules
{
	/// <summary>
	/// Set of rules for "normal" server type.
	/// </summary>
	[ServerRules(eGameServerType.GST_Normal)]
	public class DOLServerRules : NormalServerRules
	{
		public const int BG_KEEP_ID = 130;
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public override string GetPlayerGuildName(GamePlayer source, GamePlayer target)
		{
			return target.GuildName;
		}


		[ScriptLoadedEvent]
		public static void OnScriptLoaded(DOLEvent e, object sender, EventArgs args)
		{
			Keeps.AbstractGameKeep keep = Keeps.KeepMgr.getKeepByID(BG_KEEP_ID);
			log.Debug("Changing " + keep.Name + " base level to 50");
			keep.BaseLevel = 50;
			foreach (Keeps.GameKeepGuard guard in keep.Guards.Values)
			{
				Keeps.TemplateMgr.SetGuardLevel(guard);
			}

			log.Debug("Changing mob levels in " + keep.CurrentZone.Description);
			foreach (GameNPC npc in keep.CurrentZone.GetNPCsOfZone(DOL.GS.PacketHandler.eRealm.None))
			{
				if (npc is Keeps.GameKeepGuard)
					continue;
				npc.Level = (byte)(npc.Level + Util.Random(10, 35));
			}
		}
	}
}