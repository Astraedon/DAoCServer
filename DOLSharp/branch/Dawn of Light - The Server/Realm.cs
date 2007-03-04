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
		public int OnCommand(GameClient client, string[] args)
		{
			AbstractGameKeep thidranki = KeepMgr.getKeepByID(129);
			AbstractGameKeep leirvik = KeepMgr.getKeepByID(134);
			AbstractGameKeep leirvikAlbTower = KeepMgr.getKeepByID(1414);
			AbstractGameKeep leirvikMidTower = KeepMgr.getKeepByID(1158);
			AbstractGameKeep leirvikHibTower = KeepMgr.getKeepByID(1670);

			ArrayList list = new ArrayList();
			list.Add(GenerateKeepLine(thidranki));
			list.Add(GetPlayersLine(thidranki.Region));
			list.Add("");
			list.Add(GenerateKeepLine(leirvik));
			list.Add(GenerateKeepLine(leirvikAlbTower));
			list.Add(GenerateKeepLine(leirvikMidTower));
			list.Add(GenerateKeepLine(leirvikHibTower));
			list.Add(GetPlayersLine(leirvik.Region));
			list.Add("");
			list.Add("Darkness Falls: " + GlobalConstants.RealmToName((eRealm)leirvik.Realm));
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
			return "Players: Alb (" + alb + ") Mid (" + mid + ") Hib (" + hib + ") Total (" + (alb + mid + hib) + ")";
		}
	}
}