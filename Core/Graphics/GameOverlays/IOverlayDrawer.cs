﻿using Microsoft.Xna.Framework.Graphics;

namespace Aequus.Core.Graphics.GameOverlays;

public interface IOverlayDrawer {
    bool Update();
    void Draw(SpriteBatch spriteBatch);
}