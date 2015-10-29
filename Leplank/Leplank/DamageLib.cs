using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using LeagueSharp;


namespace Leplank
{
    class DamageLib
    {
        public static float GetQDamages(Obj_AI_Base qTarget)
        {
            float qdamages;
            if (Items.HasItem(3087) && Program.Player.HasBuff("itemstatikshankcharge") &&
                Program.Player.GetBuffCount("itemstatikshankcharge") == 100)
            {
                qdamages =
                    (float)
                        Program.Player.CalcDamage(qTarget, Damage.DamageType.Physical,
                            20 + ((Program.Q.Level - 1)*25) + Program.Player.FlatPhysicalDamageMod + 100 - 30);
            }
            else
            {
                qdamages =
                    (float)
                        Program.Player.CalcDamage(qTarget, Damage.DamageType.Physical,
                            20 + ((Program.Q.Level - 1)*25) + Program.Player.FlatPhysicalDamageMod);
            }

            if (Program.Player.FlatCritChanceMod > 70)
            {
                qdamages =
                    (float)
                        Program.Player.CalcDamage(qTarget, Damage.DamageType.Physical,
                            qdamages*Program.Player.FlatCritDamageMod);
            }
            // WIP

            return qdamages;
        }

        public static float GetRDamages(Obj_AI_Base rTarget)
        {
            float rdamages;
            if (Program.Player.HasBuff("gangplankrupgrade1"))
            {
                 rdamages = (float) Program.Player.CalcDamage(rTarget, Damage.DamageType.Magical,
                   (30 + (20 * Program.R.Level) + (Program.Player.FlatMagicDamageMod * 0.1)) * 18);
            }
            else 
            {
                rdamages = (float) Program.Player.CalcDamage(rTarget, Damage.DamageType.Magical,
                    (30 + (20 * Program.R.Level) + (Program.Player.FlatMagicDamageMod * 0.1))* 12);
            }
            return rdamages;
        }

        public static float GetEDamages(Obj_AI_Base eTarget, bool usingQ)
        {
            

            return Edamages;
        }
    }
}

