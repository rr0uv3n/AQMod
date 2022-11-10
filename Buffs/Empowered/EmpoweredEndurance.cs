﻿using Aequus.Items.Prefixes.Potions;
using Terraria;
using Terraria.ID;

namespace Aequus.Buffs.Empowered
{
    public class EmpoweredEndurance : EmpoweredBuffBase
    {
        public override int OriginalBuffType => BuffID.Endurance;

        public override void SetStaticDefaults()
        {
            EmpoweredPrefix.ItemToEmpoweredBuff.Add(ItemID.EndurancePotion, Type);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.endurance += 0.2f;
        }
    }
}