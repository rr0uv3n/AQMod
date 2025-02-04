﻿namespace Aequus.Common.Tiles;

public interface IOverridePlacement {
    void OverridePlacementCheck(Player player, Tile targetTile, Item item, ref int tileToCreate, ref int previewPlaceStyle, ref bool? overrideCanPlace, ref int? forcedRandom);
}
