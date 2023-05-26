﻿using Aequus.Items.Materials.Gems;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Combat {
    [LegacyName("SoulCrystal")]
    public class SoulNeglectite : ModItem {
        /// <summary>
        /// Default Value: 10
        /// </summary>
        public static int DebuffDamage = 10;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DebuffDamage);

        public override void SetDefaults() {
            Item.DefaultToAccessory(14, 20);
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().soulCrystalDamage += DebuffDamage;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<SoulGemFilled>(5)
                .AddIngredient(ItemID.HellstoneBar, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}