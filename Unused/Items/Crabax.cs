﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items {
    public class Crabax : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 0;
        }

        public override void SetDefaults() {
            Item.width = 32;
            Item.height = 32;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 20;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 10f;
            Item.axe = 25; // has the highest axe power
            Item.tileBoost = 5;
            Item.value = Item.sellPrice(gold: 2);
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Gray;
            Item.autoReuse = true;
        }
    }
}