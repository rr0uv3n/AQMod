﻿using System.Collections.Generic;

namespace Aequus.Content.DedicatedContent.DeathsEmbrace;

public class DeathsEmbrace : ModItem, IDedicatedItem {
    public string DedicateeName => "bubbyboytoo";

    public Color TextColor => new Color(20, 70, 30);

    public override void SetStaticDefaults() {
        ItemSets.DrinkParticleColors[Type] = new Color[] { Color.Black * 0.66f, Color.DarkRed with { A = 180 } * 0.66f, };
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.HeartreachPotion);
        Item.consumable = false;
        Item.buffTime = 300;
        Item.rare = ItemRarityID.LightPurple;
        Item.maxStack = 1;
        Item.value = Item.sellPrice(gold: 1);
        Item.buffType = ModContent.BuffType<DeathsEmbraceBuff>();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        foreach (var t in tooltips) {
            if (t.Mod == "Terraria" && t.Name.StartsWith("Tooltip")) {
                t.Text = t.Text.Replace("{{", $"[c/{TCommonColor.AlphaDarken(Color.Lerp(Color.Red, Color.White, 0.5f)).Hex3()}:").Replace("}}", $"]");
            }
        }
    }
}