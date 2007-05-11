using System;
using DOL.Events;
using DOL.Database;
using DOL.Database.Attributes;

namespace DOL.Database
{
	[DataTable(TableName = "Crafters")]
	public class DBCrafter : DataObject
	{
		private string m_characterID;
		private int m_skillType;
		private int m_skillAmount;

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
		public int SkillType
		{
			get { return m_skillType; }
			set
			{
				Dirty = true;
				m_skillType = value;
			}
		}

		[DataElement(AllowDbNull = false)]
		public int SkillAmount
		{
			get { return m_skillAmount; }
			set
			{
				Dirty = true;
				m_skillAmount = value;
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
	public class CraftingListMgr
	{
		[ScriptLoadedEvent]
		public static void OnScriptLoaded(DOLEvent e, object sender, EventArgs args)
		{
			GameServer.Database.RegisterDataObject(typeof(DBCrafter));
			GameServer.Database.LoadDatabaseTable(typeof(DBCrafter));

			GameEventMgr.AddHandler(GamePlayerEvent.Quit, new DOLEventHandler(PlayerQuit));
		}

		[ScriptUnloadedEvent]
		public static void OnScriptUnloaded(DOLEvent e, object sender, EventArgs args)
		{
			GameEventMgr.RemoveHandler(GamePlayerEvent.Quit, new DOLEventHandler(PlayerQuit));
		}

		public static void PlayerQuit(DOLEvent e, object sender, EventArgs args)
		{ 
			GamePlayer player = sender as GamePlayer;
			if (player == null)
				return;

			UpdateEntry(player);
		}

		private static void UpdateEntry(GamePlayer player)
		{
			if (player.CraftingPrimarySkill == eCraftingSkill.NoCrafting)
				return;

			if (player.GetCraftingSkillValue(player.CraftingPrimarySkill) < 100)
				return;

			bool create = false;
			DBCrafter crafter = (DBCrafter)GameServer.Database.SelectObject(typeof(DBCrafter), "`CharacterID` = '" + player.PlayerCharacter.ObjectId + "'");
			if (crafter == null)
			{
				crafter = new DBCrafter();
				create = true;
			}
			crafter.SkillType = (int)player.CraftingPrimarySkill;
			crafter.SkillAmount = player.GetCraftingSkillValue(player.CraftingPrimarySkill);

			if (create)
				GameServer.Database.AddNewObject(crafter);
			else GameServer.Database.SaveObject(crafter);
		}
	}
}