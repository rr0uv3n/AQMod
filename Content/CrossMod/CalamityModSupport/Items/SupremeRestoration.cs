﻿using AequusRemake.Content.Items.Potions.Healing.Restoration;
using AequusRemake.Core.CrossMod;
using AequusRemake.Systems.Items;

namespace AequusRemake.Content.CrossMod.CalamityModSupport.Items;

public class SupremeRestoration : CrossModItem, IModifyPotionDelay {
    public bool ApplyPotionDelay(Player player) {
        player.potionDelay = player.restorationDelayTime;
        player.AddBuff(BuffID.PotionSickness, player.potionDelay);
        return true;
    }

    public override void OnSetStaticDefaults() {
        Item.ResearchUnlockCount = 30;
    }

    public override void SetDefaults() {
        Item.width = 28;
        Item.height = 18;
        Item.useTurn = true;
        Item.maxStack = Item.CommonMaxStack;
        Item.healLife = 225;
        Item.useAnimation = 17;
        Item.useTime = 17;
        Item.useStyle = ItemUseStyleID.DrinkLiquid;
        Item.UseSound = SoundID.Item3;
        Item.consumable = true;
        Item.potion = true;
        Item.rare = ItemRarityID.Purple;
        Item.value = Item.buyPrice(0, 6, 50, 0);
    }

    protected override void SafeAddRecipes() {
        if (!CalamityMod.TryGetItem("Bloodstone", out ModItem bloodstone)) {
            return;
        }

        CreateRecipe(4)
            .AddIngredient<SuperRestorationPotion>(4)
            .AddIngredient(bloodstone, 3)
            .AddTile(TileID.Bottles)
            .Register()
            .SortBeforeModItem("SupremeHealingPotion", this)
            .DisableDecraft();
    }
}
