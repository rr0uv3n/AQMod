﻿using Aequus.Common.Projectiles;
using Aequus.Core.CrossMod;
using System;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Localization;

namespace Aequus.Core.Utilities;

public static class ExtendEntity {
    public static bool CanReflectAgainstShimmer(Entity entity) {
        if (entity.shimmerWet && entity.velocity.Y > 0f) {
            entity.velocity.Y = -entity.velocity.Y;
            return true;
        }
        return false;
    }
}

public static class ExtendPlayer {
    private static readonly Item[] _dummyInventory = ExtendArray.CreateArray(i => new Item(), Main.InventorySlotsTotal);

    public static Point GetSpawn(this Player player) => new Point(GetSpawnX(player), GetSpawnY(player));
    public static int GetSpawnY(this Player player) => player.SpawnY > 0 ? player.SpawnY : Main.spawnTileY;
    public static int GetSpawnX(this Player player) => player.SpawnX > 0 ? player.SpawnX : Main.spawnTileX;

    /// <param name="player">The player.</param>
    /// <param name="pickaxe">The pickaxe.</param>
    /// <param name="x">Tile X coordinate.</param>
    /// <param name="y">Tile Y coordinate.</param>
    /// <returns>Whether or not the player has enough pickaxe power to break this tile.</returns>
    public static bool CheckPickPower(this Player player, Item pickaxe, int x, int y) {
        var inv = player.inventory;
        _dummyInventory[0] = pickaxe;
        player.inventory = _dummyInventory;
        bool value = false;
        try {
            value = player.HasEnoughPickPowerToHurtTile(x, y);
        }
        catch {
        }
        player.inventory = inv;
        return value;
    }

