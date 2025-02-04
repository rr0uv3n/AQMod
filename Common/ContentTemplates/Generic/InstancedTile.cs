﻿using Terraria.ModLoader;

namespace Aequus.Common.ContentTemplates.Generic;

internal class InstancedTile : ModTile {
    private readonly string _name;
    private readonly string _texture;

    public override string Name => _name;

    public override string Texture => _texture;

    public InstancedTile(string name, string texture) {
        _name = name;
        _texture = texture;
    }
}