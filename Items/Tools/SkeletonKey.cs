﻿using Aequus.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Items.Tools
{
    public class SkeletonKey : ModItem
    {
        public override void Load()
        {
            On_ItemSlot.RightClick_ItemArray_int_int += ItemSlot_RightClick;
        }
        private static void ItemSlot_RightClick(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
        {
            var player = Main.LocalPlayer;
            if (context == ItemSlot.Context.InventoryItem && Main.mouseRight && Main.mouseRightRelease)
            {
                if (inv[slot].type == ItemID.LockBox && player.Aequus().hasSkeletonKey)
                {
                    if (ItemLoader.ConsumeItem(inv[slot], player))
                    {
                        inv[slot].stack--;
                    }
                    if (inv[slot].stack < 0)
                    {
                        inv[slot].SetDefaults();
                    }
                    SoundEngine.PlaySound(SoundID.Unlock);
                    Main.stackSplit = 30;
                    Main.mouseRightRelease = false;
                    player.OpenLockBox(inv[slot].type);
                    Recipe.FindRecipes();
                    return;
                }
            }
            orig(inv, context, slot);
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.GoldenKey);
            Item.value = Item.buyPrice(gold: 15);
            Item.rare++;
        }
    }

    public class SkeletonKeyGlobalTile : GlobalTile
    {
        public override void RightClick(int i, int j, int type)
        {
            if (type == TileID.Containers)
            {
                if (ChestType.GetStyle(Main.tile[i, j].TileFrameX) == ChestType.LockedGold && Main.LocalPlayer.Aequus().hasSkeletonKey)
                {
                    i -= Main.tile[i, j].TileFrameX % 36 / 18;
                    j -= Main.tile[i, j].TileFrameY % 36 / 18;
                    Chest.Unlock(i, j);
                }
            }
        }
    }

}