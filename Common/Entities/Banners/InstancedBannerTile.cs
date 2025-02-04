﻿using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.Drawing;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Common.Entities.Banners;

internal class InstancedBannerTile : InstancedModTile {
    internal readonly ModNPC _modNPC;
    private readonly List<ModNPC> _bannerBuffNPCs = [];

    public InstancedBannerTile(ModNPC modNPC) : base(modNPC.Name + "Banner", $"{modNPC.NamespacePath()}/Tiles/{modNPC.Name}Banner") {
        _modNPC = modNPC;
        _bannerBuffNPCs.Add(modNPC);
    }

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
        TileObjectData.newTile.Height = 3;
        TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.LavaDeath = true;
        TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom | AnchorType.Platform, TileObjectData.newTile.Width, 0);
        TileObjectData.newTile.StyleWrapLimit = 111;
        TileObjectData.addTile(Type);
        DustType = -1;
        TileID.Sets.DisableSmartCursor[Type] = true;
        AddMapEntry(new Color(13, 88, 130), Language.GetText("MapObject.Banners"));
        if (!Main.dedServ) {
            Instance<TileDrawHooks>().HangingTile[Type] = 3;
        }
    }

    public override void NearbyEffects(int i, int j, bool closer) {
        if (!closer) {
            return;
        }

        int style = Main.tile[i, j].TileFrameX / 18;
        int npcType = _modNPC.Type;
        int bannerItem = NPCLoader.GetNPC(npcType).BannerItem;
        if (ItemID.Sets.BannerStrength.IndexInRange(bannerItem) && ItemID.Sets.BannerStrength[bannerItem].Enabled) {
            foreach (var npcToBuffAgainst in _bannerBuffNPCs) {
                Main.SceneMetrics.NPCBannerBuff[npcToBuffAgainst.Type] = true;
                Main.SceneMetrics.hasBanner = true;
            }
        }
    }

    public void AddNPCBuff(ModNPC modNPC) {
        _bannerBuffNPCs.Add(modNPC);
    }

    public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) {
        if (i % 2 == 1) {
            spriteEffects = SpriteEffects.FlipHorizontally;
        }
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        if (Main.tile[i, j].TileFrameX % 18 == 0 && Main.tile[i, j].TileFrameY % 54 == 0) {
            Instance<TileDrawHooks>().AddSpecialPoint(i, j, 5);
        }
        return false;
    }
}
