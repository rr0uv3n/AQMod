﻿using Aequus.Content.Carpentery.Photobook;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.Camera
{
    public class Photobook : ModItem, ItemHooks.IUpdateVoidBag
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item92;
            Item.value = Item.buyPrice(gold: 1);
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                Main.playerInventory = false;
                Main.npcChatText = "";
                Main.editChest = false;
                Main.editSign = false;
                Aequus.UserInterface.SetState(new PhotobookUIState());
            }
            return true;
        }

        public override void UpdateInventory(Player player)
        {
            player.GetModPlayer<PhotobookPlayer>().hasPhotobook = true;
        }

        public virtual void UpdateBank(Player player, AequusPlayer aequus, int slot, int bank)
        {
            player.GetModPlayer<PhotobookPlayer>().hasPhotobook = true;
        }
    }
}