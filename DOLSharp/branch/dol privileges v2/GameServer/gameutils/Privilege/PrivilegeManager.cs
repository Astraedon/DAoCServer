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
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using DOL.Database;
using DOL.Events;
using log4net;

namespace DOL.GS.Privilege
{
    public static class PrivilegeManager
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static IDictionary<int, PrivilegeGroup> _groupCache;

        #region Initialization

        #region Load Cache

        /// <summary>
        /// Caches the DBPrivilegeGroup from the database to PrivilegeGroups within
        /// the server's memory for quick lookup and management.
        /// </summary>
        [GameServerStartedEvent]
        public static void LoadGroupCache(DOLEvent e, object sender, EventArgs arguments)
        {
            if (ServerProperties.Properties.USE_NEW_PRIVILEGE_SYSTEM)
            {
                Log.Info("[Privilege Manager] Loading Groups Cache.");
                UpdateDefaults();

                _groupCache = new Dictionary<int, PrivilegeGroup>();

                IList<PrivilegeGroup> tmpGroups = 
                    GameServer.Database.SelectAllObjects<DBPrivilegeGroup>().Select(dbGrp => new PrivilegeGroup(dbGrp)).ToList();

                foreach (PrivilegeGroup privilegeGroup in tmpGroups)
                {
                    privilegeGroup.Initialize();
                    _groupCache.Add(privilegeGroup.DBEntry.GroupIndex, privilegeGroup);
                }

                foreach (PrivilegeGroup privilegeGroup in tmpGroups)
                {
                    privilegeGroup.ResolveGroups();
                }

                foreach (PrivilegeGroup privGrp in tmpGroups.Where(privGrp => privGrp.HasCircularInheritanceChain()))
                {
                    Log.Error("[Privilege Manager] Group has Circular Inheritance Chain -> " + privGrp.DBEntry.Name);
                    _groupCache.Remove(privGrp.DBEntry.GroupIndex);
                }
            }
        }

        #endregion

        #region Insert Default Groups

        private static readonly DBPrivilegeGroup PlayerPrivilege = new DBPrivilegeGroup
        {
            GroupIndex = 1,
            Name = "player",
            DisplayName = "Player",
            Privileges = "plvl_player"
        };
        private static readonly DBPrivilegeGroup GameMasterPrivilege = new DBPrivilegeGroup
        {
            GroupIndex = 2,
            Name = "gm",
            DisplayName = "Gamemaster",
            Privileges = "plvl_gm"
        };
        private static readonly DBPrivilegeGroup AdministratorPrivilege = new DBPrivilegeGroup
        {
            GroupIndex = 3,
            Name = "admin",
            DisplayName = "Administrator",
            Commands = "*",
            Privileges = "plvl_admin;*"
        };

        private static void UpdateDefaults()
        {
            if (!ServerProperties.Properties.USE_NEW_PRIVILEGE_SYSTEM) return;

            bool modified = false;

            if (GameServer.Database.SelectObject<DBPrivilegeGroup>("GroupIndex = '1'") == null)
            {
                GameServer.Database.AddObject(PlayerPrivilege);
                modified = true;
            }

            if (GameServer.Database.SelectObject<DBPrivilegeGroup>("GroupIndex = '2'") == null)
            {
                GameServer.Database.AddObject(GameMasterPrivilege);
                modified = true;
            }

            if (GameServer.Database.SelectObject<DBPrivilegeGroup>("GroupIndex = '3'") == null)
            {
                GameServer.Database.AddObject(AdministratorPrivilege);
                modified = true;
            }

            if (modified)
                Log.Info("Adding Default Privilege Groups.");
        }

        #endregion

        #endregion

        #region Get PrivilegeGroup

        /// <summary>
        /// Lookup a privilege group from the cache by group ID.
        /// </summary>
        /// <param name="id">ID to find, null if not found.</param>
        /// <returns></returns>
        public static PrivilegeGroup GetGroupFromID(ushort id)
        {
            return _groupCache.ContainsKey(id) ? _groupCache[id] : null;
        }

        /// <summary>
        /// Lookup a privilege group from the cache by group Name.
        /// </summary>
        /// <param name="name">Name to use to look for the group.</param>
        /// <returns></returns>
        public static PrivilegeGroup GetGroupFromName(string name)
        {
            return _groupCache.Values.FirstOrDefault(pg => pg.DBEntry.Name == name);
        }

        #endregion

        #region Get DBPrivilegeGroup

