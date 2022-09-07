﻿using Aequus.Biomes;
using Aequus.Biomes.DemonSiege;
using Aequus.Biomes.Glimmer;
using Aequus.Content;
using Aequus.Content.CarpenterBounties;
using Aequus.Content.DronePylons;
using Aequus.Content.Necromancy;
using Aequus.NPCs.Boss;
using Aequus.Projectiles.Summon;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus
{
    public class PacketSystem : ModSystem
    {
        private static HashSet<PacketType> logPacketType;

        public static ModPacket NewPacket => Aequus.Instance.GetPacket();

        public static List<Rectangle> TileCoatingSync { get; private set; }

        public override void Load()
        {
            logPacketType = new HashSet<PacketType>()
            {
                PacketType.CandleSouls,
                PacketType.GlimmerStatus,
                PacketType.RemoveDemonSiege,
                PacketType.ExporterQuestsCompleted,
                PacketType.SpawnOmegaStarite,
                PacketType.StartDemonSiege,
                PacketType.SyncDronePoint,
            };
            TileCoatingSync = new List<Rectangle>();
        }

        public override void OnWorldLoad()
        {
            TileCoatingSync.Clear();
        }

        public override void OnWorldUnload()
        {
            TileCoatingSync.Clear();
        }

        public override void PostUpdateEverything()
        {
            if (TileCoatingSync.Count > 0)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    TileCoatingSync.Clear();
                    return;
                }
                Send((p) =>
                {
                    AequusTileData.SendSquares(p, TileCoatingSync);
                }, PacketType.CoatingTileSquare);
            }
        }

        public override bool HijackSendData(int whoAmI, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
        {
            if (msgType == MessageID.TileSquare && (TileChangeType)number5 == TileChangeType.None)
            {
                TileCoatingSync.Add(new Rectangle(number, (int)number2, (int)number3, (int)number4));
            }
            return false;
        }

        public static void Send(PacketType type, int capacity = 256, int to = -1, int ignore = -1)
        {
            var packet = Aequus.Instance.GetPacket(capacity);
            packet.Write((byte)type);
        }

        public static void Send(Func<ModPacket, bool> func, PacketType type, int capacity = 256, int to = -1, int ignore = -1)
        {
            var packet = Aequus.Instance.GetPacket(capacity);
            packet.Write((byte)type);
            if (func(packet))
                packet.Send(to, ignore);
        }

        public static void Send(Action<ModPacket> action, PacketType type, int capacity = 256, int to = -1, int ignore = -1)
        {
            var packet = Aequus.Instance.GetPacket(capacity);
            packet.Write((byte)type);
            action(packet);
            packet.Send(to, ignore);
        }

        public static void SendSound(string name, Vector2? location = null, float? volume = null, float? pitch = null)
        {
            Send((p) =>
            {
                p.Write(name);
                LegacyFlaggedWrite(location != null, (p) => p.WriteVector2(location.Value), p);
                LegacyFlaggedWrite(volume != null, (p) => p.Write(volume.Value), p);
                LegacyFlaggedWrite(pitch != null, (p) => p.Write(pitch.Value), p);
            }, PacketType.SyncZombieRecruitSound);
        }

        public static void LegacyFlaggedWrite(bool flag, Action<ModPacket> writeAction, ModPacket p)
        {
            p.Write(flag);
            if (flag)
            {
                writeAction(p);
            }
        }

        public static void SyncNecromancyOwner(int npc, int player)
        {
            Send((p) =>
                {
                    p.Write(npc);
                    p.Write(player);
                },
                PacketType.SyncNecromancyOwner);
        }

        public static void WriteNullableItem(Item item, BinaryWriter writer, bool writeStack = false, bool writeFavorite = false)
        {
            if (item != null)
            {
                writer.Write(true);
                ItemIO.Send(item, writer, writeStack, writeFavorite);
            }
            else
            {
                writer.Write(false);
            }
        }
        public static Item ReadNullableItem(BinaryReader reader, bool readStack = false, bool readFavorite = false)
        {
            if (reader.ReadBoolean())
            {
                var item = new Item();
                ItemIO.Receive(item, reader, readStack, readFavorite);
                return item;
            }
            else
            {
                return null;
            }
        }

        public static void WriteNullableItemList(Item[] items, BinaryWriter writer, bool writeStack = false, bool writeFavorite = false)
        {
            if (items != null)
            {
                writer.Write(true);
                if (items.Length < 0 || items.Length > byte.MaxValue)
                {
                    throw new Exception("Length of item list is invalid, must not go below 0 nor be greater than 255");
                }
                writer.Write((byte)items.Length);
                for (int i = 0; i < items.Length; i++)
                {
                    WriteNullableItem(items[i], writer, writeStack, writeFavorite);
                }
            }
            else
            {
                writer.Write(false);
            }
        }
        public static Item[] ReadNullableItemList(BinaryReader reader, bool readStack = false, bool readFavorite = false)
        {
            if (reader.ReadBoolean())
            {
                var item = new Item[reader.ReadByte()];
                for (int i = 0; i < item.Length; i++)
                {
                    item[i] = ReadNullableItem(reader, readStack, readFavorite);
                }
                return item;
            }
            else
            {
                return null;
            }
        }

        public static PacketType ReadPacketType(BinaryReader reader)
        {
            return (PacketType)reader.ReadByte();
        }

        public static void SyncNPC(NPC npc)
        {
            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
        }

        public static void HandlePacket(BinaryReader reader)
        {
            var type = ReadPacketType(reader);

            var l = Aequus.Instance.Logger;
            if (logPacketType.Contains(type))
            {
                l.Debug("Recieving Packet: " + type);
            }
            switch (type)
            {
                case PacketType.CoatingTileSquare:
                    {
                        AequusTileData.ReadSquares(reader);
                    }
                    break;

                case PacketType.CarpenterBountiesCompleted:
                    {
                        Main.player[reader.ReadInt32()].GetModPlayer<CarpenterBountyPlayer>().RecieveClientChanges(reader);
                    }
                    break;

                case PacketType.RequestTileSectionFromServer:
                    {
                        int plr = reader.ReadInt32();
                        int sectionX = reader.ReadInt32();
                        int sectionY = reader.ReadInt32();
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendSection(plr, sectionX, sectionY);
                        }
                    }
                    break;

                case PacketType.SyncDronePoint:
                    {
                        int x = reader.ReadInt32();
                        int y = reader.ReadInt32();

                        DroneWorld.RecievePacket(reader, new Point(x, y));
                    }
                    break;

                case PacketType.SyncNecromancyNPC:
                    {
                        Main.npc[reader.ReadByte()].GetGlobalNPC<NecromancyNPC>().Receive(reader);
                    }
                    break;

                case PacketType.SpawnOmegaStarite:
                    NPC.SpawnBoss(reader.ReadInt32(), reader.ReadInt32() - 1600, ModContent.NPCType<OmegaStarite>(), reader.ReadInt32());
                    break;

                case PacketType.ExporterQuestsCompleted:
                    ExporterQuests.QuestsCompleted = reader.ReadUInt16();
                    break;

                case PacketType.GlimmerStatus:
                    GlimmerSystem.ReadGlimmerStatus(reader);
                    break;

                case PacketType.CandleSouls:
                    {
                        int amt = reader.ReadInt32();
                        var v = reader.ReadVector2();
                        for (int i = 0; i < amt; i++)
                        {
                            int player = reader.ReadInt32();
                            if (Main.myPlayer == player)
                                Projectile.NewProjectile(new EntitySource_Sync("PacketType.GiveoutEnemySouls"), v, Main.rand.NextVector2Unit() * 1.5f, ModContent.ProjectileType<SoulAbsorbtion>(), 0, 0f, player);
                            Main.player[player].GetModPlayer<AequusPlayer>().candleSouls++;
                        }
                    }
                    break;

                case PacketType.SyncRecyclingMachine:
                    TERecyclingMachine.NetReceive2(reader);
                    break;

                case PacketType.SyncAequusNPC:
                    {
                        byte npc = reader.ReadByte();
                        Main.npc[npc].Aequus().Receive(npc, reader);
                    }
                    break;

                case PacketType.RemoveDemonSiege:
                    DemonSiegeSystem.ActiveSacrifices.Remove(new Point(reader.ReadUInt16(), reader.ReadUInt16()));
                    break;

                case PacketType.StartDemonSiege:
                    DemonSiegeSystem.ReceiveStartRequest(reader);
                    break;

                case PacketType.DemonSiegeSacrificeStatus:
                    DemonSiegeSacrifice.ReceiveStatus(reader);
                    break;

                case PacketType.SyncZombieRecruitSound:
                    SoundEngine.PlaySound(NecromancyNPC.ZombieRecruitSound, reader.ReadVector2());
                    break;

                case PacketType.SyncAequusPlayer:
                    {
                        if (Main.player[reader.ReadByte()].TryGetModPlayer<AequusPlayer>(out var aequus))
                        {
                            aequus.RecieveChanges(reader);
                        }
                    }
                    break;

                case PacketType.SyncNecromancyOwner:
                    {
                        int npc = reader.ReadInt32();
                        Main.npc[npc].GetGlobalNPC<NecromancyNPC>().zombieOwner = reader.ReadInt32();
                    }
                    break;

                default:
                    break;
            }
        }
    }
}