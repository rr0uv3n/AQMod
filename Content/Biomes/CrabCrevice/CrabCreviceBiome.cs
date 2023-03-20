﻿using Aequus.Common.ItemDrops;
using Aequus.Content.Biomes.CrabCrevice.Background;
using Aequus.Content.Biomes.CrabCrevice.Water;
using Aequus.Items.Accessories.Offense;
using Aequus.Items.Accessories.Utility;
using Aequus.Items.Placeable.Furniture.CraftingStation;
using Aequus.Items.Weapons.Ranged;
using Aequus.NPCs.Monsters.CrabCrevice;
using Aequus.Tiles.CrabCrevice;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes.CrabCrevice
{
    public class CrabCreviceBiome : ModBiome
    {
        public static ConfiguredMusicData music { get; private set; }

        public override string BestiaryIcon => "Aequus/Assets/UI/BestiaryIcons/CrabCrevice";

        public override string BackgroundPath => Aequus.VanillaTexture + "MapBG11";
        public override string MapBackground => BackgroundPath;

        public override ModWaterStyle WaterStyle => ModContent.GetInstance<CrabCreviceWater>();
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<CrabCreviceSurfaceBackground>();
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<CrabCreviceUGBackground>();

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override int Music => music.GetID();

        public override void Load()
        {
            if (!Main.dedServ)
            {
                music = new ConfiguredMusicData(MusicID.OceanNight, MusicID.OtherworldlyOcean);
            }
        }

        public override void SetStaticDefaults()
        {
            SetChestLoot();
        }

        public override void Unload()
        {
            UnloadChestLoot();
            music = null;
        }

        public override bool IsBiomeActive(Player player)
        {
            if (SedimentaryRockTile.BiomeCount > 150)
                return true;

            var loc = player.Center.ToTileCoordinates();
            return WorldGen.InWorld(loc.X, loc.Y, 10) && Main.tile[loc].WallType == ModContent.WallType<SedimentaryRockWallWall>();
        }

        internal static bool SpawnCrabCreviceEnemies(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {

            if (!spawnInfo.Player.Aequus().ZoneCrabCrevice
                || Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].WallType != ModContent.WallType<SedimentaryRockWallWall>()) {
                return false;
            }

            pool.Clear();
            pool.Add(NPCID.Seahorse, 0.01f);
            if (Aequus.HardmodeTier) {
                pool[ModContent.NPCType<SummonerCrab>()] = 0.2f;
            }
            pool.Add(NPCID.Crab, 1f);
            pool.Add(NPCID.SeaSnail, 0.05f);
            pool.Add(ModContent.NPCType<SoldierCrab>(), 0.5f);
            pool.Add(ModContent.NPCType<CoconutCrab>(), 0.33f);
            if (spawnInfo.Water) {
                if (!NPC.AnyNPCs(ModContent.NPCType<CrabFish>()))
                    pool.Add(ModContent.NPCType<CrabFish>(), 0.4f);
                pool.Add(NPCID.PinkJellyfish, 0.1f);
                pool.Add(NPCID.Shark, 0.05f);
                pool.Add(NPCID.Squid, 0.05f);
            }
            return true;
        }

        #region Chest Contents
        /// <summary>
        /// The primary loot pool for Crab Crevice chests
        /// </summary>
        public static ItemDrop[] ChestPrimaryLoot;
        /// <summary>
        /// The secondary loot pool for Crab Crevice chests
        /// </summary>
        public static ItemDrop[] ChestSecondaryLoot;
        /// <summary>
        /// The tertiary (3rd) loot pool for Crab Crevice chests
        /// </summary>
        public static ItemDrop[] ChestTertiaryLoot;

        public void SetChestLoot()
        {
            ChestPrimaryLoot = new ItemDrop[]
            {
                ModContent.ItemType<StarPhish>(),
                ModContent.ItemType<DavyJonesAnchor>(),
                ModContent.ItemType<ArmFloaties>(),
            };

            ChestSecondaryLoot = new ItemDrop[]
            {
                ItemID.Trident,
                ItemID.FloatingTube,
                ItemID.Flipper,
                ItemID.WaterWalkingBoots,
                ItemID.BreathingReed,
            };

            ChestTertiaryLoot = new ItemDrop[]
            {
                ModContent.ItemType<RecyclingMachine>(),
                ItemID.DivingHelmet,
                ItemID.BeachBall,
                ItemID.JellyfishNecklace,
            };
        }

        public void UnloadChestLoot()
        {
            ChestTertiaryLoot = null;
            ChestSecondaryLoot = null;
            ChestPrimaryLoot = null;
        }
        #endregion
    }
}