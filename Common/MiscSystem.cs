﻿using Terraria.ModLoader;

namespace Aequus.Common
{
    public sealed class MiscSystem : ModSystem
    {
        public override void PostUpdatePlayers()
        {
            if (AequusHelpers.Main_invasionType.IsCaching)
            {
                AequusHelpers.Main_invasionType.EndCaching();
            }
            if (AequusHelpers.Main_eclipse.IsCaching)
            {
                AequusHelpers.Main_eclipse.EndCaching();
            }
            if (AequusHelpers.Main_dayTime.IsCaching)
            {
                AequusHelpers.Main_dayTime.EndCaching();
            }
        }
    }
}