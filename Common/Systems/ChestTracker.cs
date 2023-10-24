﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Systems;

public class ChestTracker : ModSystem {
    public static readonly List<Point> UnseenChests = new();
    public static int CheckingChest;
    public static int CheckingPlayer;

    public override void SaveWorldData(TagCompound tag) {
        UnseenChests.Remove(new Point(-1, -1));
        if (UnseenChests.Count > 1) {
            tag["UnopenedChestsX"] = UnseenChests.ConvertAll((p) => p.X);
            tag["UnopenedChestsY"] = UnseenChests.ConvertAll((p) => p.Y);
        }
    }

    public override void LoadWorldData(TagCompound tag) {
        if (tag.TryGet<List<int>>("UnopenedChestsX", out var x) && tag.TryGet<List<int>>("UnopenedChestsY", out var y)) {
            if (x.Count != y.Count) {
                return;
            }

            for (int i = 0; i < x.Count; i++) {
                UnseenChests.Add(new Point(x[i], y[i]));
            }
        }
    }

    public override void ClearWorld() {
        UnseenChests?.Clear();
    }

    public static bool IsRealChest(int i) {
        return Main.chest[i] != null && !Main.chest[i].bankChest && Main.chest[i].x > 0 && Main.chest[i].y > 0 && Main.chest[i].item != null;
    }

    private void UpdateCheckChest() {
        CheckingChest %= UnseenChests.Count;
        int chestID = Chest.FindChest(UnseenChests[CheckingChest].X, UnseenChests[CheckingChest].Y);
        if (chestID == -1 || !IsRealChest(chestID)) {
            UnseenChests.RemoveAt(CheckingChest);
        }
        CheckingChest++;
    }

    private void UpdatePlayerNearChests() {
        if (Main.CurrentFrameFlags.ActivePlayersCount <= 0)
            return;
        CheckingPlayer %= Main.CurrentFrameFlags.ActivePlayersCount;
        for (int i = 0; i < UnseenChests.Count; i++) {
            if (Main.player[CheckingPlayer].Distance(UnseenChests[i].ToWorldCoordinates(16f, 16f)) < Chest.chestStackRange) {
                UnseenChests.RemoveAt(i);
                i--;
            }
        }
        CheckingPlayer++;
    }

    public override void PostUpdateEverything() {
        if (UnseenChests.Count == 0) {
            UnseenChests.Add(new Point(-1, -1));
            for (int i = 0; i < Main.maxChests; i++) {
                if (IsRealChest(i)) {
                    UnseenChests.Add(new Point(Main.chest[i].x, Main.chest[i].y));
                }
            }
        }
        //Main.NewText(UnopenedChests.Count);
        UpdateCheckChest();
        UpdatePlayerNearChests();
    }
}