﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Summon.Necro
{
    public class ShadowVeer : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().ghostShadowDash++;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Cobweb, 100)
                .AddIngredient(ItemID.DemoniteBar, 10)
                .AddTile(TileID.DemonAltar)
                .TryRegisterBefore(ItemID.MasterNinjaGear);
            CreateRecipe()
                .AddIngredient(ItemID.Cobweb, 100)
                .AddIngredient(ItemID.CrimtaneBar, 10)
                .AddTile(TileID.DemonAltar)
                .TryRegisterBefore(ItemID.MasterNinjaGear);
        }
    }
}