﻿using Aequus.Common.Utilities;
using Aequus.Content.Events.GlimmerEvent.Peaceful;
using Aequus.Content.MainMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Aequus.Content.Events.GlimmerEvent.Sky {
    public class GlimmerSky : CustomSky {
        public const string Key = "Aequus:GlimmerEventSky";

        public Asset<Texture2D> skyTexture;
        public Asset<Texture2D> pixelTexture;

        public bool active;
        public float realOpacity;
        public bool checkDistance;

        public override void Update(GameTime gameTime) {
            if (active) {
                if (Opacity < 1f) {
                    Opacity = Math.Min(Opacity + 0.02f, 1f);
                }
            }
            else {
                if (Opacity > 0f) {
                    Opacity = Math.Max(Opacity - 0.02f, 0f);
                }
            }
            if (Main.gameMenu) {
                realOpacity = GlimmerMenu.Opacity;
            }
            else if (!checkDistance || GlimmerZone.omegaStarite != -1) {
                realOpacity = Opacity;
            }
            else {
                if (GlimmerZone.EventActive) {
                    realOpacity = MathHelper.Lerp(realOpacity, Opacity * Math.Max(1f - GlimmerSystem.GetTileDistance(Main.LocalPlayer) / (float)GlimmerZone.MaxTiles, 0f), 0.05f);
                }
                else if (PeacefulGlimmerZone.EventActive) {
                    realOpacity = MathHelper.Lerp(realOpacity, Opacity * Math.Max(1f - PeacefulGlimmerZone.CalcTiles(Main.LocalPlayer) / (float)PeacefulGlimmerZone.MaxTiles, 0f), 0.05f);
                }
                else {
                    realOpacity = Opacity;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth) {
            int y = (int)(-Main.screenPosition.Y / (Main.worldSurface * 16.0 - 600.0) * 200.0);
            var destinationRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
            destinationRectangle.Height -= y;
            var skyColor = new Color(255, 255, 255, 0);
            if (Main.tenthAnniversaryWorld) {
                skyColor = Color.HotPink * Helper.Wave(Main.GlobalTimeWrappedHourly, 0.5f, 1f);
            }
            skyColor *= realOpacity;
            if (Main.gameMenu) {
                Update(Main.gameTimeCache);
            }
            if (maxDepth == float.MaxValue && minDepth != float.MaxValue) {
                spriteBatch.Draw(AequusTextures.GlimmerSky.Value, destinationRectangle, skyColor);
                return;
            }

            if (minDepth > 10f)
                return;

            if (minDepth == float.MinValue) {
                minDepth = 0f;
            }

            float approxProgress = Math.Max(minDepth / 10f, 0.1f);
            destinationRectangle.Y += (int)(y * approxProgress);

            spriteBatch.Draw(AequusTextures.GlimmerSky.Value, destinationRectangle, Color.Lerp(skyColor, Color.Blue * 0.01f, 1f - approxProgress));

            try {
                if (Main.spriteBatch != null && !Main.spriteBatch.IsDisposed && Main.instance.GraphicsDevice != null && !Main.instance.GraphicsDevice.IsDisposed) {
                    DrawStars(spriteBatch, minDepth, maxDepth, y, approxProgress);
                }
            }
            catch {
            }
        }
        public void DrawStars(SpriteBatch spriteBatch, float minDepth, float maxDepth, int y, float approxProgress) {
            var drawRectangle = new Rectangle(-200, 0, Main.screenWidth + 400, Main.screenHeight + 400);
            drawRectangle.Y += y + (int)(y * approxProgress);
            drawRectangle.Y = Math.Max(drawRectangle.Y, -400);
            var starEndColor = Main.tenthAnniversaryWorld ? Color.Pink : Color.Blue;
            var cache = new LegacySpriteBatchCache(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, cache.customEffect, cache.transformMatrix);

            var drawData = new DrawData(pixelTexture.Value, drawRectangle, Color.White * realOpacity);
            var effect = GlimmerSceneEffect.StarShader.ShaderData;

            effect.Shader.Parameters["uRotation"].SetValue(6f - realOpacity * 0.5f);
            effect.UseOpacity(realOpacity);
            effect.UseSaturation(approxProgress);
            effect.UseColor(Color.Lerp(Color.White, starEndColor, approxProgress));
            effect.Apply(drawData);

            drawData.Draw(spriteBatch);

            spriteBatch.End();
            cache.Begin(spriteBatch);
        }

        public override bool IsActive() {
            return active || realOpacity > 0.01f || Opacity > 0f;
        }

        public override float GetCloudAlpha() {
            return 1f - realOpacity * 0.75f;
        }

        public override Color OnTileColor(Color inColor) {
            inColor.A = (byte)Math.Max(inColor.B, 20 * realOpacity);
            inColor.G = (byte)Math.Max(inColor.B, 50 * realOpacity);
            inColor.B = (byte)Math.Max(inColor.B, 100 * realOpacity);
            return inColor;
        }

        public override void Reset() {
        }

        public override void OnLoad() {
        }

        public override void Activate(Vector2 position, params object[] args) {
            active = true;
            if (!Main.dedServ) {
                skyTexture ??= AequusTextures.GlimmerSky;
                pixelTexture ??= AequusTextures.Pixel;
            }
        }

        public override void Deactivate(params object[] args) {
            active = false;
        }
    }
}