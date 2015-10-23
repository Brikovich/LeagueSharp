using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Drawing;
using Color = System.Drawing.Color;


namespace BrikoEzEvade_Plugin
{
    class Program
    {
        public static Obj_AI_Base myHero { get { return ObjectManager.Player; } }
        public static string infoText;
        public static bool debug = true;
        public static bool hp_ras;
        private static Menu _Menu;
        private static bool oldactivated =false;
        private static bool activated;
        static int scanRange = 1000;
        static int alliesInRange = 0;
        static int oldAlliesInRange = 6;
        static int ennemiesInRange = 0;
        static int oldEnnemiesInRange = 6;
        static bool newRT = true;
        private static float oldHP=0;
        private static float currentHP=myHero.HealthPercent;
        private static int oldLevel = 4;
        private static int currentLevel;
        private static float lastTick = Environment.TickCount;
        private static int randomRT = 150;
        private static bool dodgeOrNah = true;


        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += _OnGameLoad;

        }
        #region OnDraw
        private static void _OnDraw(EventArgs Args)
        {
            string statusText = "Loading..";
            Color statusColor = Color.Gray;

            //Errors handler
            if (!hp_ras)
            {
                statusText = "ERROR ! Please make sure that levels HP% values are in ascending order.";
                statusColor = Color.Red;
            }

            //Activated or nah
            if (activated && hp_ras)
            {
                oldactivated = false;
                statusText = "BrikoPlugin : " + currentLevel;
                //KappaPride
                if (currentLevel == 1) statusColor = Color.LawnGreen;
                else if (currentLevel == 2) statusColor = Color.Yellow;
                else if (currentLevel == 3) statusColor = Color.Red;
                else statusColor = Color.Black;
            }
            else if (!activated && hp_ras)
            {
                statusText = "BrikoPlugin : OFF";
                statusColor = Color.Gray;
                oldHP = 0;

                if (!oldactivated)
                {
                    loadBackupEzEvade();
                    Game.PrintChat(System.DateTime.Now.ToString() + " - EzEvade settings restored");
                    oldactivated = true;
                }
            }

            //Draw status under player
            Drawing.DrawText(Drawing.WorldToScreen(myHero.Position)[0]-Drawing.GetTextExtent(statusText).Width/2-3, Drawing.WorldToScreen(myHero.Position)[1]+13, statusColor, statusText);



        }
        #endregion OnDraw

