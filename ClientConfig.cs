﻿using Aequus.Common;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Consumables.Potions;
using Aequus.Items.Misc;
using Aequus.Items.Misc.Summons;
using Microsoft.Xna.Framework;
using System;
using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace Aequus
{
    public sealed class ClientConfig : ConfigurationBase
    {
        public override bool Autoload(ref string name)
        {
            name = "ClientConfiguration";
            return base.Autoload(ref name);
        }

        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static ClientConfig Instance;

        [Header(Key + "Client.Headers.Visuals")]

        [BackgroundColor(47, 29, 140, 180)]
        [Label(Key + "Client.ScreenshakeIntensityLabel")]
        [Range(0f, 1f)]
        [DefaultValue(1f)]
        [SliderColor(120, 40, 255, 255)]
        public float ScreenshakeIntensity { get; set; }

        [BackgroundColor(47, 29, 140, 180)]
        [Label(Key + "Client.FlashIntensityLabel")]
        [Range(0f, 1f)]
        [DefaultValue(1f)]
        [SliderColor(120, 40, 255, 255)]
        public float FlashIntensity { get; set; }

        [BackgroundColor(47, 29, 140, 180)]
        [Label(Key + "Client.HighQualityLabel")]
        [DefaultValue(true)]
        public bool HighQuality { get; set; }

        [BackgroundColor(47, 29, 140, 180)]
        [Label(Key + "Client.HighQualityShadersLabel")]
        [DefaultValue(true)]
        [ReloadRequired()]
        public bool HighQualityShaders { get; set; }

        [BackgroundColor(47, 29, 140, 180)]
        [Label(Key + "Client.FlashShaderRepetitionsLabel")]
        [Tooltip(Key + "Client.FlashShaderRepetitionsTooltip")]
        [Increment(4)]
        [DefaultValue(40)]
        [Range(10, 80)]
        [Slider()]
        [SliderColor(30, 50, 120, 255)]
        public int FlashShaderRepetitions { get; set; }

        [BackgroundColor(47, 29, 140, 180)]
        [Label(Key + "Client.InfoDebugLogsLabel")]
        [Tooltip(Key + "Client.InfoDebugLogsTooltip")]
        [DefaultValue(false)]
        public bool InfoDebugLogs { get; set; }

        [BackgroundColor(47, 29, 140, 180)]
        [Label(Key + "Client.NecromancyColorLabel")]
        [Tooltip(Key + "Client.NecromancyColorTooltip")]
        [DefaultValue(typeof(Color), "100, 149, 237, 255")]
        [ColorHSLSlider()]
        public Color NecromancyColor { get; set; }

        internal static void OnModLoad(Aequus aequus)
        {
            AequusText.NewFromDict("Configuration.Client.ScreenshakeIntensity", "Label", (s) => AequusText.ItemText<Baguette>() + "  " + s);
            AequusText.NewFromDict("Configuration.Client.FlashIntensity", "Label", (s) => AequusText.ItemText<NoonPotion>() + "  " + s);
            AequusText.NewFromDict("Configuration.Client.HighQuality", "Label", (s) => AequusText.ItemText<Fluorescence>() + "  " + s);
            AequusText.NewFromDict("Configuration.Client.HighQualityShaders", "Label", (s) => AequusText.ItemText<FrozenTear>() + "  " + s);
            AequusText.NewFromDict("Configuration.Client.FlashShaderRepetitions", "Label", (s) => AequusText.ItemText<SupernovaFruit>() + "  " + s);
            AequusText.NewFromDict("Configuration.Client.InfoDebugLogs", "Label", (s) => AequusText.ItemText(ItemID.DontStarveShaderItem) + "  " + s);
        }
    }
}