﻿using Aequus.Common.Projectiles;
using Terraria.Localization;

namespace Aequus.Content.Fishing.FishingPoles;

internal class InstancedFishingBobber : InstancedModProjectile {
    private InstancedFishingPole _pole;

    public InstancedFishingBobber(string name) : base(name + "FishingBobber", typeof(InstancedFishingBobber).NamespaceFilePath() + $"/{name}FishingBobber") {
    }

    public override LocalizedText DisplayName => Language.GetText(this.GetLocalizationKey("DisplayName"));

    public override void SetDefaults() {
        Projectile.CloneDefaults(ProjectileID.BobberWooden);
    }
}