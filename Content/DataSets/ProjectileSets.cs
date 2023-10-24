﻿using Aequus.Core.DataSets;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.DataSets;

[DataID(typeof(ProjectileID))]
public class ProjectileSets : DataSet {
    public ProjectileSets() : base() {
    }

    public static readonly Dictionary<int, float> SpriteRotation = new();
    public static readonly HashSet<int> IsStar = new();

    /// <summary>
    /// Projectiles in this set deal 'heat' damage. This damage can be resisted using the Frost Potion.
    /// </summary>
    [JsonProperty]
    public static DataIDValueSet DealsHeatDamage;

    [JsonProperty]
    public static DataIDValueSet PushableByTypeId;
    [DataID(typeof(ProjAIStyleID))]
    [JsonProperty]
    public static DataIDValueSet PushableByAI;

    public override void PostSetupContent() {
        for (int i = 0; i < ProjectileLoader.ProjectileCount; i++) {
            if (ProjectileID.Search.TryGetName(i, out string name)) {
                if (name.Contains('/')) {
                    name = name.Split('/')[^1];
                }
                if (name.Contains("Star") && !name.Contains("Start")) {
                    IsStar.Add(i);
                }
            }
        }
    }

    public static float GetSpriteRotation(int projectileId) {
        if (SpriteRotation.TryGetValue(projectileId, out var rotation)) {
            return rotation;
        }

        if (Main.dedServ) {
            return 0f;
        }

        Main.instance.LoadProjectile(projectileId);
        var texture = TextureAssets.Projectile[projectileId].Value;
        int height = texture.Height / Main.projFrames[projectileId];
        // Projectile sprites which are tall are assumed to be facing upwards.
        if (texture.Width < height) {
            return MathHelper.PiOver2;
        }
        return 0f;
    }
}