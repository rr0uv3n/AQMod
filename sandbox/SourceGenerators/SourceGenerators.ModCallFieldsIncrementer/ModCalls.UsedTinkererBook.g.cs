﻿namespace Aequus;

public partial class ModCallHandler {
    [System.Runtime.CompilerServices.CompilerGenerated]
    static bool UsedTinkererBook(object[] args) {
        if (args.Length > 1) {
            if (args[1] is Mod mod) {
                LogInfo($"Mod ({mod.Name}) can remove the legacy Mod parameter. As it is no longer needed.");
            }

            if (args[^1] is bool value) {
                global::Aequus.AequusWorld.UsedTinkererBook = value;
            }
            else {
                LogError($"Mod Call Parameter index ^1 (\"value\") did not match Type \"bool\".");
            }
        }

        return global::Aequus.AequusWorld.UsedTinkererBook;
    }

    [System.Runtime.CompilerServices.CompilerGenerated]
    static System.Func<bool> UsedTinkererBookGetter(object[] args) {            
        return () => global::Aequus.AequusWorld.UsedTinkererBook;
    }

    [System.Runtime.CompilerServices.CompilerGenerated]
    static System.Action<bool> UsedTinkererBookSetter(object[] args) {            
        return value => global::Aequus.AequusWorld.UsedTinkererBook = value;
    }
}