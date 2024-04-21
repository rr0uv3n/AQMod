﻿using Aequus.Core.Networking;
using Aequus.DataSets;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Tools.Keychain;

public class KeychainPlayer : ModPlayer {
    public const int MAX_KEYS_ALLOWED = 32;
    public const string TAG_KEY = "Keychain";

    public bool hasKeyChain;
    public bool hasInfiniteKeyChain;
    public List<Item> keys = new();
    public List<Item> sortedKeysForIcons = new();

    public bool AcceptItem(Item acceptedItem) {
        lock (keys) {
            bool anyStacks = false;
            // Try stacking the accepted item into the keychain.
            foreach (Item item in keys) {
                if (item.type == acceptedItem.type) {
                    ItemLoader.TryStackItems(item, acceptedItem, out _);
                    anyStacks = true;
                    if (acceptedItem.stack <= 0) {
                        return true;
                    }
                }
            }

            // If there were any stack attempts, and this key is non-consumable, return.
            if (anyStacks && ItemDataSet.KeychainData.TryGetValue(acceptedItem.type, out var value) && value.NonConsumable) {
                return true;
            }

            // Prevent adding new keys if the limit was reached.
            if (keys.Count >= MAX_KEYS_ALLOWED) {
                return anyStacks;
            }

            // If there are still keys left, add them as a new stack.
            if (acceptedItem.stack > 0) {
                keys.Add(acceptedItem.Clone());
                acceptedItem.TurnToAir();
                return true;
            }
        }

        return false;
    }

    public bool ConsumeKey(Player player, int type) {
        lock (keys) {
            foreach (Item item in keys) {
                // Only iterate on keys which match the wanted type.
                if (item.type != type) {
                    continue;
                }

                // Return early if this key is non consumable.
                if ((ItemDataSet.KeychainData.TryGetValue(item.type, out var value) && value.NonConsumable) || hasInfiniteKeyChain) {
                    return true;
                }

                // Reduce Stack.
                item.stack--;
                if (item.stack <= 0) {
                    item.TurnToAir();
                }

                StackKeys();

                return true;
            }
        }

        return false;
    }

    public void StackKeys() {
        lock (keys) {
            List<Item> newKeys = new List<Item>(keys);
            keys.Clear();
            foreach (Item key in newKeys) {
                AcceptItem(key);
            }
        }
    }

    public void RefreshKeys() {
        lock (keys) {
            keys.RemoveAll((i) => i == null || i.IsAir);
            sortedKeysForIcons = new List<Item>(keys.Select(GetKeyIconItem));
            sortedKeysForIcons.Sort((item1, item2) => item1.Name.CompareTo(item2.Name));
        }
    }

    private Item GetKeyIconItem(Item key) {
        Item resultKey = key.Clone();
        //resultKey.stack = 1;
        return resultKey;
    }

    public void SpewItems(IEntitySource source) {
        lock (keys) {
            foreach (Item key in keys) {
                Player.QuickSpawnItemDirect(source, key, key.stack);
            }

            keys.Clear();
        }

        RefreshKeys();
    }

    public override void SaveData(TagCompound tag) {
        lock (keys) {
            if (keys.Count > 0) {
                tag[TAG_KEY] = keys;
            }
        }
    }

    public override void LoadData(TagCompound tag) {
        lock (keys) {
            if (tag.TryGet(TAG_KEY, out List<Item> keychain)) {
                keys = new List<Item>(keychain);
            }
        }

        RefreshKeys();
    }

    public override void ResetEffects() {
        hasKeyChain = false;
        hasInfiniteKeyChain = false;
    }
}

public class KeychainPlayerPacket : PacketHandler {
    public void Send(int p) {
        ModPacket packet = GetPacket();

        Player player = Main.player[p];
        KeychainPlayer keychain = player.GetModPlayer<KeychainPlayer>();

        lock (keychain) {
            packet.Write(keychain.keys.Count);
            for (int i = 0; i < keychain.keys.Count; i++) {
                ItemIO.Send(keychain.keys[i], packet, writeStack: true);
            }
        }
    }

    public override void Receive(BinaryReader reader, int sender) {
        int p = reader.ReadInt32();
        Player player = Main.player[p];
        KeychainPlayer keychain = player.GetModPlayer<KeychainPlayer>();

        lock (keychain) {
            keychain.keys.Clear();

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++) {
                keychain.keys.Add(ItemIO.Receive(reader, readStack: true));
            }

            keychain.RefreshKeys();
        }
    }
}
