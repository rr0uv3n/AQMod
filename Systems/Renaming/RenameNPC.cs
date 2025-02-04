﻿using Aequus.Content.Villagers.SkyMerchant.UI;
using System.Diagnostics;
using System.IO;
using Terraria.ModLoader.IO;

namespace Aequus.Systems.Renaming;

[LegacyName("NPCNameTag")]
[LegacyName("NameTagGlobalNPC")]
public sealed class RenameNPC : GlobalNPC {
    public bool HasCustomName => !string.IsNullOrEmpty(CustomName);

    public string CustomName { get; set; } = string.Empty;

    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
        return CanRename(entity);
    }

    public override bool PreAI(NPC npc) {
        if (npc.realLife != -1 && Main.npc[npc.realLife].TryGetGlobalNPC<RenameNPC>(out var nameTag)) {
            CustomName = nameTag.CustomName;
        }
        if (HasCustomName) {
            npc.GivenName = CustomName;
        }
        return true;
    }

    public override void SaveData(NPC npc, TagCompound tag) {
        if (HasCustomName && npc.realLife == -1) {
            if (npc.netID < NPCID.Count) {
                tag["ID"] = npc.netID; // Vanilla entities don't load properly for some reason! So I am doing this to save their ID for reloading properly.
            }

            tag["NameTag"] = CustomName;
        }
    }

    public override void LoadData(NPC npc, TagCompound tag) {
        var position = npc.position;

        // Workaround for vanilla entities not saving and loading properly
        if (npc.netID == 0 && tag.TryGet("ID", out int netID)) {
            npc.netID = netID;
            npc.type = netID;
            npc.CloneDefaults(netID);
        }

        npc.position = position;
        npc.timeLeft = (int)(NPC.activeTime * 1.25f);
        npc.wet = Collision.WetCollision(npc.position, npc.width, npc.height);
        if (tag.TryGet("NameTag", out string savedNameTag)) {
            CustomName = savedNameTag;
        }

        LogCustomName(npc);
    }

    [Conditional("DEBUG")]
    private void LogCustomName(NPC npc) {
        if (HasCustomName) {
            Mod.Logger.Debug($"netID: {npc.netID}, {npc}");
            Mod.Logger.Debug(CustomName ?? "Null");
        }
    }

    public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
        binaryWriter.Write(CustomName);
    }

    public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
        CustomName = binaryReader.ReadString();
    }

    public static bool CanRename(NPC npc) {
        return !SkyMerchantRenameUIState.NPCBlacklist.Contains(npc.netID) && (npc.townNPC || NPCID.Sets.SpawnsWithCustomName[npc.type] || (!npc.boss && !NPCID.Sets.ShouldBeCountedAsBoss[npc.type] && !npc.immortal && !npc.dontTakeDamage && !npc.SpawnedFromStatue && (npc.realLife == -1 || npc.realLife == npc.whoAmI))) && (!NPCID.Sets.RespawnEnemyID.TryGetValue(npc.netID, out int respawnId) || respawnId != 0);
    }
}