﻿using Aequus.Buffs;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Potions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Unused {
    public class TubOfCookieDough : ModItem {
        public override void SetStaticDefaults() {
            SacrificeTotal = 5;
            this.StaticDefaultsToDrink(Color.White, Color.Yellow, Color.HotPink * 1.25f);
        }

        public override void SetDefaults() {
            Item.width = 20;
            Item.height = 20;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item3;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.rare = ItemRarityID.Gray;
            Item.value = Item.sellPrice(silver: 70);
            Item.buffType = ModContent.BuffType<AstralCookieBuff>();
            Item.buffTime = 72000;
        }

        public override void AddRecipes() {
        }
    }
}