        #region OnGameLoad
        static void _OnGameLoad(EventArgs Args)
        {
            Game.OnUpdate += _OnGameUpdate;
            CustomEvents.Game.OnGameEnd += _OnGameEnd;
            Drawing.OnDraw += _OnDraw;

            // Initialize menu
            iniMenu();

            // Welcome message  
            Game.PrintChat("Briko EzEvade Plugin Loaded - By Brikovich");
            
            // Backup current ezEvade settings
            backupEzEvade();


        }
        #endregion OnGameLoad
        #region OnGameUpdate
        private static void _OnGameUpdate (EventArgs Args)
        {
           
            if (_Menu.SubMenu("levels").SubMenu("lvl1").Item("minhp_lvl1").GetValue<Slider>().Value > _Menu.SubMenu("levels").SubMenu("lvl2").Item("minhp_lvl2").GetValue<Slider>().Value && _Menu.SubMenu("levels").SubMenu("lvl2").Item("minhp_lvl2").GetValue<Slider>().Value > _Menu.SubMenu("levels").SubMenu("lvl3").Item("minhp_lvl3").GetValue<Slider>().Value && _Menu.SubMenu("levels").SubMenu("lvl3").Item("minhp_lvl3").GetValue<Slider>().Value > _Menu.SubMenu("levels").SubMenu("lvl4").Item("minhp_lvl4").GetValue<Slider>().Value)
            {
                hp_ras = true;
            }
            else
            {
                hp_ras = false;
            }
            if (_Menu.Item("enable").GetValue<KeyBind>().Active) activated = true;
            else activated = false;

            scanRange = _Menu.Item("range").GetValue<Slider>().Value;

            //stock realtime currentHP%
            currentHP = myHero.HealthPercent;

            //OnHpChange >= 1% to avoid unecessary fps drop in case no damge is taken
            if (Math.Abs(currentHP-oldHP) >= 1 && activated && hp_ras)
            {
                
                //Level 1
                if (currentHP >= _Menu.SubMenu("levels").SubMenu("lvl1").Item("minhp_lvl1").GetValue<Slider>().Value)
                {
                    currentLevel = 1;
                }
                //Level 2
                else if (currentHP < _Menu.SubMenu("levels").SubMenu("lvl1").Item("minhp_lvl1").GetValue<Slider>().Value && currentHP >= _Menu.SubMenu("levels").SubMenu("lvl2").Item("minhp_lvl2").GetValue<Slider>().Value)
                {
                    currentLevel = 2;
                }
                //Level 3
                else if (currentHP < _Menu.SubMenu("levels").SubMenu("lvl2").Item("minhp_lvl2").GetValue<Slider>().Value && currentHP >= _Menu.SubMenu("levels").SubMenu("lvl3").Item("minhp_lvl3").GetValue<Slider>().Value)
                {
                    currentLevel = 3;
                }
                //Level 4
                else if (currentHP < _Menu.SubMenu("levels").SubMenu("lvl3").Item("minhp_lvl3").GetValue<Slider>().Value)
                {
                    currentLevel = 4;
                }

                oldHP = currentHP;
            }

            //Levels manager
            if(currentLevel != oldLevel || alliesInRange != oldAlliesInRange || ennemiesInRange != oldEnnemiesInRange || newRT)
            {

                EzEvadeConfigChangePlsDontSueMeMayomie(
                    _Menu.SubMenu("levels").SubMenu("lvl" + currentLevel.ToString()).Item("ally_lvl" + currentLevel.ToString()).GetValue<Slider>().Value,
                    _Menu.SubMenu("levels").SubMenu("lvl" + currentLevel.ToString()).Item("ennemy_lvl" + currentLevel.ToString()).GetValue<Slider>().Value,
                    _Menu.SubMenu("levels").SubMenu("lvl" + currentLevel.ToString()).Item("time_lvl" + currentLevel.ToString()).GetValue<Slider>().Value,
                    _Menu.SubMenu("levels").SubMenu("lvl" + currentLevel.ToString()).Item("mode_lvl" + currentLevel.ToString()).GetValue<StringList>().SelectedIndex,
                    _Menu.SubMenu("levels").SubMenu("lvl" + currentLevel.ToString()).Item("tower_lvl" + currentLevel.ToString()).GetValue<Boolean>(),
                    _Menu.SubMenu("levels").SubMenu("lvl" + currentLevel.ToString()).Item("circular_lvl" + currentLevel.ToString()).GetValue<Boolean>(),
                    _Menu.SubMenu("levels").SubMenu("lvl" + currentLevel.ToString()).Item("fow_lvl" + currentLevel.ToString()).GetValue<Boolean>(),
                    _Menu.SubMenu("levels").SubMenu("lvl" + currentLevel.ToString()).Item("collision_lvl" + currentLevel.ToString()).GetValue<Boolean>()
                        );
                oldEnnemiesInRange = ennemiesInRange;
                oldAlliesInRange = alliesInRange;
                oldLevel = currentLevel;
                newRT = false;
            }

            //Calculate randomm stuff #EvadeHumanizer Kreygasm

            if (Environment.TickCount - lastTick > _Menu.Item("interval").GetValue<Slider>().Value * 1000)

            {
                newRT = true;
                //ReactionTime
                Random reactionRandom = new Random();
                randomRT = reactionRandom.Next(_Menu.SubMenu("levels").SubMenu("lvl" + currentLevel.ToString()).Item("rtmin_lvl" + currentLevel.ToString()).GetValue<Slider>().Value, _Menu.SubMenu("levels").SubMenu("lvl" + currentLevel.ToString()).Item("rtmax_lvl" + currentLevel.ToString()).GetValue<Slider>().Value + 1);
                lastTick = Environment.TickCount;
                //Evade status
                Random dodgingGen = new Random();
                var dodgingPerc = dodgingGen.Next(100);
                if (dodgingPerc <= _Menu.SubMenu("levels").SubMenu("lvl" + currentLevel.ToString()).Item("disable_lvl" + currentLevel.ToString()).GetValue<Slider>().Value) dodgeOrNah = false;
                else dodgeOrNah = true;

                


            }
            //Allies & ennemies in range

            //Gibe allies in range
            var actualAlliesInRange = 0;
            foreach (var ally in HeroManager.Allies)
            { 
                if (ally.Distance(myHero) <= scanRange)
                {
                    actualAlliesInRange += 1;
                }
                alliesInRange = actualAlliesInRange - 1;
            }
            //Gibe ennemies in range
            var actualEnnemiesInRange = 0;
            foreach (var ennemy in HeroManager.Enemies)
            {
                if (ennemy.Distance(myHero) <= scanRange)
                {
                    actualEnnemiesInRange += 1;
                }
                ennemiesInRange = actualEnnemiesInRange;
            }

            

        }
        #endregion OnGameUpdate
        #region OnGameEnd
        static void _OnGameEnd (EventArgs Args)
        {
            //Restore ezEvade settings
            loadBackupEzEvade();
        }
        #endregion OnGameEnd

