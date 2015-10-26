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


    }
}
