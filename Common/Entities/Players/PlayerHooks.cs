﻿using Aequus.Common.Utilities.Cil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.GameContent.Achievements;

namespace Aequus.Common.Entities.Players;

internal class PlayerHooks : LoadedType {
    public static bool QuickBuff { get; private set; }

    protected override void Load() {
        On_Player.QuickBuff += On_Player_QuickBuff;
        IL_Player.PickTile += IL_Player_PickTile;
    }

    private static void On_Player_QuickBuff(On_Player.orig_QuickBuff orig, Player player) {
        QuickBuff = true;
        orig(player);
        QuickBuff = false;
    }

    private static bool _player_PickTile;
    private void IL_Player_PickTile(ILContext il) {
        ILCursor c = new ILCursor(il);

        if (!c.TryGotoNext(MoveType.After, i => i.MatchPropertySetter(typeof(AchievementsHelper), nameof(AchievementsHelper.CurrentlyMining)))) {
            Mod.Logger.Error("Could not find mining AchievementsHelper.CurrentlyMining setter in Player.PickTile."); return;
        }

        c.Emit(OpCodes.Ldarg_0); // Player
        c.Emit(OpCodes.Ldarg_1); // X
        c.Emit(OpCodes.Ldarg_2); // Y
        c.Emit(OpCodes.Ldarg_3); // Pickaxe Power
        c.EmitDelegate((Player player, int X, int Y, int PickPower) => {
            player.GetModPlayer<AequusPlayer>().OnBreakTile(X, Y);
            if (!_player_PickTile) {
                _player_PickTile = true;
                Aequus.Instance.Logger.Info("Pick tile hook success!");
            }
        });
    }
}
