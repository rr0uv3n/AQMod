﻿using Aequus.Items.Prefixes.Potions;
using Terraria;
using Terraria.ID;

namespace Aequus.Buffs.Empowered
{
    public class EmpoweredSwiftness : EmpoweredBuffBase
    {
        public override int OriginalBuffType => BuffID.Swiftness;

        public override void SetStaticDefaults()
        {
            EmpoweredPrefix.ItemToEmpoweredBuff.Add(ItemID.SwiftnessPotion, Type);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.moveSpeed += 0.5f;
        }
    }
}