﻿using Aequus.Common.Utilities;
using System;
using Terraria.Localization;

namespace Aequus.Content.Items.Accessories.GoldenFeather;

public class GoldenWind : GoldenFeather {
    public static int LifeRegenerationAmount { get; set; } = 2;

    public override LocalizedText Tooltip => BaseTooltip.WithFormatArgs(ALanguage.Decimals(LifeRegenerationAmount / 2f), ALanguage.Seconds(-RespawnTimeAmount));

    public override int BuffType => ModContent.BuffType<GoldenWindBuff>();

    public override void Load() {
        // Prevent hook from applying twice.
    }

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(gold: 2);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.BandofRegeneration)
            .AddIngredient<GoldenFeather>()
            .AddTile(TileID.TinkerersWorkbench)
            .Register()
            .SortBeforeFirstRecipesOf(ItemID.CharmofMyths);
    }
}

public class GoldenWindBuff : GoldenFeatherBuff {
    public override LocalizedText DisplayName => ModContent.GetInstance<GoldenWind>().DisplayName;
    public override LocalizedText Description => ModContent.GetInstance<GoldenWind>().GetLocalization("BuffDescription")
        .WithFormatArgs(ALanguage.Seconds(-GoldenFeather.RespawnTimeAmount), ALanguage.Decimals(GoldenWind.LifeRegenerationAmount / 2f));

    public override void Update(Player player, ref int buffIndex) {
        if (!player.TryGetModPlayer(out AequusPlayer aequus)) {
            return;
        }

        player.lifeRegen += GoldenWind.LifeRegenerationAmount;
        aequus.accGoldenFeatherRespawnTimeModifier = Math.Min(aequus.accGoldenFeatherRespawnTimeModifier, GoldenFeather.RespawnTimeAmount);
    }
}