        #region Menu


        private static int hp;
        private static int chance;
        private static int ally;
        private static int ennemy;
        private static int time;
        private static int minrt;
        private static int maxrt;
        private static int mode;
        private static bool underTower;
        private static bool circular;
        private static bool FOW;
        private static bool collision;

        static void iniMenu()
        {
            _Menu = new Menu("BrikoEzEvade Plugin", "mainMenu");
            _Menu.AddItem(new MenuItem("enable", "Enable").SetValue(new KeyBind('T', KeyBindType.Toggle, true)));
            _Menu.AddItem(new MenuItem("range", "Scan Range").SetTooltip("Maximum range to scan for ally / ennemy").SetValue(new Slider(1000, 100, 3000)));
            _Menu.AddItem(new MenuItem("interval", "Recalculate random interval").SetTooltip("Time in sec to recalculate ezvade on/off and reaction time based on ranges defined").SetValue(new Slider(10, 1, 120)));
            var levels = new Menu("Levels Settings", "levels");
            var lvl1menu = new Menu("Level 1 - Low Danger", "lvl1");
            createLevelMenu(lvl1menu, 1);
            var lvl2menu = new Menu("Level 2 - Medium Danger", "lvl2");
            createLevelMenu(lvl2menu, 2);
            var lvl3menu = new Menu("Level 3 - High Danger", "lvl3");
            createLevelMenu(lvl3menu, 3);
            var lvl4menu = new Menu("Level 4 - Extreme", "lvl4");
            createLevelMenu(lvl4menu, 4);
            levels.AddSubMenu(lvl1menu);
            levels.AddSubMenu(lvl2menu);
            levels.AddSubMenu(lvl3menu);
            levels.AddSubMenu(lvl4menu);

            _Menu.AddSubMenu(levels);

            Menu.GetMenu("ezEvade", "ezEvade").AddSubMenu(_Menu);

        }

