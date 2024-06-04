﻿using Aequus.Common;
using Aequus.Content.Events.DemonSiege;

namespace Aequus.Old.Content.Items.Weapons.Melee.DemonCrimsonSpear;

public class Cauterizer : ModItem {
    public override void SetStaticDefaults() {
        AltarSacrifices.Register(ItemID.BloodButcherer, Type);
    }

    public override void SetDefaults() {
        Item.LazyCustomSwordDefaults<CauterizerProj>(38);
        Item.SetWeaponValues(49, 4.5f, 6);
        Item.width = 24;
        Item.height = 24;
        Item.scale = 1f;
        Item.autoReuse = true;
        Item.rare = Commons.Rare.EventDemonSiege;
        Item.value = Commons.Cost.EventDemonSiege;
    }

    public override Color? GetAlpha(Color lightColor) {
        return lightColor.MaxRGBA(200);
    }

    public override bool? UseItem(Player player) {
        //Item.FixSwing(player);
        return null;
    }

    public override bool MeleePrefix() {
        return true;
    }
}