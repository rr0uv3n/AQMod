﻿using Aequus.UI.EventProgressBars;
using System;
using Terraria;

namespace Aequus.Content.Events.GlimmerEvent
{
    public class GlimmerProgressBar : LegacyEventProgressBar
    {
        public override bool IsActive()
        {
            return Main.LocalPlayer.Aequus().ZoneGlimmer && GlimmerBiomeManager.omegaStarite == -1;
        }

        public override float GetEventProgress()
        {
            return Math.Clamp(1f - GlimmerSystem.CalcTiles(Main.LocalPlayer) / (float)GlimmerBiomeManager.MaxTiles, 0f, 1f);
        }
    }
}