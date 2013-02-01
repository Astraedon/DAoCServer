
using System.Text.RegularExpressions;

namespace DOL.GS.Privilege
{
    /// <summary>
    /// Holds the default privilege keys used by the privilege system.
    /// </summary>
    public static class PrivilegeDefaults
    {
        /// <summary>
        /// Regex for easy discovery if a raw privilege contains a set of parameters.
        /// </summary>
        public static readonly Regex ParameterizedRegex = new Regex(@"(.+)\(([a-zA-Z|]+)\)");

        /// <summary>
        /// Grants any and all permissions to a player with this, with great 
        /// power comes great responsibility.
        /// </summary>
        public const string Wildcard = "*";

        /// <summary>
        /// Prefix for all commands that are indexed under the default privileges.
        /// </summary>
        public const string CommandPrefix = "cmd_";


        #region Default Privilege Keys

        #region Legacy Privilege Levels

        /// <summary>
        /// Key that allows the privileges system to return plvl 1 for groups.
        /// </summary>
        public const string LegacyPlayer = "plvl_player";

        /// <summary>
        /// Key that allows the privileges system to return plvl 2 for groups.
        /// </summary>
        public const string LegacyGM = "plvl_gm";

        /// <summary>
        /// Key that allows the privileges system to return plvl 3 for groups.
        /// </summary>
        public const string LegacyAdministrator = "plvl_admin";

        #endregion

        /// <summary>
        /// Key that tells the server that this player has staff privileges for the 
        /// purposes of command logging.
        /// </summary>
        public const string Staff = "staff";

        /// <summary>
        /// Key telling the server that trades performed by this player should be logged.
        /// </summary>
        public const string LogTrades = "log_trades";

        /// <summary>
        /// Key telling the server that commands being used by this player should be logged as GM Actions.
        /// </summary>
        public const string LogAllCommands = "log_all_commands";

        /// <summary>
        /// Key telling the server that a player with this flag should be ignored by mobs and guards for the purposes of aggro.
        /// </summary>
        public const string NeutralPlayer = "neutral";

        /// <summary>
        /// Key telling the server this player can ignore overencumbered speed slowdowns.
        /// </summary>
        public const string StrongBack = "strong_back";

        /// <summary>
        /// Key telling the server this player doesn't take fall damage.
        /// </summary>
        public const string SafeFall = "safe_fall";

        /// <summary>
        /// Key telling the server this player is invisible (Don't show it's model and have it stealthed?)
        /// </summary>
        public const string Invisible = "invisible";

        /// <summary>
        /// Key telling the server to multiply all damage done by this player to instagib their player targets.
        /// </summary>
        public const string InstakillPlayers = "instagib_player";

        /// <summary>
        /// Key telling the server to multiply all damage done by this player to instagib their non-player targets.
        /// </summary>
        public const string InstakillMobs = "instagib_mob";

        /// <summary>
        /// Key telling the server that this player is invulnerable.
        /// </summary>
        public const string Invulnerable = "invulnerable";

        #endregion

        #region Lanaguage Manager Prefixes

        public const string ErrorPrefix = "Privilege.Error.";
        public const string GroupErrorPrefix = "PrivilegeGroup.Error.";
        public const string GainPrefix = "Privilege.Notify.Gain.";
        public const string LostPrefix = "Privilege.Notify.Lost.";
        public const string GrantPrefix = "Privilege.Notify.Grant.";
        public const string RevokePrefix = "Privilege.Notify.Revoke.";
        public const string ReceivedPrefix = "Privilege.Notify.Received.";
        public const string RevokedPrefix = "Privilege.Notify.Revoked.";

        #endregion
    }
}
