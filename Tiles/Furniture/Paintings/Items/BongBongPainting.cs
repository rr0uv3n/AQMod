﻿using Terraria;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.Paintings.Items {
    public class BongBongPainting : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings3x2>(), WallPaintings3x2.BongBongPainting);
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}