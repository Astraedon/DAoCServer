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
			AbstractGameKeep orseo = KeepMgr.getKeepByID(5);

			ArrayList list = new ArrayList();
			int count = 0;
			list.Add(thidranki.Name + ": " + GlobalConstants.RealmToName((eRealm)thidranki.Realm));
			count = WorldMgr.GetClientsOfRegionCount(thidranki.CurrentRegion.ID);
			list.Add(count + " players.");
			list.Add(orseo.Name + ": " + GlobalConstants.RealmToName((eRealm)orseo.Realm));
			count = WorldMgr.GetClientsOfRegionCount(orseo.CurrentRegion.ID);
			list.Add(count + " players.");
			list.Add("");
			list.Add("Darkness Falls: " + GlobalConstants.RealmToName((eRealm)orseo.Realm));
			count = WorldMgr.GetClientsOfRegionCount(249);
			list.Add(count + " players.");

			client.Out.SendCustomTextWindow("Realm Status", list);
			return 1;
		}
	}
}