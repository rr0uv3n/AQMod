﻿using AequusRemake.Core.Entities.Items.Dedications;
using AequusRemake.Core.Graphics;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;

namespace AequusRemake.Content.Dedicated.BeyondCoin;

public class BeyondPlatinumCoin : ModItem, DrawLayers.IDrawLayer {
    public override void SetDefaults() {
        Item.width = 8;
        Item.height = 8;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = Item.sellPrice(platinum: 100);
    }

    public override void Load() {
        DedicationRegistry.Register(this, new AnonymousDedication(new Color(105, 97, 191)));
    }

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 100;
        ItemSets.ShimmerTransformToItem[Type] = ModContent.ItemType<ShimmerCoin>();
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.PlatinumCoin, 100)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.PlatinumCoin);

        Recipe.Create(ItemID.PlatinumCoin, 100)
            .AddIngredient(Type)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.PlatinumCoin);
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.Lerp(lightColor, Color.White, 0.5f);
    }

    public override void PostUpdate() {
        if (Main.netMode == NetmodeID.Server || Main.GameUpdateCount % 3 == 0) {
            return;
        }

        Lighting.AddLight(Item.Center, Color.Blue.ToVector3() * 0.5f);

        Vector2 spawnCoordinates = Item.position + new Vector2(Main.rand.NextFloat(-2, Item.width + 2), Main.rand.NextFloat(-12f, Item.height));
        Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.05f, 0.25f);
        float lifeTime = 45f;

        FadingParticle fadingParticle = new FadingParticle();
        fadingParticle.SetBasicInfo(TextureAssets.Star[Main.rand.Next(2)], null, velocity, spawnCoordinates);
        fadingParticle.SetTypeInfo(lifeTime);
        fadingParticle.AccelerationPerFrame = velocity / lifeTime;
        fadingParticle.FadeInNormalizedTime = 0.5f;
        fadingParticle.FadeOutNormalizedTime = 0.5f;
        fadingParticle.Rotation = Main.rand.NextFloat() * MathHelper.TwoPi;
        fadingParticle.RotationVelocity += 0.05f + Main.rand.NextFloat() * 0.05f;
        fadingParticle.RotationAcceleration -= Main.rand.NextFloat() * 0.0025f;
        fadingParticle.Scale = Vector2.One * (0.1f + Main.rand.NextFloat(0.7f));
        fadingParticle.ColorTint = Color.Lerp(Color.Blue, Color.White, Main.rand.NextFloat(0.05f, 0.4f)) * fadingParticle.Scale.X * 1.5f;
        fadingParticle.ColorTint.A = 0;
        Main.ParticleSystem_World_OverPlayers.Add(fadingParticle);
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        DrawLayers.Instance.PostDrawLiquids += this;

        Texture2D texture = AequusTextures.BeyondPlatinumCoin_World;
        Rectangle frame = texture.Frame(verticalFrames: 8, frameY: (int)(Main.GameUpdateCount / 7 % 8));
        Vector2 drawCoordinates = ExtendItem.WorldDrawPos(Item, frame);
        Color color = Item.GetAlpha(lightColor);
        Vector2 origin = frame.Size() / 2f;
        Color outlineColor = Color.Pink with { A = 0 } * 0.35f;
        float timer = Main.GameUpdateCount / 60f;

        spriteBatch.Draw(AequusTextures.Bloom, drawCoordinates, null, Color.Blue with { A = 0 } * 0.4f, 0f, AequusTextures.Bloom.Size() / 2f, scale, SpriteEffects.None, 0f);
        spriteBatch.Draw(AequusTextures.FlareSoft, drawCoordinates - new Vector2(0f, 2f) * scale, null, Color.White with { A = 0 } * sin(timer, 0.2f, 0.45f), 0f, AequusTextures.FlareSoft.Size() / 2f, new Vector2(1f, 0.7f) * scale, SpriteEffects.None, 0f);

        spriteBatch.Draw(texture, drawCoordinates, frame, color, rotation, origin, scale, SpriteEffects.None, 0f);

        float outwards = sin(timer, 4f, 6f);
        float colorIntensity = sin(timer * 2f, 0f, 1f);

        for (int i = 0; i < 4; i++) {
            spriteBatch.Draw(texture, drawCoordinates + (i * MathHelper.PiOver2 + timer).ToRotationVector2() * outwards, frame, outlineColor * colorIntensity, rotation, origin, scale, SpriteEffects.None, 0f);
        }

        return false;
    }

    public void DrawOntoLayer(SpriteBatch spriteBatch, DrawLayers.DrawLayer layer) {
        DrawHelper.DrawMagicLensFlare(spriteBatch, Item.Center - Main.screenPosition, Color.Lerp(Color.BlueViolet, Color.White, 0.3f) * 0.4f, 0.9f);
    }
}