﻿using Aequus.Items.Accessories.Passive;
using Aequus.Items.Vanity.Masks;
using Aequus.Items.Weapons.Melee;
using Aequus.NPCs.Boss.OmegaStarite;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Boss
{
    public class OmegaStariteBag : TreasureBagBase
    {
        protected override int InternalRarity => ItemRarityID.LightRed;
        protected override bool PreHardmode => true;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            this.CreateLoot(itemLoot)
                .Add<CelesteTorus>(chance: 1, stack: 1)
                .Add<OmegaStariteMask>(chance: 7, stack: 1)
                .Add<UltimateSword>(chance: 1, stack: 1)
                .Coins<OmegaStarite>();
        }
    }
}