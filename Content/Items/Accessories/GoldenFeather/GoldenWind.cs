﻿using Terraria.Localization;

namespace AequusRemake.Content.Items.Accessories.GoldenFeather;

[LegacyName("AloeVera")]
public class GoldenWind : GoldenFeather {
    public static new int LifeRegenerationAmount { get; set; } = 3;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ALanguage.Seconds(-RespawnTimeAmount), ALanguage.Decimals(LifeRegenerationAmount / 2f));

    public override int BuffType => ModContent.BuffType<GoldenWindBuff>();

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(gold: 2);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.BandofRegeneration)
            .AddIngredient<GoldenFeather>()
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}