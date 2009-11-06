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
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using DOL.GS.Database;
using DOL.GS.PacketHandler;
using log4net;

namespace DOL.GS
{
	/// <summary>
	/// Fletching is the crafting skill to make fletch for archer
	/// </summary>
	public class Fletching : AbstractCraftingSkill
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Fletching()
		{
			Icon = 0x0C;
			Name = "Fletching";
			eSkill = eCraftingSkill.Fletching;
		}

		/// <summary>
		/// Check if  the player own all needed tools
		/// </summary>
		/// <param name="player">the crafting player</param>
		/// <param name="craftItemData">the object in construction</param>
		/// <returns>true if the player hold all needed tools</returns>
		public override bool CheckTool(GamePlayer player, CraftItemData craftItemData)
		{
			bool result = false;
			if(!(craftItemData.TemplateToCraft is AmmunitionTemplate))
			{
				foreach (GameCraftTool item in player.GetInRadius(typeof(GameCraftTool), CRAFT_DISTANCE))
				{
					if(item.ToolType == eCraftingToolType.Lathe) // Lathe (model = 481)
					{
						result = true;
						break;
					}
				}

				if(result == false)
				{
					player.Out.SendMessage("You do not have the tools to make the "+craftItemData.TemplateToCraft.Name+".",eChatType.CT_System,eChatLoc.CL_SystemWindow);
					player.Out.SendMessage("You must find a lathe!",eChatType.CT_System,eChatLoc.CL_SystemWindow);
					return false;
				}
			}

			bool needSmithHammer = false;
			foreach (RawMaterial material in craftItemData.RawMaterials)
			{
				if(material.MaterialTemplate.Model == 519) // metal bar
				{
					needSmithHammer = true;
					break;
				}
			}

			byte flags = 0;
			foreach (GenericItem item in player.Inventory.GetItemRange(eInventorySlot.FirstBackpack, eInventorySlot.LastBackpack))
			{
				if(!(item is CraftingTool)) continue;

				if(((CraftingTool)item).Type == eCraftingToolType.PlaningTool)
				{
					if((flags & 0x01) == 0) flags |= 0x01;
					if(!needSmithHammer || flags >= 0x03) break;
				}
				else if(needSmithHammer && ((CraftingTool)item).Type == eCraftingToolType.SmithHammer)
				{
					if((flags & 0x02) == 0) flags |= 0x02;
					if(flags >= 0x03) break;
				}
			}

			if((flags & 0x01) == 0)
			{
				player.Out.SendMessage("You do not have the tools to make the "+craftItemData.TemplateToCraft.Name+".",eChatType.CT_System,eChatLoc.CL_SystemWindow);
				player.Out.SendMessage("You must find a planing tool!",eChatType.CT_System,eChatLoc.CL_SystemWindow);
				return false;
			}

			if(needSmithHammer && (flags & 0x02) == 0)
			{
				player.Out.SendMessage("You do not have the tools to make the "+craftItemData.TemplateToCraft.Name+".",eChatType.CT_System,eChatLoc.CL_SystemWindow);
				player.Out.SendMessage("You must find a smith tool!",eChatType.CT_System,eChatLoc.CL_SystemWindow);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Calculate the minumum needed secondary crafting skill level to make the item
		/// </summary>
		public override int CalculateSecondCraftingSkillMinimumLevel(CraftItemData item)
		{
			if(item.TemplateToCraft is StaffTemplate)
			{
				return item.CraftingLevel - 35;
			}
			if(item.TemplateToCraft is RangedWeaponTemplate)
			{
				return item.CraftingLevel - 20;
			}
			if(item.TemplateToCraft is AmmunitionTemplate)
			{
				return item.CraftingLevel - 15;
			}

			return base.CalculateSecondCraftingSkillMinimumLevel(item);
		}

		/// <summary>
		/// Select craft to gain point and increase it
		/// </summary>
		/// <param name="player"></param>
		/// <param name="item"></param>
		public override void GainCraftingSkillPoints(GamePlayer player, CraftItemData item)
		{
			base.GainCraftingSkillPoints(player, item);

			if(player.CraftingPrimarySkill == eCraftingSkill.Fletching)
			{
				if(player.GetCraftingSkillValue(eCraftingSkill.Fletching)%100 == 99)
				{
					player.Out.SendMessage("You must see your trainer to raise your Fletching further!",eChatType.CT_Important,eChatLoc.CL_SystemWindow);
					return;
				}
			}
			else
			{
				int maxAchivableLevel;
				switch (player.CraftingPrimarySkill)
				{
					case eCraftingSkill.WeaponCrafting:
					{
						maxAchivableLevel = (int)(player.GetCraftingSkillValue(eCraftingSkill.WeaponCrafting) * 0.40);
						break;
					}

					case eCraftingSkill.Tailoring:
					{
						maxAchivableLevel = (int)(player.GetCraftingSkillValue(eCraftingSkill.Tailoring) * 0.75);
						break;
					}

					case eCraftingSkill.ArmorCrafting:
					{
						maxAchivableLevel = (int)(player.GetCraftingSkillValue(eCraftingSkill.ArmorCrafting) * 0.40);
						break;
					}

					default:
					{
						maxAchivableLevel = 0;
						break;
					}
				}

				if(player.GetCraftingSkillValue(eCraftingSkill.Fletching) >= maxAchivableLevel)
				{
					return;
				}
			}

			if(Util.Chance( CalculateChanceToGainPoint(player, item)))
			{
				player.IncreaseCraftingSkill(eCraftingSkill.Fletching, 1);
				player.Out.SendUpdateCraftingSkills();
			}
		}
	}
}