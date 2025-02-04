﻿using Aequus.Common;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Aequus.Content.Items.Accessories.EventPrevention;

internal class EventDeactivatorHooks : LoadedType {
    protected override void Load() {
        On_NPC.SpawnNPC += SpawnNPCUndoFlags;
        On_Projectile.FishingCheck += OverrideFishingFlags;
        IL_NPC.SpawnNPC += SpawnNPCOverrideEventsPerPlayer;
        IL_Main.UpdateAudio += OverrideEventsForMusic;
        IL_Main.CalculateWaterStyle += OverrideBloodMoonWaterStyle;
    }

    #region Hooks
    void OverrideFishingFlags(On_Projectile.orig_FishingCheck orig, Projectile projectile) {
        EventDeactivatorPlayer.CheckPlayerFlagOverrides(Main.player[projectile.owner]);
        orig(projectile);
        EventDeactivatorPlayer.UndoPlayerFlagOverrides();
    }

    // This method and the IL Hook sometimes just dont work when compiling the mod from Visual Studio.
    // REally stupid, but whatever. Atleast it SHOULD work in in-game builds (which are used in releases), from my testing.
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    void SpawnNPCUndoFlags(On_NPC.orig_SpawnNPC orig) {
        try {
            orig();
        }
        catch (Exception ex) {
            Mod!.Logger.Error(ex);
        }
        finally {
            EventDeactivatorPlayer.UndoPlayerFlagOverrides();
            //Main.NewText(Main.bloodMoon);
        }
    }

    static bool _spawnNPCOverrideEventsPerPlayer;
    void SpawnNPCOverrideEventsPerPlayer(ILContext il) {
        ILCursor c = new ILCursor(il);

        if (!c.TryGotoNext(MoveType.Before, i => i.MatchLdsfld(typeof(Main), nameof(Main.slimeRain)))) {
            Mod.Logger.Error($"Could not find Main.slimeRain ldsfld code."); return;
        }

        ILLabel slimeRainCheckLabel = c.MarkLabel();

        int loc = -1;
        if (!c.TryGotoPrev(i => i.MatchLdsfld(typeof(Main), nameof(Main.player)) && i.Next.MatchLdloc(out loc)) && loc != -1) {
            Mod.Logger.Error($"Could not find Main.player ldsfld code."); return;
        }

        c.GotoLabel(slimeRainCheckLabel);

        c.Emit(OpCodes.Ldloc, loc);
        c.EmitDelegate((int player) => {
            EventDeactivatorPlayer.UndoPlayerFlagOverrides();
            EventDeactivatorPlayer.CheckPlayerFlagOverrides(Main.player[player]);
            if (!_spawnNPCOverrideEventsPerPlayer) {
                _spawnNPCOverrideEventsPerPlayer = true;
                Aequus.Instance.Logger.Info("Event deactivation hook success!");
            }
        });
    }

    void OverrideBloodMoonWaterStyle(ILContext il) {
        ILCursor c = new ILCursor(il);
        if (!c.TryGotoNext(MoveType.After, i => i.MatchLdsfld(typeof(Main), nameof(Main.bloodMoon)))) {
            Mod.Logger.Error($"Could not find {nameof(Main)}.{nameof(Main.bloodMoon)} ldsfld-branching code."); return;
        }

        c.EmitDelegate((bool bloodMoon) => bloodMoon && (!Main.LocalPlayer.TryGetModPlayer(out EventDeactivatorPlayer eventDeactivator) || !eventDeactivator.accDisableBloodMoon));
    }

    void OverrideEventsForMusic(ILContext il) {
        const string SwapMusicField = "swapMusic";
        const BindingFlags SwapMusicBindings = BindingFlags.Static | BindingFlags.NonPublic;

        ILCursor c = new ILCursor(il);

        if (!c.TryGotoNext(MoveType.After, i => i.MatchLdsfld(typeof(Main), SwapMusicField))) {
            Mod.Logger.Error($"Could not find {nameof(Main)}.{SwapMusicField} ldsfld code."); return;
        }

        c.EmitDelegate((bool swapMusic) => EventDeactivatorPlayer.CheckPlayerFlagOverrides(Main.LocalPlayer));
        c.EmitLdsfld(typeof(Main).GetField(SwapMusicField, SwapMusicBindings));

        if (!c.TryGotoNext(MoveType.After, i => i.MatchLdsfld(typeof(Main), nameof(Main.musicBox2)))) {
            Mod.Logger.Error($"Could not find {nameof(Main)}.{nameof(Main.musicBox2)} ldsfld code."); return;
        }

        c.EmitDelegate((int musicBox2) => EventDeactivatorPlayer.UndoPlayerFlagOverrides());
        c.EmitLdsfld(typeof(Main).GetField(nameof(Main.musicBox2), BindingFlags.Static | BindingFlags.Public));
    }
    #endregion
}
