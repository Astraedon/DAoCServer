namespace DOL.GS.ServerRules
{
	/// <summary>
	/// Set of rules for "normal" server type.
	/// </summary>
	[ServerRules(eGameServerType.GST_Normal)]
	public class DOLServerRules : NormalServerRules
	{
		public override string GetPlayerGuildName(GamePlayer source, GamePlayer target)
		{
			return target.GuildName;
		}
	}
}