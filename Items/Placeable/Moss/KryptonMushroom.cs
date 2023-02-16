﻿using Aequus.Tiles.Moss;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Moss
{
    public class KryptonMushroom : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<EnemyBuffPlants>(), EnemyBuffPlants.Krypton * 3);
            Item.value = Item.sellPrice(silver: 2, copper: 50);
            Item.maxStack = 9999;
        }
    }
}