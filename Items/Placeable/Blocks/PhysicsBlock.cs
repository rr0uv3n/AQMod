﻿using Aequus.Content.Town.PhysicistNPC.Analysis;
using Aequus.Tiles.Blocks;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Blocks
{
    public class PhysicsBlock : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<PhysicsBlockTile>());
            Item.value = Item.buyPrice(silver: 2, copper: 50);
            Item.maxStack = 9999;
        }
    }
}