        static void createLevelMenu(Menu gibeMenu, int gibeLevel)
        {

            gibeDefault(gibeLevel);

            gibeMenu.AddItem(new MenuItem("minhp_lvl" + gibeLevel.ToString(), "Activate untill : HP%").SetValue(new Slider(hp, 0, 100)));
            gibeMenu.AddItem(new MenuItem("disable_lvl" + gibeLevel.ToString(), "Disable ezEvade chance% (only dangerous)").SetValue(new Slider(chance, 0, 100)));
            gibeMenu.AddItem(new MenuItem("ally_lvl" + gibeLevel.ToString(), "^ Only if min X ally around").SetValue(new Slider(ally, 0, 5)));
            gibeMenu.AddItem(new MenuItem("ennemy_lvl" + gibeLevel.ToString(), "^ Only if max X ennemy around").SetValue(new Slider(ennemy, 0, 5)));
            gibeMenu.AddItem(new MenuItem("time_lvl" + gibeLevel.ToString(), "^ Don't disable after X minutes").SetValue(new Slider(time, 0, 120)));
            gibeMenu.AddItem(new MenuItem("rtmin_lvl" + gibeLevel.ToString(), "Min Reaction Time").SetValue(new Slider(minrt, 0, 500)));
            gibeMenu.AddItem(new MenuItem("rtmax_lvl" + gibeLevel.ToString(), "Max Reaction Time").SetValue(new Slider(maxrt, 0, 500)));
            gibeMenu.AddItem(new MenuItem("mode_lvl" + gibeLevel.ToString(), "Dodging Mode").SetValue(new StringList(new[] { "Smooth", "Fastest", "Very Smooth" }, mode)));
            gibeMenu.AddItem(new MenuItem("tower_lvl" + gibeLevel.ToString(), "Dodge under Towers").SetValue(underTower));
            gibeMenu.AddItem(new MenuItem("circular_lvl" + gibeLevel.ToString(), "Dodge circular").SetValue(circular));
            gibeMenu.AddItem(new MenuItem("fow_lvl" + gibeLevel.ToString(), "Dodge FOW").SetValue(FOW));
            gibeMenu.AddItem(new MenuItem("collision_lvl" + gibeLevel.ToString(), "Check Collision").SetValue(collision));

        }
       static void gibeDefault(int gibeLevel)
        {
            switch (gibeLevel)
            {
                case 1:
                    hp = 80;
                    chance = 50;
                    ally = 0;
                    ennemy = 2;
                    time = 20;
                    minrt = 100;
                    maxrt = 200;
                    mode = 0;
                    underTower = false;
                    circular = false;
                    FOW = false;
                    collision = true;
                    break;
                case 2:
                    hp = 50;
                    chance = 25;
                    ally = 0;
                    ennemy = 1;
                    time = 25;
                    minrt = 80;
                    maxrt = 150;
                    mode = 0;
                    underTower = false;
                    circular = true;
                    FOW = false;
                    collision = true;
                    break;
                case 3:
                    hp = 20;
                    chance = 15;
                    ally = 2;
                    ennemy = 2;
                    time = 25;
                    minrt = 50;
                    maxrt = 100;
                    mode = 0;
                    underTower = true;
                    circular = true;
                    FOW = true;
                    collision = false;
                    break;
                case 4:
                    hp = 0;
                    chance = 0;
                    ally = 0;
                    ennemy = 0;
                    time = 0;
                    minrt = 0;
                    maxrt = 100;
                    mode = 1;
                    underTower = true;
                    circular = true;
                    FOW = true;
                    collision = false;
                    break;
            }
        }

