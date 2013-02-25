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
using System.Collections.Generic;
using DOL.GS.Privilege.Attributes;

namespace DOL.GS.Privilege.ParameterizedBindings
{
    [ParameterizedPrivilege(Privilege = PrivilegeDefaults.Who, RequiredParameters = 2, OptionalParameters = true)]
    public class WhoBinding : ParameterizedPrivilegeBinding
    {
        private readonly Dictionary<string, string> m_searchAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public WhoBinding(string[] rawArguments) : base(rawArguments)
        {
            for (int i = 0; i < rawArguments.Length; i += 2)
            {
                if ((i + 1) < rawArguments.Length)
                    m_searchAliases.Add(rawArguments[i], rawArguments[i + 1]);
            }
        }

        /// <summary>
        /// Does this who binding have an index for the specified string?
        /// </summary>
        /// <param name="ind"></param>
        /// <returns></returns>
        public bool IsIndexedAs(string ind)
        {
            return m_searchAliases.ContainsKey(ind);
        }

        /// <summary>
        /// Get's an alias for the specified index.
        /// </summary>
        /// <param name="ind"></param>
        /// <returns></returns>
        public string GetAliasFor(string ind)
        {
            return m_searchAliases[ind];
        }
    }
}
