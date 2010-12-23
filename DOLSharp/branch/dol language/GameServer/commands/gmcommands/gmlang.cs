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

using DOL.Database;
using DOL.Language;
using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
    [CmdAttribute(
        "&gmlang",
        ePrivLevel.GM,
        "Translation commands of the new language system.",
        "Use '/gmlang setexamine <translation> to set an translated examine article for your current target.",
        "Use '/gmlang setguild <translation> to set an translated guild name for your current target.",
        "Use '/gmlang setmessage <translation> to set an translated message article for your current target.",
        "Use '/gmlang setname <translation> to set an translated name for your current target.",
        "Use '/gmlang setsuffix <translation> to set an translated name suffix for your current target."
        )]
    public class GMLangCommandHandler : AbstractCommandHandler, ICommandHandler
    {
        #region OnCommand
        public void OnCommand(GameClient client, string[] args)
        {
            if (!ServerProperties.Properties.USE_NEW_LANGUAGE_SYSTEM)
            {
                client.Out.SendMessage("The new language system is disabled.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            if (client.Account.Language == "EN")
            {
                client.Out.SendMessage("You are using the default server language. Please change your language to the language you want to add an translations for.",
                    eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            if (args.Length < 2)
            {
                DisplaySyntax(client);
                return;
            }

            if (args.Length < 3)
            {
                switch (args[1].ToLower())
                {
                    case "setexamine":
                        IncorrectFormat(client, "setexamine");
                        break;
                    case "setguild":
                        IncorrectFormat(client, "setguild");
                        break;
                    case "setmessage":
                        IncorrectFormat(client, "setmessage");
                        break;
                    case "setname":
                        IncorrectFormat(client, "setname");
                        break;
                    case "setsuffix":
                        IncorrectFormat(client, "setsuffix");
                        break;
                    default:
                        DisplaySyntax(client);
                        break;
                }

                return;
            }

            if ((client.Player.TargetObject is WorldInventoryItem)) // WorldInventoryItem translations are not supported (yet).
            {
                client.Out.SendMessage("WorldInventoryItem translations are not supported.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            switch (args[1].ToLower())
            {
                case "setexamine":
                    SetExamine(client, args);
                    return;
                case "setguild":
                    SetGuild(client, args);
                    return;
                case "setmessage":
                    SetMessage(client, args);
                    return;
                case "setname":
                    SetName(client, args);
                    return;
                case "setsuffix":
                    SetSuffix(client, args);
                    return;
            }

            DisplaySyntax(client);
        }
        #endregion OnCommand

        #region OnCommand methods
        #region SetExamine
        private void SetExamine(GameClient client, string[] args)
        {
            if (client.Player.TargetObject == null)
            {
                YouMustSelect(client);
                return;
            }

            if (client.Player.TargetObject is GameMovingObject || client.Player.TargetObject is GameStaticItem)
            {
                if (client.Player.TargetObject is GameMine || client.Player.TargetObject is WorldInventoryItem)
                {
                    InvalidTarget(client);
                    return;
                }

                client.Out.SendMessage("Not supported yet.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            if (client.Player.TargetObject is GameNPC)
            {
                GameNPC target = client.Player.TargetObject as GameNPC;

                if (Util.IsEmpty(target.TranslationId))
                {
                    NoTranslationId(client);
                    return;
                }

                var dbo = GameServer.Database.SelectObjects<DBLanguageNPC>("TranslationId = '" + GameServer.Database.Escape(target.TranslationId) + "' " +
                                                                           "AND Language = '" + GameServer.Database.Escape(client.Account.Language) + "'");

                DBLanguageNPC row;
                if (dbo.Count > 0)
                    row = dbo[0]; // Select the first index
                else
                {
                    row = new DBLanguageNPC();
                    row.TranslationId = target.TranslationId;
                    row.Language = client.Account.Language;
                }

                if (args.Length > 3)
                    row.ExamineArticle = String.Join(" ", args, 2, args.Length - 2);
                else
                    row.ExamineArticle = args[2];

                if (dbo.Count > 0)
                {
                    for (int i = 0; i < dbo.Count; i++)
                        GameServer.Database.DeleteObject(dbo[i]); // It makes really no sense when you store 2+ rows into the table for one translation in one language
                }

                GameServer.Database.AddObject(row);

                target.RemoveFromWorld();
                target.RefreshTranslationData(client.Account.Language);
                target.AddToWorld();

                TranslationSetTo(client, "examine article", args[2]);
                return;
            }

            InvalidTarget(client);
        }
        #endregion SetExamine

        #region SetGuild
        private void SetGuild(GameClient client, string[] args)
        {
            if (client.Player.TargetObject == null)
            {
                YouMustSelect(client);
                return;
            }

            if (client.Player.TargetObject is GameNPC)
            {
                if (client.Player.TargetObject is GameMovingObject)
                {
                    InvalidTarget(client);
                    return;
                }

                GameNPC target = client.Player.TargetObject as GameNPC;

                if (Util.IsEmpty(target.TranslationId))
                {
                    NoTranslationId(client);
                    return;
                }

                var dbo = GameServer.Database.SelectObjects<DBLanguageNPC>("TranslationId = '" + GameServer.Database.Escape(target.TranslationId) + "' " +
                                                                           "AND Language = '" + GameServer.Database.Escape(client.Account.Language) + "'");

                DBLanguageNPC row;
                if (dbo.Count > 0)
                    row = dbo[0]; // Select the first index
                else
                {
                    row = new DBLanguageNPC();
                    row.TranslationId = target.TranslationId;
                    row.Language = client.Account.Language;
                }

                if (args.Length > 3)
                    row.GuildName = String.Join(" ", args, 2, args.Length - 2);
                else
                    row.GuildName = args[2];

                if (dbo.Count > 0)
                {
                    for (int i = 0; i < dbo.Count; i++)
                        GameServer.Database.DeleteObject(dbo[i]); // It makes really no sense when you store 2+ rows into the table for one translation in one language
                }

                GameServer.Database.AddObject(row);

                target.RemoveFromWorld();
                target.RefreshTranslationData(client.Account.Language);
                target.AddToWorld();

                TranslationSetTo(client, "guild name", args[2]);
                return;
            }

            InvalidTarget(client);
        }
        #endregion SetGuild

        #region SetMessage
        private void SetMessage(GameClient client, string[] args)
        {
            if (client.Player.TargetObject == null)
            {
                YouMustSelect(client);
                return;
            }

            if (client.Player.TargetObject is GameMovingObject || client.Player.TargetObject is GameStaticItem)
            {
                if (client.Player.TargetObject is GameMine || client.Player.TargetObject is WorldInventoryItem)
                {
                    InvalidTarget(client);
                    return;
                }

                client.Out.SendMessage("Not supported yet.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            if (client.Player.TargetObject is GameNPC)
            {
                GameNPC target = client.Player.TargetObject as GameNPC;

                if (Util.IsEmpty(target.TranslationId))
                {
                    NoTranslationId(client);
                    return;
                }

                var dbo = GameServer.Database.SelectObjects<DBLanguageNPC>("TranslationId = '" + GameServer.Database.Escape(target.TranslationId) + "' " +
                                                                           "AND Language = '" + GameServer.Database.Escape(client.Account.Language) + "'");

                DBLanguageNPC row;
                if (dbo.Count > 0)
                    row = dbo[0]; // Select the first index
                else
                {
                    row = new DBLanguageNPC();
                    row.TranslationId = target.TranslationId;
                    row.Language = client.Account.Language;
                }

                if (args.Length > 3)
                    row.MessageArticle = String.Join(" ", args, 2, args.Length - 2);
                else
                    row.MessageArticle = args[2];

                if (dbo.Count > 0)
                {
                    for (int i = 0; i < dbo.Count; i++)
                        GameServer.Database.DeleteObject(dbo[i]); // It makes really no sense when you store 2+ rows into the table for one translation in one language
                }

                GameServer.Database.AddObject(row);

                target.RemoveFromWorld();
                target.RefreshTranslationData(client.Account.Language);
                target.AddToWorld();

                TranslationSetTo(client, "message article", args[2]);
                return;
            }

            InvalidTarget(client);
        }
        #endregion SetMessage

        #region SetName
        private void SetName(GameClient client, string[] args)
        {
            if (client.Player.TargetObject == null)
            {
                YouMustSelect(client);
                return;
            }

            if (client.Player.TargetObject is GameMovingObject || client.Player.TargetObject is GameStaticItem)
            {
                if (client.Player.TargetObject is GameMine || client.Player.TargetObject is WorldInventoryItem)
                {
                    InvalidTarget(client);
                    return;
                }

                client.Out.SendMessage("Not supported yet.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            if (client.Player.TargetObject is GameNPC)
            {
                GameNPC target = client.Player.TargetObject as GameNPC;

                if (Util.IsEmpty(target.TranslationId))
                {
                    NoTranslationId(client);
                    return;
                }

                var dbo = GameServer.Database.SelectObjects<DBLanguageNPC>("TranslationId = '" + GameServer.Database.Escape(target.TranslationId) + "' " +
                                                                           "AND Language = '" + GameServer.Database.Escape(client.Account.Language) + "'");

                DBLanguageNPC row;
                if (dbo.Count > 0)
                    row = dbo[0]; // Select the first index
                else
                {
                    row = new DBLanguageNPC();
                    row.TranslationId = target.TranslationId;
                    row.Language = client.Account.Language;
                }

                if (args.Length > 3)
                    row.Name = String.Join(" ", args, 2, args.Length - 2);
                else
                    row.Name = args[2];

                if (dbo.Count > 0)
                {
                    for (int i = 0; i < dbo.Count; i++)
                        GameServer.Database.DeleteObject(dbo[i]); // It makes really no sense when you store 2+ rows into the table for one translation in one language
                }

                GameServer.Database.AddObject(row);

                target.RemoveFromWorld();
                target.RefreshTranslationData(client.Account.Language);
                target.AddToWorld();

                TranslationSetTo(client, "name", args[2]);
                return;
            }

            InvalidTarget(client);
        }
        #endregion SetName

        #region SetSuffix
        private void SetSuffix(GameClient client, string[] args)
        {
            if (client.Player.TargetObject == null)
            {
                YouMustSelect(client);
                return;
            }

            if (client.Player.TargetObject is GameNPC)
            {
                if (client.Player.TargetObject is GameMovingObject)
                {
                    InvalidTarget(client);
                    return;
                }

                GameNPC target = client.Player.TargetObject as GameNPC;

                if (Util.IsEmpty(target.TranslationId))
                {
                    NoTranslationId(client);
                    return;
                }

                var dbo = GameServer.Database.SelectObjects<DBLanguageNPC>("TranslationId = '" + GameServer.Database.Escape(target.TranslationId) + "' " +
                                                                           "AND Language = '" + GameServer.Database.Escape(client.Account.Language) + "'");

                DBLanguageNPC row;
                if (dbo.Count > 0)
                    row = dbo[0]; // Select the first index
                else
                {
                    row = new DBLanguageNPC();
                    row.TranslationId = target.TranslationId;
                    row.Language = client.Account.Language;
                }

                if (args.Length > 3)
                    row.Suffix = String.Join(" ", args, 2, args.Length - 2);
                else
                    row.Suffix = args[2];

                if (dbo.Count > 0)
                {
                    for (int i = 0; i < dbo.Count; i++)
                        GameServer.Database.DeleteObject(dbo[i]); // It makes really no sense when you store 2+ rows into the table for one translation in one language
                }

                GameServer.Database.AddObject(row);

                target.RemoveFromWorld();
                target.RefreshTranslationData(client.Account.Language);
                target.AddToWorld();

                TranslationSetTo(client, "name suffix", args[2]);
                return;
            }

            InvalidTarget(client);
        }
        #endregion SetSuffix
        #endregion OnCommand methods

        #region Other
        private void IncorrectFormat(GameClient client, string method)
        {
            client.Out.SendMessage("Incorrect format. Use '/gmlang " + method + " <translation>.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
        }

        private void InvalidTarget(GameClient client)
        {
            client.Out.SendMessage("Invalid target.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
        }

        private void NoTranslationId(GameClient client)
        {
            client.Out.SendMessage("Your target doesn't have an translation id.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
        }

        private void TranslationSetTo(GameClient client, string type, string translation)
        {
            client.Out.SendMessage("The translated " + type + " for your target has been set to: " + translation + " (Language: " + client.Account.Language + ")",
                    eChatType.CT_System, eChatLoc.CL_SystemWindow);
        }

        private void YouMustSelect(GameClient client)
        {
            client.Out.SendMessage("You must select a target first!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
        }
        #endregion Other
    }
}