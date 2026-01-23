namespace DOL.GS
{
    public class AblativeArmorECSGameEffect : ECSGameSpellEffect
    {
        public int RemainingValue { get; set; }

        public AblativeArmorECSGameEffect(in ECSGameEffectInitParams initParams) : base(initParams)
        {
            RemainingValue = (int) SpellHandler.Spell.Value;
        }

        public override void OnStartEffect()
        {
            // "A crystal shield covers you."
            // "A crystal shield covers {0}'s skin."
            OnEffectStartsMsg(true, false, true);
        }

        public override void OnStopEffect()
        {
            // "Your crystal shield fades."
            // "{0}'s crystal shield fades."
            OnEffectExpiresMsg(true, false, true);
        }

        public override void OnEffectPulse()
        {
            // Restore the ablative value to full on each pulse
            // This allows the spell to "refresh" the ablative armor every few seconds
            RemainingValue = (int) SpellHandler.Spell.Value;
        }
    }
}
