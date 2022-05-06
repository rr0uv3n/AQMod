﻿using Aequus.Common.Configuration;
using Aequus.Common.Utilities;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    public sealed partial class EffectsSystem : ModSystem
    {
        public static ModEffects Effects { get; private set; }

        public static MiniRandom EffectRand { get; private set; }

        public static DrawIndexCache NPCsBehindAllNPCs { get; private set; }
        public static ParticleRenderer BehindProjs { get; private set; }
        public static DrawIndexCache ProjsBehindTiles { get; private set; }

        public static ScreenShake Shake { get; private set; }

        public override void Load()
        {
            Effects = new ModEffects();
            NPCsBehindAllNPCs = new DrawIndexCache();
            ProjsBehindTiles = new DrawIndexCache();
            Shake = new ScreenShake();
            EffectRand = new MiniRandom("Split".GetHashCode(), capacity: 256 * 4);
            BehindProjs = new ParticleRenderer();
            LoadHooks();
        }
        private void LoadHooks()
        {
            On.Terraria.Main.DrawProjectiles += Hook_OnDrawProjs;
            On.Terraria.Main.DrawNPCs += Hook_OnDrawNPCs;
        }

        public override void Unload()
        {
            BehindProjs = null;
            Effects = null;
            Shake = null;
            NPCsBehindAllNPCs = null;
            ProjsBehindTiles = null;
        }

        public override void OnWorldLoad()
        {
            BehindProjs = new ParticleRenderer();
        }

        public override void PreUpdatePlayers()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                FrozenNPCEffect.ResetCounts();

                Shake.Update();
                BehindProjs.Update();
            }
        }

        internal static void UpdateScreenPosition()
        {
            Main.screenPosition += Shake.GetScreenOffset() * ClientConfig.Instance.ScreenshakeIntensity;
        }
    }
}