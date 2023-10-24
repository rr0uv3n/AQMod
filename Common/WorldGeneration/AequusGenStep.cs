﻿using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Aequus.Common.WorldGeneration;

public abstract class AequusGenStep : ModType {
    public abstract string InsertAfter { get; }

    public UnifiedRandom Random => WorldGen.genRand;

    protected sealed override void Register() {
        AequusWorldGenerationSystem.GenerationSteps.Add(this);
    }

    public virtual void EmergencyOnStepNotFound(List<GenPass> tasks) {
        tasks.Add(CreateGenerationPass());
    }

    public virtual void InsertStep(int index, List<GenPass> tasks) {
        tasks.Insert(index, CreateGenerationPass());
    }

    public abstract void Apply(GenerationProgress progress, GameConfiguration config);

    public virtual GenPass CreateGenerationPass() {
        return new PassLegacy(Name, Apply);
    }
}