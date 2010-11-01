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
    public class DBLanguageDataQuest : DataObject
    {
        private string m_id;
        private string m_name;
        private string m_acceptText;
        private string m_description;
        private string m_sourceText;
        private string m_stepText;
        private string m_advanceText;
        private string m_targetText;
        private string m_finishText;

        public DBLanguageDataQuest() { }

        public string TranslationId
        {
            get { return m_id; }
            set
            {
                Dirty = true;
                m_id = value;
            }
        }

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
        /// The translated name of the quest
        /// </summary>
        public string AcceptText
        {
            get { return m_acceptText; }
            set
            {
                Dirty = true;
                m_acceptText = value;
            }
        }

        public string Description
        {
            get { return m_description; }
            set
            {
                Dirty = true;
                m_description = value;
            }
        }

        public string SourceText
        {
            get { return m_sourceText; }
            set
            {
                Dirty = true;
                m_sourceText = value;
            }
        }

        public string StepText
        {
            get { return m_stepText; }
            set
            {
                Dirty = true;
                m_stepText = value;
            }
        }

        public string AdvanceText
        {
            get { return m_advanceText; }
            set
            {
                Dirty = true;
                m_advanceText = value;
            }
        }

        public string TargetText
        {
            get { return m_targetText; }
            set
            {
                Dirty = true;
                m_targetText = value;
            }
        }

        public string FinishText
        {
            get { return m_finishText; }
            set
            {
                Dirty = true;
                m_finishText = value;
            }
        }
    }
}
