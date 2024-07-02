﻿using Aequus.Core;

namespace Aequus.Old.Content.Items.Weapons.Melee.Valari;

public class Valari : ModItem {
    public override void SetDefaults() {
        Item.width = 40;
        Item.height = 40;
        Item.damage = 28;
        Item.useTime = 32;
        Item.useAnimation = 32;
        Item.rare = Commons.Rare.BiomeDungeon;
        Item.value = Commons.Cost.BiomeDungeon;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.UseSound = SoundID.Item1;
        Item.DamageType = DamageClass.Melee;
        Item.knockBack = 5f;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.shoot = ModContent.ProjectileType<ValariProj>();
        Item.shootSpeed = 6.5f;
        Item.autoReuse = true;
    }

    public override bool CanUseItem(Player player) {
        return player.ownedProjectileCounts[Item.shoot] < 2;
    }
}