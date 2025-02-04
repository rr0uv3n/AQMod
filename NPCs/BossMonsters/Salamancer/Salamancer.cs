﻿using Aequus.Common.ContentTemplates;
using Aequus.Common.Items;

namespace Aequus.NPCs.BossMonsters.Salamancer;

public partial class Salamancer() : UnifiedBoss(new(ItemRarity: ItemDefaults.RaritySalamancer)) {
    public Point Origin;

    public override void Load() {
#if DEBUG
        base.Load();
#endif
    }

    public override void SetDefaults() {
        NPC.width = 60;
        NPC.height = 60;
        NPC.boss = true;
        NPC.lifeMax = 8000;
    }

    public override void FindFrame(int frameHeight) {
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        return base.PreDraw(spriteBatch, screenPos, drawColor);
    }

    public override void BossLoot(ref string name, ref int potionType) {
        potionType = ModContent.ItemType<Items.Potions.Healing.Restoration.LesserRestorationPotion>();
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        base.ModifyNPCLoot(npcLoot);
    }
}
