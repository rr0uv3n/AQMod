﻿using Aequus.Common.Tiles.Components;
using Aequus.Content.Materials;
using Aequus.Core.ContentGeneration;
using Aequus.Core.Graphics.Tiles;
using Aequus.Core.Initialization;
using Terraria.Audio;
using Terraria.GameContent;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles.Scrap;

public class ScrapBlock : ModTile, ISpecialTileRenderer, ICustomPlaceSound, ITouchEffects {
    public static ModItem Item { get; private set; }

    public override void Load() {
        Item = new InstancedTileItem(this);
        Mod.AddContent(Item);

        LoadingSteps.EnqueueAddRecipes(() => {
            Item.CreateRecipe(10)
                .AddIngredient(ModContent.ItemType<CompressedTrash>())
                .AddTile(TileID.WorkBenches)
                .Register();
        });
    }

    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = false;
        Main.tileBlockLight[Type] = true;
        Main.tileMerge[Type][TileID.Sand] = true;
        Main.tileMerge[Type][TileID.HardenedSand] = true;
        Main.tileMerge[TileID.Sand][Type] = true;
        Main.tileMerge[TileID.HardenedSand][Type] = true;
        TileID.Sets.DoesntPlaceWithTileReplacement[Type] = true;
        TileID.Sets.CanPlaceNextToNonSolidTile[Type] = true;
        var name = CreateMapEntryName();
        AddMapEntry(new(169, 73, 43), name);
        AddMapEntry(new(141, 87, 70), name);
        AddMapEntry(new(90, 109, 71), name);
        AddMapEntry(new(77, 86, 70), name);
        AddMapEntry(new(110, 92, 87), name);
        DustType = DustID.Copper;
        HitSound = AequusSounds.ScrapBlockBreak;
        MineResist = 0.5f;
    }

    public override ushort GetMapOption(int i, int j) {
        var seed = Helper.TileSeed(i, j);
        return (ushort)Utils.RandomInt(ref seed, 0, 5);
    }

    public void Touch(int i, int j, Player player, AequusPlayer aequusPlayer) {
        aequusPlayer.touchingScrapBlock = true;
    }

    public void PlaySound(int i, int j, bool forced, int plr, int style, bool PlaceTile) {
        if (PlaceTile) {
            SoundEngine.PlaySound(AequusSounds.ScrapBlockPlaced, new Vector2(i * 16f + 8f, j * 16f + 8f));
        }
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        SpecialTileRenderer.Add(i, j, TileRenderLayerID.PostDrawLiquids);
        return false;
    }

    public void Render(int i, int j, byte layer) {
        var tile = Main.tile[i, j];
        var drawCoordinates = (new Vector2(i * 16f, j * 16f) - Main.screenPosition).Floor();
        var frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
        Main.spriteBatch.Draw(TextureAssets.Tile[Type].Value, drawCoordinates, frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
    }
}