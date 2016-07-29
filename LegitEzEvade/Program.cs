using System;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace LegitEzEvade
{
    class Program
    {
        #region Variables
        private static int loadingDelayStart;
        private static int loadingDelayValue = 1000; //ms
        private static int tickLimiterOld;
        private static int tickLimiterValue = 200; //ms
        private static int oldLevel = 0;
        private static int currentLevel;
        private static Menu MainMenu;
        private static Boolean restored = false;
        private static Boolean error = false;
        private static Obj_AI_Base Player;
        #endregion

        #region Bootstrap
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += _OnGameLoad;
        }
        private static void _OnGameLoad (EventArgs args)
        {
            loadingDelayStart = Environment.TickCount;
            Game.OnUpdate += LoadingDelayHandler;
            Drawing.OnDraw += _OnDraw;
            Player = ObjectManager.Player;
        }
        private static void LoadingDelayHandler(EventArgs args)
        {
            if (Environment.TickCount - loadingDelayStart > loadingDelayValue)
            {
                BackUpStore();
                Game.OnUpdate -= LoadingDelayHandler;
                if (!Menu.GetMenu("EzEvade", "ezEvade").IsRootMenu) return;
                LoadMenu();
                tickLimiterOld = Environment.TickCount;
                Game.OnUpdate += _OnUpdate;
                
                
            }
        }
        #endregion

        #region Core
        private static void _OnUpdate (EventArgs args)
        {
            if (Environment.TickCount - tickLimiterOld >= tickLimiterValue)
            {
                if (MainMenu.Item("threshold2").GetValue<Slider>().Value >= MainMenu.Item("threshold1").GetValue<Slider>().Value)
                {
                    if (!error)Game.PrintChat("<font color='#ff0000'>INCORRECT VALUES !</font> threshold 1 must be greater than treshold 2");
                    error = true;
                    return;
                }

                //Magic here
                error = false;
                Boolean enabled = MainMenu.Item("enable").GetValue<KeyBind>().Active;
                Boolean useExtra = MainMenu.Item("useExtra").GetValue<Boolean>();
                Boolean isPanicMode = MainMenu.Item("panic").GetValue<KeyBind>().Active;

                if (enabled)
                {
                    restored = false;
                    if (isPanicMode)
                    {
                        ChangeLevel(3);
                    }
                    else
                    {
                        if (useExtra)
                        {
                            int numAllies = 0; //me duh
                            float alliesPerHealth = 0;

                            float ennemiesPerHealth = 0;
                            int numEnemies = 0;
                            var scanRange = MainMenu.Item("scan").GetValue<Slider>().Value;

                            foreach (var ally in HeroManager.Allies)
                            {
                                if (!ally.IsDead && ally.Distance(Player) <= scanRange)
                                {
                                    alliesPerHealth += ally.HealthPercent;
                                    numAllies += 1;
                                }
                            }
                            foreach (var enemy in HeroManager.Enemies)
                            {
                                if (!enemy.IsDead && enemy.Distance(Player) <= scanRange)
                                {
                                    ennemiesPerHealth += enemy.HealthPercent;
                                    numEnemies += 1;
                                }
                            }
                            //divide
                            if (numAllies > 0) alliesPerHealth = alliesPerHealth / numAllies;
                            if (numEnemies > 0) ennemiesPerHealth = ennemiesPerHealth / numEnemies;

                            if (Player.HealthPercent >= MainMenu.Item("threshold1").GetValue<Slider>().Value)
                            {
                                if (numAllies - numEnemies >= MainMenu.Item("dis").GetValue<Slider>().Value) //Safe
                                {
                                    ChangeLevel(1);
                                }
                                else
                                {
                                    ChangeLevel(2);
                                }
                            }
                            else if (Player.HealthPercent < MainMenu.Item("threshold1").GetValue<Slider>().Value && Player.HealthPercent >= MainMenu.Item("threshold2").GetValue<Slider>().Value)
                            {
                                ChangeLevel(2);
                            }
                            else
                            {
                                if (numAllies - numEnemies >= MainMenu.Item("adv").GetValue<Slider>().Value || alliesPerHealth - ennemiesPerHealth >= MainMenu.Item("hpDiff").GetValue<Slider>().Value) //Safe
                                {
                                    ChangeLevel(2);
                                }
                                else
                                {
                                    ChangeLevel(3);
                                }
                            }

                            //Game.PrintChat("Allies " + numAllies + " | Ennemies : " + numEnemies + " | PerAlly : " + alliesPerHealth + " | PerEnemy : " + ennemiesPerHealth + " - Level : " + oldLevel.ToString());

                        }
                        else
                        {
                            if (Player.HealthPercent >= MainMenu.Item("threshold1").GetValue<Slider>().Value)
                            {
                                ChangeLevel(1);
                            }
                            else if (Player.HealthPercent < MainMenu.Item("threshold1").GetValue<Slider>().Value && Player.HealthPercent >= MainMenu.Item("threshold2").GetValue<Slider>().Value)
                            {
                                ChangeLevel(2);
                            }
                            else
                            {
                                ChangeLevel(3);
                            }
                        }
                    }

                }
                else
                {
                    if (!restored)
                    {
                        BackUpRestore();
                        restored = true;
                    }
                    oldLevel = 0;
                }
                tickLimiterOld = Environment.TickCount;
            }
        }

        private static void _OnDraw (EventArgs args)
        {
            string statusText = "";
            Color statusColor = Color.White;

            if (MainMenu.Item("enable").GetValue<KeyBind>().Active)
            {
                if (oldLevel == 1)
                {
                    statusText = "LvL 1 - Safe";
                    statusColor = Color.DarkGreen;
                } else if (oldLevel == 2)
                {
                    statusText = "LvL 2 - Danger";
                    statusColor = Color.Yellow;
                } else if (oldLevel == 3)
                {
                    statusText = "LvL 3 - Extreme";
                    statusColor = Color.Red;
                }

                Drawing.DrawText(Drawing.WorldToScreen(Player.Position).X - 10, Drawing.WorldToScreen(Player.Position).Y + 20, statusColor, statusText);
            }

        }

        #endregion


        #region Menu
        private static void LoadMenu()
        {
            MainMenu = new Menu("LegitEzEvade", "MainMenu", true);

            Menu lvlOne = new Menu("LvL 1 : Low Danger", "lvlOne");
            Menu lvlTwo = new Menu("LvL 2 : Medium Danger", "lvlTwo");
            Menu lvlThree = new Menu("LvL 3 : Extreme Danger", "lvlThree");

            lvlOne.AddItem(new MenuItem("lvlOneOD", "Only Dangerous").SetValue(true));
            lvlOne.AddItem(new MenuItem("lvlOneODC", "Only Dangerous On Combo").SetValue(true));
            lvlOne.AddItem(new MenuItem("lvlOneFOW", "FOW Dodging").SetValue(false));
            lvlOne.AddItem(new MenuItem("lvlOneCIR", "Circular Skillshots Dodging").SetValue(true));
            lvlOne.AddItem(new MenuItem("lvlOneEX", "Extended Dodging").SetValue(false));
            lvlOne.AddItem(new MenuItem("lvlOneCO", "Collision").SetValue(false));
            lvlOne.AddItem(new MenuItem("lvlOneMinR", "Minimum Reaction time")).SetValue(new Slider(100, 0, 500));
            lvlOne.AddItem(new MenuItem("lvlOneMaxR", "Maximum Reaction time")).SetValue(new Slider(200, 0, 500));
            lvlOne.AddItem(new MenuItem("lvlOneProfile", "Mode").SetValue<StringList>(new StringList(new[] { "Smooth", "Very Smooth", "Fastest" }, 2)));

            lvlTwo.AddItem(new MenuItem("lvlTwoOD", "Only Dangerous").SetValue(false));
            lvlTwo.AddItem(new MenuItem("lvlTwoODC", "Only Dangerous On Combo").SetValue(true));
            lvlTwo.AddItem(new MenuItem("lvlTwoFOW", "FOW Dodging").SetValue(true));
            lvlTwo.AddItem(new MenuItem("lvlTwoCIR", "Circular Skillshots Dodging").SetValue(true));
            lvlTwo.AddItem(new MenuItem("lvlTwoEX", "Extended Dodging").SetValue(false));
            lvlTwo.AddItem(new MenuItem("lvlTwoCO", "Collision").SetValue(true));
            lvlTwo.AddItem(new MenuItem("lvlTwoMinR", "Minimum Reaction time")).SetValue(new Slider(100, 0, 500));
            lvlTwo.AddItem(new MenuItem("lvlTwoMaxR", "Maximum Reaction time")).SetValue(new Slider(200, 0, 500));
            lvlTwo.AddItem(new MenuItem("lvlTwoProfile", "Mode").SetValue<StringList>(new StringList(new[] { "Smooth", "Very Smooth", "Fastest" }, 0)));

            lvlThree.AddItem(new MenuItem("lvlThreeOD", "Only Dangerous").SetValue(false));
            lvlThree.AddItem(new MenuItem("lvlThreeODC", "Only Dangerous On Combo").SetValue(false));
            lvlThree.AddItem(new MenuItem("lvlThreeFOW", "FOW Dodging").SetValue(true));
            lvlThree.AddItem(new MenuItem("lvlThreeCIR", "Circular Skillshots Dodging").SetValue(true));
            lvlThree.AddItem(new MenuItem("lvlThreeEX", "Extended Dodging").SetValue(false));
            lvlThree.AddItem(new MenuItem("lvlThreeCO", "Collision").SetValue(false));
            lvlThree.AddItem(new MenuItem("lvlThreeMinR", "Minimum Reaction time")).SetValue(new Slider(0, 0, 500));
            lvlThree.AddItem(new MenuItem("lvlThreeMaxR", "Maximum Reaction time")).SetValue(new Slider(100, 0, 500));
            lvlThree.AddItem(new MenuItem("lvlThreeProfile", "Mode").SetValue<StringList>(new StringList(new[] { "Smooth", "Very Smooth", "Fastest" }, 2)));

            MainMenu.AddSubMenu(lvlOne);
            MainMenu.AddSubMenu(lvlTwo);
            MainMenu.AddSubMenu(lvlThree);

            MainMenu.AddItem(new MenuItem("sep", "- Setup"));

            MainMenu.AddItem(new MenuItem("threshold1", "Health % threshold 1").SetValue(new Slider(70, 1, 99)).SetTooltip("LvL 1 : threshold 1 -> 100% | LvL 2 : threshold 2 -> threshold 1 | LvL 3 : 0% -> threshold 2"));
            MainMenu.AddItem(new MenuItem("threshold2", "Health % threshold 2").SetValue(new Slider(30, 1, 99)).SetTooltip("LvL 1 : threshold 1 -> 100% | LvL 2 : threshold 2 -> threshold 1 | LvL 3 : 0% -> threshold 2"));
            MainMenu.AddItem(new MenuItem("enable", "Enable / Disable").SetValue(new KeyBind('T', KeyBindType.Toggle, true)));
            MainMenu.AddItem(new MenuItem("panic", "Panic Key").SetValue(new KeyBind('Z', KeyBindType.Press)).SetTooltip("Will force LvL 3 : Extreme"));


            MainMenu.AddItem(new MenuItem("sep2", "- Extra safety checks").SetTooltip("Advanced settings, leave as provided or read thread"));

            MainMenu.AddItem(new MenuItem("useExtra", "Enable extra safety checks")).SetValue(true);
            MainMenu.AddItem(new MenuItem("scan", "Scan Range")).SetValue(new Slider(2500, 400, 5000));
            MainMenu.AddItem(new MenuItem("dis", "Number Disadvantage")).SetValue(new Slider(0, 0, 4));
            MainMenu.AddItem(new MenuItem("adv", "Number Advantage")).SetValue(new Slider(2, 0, 4));
            MainMenu.AddItem(new MenuItem("hpDiff", "HP% Advantage")).SetValue(new Slider(30, 0, 100));

            MainMenu.AddToMainMenu();

        }
        #endregion

        #region Utility
        private static void ChangeItemBool (string itemName, Boolean newValue)
        {
            Menu.GetMenu("EzEvade", "ezEvade").Item(itemName).SetValue(newValue);
        }
        private static void ChangeItemBoolKeybind(string itemName, Boolean newValue)
        {
            var userKey = Menu.GetMenu("EzEvade", "ezEvade").Item(itemName).GetValue<KeyBind>().Key;
            var keybindType = Menu.GetMenu("EzEvade", "ezEvade").Item(itemName).GetValue<KeyBind>().Type;
            Menu.GetMenu("EzEvade", "ezEvade").Item(itemName).SetValue<KeyBind>(new KeyBind(userKey, keybindType, newValue));
        }
        private static void ChangeItemSlider(string itemName, int newValue)
        {
            var menuMin = Menu.GetMenu("EzEvade", "ezEvade").Item(itemName).GetValue<Slider>().MinValue;
            var menuMax = Menu.GetMenu("EzEvade", "ezEvade").Item(itemName).GetValue<Slider>().MaxValue;
            Menu.GetMenu("EzEvade", "ezEvade").Item(itemName).SetValue<Slider>(new Slider(newValue, menuMin, menuMax));
        }

        private static void ChangeLevel(int level)
        {
            if (oldLevel != level || oldLevel == 0)
            {
                if (level == 1)
                {
                    ExMenu("lvlOne");
                    oldLevel = level;
                } else if (level == 2)
                {
                    ExMenu("lvlTwo");
                    oldLevel = level;
                } else
                {
                    ExMenu("lvlThree");
                    oldLevel = level;
                }
            }
        }

        private static void ExMenu (string levelMenu)
        {
            var OD = MainMenu.Item(levelMenu+"OD").GetValue<Boolean>();
            var ODC = MainMenu.Item(levelMenu+"ODC").GetValue<Boolean>();
            var FOW = MainMenu.Item(levelMenu+"FOW").GetValue<Boolean>();
            var CIR = MainMenu.Item(levelMenu+"CIR").GetValue<Boolean>();
            var EX = MainMenu.Item(levelMenu+"EX").GetValue<Boolean>();
            var CO = MainMenu.Item(levelMenu+"CO").GetValue<Boolean>();
            var MinR = MainMenu.Item(levelMenu+"MinR").GetValue<Slider>().Value;
            var MaxR = MainMenu.Item(levelMenu+"MaxR").GetValue<Slider>().Value;
            var Profile = MainMenu.Item(levelMenu+"Profile").GetValue<StringList>().SelectedIndex;
            int MODE;

            if (Profile == 0) MODE = 0;
            else if (Profile == 1) MODE = 2;
            else MODE = 1;
       
            Random rnd = new Random();
            int RT = rnd.Next(MinR, MaxR);

            ChangeItemBool("DodgeDangerous", OD);
            ChangeItemBool("DodgeDangerousKeyEnabled", ODC);
            ChangeItemBool("DodgeFOWSpells", FOW);
            ChangeItemBool("DodgeCircularSpells", CIR);
            ChangeItemBool("EnableEvadeDistance", EX);
            ChangeItemBool("CheckSpellCollision", CO);
            ChangeItemSlider("ReactionTime", RT);
            Menu.GetMenu("EzEvade", "ezEvade").Item("EvadeMode").SetValue(new StringList(new[] { "Smooth", "Fastest", "Very Smooth", "Hawk", "Kurisu" }, MODE));
        }

        private static Boolean ODB;
        private static Boolean ODCB;
        private static Boolean FOWB;
        private static Boolean CIRB;
        private static Boolean EXB;
        private static Boolean COB;
        private static int RTB;
        private static int MODEB;

        private static void BackUpStore()
        {
            ODB = Menu.GetMenu("EzEvade", "ezEvade").Item("DodgeDangerous").GetValue<Boolean>();
            ODCB = Menu.GetMenu("EzEvade", "ezEvade").Item("DodgeOnlyOnComboKeyEnabled").GetValue<Boolean>();
            FOWB = Menu.GetMenu("EzEvade", "ezEvade").Item("DodgeFOWSpells").GetValue<Boolean>();
            CIRB = Menu.GetMenu("EzEvade", "ezEvade").Item("DodgeCircularSpells").GetValue<Boolean>();
            EXB = Menu.GetMenu("EzEvade", "ezEvade").Item("EnableEvadeDistance").GetValue<Boolean>();
            COB = Menu.GetMenu("EzEvade", "ezEvade").Item("CheckSpellCollision").GetValue<Boolean>();
            RTB = Menu.GetMenu("EzEvade", "ezEvade").Item("ReactionTime").GetValue<Slider>().Value;
            MODEB = Menu.GetMenu("EzEvade", "ezEvade").Item("EvadeMode").GetValue<StringList>().SelectedIndex;
        }
        private static void BackUpRestore()
        {
            ChangeItemBool("DodgeDangerous", ODB);
            ChangeItemBool("DodgeDangerousKeyEnabled", ODCB);
            ChangeItemBool("DodgeFOWSpells", FOWB);
            ChangeItemBool("DodgeCircularSpells", CIRB);
            ChangeItemBool("EnableEvadeDistance", EXB);
            ChangeItemBool("CheckSpellCollision", COB);
            ChangeItemSlider("ReactionTime", RTB);
            Menu.GetMenu("EzEvade", "ezEvade").Item("EvadeMode").SetValue(new StringList(new[] { "Smooth", "Fastest", "Very Smooth", "Hawk", "Kurisu" }, MODEB));
            Game.PrintChat("Restored EzEvade settings");
        }
    #endregion
}
}
