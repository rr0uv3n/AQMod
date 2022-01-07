﻿using AQMod.Common.DeveloperTools;
using AQMod.Effects;
using AQMod.Effects.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace AQMod.Assets
{
    public static class AQTextures
    {
        public const string None = "Assets/empty";
        public const string Error = "Assets/error";

        public static Texture2D[] Particles { get; private set; }
        public static Texture2D[] Lights { get; private set; }
        public static Texture2D[] Trails { get; private set; }

        public static Texture2D Pixel { get; private set; }

        internal static void Load()
        {
            Pixel = ModContent.GetTexture("AQMod/Assets/Pixel");
            LoadDictionaries();
        }

        private static void LoadDictionaries()
        {
            aqdebug.SupressLogAccess();
            Particles = FillArray<ParticleTex>("Particles/Particle");
            Lights = FillArray<LightTex>("Lights/Light");
            Trails = FillArray<TrailTex>("Trails/Trail");
            aqdebug.RepairLogAccess();
        }

        private static Texture2D[] FillArray<T>(string pathWithoutNumbers) where T : class
        {
            int count = IdentityAttribute.GetCount<T>();
            var t = new Texture2D[count];
            for (int i = 0; i < count; i++)
            {
                t[i] = AQMod.LoggableTexture("AQMod/Assets/" + pathWithoutNumbers + "_" + i);
            }
            return t;
        }

        internal static void Unload()
        {
            Pixel = null;
            Particles = null;
            Lights = null;
            Trails = null;
        }
    }
}