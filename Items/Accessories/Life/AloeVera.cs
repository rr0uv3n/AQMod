﻿using Aequus.Common.Items.EquipmentBooster;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Life {
    public class AloeVera : ModItem {
        public static int LifeRegen = 4;
        public static float AddMultiplier = 0.5f;
        public static float DebuffResistMultiplier = 0.75f;

        public static int LifeRegenForTip => LifeRegen / 2;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(
            LifeRegenForTip,
            TextHelper.Create.MultiplierPercentDifference(AddMultiplier, TextHelper.DefaultPercent),
            TextHelper.Create.MultiplierPercentDifference(DebuffResistMultiplier, TextHelper.DefaultPercent));

        public override void SetStaticDefaults() {
            EquipBoostDatabase.Instance.SetEntry(this, new EquipBoostEntry(base.Tooltip.WithFormatArgs(
                LifeRegenForTip * 2, 
                TextHelper.Create.MultiplierPercentDifference(MathF.Pow(AddMultiplier, 2), TextHelper.DefaultPercent),
                TextHelper.Create.MultiplierPercentDifference(MathF.Pow(DebuffResistMultiplier, 2), TextHelper.DefaultPercent))));
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory(16, 16);
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(gold: 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().regenerationMultiplier += AddMultiplier;
            player.Aequus().regenerationFlat += LifeRegen;
            player.Aequus().regenerationBadMultiplier *= DebuffResistMultiplier;
        }
    }
}