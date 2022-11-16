﻿using Aequus.Biomes;
using Aequus.Biomes.Glimmer;
using Aequus.Common.ModPlayers;
using Aequus.NPCs.Friendly.Critter;
using Aequus.NPCs.Monsters.CrabCrevice;
using Aequus.NPCs.Monsters.Night;
using Aequus.NPCs.Monsters.Night.Glimmer;
using Aequus.NPCs.Monsters.Sky;
using Aequus.NPCs.Monsters.Sky.GaleStreams;
using Aequus.NPCs.Monsters.Underworld;
using Aequus.Tiles.CrabCrevice;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Aequus.NPCs.GlobalNPCs
{
    public class SpawnsManager : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (player.GetModPlayer<AequusPlayer>().ZoneDemonSiege)
            {
                spawnRate = Math.Min(spawnRate, 100);
                maxSpawns = Math.Max(maxSpawns, 8);
                return;
            }
            if (player.Zen())
            {
                spawnRate *= 10000;
                maxSpawns = 0;
                return;
            }
            if (player.ZoneSkyHeight && GaleStreamsBiome.TimeForMeteorSpawns())
            {
                spawnRate /= 2;
                maxSpawns *= 2;
            }
            if (player.GetModPlayer<AequusPlayer>().ZoneGaleStreams)
            {
                spawnRate /= 2;
            }
            if (IsClose<RedSprite>(player) || IsClose<SpaceSquid>(player))
            {
                spawnRate *= 3;
                maxSpawns = Math.Min(maxSpawns, 2);
            }
            if (player.Aequus().ZoneGlimmer)
            {
                spawnRate /= 3;
                maxSpawns = (int)(maxSpawns * 1.25f);
            }
            else if (player.Aequus().ZonePeacefulGlimmer)
            {
                spawnRate /= 2;
                maxSpawns /= 2;
            }
            if (player.Aequus().ZoneCrabCrevice)
            {
                spawnRate /= 2;
                maxSpawns = (int)(maxSpawns * 1.25f);
            }
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Aequus().ZoneDemonSiege)
            {
                pool.Clear();
                pool.Add(ModContent.NPCType<TrapperImp>(), 0.33f);
                pool.Add(ModContent.NPCType<Cindera>(), 0.33f);
                pool.Add(ModContent.NPCType<Magmabubble>(), 0.33f);
                return;
            }
            if (DontAddNewSpawns(spawnInfo))
            {
                return;
            }
            bool surface = spawnInfo.SpawnTileY < Main.worldSurface;
            if (spawnInfo.Player.ZoneSkyHeight && GaleStreamsBiome.TimeForMeteorSpawns())
            {
                AdjustSpawns(pool, 0.75f);
                if (GaleStreamsBiome.IsThisSpace(spawnInfo.SpawnTileY * 16f))
                    pool.Add(ModContent.NPCType<Meteor>(), 2f);
            }
            if (spawnInfo.Player.GetModPlayer<AequusPlayer>().ZoneGaleStreams && !spawnInfo.PlayerSafe)
            {
                AdjustSpawns(pool, MathHelper.Lerp(1f, 0.25f, SpawnCondition.Sky.Chance));
                if (Aequus.HardmodeTier && !(IsClose<RedSprite>(spawnInfo.Player) || IsClose<SpaceSquid>(spawnInfo.Player)))
                {
                    pool.Add(ModContent.NPCType<RedSprite>(), 0.06f * SpawnCondition.Sky.Chance);
                    pool.Add(ModContent.NPCType<SpaceSquid>(), 0.06f * SpawnCondition.Sky.Chance);
                }
                if (!NPC.AnyNPCs(ModContent.NPCType<Vraine>()))
                    pool.Add(ModContent.NPCType<Vraine>(), 1f * SpawnCondition.Sky.Chance);
                if (WorldGen.SolidTile(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY))
                {
                    pool.Add(ModContent.NPCType<WhiteSlime>(), 0.3f * SpawnCondition.Sky.Chance);
                }
                pool.Add(ModContent.NPCType<StreamingBalloon>(), 0.6f * SpawnCondition.Sky.Chance);
            }
            if (!Main.dayTime && surface)
            {
                if (GlimmerBiome.EventActive)
                {
                    int tiles = GlimmerSystem.CalcTiles(spawnInfo.Player);
                    if (tiles < GlimmerBiome.MaxTiles)
                    {
                        if (tiles < GlimmerBiome.SuperStariteTile)
                        {
                            pool.Clear();
                            pool.Add(ModContent.NPCType<SuperStarite>(), GlimmerBiome.SuperStariteSpawn);
                        }
                        pool.Add(ModContent.NPCType<DwarfStariteCritter>(), 2f);
                        pool.Add(ModContent.NPCType<Starite>(), GlimmerBiome.StariteSpawn);
                        int hyperStariteCount = tiles < GlimmerBiome.UltraStariteTile ? 2 : 1;
                        if (tiles < GlimmerBiome.HyperStariteTile && NPC.CountNPCS(ModContent.NPCType<HyperStarite>()) < hyperStariteCount)
                        {
                            pool.Add(ModContent.NPCType<HyperStarite>(), GlimmerBiome.HyperStariteSpawn);
                        }
                        if (tiles < GlimmerBiome.UltraStariteTile && !NPC.AnyNPCs(ModContent.NPCType<UltraStarite>()))
                        {
                            pool.Add(ModContent.NPCType<UltraStarite>(), GlimmerBiome.UltraStariteSpawn);
                        }
                        return;
                    }
                }

                if (Main.bloodMoon)
                {
                    if (!NPC.AnyNPCs(ModContent.NPCType<BloodMimic>()))
                    {
                        pool.Add(ModContent.NPCType<BloodMimic>(), 0.03f);
                    }
                }
                else
                {
                    pool.Add(ModContent.NPCType<DwarfStariteCritter>(), spawnInfo.Player.Aequus().ZonePeacefulGlimmer ? 3f : 0.01f);
                }
            }
            if (spawnInfo.Player.Aequus().ZoneCrabCrevice
                && Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].WallType == ModContent.WallType<SedimentaryRockWallWall>())
            {
                pool.Clear();
                pool.Add(NPCID.Seahorse, 0.01f);
                if (Main.hardMode)
                {
                    pool.Add(ModContent.NPCType<SummonerCrab>(), 0.2f);
                }
                pool.Add(NPCID.Crab, 1f);
                pool.Add(NPCID.SeaSnail, 0.05f);
                pool.Add(ModContent.NPCType<SoldierCrab>(), 0.5f);
                pool.Add(ModContent.NPCType<CoconutCrab>(), 0.33f);
                if (spawnInfo.Water)
                {
                    if (!NPC.AnyNPCs(ModContent.NPCType<CrabFish>()))
                        pool.Add(ModContent.NPCType<CrabFish>(), 0.4f);
                    pool.Add(NPCID.PinkJellyfish, 0.1f);
                    pool.Add(NPCID.Shark, 0.05f);
                    pool.Add(NPCID.Squid, 0.05f);
                }
            }
        }

        private static void AdjustSpawns(IDictionary<int, float> pool, float amt)
        {
            var enumerator = pool.GetEnumerator();
            while (enumerator.MoveNext())
            {
                pool[enumerator.Current.Key] *= amt;
            }
        }

        public static bool IsClose<T>(Player player) where T : ModNPC
        {
            return player.isNearNPC(ModContent.NPCType<T>(), 2000f);
        }

        public static bool DontAddNewSpawns(NPCSpawnInfo spawnInfo, bool checkPillars = true, bool checkMoonSunEvents = true, bool checkInvasion = true)
        {
            if (checkPillars && (spawnInfo.Player.ZoneTowerNebula || spawnInfo.Player.ZoneTowerSolar || spawnInfo.Player.ZoneTowerStardust || spawnInfo.Player.ZoneTowerVortex))
            {
                return true;
            }
            if (spawnInfo.Player.ZoneOverworldHeight || spawnInfo.Player.ZoneSkyHeight)
            {
                if (checkMoonSunEvents && (Main.eclipse || Main.pumpkinMoon || Main.snowMoon))
                {
                    return true;
                }
                if (checkInvasion && spawnInfo.Invasion)
                {
                    return true;
                }
            }
            return false;
        }

        public static void ForceZen(Vector2 mySpot, float zenningDistance)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active)
                {
                    Main.player[i].GetModPlayer<ZenPlayer>().forceZen = true;
                }
            }
        }
        public static void ForceZen(NPC npc)
        {
            ForceZen(npc.Center, 2000f);
        }
    }
}