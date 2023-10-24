﻿using Aequus.Common.Items.EquipmentBooster;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Equipment.Accessories.Informational.Monocle; 

public class RichMansMonocle : ModItem {
    public override void SetStaticDefaults() {
        EquipBoostDatabase.Instance.SetNoEffect(Type);
        ItemID.Sets.WorksInVoidBag[Type] = true;
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<ShimmerMonocle>();
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.LifeformAnalyzer);
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<AequusPlayer>().accMonocle = true;
    }
}