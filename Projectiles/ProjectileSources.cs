﻿using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Projectiles
{
    public class ProjectileSources : GlobalProjectile
    {
        public static int ParentProjectile;
        public static int ParentNPC;

        public int itemUsed = 0;
        public int ammoUsed = 0;
        public int npcOwner = -1;
        public int projectileOwnerIdentity = -1;
        public int projectileOwner = -1;

        public override bool InstancePerEntity => true;

        public bool FromItem => itemUsed > 0;
        public bool FromAmmo => ammoUsed > 0;
        public bool HasProjectileOwner => projectileOwnerIdentity > -1;
        public bool HasNPCOwner => npcOwner > -1;

        public override void Load()
        {
            ParentProjectile = -1;
            ParentNPC = -1;
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            itemUsed = -1;
            ammoUsed = -1;
            npcOwner = ParentNPC;
            projectileOwnerIdentity = ParentProjectile;
            if (!projectile.hostile && projectile.owner > -1 && projectile.owner < Main.maxPlayers)
            {
                int projOwner = Main.player[projectile.owner].Aequus().projectileIdentity;
                if (projOwner != -1)
                {
                    projectileOwnerIdentity = projOwner;
                }
            }
            if (source is EntitySource_ItemUse_WithAmmo itemUse_WithAmmo)
            {
                itemUsed = itemUse_WithAmmo.Item.netID;
                ammoUsed = itemUse_WithAmmo.AmmoItemIdUsed;
            }
            else if (source is EntitySource_ItemUse itemUse)
            {
                itemUsed = itemUse.Item.netID;
            }
            else if (source is EntitySource_Parent parent)
            {
                if (parent.Entity is NPC)
                {
                    npcOwner = parent.Entity.whoAmI;
                }
                else if (parent.Entity is Projectile parentProjectile)
                {
                    projectileOwnerIdentity = parentProjectile.identity;
                }
            }
            projectileOwner = projectileOwnerIdentity;
        }

        public override bool PreAI(Projectile projectile)
        {
            if (projectile.friendly && projectile.owner >= 0 && projectile.owner != 255)
            {
                if (projectileOwnerIdentity > 0)
                {
                    projectileOwner = AequusHelpers.FindProjectileIdentity(projectile.owner, projectileOwnerIdentity);
                    if (projectileOwner == -1)
                    {
                        projectileOwnerIdentity = -1;
                    }
                }
            }
            return true;
        }

        public static void DefaultToExplosion(Projectile projectile, int size, DamageClass damageClass, int timeLeft = 2)
        {
            projectile.width = size;
            projectile.height = size;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.DamageType = damageClass;
            projectile.aiStyle = -1;
            projectile.timeLeft = timeLeft;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = projectile.timeLeft + 1;
            projectile.penetrate = -1;
        }

        public int ProjectileOwner(Projectile projectile)
        {
            return AequusHelpers.FindProjectileIdentity(projectile.owner, projectileOwnerIdentity);
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            bitWriter.WriteBit(itemUsed > 0);
            if (itemUsed > 0)
            {
                binaryWriter.Write((ushort)itemUsed);
            }
            bitWriter.WriteBit(ammoUsed > 0);
            if (ammoUsed > 0)
            {
                binaryWriter.Write((ushort)ammoUsed);
            }
            bitWriter.WriteBit(npcOwner > -1);
            if (npcOwner > -1)
            {
                binaryWriter.Write((ushort)npcOwner);
            }
            bitWriter.WriteBit(projectileOwnerIdentity > -1);
            if (projectileOwnerIdentity > -1)
            {
                binaryWriter.Write((ushort)projectileOwnerIdentity);
            }
            bitWriter.WriteBit(projectileOwner > -1);
            if (projectileOwner > -1)
            {
                binaryWriter.Write((ushort)projectileOwner);
            }
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            if (bitReader.ReadBit())
            {
                itemUsed = binaryReader.ReadUInt16();
            }
            if (bitReader.ReadBit())
            {
                ammoUsed = binaryReader.ReadUInt16();
            }
            if (bitReader.ReadBit())
            {
                npcOwner = binaryReader.ReadUInt16();
            }
            if (bitReader.ReadBit())
            {
                projectileOwnerIdentity = binaryReader.ReadUInt16();
            }
            if (bitReader.ReadBit())
            {
                projectileOwner = binaryReader.ReadUInt16();
            }
       }
    }
}