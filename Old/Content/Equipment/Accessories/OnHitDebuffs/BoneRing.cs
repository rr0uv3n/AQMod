﻿using Aequus.Common.Items;
using Terraria.Localization;

namespace Aequus.Old.Content.Equipment.Accessories.OnHitDebuffs;

[AutoloadEquip(EquipType.HandsOn)]
[LegacyName("BoneHawkRing")]
public class BoneRing : ModItem {
    public static int DebuffDuration { get; set; } = 30;
    public static float MovementSpeedMultiplier { get; set; } = 0.4f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ExtendLanguage.Percent(MovementSpeedMultiplier), ExtendLanguage.Seconds(DebuffDuration));

    public override void SetDefaults() {
        Item.DefaultToAccessory(20, 14);
        Item.rare = ItemCommons.Rarity.DungeonLoot;
        Item.value = ItemCommons.Price.DungeonLoot;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().accBoneRing++;
    }
}