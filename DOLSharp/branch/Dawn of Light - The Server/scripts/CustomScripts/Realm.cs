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
			//AbstractGameKeep orseo = KeepMgr.getKeepByID(5);
			AbstractGameKeep leirvik = KeepMgr.getKeepByID(134);
			AbstractGameKeep molvik = KeepMgr.getKeepByID(132);

			ArrayList list = new ArrayList();
			int alb = 0, mid = 0, hib = 0;
			list.Add(thidranki.Name + ": " + GlobalConstants.RealmToName((eRealm)thidranki.Realm));
			if (thidranki.InCombat)
				list.Add("Under Siege");
			alb = WorldMgr.GetClientsOfRegionCount((ushort)thidranki.Region, 1);
			mid = WorldMgr.GetClientsOfRegionCount((ushort)thidranki.Region, 2);
			hib = WorldMgr.GetClientsOfRegionCount((ushort)thidranki.Region, 3);
			list.Add("Players: Alb (" + alb + ") Mid (" + mid + ") Hib (" + hib + ") Total (" + (alb + mid + hib) + ")");

			list.Add(molvik.Name + ": " + GlobalConstants.RealmToName((eRealm)molvik.Realm));
			if (molvik.InCombat)
				list.Add("Under Siege");
			alb = WorldMgr.GetClientsOfRegionCount((ushort)molvik.Region, 1);
			mid = WorldMgr.GetClientsOfRegionCount((ushort)molvik.Region, 2);
			hib = WorldMgr.GetClientsOfRegionCount((ushort)molvik.Region, 3);
			list.Add("Players: Alb (" + alb + ") Mid (" + mid + ") Hib (" + hib + ") Total (" + (alb + mid + hib) + ")");


			string guildName = leirvik.Guild == null ? "" : " (" + leirvik.Guild.Name + ")";
			list.Add(leirvik.Name + ": " + GlobalConstants.RealmToName((eRealm)leirvik.Realm) + guildName);
			if (leirvik.InCombat)
				list.Add("Under Siege");
			alb = WorldMgr.GetClientsOfRegionCount((ushort)leirvik.Region, 1);
			mid = WorldMgr.GetClientsOfRegionCount((ushort)leirvik.Region, 2);
			hib = WorldMgr.GetClientsOfRegionCount((ushort)leirvik.Region, 3);
			list.Add("Players: Alb (" + alb + ") Mid (" + mid + ") Hib (" + hib + ") Total (" + (alb + mid + hib) + ")");

			list.Add("");
			list.Add("Darkness Falls: " + GlobalConstants.RealmToName((eRealm)leirvik.Realm));
			alb = WorldMgr.GetClientsOfRegionCount(249, 1);
			mid = WorldMgr.GetClientsOfRegionCount(249, 2);
			hib = WorldMgr.GetClientsOfRegionCount(249, 3);
			list.Add("Players: Alb (" + alb + ") Mid (" + mid + ") Hib (" + hib + ") Total (" + (alb + mid + hib) + ")");

			client.Out.SendCustomTextWindow("Realm Status", list);
			return 1;
		}
	}
}