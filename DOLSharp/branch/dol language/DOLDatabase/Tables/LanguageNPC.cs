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

using DOL.Database.Attributes;

namespace DOL.Database
{
    public class DBLanguageNPC : DataObject
    {
        private string m_id;
        private string m_translationUnique;
        private string m_examineArticle;
        private string m_messageArticleFemale;
        private string m_messageArticleMale;
        private string m_name;
        private string m_guildName;
        private string m_language;

        public DBLanguageNPC() { }

        /// <summary>
        /// The translation id
        /// </summary>
        [DataElement(AllowDbNull = false)]
        public string TranslationId
        {
            get { return m_id; }
            set
            {
                Dirty = true;
                m_id = value;
            }
        }

        /// <summary>
        /// The translation unique to get the correct translation
        /// </summary>
        [DataElement(AllowDbNull = true)]
        public string TranslationUnique
        {
            get { return m_translationUnique; }
            set
            {
                Dirty = true;
                m_translationUnique = value;
            }
        }

        /// <summary>
        /// The translated npc examine article:
        /// 
        /// You examine the apple tree.
        /// The apple tree yells: Stop stealing my apples!
        /// 
        /// You examine an apple tree.
        /// An apple tree yells: Stop stealing my apples!
        /// 
        /// You examine the bandit.
        /// The bandit yells: Stop touching my wife!
        /// 
        /// You examine an bandit.
        /// An bandit yells: Stop touching my wife!
        /// 
        /// You examine the bandit.
        /// The bandit yells: Stop touching me!
        /// 
        /// You examine an bandit.
        /// An bandit yells: Stop touching me!
        /// </summary>
        [DataElement(AllowDbNull = true)]
        public string ExamineArticle
        {
            get { return m_examineArticle; }
            set
            {
                Dirty = true;
                m_examineArticle = value;
            }
        }

        /// <summary>
        /// The translated npc female message article:
        /// 
        /// English: Apo has just killed by an Armswoman.
        /// German : Apo wurde gerade getötet, eine Waffenmeisterin war stärker.
        /// </summary>
        [DataElement(AllowDbNull = true)]
        public string MessageArticleFemale
        {
            get { return m_messageArticleFemale; }
            set
            {
                Dirty = true;
                m_messageArticleFemale = value;
            }
        }

        /// <summary>
        /// The translated npc male message article
        /// 
        /// English: Apo hast just killed by an Armsmen.
        /// German : Apo wurde gerade getötet, ein Waffenmeister war stärker.
        /// </summary>
        [DataElement(AllowDbNull = true)]
        public string MessageArticleMale
        {
            get { return m_messageArticleMale; }
            set
            {
                Dirty = true;
                m_messageArticleMale = value;
            }
        }

        /// <summary>
        /// The translated npc name
        /// </summary>
        [DataElement(AllowDbNull = false)]
        public string Name
        {
            get { return m_name; }
            set
            {
                Dirty = true;
                m_name = value;
            }
        }

        /// <summary>
        /// The translated npc guild name
        /// </summary>
        [DataElement(AllowDbNull = true)]
        public string GuildName
        {
            get { return m_guildName; }
            set
            {
                Dirty = true;
                m_guildName = value;
            }
        }

        /// <summary>
        /// The language
        /// </summary>
        [DataElement(AllowDbNull = false)]
        public string Language
        {
            get { return m_language; }
            set
            {
                Dirty = true;
                m_language = value;
            }
        }
    }
}