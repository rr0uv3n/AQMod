﻿using Aequus.Items;
using Aequus.Items.Accessories.Combat.Passive;
using Aequus.Items.Weapons.Magic;
using Aequus.Items.Weapons.Ranged;
using Aequus.Items.Weapons.Summon.Minion;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.BossMonsters.OmegaStarite.Rewards {
    public class OmegaStariteBag : TreasureBagBase {
        protected override int InternalRarity => ItemRarityID.LightRed;
        protected override bool PreHardmode => true;

        public override void ModifyItemLoot(ItemLoot itemLoot) {
            this.CreateLoot(itemLoot)
                .Add<CelesteTorus>(chance: 1, stack: 1)
                .Add<OmegaStariteMask>(chance: 7, stack: 1)
                .AddOptions(chance: 1, ModContent.ItemType<Raygun>(), ModContent.ItemType<Gamestar>(), ModContent.ItemType<ScribbleNotebook>())
                .Coins<OmegaStarite>();
        }
    }
}