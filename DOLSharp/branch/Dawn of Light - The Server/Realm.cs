using System.Collections;
using DOL.GS.Keeps;
using DOL.GS.PacketHandler;

namespace DOL.GS.Scripts
{
	[CmdAttribute(
		"&realm",
		1,
		"Show realm status", "/realm")]
	public class RealmCommandHandler : AbstractCommandHandler, ICommandHandler 
	{
		private const int NUMBER_KEEPS = 7;
		private const int NUMBER_TOWERS = NUMBER_KEEPS * 4;

		public int OnCommand(GameClient client, string[] args)
		{
			AbstractGameKeep thidranki = KeepMgr.getKeepByID(129);

			ArrayList list = new ArrayList();
			list.Add("Frontier Status:");
			foreach (string str in GenerateFrontierStatus())
				list.Add(str);
			list.Add("");
			list.Add("Battlegrounds Status:");
			list.Add(GenerateKeepLine(thidranki));
			list.Add(GetPlayersLine(thidranki.Region));
			list.Add("");
			list.Add("Darkness Falls: " + GlobalConstants.RealmToName(ServerRules.DFEnterJumpPoint.DarknessFallOwner));
			list.Add(GetPlayersLine(249));

			client.Out.SendCustomTextWindow("Realm Status", list);
			return 1;
		}

		private static string GenerateKeepLine(AbstractGameKeep keep)
		{
			string guild = keep.Guild == null ? "" : " (" + keep.Guild.Name + ")";
			string siege = "";
			if (keep.InCombat)
				siege = " (Under Siege)";
			return keep.Name + ": " + GlobalConstants.RealmToName((eRealm)keep.Realm) + guild + siege;
		}

		private static string GetPlayersLine(int regionID)
		{
			int alb = 0, mid = 0, hib = 0;
			alb = WorldMgr.GetClientsOfRegionCount((ushort)regionID, 1);
			mid = WorldMgr.GetClientsOfRegionCount((ushort)regionID, 2);
			hib = WorldMgr.GetClientsOfRegionCount((ushort)regionID, 3);
			return "";
			return "Players: Alb (" + alb + ") Mid (" + mid + ") Hib (" + hib + ") Total (" + (alb + mid + hib) + ")";
		}

		private static ArrayList GenerateFrontierStatus()
		{
			ArrayList list = new ArrayList();
			for (int i = 1; i <= 3; i++)
			{
				string msg = "";
				eRealm realm = (eRealm)i;
				int towersbyrealm = KeepMgr.GetTowerCountByRealm(realm);
				int towernum = towersbyrealm - NUMBER_TOWERS;
				string towers = towernum.ToString();
				if (towernum > 0)
					towers = "+" + towers;
				int keepsbyrealm = KeepMgr.GetKeepCountByRealm(realm);
				int keepnum = keepsbyrealm - NUMBER_KEEPS;
				string keeps = keepnum.ToString();
				if (keepnum > 0)
					keeps = "+" + keeps;

				msg += GlobalConstants.RealmToName(realm) + ": Towers " + towersbyrealm + " (" + towers + ") Keeps " + keepsbyrealm + " (" + keeps + ")" + " Keep Balance: (" + KeepMgr.GetRealmBonusLevel(realm) + ")";
				list.Add(msg);
			}
			return list;
		}
	}
}