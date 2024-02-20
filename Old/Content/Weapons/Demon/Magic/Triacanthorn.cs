﻿using Aequus.Common.Items;
using Aequus.Content.Events.DemonSiege;
using Aequus.Core.Initialization;

namespace Aequus.Old.Content.Weapons.Demon.Magic;

[LegacyName("Wabbajack")]
[AutoloadGlowMask]
public class Triacanthorn : ModItem {
    public override void SetStaticDefaults() {
        Item.staff[Type] = true;
        AltarSacrifices.Register(ItemID.Vilethorn, Type);
    }

    public override void SetDefaults() {
        Item.width = 28;
        Item.height = 28;
        Item.damage = 17;
        Item.useTime = 40;
        Item.useAnimation = 40;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.UseSound = SoundID.Item83;
        Item.DamageType = DamageClass.Magic;
        Item.mana = 10;
        Item.knockBack = 1f;
        Item.shoot = ModContent.ProjectileType<TriacanthornProj>();
        Item.ArmorPenetration = 10;
        Item.shootSpeed = 16f;
        Item.noMelee = true;
        Item.rare = ItemCommons.Rarity.DemonSiegeTier1Loot;
        Item.value = ItemCommons.Price.DemonSiegeLoot;
    }

    public override Color? GetAlpha(Color lightColor) {
        return lightColor.MaxRGBA(200);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
        position += Vector2.Normalize(velocity) * 34f;
    }
}