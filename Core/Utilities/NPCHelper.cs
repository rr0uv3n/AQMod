﻿using Aequus.Common.CrossMod;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus;

public static class NPCHelper {
    #region Drops
    public static NPCLoot GetNPCLoot(int npcId, ItemDropDatabase database = null) {
        return new NPCLoot(npcId, database ?? Main.ItemDropsDB);
    }
    public static NPCLoot GetNPCLoot(this NPC npc, ItemDropDatabase database = null) {
        return GetNPCLoot(npc.netID, database);
    }
    public static List<IItemDropRule> GetDropRules(int npcId, ItemDropDatabase database = null) {
        return (database ?? Main.ItemDropsDB).GetRulesForNPCID(npcId, includeGlobalDrops: false);
    }
    public static List<IItemDropRule> GetDropRules(this NPC npc, ItemDropDatabase database = null) {
        return GetDropRules(npc.netID, database);
    }

    public static void InheritDropRules(int parentNPCId, int childNPCId, ItemDropDatabase database = null) {
        var drops = GetDropRules(parentNPCId, database);
        var npcLoot = GetNPCLoot(childNPCId, database);
        foreach (var d in drops) {
            npcLoot.Add(d);
        }
    }
    #endregion

    public static bool ClearBuff(this NPC npc, int buffId) {
        int index = npc.FindBuffIndex(buffId);
        if (index != -1) {
            npc.DelBuff(buffId);
            return true;
        }
        return false;
    }

    public static void SyncNPC(NPC npc) {
        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
    }

    public static bool TryRetargeting(this NPC npc, bool faceTarget = true) {
        npc.TargetClosest(faceTarget: faceTarget);
        return npc.HasValidTarget;
    }

    public static bool IsProbablyACritter(this NPC npc) {
        return NPCID.Sets.CountsAsCritter[npc.type] || (npc.lifeMax < 5 && npc.lifeMax != 1);
    }

    public static void ClearAI(this NPC npc) {
        for (int i = 0; i < NPC.maxAI; i++) {
            npc.ai[i] = 0f;
        }
    }

    #region Buffs
    public static void SetImmune(int npcId, int buffId, bool? value = true) {
        NPCID.Sets.SpecificDebuffImmunity[npcId][buffId] = value;
    }

    public static bool IsImmune(this NPC npc, params int[] buffIds) {
        return IsImmune(npc.type, buffIds);
    }
    public static bool IsImmune(int npcId, params int[] buffIds) {
        if (NPCID.Sets.ImmuneToAllBuffs[npcId]) {
            return true;
        }
        if (NPCID.Sets.ImmuneToRegularBuffs[npcId] && !BuffID.Sets.IsATagBuff[npcId]) {
            return true;
        }
        foreach (int buffId in buffIds) {
            if (NPCID.Sets.SpecificDebuffImmunity[npcId][buffId] == true) {
                return true;
            }
        }
        return false;
    }

    public static bool IsImmune(this NPC npc, int buffId) {
        return IsImmune(npc.type, buffId);
    }
    public static bool IsImmune(int npcId, int buffId) {
        return NPCID.Sets.SpecificDebuffImmunity[npcId][buffId] == true;
    }
    #endregion

    #region Shops 
    public static NPCShop AddCustomValue(this NPCShop shop, int itemType, int customValue, params Condition[] conditions) {
        var item = new Item(itemType) {
            shopCustomPrice = customValue
        };
        return shop.Add(item, conditions);
    }
    public static NPCShop AddCustomValue<T>(this NPCShop shop, int customValue, params Condition[] conditions) where T : ModItem {
        return shop.AddCustomValue(ModContent.ItemType<T>(), customValue, conditions);
    }
    public static NPCShop AddCustomValue(this NPCShop shop, int itemType, double customValue, params Condition[] conditions) {
        return shop.AddCustomValue(itemType, (int)Math.Round(customValue), conditions);
    }
    public static NPCShop AddCustomValue<T>(this NPCShop shop, double customValue, params Condition[] conditions) where T : ModItem {
        return shop.AddCustomValue<T>((int)Math.Round(customValue), conditions);
    }

    public static int FindNextShopSlot(Item[] items) {
        for (int i = 0; i < items.Length; i++) {
            if (items[i] == null) {
                return i;
            }
        }
        return -1;
    }

    internal static NPCShop AddCrossMod<T>(this NPCShop shop, string itemName, params Condition[] conditions) where T : ModSupport<T> {
        if (!ModSupport<T>.TryFind<ModItem>(itemName, out var modItem)) {
            return shop;
        }
        return shop.Add(modItem.Type, conditions);
    }
    #endregion
}