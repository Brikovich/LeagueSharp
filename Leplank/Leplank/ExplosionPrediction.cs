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

    //Health decay prediction, use Q  or AA (within Q range or AA range) to perfectly timed selected barrel explosion
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

                var qq = Environment.TickCount - targetBarrel.time + (Program.Player.Distance(targetBarrel.barrel) / 2800f + Program.Q.Delay) * 700;
                if (targetBarrel.barrel.Distance(Program.Player) <= Program.Q.Range)
                {
                
                    if (Utility.DelayAction.ActionList.Count==0)
                        Utility.DelayAction.Add(Convert.ToInt32(time - qq), () => Program.Q.CastOnUnit(targetBarrel.barrel));
                    
                }
        }
        public static void autoAttack (BarrelsManager.Barrel targetBarrel)
        {
            float time;
            if (Program.Player.Level < 7)
                time = 4f * 1000;
            else if (Program.Player.Level >= 7 && Program.Player.Level < 13)
                time = 2f * 1000;
            else
                time = 1f * 1000;

            var qq = Environment.TickCount - targetBarrel.time + Program.Player.AttackDelay;
            if (targetBarrel.barrel.Distance(Program.Player) <= Program.Player.AttackRange)
            {
                if (Utility.DelayAction.ActionList.Count == 0)
                    Utility.DelayAction.Add(Convert.ToInt32(time - qq), () => Program.Player.IssueOrder(GameObjectOrder.AttackUnit, targetBarrel.barrel));
            }
            
        }

    }
}
