/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DOL.Database;
using System.Reflection;
using log4net;
using DOL.GS.PacketHandler;
using DOL.GS.Quests;
using DOL.GS.Scripts;

namespace DOL.GS
{
	/// <summary>
	/// The artifact manager.
	/// </summary>
	/// <author>Aredhel</author>
    public sealed class ArtifactMgr
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Hashtable m_artifacts;
        private static Hashtable m_artifactVersions;
        private static Hashtable m_artifactBooks;

        public enum Book { NoPage = 0x0, Page1 = 0x1, Page2 = 0x2, Page3 = 0x4, AllPages = 0x7 };


        public static bool Init()
        {
            return LoadArtifacts();
        }

        /// <summary>
        /// Load artifacts from the DB.
        /// </summary>
        public static bool LoadArtifacts()
        {
            DataObject[] dbo = GameServer.Database.SelectAllObjects(typeof(Artifact));
			m_artifacts = new Hashtable();
            foreach (Artifact artifact in dbo)
                m_artifacts.Add(artifact.ArtifactID, artifact);

            dbo = GameServer.Database.SelectAllObjects(typeof(ArtifactXItem));
            m_artifactVersions = new Hashtable();
            ArrayList versionList;
            foreach (ArtifactXItem artifactVersion in dbo)
            {
                versionList = (ArrayList)m_artifactVersions[artifactVersion.ArtifactID];
                if (versionList == null)
                {
                    versionList = new ArrayList();
                    m_artifactVersions.Add(artifactVersion.ArtifactID, versionList);
                }
                versionList.Add(artifactVersion);
            }

            dbo = GameServer.Database.SelectAllObjects(typeof(ArtifactBook));
            m_artifactBooks = new Hashtable();
            foreach (ArtifactBook artifactBook in dbo)
                m_artifactBooks[artifactBook.ArtifactID] = artifactBook;

            log.Info(String.Format("{0} artifacts ({1} versions) and {2} books loaded", 
				m_artifacts.Count, m_artifactVersions.Count, m_artifactBooks.Count));
            return true;
		}

        /// <summary>
        /// Find all artifacts from a particular zone.
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public static ArrayList GetArtifactsFromZone(String zone)
        {
            ArrayList artifacts = new ArrayList();

            if (zone != null)
                lock (m_artifacts.SyncRoot)
                    foreach (DictionaryEntry entry in m_artifacts)
                        if ((entry.Value as Artifact).Zone == zone)
                            artifacts.Add(entry.Value);

            return artifacts;
        }

		/// <summary>
		/// Get a list of all scholars studying this artifact.
		/// </summary>
		/// <param name="artifactID"></param>
		/// <returns></returns>
		public static String[] GetScholarsFromArtifactID(String artifactID)
		{
			String[] scholars = null;
			Artifact artifact = null;
			if (artifactID != null)
				lock (m_artifacts.SyncRoot)
					artifact = (Artifact)m_artifacts[artifactID];
			if (artifact != null)
				scholars = artifact.ScholarID.Split(';');
			return scholars;
		}

		#region Artifact Versions

		/// <summary>
        /// Get a list of all versions for this artifact.
        /// </summary>
        /// <param name="artifactID"></param>
        /// <returns></returns>
        private static ArrayList GetArtifactVersions(String artifactID)
        {
            ArrayList versions = null;
            if (artifactID != null)
            {
                lock (m_artifactVersions.SyncRoot)
                    versions = (ArrayList)m_artifactVersions[artifactID];
            }

            return (versions == null) ? new ArrayList() : versions;
        }

        /// <summary>
        /// Create a hashtable containing all item templates that are valid for
        /// this class.
        /// </summary>
        /// <param name="artifactID"></param>
        /// <param name="charClass"></param>
        /// <returns></returns>
        public static Hashtable GetArtifactVersionsFromClass(String artifactID, 
            eCharacterClass charClass)
        {
            if (artifactID == null)
                return null;

            ArrayList allVersions = GetArtifactVersions(artifactID);
            Hashtable classVersions = new Hashtable();

            lock (allVersions.SyncRoot)
            {
                ItemTemplate itemTemplate;
                foreach (ArtifactXItem version in allVersions)
                {
                    itemTemplate = (ItemTemplate)GameServer.Database.FindObjectByKey(typeof(ItemTemplate),
                        version.ItemID);

                    if (itemTemplate == null)
                    {
                        log.Warn(String.Format("Artifact item template '{0}' is missing",
                            version.ItemID));
                    }
                    else
                    {
                        String[] classIDs = itemTemplate.AllowedClasses.Split(';');
                        foreach (String classID in classIDs)
                        {
                            try
                            {
                                if (Int16.Parse(classID) == (int)charClass)
                                {
                                    classVersions.Add(version.Version, itemTemplate);
                                    break;
                                }
                            }
                            catch
                            {
                                log.Warn(String.Format("Invalid class ID '{0}' for item template '{1}'",
                                    classID, itemTemplate.Id_nb));
                            }
                        }
                    }
                }
            }

            return classVersions;
		}

		#endregion

		#region Encounters & Quests

