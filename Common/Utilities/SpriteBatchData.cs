﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using Terraria.ModLoader;

namespace Aequus.Common.Utilities
{
    public sealed class SpriteBatchData : ILoadable
    {
        private static FieldInfo sortModeField;
        private static FieldInfo blendStateField;
        private static FieldInfo depthStencilStateField;
        private static FieldInfo rasterizerStateField;
        private static FieldInfo samplerStateField;
        private static FieldInfo customEffectField;
        private static FieldInfo transformMatrixField;

        public SpriteSortMode sortMode;
        public BlendState blendState;
        public DepthStencilState depthStencilState;
        public RasterizerState rasterizerState;
        public SamplerState samplerState;
        public Effect customEffect;
        public Matrix transformMatrix;

        public SpriteBatchData()
        {
        }

        public SpriteBatchData(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState,
            RasterizerState rasterizerState, Effect effect, Matrix transformMatrix)
        {
            this.sortMode = sortMode;
            this.blendState = blendState;
            this.samplerState = samplerState;
            this.depthStencilState = depthStencilState;
            this.rasterizerState = rasterizerState;
            customEffect = effect;
            this.transformMatrix = transformMatrix;
        }

        public SpriteBatchData(SpriteBatch spriteBatch)
        {
            FromSpriteBatch(spriteBatch);
        }

        private void FromSpriteBatch(SpriteBatch spriteBatch)
        {
            sortMode = sortModeField.GetValue<SpriteSortMode>(spriteBatch);
            blendState = blendStateField.GetValue<BlendState>(spriteBatch);
            samplerState = samplerStateField.GetValue<SamplerState>(spriteBatch);
            depthStencilState = depthStencilStateField.GetValue<DepthStencilState>(spriteBatch);
            rasterizerState = rasterizerStateField.GetValue<RasterizerState>(spriteBatch);
            customEffect = customEffectField.GetValue<Effect>(spriteBatch);
            transformMatrix = transformMatrixField.GetValue<Matrix>(spriteBatch);
        }

        public void Begin(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, customEffect, transformMatrix);
        }

        public void BeginCustom(SpriteBatch spriteBatch,
            NullableOption<SpriteSortMode> spriteSortMode = default(NullableOption<SpriteSortMode>),
            NullableOption<BlendState> blendState = default(NullableOption<BlendState>),
            NullableOption<SamplerState> samplerState = default(NullableOption<SamplerState>),
            NullableOption<DepthStencilState> depthStencilState = default(NullableOption<DepthStencilState>),
            NullableOption<RasterizerState> rasterizerState = default(NullableOption<RasterizerState>),
            NullableOption<Effect> customEffect = default(NullableOption<Effect>),
            NullableOption<Matrix> transformMatrix = default(NullableOption<Matrix>))
        {
            spriteBatch.Begin(spriteSortMode.Get(this.sortMode), blendState.Get(this.blendState),
                samplerState.Get(this.samplerState), depthStencilState.Get(this.depthStencilState),
                rasterizerState.Get(this.rasterizerState), customEffect.Get(this.customEffect), transformMatrix.Get(this.transformMatrix));
        }

        void ILoadable.Load(Mod mod)
        {
            var t = typeof(SpriteBatch);
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            sortModeField = t.GetField(nameof(sortMode), flags);
            blendStateField = t.GetField(nameof(blendState), flags);
            samplerStateField = t.GetField(nameof(samplerState), flags);
            depthStencilStateField = t.GetField(nameof(depthStencilState), flags);
            rasterizerStateField = t.GetField(nameof(rasterizerState), flags);
            customEffectField = t.GetField(nameof(customEffect), flags);
            transformMatrixField = t.GetField(nameof(transformMatrix), flags);
        }

        void ILoadable.Unload()
        {
            sortModeField = null;
            blendStateField = null;
            samplerStateField = null;
            depthStencilStateField = null;
            rasterizerStateField = null;
            customEffectField = null;
            transformMatrixField = null;
        }
    }
}