    /// <summary>
    /// Gives the player an item without dropping it onto the floor. (Unless they cannot pick it up)
    /// <para>Automatically syncs the item in multiplayer.</para>
    /// <para>Note: May only work if <see cref="Main.myPlayer"/> is the <paramref name="player"/>.</para>
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="item">The Item.</param>
    /// <param name="source">Item source.</param>
    /// <param name="getItemSettings">The Get Item settings.</param>
    public static void GiveItem(this Player player, Item item, IEntitySource source, GetItemSettings getItemSettings) {
        item.Center = player.Center;
        item = player.GetItem(player.whoAmI, item, getItemSettings);

        if (item != null && !item.IsAir) {
            int newItemIndex = Item.NewItem(source, player.getRect(), item);
            Main.item[newItemIndex].newAndShiny = false;
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                NetMessage.SendData(MessageID.SyncItem, number: newItemIndex, number2: 1f);
            }
        }
    }
    /// <summary>
    /// Gives the player an item without dropping it onto the floor. (Unless they cannot pick it up)
    /// <para>Automatically syncs the item in multiplayer.</para>
    /// <para>Note: May only work if <see cref="Main.myPlayer"/> is the <paramref name="player"/>.</para>
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="type">The Item Id.</param>
    /// <param name="stack">The Item's stack.</param>
    /// <param name="prefix">The Item's prefix.</param>
    /// <param name="source">Item source.</param>
    /// <param name="getItemSettings">The Get Item settings.</param>
    public static void GiveItem(this Player player, int type, IEntitySource source, GetItemSettings getItemSettings, int stack = 1, int prefix = 0) {
        player.GiveItem(new Item(type, stack, prefix), source, getItemSettings);
    }

    public static Chest GetCurrentChest(this Player player, bool ignoreVoidBag = false) {
        if (player.chest > -1) {
            return Main.chest[player.chest];
        }
        else if (player.chest == -2) {
            return player.bank;
        }
        else if (player.chest == -3) {
            return player.bank2;
        }
        else if (player.chest == -4) {
            return player.bank3;
        }
        else if (!ignoreVoidBag && player.chest == -5) {
            return player.bank4;
        }
        return null;
    }

    public static Item HeldItemFixed(this Player player) {
        if (Main.myPlayer == player.whoAmI && player.selectedItem == 58 && Main.mouseItem != null && !Main.mouseItem.IsAir) {
            return Main.mouseItem;
        }
        return player.HeldItem;
    }

    /// <summary>
    /// Gets the "Player Focus". This is by default the player's centre, but when using the Drone, this returns the drone's position.
    /// <para>This position is used to make Radon Fog disappear when approched by the player, or by their controlled drone.</para>
    /// <para>This position also ignores camera panning effects, like screen shakes, scoping, ect.</para>
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static Vector2 GetPlayerFocusPosition(this Player player) {
        if (Main.DroneCameraTracker.TryTracking(out var dronePosition)) {
            return dronePosition;
        }

        return player.Center;
    }

    public static bool IsFalling(this Player player) {
        return Helper.IsFalling(player.velocity, player.gravDir);
    }

    /// <summary>
    /// Spawns Flask and other "Enchantment" dusts, like the Magma Stone flames.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="velocity"></param>
    /// <param name="player"></param>
    /// <param name="showMagmaStone"></param>
    /// <returns>A potential dust instance, if any spawn. If none spawn, the result will be null.</returns>
    public static Dust SpawnEnchantmentDusts(Vector2 position, Vector2 velocity, Player player, bool showMagmaStone = true) {
        if (player.magmaStone && showMagmaStone && Main.rand.NextBool(3)) {
            var d = Dust.NewDustPerfect(position, DustID.Torch, velocity * 2f, Alpha: 100, Scale: 2.5f);
            d.noGravity = true;
            return d;
        }
        switch (player.meleeEnchant) {
            case 1: {
                    if (Main.rand.NextBool(3)) {
                        var d = Dust.NewDustPerfect(position, DustID.Venom, velocity * 2f, Alpha: 100);
                        d.noGravity = true;
                        d.fadeIn = 1.5f;
                        d.velocity *= 0.25f;
                        return d;
                    }
                }
                break;

            case 2: {
                    if (Main.rand.NextBool(2)) {
                        var d = Dust.NewDustPerfect(position, DustID.CursedTorch, new Vector2(velocity.X * 0.2f * player.direction * 3f, velocity.Y * 0.2f), Alpha: 100, Scale: 2.5f);
                        d.noGravity = true;
                        d.velocity *= 0.7f;
                        d.velocity.Y -= 0.5f;
                        return d;
                    }
                }
                break;

            case 3: {
                    if (Main.rand.NextBool(2)) {
                        var d = Dust.NewDustPerfect(position, DustID.Torch, new Vector2(velocity.X * 0.2f * player.direction * 3f, velocity.Y * 0.2f), Alpha: 100, Scale: 2.5f);
                        d.noGravity = true;
                        d.velocity *= 0.7f;
                        d.velocity.Y -= 0.5f;
                        return d;
                    }
                }
                break;

            case 4: {
                    if (Main.rand.NextBool(2)) {
                        var d = Dust.NewDustPerfect(position, DustID.Enchanted_Gold, new Vector2(velocity.X * 0.2f * player.direction * 3f, velocity.Y * 0.2f), Alpha: 100, Scale: 2.5f);
                        d.noGravity = true;
                        d.velocity *= 0.7f;
                        d.velocity.Y -= 0.5f;
                        return d;
                    }
                }
                break;

            case 5: {
                    if (Main.rand.NextBool(2)) {
                        var d = Dust.NewDustPerfect(position, DustID.IchorTorch, velocity, Alpha: 100, Scale: 2.5f);
                        d.velocity.X += player.direction;
                        d.velocity.Y -= 0.2f;
                        return d;
                    }
                }
                break;

            case 6: {
                    if (Main.rand.NextBool(2)) {
                        var d = Dust.NewDustPerfect(position, DustID.IceTorch, velocity, Alpha: 100, Scale: 2.5f);
                        d.velocity.X += player.direction;
                        d.velocity.Y -= 0.2f;
                        return d;
                    }
                }
                break;

            case 7: {
                    if (Main.rand.NextBool(40)) {
                        var g = Gore.NewGorePerfect(player.GetSource_ItemUse(player.HeldItem), position, velocity, Main.rand.Next(276, 283));
                        g.velocity.X *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                        g.velocity.Y *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                        g.scale *= 1f + Main.rand.Next(-20, 21) * 0.01f;
                        g.velocity.X += Main.rand.Next(-50, 51) * 0.05f;
                        g.velocity.Y += Main.rand.Next(-50, 51) * 0.05f;
                    }
                    else if (Main.rand.NextBool(20)) {
                        var d = Dust.NewDustPerfect(position, Main.rand.Next(139, 143), velocity, Scale: 1.2f);
                        d.velocity.X *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                        d.velocity.Y *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                        d.velocity.X += Main.rand.Next(-50, 51) * 0.05f;
                        d.velocity.Y += Main.rand.Next(-50, 51) * 0.05f;
                        d.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                        return d;
                    }
                }
                break;

            case 8: {
                    if (Main.rand.NextBool(3)) {
                        var d = Dust.NewDustPerfect(position, DustID.Poisoned, velocity * 2f, Alpha: 100);
                        d.noGravity = true;
                        d.fadeIn = 1.5f;
                        d.velocity *= 0.25f;
                        return d;
                    }
                }
                break;
        }
        return null;
    }

    public static IEntitySource GetSource_HeldItem(this Player player) {
        return player.GetSource_ItemUse(player.HeldItemFixed());
    }

    public static Item FindItemInInvOrVoidBag(this Player player, Predicate<Item> search, out bool inVoidBag) {
        var i = Array.Find(player.inventory, search);
        inVoidBag = false;
        if (i != null) {
            return i;
        }

        inVoidBag = true;
        return player.IsVoidVaultEnabled ? Array.Find(player.bank4.item, search) : null;
    }

    #region Biomes
    public static bool ZoneGlimmer(this Player player) {
        return false;
    }
    public static bool ZoneDemonSiege(this Player player) {
        return false;
    }
    public static bool ZoneGaleStreams(this Player player) {
        return false;
    }
    #endregion

    #region Damage & Crit
    /// <summary>
    /// Checks weird damage restrictions for NPCs. This literally only checks for Fairies as of right now.
    /// </summary>
    /// <param name="npc"></param>
    /// <returns>Whether the enemy should be immune to this hit (true = Do not hit)</returns>
    public static bool WeirdNPCHitRestrictions(NPC npc) {
        return npc.aiStyle == NPCAIStyleID.Fairy && !(npc.ai[2] <= 1f);
    }

    public static bool RollCrit(this Player player, Item item) {
        return !item.DamageType.UseStandardCritCalcs ? false : Main.rand.Next(100) < player.GetWeaponCrit(item);
    }
    public static bool RollCrit<T>(this Player player) where T : DamageClass {
        return player.RollCrit(ModContent.GetInstance<T>());
    }
    public static bool RollCrit(this Player player, DamageClass damageClass) {
        return !damageClass.UseStandardCritCalcs ? false : Main.rand.Next(100) < player.GetTotalCritChance(damageClass);
    }
    #endregion

    #region Player Draw Set
    public static void Draw(this ref PlayerDrawSet drawInfo, Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, int shader, float inactiveLayerDepth = 0f) {
        drawInfo.DrawDataCache.Add(new DrawData(texture, position, sourceRect, color, rotation, origin, scale, effect, inactiveLayerDepth) with { shader = shader });
    }
    public static void Draw(this ref PlayerDrawSet drawInfo, Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effect, int shader, float inactiveLayerDepth = 0f) {
        drawInfo.DrawDataCache.Add(new DrawData(texture, position, sourceRect, color, rotation, origin, scale, effect, inactiveLayerDepth) with { shader = shader });
    }
    public static void Draw(this ref PlayerDrawSet drawInfo, Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, float inactiveLayerDepth = 0f) {
        drawInfo.DrawDataCache.Add(new DrawData(texture, position, sourceRect, color, rotation, origin, scale, effect, inactiveLayerDepth));
    }
    public static void Draw(this ref PlayerDrawSet drawInfo, Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effect, float inactiveLayerDepth = 0f) {
        drawInfo.DrawDataCache.Add(new DrawData(texture, position, sourceRect, color, rotation, origin, scale, effect, inactiveLayerDepth));
    }
    public static void Draw(this ref PlayerDrawSet drawInfo, DrawData drawData) {
        drawInfo.DrawDataCache.Add(drawData);
    }
    #endregion
}

