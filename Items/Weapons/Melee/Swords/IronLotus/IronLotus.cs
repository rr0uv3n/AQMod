﻿using Aequus.Common.Items;
using Aequus.Common.Items.Dedications;

namespace Aequus.Items.Weapons.Melee.Swords.IronLotus;

public class IronLotus : ModItem {
    public override void Load() {
        DedicationRegistry.Register(this, new DefaultDedication("Blossom", new Color(110, 60, 30, 255)));
    }

    public override void SetStaticDefaults() {
        ItemID.Sets.Spears[Type] = true;
    }

    public override void SetDefaults() {
        Item.DefaultToAequusSword<IronLotusProj>(24);
        Item.SetWeaponValues(210, 2f, 16);
        Item.width = 20;
        Item.height = 20;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.sellPrice(gold: 10);
        Item.autoReuse = true;
    }

    public override bool? UseItem(Player player) {
        Item.FixSwing(player);
        return null;
    }

    public override bool MeleePrefix() {
        return true;
    }
}