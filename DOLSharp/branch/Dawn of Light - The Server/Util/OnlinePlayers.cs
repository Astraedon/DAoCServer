using System;
using DOL.Events;
using DOL.Database;
using DOL.Database.Attributes;

namespace DOL.Database
{
	[DataTable(TableName = "OnlinePlayers")]
	public class DBOnlinePlayer : DataObject
	{
		private string m_characterID;
		private DateTime m_loginTime;

		[PrimaryKey]
		public string CharacterID
		{
			get { return m_characterID; }
			set
			{
				Dirty = true;
				m_characterID = value;
			}
		}

		[DataElement(AllowDbNull = false)]
		public DateTime LoginTime
		{
			get { return m_loginTime; }
			set
			{
				Dirty = true;
				m_loginTime = value;
			}
		}

		public override bool AutoSave
		{
			get { return true; }
			set { }
		}

	}
}

namespace DOL.GS.Scripts
{
	public class OnlinePlayerMgr
	{
		[ScriptLoadedEvent]
		public static void OnScriptLoaded(DOLEvent e, object sender, EventArgs args)
		{
			GameServer.Database.RegisterDataObject(typeof(DBOnlinePlayer));
			GameServer.Database.LoadDatabaseTable(typeof(DBOnlinePlayer));

			DBOnlinePlayer[] players = (DBOnlinePlayer[])GameServer.Database.SelectAllObjects(typeof(DBOnlinePlayer));
			foreach (DBOnlinePlayer player in players)
				GameServer.Database.DeleteObject(player);

			GameEventMgr.AddHandler(GamePlayerEvent.GameEntered, new DOLEventHandler(PlayerEnter));
			GameEventMgr.AddHandler(GamePlayerEvent.Quit, new DOLEventHandler(PlayerQuit));
		}

		[ScriptUnloadedEvent]
		public static void OnScriptUnloaded(DOLEvent e, object sender, EventArgs args)
		{
			GameEventMgr.RemoveHandler(GamePlayerEvent.GameEntered, new DOLEventHandler(PlayerEnter));
			GameEventMgr.RemoveHandler(GamePlayerEvent.Quit, new DOLEventHandler(PlayerQuit));
		}

		public static void PlayerEnter(DOLEvent e, object sender, EventArgs args)
		{
			GamePlayer player = sender as GamePlayer;
			if (player == null)
				return;

			DBOnlinePlayer dbp = new DBOnlinePlayer();
			dbp.CharacterID = player.PlayerCharacter.ObjectId;
			dbp.LoginTime = DateTime.Now;
			GameServer.Database.AddNewObject(dbp);
		}

		public static void PlayerQuit(DOLEvent e, object sender, EventArgs args)
		{ 
			GamePlayer player = sender as GamePlayer;
			if (player == null)
				return;

			DataObject dbp = GameServer.Database.FindObjectByKey(typeof(DBOnlinePlayer), player.PlayerCharacter.ObjectId);
			if (dbp != null)
				GameServer.Database.DeleteObject(dbp);
		}
	}
}