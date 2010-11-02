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
#define NOENCRYPTION
using System;
using System.Reflection;

using DOL.Language;
using DOL.GS.Housing;
using DOL.GS.PlayerTitles;
using log4net;

namespace DOL.GS.PacketHandler
{
	[PacketLib(179, GameClient.eClientVersion.Version179)]
	public class PacketLib179 : PacketLib178
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Constructs a new PacketLib for Version 1.79 clients
		/// </summary>
		/// <param name="client">the gameclient this lib is associated with</param>
		public PacketLib179(GameClient client):base(client)
		{
		}

		public override void SendUpdatePlayer()
		{
			GamePlayer player = m_gameClient.Player;
			if (player == null)
				return;
			
			GSTCPPacketOut pak = new GSTCPPacketOut(GetPacketCode(eServerPackets.VariousUpdate));
			pak.WriteByte(0x03); //subcode
			pak.WriteByte(0x0f); //number of entry
			pak.WriteByte(0x00); //subtype
			pak.WriteByte(0x00); //unk
			//entry :

			pak.WriteByte(player.GetDisplayLevel(m_gameClient.Player)); //level
			pak.WritePascalString(player.Name); // player name
			pak.WriteByte((byte) (player.MaxHealth >> 8)); // maxhealth high byte ?
            if (player.Gender > 0)
                pak.WritePascalString(LanguageMgr.GetTranslation(m_gameClient, eTranslationKey.SystemText, player.CharacterClass.Name,
                    "PlayerClass" + player.CharacterClass.Name.Trim() + "Female")); // class name
            else
                pak.WritePascalString(LanguageMgr.GetTranslation(m_gameClient, eTranslationKey.SystemText, player.CharacterClass.Name,
                    "PlayerClass" + player.CharacterClass.Name.Trim() + "Male")); // class name
			pak.WriteByte((byte) (player.MaxHealth & 0xFF)); // maxhealth low byte ?
			pak.WritePascalString(LanguageMgr.GetTranslation(m_gameClient, eTranslationKey.SystemText, /*"The "+*/player.CharacterClass.Profession, "")); // Profession
			pak.WriteByte(0x00); //unk
            if (player.Gender > 0)
                pak.WritePascalString(LanguageMgr.GetTranslation(m_gameClient, eTranslationKey.SystemText, player.CharacterClass.GetTitle(player.Level),
                    "ClassLevelTitle" + player.CharacterClass.GetTitle(player.Level).Trim() + "Female"));
            else
                pak.WritePascalString(LanguageMgr.GetTranslation(m_gameClient, eTranslationKey.SystemText, player.CharacterClass.GetTitle(player.Level),
                    "ClassLevelTitle" + player.CharacterClass.GetTitle(player.Level).Trim() + "Male"));
			//todo make function to calcule realm rank
			//client.Player.RealmPoints
			//todo i think it s realmpoint percent not realrank
			pak.WriteByte((byte) player.RealmLevel); //urealm rank
            if (player.Gender > 0)
                pak.WritePascalString(LanguageMgr.GetTranslation(m_gameClient, eTranslationKey.SystemText, player.RealmTitle,
                    "RealmTitle" + player.RealmTitle.Trim() + "Female")); // Realm title
            else
                pak.WritePascalString(LanguageMgr.GetTranslation(m_gameClient, eTranslationKey.SystemText, player.RealmTitle,
                    "RealmTitle" + player.RealmTitle.Trim() + "Male")); // Realm title
			pak.WriteByte((byte) player.RealmSpecialtyPoints); // realm skill points
            if (player.Gender > 0)
                pak.WritePascalString(LanguageMgr.GetTranslation(m_gameClient, eTranslationKey.SystemText, player.CharacterClass.BaseName,
                    "BaseClass" + player.CharacterClass.BaseName.Trim() + "Female")); // base class
            else
                pak.WritePascalString(LanguageMgr.GetTranslation(m_gameClient, eTranslationKey.SystemText, player.CharacterClass.BaseName,
                    "BaseClass" + player.CharacterClass.BaseName.Trim() + "Male")); // base class
			pak.WriteByte((byte)(HouseMgr.GetHouseNumberByPlayer(player) >> 8)); // personal house high byte
			pak.WritePascalString(player.GuildName); // Guild name
			pak.WriteByte((byte)(HouseMgr.GetHouseNumberByPlayer(player) & 0xFF)); // personal house low byte
			pak.WritePascalString(player.LastName); // Last name
			pak.WriteByte((byte)(player.MLLevel+1)); // ML Level (+1)
            if (player.Gender > 0)
                pak.WritePascalString(LanguageMgr.GetTranslation(m_gameClient, eTranslationKey.SystemText, player.RaceName,
                    "PlayerRace" + player.RaceName.Trim() + "Female")); // Race name
            else
                pak.WritePascalString(LanguageMgr.GetTranslation(m_gameClient, eTranslationKey.SystemText, player.RaceName,
                    "PlayerRace" + player.RaceName.Trim() + "Male")); // Race name
			pak.WriteByte(0x0);
			
			if (player.GuildRank != null)
				pak.WritePascalString(player.GuildRank.Title); // Guild title
			else
				pak.WritePascalString("");
			pak.WriteByte(0x0);

			AbstractCraftingSkill skill = CraftingMgr.getSkillbyEnum(player.CraftingPrimarySkill);
            if (skill != null)
                pak.WritePascalString(LanguageMgr.GetTranslation(m_gameClient, eTranslationKey.SystemText, skill.Name,
                        "CrafterGuild" + skill.Name.Trim())); //crafter guilde: alchemist
            else
                pak.WritePascalString(""); //no craft skill at start

			pak.WriteByte(0x0);
            pak.WritePascalString(LanguageMgr.GetTranslation(m_gameClient, eTranslationKey.SystemText, player.CraftTitle, "CrafterTitle" + player.CraftTitle.Trim())); //crafter title: legendary alchemist
			pak.WriteByte(0x0);
            if(player.ML > 0)
                pak.WritePascalString(LanguageMgr.GetTranslation(m_gameClient, eTranslationKey.SystemText, player.MLTitle,
                "MasterTitle" + player.MLTitle.Trim())); //ML title || Apo: No Female/Male translations for this.
            else
                pak.WritePascalString("");

			// new in 1.75
			pak.WriteByte(0x0);
            if (player.CurrentTitle != PlayerTitleMgr.ClearTitle)
            {
                if(player.Gender > 0)
                    pak.WritePascalString(LanguageMgr.GetTranslation(m_gameClient, eTranslationKey.SystemText, player.CurrentTitle.GetValue(player),
                        "PlayerTitle" + player.CurrentTitle.GetDescription(player).Trim() + "Female")); // new in 1.74 - Custom title
                else
                    pak.WritePascalString(LanguageMgr.GetTranslation(m_gameClient, eTranslationKey.SystemText, player.CurrentTitle.GetValue(player),
                        "PlayerTitle" + player.CurrentTitle.GetDescription(player).Trim() + "Male")); // new in 1.74 - Custom title
            }
            else
                pak.WritePascalString(LanguageMgr.GetTranslation(m_gameClient, eTranslationKey.SystemText, "None", "None")); 

			// new in 1.79
			if(player.Champion)
				pak.WriteByte((byte)(player.ChampionLevel+1)); // Champion Level (+1)
			else
				pak.WriteByte(0x0);
			pak.WritePascalString(LanguageMgr.GetTranslation(m_gameClient, eTranslationKey.SystemText, player.CLTitle,
                "ChampTitle" + player.CLTitle.Trim())); // Champion Title || Apo: Not sure if champion titles have it's own translation for female/male
			SendTCP(pak);
		}

		public override void SendUpdatePoints()
		{
			if (m_gameClient.Player == null)
				return;
			GSTCPPacketOut pak = new GSTCPPacketOut(GetPacketCode(eServerPackets.CharacterPointsUpdate));
			pak.WriteInt((uint)m_gameClient.Player.RealmPoints);
			pak.WriteShort(m_gameClient.Player.LevelPermill);
			pak.WriteShort((ushort) m_gameClient.Player.SkillSpecialtyPoints);
			pak.WriteInt((uint)m_gameClient.Player.BountyPoints);
			pak.WriteShort((ushort) m_gameClient.Player.RealmSpecialtyPoints);
			pak.WriteShort(m_gameClient.Player.ChampionLevelPermill);
			SendTCP(pak);
		}
	}
}
