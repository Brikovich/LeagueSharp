using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Leplank
{
    class ExplosionPrediction
    {
        public static void castQ (BarrelsManager.Barrel targetBarrel)
        {
            
                float time;

                if(Program.Player.Level<7)
                    time = 4f * 1000;
                else if(Program.Player.Level >=7 && Program.Player.Level < 13)
                    time = 2f * 1000;
                else
                    time = 1f * 1000;

                var qq = Environment.TickCount - targetBarrel.time + (Program.Player.Distance(targetBarrel.barrel) / 2800f + Program.Q.Delay) * 1000;
                Utility.DelayAction.Add(Convert.ToInt32(time-qq), () => Program.Q.CastOnUnit(targetBarrel.barrel)); 
        }


    }
}
