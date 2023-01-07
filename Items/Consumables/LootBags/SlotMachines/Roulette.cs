﻿using Aequus.Content.CrossMod;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.LootBags.SlotMachines
{
    public class Roulette : SlotMachineItemBase
    {
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            var builder = this.CreateLoot(itemLoot);
            builder.AddRouletteItem(ItemID.Spear)
                .AddRouletteItem(ItemID.Blowpipe)
                .AddRouletteItem(ItemID.WoodenBoomerang)
                .AddRouletteItem(ItemID.Aglet)
                .AddRouletteItem(ItemID.ClimbingClaws)
                .AddRouletteItem(ItemID.Umbrella)
                .AddRouletteItem(ItemID.WandofSparking)
                .AddRouletteItem(ItemID.Radar)
                .AddOptions(chance: 3, ItemID.HerbBag, ItemID.CanOfWorms)
                .Add(ItemID.SilverCoin, chance: 1, stack: (10, 40));
            if (ThoriumMod.Instance != null)
            {
                if (ThoriumMod.Instance.TryFind("RecoveryWand", out ModItem modItem))
                    builder.AddRouletteItem(modItem.Type);
                if (ThoriumMod.Instance.TryFind("Flute", out modItem))
                    builder.AddRouletteItem(modItem.Type);
            }
        }
    }
}