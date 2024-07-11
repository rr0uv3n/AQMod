﻿using AequusRemake.Core.ContentGeneration;
using Terraria.Localization;

namespace AequusRemake.Content.Fishing;

internal class InstancedFishingBobber : InstancedModProjectile {
    public InstancedFishingBobber(ModItem fishingPole) : base(fishingPole.Name + "Bobber", fishingPole.Texture + "Bobber") { }

    public override LocalizedText DisplayName => Language.GetText(this.GetLocalizationKey("DisplayName"));

    public override void SetDefaults() {
        Projectile.CloneDefaults(ProjectileID.BobberWooden);
    }
}