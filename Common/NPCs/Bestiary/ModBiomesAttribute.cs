﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aequus.Common.NPCs.Bestiary;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal class ModBiomesAttribute : Attribute {
    public readonly Type[] _modBiomeTypes;

    public ModBiomesAttribute(params Type[] modBiomeTypes) {
        _modBiomeTypes = modBiomeTypes;
    }
}

internal class ModBiomesGlobalNPC : GlobalNPC {
    public override bool AppliesToEntity(NPC npc, bool lateInstantiation) {
        return npc.ModNPC != null && npc.ModNPC.GetType().GetCustomAttributes<ModBiomesAttribute>().Any();
    }

    public override void SetDefaults(NPC npc) {
        // NOTE: Attributes are reinitialized each time they are requested.
        foreach (var attr in npc.ModNPC.GetType().GetCustomAttributes<ModBiomesAttribute>()) {
            IEnumerable<ModBiome> modBiomes = ModContent.GetContent<ModBiome>();

            int startIndex;
            if (npc.ModNPC.SpawnModBiomes == null) {
                startIndex = 0;
                npc.ModNPC.SpawnModBiomes = new int[attr._modBiomeTypes.Length];
            }
            else {
                var arr = npc.ModNPC.SpawnModBiomes;
                startIndex = arr.Length;
                Array.Resize(ref arr, arr.Length + attr._modBiomeTypes.Length);
                npc.ModNPC.SpawnModBiomes = arr;
            }

            
            int attributeIndex = 0;
            do {
                npc.ModNPC.SpawnModBiomes[startIndex] = modBiomes.Where((mb) => mb.GetType().Equals(attr._modBiomeTypes[attributeIndex])).First().Type;
                startIndex++;
                attributeIndex++;
            }
            while (startIndex < npc.ModNPC.SpawnModBiomes.Length && attributeIndex < attr._modBiomeTypes.Length);
        }
    }
}