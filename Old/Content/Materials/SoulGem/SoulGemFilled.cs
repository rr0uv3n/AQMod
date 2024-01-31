﻿using Aequus.Common.Items;
using Terraria.GameContent;

namespace Aequus.Old.Content.Materials.SoulGem;

public class SoulGemFilled : ModItem {
    public override string Texture => ModContent.GetInstance<SoulGem>().Texture;

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 25;
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SoulGem>();
        ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.Amber;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<SoulGemFilledTile>());
        Item.rare = ItemRarityID.Pink;
        Item.value = Item.buyPrice(gold: 1, silver: 50);
    }

    public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, Color.White with { A = 0 } * Helper.Oscillate(Main.GlobalTimeWrappedHourly * 2.5f, 0.1f, 0.33f),
            0f, origin, scale, SpriteEffects.None, 0f);
    }

    public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) {
        Main.GetItemDrawFrame(Type, out Texture2D texture, out var frame);
        spriteBatch.Draw(texture, ExtendItem.WorldDrawPos(Item, frame) + new Vector2(0f, -2f), frame, Color.White with { A = 0 } * Helper.Oscillate(Main.GlobalTimeWrappedHourly * 2.5f, 0.33f, 0.66f),
            rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
    }

    public override void AddRecipes() {
        Recipe.Create(ItemID.LifeCrystal)
            .AddIngredient(Type)
            .AddIngredient<BloodyTearstone>()
            .AddTile(TileID.DemonAltar)
            .Register()
            .DisableDecraft();
    }
}