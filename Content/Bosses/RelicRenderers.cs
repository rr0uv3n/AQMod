﻿using Aequus.Core.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Aequus.Content.Bosses;

public interface IRelicRenderer {
    void DrawRelic(int x, int y, Vector2 drawCoordinates, Color drawColor, SpriteEffects spriteEffects, float glow);

    public static void Draw(SpriteBatch spriteBatch, Texture2D texture, Vector2 drawPos, Rectangle frame, Color color, Vector2 origin, SpriteEffects effects, float glow) {
        drawPos /= 4f;
        drawPos.Floor();
        drawPos *= 4f;

        spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);

        float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 2f) * 0.3f + 0.7f;
        var effectColor = color with { A = 0 } * 0.1f * scale;
        for (float theta = 0f; theta < 1f; theta += 355f / (678f * MathHelper.Pi)) {
            spriteBatch.Draw(texture, drawPos + (MathHelper.TwoPi * theta).ToRotationVector2() * (6f + glow * 2f), frame, effectColor, 0f, origin, 1f, effects, 0f);
        }
    }
}

public record class BasicRelicRenderer(RequestCache<Texture2D> Texture) : IRelicRenderer {
    public void DrawRelic(int x, int y, Vector2 drawCoordinates, Color drawColor, SpriteEffects spriteEffects, float glow) {
        var frame = Texture.Bounds();
        IRelicRenderer.Draw(Main.spriteBatch, Texture.Value, drawCoordinates, frame, drawColor, frame.Size() / 2f, spriteEffects, glow);
    }
}

public record class OmegaStariteRelicRenderer(RequestCache<Texture2D> Texture) : IRelicRenderer {
    public int FrameCount { get; set; } = 5;

    public void DrawRelic(int x, int y, Vector2 drawCoordinates, Color drawColor, SpriteEffects spriteEffects, float glow) {
        var tile = Main.tile[x, y];
        var baseFrame = new Rectangle(tile.TileFrameX, 0, 48, 48);

        var texture = Texture.Value;
        var baseOrbFrame = new Rectangle(baseFrame.X, baseFrame.Y + baseFrame.Height + 2, 16, 16);
        var orbFrame = baseOrbFrame;
        var orbOrigin = orbFrame.Size() / 2f;
        float f = Main.GlobalTimeWrappedHourly % (MathHelper.TwoPi / 5f) - MathHelper.PiOver2;
        int k = 0;
        int direction = spriteEffects.HasFlag(SpriteEffects.FlipHorizontally) ? 1 : -1;
        float oscYMagnitude = 0.7f * direction;
        float oscXMagnitude = baseFrame.Width / 2f - 2f;
        float orbYOffset = 4f;
        for (; f <= MathHelper.Pi - MathHelper.PiOver2; f += MathHelper.TwoPi / 5f) {
            float wave = (float)Math.Sin(f * -direction);
            float z = (float)Math.Sin(f + MathHelper.PiOver2);
            orbFrame.Y = baseOrbFrame.Y + (int)MathHelper.Clamp(2 + z * 2.5f, 0f, FrameCount) * orbFrame.Height;
            k++;
            IRelicRenderer.Draw(Main.spriteBatch, texture, drawCoordinates + new Vector2(wave * oscXMagnitude, wave * orbFrame.Height * oscYMagnitude+ orbYOffset), orbFrame, drawColor, orbOrigin, spriteEffects, glow);
        }
        IRelicRenderer.Draw(Main.spriteBatch, texture, drawCoordinates, baseFrame, drawColor, baseFrame.Size() / 2f, spriteEffects, glow);
        for (; k < 5; f += MathHelper.TwoPi / 5f) {
            float wave = (float)Math.Sin(f * -direction);
            float z = (float)Math.Sin(f + MathHelper.PiOver2);
            orbFrame.Y = baseOrbFrame.Y + (int)MathHelper.Clamp(2 + z * 2.5f, 0f, FrameCount) * orbFrame.Height;
            k++;
            IRelicRenderer.Draw(Main.spriteBatch, texture, drawCoordinates + new Vector2(wave * oscXMagnitude, wave * orbFrame.Height * oscYMagnitude + orbYOffset), orbFrame, drawColor, orbOrigin, spriteEffects, glow);
        }

    }
}