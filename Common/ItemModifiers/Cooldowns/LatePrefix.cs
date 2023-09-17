﻿namespace Aequus.Common.ItemModifiers.Cooldowns;

public class LatePrefix : CooldownPrefixBase {
    public override float CooldownMultiplier => 1.1f;

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) {
        damageMult = 0.95f;
        useTimeMult = 1.2f;
    }

    public override void ModifyValue(ref float valueMult) {
        valueMult = 0.7f;
    }
}