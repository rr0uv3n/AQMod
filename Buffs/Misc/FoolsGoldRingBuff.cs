﻿using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Misc
{
    public class FoolsGoldRingBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.05f;
            player.GetCritChance(DamageClass.Generic) += 2;
            player.GetAttackSpeed(DamageClass.Melee) += 0.05f;
            player.GetKnockback(DamageClass.Summon) += 0.1f;
            player.statDefense += 2;
        }
    }
}