﻿using Aequus.Core.CodeGeneration;
using System.IO;
using System.Runtime.CompilerServices;
using Terraria.ModLoader.IO;

namespace Aequus.Core.Utilities;

internal static class SourceGeneratorExtensions {
    // TODO -- Optimize booleans?

    #region Save & Load
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SaveObj<T>(this ISGUtil self, TagCompound tag, string name, T obj) {
        if (obj?.Equals(default(T)) == true) {
            tag[name] = obj;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LoadObj<T>(this ISGUtil self, TagCompound tag, string name, ref T obj) {
        obj = default;
        if (tag.TryGet(name, out T result)) {
            obj = result;
        }
    }
    #endregion

    #region Send
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SendObj(this ISGUtil self, BinaryWriter writer, bool obj) {
        writer.Write(obj);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SendObj(this ISGUtil self, BinaryWriter writer, int obj) {
        writer.Write(obj);
    }
    #endregion

    #region Receive
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReceiveObj(this ISGUtil self, BinaryReader reader, ref bool obj) {
        obj = reader.ReadBoolean();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReceiveObj(this ISGUtil self, BinaryReader reader, ref int obj) {
        obj = reader.ReadInt32();
    }
    #endregion
}