﻿using Aequus.Tiles.Furniture.HardmodeChests;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture.Misc
{
    public class HardJungleChest : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<HardJungleChestTile>());
            Item.value = Item.sellPrice(silver: 10);
        }
    }
}