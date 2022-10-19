﻿using Aequus.Items.Prefixes.Potions;
using Terraria;
using Terraria.ID;

namespace Aequus.Buffs.Empowered
{
    public class EmpoweredFishing : EmpoweredBuffBase
    {
        public override int OriginalBuffType => BuffID.Fishing;

        public override void SetStaticDefaults()
        {
            EmpoweredPrefix.ItemToEmpoweredBuff.Add(ItemID.FishingPotion, Type);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.fishingSkill += 30;
        }
    }
}