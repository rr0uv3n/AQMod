﻿using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace Aequus.Content.Necromancy.Renderer
{
    public class RenderData
    {
        public readonly List<Projectile> Projs;
        public readonly List<NPC> NPCs;
        public readonly int Team;

        public RenderTarget2D renderTargetCache;
        public Func<Color> getDrawColor;
        public int setRenderTargetForAdoption;

        public int Index { get; internal set; }

        public RenderData(Func<Color> getColor, int team = -1)
        {
            getDrawColor = getColor;
            Team = team;
            Projs = new List<Projectile>();
            NPCs = new List<NPC>();
        }

        public bool ContainsContents()
        {
            return NPCs.Count > 0 || Projs.Count > 0;
        }

        public void DrawContents(SpriteBatch spriteBatch)
        {
            if (!ClientConfig.Instance.NecromancyOutlines)
            {
                NPCs.Clear();
                Projs.Clear();
                return;
            }
            Begin.GeneralEntities.Begin(spriteBatch);
            try
            {
                foreach (var n in NPCs)
                {
                    if (n.active)
                        Main.instance.DrawNPC(n.whoAmI, n.behindTiles);
                }
                foreach (var p in Projs)
                {
                    if (p.active)
                        Main.instance.DrawProj(p.whoAmI);
                }
            }
            catch
            {
            }
            spriteBatch.End();
            Projs.Clear();
            NPCs.Clear();
        }

        public bool CheckRenderTarget(out bool request)
        {
            request = false;
            if (EnsureTarget())
            {
                return true;
            }
            if (GhostRenderer.OrphanedRenderTargets.Count > 0)
            {
                if (renderTargetCache != null)
                {
                    GhostRenderer.OrphanedRenderTargets.Add(renderTargetCache);
                }
                renderTargetCache = GhostRenderer.OrphanedRenderTargets[0];
                GhostRenderer.OrphanedRenderTargets.RemoveAt(0);
            }
            else
            {
                request = true;
            }
            if (!EnsureTarget())
            {
                renderTargetCache = null;
                return false;
            }
            return true;
        }

        private bool EnsureTarget()
        {
            return renderTargetCache != null && !renderTargetCache.IsDisposed && !renderTargetCache.IsContentLost 
                && renderTargetCache.Width == Main.screenWidth / 2 && renderTargetCache.Height == Main.screenHeight / 2;
        }

        public void CheckSettingAdoption()
        {
            if (renderTargetCache != null)
            {
                setRenderTargetForAdoption++;
                if (setRenderTargetForAdoption > 60)
                {
                    if (GhostRenderer.OrphanedRenderTargets.Count < ColorTargetID.Count)
                    {
                        GhostRenderer.OrphanedRenderTargets.Add(renderTargetCache);
                    }
                    renderTargetCache = null;
                }
            }
        }
    }
}