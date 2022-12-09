﻿using Aequus.Items;
using Aequus.Items.Tools.Misc;
using Terraria;

namespace Aequus.Content.CarpenterBounties
{
    public class WeirdRoofBounty : PirateShipBounty
    {
        public override bool IsBountyAvailable()
        {
            return false;
        }

        public override Item ProvideBountyRewardItem()
        {
            return AequusItem.SetDefaults<WhiteFlag>(); // Circle Block Tool
        }
    }
}