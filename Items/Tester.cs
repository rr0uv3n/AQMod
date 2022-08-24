﻿using Aequus.Common;
using Aequus.Content.CarpenterBounties;
using Aequus.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items
{
    internal class Tester : ModItem
    {
        public override string Texture => AequusHelpers.GetPath<Gamestar>();

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.width = 20;
            Item.height = 20;
        }

        public override bool? UseItem(Player player)
        {
            player.GetModPlayer<CarpenterBountyPlayer>().CompletedBounties.Clear();
            var rect = Utils.CenteredRectangle(player.Center.ToTileCoordinates().ToVector2(), new Vector2(40f, 40f)).Fluffize(10);
            AequusHelpers.dustDebug(rect.WorldRectangle());
            Main.NewText(CarpenterSystem.BountiesByID[0].CheckConditions(new CarpenterBounty.ConditionInfo(new TileMapCache(rect)), out string reply));
            Main.NewText(reply);
            return true;
        }
    }
}