﻿using System;

namespace Aequus.Content.Equipment.Accessories.GoldenFeather;

public class GoldenWindBuff : ModBuff {
    public override void SetStaticDefaults() {
        Main.buffNoTimeDisplay[Type] = true;
        Main.buffNoSave[Type] = true;
        Main.persistentBuff[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex) {
        if (!player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer)) {
            return;
        }
        player.lifeRegen += GoldenWind.LifeRegenerationAmount;
        if (aequusPlayer.respawnTimeModifier > GoldenFeather.RespawnTimeAmount) {
            aequusPlayer.respawnTimeModifier = Math.Max(aequusPlayer.respawnTimeModifier - GoldenFeather.RespawnTimeAmount, GoldenFeather.RespawnTimeAmount);
        }
    }

    public override bool RightClick(int buffIndex) {
        return false;
    }
}