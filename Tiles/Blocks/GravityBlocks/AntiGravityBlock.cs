﻿using Aequus.NPCs.Town.PhysicistNPC.Analysis;
using Aequus.Tiles.Base;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Blocks.GravityBlocks {
    [LegacyName("ForceAntiGravityBlock")]
    public class AntiGravityBlock : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 100;
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<AntiGravityBlockTile>());
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 1);
        }
    }

    [LegacyName("ForceAntiGravityBlockTile")]
    public class AntiGravityBlockTile : GravityBlockBase {
        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            Main.tileLighted[Type] = true;
            DustType = DustID.OrangeStainedGlass;
            GravityType = -1;
            Auras = new[] { AequusTextures.AntiGravityAura_0, AequusTextures.AntiGravityAura_1 };
            DustTexture = AequusTextures.AntiGravityDust;
            AddMapEntry(Color.Orange, TextHelper.GetItemName<AntiGravityBlock>());
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
            r = 1f;
            g = 0.5f;
            b = 0.1f;
        }
    }
}