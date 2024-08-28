﻿using System.Collections.Generic;

namespace Aequus.Common.ContentTemplates.Armor;

public abstract class UnifiedArmorSet : ModTexturedType, IUnifiedTemplate, ILocalizedModType {
    public string LocalizationCategory => "Items.Armor";

    List<ModType> IUnifiedTemplate.ToLoad { get; init; } = [];

    public override string Texture => $"{this.NamespacePath()}/{Name.Replace("Armor", "")}";

    protected sealed override void Register() {
        this.AddAllContent();
    }

    public sealed override void SetupContent() {
        SetStaticDefaults();
    }
}