﻿using Aequus.Common.Items.EquipmentBooster;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Equipment.Accessories.Informational.Calendar;

public class Calendar : ModItem {
    public override void SetStaticDefaults() {
        EquipBoostDatabase.Instance.SetNoEffect(Type);
        ItemID.Sets.WorksInVoidBag[Type] = true;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.FishermansGuide);
        Item.DefaultToPlaceableTile(ModContent.TileType<CalendarTile>());
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<AequusPlayer>().accDayCalendar = true;
    }
}