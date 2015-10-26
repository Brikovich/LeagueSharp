using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
namespace Leplank
{
    class Gangplank
    {

        public static void _Orbwalking(EventArgs args)
        {
            if (Program.Player.IsDead)
            {
                return;
            }
            #region Orbwalker modes
            var activeOrbwalker = Menus._orbwalker.ActiveMode;
            switch (activeOrbwalker)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    WaveClear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Mixed();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    break;
            }
          #endregion Orbwalker modes

            WManager();
        }

        private static void Combo()
        {
            
        }

        private static void WaveClear()
        {
            
        }

        private static void Mixed()
        {
            
        }

        private static void LastHit()
        {
            var minionlhtarget =
                MinionManager.GetMinions(Program.Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                    .Where(
                        mlh =>
                            mlh.SkinName != "GangplankBarrel" && // It makes the program check if it's not a barrel because Powder Kegs 
                            mlh.Health < Program.Player.GetSpellDamage(mlh, SpellSlot.Q)) // are considered as Obj ai minions so it may cause some bugs if not checked
                    .OrderByDescending(mlh => mlh.Distance(Program.Player)) // Prioritize minions that's are far from the player
                    .FirstOrDefault();
            if (Menus.GetBool("Leplank.lh.q") && Program.Player.ManaPercent >= Menus.GetSlider("Leplank.lh.qmana") &&
                Program.Q.IsReady() && minionlhtarget != null) // Check config
            {
                Program.Q.CastOnUnit(minionlhtarget);
            }
        }

        private static void WManager()
        {
            if (!Program.W.IsReady() || Program.Player.InFountain() || Program.Player.IsRecalling())
            {
                return;
            }
            #region Cleanser
            if (Menus.GetBool("Leplank.cleansermanager.enabled"))
            {
               if ((
                    (Program.Player.HasBuffOfType(BuffType.Charm) && Menus.GetBool("Leplank.cleansermanager.charm"))
                    || (Program.Player.HasBuffOfType(BuffType.Flee) && Menus.GetBool("Leplank.cleansermanager.flee"))
                    || (Program.Player.HasBuffOfType(BuffType.Polymorph) && Menus.GetBool("Leplank.cleansermanager.polymorph"))
                    || (Program.Player.HasBuffOfType(BuffType.Snare) && Menus.GetBool("Leplank.cleansermanager.snare"))
                    || (Program.Player.HasBuffOfType(BuffType.Stun) && Menus.GetBool("Leplank.cleansermanager.stun"))
                    || (Program.Player.HasBuffOfType(BuffType.Taunt) && Menus.GetBool("Leplank.cleansermanager.taunt"))
                    || (Program.Player.HasBuff("summonerexhaust") && Menus.GetBool("Leplank.cleansermanager.exhaust"))
                    || (Program.Player.HasBuffOfType(BuffType.Suppression) && Menus.GetBool("Leplank.cleansermanager.suppression"))
                   ) && Program.Player.ManaPercent >= Menus.GetSlider("Leplank.cleansermanager.mana") && Program.Player.HealthPercent < Menus.GetSlider("Leplank.cleansermanager.health"))
               {
                   Utility.DelayAction.Add(Menus.GetSlider("Leplank.cleansermanager.delay") + Game.Ping, () =>
                   {
                       Program.W.Cast();
                   });
               }
            }
            #endregion Cleanser

            #region Healer
            if (Program.Player.HealthPercent <= Menus.GetSlider("Leplank.misc.healmin") &&
                Program.Player.ManaPercent >= Menus.GetSlider("Leplank.misc.healminmana"))
            {
                Utility.DelayAction.Add(100 + Game.Ping, () =>
                {
                    Program.W.Cast();
                }
                );
            }
            #endregion Healer
        }
        
    

    }
}
