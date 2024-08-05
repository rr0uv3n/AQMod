﻿using Aequus.Common.CodeGeneration;
using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Aequus.Common.Structures;
using System.Collections.Generic;

namespace Aequus;

public partial class AequusPlayer {
    [CompilerGenerated]
    public Item accGoldenFeather;
    [CompilerGenerated]
    public int accGoldenFeatherRespawnTimeModifier;
    [CompilerGenerated]
    public byte accGoldenFeatherTeammate;
    [CompilerGenerated]
    public bool accInfoDayCalendar;
    [CompilerGenerated]
    public bool accInfoDebuffDPS;
    [CompilerGenerated]
    public bool accInfoMoneyMonocle;
    [CompilerGenerated]
    public bool accInfoShimmerMonocle;
    [CompilerGenerated]
    public Item accWeightedHorseshoe;
    [CompilerGenerated]
    public bool showHorseshoeAnvilRope;
    [CompilerGenerated]
    public int cHorseshoeAnvil;
    
    [CompilerGenerated]
    private void ResetEffectsInner() {
        SourceGeneratorTools.ResetObj(ref accGoldenFeather);
        SourceGeneratorTools.ResetObj(ref accGoldenFeatherRespawnTimeModifier);
        SourceGeneratorTools.ResetObj(ref accGoldenFeatherTeammate);
        SourceGeneratorTools.ResetObj(ref accWeightedHorseshoe);
        SourceGeneratorTools.ResetObj(ref showHorseshoeAnvilRope);
        SourceGeneratorTools.ResetObj(ref cHorseshoeAnvil);
    }
    
    [CompilerGenerated]
    private void PostUpdateEquipsInner() {
        Content.Items.Accessories.GoldenFeather.GoldenFeather.UpdateGoldenFeather(Player, this);
        Content.Items.Accessories.WeightedHorseshoe.WeightedHorseshoe.OnPostUpdateEquips(Player, this);
    }
    
    [CompilerGenerated]
    private void ResetInfoAccessoriesInner() {
        SourceGeneratorTools.ResetObj(ref accInfoDayCalendar);
        SourceGeneratorTools.ResetObj(ref accInfoDebuffDPS);
        SourceGeneratorTools.ResetObj(ref accInfoMoneyMonocle);
        SourceGeneratorTools.ResetObj(ref accInfoShimmerMonocle);
    }
    
    [CompilerGenerated]
    private void MatchInfoAccessoriesInner(AequusPlayer other) {
        accInfoDayCalendar |= other.accInfoDayCalendar;
        accInfoDebuffDPS |= other.accInfoDebuffDPS;
        accInfoMoneyMonocle |= other.accInfoMoneyMonocle;
        accInfoShimmerMonocle |= other.accInfoShimmerMonocle;
    }
}