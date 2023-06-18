﻿using Aequus.Items;
using Aequus.Tiles.CrabCrevice.Ambient;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.CrabCrevice {
    [LegacyName("SedimentaryRock")]
    public class SedimentaryRockItem : FancyBlockItemBase<SedimentaryRockTile> {
        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.SandBlock)
                .AddIngredient(ItemID.StoneBlock)
                .AddCondition(Condition.InBeach)
                .AddCondition(Condition.NearWater)
                .TryRegisterAfter(ItemID.ObsidianBackEcho);
            CreateRecipe()
                .AddIngredient<SedimentaryRockWallItem>(4)
                .AddTile(TileID.WorkBenches)
                .TryRegisterAfter(ItemID.ObsidianBackEcho);
        }
    }

    public class SedimentaryRockTile : ModTile {
        public static int BiomeCount;

        public override void SetStaticDefaults() {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[Type][TileID.Sand] = true;
            Main.tileMerge[Type][TileID.HardenedSand] = true;
            Main.tileMerge[TileID.Sand][Type] = true;
            Main.tileMerge[TileID.HardenedSand][Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.Conversion.Sandstone[Type] = true;
            AddMapEntry(new Color(160, 149, 97));
            DustType = DustID.Sand;
            HitSound = SoundID.Tink;
            MineResist = 1.25f;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
            //TileFramingHelper.MergeWithFrame(i, j, Type, TileID.Sand);
            return true;
        }

        public override void RandomUpdate(int i, int j) {
            if (WorldGen.genRand.NextBool(12)) {
                for (int k = -5; k <= 5; k++) {
                    for (int l = -5; l <= 5; l++) {
                        if (Main.tile[i + k, j + l].HasTile && Main.tile[i + k, j + l].TileType == ModContent.TileType<SeaPickleTile>()) {
                            return;
                        }
                    }
                }

                var p = new List<Point>();
                if (!Main.tile[i + 1, j].HasTile) {
                    p.Add(new(i + 1, j));
                }
                if (!Main.tile[i - 1, j].HasTile) {
                    p.Add(new(i - 1, j));
                }
                if (!Main.tile[i, j + 1].HasTile) {
                    p.Add(new(i, j + 1));
                }
                if (!Main.tile[i, j - 1].HasTile) {
                    p.Add(new(i, j - 1));
                }

                if (p.Count > 0) {
                    var chosen = WorldGen.genRand.Next(p);
                    if (ModContent.GetInstance<SeaPickleTile>().CanPlace(chosen.X, chosen.Y)) {
                        WorldGen.PlaceTile(chosen.X, chosen.Y, ModContent.TileType<SeaPickleTile>(), mute: true);
                        if (Main.netMode != NetmodeID.SinglePlayer) {
                            NetMessage.SendTileSquare(-1, chosen.X, chosen.Y);
                        }
                    }
                }
            }
            if (Main.tile[i, j - 1].HasTile)
                return;

            if (Main.tile[i, j - 1].LiquidAmount == 255 && Main.tile[i, j - 1].LiquidType == LiquidID.Water && WorldGen.genRand.NextBool(8)) {
                WorldGen.PlaceTile(i, j, ModContent.TileType<CrabFloorPlants>(), mute: true, style: WorldGen.genRand.Next(15));
                if (Main.netMode != NetmodeID.SinglePlayer) {
                    NetMessage.SendTileSquare(-1, i, j);
                }
            }
            else if (Main.tile[i, j - 1].LiquidAmount > 128 && Main.tile[i, j - 1].LiquidType == LiquidID.Water && WorldGen.genRand.NextBool(8)) {
                if (Main.rand.NextBool()) {
                    WorldGen.PlaceTile(i, j - 1, TileID.BeachPiles, mute: true, style: WorldGen.genRand.Next(15));
                }
                else {
                    WorldGen.PlaceTile(i, j - 1, TileID.Coral, mute: true);
                }
                if (Main.netMode != NetmodeID.SinglePlayer) {
                    NetMessage.SendTileSquare(-1, i, j - 1);
                }
            }
            else if (WorldGen.genRand.NextBool(8)) {
                if (Main.tile[i - 1, j - 1].HasTile || Main.tile[i + 1, j - 1].HasTile)
                    return;
                WorldGen.PlaceTile(i, j - 1, ModContent.TileType<CrabGrassBig>(), mute: true);
                if (Main.netMode != NetmodeID.SinglePlayer) {
                    NetMessage.SendTileSquare(-1, i, j - 1);
                }
            }

            if (WorldGen.genRand.NextBool(2) && Main.tile[i, j - 1].LiquidAmount > 0 && Main.tile[i, j - 1].LiquidType == LiquidID.Water) {
                for (int l = j - 2; l > 50; l--) {
                    if (Main.tile[i, l].LiquidAmount <= 0) {
                        if (!Main.tile[i, l].HasTile) {
                            WorldGen.PlaceTile(i, l + 1, ModContent.TileType<CrabHydrosailia>(), mute: true, style: WorldGen.genRand.Next(6));
                            if (Main.netMode != NetmodeID.SinglePlayer) {
                                NetMessage.SendTileSquare(-1, i, l + 1);
                            }
                        }
                        break;
                    }
                }
            }
        }
    }
}