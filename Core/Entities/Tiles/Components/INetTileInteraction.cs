﻿using System.IO;

namespace AequusRemake.Core.Entities.Tiles.Components;
public interface INetTileInteraction {
    public void Send(int i, int j, BinaryWriter binaryWriter) {
    }

    public void Receive(int i, int j, BinaryReader binaryReader, int sender);
}