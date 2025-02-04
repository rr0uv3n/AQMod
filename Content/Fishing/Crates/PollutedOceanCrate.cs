﻿#if POLLUTED_OCEAN
using Aequus.Common.Entities.Items;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.ObjectData;

namespace Aequus.Content.Fishing.Crates;

internal class PollutedOceanCrate : UnifiedModCrate {
    public override void Load() {
        base.Load();
        RandomItemFrames.FrameCount[PreHardmodeVariant.Type] = 7;
        RandomItemFrames.FrameCount[HardmodeVariant.Type] = 14;
    }

    protected override void OnSetStaticDefaults() {
        TileObjectData.newTile.StyleWrapLimit = 7;
        TileObjectData.newTile.RandomStyleRange = 7;
        TileObjectData.newTile.StyleMultiplier = 7;
        TileObjectData.newTile.StyleHorizontal = true;

        TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
        TileObjectData.newSubTile.RandomStyleRange = 14;
        TileObjectData.addSubTile(1);
    }

    public override IEnumerable<Item> GetItemDrops(int i, int j) {
        Tile tile = Main.tile[i, j];
        if (tile.TileFrameY >= 38) {
            yield return new Item(HardmodeVariant.Type);
        }
        else {
            yield return new Item(PreHardmodeVariant.Type);
        }
    }

    public override IEnumerable<IItemDropRule> ModifyCrateLoot(ModItem crate, ItemLoot loot) {
        foreach (IItemDropRule rule in base.ModifyCrateLoot(crate, loot)) {
            yield return rule;
        }

        yield return ItemDropRule.NotScalingWithLuck(Instance<Tiles.PollutedOcean.PolymerSands.PolymerSand>().Item.Type, 3, 20, 50);
    }
}
#endif