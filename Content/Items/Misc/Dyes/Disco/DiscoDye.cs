﻿using Aequus.Content.Items.Material.OmniGem;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Aequus.Content.Items.Misc.Dyes.Disco;

public class DiscoDye : DyeItemBase {
    public override int Rarity => ItemRarityID.Green;

    public override string Pass => "DiscoPass";

    public override ArmorShaderData CreateShaderData() {
        return new ArmorShaderData(Effect, Pass).UseOpacity(1f);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.BottledWater)
            .AddIngredient<OmniGem>()
            .AddTile(TileID.DyeVat)
            .Register();
    }
}