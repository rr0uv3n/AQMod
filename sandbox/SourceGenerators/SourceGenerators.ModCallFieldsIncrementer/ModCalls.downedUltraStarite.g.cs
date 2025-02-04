﻿namespace Aequus;

public partial class ModCallHandler {
    [System.Runtime.CompilerServices.CompilerGenerated]
    static bool downedUltraStarite(object[] args) {
        if (args.Length > 1) {
            if (args[1] is Mod mod) {
                LogInfo($"Mod ({mod.Name}) can remove the legacy Mod parameter. As it is no longer needed.");
            }

            if (args[^1] is bool value) {
                global::Aequus.AequusWorld.downedUltraStarite = value;
            }
            else {
                LogError($"Mod Call Parameter index ^1 (\"value\") did not match Type \"bool\".");
            }
        }

        return global::Aequus.AequusWorld.downedUltraStarite;
    }

    [System.Runtime.CompilerServices.CompilerGenerated]
    static System.Func<bool> downedUltraStariteGetter(object[] args) {            
        return () => global::Aequus.AequusWorld.downedUltraStarite;
    }

    [System.Runtime.CompilerServices.CompilerGenerated]
    static System.Action<bool> downedUltraStariteSetter(object[] args) {            
        return value => global::Aequus.AequusWorld.downedUltraStarite = value;
    }
}