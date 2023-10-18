﻿using Aequus.Common.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Tools.Bellows;

public class Bellows : ModItem {
    public override void SetDefaults() {
        Item.width = 20;
        Item.height = 20;
        Item.knockBack = 0.3f;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.reuseDelay = 5;
        Item.UseSound = SoundID.DoubleJump;
        Item.autoReuse = true;
        Item.rare = ItemCommons.Rarity.SkyMerchantShopItem;
        Item.value = ItemCommons.Price.SkyMerchantShopItem;
        Item.shoot = ModContent.ProjectileType<BellowsProj>();
        Item.shootSpeed = 1f;
        Item.noUseGraphic = true;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        try {
            tooltips.Insert(tooltips.GetIndex("Knockback"), new TooltipLine(Mod, "Knockback", TextHelper.GetKnockbackText(Item.knockBack)));
            tooltips.Insert(tooltips.GetIndex("Speed"), new TooltipLine(Mod, "Speed", TextHelper.GetUseAnimationText(Item.useAnimation)));
        }
        catch {

        }
    }
}