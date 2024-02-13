﻿using Aequus.Common.NPCs;
using Aequus.Common.NPCs.Bestiary;
using Aequus.Common.NPCs.Components;
using Aequus.Content.Biomes.PollutedOcean;
using Aequus.Content.DataSets;
using Aequus.Content.Equipment.Accessories.ScavengerBag;
using Aequus.Content.Tiles.Banners;
using Aequus.Content.Tiles.Statues;
using Aequus.Core.ContentGeneration;
using Aequus.Core.Initialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Aequus.Content.Enemies.PollutedOcean.Scavenger;

[AutoloadBanner]
[AutoloadStatue]
[ModBiomes(typeof(PollutedOceanBiome))]
public partial class Scavenger : AIFighterLegacy, IPreDropItems, IPostPopulateItemDropDatabase {
    public const int SLOT_HEAD = 0;
    public const int SLOT_BODY = 1;
    public const int SLOT_LEGS = 2;
    public const int SLOT_ACCS = 3;
    public const int ARMOR_COUNT = 4;

    public static int ExtraEquipChance { get; set; } = 2;
    public static int ItemDropChance { get; set; } = 4;
    public static int TravelingMerchantBuilderItemChance { get; set; } = 20;

    private int serverWhoAmI = Main.maxPlayers;
    private Player playerDummy;
    public Item[] armor;
    public Item weapon;
    public int attackAnimation;

    public override float SpeedCap => base.SpeedCap + runSpeedCap;

    public override float Acceleration => base.Acceleration + acceleration;

    #region Initialization
    public Scavenger() {
        weapon = new();
        armor = new Item[ARMOR_COUNT];
        for (int i = 0; i < armor.Length; i++) {
            armor[i] = new();
        }
    }

