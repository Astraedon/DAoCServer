
namespace DOL.GS.Privilege
{
    /// <summary>
    /// Holds the default privilege keys used by the privilege system.
    /// </summary>
    public static class DefaultPrivileges
    {
        /// <summary>
        /// Grants any and all permissions to a player with this, with great 
        /// power comes great responsibility.
        /// </summary>
        public const string Wildcard = "*";

        /// <summary>
        /// Prefix for all commands that are indexed under the default privileges.
        /// </summary>
        public const string CommandPrefix = "cmd_";

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
    }
}
