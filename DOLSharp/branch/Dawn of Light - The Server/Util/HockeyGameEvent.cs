//Dinberg, perhaps the second of June '07? Meh, tis the holiday, I loose track of time.
using System;
using System.Reflection;
using DOL.Events;
using DOL.GS.PacketHandler;
using log4net;
using System.Collections;
using DOL.AI;
using DOL.AI.Brain;
using DOL.GS.Keeps;
using DOL.Database;
using DOL.GS;
using DOL.GS.Effects;
using DOL.GS.SkillHandler;
using DOL.GS.Spells;

namespace DOL.GS.GameEvents
{

    public class HockeyEvent
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// These are the hockey field posts.
        /// </summary>
        public class PostMarker : GameNPC
        {
            public PostMarker()
                : base()
            {
                X = MatchX;
                Y = MatchY;
                Z = 0;
                Heading = 0x0;
                Name = "Tomte Adjudicator";
                GuildName = "Dinberg's Tomte Hockey";
                Model = 1939;
                Size = 50;
                Level = 0;
                Realm = 3;
                MaxSpeedBase = 0;
                CurrentRegionID = 200;
                //(uint)eFlags.DONTSHOWNAME + (uint)eFlags.CANTTARGET + 
                Flags = (uint)eFlags.DONTSHOWNAME + (uint)eFlags.CANTTARGET + (uint)eFlags.PEACE;
                BlankBrain brain = new BlankBrain();
                SetOwnBrain(brain);
            }

            public override bool Interact(GamePlayer player)
            {
                Say("Watch the game, not me!");
                return base.Interact(player);
            }

        }

        public class GoalMarker : GameNPC
        {
            public GoalMarker()
                : base()
            {
                X = MatchX;
                Y = MatchY;
                Z = 0;
                Heading = 0x0;
                Name = "Goal";
                GuildName = "Hockey";
                Model = 1939;
                Size = 50;
                Level = 0;
                Realm = 3;
                MaxSpeedBase = 0;
                CurrentRegionID = 200;
                //(uint)eFlags.DONTSHOWNAME + (uint)eFlags.CANTTARGET + 
                Flags = (uint)eFlags.PEACE;
                BlankBrain brain = new BlankBrain();
                SetOwnBrain(brain);
            }

            public override bool Interact(GamePlayer player)
            {
                Say("I'm a goal post!");
                return base.Interact(player);
            }

        }
        /// <summary>
        /// Our hockey judge (his brain governs the game!).
        /// </summary>
        public class Judge : GameNPC
        {
            public Judge()
                : base()
            {
                //Place the judge sort of offcourt...
                X = MatchX + (int)(MatchXSize + 200);
                Y = MatchY + MatchYSize;
                Z = 0;
                Heading = 0x0;
                Name = "Master Dinberg";
                GuildName = "Tomte Hockey";
                Model = 672;
                Size = 80;
                Level = 60;
                Realm = 3;
                CurrentRegionID = 200;
                Flags = (uint)eFlags.PEACE;
                JudgeBrain Brain = new JudgeBrain();
                SetOwnBrain(Brain);
                MaxSpeedBase = 250;
            }

            public virtual bool AddPlayerCheck(GamePlayer player)
            {
                if (m_players.Count > 0)
                {
                    foreach (GamePlayer check in m_players)
                    {
                        if (check == player)
                            return false;
                    }
                }
                return true;
            }
            
