﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.Misc
{
    public class WhiteFlag : ModItem
    {
        public static Color TextColor => new Color(222, 222, 222, 255);

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 28;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 10);
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item8;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Insert(tooltips.GetIndex("JourneyResearch"), new TooltipLine(Mod, "Activity", "(" + AequusText.GetText(Main.LocalPlayer.Aequus().ghostTombstones ? "Active" : "Inactive") + ")") { OverrideColor = TextColor });
        }

        public override bool? UseItem(Player player)
        {
            AequusWorld.whiteFlag = !AequusWorld.whiteFlag;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                AequusText.Broadcast($"WhiteFlag.{(AequusWorld.whiteFlag ? "True" : "False")}", TextColor);
            }
            return true;
        }
    }
}