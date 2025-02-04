﻿using Terraria.GameContent;
using Terraria.Localization;

namespace Aequus.Content.Items.Accessories.PotionCanteen;

public class PotionCanteen : UnifiedCanteen {
    public PotionCanteen() : base(new CanteenInfo(
        MaxBuffs: 1,
        PotionsRequiredToAddBuff: 15,
        Rarity: ItemRarityID.Blue,
        Value: Item.buyPrice(gold: 5)
    )) { }

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(_info.MaxBuffs, _info.PotionsRequiredToAddBuff);

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        spriteBatch.Draw(AequusTextures.PotionCanteenEmpty, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
        if (!HasBuffs()) {
            return true;
        }

        var liquidColor = GetPotionColors();
        float a = drawColor.A > 0 ? drawColor.A / 255f : Main.inventoryBack.A / 255f;
        spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, liquidColor with { A = 255 }, 0f, origin, scale, SpriteEffects.None, 0f);
        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        Main.GetItemDrawFrame(Type, out Texture2D texture, out Rectangle frame);
        var position = Item.Center - Main.screenPosition;
        var origin = frame.Size() / 2f;
        spriteBatch.Draw(AequusTextures.PotionCanteenEmpty, position, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
        if (HasBuffs()) {
            var liquidColor = GetPotionColors();
            spriteBatch.Draw(texture, position, frame, lightColor.MultiplyRGBA(liquidColor), rotation, origin, scale, SpriteEffects.None, 0f);
        }
        return false;
    }
}