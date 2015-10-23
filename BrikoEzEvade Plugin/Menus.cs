using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace BrikoEzEvade_Plugin
{
    class Menus
    {
        public static Menu _Menu;
        public static int hp;
        public static int chance;
        public static int ally;
        public static int ennemy;
        public static int time;
        public static int minrt;
        public static int maxrt;
        public static int mode;
        public static bool underTower;
        public static bool circular;
        public static bool FOW;
        public static bool collision;

        public static void iniMenu()
        {
            _Menu = new Menu("Legit EzEvade Plugin", "legitEzEvade");
            _Menu.AddItem(new MenuItem("enable", "Enable").SetValue(new KeyBind('T', KeyBindType.Toggle, true)));
            _Menu.AddItem(new MenuItem("reset", "Reset to default").SetValue(false));
            _Menu.AddItem(new MenuItem("range", "Scan Range").SetValue(new Slider(1000, 0, 3000)));
            _Menu.AddItem(new MenuItem("interval", "Recalculate random interval").SetValue(new Slider(20, 0, 120)));
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

        public static void createLevelMenu(Menu gibeMenu, int gibeLevel)
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
        public static void gibeDefault(int gibeLevel)
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

    }
}
