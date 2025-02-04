﻿using Terraria.Audio;
using static Aequus.Common.Buffs.BuffHooks;

namespace Aequus.Content.Items.Weapons.Classless.StunGun;

public class StunGunDebuff : ModBuff, IOnAddBuff/*, IAddRecipeGroups*/ {
    public override string Texture => AequusTextures.TemporaryDebuffIcon;

    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
        BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.Slow);
    }

    #region On Add Buff
    void IOnAddBuff.PostAddBuff(NPC npc, int duration, bool quiet) {
        if (npc.HasBuff<StunGunDebuff>()) {
            SoundEngine.PlaySound(AequusSounds.InflictStunned with { Volume = 0.3f, Pitch = 0.175f, PitchVariance = 0.05f });
        }
    }
    #endregion

    private static void EmitParticles(Entity entity, int[] buffTime, ref int buffIndex) {
        var dustSpotFront = entity.Center + StunGun.GetVisualOffset(entity.width, StunGun.GetVisualTime(StunGun.VisualTimer, front: true), entity.whoAmI);
        var dustSpotBack = entity.Center + StunGun.GetVisualOffset(entity.width, StunGun.GetVisualTime(StunGun.VisualTimer, front: false), entity.whoAmI);
        float dustScale = StunGun.GetVisualScale(entity.Size.Length());
        int dustSize = (int)(5 * dustScale);

        if (buffTime[buffIndex] <= 1) {
            for (int i = 0; i < 2; i++) {
                var d = Dust.NewDustPerfect(dustSpotFront + Main.rand.NextVector2Square(-dustSize, dustSize), DustID.Electric);
                d.velocity *= 2f;
                d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.2f);
                d.noGravity = true;

                d = Dust.NewDustPerfect(dustSpotBack + Main.rand.NextVector2Square(-dustSize, dustSize), DustID.Electric);
                d.velocity *= 0.5f;
                d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.2f);
                d.noGravity = true;
            }
        }
        if (Main.GameUpdateCount % 15 == 0) {
            var d = Dust.NewDustPerfect(dustSpotFront + Main.rand.NextVector2Square(-dustSize, dustSize), DustID.Electric, Scale: 0.6f * dustScale);
            d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.2f);
            d.noGravity = true;

            d = Dust.NewDustPerfect(dustSpotBack + Main.rand.NextVector2Square(-dustSize, dustSize), DustID.Electric, Scale: 0.6f * dustScale);
            d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.2f);
            d.noGravity = true;
        }
    }

    public override void Update(NPC npc, ref int buffIndex) {
        if (!npc.TryGetGlobalNPC(out StunGunNPC stunNPC)) {
            return;
        }

        stunNPC.SetStunState(npc);
        //npc.netOffset.X = Main.rand.NextFloat(-2f, 2f);

        EmitParticles(npc, npc.buffTime, ref buffIndex);
    }

    public override void Update(Player player, ref int buffIndex) {
        player.frozen = true;

        EmitParticles(player, player.buffTime, ref buffIndex);
    }
}