public static partial class ExtendNPC {
    /// <summary>Helper method which reflects an NPC against shimmer.</summary>
    public static void UpdateShimmerReflection(this NPC projectile) {
        if (ExtendEntity.CanReflectAgainstShimmer(projectile)) {
            projectile.netUpdate = true;
        }
    }

    public static void Kill(this NPC npc) {
        npc.StrikeInstantKill();
    }

    public static void KillEffects(this NPC npc, bool quiet = false) {
        npc.life = Math.Min(npc.life, -1);
        npc.HitEffect();
        npc.active = false;
        if (Main.netMode != NetmodeID.SinglePlayer && !quiet) {
            NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, 9999 + npc.lifeMax * 2 + npc.defense * 2);
        }
    }

    /// <summary>Manually sends a <see cref="MessageID.SyncNPC"/> packet to sync this NPC.</summary>
    public static void SyncNPC(NPC npc) {
        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
    }

    /// <summary>Runs <see cref="NPC.TargetClosest(bool)"/>, and returns <see cref="NPC.HasValidTarget"/>.</summary>
    public static bool TryRetargeting(this NPC npc, bool faceTarget = true) {
        npc.TargetClosest(faceTarget: faceTarget);
        return npc.HasValidTarget;
    }

    /// <returns>Whether or not this is most likely a critter, dependant on multiple checks of their stats and values in data sets.</returns>
    public static bool IsProbablyACritter(this NPC npc) {
        return NPCSets.CountsAsCritter[npc.type] || (npc.lifeMax < 5 && npc.lifeMax != 1 && npc.damage <= 0);
    }

    #region Drawing
    /// <summary>Replicates the drawing of misc vanilla status effects. This is used to support these effects on NPCs with custom rendering.</summary>
    public static void DrawNPCStatusEffects(SpriteBatch spriteBatch, NPC npc, Vector2 screenPos) {
        var halfSize = npc.frame.Size() / 2f;
        if (npc.confused) {
            spriteBatch.Draw(TextureAssets.Confuse.Value, new Vector2(npc.position.X - screenPos.X + npc.width / 2 - TextureAssets.Npc[npc.type].Width() * npc.scale / 2f + halfSize.X * npc.scale, npc.position.Y - screenPos.Y + npc.height - TextureAssets.Npc[npc.type].Height() * npc.scale / Main.npcFrameCount[npc.type] + 4f + halfSize.Y * npc.scale + Main.NPCAddHeight(npc) - TextureAssets.Confuse.Height() - 20f), (Rectangle?)new Rectangle(0, 0, TextureAssets.Confuse.Width(), TextureAssets.Confuse.Height()), npc.GetShimmerColor(new Color(250, 250, 250, 70)), npc.velocity.X * -0.05f, TextureAssets.Confuse.Size() / 2f, Main.essScale + 0.2f, SpriteEffects.None, 0f);
        }
    }
    #endregion

    #region Buffs
    /// <summary>Clears a buff of the specified ID from this NPC.</summary>
    /// <param name="npc"></param>
    /// <param name="buffId">The Buff Id to clear from this NPC.</param>
    public static bool ClearBuff(this NPC npc, int buffId) {
        int index = npc.FindBuffIndex(buffId);
        if (index != -1) {
            npc.DelBuff(buffId);
            return true;
        }
        return false;
    }
    #endregion
}