        #endregion Menu
        #region EzEvadeConfigChanger
        private static void EzEvadeConfigChangePlsDontSueMeMayomie(int minAlly, int minEnemy, int afterTime, int dogingMode, bool underTower, bool circular, bool FOW, bool collision)
        {
            //Disable EzEvade Sometimes
            if (alliesInRange >= minAlly && ennemiesInRange <= minEnemy && (Game.ClockTime+30)/60 <= afterTime && dodgeOrNah == false)
            {
                Menu.GetMenu("ezEvade", "ezEvade").Item("DodgeDangerous").SetValue<Boolean>(true);
            }
            else
            {
                Menu.GetMenu("ezEvade", "ezEvade").Item("DodgeDangerous").SetValue<Boolean>(false);
            }

            //Other settings
            Menu.GetMenu("ezEvade", "ezEvade").Item("ReactionTime").SetValue<Slider>(new Slider(randomRT, 0, 500));
            Menu.GetMenu("ezEvade", "ezEvade").Item("EvadeMode").SetValue<StringList>(new StringList(new[] { "Smooth", "Fastest", "Very Smooth" }, dogingMode));
            Menu.GetMenu("ezEvade", "ezEvade").Item("PreventDodgingUnderTower").SetValue<Boolean>(underTower);
            Menu.GetMenu("ezEvade", "ezEvade").Item("DodgeCircularSpells").SetValue<Boolean>(circular);
            Menu.GetMenu("ezEvade", "ezEvade").Item("DodgeFOWSpells").SetValue<Boolean>(FOW);
            Menu.GetMenu("ezEvade", "ezEvade").Item("CheckSpellCollision").SetValue<Boolean>(collision);


        }
        #endregion EzEvadeConfigChanger
        #region backupEzEvade
        private static int backuprt;
        private static int backupmode;
        private static bool backupunderTower;
        private static bool backupcircular;
        private static bool backupFOW;
        private static bool backupcollision;
        private static bool backuponlyDangerous;
        private static bool backupactivated;
        static void backupEzEvade()
        {
            backuprt = Menu.GetMenu("ezEvade", "ezEvade").Item("ReactionTime").GetValue<Slider>().Value;
            backupmode = Menu.GetMenu("ezEvade", "ezEvade").Item("EvadeMode").GetValue<StringList>().SelectedIndex;
            backupunderTower = !Menu.GetMenu("ezEvade", "ezEvade").Item("PreventDodgingUnderTower").GetValue<bool>();
            backupcircular = Menu.GetMenu("ezEvade", "ezEvade").Item("DodgeCircularSpells").GetValue<bool>();
            backupFOW = Menu.GetMenu("ezEvade", "ezEvade").Item("DodgeFOWSpells").GetValue<bool>();
            backupcollision = Menu.GetMenu("ezEvade", "ezEvade").Item("CheckSpellCollision").GetValue<bool>();
            backuponlyDangerous = Menu.GetMenu("ezEvade", "ezEvade").Item("DodgeDangerous").GetValue<bool>();
            backupactivated = Menu.GetMenu("ezEvade", "ezEvade").Item("DodgeSkillshots").GetValue<KeyBind>().Active;
        }
        static void loadBackupEzEvade()
        {
            var keyBind = Menu.GetMenu("ezEvade", "ezEvade").Item("DodgeSkillShots").GetValue<KeyBind>();
            Menu.GetMenu("ezEvade", "ezEvade").Item("ReactionTime").SetValue<Slider>(new Slider(backuprt, 0, 500));
            Menu.GetMenu("ezEvade", "ezEvade").Item("EvadeMode").SetValue<StringList>(new StringList(new[] { "Smooth", "Fastest", "Very Smooth" }, backupmode));
            Menu.GetMenu("ezEvade", "ezEvade").Item("PreventDodgingUnderTower").SetValue<Boolean>(backupunderTower);
            Menu.GetMenu("ezEvade", "ezEvade").Item("DodgeCircularSpells").SetValue<Boolean>(backupcircular);
            Menu.GetMenu("ezEvade", "ezEvade").Item("DodgeFOWSpells").SetValue<Boolean>(backupFOW);
            Menu.GetMenu("ezEvade", "ezEvade").Item("CheckSpellCollision").SetValue<Boolean>(backupcollision);
            Menu.GetMenu("ezEvade", "ezEvade").Item("DodgeDangerous").SetValue<Boolean>(backuponlyDangerous);
            Menu.GetMenu("ezEvade", "ezEvade").Item("DodgeSkillShots").SetValue(new KeyBind(keyBind.Key, KeyBindType.Toggle, true));
        }
        #endregion


    }
}
