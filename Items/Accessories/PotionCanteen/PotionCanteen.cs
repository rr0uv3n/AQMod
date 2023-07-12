﻿using Aequus;
using Aequus.Common.Buffs;
using Aequus.Common.Items;
using Aequus.Content;
using Aequus.Common.Items.EquipmentBooster;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Items.Accessories.PotionCanteen {
    public class PotionCanteen : ModItem, ItemHooks.IHookPickupText {
        public int itemIDLookup;
        public int buffID;

        public bool HasBuff => buffID > 0;

        public static LocalizedText AltName { get; private set; }

        public override void SetStaticDefaults() {
            EquipBoostDatabase.Instance.SetNoEffect(Type);
            AltName = this.GetLocalization("DisplayNameAlt");
        }

        public void SetPotionDefaults() {
            Item.buffType = buffID;
            Item.rare = ItemRarityID.Orange;
            if (itemIDLookup > 0) {
                Item.rare += ContentSamples.ItemsByType[itemIDLookup].rare;
            }
            Item.rare = Math.Min(Item.rare, ItemRarityID.Purple);
            Item.Prefix(Item.prefix);
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory(20, 20);
            Item.value = Item.buyPrice(gold: 10);
            SetPotionDefaults();
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            if (buffID > 0) {
                if (Main.myPlayer == player.whoAmI) {
                    AequusBuff.preventRightClick.Add(buffID);
                }
                player.AddBuff(buffID, 1, quiet: true);
            }
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.Bottle)
                .AddIngredient(ItemID.CrystalShard, 20)
                .AddIngredient(ItemID.PixieDust, 30)
                .Register();

            for (int i = 0; i < ItemLoader.ItemCount; i++) {
                if (AequusItem.IsPotion(ContentSamples.ItemsByType[i])) {
                    var r = CreateRecipe()
                        .AddIngredient<PotionCanteen>()
                        .AddIngredient(i, 5);
                    var canteen = r.createItem.ModItem<PotionCanteen>();
                    canteen.itemIDLookup = i;
                    canteen.buffID = ContentSamples.ItemsByType[i].buffType;
                    canteen.SetPotionDefaults();
                    r.Register();
                }
            }
        }

        public string GetName(string originalName) {
            return buffID <= 0 ? Lang.GetItemName(Type).Value : AltName.Format(Lang.GetBuffName(buffID));
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            if (buffID > 0) {
                foreach (var t in tooltips) {
                    if (t.Name == "ItemName") {
                        t.Text = GetName(t.Text);
                    }
                    if (t.Name == "Tooltip0") {
                        if (buffID < BuffID.Count) {
                            t.Text = Language.GetTextValue($"BuffDescription.{BuffID.Search.GetName(buffID)}");
                        }
                        else {
                            var modBuff = BuffLoader.GetBuff(buffID);
                            t.Text = Language.GetTextValue($"Mods.{modBuff.Mod.Name}.BuffDescription.{modBuff.Name}");
                        }
                    }
                }
            }
        }

        private Rectangle GetLiquidFrame(Texture2D liquidTexture) {
            return liquidTexture.Frame(verticalFrames: 16, frameY: (int)Main.GameUpdateCount / 7 % 15);
        }

        private Color GetLiquidColor() {
            return PotionColorsDatabase.GetColorFromItemID(itemIDLookup).HueAdd(Helper.Wave(Main.GlobalTimeWrappedHourly, -0.04f, 0.04f)) * Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.8f, 1f);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            if (!HasBuff) {
                return true;
            }
            var liquidTexture = AequusTextures.PotionCanteen_Liquid.Value;
            var liquidFrame = GetLiquidFrame(liquidTexture);
            var liquidColor = GetLiquidColor();
            float a = drawColor.A > 0 ? drawColor.A / 255f : Main.inventoryBack.A / 255f;
            spriteBatch.Draw(liquidTexture, position, liquidFrame, liquidColor* a, 0f, origin, scale, SpriteEffects.None, 0f);
            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
            var texture = TextureAssets.Item[Type].Value;
            var position = Item.Center - Main.screenPosition;
            var origin = texture.Size() / 2f;
            Item.GetItemDrawData(out var frame);
            if (HasBuff) {
                var liquidTexture = AequusTextures.PotionCanteen_Liquid.Value;
                var liquidFrame = GetLiquidFrame(liquidTexture);
                var liquidColor = GetLiquidColor();
                spriteBatch.Draw(liquidTexture, position, liquidFrame, liquidColor, rotation, origin, scale, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(texture, position, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void SaveData(TagCompound tag) {
            if (!HasBuff)
                return;
            AequusBuff.SaveBuffID(tag, "Buff", buffID);
            AequusItem.SaveItemID(tag, "Item", itemIDLookup);
        }

        public override void LoadData(TagCompound tag) {
            buffID = AequusBuff.LoadBuffID(tag, "Buff");
            itemIDLookup = AequusItem.LoadItemID(tag, "Item");
            SetPotionDefaults();
        }

        public void OnPickupText(int index, PopupTextContext context, int stack, bool noStack, bool longText) {
            SetPotionDefaults();
            Main.popupText[index].name = GetName(Main.popupText[index].name);
        }
    }
}