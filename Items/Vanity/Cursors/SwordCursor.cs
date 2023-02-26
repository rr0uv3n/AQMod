﻿using Aequus.Content.CursorDyes;
using Terraria.ModLoader;

namespace Aequus.Items.Vanity.Cursors
{
    [LegacyName("SwordCursorDye")]
    public class SwordCursor : CursorDyeBase
    {
        public override ICursorDye InitalizeDye()
        {
            return new TextureChangeCursor($"{Texture}/SwordCursor");
        }
    }
}