﻿using Aequus.Common.Items.Components;

namespace Aequus.Content.Potions.Healing.Restoration;

public class GreaterRestorationPotion : ModItem, IApplyPotionDelay {
    public bool ApplyPotionDelay(Player player) {
        player.potionDelay = player.restorationDelayTime;
        player.AddBuff(BuffID.PotionSickness, player.potionDelay);
        return true;
    }

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 30;
    }

    public override void SetDefaults() {
        Item.UseSound = SoundID.Item3;
        Item.healLife = 135;
        Item.useStyle = ItemUseStyleID.DrinkLiquid;
        Item.useTurn = true;
        Item.useAnimation = 17;
        Item.useTime = 17;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.width = 14;
        Item.height = 24;
        Item.potion = true;
        Item.value = Item.sellPrice(silver: 20);
        Item.rare = ItemRarityID.Orange;
    }

    public override void AddRecipes() {
        var Player = Main.LocalPlayer;
        //AccessorySlotLoader accessorySlotLoader = LoaderManager.Get<AccessorySlotLoader>();
        //ModAccessorySlotPlayer accessorySlotPlayer = Player.GetModPlayer<ModAccessorySlotPlayer>();
        //for (int i = 0; i < accessorySlotPlayer.SlotCount; i++) {
        //    if (accessorySlotLoader.ModdedIsItemSlotUnlockedAndUsable(i, Player)) {
        //        ModAccessorySlot slot = accessorySlotLoader.Get(i, Player);
        //        if (slot.FunctionalItem != null && !slot.FunctionalItem.IsAir) {
        //        }
        //    }
        //}
        CreateRecipe(3)
            .AddIngredient(ItemID.BottledWater, 3)
            .AddIngredient(ItemID.PixieDust, 3)
            .AddIngredient(ItemID.Sapphire)
            .AddTile(TileID.Bottles)
            .Register()
            .DisableDecraft();
    }
}