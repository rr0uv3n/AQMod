﻿using Aequus.Common.Items.EquipmentBooster;
using System;
using System.Collections.Generic;

namespace Aequus.Items.Equipment.Accessories.CrownOfBlood;

public class CrownOfBloodPlayer : ModPlayer {
    private readonly List<DeferredEquipBoost> _equipBoostsToUpdate = new();

    public void UpdateEquipBoost(Item item) {
        _equipBoostsToUpdate.Add(new DeferredEquipBoost(item));
    }

    public override void PostUpdateEquips() {
        AequusPlayer.EquipmentModifierUpdate = true;
        try {
            foreach (DeferredEquipBoost boost in _equipBoostsToUpdate) {
                if (boost.Item.TryGetGlobalItem(out EquipBoostGlobalItem equipBoostItem)) {
                    equipBoostItem.equipEmpowerment?.ApplyModifier(boost.Item, Player, Player.GetModPlayer<AequusPlayer>());
                }
            }
        }
        catch (Exception ex) {
            Mod.Logger.Error(ex);
        }
        finally {
            AequusPlayer.EquipmentModifierUpdate = false;
        }

        _equipBoostsToUpdate.Clear();
    }
}

public readonly record struct DeferredEquipBoost(Item Item);