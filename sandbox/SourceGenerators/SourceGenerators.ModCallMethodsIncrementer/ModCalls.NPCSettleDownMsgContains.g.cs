﻿namespace Aequus;

public partial class ModCallHandler {
    [System.Runtime.CompilerServices.CompilerGenerated]
    static bool NPCSettleDownMsgContains(object[] args) {
        if (args[1] is not int npcId) {
            LogError($"Mod Call Parameter index 1 (\"npcId\") did not match Type \"int\".");
            return default;
        }

        return global::Aequus.Content.Villagers.NPCSettleDownMessage.InBlacklist(npcId);
    }
}