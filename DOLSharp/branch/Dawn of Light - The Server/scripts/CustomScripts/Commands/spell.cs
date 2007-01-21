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
using DOL.GS.PacketHandler;
using DOL.GS.Spells;
namespace DOL.GS.Scripts
{
	[CmdAttribute(
		"&spell",
		2,
		"cast a spell",
		"/spell <spellid>")]
	public class SpellCommandHandler : ICommandHandler
	{
		public int OnCommand(GameClient client, string[] args)
		{
			if (args.Length < 2)
			{
				client.Out.SendMessage("Usage: /spell <spellid>", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return 0;
			}
			ushort spellID = 0;
			try
			{
				spellID = Convert.ToUInt16(args[1]);
				Spell spell = SkillBase.GetSpellByID(spellID);
				ISpellHandler spellhandler = Scripts.ScriptMgr.CreateSpellHandler(client.Player, spell, SkillBase.GetSpellLine(GlobalSpellsLines.Reserved_Spells));
				GameObject obj = client.Player.TargetObject;
				GameLiving target = null;
				if (spell != null)
				{
					if (obj == null)
						target = client.Player;
					else if (obj is GameLiving)
						target = (GameLiving)obj;
					spellhandler.StartSpell(target);
					client.Out.SendMessage("Name: " + spell.Name + " Type: " + spell.SpellType + " Value: " + spell.Value + " Damage: " + spell.Damage + " Range: " + spell.Range + " Target: " + spell.Target, eChatType.CT_System, eChatLoc.CL_SystemWindow);
				}
				else { client.Out.SendMessage("Spell ID " + spellID + " not found!", eChatType.CT_System, eChatLoc.CL_SystemWindow); }
			}
			catch
			{
				client.Out.SendMessage("Usage: /spell <spellid>", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return 0;
			}
			return 1;
		}
	}
}