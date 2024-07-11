﻿using System.Reflection;

namespace AequusRemake.Core.Assets;

public abstract class AssetManager<T> : ILoad where T : class {
    public void Load(Mod mod) {
        OnLoad(mod);
    }

    protected virtual void OnLoad(Mod mod) {
    }

    public void Unload() {
        OnUnload();
        foreach (var f in GetType().GetFields(BindingFlags.Public | BindingFlags.Static)) {
            if (f.FieldType.Equals(typeof(RequestCache<T>))) {
                (f.GetValue(this) as RequestCache<T>)?.Unload();
            }
        }
    }

    protected virtual void OnUnload() {
    }
}