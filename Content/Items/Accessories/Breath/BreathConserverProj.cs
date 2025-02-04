﻿using Aequus;
using Aequus.Common.Entities.Projectiles.AI;
using Aequus.Particles.Common;
using Aequus.Particles.Dusts;
using System;

namespace Aequus.Content.Items.Accessories.Breath;

public class BreathConserverProj : ModProjectile {
    public override string Texture => AequusTextures.Projectile(ProjectileID.VampireHeal);

    public override void SetDefaults() {
        Projectile.CloneDefaults(ProjectileID.VampireHeal);
        Projectile.ignoreWater = true;
        Projectile.aiStyle = -1;
        Projectile.hide = true;
    }

    public override void AI() {
        Player owner = Main.player[Projectile.owner];
        AequusPlayer aequus = owner.GetModPlayer<AequusPlayer>();
        Player target = Main.player[(int)Projectile.ai[0]];

        if (aequus.accBreathRestoreStacks <= 0 || target.breath >= target.breathMax) {
            Projectile.Kill();
            return;
        }

        HealAI.AI(Projectile, target, HealBreath);

        if (Main.netMode != NetmodeID.Server) {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), newColor: new Color(30, 60, 88, 30), Scale: 2f);
            d.velocity *= 0.2f;
            if (Projectile.timeLeft % 12 == 0 && Collision.WetCollision(Projectile.position, Projectile.width, Projectile.height)) {
                var bigBubble = Instance<UnderwaterBubbles>().New();
                bigBubble.Location = Projectile.Center;
                bigBubble.Frame = (byte)Main.rand.Next(1, 3);
                bigBubble.Velocity = Main.rand.NextVector2Unit() * -Projectile.velocity * 0.05f;
                bigBubble.UpLift = Main.rand.NextFloat(0.001f, 0.003f);
            }
        }
    }

    private void HealBreath(Player target) {
        Player owner = Main.player[Projectile.owner];
        AequusPlayer Aequus = owner.GetModPlayer<AequusPlayer>();

        int restoreBreath = Math.Max((int)(target.breathMax * BreathConserver.RestoreBreathMaxOnEnemyKill), 1) * Aequus.accBreathRestoreStacks;
        target.GetModPlayer<AequusPlayer>().HealBreath(restoreBreath);

        if (Main.netMode != NetmodeID.Server) {
            var bigBubble = Instance<UnderwaterBubbles>().New();
            bigBubble.Location = target.MouthPosition ?? target.Center;
            bigBubble.Frame = (byte)Main.rand.Next(5, 7);
            bigBubble.Velocity = Vector2.Zero;
            bigBubble.UpLift = 0.005f;
        }
    }
}