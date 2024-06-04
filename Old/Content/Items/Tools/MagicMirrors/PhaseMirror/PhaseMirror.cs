﻿using System.Collections.Generic;
using tModLoaderExtended.Terraria.GameContent.Creative;

namespace Aequus.Old.Content.Items.Tools.MagicMirrors.PhaseMirror;

[FilterOverride(FilterOverride.Tools)]
public class PhaseMirror : ModItem, IPhaseMirror {
    [CloneByReference]
    public List<(int, int, Dust)> DustEffectCache { get; set; }
    public int UseAnimationMax => 64;

    public override void SetStaticDefaults() {
#if !DEBUG
        TownNPCs.PhysicistNPC.Analysis.AnalysisSystem.PhysicistPrimaryRewardItems.Add(Type);
#endif
        ItemSets.WorksInVoidBag[Type] = true;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.IceMirror);
        Item.rare = ItemRarityID.Green;
        Item.useTime = UseAnimationMax;
        Item.useAnimation = UseAnimationMax;
        DustEffectCache = new();
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame) {
        if (!player.JustDroppedAnItem) {
            IPhaseMirror.UsePhaseMirror(player, Item, this);
        }
    }

    public void Teleport(Player player, Item item, IPhaseMirror me) {
        player.Spawn(PlayerSpawnContext.RecallFromItem);
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<AequusPlayer>().infiniteWormhole = true;
    }

    public void GetPhaseMirrorDust(Player player, Item item, IPhaseMirror me, out int dustType, out Color dustColor) {
        dustType = DustID.MagicMirror;
        dustColor = Color.White;
    }
}