public static class ExtendShop {
    /// <summary>Finds the next null index in an item array. Items which are "Air" are empty slots which were added to not contain any items inside them.</summary>
    /// <param name="itemList">The shop's item list.</param>
    public static int FindNextIndex(Item[] itemList) {
        for (int i = 0; i < itemList.Length; i++) {
            if (itemList[i] == null) {
                return i;
            }
        }

        return itemList.Length;
    }

    /// <summary>Adds an item with a custom value. (<paramref name="customValue"/>)</summary>
    /// <param name="shop"></param>
    /// <param name="itemType">The Item Id to add.</param>
    /// <param name="customValue">The custom value to assign.</param>
    /// <param name="conditions">Conditions for this item.</param>
    public static NPCShop AddCustomValue(this NPCShop shop, int itemType, int customValue, params Condition[] conditions) {
        var item = new Item(itemType) {
            shopCustomPrice = customValue
        };
        return shop.Add(item, conditions);
    }
    /// <summary><inheritdoc cref="AddCustomValue(NPCShop, int, int, Condition[])"/></summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="shop"></param>
    /// <param name="customValue">The custom value to assign.</param>
    /// <param name="conditions">Conditions for this item.</param>
    public static NPCShop AddCustomValue<T>(this NPCShop shop, int customValue, params Condition[] conditions) where T : ModItem {
        return shop.AddCustomValue(ModContent.ItemType<T>(), customValue, conditions);
    }
    /// <summary><inheritdoc cref="AddCustomValue(NPCShop, int, int, Condition[])"/></summary>
    /// <param name="itemType">The Item Id to add.</param>
    /// <param name="shop"></param>
    /// <param name="customValue">The custom value to assign.</param>
    /// <param name="conditions">Conditions for this item.</param>
    public static NPCShop AddCustomValue(this NPCShop shop, int itemType, double customValue, params Condition[] conditions) {
        return shop.AddCustomValue(itemType, (int)Math.Round(customValue), conditions);
    }
    /// <summary><inheritdoc cref="AddCustomValue(NPCShop, int, int, Condition[])"/></summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="shop"></param>
    /// <param name="customValue">The custom value to assign.</param>
    /// <param name="conditions">Conditions for this item.</param>
    public static NPCShop AddCustomValue<T>(this NPCShop shop, double customValue, params Condition[] conditions) where T : ModItem {
        return shop.AddCustomValue<T>((int)Math.Round(customValue), conditions);
    }

