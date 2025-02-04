﻿using Aequus.Common.CodeGeneration;
using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Aequus.Common.Structures;
using System.Collections.Generic;

namespace Aequus;

public partial class AequusItem {
    
    [CompilerGenerated]
    private void ModifyTooltipsInner(Item item, List<TooltipLine> tooltips) {
        Content.Items.Materials.Ores.Aluminum.AddAluminumMiningTip(item, tooltips);
    }
}