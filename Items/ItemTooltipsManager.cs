﻿using Aequus.Common.Configuration;
using Aequus.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Aequus.Items
{
    public sealed partial class ItemTooltipsHelper : GlobalItem
    {
        public static Color MysteriousGuideTooltip => new Color(225, 100, 255, 255);
        public static Color DemonSiegeTooltip => new Color(255, 170, 150, 255);

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.createTile > -1)
            {
                AequusHelpers.Iterations = 0;
                GetSpecialTileTooltip(item.createTile, out bool tinkerersWorkshopItem);
                if (tinkerersWorkshopItem)
                {
                    int index = GetLineIndex(tooltips, "Tooltip#");
                    tooltips.Insert(index + 2, new TooltipLine(Mod, "HookBarbsStation", Aequus.GetText("Tooltips.HookBarbStation")));
                }
            }
            if (Dedicated.TryGetValue(item.type, out var dedication))
            {
                tooltips.Add(new TooltipLine(Mod, "DedicatedItem", Aequus.GetText("Tooltips.DedicatedItem")) { OverrideColor = dedication.color });
            }
        }
        private void GetSpecialTileTooltip(int tile, out bool tinkerersWorkshopItem)
        {
            tinkerersWorkshopItem = tile == TileID.TinkerersWorkbench;
            if (tile >= Main.maxTileSets && AequusHelpers.Iterations < 10)
            {
                var adjTiles = TileLoader.GetTile(tile).AdjTiles;
                if (adjTiles == null || adjTiles.Length == 0)
                {
                    return;
                }
                foreach (var tile2 in adjTiles)
                {
                    AequusHelpers.Iterations++;
                    GetSpecialTileTooltip(tile2, out bool tinkerersWorkshopItem2);
                    AequusHelpers.Iterations = 0;
                    if (tinkerersWorkshopItem2)
                        tinkerersWorkshopItem = true;
                }
            }
        }

        public static int GetLineIndex(List<TooltipLine> tooltips, string lineName)
        {
            int myIndex = FindLineIndex(lineName);
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].Mod == "Terraria" && FindLineIndex(tooltips[i].Name) >= myIndex)
                {
                    return i;
                }
            }
            return 1;
        }

        private static int FindLineIndex(string name)
        {
            for (int i = 0; i < OutdatedTooltipNames.Length; i++)
            {
                if (name == OutdatedTooltipNames[i])
                {
                    return i;
                }
            }
            return -1;
        }

        public static void ChangeVanillaLine(List<TooltipLine> tooltips, string name, Action<TooltipLine> modify)
        {
            foreach (var t in tooltips)
            {
                if (t.Name == name)
                {
                    modify(t);
                    return;
                }
            }
        }

        internal static string UseAnimText(float useAnimation)
        {
            if (useAnimation <= 8)
            {
                return Language.GetTextValue("LegacyTooltip.6");
            }
            else if (useAnimation <= 20)
            {
                return Language.GetTextValue("LegacyTooltip.7");
            }
            else if (useAnimation <= 25)
            {
                return Language.GetTextValue("LegacyTooltip.8");
            }
            else if (useAnimation <= 30)
            {
                return Language.GetTextValue("LegacyTooltip.9");
            }
            else if (useAnimation <= 35)
            {
                return Language.GetTextValue("LegacyTooltip.10");
            }
            else if (useAnimation <= 45)
            {
                return Language.GetTextValue("LegacyTooltip.11");
            }
            else if (useAnimation <= 55)
            {
                return Language.GetTextValue("LegacyTooltip.12");
            }
            return Language.GetTextValue("LegacyTooltip.13");
        }

        internal static string KBText(float knockback)
        {
            if (knockback == 0f)
            {
                return Language.GetTextValue("LegacyTooltip.14");
            }
            else if (knockback <= 1.5)
            {
                return Language.GetTextValue("LegacyTooltip.15");
            }
            else if (knockback <= 3f)
            {
                return Language.GetTextValue("LegacyTooltip.16");
            }
            else if (knockback <= 4f)
            {
                return Language.GetTextValue("LegacyTooltip.17");
            }
            else if (knockback <= 6f)
            {
                return Language.GetTextValue("LegacyTooltip.18");
            }
            else if (knockback <= 7f)
            {
                return Language.GetTextValue("LegacyTooltip.19");
            }
            else if (knockback <= 9f)
            {
                return Language.GetTextValue("LegacyTooltip.20");
            }
            else if (knockback <= 11f)
            {
                return Language.GetTextValue("LegacyTooltip.21");
            }
            return Language.GetTextValue("LegacyTooltip.22");
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Aequus" && line.Name == "DedicatedItem")
            {
                DrawDedicatedTooltip(line);
                return false;
            }
            return true;
        }

        public static void DrawDevTooltip(DrawableTooltipLine line)
        {
            DrawDevTooltip(line.Text, line.X, line.Y, line.Rotation, line.Origin, line.BaseScale, line.OverrideColor.GetValueOrDefault(line.Color));
        }
        public static void DrawDevTooltip(string text, int x, int y, Color color)
        {
            DrawDevTooltip(text, x, y, 0f, Vector2.Zero, Vector2.One, color);
        }
        public static void DrawDevTooltip(string text, int x, int y, float rotation, Vector2 origin, Vector2 baseScale, Color color)
        {
            if (string.IsNullOrWhiteSpace(text)) // since you can rename items.
            {
                return;
            }
            var font = FontAssets.MouseText.Value;
            var size = font.MeasureString(text);
            var center = size / 2f;
            var transparentColor = color * 0.4f;
            transparentColor.A = 0;
            var texture = Aequus.MyTex("Assets/TextBloom");
            var spotlightOrigin = texture.Size() / 2f;
            float spotlightRotation = rotation + MathHelper.PiOver2;
            var spotlightScale = new Vector2(1.2f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 4f) * 0.145f, center.Y * 0.15f);

            // black BG
            Main.spriteBatch.Draw(texture, new Vector2(x, y - 6f) + center, null, Color.Black * 0.3f, rotation,
            spotlightOrigin, new Vector2(size.X / texture.Width * 2f, center.Y / texture.Height * 2.5f), SpriteEffects.None, 0f);
            ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, font, text, new Vector2(x, y), Color.Black,
                rotation, origin, baseScale);

            if (ModContent.GetInstance<ClientConfiguration>().HighQuality)
            {
                int reset = EffectsSystem.EffectRand.SetRand(Main.LocalPlayer.name.GetHashCode());

                // particles
                var particleTexture = Aequus.MyTex("Assets/Bloom");
                var particleOrigin = particleTexture.Size() / 2f;
                int amt = (int)EffectsSystem.EffectRand.Rand(size.X / 3, size.X);
                for (int i = 0; i < amt; i++)
                {
                    float lifeTime = (EffectsSystem.EffectRand.Rand(20f) + Main.GlobalTimeWrappedHourly * 2f) % 20f;
                    int baseParticleX = (int)EffectsSystem.EffectRand.Rand(4f, size.X - 4f);
                    int particleX = baseParticleX + (int)AequusHelpers.Wave(lifeTime + Main.GlobalTimeWrappedHourly * EffectsSystem.EffectRand.Rand(2f, 5f), -EffectsSystem.EffectRand.Rand(3f, 10f), EffectsSystem.EffectRand.Rand(3f, 10f));
                    int particleY = (int)EffectsSystem.EffectRand.Rand(10f);
                    float scale = EffectsSystem.EffectRand.Rand(0.2f, 0.4f);
                    if (baseParticleX > 14 && baseParticleX < size.X - 14 && EffectsSystem.EffectRand.RandChance(6))
                    {
                        scale *= 2f;
                    }
                    scale /= 4.5f;
                    var clr = color;
                    if (lifeTime < 0.3f)
                    {
                        clr *= lifeTime / 0.3f;
                    }
                    if (lifeTime < 5f)
                    {
                        if (lifeTime > MathHelper.PiOver2)
                        {
                            float timeMult = (lifeTime - MathHelper.PiOver2) / MathHelper.PiOver2;
                            scale -= timeMult * 0.4f;
                            if (scale < 0f)
                            {
                                continue;
                            }
                            int colorMinusAmount = (int)(timeMult * 255f);
                            clr.R = (byte)Math.Max(clr.R - colorMinusAmount, 0);
                            clr.G = (byte)Math.Max(clr.G - colorMinusAmount, 0);
                            clr.B = (byte)Math.Max(clr.B - colorMinusAmount, 0);
                            clr.A = (byte)Math.Max(clr.A - colorMinusAmount, 0);
                            if (clr.R == 0 && clr.G == 0 && clr.B == 0 && clr.A == 0)
                            {
                                continue;
                            }
                        }
                        if (scale > 0.4f)
                        {
                            Main.spriteBatch.Draw(particleTexture, new Vector2(x + particleX, y + particleY - lifeTime * 15f + 10), null, clr * 2f, 0f, particleOrigin, scale * 0.5f, SpriteEffects.None, 0f);
                        }
                        Main.spriteBatch.Draw(particleTexture, new Vector2(x + particleX, y + particleY - lifeTime * 15f + 10), null, clr, 0f, particleOrigin, scale, SpriteEffects.None, 0f);
                    }
                }

                EffectsSystem.EffectRand.SetRand(reset);
            }

            // light effect
            Main.spriteBatch.Draw(texture, new Vector2(x, y - 6f) + center, null, transparentColor * 0.3f, spotlightRotation,
               spotlightOrigin, spotlightScale * 1.1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, new Vector2(x, y - 6f) + center, null, transparentColor * 0.3f, spotlightRotation,
               spotlightOrigin, spotlightScale * 1.1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, new Vector2(x, y - 6f) + center, null, transparentColor * 0.35f, spotlightRotation,
               spotlightOrigin, spotlightScale * 2f, SpriteEffects.None, 0f);

            // colored text
            ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, new Vector2(x, y), color,
                rotation, origin, baseScale);

            // glowy effect on text
            float wave = AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 10f, 0f, 1f);
            for (int i = 1; i <= 2; i++)
            {
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, new Vector2(x + wave * 1f * i, y), transparentColor,
                    rotation, origin, baseScale);
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, new Vector2(x - wave * 1f * i, y), transparentColor,
                    rotation, origin, baseScale);
            }
        }

        public static void DrawDedicatedTooltip(DrawableTooltipLine line)
        {
            DrawDedicatedTooltip(line.Text, line.X, line.Y, line.Rotation, line.Origin, line.BaseScale, line.OverrideColor.GetValueOrDefault(line.Color));
        }
        public static void DrawDedicatedTooltip(string text, int x, int y, Color color)
        {
            DrawDedicatedTooltip(text, x, y, 0f, Vector2.Zero, Vector2.One, color);
        }
        public static void DrawDedicatedTooltip(string text, int x, int y, float rotation, Vector2 origin, Vector2 baseScale, Color color)
        {
            float brightness = Main.mouseTextColor / 255f;
            float brightnessProgress = (Main.mouseTextColor - 190f) / (byte.MaxValue - 190f);
            color = Colors.AlphaDarken(color);
            color.A = 0;
            var font = FontAssets.MouseText.Value;
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, new Vector2(x, y), new Color(0, 0, 0, 255), rotation, origin, baseScale);
            for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.PiOver2 + 0.01f)
            {
                var coords = new Vector2(x, y);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, coords, new Color(0, 0, 0, 255), rotation, origin, baseScale);
            }
            for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.PiOver2 + 0.01f)
            {
                var coords = new Vector2(x, y) + f.ToRotationVector2() * (brightness / 2f);
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, coords, color * 0.8f, rotation, origin, baseScale);
            }
            for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.PiOver4 + 0.01f)
            {
                var coords = new Vector2(x, y) + (f + Main.GlobalTimeWrappedHourly).ToRotationVector2() * (brightnessProgress * 3f);
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, coords, color * 0.2f, rotation, origin, baseScale);
            }
        }
    }
}