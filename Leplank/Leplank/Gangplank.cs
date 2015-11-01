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

            if (Menus._menu.Item("Leplank.misc.events.qlhtoggle").GetValue<KeyBind>().Active)
            {
                LastHit();
            }
            WManager();
        }

        private static void Combo()
        {
            
        }

        private static void WaveClear()
        {
            var minionswc =
                MinionManager.GetMinions(Program.Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                    .Where(mwc => mwc.SkinName != "GangplankBarrel")
                    .OrderByDescending(mlh => mlh.Distance(Program.Player)).ToList();
            // Items
            if (Menus.GetBool("Leplank.item.hydra") &&
                (MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 390).Count > 2 ||
                 MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 390, MinionTypes.All, MinionTeam.Neutral)
                     .Count >= 1) &&
                Items.HasItem(3074) &&
                Items.CanUseItem(3074))
            {
                Items.UseItem(3074); //hydra, range of active = 400
            }
            if (Menus.GetBool("Leplank.item.tiamat") &&
                (MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 390).Count > 2 ||
                 MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 390, MinionTypes.All, MinionTeam.Neutral)
                     .Count >= 1) &&
                Items.HasItem(3077) &&
                Items.CanUseItem(3077))
            {
                Items.UseItem(3077); //tiamat, range of active = 400
            }

            if (Menus.GetBool("Leplank.misc.barrelmanager.edisabled") == false &&
                Menus.GetBool("Leplank.lc.e") && Program.E.IsReady())
            {
                var posE = Program.E.GetCircularFarmLocation(minionswc, Program.EexplosionRange);
                if (posE.MinionsHit >= Menus.GetSlider("Leplank.lc.emin") &&
                    (!BarrelsManager.savedBarrels.Any() ||
                     BarrelsManager.closestToPosition(Program.Player.ServerPosition).barrel.Distance(Program.Player) > Program.Q.Range) &&
                    Program.E.Instance.Ammo > Menus.GetSlider("Leplank.misc.barrelmanager.estacks"))
                {
                    Program.E.Cast(posE.Position);
                }
                
             
            }

            if (BarrelsManager.savedBarrels.Any() ||
                BarrelsManager.closestToPosition(Program.Player.ServerPosition).barrel.Distance(Program.Player) <
                Program.Q.Range + 100) // Extra range
            {
                var minionsInERange =
                    MinionManager.GetMinions(
                        BarrelsManager.closestToPosition(Program.Player.ServerPosition).barrel.Position,
                        Program.EexplosionRange, MinionTypes.All, MinionTeam.NotAlly);

                if (Menus.GetBool("Leplank.lc.qone") &&
                    Program.Q.IsInRange(BarrelsManager.closestToPosition(Program.Player.ServerPosition).barrel) &&
                    Program.Q.IsReady() && Program.Player.ManaPercent > Menus.GetSlider("Leplank.lc.qonemana"))               
                {
                    if ((Program.Q.Level >= 3 &&
                         minionsInERange.Where(m => m.Health < DamageLib.GetEDamages(m, true)).ToList().Count >= 3) ||
                        (Program.Q.Level == 2 &&
                         minionsInERange.Where(m => m.Health < DamageLib.GetEDamages(m, true)).ToList().Count >= 2) ||
                        (Program.Q.Level == 1 &&
                         minionsInERange.Where(m => m.Health < DamageLib.GetEDamages(m, true)).ToList().Any()) ||
                        (Program.Q.Level == 1 && minionsInERange.Count < 2))
                    {
                        ExplosionPrediction.castQ(BarrelsManager.closestToPosition(Program.Player.ServerPosition));
                    }
                }
                if (!Program.Q.IsReady() &&
                    Program.Player.Distance(BarrelsManager.closestToPosition(Program.Player.ServerPosition).barrel) <
                    Program.Player.AttackRange)
                {
                    ExplosionPrediction.autoAttack(BarrelsManager.closestToPosition(Program.Player.ServerPosition));
                }
            }
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
                            mlh.Health < DamageLib.GetQDamages(mlh)) // are considered as Obj ai minions so it may cause some bugs if not checked
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
            if (Menus.GetBool("Leplank.misc.events.wheal") && Program.Player.HealthPercent <= Menus.GetSlider("Leplank.misc.events.healmin") &&
                Program.Player.ManaPercent >= Menus.GetSlider("Leplank.misc.events.healminmana"))
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
