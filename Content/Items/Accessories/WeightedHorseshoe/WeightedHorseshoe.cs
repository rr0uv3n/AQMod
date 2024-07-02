﻿using Aequus.Core;
using Aequus.Core.CodeGeneration;
using Aequus.Core.Entities.Items.Components;
using System;

namespace Aequus.Content.Items.Accessories.WeightedHorseshoe;

[Gen.AequusPlayer_ResetField<Item>("accWeightedHorseshoe")]
[Gen.AequusPlayer_ResetField<bool>("showHorseshoeAnvilRope")]
[Gen.AequusPlayer_ResetField<int>("cHorseshoeAnvil")]
public class WeightedHorseshoe : ModItem, IUpdateItemDye {
    public static float MaxFallSpeedMultiplier { get; set; } = 2f;
    public static float DamagingFallSpeedThreshold { get; set; } = 11f;
    public static float EnemyFallDamage { get; set; } = 75f;
    public static float EnemyFallKnockback { get; set; } = 10f;
    public static float SlimeMountFallDamageMultiplier { get; set; } = 2f;

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = Commons.Rare.NPCSkyMerchant;
        Item.value = Commons.Cost.NPCSkyMerchant;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        if (player.grappling[0] != -1) {
            return;
        }

        var aequusPlayer = player.GetModPlayer<AequusPlayer>();
        player.maxFallSpeed *= MaxFallSpeedMultiplier;
        aequusPlayer.accWeightedHorseshoe = Item;

        int gravityDirection = Math.Sign(player.gravDir);
        if (Math.Sign(player.velocity.Y) != gravityDirection) {
            return;
        }

        float fallSpeed = Math.Abs(player.velocity.Y);
        if (fallSpeed > DamagingFallSpeedThreshold) {
            aequusPlayer.visualAfterImages = true;
        }
    }

    private void UpdateFloorEffects(Player player, AequusPlayer aequusPlayer) {
        int gravDir = Math.Sign(player.gravDir);
        float playerHeight = gravDir == -1 ? -2f : player.height + 2f;
        var floorCoordinates = new Vector2(player.position.X + player.width / 2f, player.position.Y + playerHeight);
        var floorTileCoordinates = floorCoordinates.ToTileCoordinates();
        if (!TileHelper.ScanDown(floorTileCoordinates, 3, out floorTileCoordinates)) {
            return;
        }

        var floorTile = Framing.GetTileSafely(floorTileCoordinates);
        if (floorTile.HasUnactuatedTile && (Main.tileSolid[floorTile.TileType] || Main.tileSolidTop[floorTile.TileType]) && Helper.IsFalling(aequusPlayer.transitionVelocity, player.gravDir)) {
            float oldFallSpeed = Math.Abs(aequusPlayer.transitionVelocity.Y);
            if (oldFallSpeed > DamagingFallSpeedThreshold && player.velocity.Y < 1f) {
                float intensity = Math.Min((oldFallSpeed - DamagingFallSpeedThreshold) / 10f, 1f);
                int particleAmount = (int)Math.Max(60f * intensity, 5f);

                for (int i = 0; i < particleAmount; i++) {
                    var d = Main.dust[WorldGen.KillTile_MakeTileDust(floorTileCoordinates.X, floorTileCoordinates.Y, floorTile)];
                    d.position = floorCoordinates;
                    d.position.X += Main.rand.NextFloat(-player.width / 2f, player.width / 2f);
                    d.position.Y += Main.rand.NextFloat(-2f, 2f);
                    d.noGravity = true;
                    d.velocity *= 0.5f;
                    d.velocity.X += Main.rand.NextFloat(-i / 10f, i / 10f);
                    d.velocity /= 0.5f;
                    d.scale *= Main.rand.NextFloat(0.9f, 1.1f + (particleAmount - i) / (float)particleAmount);
                    if (Math.Sign(d.velocity.Y) == gravDir) {
                        d.velocity.Y *= 0.1f;
                    }
                }
                //Main.NewText($"emit a big gigashit volcano of intensity: {intensity}");
            }
        }
    }

    public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
        if (isSetToHidden) {
            return;
        }

        var aequusPlayer = player.GetModPlayer<AequusPlayer>();
        aequusPlayer.showHorseshoeAnvilRope = true;
        aequusPlayer.cHorseshoeAnvil = dyeItem.dye;

        UpdateFloorEffects(player, aequusPlayer);
    }

    private static void AdjustDamage(Player player, ref AequusPlayer.MiscDamageHit hitInfo) {
        if (player.mount.IsConsideredASlimeMount) {
            if (player.velocity.Y >= DamagingFallSpeedThreshold) {
                hitInfo.Damage *= SlimeMountFallDamageMultiplier;
            }
            hitInfo.DamagingHitbox.Inflate(6, 0);
            //hitInfo.DamagingHitbox.Height *= 2;
            hitInfo.DamageClass = DamageClass.Summon;
        }
    }

    private static void OnHitNPCWithHorseshoe(Player player, int amountHit, AequusPlayer.MiscDamageHit hitInfo) {
        if (amountHit == 0) {
            return;
        }
        if (player.mount.IsConsideredASlimeMount) {
            player.velocity.Y = -10f;
        }
    }

    [Gen.AequusPlayer_PostUpdateEquips]
    internal static void OnPostUpdateEquips(Player player, AequusPlayer aequusPlayer) {
        int visualProj = ModContent.ProjectileType<WeightedHorseshoeVisual>();
        if (aequusPlayer.showHorseshoeAnvilRope && Main.myPlayer == player.whoAmI && player.ownedProjectileCounts[visualProj] < 1) {
            Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, player.velocity * 0.5f, visualProj, 0, 0f, player.whoAmI);
        }

        if (aequusPlayer.accWeightedHorseshoe == null || player.velocity.Y < DamagingFallSpeedThreshold) {
            return;
        }

        AequusPlayer.MiscDamageHit hitInfo = new() {
            DamagingHitbox = Utils.CenteredRectangle(player.Bottom, new(player.width + 12, player.velocity.Y * 2f)),
            Damage = EnemyFallDamage,
            DamageClass = DamageClass.Melee,
            Knockback = EnemyFallKnockback
        };

        AdjustDamage(player, ref hitInfo);

        int amountDamaged = player.CollideWithNPCs(hitInfo.DamagingHitbox, player.GetTotalDamage(hitInfo.DamageClass).ApplyTo((float)hitInfo.Damage), player.GetTotalKnockback(hitInfo.DamageClass).ApplyTo(hitInfo.Knockback), 10, 4, hitInfo.DamageClass);
        OnHitNPCWithHorseshoe(player, amountDamaged, hitInfo);
    }
}