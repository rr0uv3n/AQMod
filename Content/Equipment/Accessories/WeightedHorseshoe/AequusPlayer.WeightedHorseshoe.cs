﻿using Aequus.Content.Equipment.Accessories.WeightedHorseshoe;
using Aequus.Core.CodeGeneration;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public Item accWeightedHorseshoe;
    [ResetEffects]
    public bool showHorseshoeAnvilRope;
    [ResetEffects]
    public int cHorseshoeAnvil;

    private void WeightedHorseshoe_FallDamageAdjustments(ref MiscDamageHit hitInfo) {
        if (Player.mount.IsConsideredASlimeMount) {
            if (Player.velocity.Y >= WeightedHorseshoe.DamagingFallSpeedThreshold) {
                hitInfo.Damage *= WeightedHorseshoe.SlimeMountFallDamageMultiplier;
            }
            hitInfo.DamagingHitbox.Inflate(6, 0);
            //hitInfo.DamagingHitbox.Height *= 2;
            hitInfo.DamageClass = DamageClass.Summon;
        }
    }

    private void WeightedHorseshoe_OnHitNPCs(int amountHit, MiscDamageHit hitInfo) {
        if (amountHit == 0) {
            return;
        }
        if (Player.mount.IsConsideredASlimeMount) {
            Player.velocity.Y = -10f;
        }
    }

    private void UpdateAnvilVisual() {
        if (showHorseshoeAnvilRope && Main.myPlayer == Player.whoAmI && Player.ownedProjectileCounts[ModContent.ProjectileType<WeightedHorseshoeVisual>()] < 1) {
            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * 0.5f, ModContent.ProjectileType<WeightedHorseshoeVisual>(), 0, 0f, Player.whoAmI);
        }
    }

    private void UpdateWeightedHorseshoe() {
        UpdateAnvilVisual();
        if (accWeightedHorseshoe == null || Player.velocity.Y < WeightedHorseshoe.DamagingFallSpeedThreshold) {
            return;
        }

        MiscDamageHit hitInfo = new() {
            DamagingHitbox = Utils.CenteredRectangle(Player.Bottom, new(Player.width + 12, Player.velocity.Y * 2f)),
            Damage = WeightedHorseshoe.EnemyFallDamage,
            DamageClass = DamageClass.Melee,
            Knockback = WeightedHorseshoe.EnemyFallKnockback
        };
        WeightedHorseshoe_FallDamageAdjustments(ref hitInfo);
        int amountDamaged = Player.CollideWithNPCs(hitInfo.DamagingHitbox, Player.GetTotalDamage(hitInfo.DamageClass).ApplyTo((float)hitInfo.Damage), Player.GetTotalKnockback(hitInfo.DamageClass).ApplyTo(hitInfo.Knockback), 10, 4, hitInfo.DamageClass);
        WeightedHorseshoe_OnHitNPCs(amountDamaged, hitInfo);
    }
}