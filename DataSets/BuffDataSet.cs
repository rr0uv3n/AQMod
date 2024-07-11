﻿using AequusRemake.DataSets.Structures;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AequusRemake.DataSets;

public class BuffDataSet : DataSet {
    /// <summary>Buffs in this set should not have their duration altered, like the Luck Potion effect.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<BuffID>> CannotChangeDuration { get; private set; } = new();
    /// <summary>Set for compatibility with Thorium.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<BuffID>> PlayerStatusDebuff { get; private set; } = new();
    /// <summary>Set for compatibility with Thorium.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<BuffID>> PlayerDoTDebuff { get; private set; } = new();

    [JsonIgnore]
    public static Dictionary<IDEntry<BuffID>, List<IDEntry<BuffID>>> BuffConflicts { get; private set; } = new();

    public static void RegisterConflict(int buffID, int buffID2) {
        AequusRemake.OnPostSetupContent += () => {
            RegisterConflictInner(buffID, buffID2);
            RegisterConflictInner(buffID2, buffID);
        };
    }

    private static void RegisterConflictInner(int buffID, int conflictor) {
        (CollectionsMarshal.GetValueRefOrAddDefault(BuffConflicts, buffID, out _) ??= new()).Add(conflictor);
    }
}