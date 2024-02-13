﻿using Aequus.Core;

namespace Aequus.Content.Equipment.Accessories.Informational.Monocle;

[WorkInProgress]
public class ShimmerMonocle : ModItem {
    public override void SetStaticDefaults() {
        ItemSets.WorksInVoidBag[Type] = true;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.LifeformAnalyzer);
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<AequusPlayer>().accShimmerMonocle = true;
    }
}