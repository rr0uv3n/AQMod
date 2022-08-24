﻿using Aequus.Items.Misc;
using Aequus.Items.Misc.Energies;
using Aequus.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic
{
    [GlowMask]
    public class PentalScythe : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.width = 32;
            Item.height = 32;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<PentalScytheProj>();
            Item.shootSpeed = 25f;
            Item.mana = 7;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item109;
            Item.value = ItemDefaults.DemonSiegeValue;
            Item.knockBack = 2f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DemonScythe)
                .AddIngredient<Hexoplasm>(6)
                .AddIngredient<DemonicEnergy>(3)
                .AddTile(TileID.Bookcases)
                .RegisterAfter(ItemID.GoldenShower);
        }
    }
}