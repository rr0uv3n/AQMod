﻿using Aequus.Common.ContentTemplates;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.PollutedOcean.Ambient.Pots;

public class TrashPots2x2 : UnifiedBreakablePot {
    protected override void SetupTileObjectData() {
        base.SetupTileObjectData();
        TileObjectData.newTile.DrawYOffset = 2;
    }

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        HitSound = AequusSounds.TileBreak_TrashBag with { Pitch = -0.1f, PitchVariance = 0.06f };
    }

    protected override bool DoSpecialBiomeTorch(ref int itemID) {
        //itemID = ModContent.GetInstance<PollutedOceanBiomeUnderground>().BiomeTorchItemType;
        return true;
    }

    protected override int ChooseGlowstick(int i, int j) {
        return ItemID.Glowstick;
        //return ModContent.GetInstance<PollutedOceanBiomeUnderground>().BiomeTorchItemType;
    }
}