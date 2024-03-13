﻿using Aequus.Core.Initialization;
using System;

namespace Aequus.Core.ContentGeneration;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal sealed class AutoloadBannerAttribute : AutoloadXAttribute {
    private readonly int _legacyId;

    public AutoloadBannerAttribute(int legacyId = -1) {
        _legacyId = legacyId;
    }

    internal override void Load(ModType modType) {
        if (modType is not ModNPC modNPC) {
            throw new Exception($"{nameof(AutoloadBannerAttribute)} can only be applied to ModNPCs.");
        }

        BannerLoader.RegisterBanner(modNPC, _legacyId);
    }
}
