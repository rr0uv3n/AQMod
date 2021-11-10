﻿using AQMod.Assets.DrawCode;
using AQMod.Common.Utilities;
using AQMod.NPCs.Boss.Crabson;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Common.SceneLayers
{
    public class JerryCrabsonLayer : SceneLayer
    {
        protected override void Draw()
        {
            int crabsonType = ModContent.NPCType<JerryCrabson>();
            var chain = ModContent.GetTexture(AQUtils.GetPath<JerryClaw>("_Chain"));
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active)
                {
                    if (Main.npc[i].type == crabsonType)
                    {
                        var crabsonPosition = Main.npc[i].Center;
                        DrawMethods.DrawJerryChain(chain, new Vector2(crabsonPosition.X - 24f, crabsonPosition.Y), Main.npc[(int)Main.npc[i].localAI[0]].Center);
                        DrawMethods.DrawJerryChain(chain, new Vector2(crabsonPosition.X + 24f, crabsonPosition.Y), Main.npc[(int)Main.npc[i].localAI[1]].Center);
                    }
                }
            }
        }
    }
}