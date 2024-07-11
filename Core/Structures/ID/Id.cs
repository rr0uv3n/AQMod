﻿namespace AequusRemake.Core.Structures.ID;

internal readonly record struct Id(int Type) : IProvideId {
    public int GetId() {
        return Type;
    }
}