﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Unused.DebugItems
{
    internal class FlashEffect : ModItem
    {
        public override string Texture => AequusTextures.Fluorescence.Path;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(4, 6));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            SacrificeTotal = 0;
            Tooltip.SetDefault(
                """
                Activates a flash effect on your cursor
                Testing Item
                """);
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.rare = ItemRarityID.Red;
            Item.width = 20;
            Item.height = 20;
            Item.color = Main.OurFavoriteColor;
        }

        public override bool? UseItem(Player player)
        {
            int x = Helper.MouseTileX;
            int y = Helper.MouseTileY;

            ScreenFlash.Flash.Set(Main.MouseWorld, 0.8f, 0.9f);
            return true;
        }
    }
}