﻿using Aequus.Core.ContentGeneration;
using Aequus.Core.Initialization;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles.Scrap;

public class ScrapBrick : ModTile {
    public override void Load() {
        ModItem item = new InstancedTileItem(this);
        Mod.AddContent(item);

        LoadingSteps.EnqueueAddRecipes(() => {
            item.CreateRecipe()
                .AddIngredient(ScrapBlock.Item)
                .AddIngredient(ItemID.StoneBlock)
                .AddTile(TileID.Furnaces)
                .Register();
        });
    }

    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        AddMapEntry(new(106, 82, 76));
        DustType = DustID.Tin;
        HitSound = SoundID.Tink;
        MineResist = 1.5f;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }
}