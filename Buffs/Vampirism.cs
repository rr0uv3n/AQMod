﻿using Aequus.Common.Players;
using Terraria;

namespace Aequus.Buffs
{
    public class Vampirism : TimerActiveBuff
    {
        public override int GetTick(Player player)
        {
            return player.GetModPlayer<VampirismPlayer>().vampirism;
        }

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.GetModPlayer<VampirismPlayer>().IsVampire)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
                return;
            }
            base.Update(player, ref buffIndex);
        }
    }
}