    public override void SetStaticDefaults() {
        SetupAccessoryUsages();
        SetupDrawLookups();
        Main.npcFrameCount[Type] = 20;
        NPCSets.NPCBestiaryDrawOffset[Type] = new() {
            Velocity = -1f,
            Scale = 1f,
        };
        NPCSets.StatueSpawnedDropRarity[Type] = 0.05f;
        NPCMetadata.PushableByTypeId.Add(Type);
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ScavengerBag>(), ScavengerLootBag.BackpackDropRate));
        npcLoot.Add(ItemDropRule.Common(ItemID.PaintSprayer, TravelingMerchantBuilderItemChance));
        npcLoot.Add(ItemDropRule.Common(ItemID.PortableCementMixer, TravelingMerchantBuilderItemChance));
        npcLoot.Add(ItemDropRule.Common(ItemID.ExtendoGrip, TravelingMerchantBuilderItemChance));
        npcLoot.Add(ItemDropRule.Common(ItemID.BrickLayer, TravelingMerchantBuilderItemChance));
    }

    public virtual void PostPopulateItemDropDatabase(Aequus aequus, ItemDropDatabase database) {
        ExtendLoot.InheritDropRules(NPCID.Skeleton, Type, database);
    }

    public override void SetDefaults() {
        NPC.width = 18;
        NPC.height = 40;
        NPC.aiStyle = 3;
        NPC.damage = 20;
        NPC.defense = 8;
        NPC.lifeMax = 60;
        NPC.HitSound = SoundID.NPCHit2;
        NPC.DeathSound = SoundID.NPCDeath2;
        NPC.knockBackResist = 0.5f;
        NPC.value = Item.silver;
        AnimationType = NPCID.Skeleton;
        NPC.aiStyle = -1;
    }
    #endregion

    private void PassDownStatsToPlayer() {
        if (Main.netMode == NetmodeID.Server && serverWhoAmI != Main.myPlayer) {
            serverWhoAmI = Main.myPlayer;
            NPC.netUpdate = true;
        }

        playerDummy.statLife = NPC.life;
        playerDummy.statLifeMax = NPC.lifeMax;
        playerDummy.statLifeMax2 = NPC.lifeMax;
        playerDummy.statDefense = Player.DefenseStat.Default + NPC.defense;
        playerDummy.armor[0] = armor[SLOT_HEAD];
        playerDummy.armor[1] = armor[SLOT_BODY];
        playerDummy.armor[2] = armor[SLOT_LEGS];
        playerDummy.armor[3] = armor[SLOT_ACCS];
        playerDummy.selectedItem = 0;
        playerDummy.inventory[0] = weapon;
        playerDummy.whoAmI = serverWhoAmI;
    }

    private void InitPlayer() {
        playerDummy = new();
        PassDownStatsToPlayer();
    }

    private bool SetItem(ref Item item, int itemId, int stack = 1) {
        if (itemId <= 0) {
            item.TurnToAir();
            return false;
        }
        item.SetDefaults(itemId);
        item.stack = stack;
        return true;
    }

    private bool SetItem(ref Item item, List<int> itemList, UnifiedRandom random) {
        return SetItem(ref item, Main.rand.Next(itemList));
    }

    private void RandomizeArmor(UnifiedRandom random) {
        SetItem(ref weapon, ScavengerEquipment.ScavengerWeapons.Select((i) => i.Id).ToList(), random);
        var options = new List<int>() { SLOT_HEAD, SLOT_BODY, SLOT_LEGS, SLOT_ACCS };
        while (options.Count > 0) {
            int choice = random.Next(options);

            bool value = choice switch {
                SLOT_HEAD => SetItem(ref armor[SLOT_HEAD], ScavengerEquipment.ScavengerHelmets.Select((i) => i.Id).ToList(), random),
                SLOT_BODY => SetItem(ref armor[SLOT_BODY], ScavengerEquipment.ScavengerBreastplates.Select((i) => i.Id).ToList(), random),
                SLOT_LEGS => SetItem(ref armor[SLOT_LEGS], ScavengerEquipment.ScavengerLeggings.Select((i) => i.Id).ToList(), random),
                SLOT_ACCS => SetItem(ref armor[SLOT_ACCS], ScavengerEquipment.ScavengerAccessories.Select((i) => i.Id).ToList(), random),
                _ => false
            };

            if (!random.NextBool(ExtraEquipChance)) {
                break;
            }

            if (value) {
                options.Remove(choice);
            }
        }
    }

    public override void OnSpawn(IEntitySource source) {
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        var source = NPC.GetSource_FromThis();

        if (NPC.life <= 0) {
            for (int i = 0; i < 20; i++) {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Bone, 2.5f * hit.HitDirection, -2.5f);
            }

            NPC.NewGore(AequusTextures.ScavengerGoreHead, NPC.position, NPC.velocity, Scale: NPC.scale);
            for (int i = 0; i < 2; i++) {
                Gore.NewGore(source, NPC.position + new Vector2(0f, 20f), NPC.velocity, 43, NPC.scale);
                Gore.NewGore(source, NPC.position + new Vector2(0f, 34f), NPC.velocity, 44, NPC.scale);
            }
        }
        else {
            for (int i = 0; i < hit.Damage / (double)NPC.lifeMax * 50f; i++) {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Bone, hit.HitDirection, -1f);
            }
        }
    }

    private bool DoAttack() {
        float attackDistance = 200f;
        var target = Main.player[NPC.target];
        if (NPC.velocity.Y == 0f && NPC.Distance(target.Center) < attackDistance && Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height)) {
            NPC.velocity.X *= 0.9f;
            NPC.TargetClosest(faceTarget: true);

            attackAnimation++;
            if (attackAnimation > 60) {
                attackAnimation = 0;
                if (weapon.shoot > ProjectileID.None && Main.netMode != NetmodeID.MultiplayerClient) {
                    int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(target.Center) * weapon.shootSpeed, weapon.shoot, weapon.damage, weapon.knockBack, Main.myPlayer);
                    Main.projectile[p].friendly = false;
                    Main.projectile[p].hostile = true;
                    Main.projectile[p].noDropItem = true;
                }
                if (weapon.UseSound != null) {
                    SoundEngine.PlaySound(weapon.UseSound, NPC.Center);
                }
            }
            return true;
        }
        return false;
    }

    public override void AI() {
        if (playerDummy == null) {
            RandomizeArmor(Main.rand);
            InitPlayer();
        }
        playerDummy.Bottom = NPC.Bottom;
        acceleration = -0.08f;
        runSpeedCap = 0f;
        playerDummy.ResetEffects();
        NPC.defense = NPC.defDefense;
        for (int i = 0; i < armor.Length; i++) {
            NPC.defense += armor[i].defense;
        }
        PassDownStatsToPlayer();
        if (!armor[SLOT_ACCS].IsAir) {
            playerDummy.ApplyEquipFunctional(armor[SLOT_ACCS], hideVisual: false);
        }
        if (NPC.TryGetGlobalNPC<AequusNPC>(out var aequusNPC)) {
            acceleration += playerDummy.runAcceleration;
            aequusNPC.statSpeedX += playerDummy.moveSpeed - 1f;
            runSpeedCap += playerDummy.accRunSpeed / 1.5f;
            if (NPC.velocity.Y < 0f) {
                if (playerDummy.jumpBoost) {
                    aequusNPC.statSpeedY += 0.5f;
                }
                aequusNPC.statSpeedY += playerDummy.jumpSpeedBoost / 4f;
            }
        }

        //bool attacking = DoAttack();
        bool attacking = false;
        if (!armor[SLOT_ACCS].IsAir && CustomAccessoryUsage.TryGetValue(armor[SLOT_ACCS].type, out var accessoryUpdate)) {
            accessoryUpdate(this, attacking);
        }

        if (attacking) {
            return;
        }

        base.AI();
    }

    public override void FindFrame(int frameHeight) {
        base.FindFrame(frameHeight);
    }

    public override void OnKill() {
        //TryDroppingItem(weapon, Main.rand);
        //for (int i = 0; i < armor.Length; i++) {
        //    TryDroppingItem(armor[i], Main.rand);
        //}

        var dropsRegisterList = new List<Item>();
        for (int i = 0; i < armor.Length; i++) {
            if (armor[i] != null && !armor[i].IsAir && Main.rand.NextBool(ItemDropChance)) {
                dropsRegisterList.Add(armor[i].Clone());
            }
        }

        ScavengerLootBag.AddDropsToList(NPC, dropsRegisterList);

        dropsRegisterList.RemoveAll((i) => i.ModItem is ScavengerBag);

        if (dropsRegisterList.Count <= 0) {
            return;
        }

        int bag = NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ScavengerLootBag>());
        if (bag == Main.maxNPCs || Main.npc[bag].ModNPC is not ScavengerLootBag lootBagNPC) {
            return;
        }

        foreach (var drop in dropsRegisterList) {
            drop.Prefix(-1);
        }
        lootBagNPC.drops = dropsRegisterList.ToArray();
        Main.npc[bag].velocity.X += Main.rand.NextFloat(-3f, 3f);
        Main.npc[bag].velocity.Y = -4f;
        Main.npc[bag].netUpdate = true;

        //void TryDroppingItem(Item item, UnifiedRandom random) {
        //    if (item != null && !item.IsAir && random.NextBool(ItemDropChance)) {
        //        Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), item.type, item.stack, prefixGiven: -1);
        //    }
        //}
    }

    #region IO
    public override void SaveData(TagCompound tag) {
        TrySaveItem("Head", armor[SLOT_HEAD]);
        TrySaveItem("Body", armor[SLOT_BODY]);
        TrySaveItem("Legs", armor[SLOT_LEGS]);
        TrySaveItem("Acc", armor[SLOT_ACCS]);
        TrySaveItem("Weapon", weapon);

        void TrySaveItem(string name, Item item) {
            if (item != null && !item.IsAir) {
                tag[name] = item;
            }
        }
    }

    public override void LoadData(TagCompound tag) {
        TryLoadItem("Head", ref armor[SLOT_HEAD]);
        TryLoadItem("Body", ref armor[SLOT_BODY]);
        TryLoadItem("Legs", ref armor[SLOT_LEGS]);
        TryLoadItem("Acc", ref armor[SLOT_ACCS]);
        TryLoadItem("Weapon", ref weapon);

        void TryLoadItem(string name, ref Item item) {
            if (tag.TryGet<Item>(name, out var loadedItem)) {
                item = loadedItem;
            }
        }
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(serverWhoAmI);
        writer.Write(attackAnimation);
        writer.Write(accessoryUseData);
        writer.Write(weapon.type);
        writer.Write(weapon.stack);
        for (int i = 0; i < armor.Length; i++) {
            writer.Write(armor[i].type);
            writer.Write(armor[i].stack);
        }
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        serverWhoAmI = reader.ReadInt32();
        attackAnimation = reader.ReadInt32();
        accessoryUseData = reader.ReadSingle();
        SetItem(ref weapon, reader.ReadInt32(), reader.ReadInt32());
        for (int i = 0; i < armor.Length; i++) {
            SetItem(ref armor[i], reader.ReadInt32(), reader.ReadInt32());
        }
    }
    #endregion

    public bool PreDropItems(Player closestPlayer) {
        return false;
    }
}