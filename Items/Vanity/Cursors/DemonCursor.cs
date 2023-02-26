﻿using Aequus.Content.CursorDyes;
using Terraria.ModLoader;

namespace Aequus.Items.Vanity.Cursors
{
    [LegacyName("DemonicCursorDye")]
    public class DemonCursor : CursorDyeBase
    {
        public override ICursorDye InitalizeDye()
        {
            return new TextureChangeCursor($"{Texture}/DemonCursor");
        }
    }
}