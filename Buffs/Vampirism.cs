﻿using Aequus.Common.Players;
using Aequus.Items;
using Terraria;

namespace Aequus.Buffs
{
    public class Vampirism : TimerActiveBuff
    {
        public override int GetTick(Player player)
        {
            return player.GetModPlayer<PlayerVampirism>().vampirism;
        }

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            AequusBuff.ConcoctibleBuffsBlacklist.Add(Type);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.GetModPlayer<PlayerVampirism>().IsVampire)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
                return;
            }
            base.Update(player, ref buffIndex);
        }
    }
}