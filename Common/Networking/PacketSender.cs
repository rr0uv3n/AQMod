﻿using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Networking
{
    public sealed class PacketSender
    {
        public static void Send(Action<ModPacket> action, PacketType type, int capacity = 256, int to = -1, int ignore = -1)
        {
            var packet = Aequus.Instance.GetPacket(capacity);
            packet.Write((byte)type);
            action(packet);
            packet.Send(to, ignore);
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
    }
}