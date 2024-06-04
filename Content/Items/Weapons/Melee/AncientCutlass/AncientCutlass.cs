﻿using Aequus.DataSets;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.GameContent.ItemDropRules;
using tModLoaderExtended.Terraria;

namespace Aequus.Content.Items.Weapons.Melee.AncientCutlass;

public class AncientCutlass : ModItem {
    private readonly MethodInfo _resolveRules = typeof(ItemDropResolver).GetMethod("ResolveRule", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
    public static int DropChance { get; private set; } = 2;
    public static int ValueDropChanceDecrease { get; private set; } = Item.silver;

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.DyeTradersScimitar);
        Item.crit = 8;
    }

    public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) {
        NPC realTarget = target;
        if (realTarget.realLife > -1) {
            realTarget = Main.npc[realTarget.realLife];
        }

        int dropChance = DropChance * Math.Max(target.rarity + 1, 1);
        if (target.boss || NPCDataSet.CannotPickpocketItemsFrom.Contains(target.type) || !Main.rand.NextBool(dropChance)) {
            return;
        }

        List<IItemDropRule> drops = Main.ItemDropsDB.GetRulesForNPCID(realTarget.netID, includeGlobalDrops: false);

        if (drops.Count == 0) {
            return;
        }

        NewItemCache.Begin();
        try {
            ItemDropAttemptResult result;
            do {
                int randomIndex = Main.rand.Next(drops.Count);
                IItemDropRule dropRule = Main.rand.Next(drops);
                DropAttemptInfo dropInfo = ExtendLoot.GetDropAttemptInfo(realTarget, player);

                result = (ItemDropAttemptResult)_resolveRules.Invoke(Main.ItemDropSolver, [dropRule, dropInfo]);
                drops.RemoveAt(randomIndex);
            }
            while (drops.Count > 0 && result.State != ItemDropAttemptResultState.Success);
        }
        catch (Exception ex) {
            Mod.Logger.Error(ex);
        }
        NewItemCache.End();

        Rectangle hitbox = target.Hitbox;
        foreach (Item item in NewItemCache.DroppedItems) {
            if (item.stack < 1) {
                continue;
            }

            item.stack = Math.Max(item.stack / 10, 1);
            int valueDropChance = item.value / ValueDropChanceDecrease * item.stack;
            if (valueDropChance > 0 && !Main.rand.NextBool(valueDropChance + 1)) {
                continue;
            }

            Item.NewItem(target.GetSource_Loot("Aequus: Steal"), hitbox, item);
        }
    }
}