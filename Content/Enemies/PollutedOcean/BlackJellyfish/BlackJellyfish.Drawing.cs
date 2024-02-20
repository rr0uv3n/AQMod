﻿using Aequus.Common.NPCs;
using System;
using System.Collections.Generic;
using Terraria.GameContent;

namespace Aequus.Content.Enemies.PollutedOcean.BlackJellyfish;

public partial class BlackJellyfish : AIJellyfish {
    private static readonly List<int> _drawList = new();

    private Vector2[] lightningDrawCoordinates;
    private float[] lightningDrawRotations;

    public override Color? GetAlpha(Color drawColor) {
        return drawColor * GetLightingIntensity();
    }

    public override void DrawBehind(int index) {
        if (NPC.ai[2] > 0f) {
            _drawList.Add(NPC.whoAmI);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        var drawCoordinates = NPC.Center;
        float opacity = NPC.Opacity;
        var origin = NPC.frame.Size() / 2f;
        //origin.X += 1f;
        //origin.Y += 6f;
        drawColor = NPC.GetAlpha(NPC.GetNPCColorTintedByBuffs(drawColor));
        if (!NPC.IsABestiaryIconDummy) {
            if (NPC.ai[2] > shockAttackLength) {
                return false;
            }
            else {
                opacity *= 1f - Math.Min(NPC.ai[2] / shockAttackLength, 1f);
            }
        }
        spriteBatch.Draw(TextureAssets.Npc[Type].Value, drawCoordinates - screenPos, NPC.frame, drawColor * opacity * 0.92f, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
        spriteBatch.Draw(AequusTextures.BlackJellyfish_Bag, drawCoordinates - screenPos, NPC.frame, drawColor * opacity, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
        return false;
    }

    private static void DrawExplodingJellyfishesLayer(SpriteBatch spriteBatch) {
        for (int i = 0; i < _drawList.Count; i++) {
            int jellyfish = _drawList[i];
            if (Main.npc[jellyfish].active && Main.npc[jellyfish].ModNPC is BlackJellyfish blackJellyfish) {
                blackJellyfish.DrawLightning(spriteBatch);
            }
        }
        _drawList.Clear();
    }

    private void DrawLightning(SpriteBatch spriteBatch) {
        var drawCoordinates = NPC.Center;

        if (lightningDrawCoordinates == null) {
            const int LightningSegments = 36;
            lightningDrawCoordinates = new Vector2[LightningSegments];
            lightningDrawRotations = new float[LightningSegments];
        }
        float attackProgress = Math.Min(NPC.ai[2] / shockAttackLength, 1f);
        float attackRange = MathF.Pow(attackProgress, 2f) * AttackRange;
        if (NPC.ai[2] > shockAttackLength) {
            float deathAnimation = (NPC.ai[2] - shockAttackLength) / 14f;
            if (deathAnimation < 0.5f) {
                attackRange += MathF.Sin(deathAnimation / 0.5f * MathHelper.Pi) * 16f;
            }
            else {
                attackRange *= 1f - (deathAnimation - 0.5f) / 0.5f;
            }
        }
        for (int i = 0; i < lightningDrawCoordinates.Length; i++) {
            float rotation = i * MathHelper.TwoPi / (lightningDrawCoordinates.Length - 1) + Main.GlobalTimeWrappedHourly * 38f;
            lightningDrawCoordinates[i] = drawCoordinates + new Vector2(attackRange, 0f).RotatedBy(rotation) - Main.screenPosition + Main.rand.NextVector2Square(-attackProgress, attackProgress) * 6f;
            lightningDrawRotations[i] = rotation - MathHelper.PiOver2;
        }

        Vector2 origin = NPC.frame.Size() / 2f;
        Color lightningColor = new Color(255, 200, 100, 10);
        float attackRangeNormalized = attackRange / AttackRange;
        if (NPC.ai[2] < shockAttackLength) {
            Color color = Color.Black;
            if (NPC.ai[2] % 8 < 4) {
                color = lightningColor;
            }
            drawCoordinates += Main.rand.NextVector2Square(-attackProgress, attackProgress) * 4f - Main.screenPosition;
            float npcScale = NPC.scale + attackProgress * 0.3f;
            for (int i = 0; i < 4; i++) {
                spriteBatch.Draw(TextureAssets.Npc[Type].Value, drawCoordinates + new Vector2(2f * NPC.scale, 0f).RotatedBy(i * MathHelper.PiOver2 + NPC.rotation), NPC.frame, lightningColor with { A = 60 } * attackProgress, NPC.rotation, origin, npcScale, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(TextureAssets.Npc[Type].Value, drawCoordinates, NPC.frame, color * attackProgress, NPC.rotation, origin, npcScale, SpriteEffects.None, 0f);
        }
        else {
            spriteBatch.Draw(AequusTextures.BloomStrong, drawCoordinates - Main.screenPosition, null, lightningColor * attackRangeNormalized, 0f, AequusTextures.BloomStrong.Size() / 2f, MathF.Pow(attackRange / AttackRange, 1.5f), SpriteEffects.None, 0f);
            spriteBatch.Draw(AequusTextures.Bloom, drawCoordinates - Main.screenPosition, null, lightningColor * attackRangeNormalized, 0f, AequusTextures.Bloom.Size() / 2f, MathF.Pow(attackRange / AttackRange, 3f) * 1.25f, SpriteEffects.None, 0f);
        }

        DrawHelper.DrawBasicVertexLineWithProceduralPadding(AequusTextures.BlackJellyfishVertexStrip, lightningDrawCoordinates, lightningDrawRotations,
            p => lightningColor * attackRangeNormalized * NPC.Opacity,
            p => Math.Max(attackRangeNormalized < 1f ? attackRangeNormalized : MathF.Pow(attackRangeNormalized, 1.5f), 0.25f) * NPC.Opacity * 8f
        );
    }
}