﻿using AequusRemake.Core;
using Terraria.Localization;

namespace AequusRemake.Content.Items.Accessories.GoldenFeather;

[LegacyName("BloodCrystal", "BloodCurcleav")]
public class GoldenFeather : ModItem {
    public static int RespawnTimeAmount { get; set; } = -300;
    public static int LifeRegenerationAmount { get; set; } = 1;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ALanguage.Seconds(-RespawnTimeAmount), ALanguage.Decimals(LifeRegenerationAmount / 2f));

    public virtual int BuffType => ModContent.BuffType<GoldenFeatherBuff>();

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = Commons.Rare.NPCSkyMerchant;
        Item.value = Commons.Cost.NPCSkyMerchant;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().accGoldenFeather = Item;
    }
}