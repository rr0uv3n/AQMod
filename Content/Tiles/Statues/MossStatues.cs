﻿using AequusRemake.Core.ContentGeneration;
using tModLoaderExtended.Terraria.GameContent.Creative;

namespace AequusRemake.Content.Tiles.Statues;

public class MossStatues : StatueTileTemplate {
    public const int STYLE_ARGON = 0;
    public const int STYLE_KRYPTON = 1;
    public const int STYLE_NEON = 2;
    public const int STYLE_XENON = 3;

    public override void Load() {
        AddItem(STYLE_ARGON, "Argon");
        AddItem(STYLE_KRYPTON, "Krypton");
        AddItem(STYLE_NEON, "Neon");
        AddItem(STYLE_XENON, "Xenon");

        void AddItem(int style, string name) => Mod.AddContent(new InstancedTileItem(this, style, name, value: Item.sellPrice(copper: 60), journeyOverride: new JourneySortByTileId(TileID.Statues)));
    }
}