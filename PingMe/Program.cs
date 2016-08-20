using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace PingMe
{
    class Program
    {

        private static Obj_AI_Hero Me;
        private static Menu MainMenu;
        private static List<Obj_AI_Hero> Allies;
        private static List<Obj_AI_Hero> Ennemies;
        private static List<string> R2winChamps = new List<string>(new string[] { "shen", "soraka", "ashe", "ezreal", "draven", "gangplank", "karthus", "pantheon", "ziggs", "nocturne", "lux", "ryze", "xerath", "twistedfate" });
        private static bool hasTP;
        private static bool hasLongRangeUlt;
        private static double allylastPing = 0;
        private static double enemylastPing = 0;
        private static Obj_AI_Hero lastpingedally = null;
        private static Obj_AI_Hero lastpingedenemy = null;
        private static Spell TPSpell;
        private static Spell RSpell;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += _OnGameLoad;
        }

        private static void _OnGameLoad (EventArgs args)
        {
            
            Me = ObjectManager.Player;
            Allies = new List<Obj_AI_Hero>();
            Ennemies = new List<Obj_AI_Hero>();

            if (Me.Spellbook.Spells.Any(x => x.Name.ToLower() == "summonerteleport")) { hasTP = true; TPSpell = new Spell(Me.Spellbook.Spells.Where(x => x.Name.ToLower() == "summonerteleport").FirstOrDefault().Slot); }
            else hasTP = false;

            if (R2winChamps.Contains(Me.ChampionName.ToLower())) { hasLongRangeUlt = true; RSpell = new Spell(SpellSlot.R); }
            else hasLongRangeUlt = false;

            foreach (var ally in HeroManager.Allies)
            {
                if(!ally.IsMe) Allies.Add(ally);
            }
            foreach (var enemy in HeroManager.Enemies)
            {
                Ennemies.Add(enemy);
            }
            _BuildMenu();
            Game.OnUpdate += _OnUpdate;
        }

        private static void _OnUpdate(EventArgs args)
        {

            bool ping = false;
            if (MainMenu.Item("PingEnemy", true).GetValue<Boolean>() || MainMenu.Item("PingAlly", true).GetValue<Boolean>())
            {
                if (hasTP && !hasLongRangeUlt)
                {
                    if (MainMenu.Item("TpPing", true).GetValue<Boolean>())
                    {
                        if (TPSpell.IsReady())
                        {
                            ping = true;
                        }
                    }
                    else ping = true;
                }
                else if (!hasLongRangeUlt) ping = true;

                if (hasLongRangeUlt && !hasTP)
                {
                    if (MainMenu.Item("UltPing", true).GetValue<Boolean>())
                    {
                        if (RSpell.IsReady())
                        {
                            ping = true;
                        }
                    }
                    else ping = true;
                }
                else if (!hasTP) ping = true;

                if (hasLongRangeUlt && hasTP)
                {
                    if (MainMenu.Item("UltPing", true).GetValue<Boolean>() && MainMenu.Item("TpPing", true).GetValue<Boolean>())
                    {
                        if (RSpell.IsReady() || TPSpell.IsReady()) ping = true;
                    }
                    else if (MainMenu.Item("UltPing", true).GetValue<Boolean>() && !MainMenu.Item("TpPing", true).GetValue<Boolean>())
                    {
                        if (RSpell.IsReady()) ping = true;
                    }
                    else if (!MainMenu.Item("UltPing", true).GetValue<Boolean>() && MainMenu.Item("TpPing", true).GetValue<Boolean>())
                    {
                        if (TPSpell.IsReady()) ping = true;
                    }
                    else ping = true;
                }
                else if (!hasTP && !hasLongRangeUlt) ping = true;

            }


            if (MainMenu.Item("PingAlly", true).GetValue<Boolean>() && ping)
            {
                foreach (var ally in HeroManager.Allies)
                {
                    if (!ally.IsMe && !ally.InFountain() && !ally.IsDead && !ally.IsRecalling() && ally.Distance(Me) >= 2000 && ally.HealthPercent < MainMenu.SubMenu("allyMenu").Item("ally" + ally.ChampionName + "HPPERCENT").GetValue<Slider>().Value && HeroManager.Enemies.Any(x=>x.Distance(ally) <= 1200) && (allylastPing == 0 || Environment.TickCount - allylastPing > 2000) && ((lastpingedally == null || lastpingedally != ally) || Environment.TickCount - allylastPing > 30000))
                    {
                        Game.ShowPing(PingCategory.Danger, ally.Position, true);
                        Utility.DelayAction.Add(200, () => Game.ShowPing(PingCategory.Danger, ally.Position, true));
                        Utility.DelayAction.Add(400, () => Game.ShowPing(PingCategory.Danger, ally.Position, true));
                        lastpingedally = ally;
                        allylastPing = Environment.TickCount;
                    }
                }
            }

            if (MainMenu.Item("PingEnemy", true).GetValue<Boolean>() && ping)
            {
                foreach (var enemy in HeroManager.Enemies)
                {
                    if (!enemy.IsDead && enemy.IsVisible && enemy.Distance(Me) >= 3000 && enemy.HealthPercent < MainMenu.SubMenu("enemyMenu").Item("enemy" + enemy.ChampionName + "HPPERCENT").GetValue<Slider>().Value && (enemylastPing == 0 || Environment.TickCount - enemylastPing > 3000) && ( (lastpingedenemy == null || lastpingedenemy != enemy) || Environment.TickCount - enemylastPing > 30000) )
                    {
                        Game.ShowPing(PingCategory.Fallback, enemy.Position, true);
                        Utility.DelayAction.Add(200, () => Game.ShowPing(PingCategory.Fallback, enemy.Position, true));
                        Utility.DelayAction.Add(400, () => Game.ShowPing(PingCategory.Fallback, enemy.Position, true));
                        lastpingedenemy = enemy;
                        enemylastPing = Environment.TickCount;
                    }
                }
            }


        }

        #region Menu
        private static void _BuildMenu()
        {
            MainMenu = new Menu("PingMe", "MainMenu", true);

            var allyMenu = new Menu("Allies", "allyMenu");
            {
                allyMenu.AddItem(new MenuItem("sep0", "-- Note : 0% = disable"));
                foreach(var ally in Allies)
                {
                    allyMenu.AddItem(new MenuItem("ally" + ally.ChampionName + "HPPERCENT", ally.ChampionName + " HP%")).SetValue<Slider>(new Slider(20, 0, 100));
                }
                MainMenu.AddSubMenu(allyMenu);
            }

            var enemyMenu = new Menu("Ennemies", "enemyMenu");
            {
                enemyMenu.AddItem(new MenuItem("sep1", "-- Note : 0% = disable"));
                foreach (var enemy in Ennemies)
                {
                    enemyMenu.AddItem(new MenuItem("enemy" + enemy.ChampionName + "HPPERCENT", enemy.ChampionName + " HP%")).SetValue<Slider>(new Slider(20, 0, 100));
                }
                MainMenu.AddSubMenu(enemyMenu);
            }

            MainMenu.AddItem(new MenuItem("PingAlly", "Ping locally when an ally is low", true).SetValue<Boolean>(false));
            MainMenu.AddItem(new MenuItem("PingEnemy", "Ping locally when an enemy is low", true).SetValue<Boolean>(true));
            if (hasTP) MainMenu.AddItem(new MenuItem("TpPing", "Ping if TP is up", true).SetValue<Boolean>(true));
            if (hasLongRangeUlt) MainMenu.AddItem(new MenuItem("UltPing", "Ping if R is up", true).SetValue<Boolean>(true));
            MainMenu.AddToMainMenu();
        }
        #endregion


    }
}
