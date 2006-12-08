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
			list.Add(thidranki.Name + ": " + GlobalConstants.RealmToName((eRealm)thidranki.Realm));
			list.Add(orseo.Name + ": " + GlobalConstants.RealmToName((eRealm)orseo.Realm));
			list.Add("");
			list.Add("Darkness Falls: " + GlobalConstants.RealmToName((eRealm)orseo.Realm));

			client.Out.SendCustomTextWindow("Realm Status", list);
			return 1;
		}
	}
}