		/// <summary>
		/// Get the quest type from the quest type string.
		/// </summary>
		/// <param name="questTypeString"></param>
		/// <returns></returns>
		public static Type GetQuestType(String questTypeString)
		{
			Type questType = null;
			foreach (Assembly asm in ScriptMgr.Scripts)
			{
				questType = asm.GetType(questTypeString);
				if (questType != null)
					break;
			}

			if (questType == null)
				questType = Assembly.GetAssembly(typeof(GameServer)).GetType(questTypeString);

			return questType;
		}

		/// <summary>
		/// Get the quest type for the encounter from the artifact ID.
		/// </summary>
		/// <param name="artifactID"></param>
		/// <returns></returns>
		public static Type GetEncounterType(String artifactID)
		{
			if (artifactID == null)
				return null;

			Artifact artifact = null;
			lock (m_artifacts.SyncRoot)
				artifact = (Artifact)m_artifacts[artifactID];

			return GetQuestType(artifact.EncounterID);
		}

		/// <summary>
		/// Grant credit for an artifact.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="artifactID"></param>
		/// <returns></returns>
		public static bool GrantArtifactCredit(GamePlayer player, String artifactID)
		{
			if (player == null || artifactID == null)
				return false;

			Artifact artifact;
			lock (m_artifacts.SyncRoot)
				artifact = (Artifact) m_artifacts[artifactID];

			if (artifact == null)
				return false;

			Type questType = GetQuestType(artifact.EncounterID);
			if (questType == null)
				return false;

			if (player.HasFinishedQuest(questType) > 0)
				return false;

			AbstractQuest quest = (AbstractQuest)(System.Activator.CreateInstance(questType,
				new object[] { player }));

			if (quest == null)
				return false;

			quest.FinishQuest();
			return true;
		}

		#endregion

		#region Scrolls & Books

        /// <summary>
        /// Find the matching artifact for this book.
        /// </summary>
        /// <param name="bookID"></param>
        /// <returns></returns>
        public static String GetArtifactIDFromBookID(String bookID)
        {
            if (bookID == null)
                return null;

            String artifactID = null;
            lock (m_artifactBooks.SyncRoot)
            {
                foreach (DictionaryEntry entry in m_artifactBooks)
                {
                    if ((entry.Value as ArtifactBook).BookID == bookID)
                    {
                        artifactID = (entry.Value as ArtifactBook).ArtifactID;
                        break;
                    }
                }
            }

            return artifactID;
        }

		/// <summary>
		/// Check whether these 2 items can be combined.
		/// </summary>
		/// <param name="item1"></param>
		/// <param name="item2"></param>
		/// <returns></returns>
		public static bool CanCombine(InventoryItem item1, InventoryItem item2)
		{
            String artifactID1 = null;
            Book pageNumbers1 = GetPageNumbers(item1, ref artifactID1);
            if (pageNumbers1 == Book.NoPage || pageNumbers1 == Book.AllPages)
                return false;

            String artifactID2 = null;
            Book pageNumbers2 = GetPageNumbers(item2, ref artifactID2);
            if (pageNumbers2 == Book.NoPage || pageNumbers2 == Book.AllPages)
                return false;

            if (artifactID1 != artifactID2 ||
                (Book)((int)pageNumbers1 & (int)pageNumbers2) != Book.NoPage)
                return false;

            return true;
		}

        /// <summary>
        /// Check which scroll pages are in this item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="artifactID"></param>
        /// <returns></returns>
        public static Book GetPageNumbers(InventoryItem item, ref String artifactID)
        {
            if (item.Object_Type != (int)eObjectType.Magical
                || item.Item_Type != (int)eInventorySlot.FirstBackpack)
                return Book.NoPage;

            lock (m_artifactBooks.SyncRoot)
            {
                ArtifactBook artifactBook;
                foreach (DictionaryEntry entry in m_artifactBooks)
                {
                    artifactBook = (ArtifactBook) entry.Value;
                    artifactID = artifactBook.ArtifactID;
                    if (item.Name == artifactBook.Scroll1)
                        return Book.Page1;
                    else if (item.Name == artifactBook.Scroll2)
                        return Book.Page2;
                    else if (item.Name == artifactBook.Scroll3)
                        return Book.Page3;
                    else if (item.Name == artifactBook.Scroll12)
                        return (Book)((int)Book.Page1 | (int)Book.Page2);
                    else if (item.Name == artifactBook.Scroll13)
                        return (Book)((int)Book.Page1 | (int)Book.Page3);
                    else if (item.Name == artifactBook.Scroll23)
                        return (Book)((int)Book.Page2 | (int)Book.Page3);
                    else if (item.Name == artifactBook.BookID)
                        return Book.AllPages;
                }
            }

            return Book.NoPage;
        }

		/// <summary>
		/// Find the artifact that matches this book.
		/// </summary>
		/// <param name="book"></param>
		/// <returns></returns>
		public static Artifact GetArtifactFromBook(InventoryItem book)
		{
			if (book == null)
				return null;

			String artifactID = null;
			if (GetPageNumbers(book, ref artifactID) != Book.AllPages)
				return null;

			if (artifactID == null)
				return null;

			lock (m_artifacts.SyncRoot)
				return (Artifact)m_artifacts[artifactID];
		}

