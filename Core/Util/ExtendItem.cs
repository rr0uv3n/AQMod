﻿using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;

namespace AequusRemake.Core.Util;

public static class ExtendItem {
    public static readonly IItemEntryFilter ToolFilter = new ItemFilters.Tools();

    public static void CloneResearchCount(this Item item, int itemToCopy) {
        if (CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.TryGetValue(itemToCopy, out var researchCount)) {
            item.ResearchUnlockCount = researchCount;
        }
        else {
            item.ResearchUnlockCount = 0;
        }
    }

    public static void DropHearts(IEntitySource source, Rectangle hitbox, int guaranteedAmount, int randomAmount) {
        for (int i = 0; i < guaranteedAmount; i++) {
            Item.NewItem(source, hitbox, ItemID.Heart);
        }
        randomAmount = Main.rand.Next(randomAmount);
        for (int i = 0; i < randomAmount; i++) {
            Item.NewItem(source, hitbox, ItemID.Heart);
        }
    }

    /// <summary>Returns all 'Overrides' of a specified item. (<see cref="ContentSamples.CreativeResearchItemPersistentIdOverride"/>)</summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public static IEnumerable<int> GetAllOverridesOfItemId(int itemId) {
        yield return itemId;

        foreach (var i in ContentSamples.CreativeResearchItemPersistentIdOverride) {
            if (i.Value == itemId) {
                yield return i.Key;
            }
        }

        if (ContentSamples.CreativeResearchItemPersistentIdOverride.TryGetValue(itemId, out int myAlt) && itemId != myAlt) {
            foreach (var i in GetAllOverridesOfItemId(myAlt)) {
                yield return i;
            }
        }
    }

    /// <summary>Attempts to transform an item into a different item id, while preserving its positon, prefix, and stack</summary>
    /// <param name="item"></param>
    /// <param name="newType"></param>
    public static void Transform(this Item item, int newType) {
        // TODO: Find a way to preserve global item content?

        var position = item.Bottom;
        int whoAmI = item.whoAmI;
        int prefix = item.prefix;
        int stack = item.stack;
        item.SetDefaults(newType);
        item.Prefix(prefix);
        item.stack = stack;
        item.whoAmI = whoAmI; // Unused so not needed?
        item.Bottom = position;
    }

    /// <param name="item"></param>
    /// <param name="frame"></param>
    /// <returns>Vanilla-based method for calculating the draw position for an item in the world.</returns>
    public static Vector2 WorldDrawPos(Item item, Rectangle frame) {
        return new Vector2(item.position.X - Main.screenPosition.X + frame.Width / 2 + item.width / 2 - frame.Width / 2, item.position.Y - Main.screenPosition.Y + frame.Height / 2 + item.height - frame.Height + 2f);
    }

    public static void LazyCustomSwordDefaults<T>(this Item item, int swingTime) where T : ModProjectile {
        item.useTime = swingTime;
        item.useAnimation = swingTime;
        item.shoot = ModContent.ProjectileType<T>();
        item.shootSpeed = 1f;
        item.DamageType = DamageClass.Melee;
        item.useStyle = ItemUseStyleID.Shoot;
        item.channel = true;
        item.noMelee = true;
        item.noUseGraphic = true;
    }

    #region Statics
    /// <summary>Registers this item as a drink (Potion).</summary>
    /// <param name="modItem"></param>
    /// <param name="colors">Particle Colors for when this potion is consumed.</param>
    public static void StaticDefaultsToPotion(this ModItem modItem, params Color[] colors) {
        ItemSets.DrinkParticleColors[modItem.Type] = colors;
        Main.RegisterItemAnimation(modItem.Type, new DrawAnimationVertical(int.MaxValue, 3));
    }

    /// <summary>Registers this item as a drink. The difference between this and <see cref="StaticDefaultsToPotion(ModItem, Color[])"/> is that drinks have held sprites.</summary>
    /// <param name="modItem"></param>
    /// <param name="colors">Particle Colors for when this drink is consumed.</param>
    public static void StaticDefaultsToFoodDrink(this ModItem modItem, params Color[] colors) {
        ItemSets.IsFood[modItem.Type] = true;
        ItemSets.DrinkParticleColors[modItem.Type] = colors;
        Main.RegisterItemAnimation(modItem.Type, new DrawAnimationVertical(int.MaxValue, 3));
    }

    /// <summary>Registers this item as a food.</summary>
    /// <param name="modItem"></param>
    /// <param name="colors">Particle Colors for when this food is eaten.</param>
    public static void StaticDefaultsToFood(this ModItem modItem, params Color[] colors) {
        ItemSets.IsFood[modItem.Type] = true;
        ItemSets.FoodParticleColors[modItem.Type] = colors;
        Main.RegisterItemAnimation(modItem.Type, new DrawAnimationVertical(int.MaxValue, 3));
    }
    #endregion
}