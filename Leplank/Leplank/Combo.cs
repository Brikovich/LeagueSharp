﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Leplank
{
    class Combo
    {
        public static void Classic()
        {
            
        }

       // GOD DAMN FUCKING ADVANCED COMBO SWAGG GOD LORD L O G I C, bitch please #kappa
        public static void BarrelLord()
        {
            #region BarrelLord™           
            
            var enemies = Program.Player.GetEnemiesInRange(Program.E.Range);
            var target = TargetSelector.GetTarget(Program.E.Range, TargetSelector.DamageType.Physical);
            #region R
            if (Menus.GetBool("Leplank.combo.r") && Program.R.IsReady())
            {
                var rfocus =
                    HeroManager.Enemies.FirstOrDefault(
                        e =>
                            e.Health < DamageLib.GetRDamages(e) &&
                            e.GetEnemiesInRange(300).Count > (Menus.GetSlider("Leplank.combo.rmin") - 1) &&
                            e.Distance(Program.Player) < 1200);
                if (rfocus != null)
                {
                    Program.R.Cast(Prediction.GetPrediction(rfocus, Program.R.Delay, 300, float.MaxValue).CastPosition);
                }
            }
            #endregion R

            switch (enemies.Count)
            {
                case 0:
                    return;

                case 1: // 1v1                   
                    if (Program.Estacks == 0 && Program.Q.IsReady() && Program.Q.IsInRange(target) && Program.E.Instance.CooldownExpires > Program.Q.Instance.Cooldown)
                    {
                        Program.Q.CastOnUnit(target);
                    }


                    break;
                case 2:

                    break;
                case 3:

                    break;
                case 4:

                    break;
                case 5:

                    break;
            }


            #endregion BarrelLord™
        }


    }
}