            public override bool Interact(GamePlayer player)
            {
                //Ending game phase  - the trainer will take all sticks back in!
                if (m_endingGame == true)
                {
                    //Find the hockeystick in their inventory.
                    InventoryItem stick = player.Inventory.GetFirstItemByID("hockey_stick", eInventorySlot.Min_Inv, eInventorySlot.Max_Inv);
                    if (stick != null)
                    {
                        //This is the part with the winners!
                        string team = "";

                        //Work out which team we were on by our stick's colour...
                        if (stick.Color == 1)
                            team = "red";
                        else
                            team = "blue";

                        //now, were we on the winning team?
                        if (team == m_winner)
                        {
                            player.Out.SendMessage("Wow, you are one of the skilled ones, eh? Congratulations on the victory!", eChatType.CT_Broadcast, eChatLoc.CL_ChatWindow);
                            //reward here
                            //Distribute the bets aswell - how much the player paid x 2 (or 2xsum / MaxPlayers?)
                            if (m_wager > 0)
                            {
                                player.AddMoney(Convert.ToInt64(Math.Round((double)(2 * m_wager * 100))));
                                player.Out.SendMessage("You gain " + Math.Round((double)(2 * m_wager/1000)) + " gold from your victory!", eChatType.CT_Broadcast, eChatLoc.CL_ChatWindow);
                                player.Out.SendUpdatePlayer();
                            }
                            
                        }
                        else
                        {
                            player.Out.SendMessage("Ahh, better luck next time, eh? Well, I don't have anything for you right now, but perhaps if you win I'll be able to pull something up!", eChatType.CT_Broadcast, eChatLoc.CL_ChatWindow);
                        }

                        //Delete the stick
                        player.Inventory.RemoveItem(stick);
                        if (m_playersPlaying != 0 && m_endingGame == true)
                        {
                            m_playersPlaying -= 1;
                            if (m_playersPlaying == 0)
                            {
                                m_endingGame = false;
                                foreach (GamePlayer victims in GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
                                {
                                    victims.Out.SendMessage("The sticks have all been handed in! Please feel free to take another game!", eChatType.CT_Broadcast, eChatLoc.CL_ChatWindow);
                                    log.Info("Tomte Hockey sticks all handed in! May another game commence!");
                                }
                            }
                        }
                    }
                    /*//This is the part with the winners!
                    string team = "";
                    
                    //Work out which team we were on by our stick's colour...
                    if (stick.Color == 1)
                        team = "red";
                    else
                        team = "blue";

                    //now, were we on the winning team?
                    if (team == m_winner)
                    {
                        player.Out.SendMessage("Wow, you are one of the skilled ones, eh? Congratulations on the victory!", eChatType.CT_Broadcast, eChatLoc.CL_ChatWindow);
                        //reward here
                        //Distribute the bets aswell - how much the player paid x 2 (or 2xsum / MaxPlayers?)
                        if (m_wager > 0)
                        {
                            player.AddMoney(Convert.ToInt64(Math.Round((double)(2 * m_wager) / m_maxPlayers)));
                            player.Out.SendMessage("You gain " + Math.Round((double)(2 * m_wager) / m_maxPlayers) + " gold from your victory!", eChatType.CT_Broadcast, eChatLoc.CL_ChatWindow); 
                            player.Out.SendUpdatePlayer();
                        }
                        
                    }
                    else
                    {
                        player.Out.SendMessage("Ahh, better luck next time, eh? Well, I don't have anything for you right now, but perhaps if you win I'll be able to pull something up!", eChatType.CT_Broadcast, eChatLoc.CL_ChatWindow);
                    }*/
                }

                //Neither a game is set up or started
                if (m_startPhase == false && GamePlaying == false)
                {
                player.Out.SendMessage("Greetings to you, I am Dinberg, the tomte herder trainer. I train this little puck here in the rules and game of Tomte Herding. [Tomte herding], you say? Why yes, tomte herding! It's a revolutionary new game played by people like you; the rules are simple, get the puck into the other net! Would you like a [game]? I'd guess so, I would!", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
                }

                //The game is set up, and teams must be allocated!
                if (m_startPhase == true)
                {
                    //Basically, add a handler to each player so that only those players can affect the puck.
                    //m_players.Add(player);
                    //assume we have the property, unless we can proove we dont, in which case give us the property.
                    
                    //Only let them play if they do not have a hockey stick already, and can afford the wager!
                    if (PlayerHasHockeyStick(player) == false)
                    {
                        if (m_playersPlaying < m_maxPlayers)
                        {
                            //Paying the wager - can we afford it?
                            if (m_wager > 0)
                            {
                                bool pay = player.RemoveMoney(Convert.ToInt64(m_wager * 100));
                                if (pay == true)
                                {
                                    player.Out.SendMessage("You pay the wager: " + Math.Round((double)m_wager / 1000) + " gold!", eChatType.CT_Broadcast, eChatLoc.CL_SystemWindow);
                                }
                                else
                                {
                                    player.Out.SendMessage("You cannot pay the wager!", eChatType.CT_BattleGroup, eChatLoc.CL_SystemWindow);
                                    return true; //return and go no further - do not join the match!
                                }
                            }

                            //store the players in the array list - to be used for prizes!
                            //m_players[m_playersPlaying] = player;
                            m_playersPlaying += 1;
                            foreach (GamePlayer person in GetPlayersInRadius(4000))
                            {
                                person.Out.SendMessage(m_playersPlaying + "/" + m_maxPlayers + " filled! Hurry now or you'll be left out!", eChatType.CT_Broadcast, eChatLoc.CL_ChatWindow);
                            }

                            /*//WAGER ****************************************
                            if (m_wager > 0)
                            {
                            player.RemoveMoney(Convert.ToInt64(m_wager));
                            player.Out.SendMessage("You pay the wager: " + Math.Round((double)m_wager/1000) + " gold!", eChatType.CT_Broadcast, eChatLoc.CL_SystemWindow);
                            }*/
                        }

                        //if (m_playersPlaying <= m_maxPlayers / 2) This is changed to cater for blue and red team players.
                        if (m_bluePlayers < m_redPlayers)
                        {
                            player.Out.SendMessage("You are playing for the Blue team!", eChatType.CT_Say, eChatLoc.CL_ChatWindow);
                            player.MoveTo(200, MatchX - (int)Math.Round((double)(MatchXSize / 2) + Util.Random(0, 200)), MatchY, 0, player.Heading);
//hockey stick.
                            InventoryItem BlueStick = new InventoryItem(hockeyStick);
                            BlueStick.Color = 3;
                            eInventorySlot hockeyloc = player.Inventory.FindFirstEmptySlot(eInventorySlot.FirstBackpack, eInventorySlot.LastBackpack);
                            bool bluegood = player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, BlueStick);
                            if (bluegood == true)
                            {
                                player.Inventory.MoveItem(eInventorySlot.TwoHandWeapon, hockeyloc, 1);
                            }
                            m_bluePlayers += 1;
                            //Blue team has more players - give the other player to the read team.
                        }
                        else
                        {
                            player.Out.SendMessage("You are playing for the Red team!", eChatType.CT_Say, eChatLoc.CL_ChatWindow);
                            player.X = MatchX + Util.Random(-100, 100);
                            player.Y = MatchY + Util.Random(-100, 100);
                            player.MoveTo(200, MatchX + (int)Math.Round((double)(MatchXSize / 2) - Util.Random(0, 200)), MatchY, 0, player.Heading);


                            //InventoryItem testStick = player.Inventory.GetFirstItemByID("hockey_stick", 0, eInventorySlot.LastBackpack);
                            InventoryItem RedStick = new InventoryItem(hockeyStick);
                            RedStick.Color = 1;
                            eInventorySlot hockeyloc = player.Inventory.FindFirstEmptySlot(eInventorySlot.FirstBackpack, eInventorySlot.LastBackpack);
                            bool good = player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, RedStick);
                            if (good == true)
                            {
                                player.Inventory.MoveItem(eInventorySlot.TwoHandWeapon, hockeyloc, 1);

                            }
                            m_redPlayers += 1;

                        }
                        //Now, add linkdead/quit handlers so that the player cannot scarper off with the hockey stick and claim a groups prize later.
                        GameEventMgr.AddHandler(player, GamePlayerEvent.Quit, new DOLEventHandler(RemoveHockeyStick));
                        GameEventMgr.AddHandler(player, GamePlayerEvent.Linkdeath, new DOLEventHandler(RemoveHockeyStick));
                        GameEventMgr.AddHandler(player, GamePlayerEvent.RegionChanged, new DOLEventHandler(RemoveHockeyStick));
                    }
                    else
                    {
                        player.Out.SendMessage("Your handing your sticks in? I assume you set something up incorrectly - just use [cancel] then and I'll cease the game 'lobby' and you can set up another game. Wonderful!", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
                        InventoryItem stick = player.Inventory.GetFirstItemByID("hockey_stick", eInventorySlot.Min_Inv, eInventorySlot.Max_Inv);
                        if (stick != null)
                        {
                            //First, remove them from their team...
                            if (stick.Color == 1)
                                m_redPlayers -= 1;
                            else
                                m_bluePlayers -= 1;

                            //Delete the stick
                            player.Inventory.RemoveItem(stick);
                            if (m_playersPlaying != 0 && m_startPhase == true)
                            {
                                m_playersPlaying -= 1;
                                foreach (GamePlayer person in GetPlayersInRadius(4000))
                                {
                                    person.Out.SendMessage(m_playersPlaying + "/" + m_maxPlayers + " filled! " + player.Name + " elects to not play this match!", eChatType.CT_Broadcast, eChatLoc.CL_ChatWindow);
                                }
                                if (m_playersPlaying == 0)
                                {
                                    foreach (GamePlayer victims in GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
                                    {
                                        victims.Out.SendMessage("The sticks have all been handed in! Please feel free to take another game!", eChatType.CT_Broadcast, eChatLoc.CL_ChatWindow);
                                        log.Info("Tomte Hockey sticks all handed in! May another game commence!");
                                    }
                                }
                            }
                            //Here we have to remove the handlers, or this player who no longer is playign will release player count when he moves!
                            GameEventMgr.RemoveHandler(player, GamePlayerEvent.Quit, new DOLEventHandler(RemoveHockeyStick));
                            GameEventMgr.RemoveHandler(player, GamePlayerEvent.Linkdeath, new DOLEventHandler(RemoveHockeyStick));
                            GameEventMgr.RemoveHandler(player, GamePlayerEvent.RegionChanged, new DOLEventHandler(RemoveHockeyStick));

                        }
                    }
                    
                    if (m_playersPlaying == m_maxPlayers)
                    {
                        foreach (GamePlayer players in GetPlayersInRadius(5000))
                        {
                            players.Out.SendMessage("Move it people, the game has started!", eChatType.CT_Broadcast, eChatLoc.CL_ChatWindow);
                        }
                        m_startPhase = false;
                        GamePlaying = true;
                        WalkTo(MatchX, MatchY, 0, 240);
                    }
                }
                if (GamePlaying == true && m_playersPlaying > 0)
                {
                    if (PlayerHasHockeyStick(player) == false)
                    {
                        player.Out.SendMessage("Sorry, someone's already playing - try coming back later, or watch them and challenge the winner when he comes off! Hehe.....", eChatType.CT_Say, eChatLoc.CL_ChatWindow);
                    }
                    else
                    {
                        player.Out.SendMessage("No, not me fool! Get to the pitch, your supposed to be playing!", eChatType.CT_Say, eChatLoc.CL_ChatWindow);
                    }
                }
                
                //Someone may deactivate a game that has no players (and therefore cannot end) by simply interacting with the Master.
                if (GamePlaying == true && m_playersPlaying <= 0)
                    GamePlaying = false;
                return base.Interact(player);
            }

            public void RemoveHockeyStick(DOLEvent e, object sender, EventArgs args)
            {
                GamePlayer player = sender as GamePlayer;
                if (player == null)
                    return;

                //Find the hockeystick in their inventory.
                InventoryItem stick = player.Inventory.GetFirstItemByID("hockey_stick", eInventorySlot.Min_Inv, eInventorySlot.Max_Inv);
                if (stick != null)
                    player.Inventory.RemoveItem(stick);
                if (m_playersPlaying != 0 && (m_endingGame == true || m_startPhase == true))
                {
                    //Here's the problem - I've moved this to outside whether or not the start phase or ending phase is occuring. Why? Because, otherwise, if a player goes linkdead ingame he will loose his stick yet will not reduce m_playerPlaying.
                    //m_playersPlaying -= 1;
                    m_playersPlaying -= 1;
                    if (m_playersPlaying == 0)
                    {
                        m_endingGame = false;
                        foreach (GamePlayer victims in GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
                        {
                            victims.Out.SendMessage("The sticks have all been handed in! Please feel free to take another game!", eChatType.CT_Broadcast, eChatLoc.CL_ChatWindow);
                            log.Info("Tomte Hockey sticks all handed in! May another game commence!");
                        }
                    }
                    
                }
                if (GamePlaying == true && m_playersPlaying > 0)
                {
                    m_playersPlaying -= 1;
                    log.Info("Player threw a trigger and can no longer play hockey, player count reduced accordingly. Right click to cancel!");

                }
                GameEventMgr.RemoveHandler(player, GamePlayerEvent.Quit, new DOLEventHandler(RemoveHockeyStick));
                GameEventMgr.RemoveHandler(player, GamePlayerEvent.Linkdeath, new DOLEventHandler(RemoveHockeyStick));
                GameEventMgr.RemoveHandler(player, GamePlayerEvent.RegionChanged, new DOLEventHandler(RemoveHockeyStick));
            }

            public virtual bool PlayerHasHockeyStick(GamePlayer player)
            {
                InventoryItem item = player.Inventory.GetFirstItemByID("hockey_stick", eInventorySlot.Min_Inv, eInventorySlot.Max_Inv);
                    if (item != null)
                        return true;
                
                return false;
            }

            public override bool ReceiveItem(GameLiving source, InventoryItem item)
            {
                if (item.Id_nb == "hockey_stick")
                {
                    GamePlayer player = source as GamePlayer;
                    if (player != null)
                    {
                        player.Inventory.RemoveItem(item);
                        if (m_playersPlaying != 0 && m_endingGame == true)
                        {
                            m_playersPlaying -= 1;
                            if (m_playersPlaying == 0)
                            {
                                m_endingGame = false;
                                foreach (GamePlayer victims in GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
                                {
                                    victims.Out.SendMessage("The sticks have all been handed in! Please feel free to take another game!", eChatType.CT_Broadcast, eChatLoc.CL_ChatWindow);
                                    log.Info("Tomte Hockey sticks all handed in! May another game commence!");
                                }
                            }
                        }
                        player.Out.SendMessage("There you go, you have handed back your stick! Here is your prize;", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
                        return true;
                    }
                    return base.ReceiveItem(source, item);
                }
                else
                {
                    return base.ReceiveItem(source, item);
                }
            }
            public override bool SayReceive(GameLiving source, string str)
            {
                React(source, str);
                return base.SayReceive(source, str);
            }

            public override bool WhisperReceive(GameLiving source, string str)
            {
                React(source, str);
                return base.WhisperReceive(source, str);
            }

            public virtual void React(GameLiving source, string str)
            {
                //is it a player whispering?
                if (!(source is GamePlayer))
                    return;
                GamePlayer player = source as GamePlayer;
                switch (str)
                {
                    case "game":
                        {
                            if (m_startPhase == false && GamePlaying == false && m_endingGame == false)
                            {
                                player.Out.SendMessage("Ahh wonderful! You are one of my favourite people now, heh. Fancy a game for [four], [six], [eight] or a wonderful [duel]? You may also place a [wager], so that people will have to pay to play - the winning team takes the loot!", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
                            }
                        }
                        break;
                    case "four":
                        {
                            if (m_startPhase == false && GamePlaying == false && m_endingGame == false)
                            {
                                player.Out.SendMessage("Four players set - a grand number! Tell me when you are ready to [start].", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
                                m_maxPlayers = 4;
                            }
                        }
                        break;
                    case "six":
                        {
                            if (m_startPhase == false && GamePlaying == false && m_endingGame == false)
                            {
                                player.Out.SendMessage("Six players set! Tell me when you are ready to [start].", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
                                m_maxPlayers = 6;
                            }
                        }
                        break;
                    case "eight":
                        {
                            if (m_startPhase == false && GamePlaying == false && m_endingGame == false)
                            {
                                player.Out.SendMessage("Eight players?!! A big game, let's hope it goes well - ready to [start]?", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
                                m_maxPlayers = 8;
                            }
                        }
                        break;
                    case "duel":
                        {
                            if (m_startPhase == false && GamePlaying == false && m_endingGame == false)
                            {
                                player.Out.SendMessage("Clash of two titans? If you wish! Just say the word when you wanna [start].", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
                                m_maxPlayers = 2;
                            }
                        }
                        break;
                    case "wager": // int wager!
                        {
                            if (m_startPhase == false && GamePlaying == false && m_endingGame == false)
                            {
                                player.Out.SendMessage("To select the wager, you must choose either [0], [1], [5], [50], [500] or [1000] gold pieces!", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
                            }
                        }
                        break;
                    case "0": // int wager!
                        {
                            if (m_startPhase == false && GamePlaying == false && m_endingGame == false)
                            {
                                m_wager = 0 * 1000;
                            }
                        }
                        break;
                    case "1": // int wager!
                        {
                            if (m_startPhase == false && GamePlaying == false && m_endingGame == false)
                            {
                                m_wager = 1 * 1000;
                            }
                        }
                        break;
                    case "5": // int wager!
                        {
                            if (m_startPhase == false && GamePlaying == false && m_endingGame == false)
                            {
                                m_wager = 5 * 1000;
                            }
                        }
                        break;
                    case "50": // int wager!
                        {
                            if (m_startPhase == false && GamePlaying == false && m_endingGame == false)
                            {
                                m_wager = 50 * 1000;
                            }
                        }
                        break;
                    case "500": // int wager!
                        {
                            if (m_startPhase == false && GamePlaying == false && m_endingGame == false)
                            {
                                m_wager = 500 * 1000;
                            }
                        }
                        break;
                    case "1000": // int wager!
                        {
                            if (m_startPhase == false && GamePlaying == false && m_endingGame == false)
                            {
                                m_wager = 1000 * 1000;
                            }
                        }
                        break;
                    case "start":
                        {
                            if (m_startPhase == false && GamePlaying == false && m_endingGame == false)
                            {
                                player.Out.SendMessage("Then let's move it! Blue team should right click me first, then when the blue team is full the rest will go to the red team. When the two teams are full, the game will commence! If you set the game incorrectly, please just [cancel] it and set it up again.", eChatType.CT_Broadcast, eChatLoc.CL_PopupWindow);
                                player.Out.SendMessage("The wager has been set to: " + Math.Round((double)m_wager / 1000) + " gold pieces. Upon clicking you will automatically be charged the money. A successful victory will give you double your initial wager!", eChatType.CT_Broadcast, eChatLoc.CL_SystemWindow);
                                m_startPhase = true;
                                m_playersPlaying = 0;
                                m_redPlayers = 0;
                                m_bluePlayers = 0;
                                
                                //m_players.Clear();
                            }
                        }
                        break;
                    case "practice":
                        {
                            
                            if (m_startPhase == true)
                            {
                                if (m_wager == 0)
                                {
                                    //prevents someone starting a game with no players, thereby locking the game up until server reboot!
                                    if (m_playersPlaying > 0)
                                    {
                                        foreach (GamePlayer players in GetPlayersInRadius(5000))
                                        {
                                            players.Out.SendMessage("Move it people, the game has started!", eChatType.CT_Broadcast, eChatLoc.CL_ChatWindow);
                                        }
                                        m_startPhase = false;
                                        GamePlaying = true;
                                        WalkTo(MatchX, MatchY, 0, 240); ;
                                    }
                                }
                                else
                                {
                                    player.Out.SendMessage("Can't practice on a wager game I'm afraid!", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
                                }
                                
                            }
                            
                        }
                        break;
                    case "Tomte herding":
                        {
                            player.Out.SendMessage("Ahh, so you've never played I take it? Well, it can be confusing for first timers. First, you set up the game - declare how many players, and your bet (if any - you can always play for practice). Once that's been done, you hit start, and players will need to interact with me to be given their hockey sticks. When both teams are full, you will be teleported to the pitch, on your side of the field - run to score in the enemies goal! Equip the hockeystick in the two-handed slot and your ready to play! The puck will flee from you, and you must guide it into the opposing teams net to score. Try not to get it into your own net, that's not a popular thing to do!", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
                            player.Out.SendMessage("Hope you have a fun game today!", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
                        }
                        break;
                    case "cancel":
                        {
                            //Can cancel either if all sticks are handed in at the start, or if a game has been abandoned at the 'playing' stage.
                            if ((m_startPhase == true || GamePlaying == true) && m_playersPlaying <= 0)
                            {
                                player.Out.SendMessage("The game has been cancelled! You may now reset the game parameters.", eChatType.CT_Advise, eChatLoc.CL_ChatWindow);
                                m_startPhase = false;
                                GamePlaying = false;
                            }
                            else
                            {
                                player.Out.SendMessage("I'm afraid I can't cancel; there are still sticks about! Either finish the game if one is playing, or hand all the sticks back in if you were setting one up.", eChatType.CT_Advise, eChatLoc.CL_ChatWindow);
                            }
                        }
                        break;
                }
            }

        }

        public class JudgeBrain : APlayerVicinityBrain, IAggressiveBrain
        {
            #region Judge
            /// <summary>
            /// Defines a logger for this class.
            /// </summary>
            private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            public const int MAX_AGGRO_DISTANCE = 3600;

            /// <summary>
            /// Constructs a new StandardMobBrain
            /// </summary>
            public JudgeBrain()
                : base()
            {
                m_aggroLevel = 0;
                m_aggroMaxRange = 200;
            }


            /// <summary>
            /// Returns the string representation of the StandardMobBrain
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return base.ToString() + ", m_aggroLevel=" + m_aggroLevel.ToString() + ", m_aggroMaxRange=" + m_aggroMaxRange.ToString();
            }

            #region AI

            /// <summary>
            /// Do the mob AI
            /// </summary>
            public override void Think()
            {
              /* //Run to the game if one is playing because, after all, we are the referee!
                if (GamePlaying == true && WorldMgr.CheckDistance(Body, MatchX, MatchY, Body.Z, 100))
                    Body.MoveTo(200, MatchX, MatchY, 0, 0x0);
                if (GamePlaying == false && WorldMgr.CheckDistance(Body, Body.SpawnX, Body.SpawnY, Body.SpawnZ, 100))
                    Body.MoveTo(200, Body.SpawnX, Body.SpawnY, 0, 0x0);*/
                
                //Send out some spell effects to indicate which goal belongs to whom...
                /*foreach (GamePlayer player in Body.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
                {
                    player.Out.SendSpellEffectAnimation(m_goal1, m_goal1, 2300, 0, false, 1);
                    player.Out.SendSpellEffectAnimation(m_goal2, m_goal2, 10307, 0, false, 1);
                }*/
                //Check the puck has not passed into any teams goal yet.
                if (WorldMgr.CheckDistance(m_puck, m_goal1, 250))
                {
                    m_puck.Yell(m_puck.TargetObject.Name + " scores a point!");
                    BlueScore += 1;
                    m_puck.X = MatchX;
                    m_puck.Y = MatchY - 200;
                    //broadcast score update.
                    foreach (GamePlayer player in Body.GetPlayersInRadius(4000))
                    {
                        player.Out.SendMessage("Blue team scores a point, placing it at " + BlueScore + " to Red's " + RedScore + "!", eChatType.CT_Broadcast, eChatLoc.CL_SystemWindow);
                        player.Out.SendObjectUpdate(m_puck);
                    }
                    if (BlueScore >= 3)
                    {
                        RedScore = 0;
                        BlueScore = 0;
                        GamePlaying = false;
                        m_endingGame = true;
                        m_winner = "blue";
                        Body.MoveTo(200, Body.SpawnX, Body.SpawnY, 0, 0x0);
                        //broadcast score update.
                        foreach (GamePlayer player in Body.GetPlayersInRadius(4000))
                        {
                            player.Out.SendMessage("Blue team wins! Please make your way back to the herder master to return your sticks to receieve a parting gift or rewards!", eChatType.CT_Broadcast, eChatLoc.CL_SystemWindow);
                        }
                    }
                }
                if (WorldMgr.CheckDistance(m_puck, m_goal2, 250))
                {
                    m_puck.Yell(m_puck.TargetObject.Name + " scores a point!");
                    RedScore += 1;
                    m_puck.X = MatchX;
                    m_puck.Y = MatchY;
                    //broadcast score update.
                    foreach (GamePlayer player in Body.GetPlayersInRadius(4000))
                    {
                        player.Out.SendMessage("Red team scores a point, placing it at " + RedScore + " to Blue's " + BlueScore + "!", eChatType.CT_Broadcast, eChatLoc.CL_SystemWindow);
                        player.Out.SendObjectUpdate(m_puck);
                    }
                    if (RedScore >= 3)
                    {
                        RedScore = 0;
                        BlueScore = 0;
                        GamePlaying = false;
                        m_endingGame = true;
                        m_winner = "red";
                        Body.MoveTo(200, Body.SpawnX, Body.SpawnY, 0, 0x0);
                        //broadcast score update.
                        foreach (GamePlayer player in Body.GetPlayersInRadius(4000))
                        {
                            player.Out.SendMessage("Red team wins! Please make your way back to the herder master to return your sticks to receieve a parting gift or rewards!", eChatType.CT_Broadcast, eChatLoc.CL_SystemWindow);
                        }
                    }
                }
            }

            /// <summary>
            /// For Prey, aggro will probably be low. The think interval will be set to default of 1.5
            /// </summary>
            public override int ThinkInterval
            {
                get { return 1000; }
            }

            #region Aggro (for sake of override)
            /// <summary>
            /// Max Aggro range in that this npc searches for enemies
            /// </summary>
            protected int m_aggroMaxRange;
            /// <summary>
            /// Aggressive Level of this npc
            /// </summary>
            protected int m_aggroLevel;
            /// <summary>
            /// List of livings that this npc has aggro on, living => aggroamount
            /// </summary>
            protected readonly Hashtable m_aggroTable = new Hashtable();

            /// <summary>
            /// Aggressive Level in % 0..100, 0 means not Aggressive
            /// </summary>
            public virtual int AggroLevel
            {
                get { return m_aggroLevel; }
                set { m_aggroLevel = value; }
            }

            /// <summary>
            /// Range in that this npc aggros
            /// </summary>
            public virtual int AggroRange
            {
                get { return m_aggroMaxRange; }
                set { m_aggroMaxRange = value; }
            }

            /// <summary>
            /// Add living to the aggrolist
            /// aggroamount can be negative to lower amount of aggro		
            /// </summary>
            /// <param name="living"></param>
            /// <param name="aggroamount"></param>
            public virtual void AddToAggroList(GameLiving living, int aggroamount)
            {
                if (m_body.IsConfused) return;

                if (living == null) return;
                //			log.Debug(Body.Name + ": AddToAggroList="+(living==null?"(null)":living.Name)+", "+aggroamount);

                // only protect if gameplayer and aggroamout > 0
                if (living is GamePlayer && aggroamount > 0)
                {
                    GamePlayer player = (GamePlayer)living;

                    if (player.PlayerGroup != null)
                    { // player is in group, add whole group to aggro list
                        lock (m_aggroTable.SyncRoot)
                        {
                            foreach (GamePlayer groupPlayer in player.PlayerGroup.GetPlayersInTheGroup())
                            {
                                if (m_aggroTable[groupPlayer] == null)
                                {
                                    m_aggroTable[groupPlayer] = 1L;	// add the missing group member on aggro table
                                }
                            }
                        }
                    }

                    //ProtectEffect protect = (ProtectEffect) player.EffectList.GetOfType(typeof(ProtectEffect));
                    foreach (ProtectEffect protect in player.EffectList.GetAllOfType(typeof(ProtectEffect)))
                    {
                        // if no aggro left => break
                        if (aggroamount <= 0) break;

                        //if (protect==null) continue;
                        if (protect.ProtectTarget != living) continue;
                        if (protect.ProtectSource.IsStunned) continue;
                        if (protect.ProtectSource.IsMezzed) continue;
                        if (protect.ProtectSource.IsSitting) continue;
                        if (protect.ProtectSource.ObjectState != GameObject.eObjectState.Active) continue;
                        if (!protect.ProtectSource.IsAlive) continue;
                        if (!protect.ProtectSource.InCombat) continue;

                        if (!WorldMgr.CheckDistance(living, protect.ProtectSource, ProtectAbilityHandler.PROTECT_DISTANCE))
                            continue;
                        // P I: prevents 10% of aggro amount
                        // P II: prevents 20% of aggro amount
                        // P III: prevents 30% of aggro amount
                        // guessed percentages, should never be higher than or equal to 50%
                        int abilityLevel = protect.ProtectSource.GetAbilityLevel(Abilities.Protect);
                        int protectAmount = (int)((abilityLevel * 0.10) * aggroamount);

                        if (protectAmount > 0)
                        {
                            aggroamount -= protectAmount;
                            protect.ProtectSource.Out.SendMessage("You are protecting " + player.GetName(0, false) + " and distract " + Body.GetName(0, false) + ".", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                            //						player.Out.SendMessage("You are protected by " + protect.ProtectSource.GetName(0, false) + " from " + Body.GetName(0, false) + ".", eChatType.CT_System, eChatLoc.CL_SystemWindow);

                            lock (m_aggroTable.SyncRoot)
                            {
                                if (m_aggroTable[protect.ProtectSource] != null)
                                {
                                    long amount = (long)m_aggroTable[protect.ProtectSource];
                                    amount += protectAmount;
                                    m_aggroTable[protect.ProtectSource] = amount;
                                }
                                else
                                {
                                    m_aggroTable[protect.ProtectSource] = (long)protectAmount;
                                }
                            }
                        }
                    }
                }

                lock (m_aggroTable.SyncRoot)
                {
                    if (m_aggroTable[living] != null)
                    {
                        long amount = (long)m_aggroTable[living];
                        amount += aggroamount;
                        if (amount <= 0)
                        {
                            m_aggroTable.Remove(living);
                        }
                        else
                        {
                            m_aggroTable[living] = amount;
                        }
                    }
                    else
                    {
                        if (aggroamount > 0)
                        {
                            m_aggroTable[living] = (long)aggroamount;
                        }
                    }


                }
            }

            /// <summary>
            /// Get current amount of aggro on aggrotable
            /// </summary>
            /// <param name="living"></param>
            /// <returns></returns>
            public virtual long GetAggroAmountForLiving(GameLiving living)
            {
                lock (m_aggroTable.SyncRoot)
                {
                    if (m_aggroTable[living] != null)
                    {
                        return (long)m_aggroTable[living];
                    }
                    return 0;
                }
            }

            /// <summary>
            /// Remove one living from aggro list
            /// </summary>
            /// <param name="living"></param>
            public virtual void RemoveFromAggroList(GameLiving living)
            {
                lock (m_aggroTable.SyncRoot)
                {
                    m_aggroTable.Remove(living);
                }

            }

            /// <summary>
            /// Remove all livings from the aggrolist
            /// </summary>
            public virtual void ClearAggroList()
            {
                lock (m_aggroTable.SyncRoot)
                {
                    m_aggroTable.Clear();
                }
            }

            /// <summary>
            /// Makes a copy of current aggro list
            /// </summary>
            /// <returns></returns>
            public virtual Hashtable CloneAggroList()
            {
                lock (m_aggroTable.SyncRoot)
                {
                    return (Hashtable)m_aggroTable.Clone();
                }
            }

            /// <summary>
            /// calculate the aggro of this npc against another living
            /// </summary>
            /// <param name="target"></param>
            /// <returns></returns>
            public virtual int CalculateAggroLevelToTarget(GameLiving target)
            {
                if (GameServer.ServerRules.IsSameRealm(Body, target, true)) return 0;
                if (AggroLevel >= 100) return 100;
                if (target.IsObjectGreyCon(Body)) return 0;	// only attack if green+ to target
                //if (Level <= 3) return 0;	// workaround, dont aggro for newbie mobs
                if (Body.Faction != null && target is GamePlayer)
                {
                    GamePlayer player = (GamePlayer)target;
                    AggroLevel = Body.Faction.GetAggroToFaction(player);
                }
                return AggroLevel;
            }

            #endregion




            #endregion
            #endregion
        }

        /// <summary>
        /// Our hockey puck.
        /// </summary>
        public class Puck : GameNPC
        {
            public Puck()
                : base()
            {
               
                X = MatchX;
                Y = MatchY;
                Z = 0;
                Heading = 0x0;
                Name = "Hockey Puck";
                GuildName = "Dinberg's Tomte Hockey";
                Model = 672;
                Size = 25;
                Level = 60;
                Realm = 3;
                CurrentRegionID = 200;
                MaxSpeedBase = 210;
                Flags = (uint)eFlags.CANTTARGET + (uint)eFlags.PEACE;
                PuckBrain puckBrain = new PuckBrain();
                SetOwnBrain(puckBrain);
            }

        }

        public class PuckBrain : APlayerVicinityBrain, IAggressiveBrain
        {
            #region Puck
            /// <summary>
            /// Defines a logger for this class.
            /// </summary>
            private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            public const int MAX_AGGRO_DISTANCE = 3600;

            /// <summary>
            /// Constructs a new StandardMobBrain
            /// </summary>
            public PuckBrain()
                : base()
            {
                m_aggroLevel = 0;
                m_aggroMaxRange = 200;
            }

            /// <summary>
            /// Returns the string representation of the StandardMobBrain
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return base.ToString() + ", m_aggroLevel=" + m_aggroLevel.ToString() + ", m_aggroMaxRange=" + m_aggroMaxRange.ToString();
            }

            #region AI
            //How many people to flee from in the immediate radius.
            int PeopleThatScareMeInMyRange;

            /// <summary>
            /// Do the mob AI
            /// </summary>
            public override void Think()
            {
                //Don't do anything if we are in a goal zone, or if the game isn't being played.
                if (!(WorldMgr.CheckDistance(m_puck, m_goal1, 250) || WorldMgr.CheckDistance(m_puck, m_goal1, 250)) && GamePlaying == true)
                {
                    PeopleThatScareMeInMyRange = 0;

                    //Run away from players
                    foreach (GamePlayer player in Body.GetPlayersInRadius((ushort)AggroRange))
                    {
                        //check if we must flee..
                        if (DoWeFlee(player))
                        {
                            CalculateFleeTarget(player);
                            PeopleThatScareMeInMyRange += 1;
                        }
                    }

                    //Turn off our flight.
                    if (PeopleThatScareMeInMyRange == 0)
                        m_fleeing = false;

                    //have we scored?
                    if (Body.X < (MatchX - (int)Math.Round((double)(MatchXSize / 2))))
                    {
                        Body.Say(Body.TargetObject.Name + " takes it off the court!");
                        Body.X = MatchX;
                        Body.Y = MatchY;
                    }

                    //have we scored?
                    if (Body.X > (MatchX + (int)Math.Round((double)(MatchXSize / 2))))
                    {
                        Body.Say(Body.TargetObject.Name + " takes it off the court!");
                        Body.X = MatchX;
                        Body.Y = MatchY;
                    }

                    if (Body.Y < (MatchY - (int)Math.Round((double)(MatchYSize / 2))))
                    {
                        Body.X = MatchX;
                        Body.Y = MatchY;
                        Body.Say(Body.TargetObject.Name + " takes it off the court!");
                    }

                    if (Body.Y > (MatchY + (int)Math.Round((double)(MatchYSize / 2))))
                    {
                        Body.X = MatchX;
                        Body.Y = MatchY;
                        Body.Say(Body.TargetObject.Name + " takes it off the court!");
                    }
                }
            }
            #region Flight
            //this is the variable that says whether or not we are fleeing.
            bool m_fleeing = false;
            /// <summary>
            /// The base location for the herd, that the prey will try to stick to.
            /// </summary>
            /// <param name="target">The target to get the herd location from.</param>
            /// <returns>A Point3D representing the herd's home coordinates.</returns>
            protected virtual IPoint3D HerdLocation(GameNPC target)
            {
                int Spawnx = target.SpawnX;
                int Spawny = target.SpawnY;
                int Spawnz = target.SpawnZ;
                return new Point3D(Spawnx, Spawny, Spawnz);
            }
            /// <summary>
            /// Checks to see if the mob's spawn is safe.
            /// </summary>
            /// <returns>True if it is safe.</returns>
            protected virtual bool IsSpawnSafe()
            {
                IPoint3D spawn = HerdLocation(Body);
                //visibility distance
                foreach (GameNPC threat in Body.GetNPCsInRadius((ushort)(AggroRange * 3)))
                {
                    //check if it's in the spawn's safe zone.
                    float xdist = (threat.X - spawn.X);
                    float ydist = (threat.Y - spawn.Y);
                    float distance = (float)Math.Sqrt(Math.Pow(xdist, 2) + Math.Pow(xdist, 2));
                    if (distance > AggroRange)
                        return false;
                }
                return true;
            }
            /// <summary>
            /// Checks to see if there is anyone within the NPC's danger radius that would cause it to flee.
            /// </summary>
            protected virtual bool DoWeFlee(GameLiving target)
            {
                
                
                //only run from members who are playing the game..
                if (target is GamePlayer)
                {
                    //for now, does the player hold a hockey stick?
                    GamePlayer player = target as GamePlayer;
                    InventoryItem stick = player.Inventory.GetFirstItemByID("hockey_stick", eInventorySlot.TwoHandWeapon, eInventorySlot.TwoHandWeapon);
                    if (stick == null)
                        return false;
                }
                if (Body.Model == target.Model)
                    return false;
                if (target is GameNPC)
                {
                    if (Body.Faction != null)
                    {
                        if ((target as GameNPC).Faction == Body.Faction)
                            return false;
                    }
                }
                //and if we pass all the checks, we run!
                return true;

            }

            int FleeX, FleeY;
            ///<summary>Calculate flee target.</summary>
            ///<param name="target">The target to flee.</param>
            protected virtual void CalculateFleeTarget(GameLiving target)
            {
                //This is the position of the target we are fleeing from...
                Point3D stalker = new Point3D(target.X, target.Y, target.Z);
                //Calculate the angle we need to face the target...
                ushort NotTarget = Body.GetHeadingToTarget(stalker);
                Body.TargetObject = target;
                //Using this NotTarget angle, we will do exactly that - 'NotTarget' it ;)
                //Find the angle to turn to escape!
                ushort TargetAngle = (ushort)((NotTarget + 2048) % 4096);
                //We face the way we are to run
                Body.Heading = TargetAngle;
                //Start the mob walking in that direction.
                //Body.Walk(200); //We will need to cease this later in a check to see if it's safe.

                //Having found the target angle, get the spot x units in front of us that relates to this.
                Body.GetSpotFromHeading(300, out FleeX, out FleeY);
                //Now we know the X and Y positions of our target fleespot, stop us from following if we were.
                Body.StopFollow();
                Body.StopAttack();
                //Now make us walk to that target position.
                Body.WalkTo(FleeX, FleeY, Body.Z, Body.MaxSpeed);
                //And importantly, say that we are now fleeing.
                m_fleeing = true;
            }

            #endregion

            /// <summary>
            /// For Prey, aggro will probably be low. The think interval will be set to default of 1.5
            /// </summary>
            public override int ThinkInterval
            {
                get { return 1000; }
            }

            #region Aggro (for sake of override)
            /// <summary>
            /// Max Aggro range in that this npc searches for enemies
            /// </summary>
            protected int m_aggroMaxRange;
            /// <summary>
            /// Aggressive Level of this npc
            /// </summary>
            protected int m_aggroLevel;
            /// <summary>
            /// List of livings that this npc has aggro on, living => aggroamount
            /// </summary>
            protected readonly Hashtable m_aggroTable = new Hashtable();

            /// <summary>
            /// Aggressive Level in % 0..100, 0 means not Aggressive
            /// </summary>
            public virtual int AggroLevel
            {
                get { return m_aggroLevel; }
                set { m_aggroLevel = value; }
            }

            /// <summary>
            /// Range in that this npc aggros
            /// </summary>
            public virtual int AggroRange
            {
                get { return m_aggroMaxRange; }
                set { m_aggroMaxRange = value; }
            }

            /// <summary>
            /// Add living to the aggrolist
            /// aggroamount can be negative to lower amount of aggro		
            /// </summary>
            /// <param name="living"></param>
            /// <param name="aggroamount"></param>
            public virtual void AddToAggroList(GameLiving living, int aggroamount)
            {
                if (m_body.IsConfused) return;

                if (living == null) return;
                //			log.Debug(Body.Name + ": AddToAggroList="+(living==null?"(null)":living.Name)+", "+aggroamount);

                // only protect if gameplayer and aggroamout > 0
                if (living is GamePlayer && aggroamount > 0)
                {
                    GamePlayer player = (GamePlayer)living;

                    if (player.PlayerGroup != null)
                    { // player is in group, add whole group to aggro list
                        lock (m_aggroTable.SyncRoot)
                        {
                            foreach (GamePlayer groupPlayer in player.PlayerGroup.GetPlayersInTheGroup())
                            {
                                if (m_aggroTable[groupPlayer] == null)
                                {
                                    m_aggroTable[groupPlayer] = 1L;	// add the missing group member on aggro table
                                }
                            }
                        }
                    }

                    //ProtectEffect protect = (ProtectEffect) player.EffectList.GetOfType(typeof(ProtectEffect));
                    foreach (ProtectEffect protect in player.EffectList.GetAllOfType(typeof(ProtectEffect)))
                    {
                        // if no aggro left => break
                        if (aggroamount <= 0) break;

                        //if (protect==null) continue;
                        if (protect.ProtectTarget != living) continue;
                        if (protect.ProtectSource.IsStunned) continue;
                        if (protect.ProtectSource.IsMezzed) continue;
                        if (protect.ProtectSource.IsSitting) continue;
                        if (protect.ProtectSource.ObjectState != GameObject.eObjectState.Active) continue;
                        if (!protect.ProtectSource.IsAlive) continue;
                        if (!protect.ProtectSource.InCombat) continue;

                        if (!WorldMgr.CheckDistance(living, protect.ProtectSource, ProtectAbilityHandler.PROTECT_DISTANCE))
                            continue;
                        // P I: prevents 10% of aggro amount
                        // P II: prevents 20% of aggro amount
                        // P III: prevents 30% of aggro amount
                        // guessed percentages, should never be higher than or equal to 50%
                        int abilityLevel = protect.ProtectSource.GetAbilityLevel(Abilities.Protect);
                        int protectAmount = (int)((abilityLevel * 0.10) * aggroamount);

                        if (protectAmount > 0)
                        {
                            aggroamount -= protectAmount;
                            protect.ProtectSource.Out.SendMessage("You are protecting " + player.GetName(0, false) + " and distract " + Body.GetName(0, false) + ".", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                            //						player.Out.SendMessage("You are protected by " + protect.ProtectSource.GetName(0, false) + " from " + Body.GetName(0, false) + ".", eChatType.CT_System, eChatLoc.CL_SystemWindow);

                            lock (m_aggroTable.SyncRoot)
                            {
                                if (m_aggroTable[protect.ProtectSource] != null)
                                {
                                    long amount = (long)m_aggroTable[protect.ProtectSource];
                                    amount += protectAmount;
                                    m_aggroTable[protect.ProtectSource] = amount;
                                }
                                else
                                {
                                    m_aggroTable[protect.ProtectSource] = (long)protectAmount;
                                }
                            }
                        }
                    }
                }

                lock (m_aggroTable.SyncRoot)
                {
                    if (m_aggroTable[living] != null)
                    {
                        long amount = (long)m_aggroTable[living];
                        amount += aggroamount;
                        if (amount <= 0)
                        {
                            m_aggroTable.Remove(living);
                        }
                        else
                        {
                            m_aggroTable[living] = amount;
                        }
                    }
                    else
                    {
                        if (aggroamount > 0)
                        {
                            m_aggroTable[living] = (long)aggroamount;
                        }
                    }

                    
                }
            }

            /// <summary>
            /// Get current amount of aggro on aggrotable
            /// </summary>
            /// <param name="living"></param>
            /// <returns></returns>
            public virtual long GetAggroAmountForLiving(GameLiving living)
            {
                lock (m_aggroTable.SyncRoot)
                {
                    if (m_aggroTable[living] != null)
                    {
                        return (long)m_aggroTable[living];
                    }
                    return 0;
                }
            }

            /// <summary>
            /// Remove one living from aggro list
            /// </summary>
            /// <param name="living"></param>
            public virtual void RemoveFromAggroList(GameLiving living)
            {
                lock (m_aggroTable.SyncRoot)
                {
                    m_aggroTable.Remove(living);
                }
                
            }

            /// <summary>
            /// Remove all livings from the aggrolist
            /// </summary>
            public virtual void ClearAggroList()
            {
                lock (m_aggroTable.SyncRoot)
                {
                    m_aggroTable.Clear();
                }
            }

            /// <summary>
            /// Makes a copy of current aggro list
            /// </summary>
            /// <returns></returns>
            public virtual Hashtable CloneAggroList()
            {
                lock (m_aggroTable.SyncRoot)
                {
                    return (Hashtable)m_aggroTable.Clone();
                }
            }

            /// <summary>
            /// calculate the aggro of this npc against another living
            /// </summary>
            /// <param name="target"></param>
            /// <returns></returns>
            public virtual int CalculateAggroLevelToTarget(GameLiving target)
            {
                if (GameServer.ServerRules.IsSameRealm(Body, target, true)) return 0;
                if (AggroLevel >= 100) return 100;
                if (target.IsObjectGreyCon(Body)) return 0;	// only attack if green+ to target
                //if (Level <= 3) return 0;	// workaround, dont aggro for newbie mobs
                if (Body.Faction != null && target is GamePlayer)
                {
                    GamePlayer player = (GamePlayer)target;
                    AggroLevel = Body.Faction.GetAggroToFaction(player);
                }
                return AggroLevel;
            }

            #endregion



            
            #endregion
            #endregion
        }

        

        /// <summary>
        /// The Court's X position
        /// </summary>
        private static int MatchX = 336915;
        /// <summary>
        /// The Court's Y position
        /// </summary>
        private static int MatchY = 488270;
        /// <summary>
        /// X size
        /// </summary>
        private static int MatchXSize = 2000;
        /// <summary>
        /// Y size
        /// </summary>
        private static int MatchYSize = 1000;
        private static Puck m_puck;

        //Goals
        private static GoalMarker m_goal1;
        private static GoalMarker m_goal2;

        // The marker collection
        private static PostMarker m_marker1;
        private static PostMarker m_marker2;
        private static PostMarker m_marker3;
        private static PostMarker m_marker4;
        private static PostMarker m_marker5;
        private static PostMarker m_marker6;

        //score
        private static int BlueScore;
        private static int RedScore;

        //ref
        private static Judge m_ref;

        //Game being played?
        private static bool GamePlaying = false;

            //Game stats - who won, prize money etc.
            private static string m_winner = "none";
            /// <summary>
            /// Maxplayers that can play....simple, eh?
            /// </summary>
            private static int m_maxPlayers = 4;
            /// <summary>
            /// the wager, in COPPER
            /// </summary>
            private static int m_wager = 0;
            /// <summary>
            /// The starting phase before the match begins.
            /// </summary>
            private static bool m_startPhase;
        //private static int currentAddPlayer = 0;
        /// <summary>
        /// How many red players are playing.
        /// </summary>
        private static int m_redPlayers;
        /// <summary>
        /// How many blue players are playing.
        /// </summary>
        private static int m_bluePlayers;
        private static int m_playersPlaying;

        private static IList m_players;

        private static ItemTemplate hockeyStick;
        private static bool m_endingGame = false;

        [ScriptLoadedEvent]
        public static void OnScriptCompiled(DOLEvent e, object sender, EventArgs args)
        {
            //Markers*****************
            
            //Top left
            m_marker1 = new PostMarker();
            m_marker1.AddToWorld();
            m_marker1.X = MatchX - (int)Math.Round((double)(MatchXSize/2));
            m_marker1.Y = MatchY + (int)Math.Round((double)(MatchYSize / 2));
            m_marker1.CurrentRegionID = 200;
            log.Info("Marker placed");
            //Top middle
            m_marker2 = new PostMarker();
            m_marker2.AddToWorld();
            m_marker2.X = MatchX;
            m_marker2.Y = MatchY + (int)Math.Round((double)(MatchYSize / 2));
            m_marker2.CurrentRegionID = 200;
            log.Info("Marker placed");
            //Top right
            m_marker3 = new PostMarker();
            m_marker3.AddToWorld();
            m_marker3.X = MatchX + (int)Math.Round((double)(MatchXSize / 2));
            m_marker3.Y = MatchY + (int)Math.Round((double)(MatchYSize / 2));
            m_marker3.CurrentRegionID = 200;
            log.Info("Marker placed");
            //Bottom left
            m_marker4 = new PostMarker();
            m_marker4.AddToWorld();
            m_marker4.X = MatchX - (int)Math.Round((double)(MatchXSize / 2));
            m_marker4.Y = MatchY - (int)Math.Round((double)(MatchYSize / 2));
            m_marker4.CurrentRegionID = 200;
            log.Info("Marker placed");
            //Bottom middle
            m_marker5 = new PostMarker();
            m_marker5.AddToWorld();
            m_marker5.X = MatchX;
            m_marker5.Y = MatchY - (int)Math.Round((double)(MatchYSize / 2));
            m_marker5.CurrentRegionID = 200;
            log.Info("Marker placed");
            //Bottom right
            m_marker6 = new PostMarker();
            m_marker6.AddToWorld();
            m_marker6.X = MatchX + (int)Math.Round((double)(MatchXSize / 2));
            m_marker6.Y = MatchY - (int)Math.Round((double)(MatchYSize / 2));
            m_marker6.CurrentRegionID = 200;
            log.Info("Marker placed");

            //Goals
            m_goal1 = new GoalMarker();
            m_goal1.AddToWorld();
            m_goal1.X = MatchX + (int)Math.Round((double)(MatchXSize / 2));
            m_goal1.Y = MatchY;
            m_goal1.Name = "Red Goal";
            m_goal1.Size = 80;
            m_goal1.CurrentRegionID = 200;
            log.Info("Red Goal placed");

            //Goals
            m_goal2 = new GoalMarker();
            m_goal2.AddToWorld();
            m_goal2.X = MatchX - (int)Math.Round((double)(MatchXSize / 2));
            m_goal2.Y = MatchY;
            m_goal2.Name = "Blue Goal";
            m_goal2.Size = 80;
            m_goal2.CurrentRegionID = 200;
            log.Info("Blue Goal placed");

            //Puck*******************
            m_puck = new Puck();
            m_puck.AddToWorld();

            BlueScore = 0;
            RedScore = 0;
            log.Info("Puck placed");


            //Ref.
            m_ref = new Judge();
            m_ref.AddToWorld();

            //Items - the two hockey hockeySticks.
            hockeyStick = (ItemTemplate)GameServer.Database.FindObjectByKey(typeof(ItemTemplate), "hockey_stick");
            if (hockeyStick == null)
            {
                hockeyStick = new ItemTemplate();
                hockeyStick.Name = "Hockey Stick";
                if (log.IsWarnEnabled)
                    log.Warn("Could not find " + hockeyStick.Name + " , creating it ...");

                hockeyStick.Weight = 10;
                hockeyStick.Model = 19;
                hockeyStick.Hand = 1;
                hockeyStick.Item_Type = 12;
                hockeyStick.Object_Type = (int)eObjectType.Staff;
                
                hockeyStick.Id_nb = "hockey_stick";
                hockeyStick.IsPickable = false;
                hockeyStick.IsDropable = false;
            }

            if (log.IsInfoEnabled)
                log.Info("HockeyEvent initialized");
        }

        [ScriptUnloadedEvent]
        public static void OnScriptUnloaded(DOLEvent e, object sender, EventArgs args)
        {
            //marker deletion
            if (m_marker1 != null)
                m_marker1.Delete();
            if (m_marker2 != null)
                m_marker2.Delete();
            if (m_marker3 != null)
                m_marker3.Delete();
            if (m_marker4 != null)
                m_marker4.Delete();
            if (m_marker5 != null)
                m_marker5.Delete();
            if (m_marker6 != null)
                m_marker6.Delete();

            //goal deletion
            if (m_goal1 != null)
                m_goal1.Delete();
            if (m_goal2 != null)
                m_goal2.Delete();

            //puck deletion
            if (m_puck != null)
                m_puck.Delete();

            //ref deletion
            if (m_ref != null)
                m_ref.Delete();
        }


    }

}