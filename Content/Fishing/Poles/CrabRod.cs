﻿using Aequus.Projectiles.Misc.Bobbers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Fishing.Poles
{
    public class CrabRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WoodFishingPole);
            Item.fishingPole = 45;
            Item.shootSpeed = 24f;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 80);
            Item.shoot = ModContent.ProjectileType<CrabBobber>();
        }
    }
}