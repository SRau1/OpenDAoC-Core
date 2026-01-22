using System;

namespace DOL.GS.PropertyCalc
{
    /// <summary>
    /// Flat (additive) WeaponSkill bonus.
    /// This is intentionally separate from eProperty.WeaponSkill which is a percent multiplier.
    /// </summary>
    [PropertyCalculator(eProperty.WeaponSkillBonus)]
    public class WeaponSkillBonusCalculator : PropertyCalculator
    {
        public override int CalcValue(GameLiving living, eProperty property)
        {
            int value =
                living.BaseBuffBonusCategory[property]
                + living.SpecBuffBonusCategory[property]
                + living.OtherBonus[property]
                + living.ItemBonus[property]
                - Math.Abs(living.DebuffCategory[property]);

            return Math.Max(0, value);
        }
    }
}