        /// <summary>
        /// Gets a DBPrivilegeBinding for an account, if it cannot find one it will 
        /// create one and add it to the database for you. This should only be used
        /// by the system itself to set up the privileges, otherwise you should access
        /// the privilege bindings on GameClient or GamePlayer.
        /// </summary>
        /// <param name="acct">Account to get binding for.</param>
        /// <returns></returns>
        public static DBPrivilegeBinding GetBindingForAccount(Account acct)
        {
            DBPrivilegeBinding binding = GameServer.Database.SelectObject<DBPrivilegeBinding>
                ("(Identifier ='" + GameServer.Database.Escape(acct.Name) + "')");

            if (binding == null)
            {
                binding = new DBPrivilegeBinding
                {
                    Identifier = acct.Name,
                    Groups = acct.PrivLevel + ""
                };

                GameServer.Database.AddObject(binding);
                GameServer.Database.SaveObject(binding);
            }

            return binding;
        }

        /// <summary>
        /// Retrieve privilege binding for an individual player via GamePlayer object.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static DBPrivilegeBinding GetDBBindingForPlayer(GamePlayer player)
        {
            DBPrivilegeBinding binding = GetDBBindingForAcctPlayer(player.Client.Account.Name, player.Name);

            if (binding == null)
            {
                binding = new DBPrivilegeBinding
                {
                    Identifier = player.DBCharacter.AccountName + "|" + player.Name
                };

                GameServer.Database.AddObject(binding);
                GameServer.Database.SaveObject(binding);
            }


            return binding;
        }

        /// <summary>
        /// Gets a DBPrivilegeBinding for a player by account name + player name.
        /// </summary>
        /// <param name="acctName">Account Name to use for lookup.</param>
        /// <param name="playerName">Player Name to use for lookup.</param>
        /// <returns></returns>
        public static DBPrivilegeBinding GetDBBindingForAcctPlayer(string acctName, string playerName)
        {
            return GameServer.Database.SelectObject<DBPrivilegeBinding>("Identifier = '" + acctName + "|" + playerName + "'");
        }

        #endregion

        #region Legacy Support

        /// <summary>
        /// Get target's privilege level under legacy system.
        /// 
        /// This shouldn't be used anywhere that isn't already in the server
        /// as it relies upon defaults privileges built into the server.
        /// </summary>
        /// <param name="target">The target's privilege level under the legacy system.</param>
        /// <returns></returns>
        public static uint AsPrivilegeLevel(this GameClient cli)
        {
            if (cli.Player != null)
            {
                if (cli.Player.PlayerPrivileges.HasPrivilege("plvl_admin")) return 3;
                if (cli.Player.PlayerPrivileges.HasPrivilege("plvl_gm")) return 2;
                if (cli.Player.PlayerPrivileges.HasPrivilege("plvl_player")) return 1;
            }

            if (cli.AccountPrivileges.HasPrivilege("plvl_admin")) return 3;
            if (cli.AccountPrivileges.HasPrivilege("plvl_gm")) return 2;
            if (cli.AccountPrivileges.HasPrivilege("plvl_player")) return 1;
            return 0;
        }

        /// <summary>
        /// Checks if a logged on GameClient can use the specified command.
        /// 
        /// Checking order is Legacy (Plvl) > Command Privilege Group > Cmd Privilege
        /// </summary>
        /// <param name="client">Client to check for, factors in player privileges.</param>
        /// <param name="command">GameCommand to check for.</param>
        /// <param name="pars">Argument line for command.</param>
        /// <returns></returns>
        public static bool CanUseCommand(this GameClient client, ScriptMgr.GameCommand command, string[] pars)
        {
            string rawCmd = pars[0].Substring(1, pars[0].Length - 1);

            if (ServerProperties.Properties.USE_NEW_PRIVILEGE_SYSTEM)
            {
                return
                    client.Account.PrivLevel >= command.m_lvl ||
                    HasPrivilege(client.Player, "cmd_" + rawCmd) ||
                    (command.m_privilege != null && HasPrivilege(client.Player, command.m_privilege));

            }

            if (client.Account.PrivLevel < command.m_lvl)
            {
                if (!SinglePermission.HasPermission(client.Player, rawCmd))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        /// <summary>
        /// Checks if a specific target has a privilege under the specified key.
        /// 
        /// Order of checking is Account -> Player so if the privilege is not on the player
        /// account then we check the player and after that theres nothing because theres
        /// only two stages of checks.
        /// </summary>
        /// <param name="target">Target of the check.</param>
        /// <param name="privilegeKey">Privilege key to search for.</param>
        /// <returns></returns>
        public static bool HasPrivilege(this GamePlayer target, string privilegeKey)
        {
            if (!privilegeKey.StartsWith("cmd_") && !ServerProperties.Properties.USE_NEW_PRIVILEGE_SYSTEM) throw new PrivilegeException("Cannot check for special privileges with legacy system.");

            PrivilegeBinding acctPriv = target.Client.AccountPrivileges;
            PrivilegeBinding playerPriv = target.PlayerPrivileges;

            return acctPriv.HasPrivilege(privilegeKey.ToLower()) || playerPriv.HasPrivilege(privilegeKey.ToLower());
        }
    }
}
