﻿using AequusRemake.Core.Graphics;
using AequusRemake.Core.Structures.Particles;
using System;

namespace AequusRemake.Content.Items.Tools.AnglerLamp;

public class WispLanternParticles : ParticleArray<WispLanternParticles.Particle> {
    public override int ParticleCount => 50;

    public override void Draw(SpriteBatch spriteBatch) {
        spriteBatch.BeginDusts();

        Texture2D texture = AequusTextures.FlareSoft;
        Rectangle frame = texture.Frame();
        Vector2 origin = frame.Size() / 2f;

        Texture2D lensFlare = AequusTextures.LensFlare;
        Rectangle lensFlareFrame = lensFlare.Frame();
        Vector2 lensFlareOrigin = lensFlareFrame.Size() / 2f;
        lock (this) {
            for (int k = 0; k < Particles.Length; k++) {
                Particle particle = Particles[k];

                if (particle == null || !particle.Active) {
                    continue;
                }

                float animation = particle.Animation;
                Vector2 drawLocation = particle.Location - Main.screenPosition;
                Color color = particle.Color with { A = 100 };
                Color whiteColor = Color.White with { A = 0 } * Math.Min(animation, 1f);
                float rotation = particle.Rotation;
                float scale = particle.Scale * 0.8f;

                if (animation < 0.4f) {
                    float backFlareScale = MathF.Sin(animation / 0.4f * MathHelper.Pi);
                    spriteBatch.Draw(texture, drawLocation, frame, color * 0.5f, 0f, origin, new Vector2(1.5f, 0.5f) * backFlareScale, SpriteEffects.None, 0f);
                }
                if (animation < 4f) {
                    var backDrawPosition = drawLocation + Main.rand.NextVector2Square(-4f + animation, 4f - animation) * 0.5f;
                    var backScale = new Vector2(scale * (4f - animation) * 0.3f, scale);
                    spriteBatch.Draw(texture, backDrawPosition, frame, color, rotation, origin, backScale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(texture, backDrawPosition, frame, color, rotation + MathHelper.PiOver2, origin, backScale, SpriteEffects.None, 0f);
                }
                spriteBatch.Draw(texture, drawLocation, frame, whiteColor, rotation, origin, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture, drawLocation, frame, whiteColor, rotation + MathHelper.PiOver2, origin, scale, SpriteEffects.None, 0f);

                spriteBatch.Draw(lensFlare, drawLocation, lensFlareFrame, Color.White * 0.5f * scale, 0f, lensFlareOrigin, 0.66f, SpriteEffects.None, 0f);
            }
        }

        spriteBatch.End();
    }

    public override void Update() {
        for (int i = 0; i < Particles.Length; i++) {
            Particle particle = Particles[i];
            if (particle == null || !particle.Active) {
                continue;
            }
            Active = true;

            particle.Animation += 0.05f;
            particle.Rotation += 0.3f * particle.Scale;
            if (particle.Animation < 4f) {
                particle.Scale *= 0.96f;

                if (particle.NPCAnchor != -1) {
                    if (particle.NPCOffset == Vector2.Zero) {
                        particle.NPCOffset = Main.npc[particle.NPCAnchor].Center - particle.Location;
                    }
                    if (particle.Animation < 2f) {
                        particle.Location = Vector2.Lerp(particle.Location, Main.npc[particle.NPCAnchor].Center + particle.NPCOffset, 1f - particle.Animation / 2f);
                    }
                }

                var d = Dust.NewDustPerfect(particle.Location, DustID.Torch, Alpha: 150, Scale: particle.Scale * 5f);
                d.noGravity = true;
                d.velocity *= 2f;
                continue;
            }

            particle.Scale -= 0.05f;
            if (particle.Scale <= 0f) {
                particle.Active = false;
            }
        }
    }

    public override void OnActivate() {
        DrawLayers.Instance.PostDrawDust += Draw;
    }

    public override void Deactivate() {
        DrawLayers.Instance.PostDrawDust -= Draw;
    }

    public class Particle : IParticle {
        private bool _active;
        public bool Active {
            get => _active;
            set {
                if (value) {
                    Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    NPCAnchor = -1;
                    NPCOffset = Vector2.Zero;
                    Animation = 0f;
                    Opacity = 1f;
                }
                _active = value;
            }
        }

        public int NPCAnchor;
        public Vector2 NPCOffset;
        public Vector2 Location;

        public float Rotation;

        public float Scale;

        public float Opacity;

        public Color Color;

        public float Animation;
    }

    /*
    public float Animation;
    public int npc = -1;
    public Vector2 npcOffset;

    protected override void SetDefaults() {
        SetTexture(AequusRemakeTextures.Flare2);
        Rotation = Main.rand.NextFloat(-0.05f, 0.05f);
        Animation = 0f;
        npc = -1;
    }

    public override void Update(ref ParticleRendererSettings settings) {
        Animation += 0.05f;
        Rotation += 0.3f * Scale;
        if (Animation < 4f) {
            Scale *= 0.96f;

            if (npc != -1) {
                if (npcOffset == Vector2.Zero) {
                    npcOffset = Main.npc[npc].Center - Position;
                }
                if (Animation < 2f) {
                    Position = Vector2.Lerp(Position, Main.npc[npc].Center + npcOffset, 1f - Animation / 2f);
                }
            }

            var d = Dust.NewDustPerfect(Position, 156, Alpha: 150, Scale: Scale * 2f);
            d.noGravity = true;
            d.velocity *= 2f;
            return;
        }
        base.Update(ref settings);
    }

    public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch) {
        var color = GetParticleColor(ref settings);
        var drawCoordinates = Position - Main.screenPosition;
        if (Animation < 0.4f) {
            spritebatch.Draw(texture, drawCoordinates, frame, color with { A = 100 } * 0.33f, 0f, origin, new Vector2(2f, 0.5f) * MathF.Sin(Animation / 0.4f * MathHelper.Pi), SpriteEffects.None, 0f);
        }
        if (Animation < 4f) {
            var backDrawPosition = drawCoordinates + Main.rand.NextVector2Square(-4f + Animation, 4f - Animation) * 0.5f;
            var scale = new Vector2(Scale * (4f - Animation) * 0.3f, Scale);
            spritebatch.Draw(texture, backDrawPosition, frame, color with { A = 100 } * 0.5f, Rotation, origin, scale, SpriteEffects.None, 0f);
            spritebatch.Draw(texture, backDrawPosition, frame, color with { A = 100 } * 0.5f, Rotation + MathHelper.PiOver2, origin, scale, SpriteEffects.None, 0f);
        }
        spritebatch.Draw(texture, drawCoordinates, frame, Color.White with { A = 0 } * Math.Min(Animation * 2f, 1f), Rotation, origin, Scale, SpriteEffects.None, 0f);
    }
    */
}