        /// <summary>
        /// Whether or not the player has the complete book for this 
        /// artifact in his backpack.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="artifactID"></param>
        /// <returns></returns>
        public static bool HasBookForArtifact(GamePlayer player, String artifactID)
        {
			if (player == null || artifactID == null)
				return false;

			String bookID;
			lock (m_artifactBooks.SyncRoot)
			{
				ArtifactBook artifactBook = (ArtifactBook)m_artifactBooks[artifactID];
				if (artifactBook == null)
				{
					log.Warn(String.Format("Can't find book for artifact \"{0}\"", artifactID));
					return false;
				}
				bookID = artifactBook.BookID;
			}

			ICollection backpack = player.Inventory.GetItemRange(eInventorySlot.FirstBackpack,
				eInventorySlot.LastBackpack);
			foreach (InventoryItem item in backpack)
			{
				if (item.Object_Type == (int)eObjectType.Magical &&
					item.Item_Type == (int)eInventorySlot.FirstBackpack &&
					item.Name == bookID)
					return true;
			}

			return false;
        }

		/// <summary>
		/// Whether or not the item is an artifact scroll.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static bool IsArtifactScroll(InventoryItem item)
		{
            String artifactID = null;
            Book pageNumbers = GetPageNumbers(item, ref artifactID);
            return (pageNumbers != Book.NoPage && pageNumbers != Book.AllPages);
		}

		/// <summary>
		/// Combine 2 scrolls.
		/// </summary>
		/// <param name="scroll1"></param>
		/// <param name="scroll2"></param>
		/// <returns></returns>
		public static GameInventoryItem CombineScrolls(InventoryItem scroll1, InventoryItem scroll2)
		{
			if (!CanCombine(scroll1, scroll2))
				return null;

            String artifactID = null;
            Book combinedPages = (Book)((int)GetPageNumbers(scroll1, ref artifactID) |
                (int)GetPageNumbers(scroll2, ref artifactID));

            return CreatePages(artifactID, combinedPages);
		}

        /// <summary>
        /// Create a scroll or book containing the given page numbers.
        /// </summary>
        /// <param name="artifactID"></param>
        /// <param name="pageNumbers"></param>
        /// <returns></returns>
        private static GameInventoryItem CreatePages(String artifactID, Book pageNumbers)
        {
            if (artifactID == null || pageNumbers == Book.NoPage)
                return null;

            ArtifactBook artifactBook;
            lock (m_artifactBooks.SyncRoot)
                artifactBook = (ArtifactBook)m_artifactBooks[artifactID];

            if (artifactBook == null)
                return null;

            GameInventoryItem scroll = GameInventoryItem.CreateFromTemplate("artifact_scroll");
            if (scroll == null)
                return null;

            String scrollTitle = null;
			int scrollModel = 499;
            switch (pageNumbers)
            {
                case Book.Page1: 
					scrollTitle = artifactBook.Scroll1;
					scrollModel = artifactBook.ScrollModel1;
                    break;
                case Book.Page2: 
					scrollTitle = artifactBook.Scroll2;
					scrollModel = artifactBook.ScrollModel1;
                    break;
                case Book.Page3: 
					scrollTitle = artifactBook.Scroll3;
					scrollModel = artifactBook.ScrollModel1;
                    break;
                case (Book)((int)Book.Page1 | (int)Book.Page2): 
					scrollTitle = artifactBook.Scroll12;
					scrollModel = artifactBook.ScrollModel2;
                    break;
                case (Book)((int)Book.Page1 | (int)Book.Page3): 
					scrollTitle = artifactBook.Scroll13;
					scrollModel = artifactBook.ScrollModel2;
                    break;
                case (Book)((int)Book.Page2 | (int)Book.Page3): 
					scrollTitle = artifactBook.Scroll23;
					scrollModel = artifactBook.ScrollModel2;
                    break;
                case Book.AllPages: 
					scrollTitle = artifactBook.BookID;
					scroll.Item.Model = artifactBook.BookModel;
                    break;
            }

            scroll.Name = scrollTitle;
            scroll.Item.Name = scrollTitle;
			scroll.Model = (ushort)scrollModel;
			scroll.Item.Model = (ushort)scrollModel;
            return scroll;
        }

		/// <summary>
        /// Create a scroll from a particular book.
        /// </summary>
        /// <param name="artifactID">The artifact's ID.</param>
        /// <param name="pageNumber">Scroll page number (1-3).</param>
        /// <returns>An item that can be picked up by a player (or null).</returns>
        public static GameInventoryItem CreateScroll(String artifactID, int pageNumber)
        {
			if (pageNumber < 1 || pageNumber > 3)
				return null;

            switch (pageNumber)
            {
                case 1: return CreatePages(artifactID, Book.Page1);
                case 2: return CreatePages(artifactID, Book.Page2);
                case 3: return CreatePages(artifactID, Book.Page3);
            }

            return null;
        }

		#endregion
    }
}
