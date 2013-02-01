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

using DOL.GS.Commands;
using DOL.GS.ServerProperties;

namespace DOL.GS.Privilege
{
    [CmdAttribute(
        Cmd = "&privilegegroup",
        Aliases = new[]
            {
                "&modgrp",
                "&pgrp"
            },
        Level = (uint) ePrivLevel.Admin,
        Privilege = "privilege_mod",
        Description = "PrivilegeGroup.Command.Description",
        Usage = new string[]
            {
             
            })]
    public class PrivilegeGroupCommand : AbstractCommandHandler, ICommandHandler
    {
        public void OnCommand(GameClient client, string[] args)
        {
            if (!Properties.USE_NEW_PRIVILEGE_SYSTEM)
            {
                DisplayMessage(client, "This command is not available whilst using the legacy system.");
                return;
            }

            if (args.Length < 2)
            {
                DisplaySyntax(client);
                return;
            }


        }
    }
}