    /// <summary>Attempts to add an item from another mod, if it exists and that mod is loaded.</summary>
    /// <typeparam name="T">The Mod.</typeparam>
    /// <param name="shop"></param>
    /// <param name="itemName">The internal name of the item.</param>
    /// <param name="conditions">Conditions for this item.</param>
    internal static NPCShop AddCrossMod<T>(this NPCShop shop, string itemName, params Condition[] conditions) where T : SupportedMod<T> {
        if (!SupportedMod<T>.TryFind<ModItem>(itemName, out var modItem)) {
            return shop;
        }
        return shop.Add(modItem.Type, conditions);
    }
}

public static class ExtendProjectile {
    internal static readonly Projectile _dummyProjectile = new Projectile();

    public static bool IsChildOrNoSpecialEffects(this Projectile projectile) {
        return projectile.GetGlobalProjectile<ProjectileItemData>().NoSpecialEffects || projectile.GetGlobalProjectile<ProjectileSource>().isProjectileChild;
    }

    public static void SetDefaultNoInteractions(this Projectile projectile) {
        projectile.tileCollide = false;
        projectile.ignoreWater = true;
        projectile.aiStyle = -1;
        projectile.penetrate = -1;
    }

    public static void SetDefaultHeldProj(this Projectile projectile) {
        projectile.SetDefaultNoInteractions();
    }

    public static float CappedMeleeScale(this Player player) {
        var item = player.HeldItem;
        return Math.Clamp(player.GetAdjustedItemScale(item), 0.5f * item.scale, 2f * item.scale);
    }

    public static void MeleeScale(Projectile proj) {
        float scale = Main.player[proj.owner].CappedMeleeScale();
        if (scale != 1f) {
            proj.scale *= scale;
            proj.width = (int)(proj.width * proj.scale);
            proj.height = (int)(proj.height * proj.scale);
        }
    }

    /// <summary>
    /// Attempts to find a projectile index using the identity and owner provided. Returns true if the projectile output is not -1.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="identity"></param>
    /// <param name="projectile"></param>
    /// <returns></returns>
    public static bool TryFindProjectileIdentity(int owner, int identity, out int projectile) {
        projectile = FindProjectileIdentity(owner, identity);
        return projectile != -1;
    }

    /// <summary>
    /// Attempts to find a projectile index using the identity and owner provided. Returns -1 otherwise.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="identity"></param>
    /// <returns></returns>
    public static int FindProjectileIdentity(int owner, int identity) {
        for (int i = 0; i < 1000; i++) {
            if (Main.projectile[i].owner == owner && Main.projectile[i].identity == identity && Main.projectile[i].active) {
                return i;
            }
        }
        return -1;
    }
    public static int FindProjectileIdentityOtherwiseFindPotentialSlot(int owner, int identity) {
        int projectile = FindProjectileIdentity(owner, identity);
        if (projectile == -1) {
            for (int i = 0; i < 1000; i++) {
                if (!Main.projectile[i].active) {
                    projectile = i;
                    break;
                }
            }
        }
        if (projectile == 1000) {
            projectile = Projectile.FindOldestProjectile();
        }
        return projectile;
    }

    /// <summary>Helper method which reflects a projectile against shimmer.</summary>
    public static void UpdateShimmerReflection(this Projectile projectile) {
        if (ExtendEntity.CanReflectAgainstShimmer(projectile)) {
            projectile.netUpdate = true;
        }
    }


    #region Drawing
    public static Rectangle Frame(this Projectile projectile) {
        return TextureAssets.Projectile[projectile.type].Value.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
    }

    public static void GetDrawInfo(this Projectile projectile, out Texture2D texture, out Vector2 offset, out Rectangle frame, out Vector2 origin, out int trailLength) {
        texture = TextureAssets.Projectile[projectile.type].Value;
        offset = projectile.Size / 2f;
        frame = projectile.Frame();
        origin = frame.Size() / 2f;
        trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
    }
    #endregion
}