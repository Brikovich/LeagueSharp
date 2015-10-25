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
            
        }


    }
}
