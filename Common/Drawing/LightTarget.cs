﻿using Aequus.Common.Graphics.Shaders;
using Aequus.Common.Utilities;
using System;

namespace Aequus.Common.Drawing;

[Autoload(Side = ModSide.Client)]
public class LightTarget : LoadedType {
    public RenderTarget2D? RenderTarget { get; private set; }
    public bool TargetAvailable { get; private set; }

    public bool Needed { get; set; }

    public int LightmapX;
    public int LightmapY;
    public Vector2 LightmapScreenPosition;
    private VertexPositionColor[] _lightMapVertices = Array.Empty<VertexPositionColor>();
    private short[] _lightMapIndices = Array.Empty<short>();

    void Main_OnPreDraw(GameTime obj) {
        TargetAvailable = false;
        if (Needed) {
            int offscreenTiles = 8;

            int width = Main.screenWidth / 16 + offscreenTiles - 1;
            int height = Main.screenHeight / 16 + offscreenTiles - 1;
            int totalLength = width * height * 6;
            if (_lightMapIndices.Length != totalLength) {
                _lightMapIndices = new short[totalLength];
                SetIndices(width, height);
            }

            width = Main.screenWidth / 16 + offscreenTiles;
            height = Main.screenHeight / 16 + offscreenTiles;
            totalLength = width * height;

            if (_lightMapVertices.Length != totalLength) {
                Array.Resize(ref _lightMapVertices, totalLength);
                SetVertexPositions(width, height);
            }

            if (Main.renderCount == 1) {
                SampleLighting(width, height);
            }

            GraphicsDevice g = Main.instance.GraphicsDevice;
            int targetWidth = Main.screenWidth + offscreenTiles * 8;
            int targetHeight = Main.screenHeight + offscreenTiles * 8;
            if (DrawHelper.BadRenderTarget(RenderTarget, targetWidth, targetHeight)) {
                try {
                    if (Main.IsGraphicsDeviceAvailable) {
                        RenderTarget = new RenderTarget2D(g, targetWidth, targetHeight, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                    }
                }
                catch {
                }
                return;
            }

            DrawHelper.gdHelper.EnqueueRenderTargetBindings(g);

            try {
                g.SetRenderTarget(RenderTarget);
                g.Clear(Color.Transparent);

                Effect effect = AequusShaders.VertexShader.Value;
                effect.CurrentTechnique = effect.Techniques["Untextured"];

                Vector2 offset = LightmapScreenPosition - Main.screenPosition;
                Matrix m = DrawHelper.WorldViewPointMatrix;
                m = Matrix.Multiply(Matrix.CreateTranslation(offset.X + 8, offset.Y + 8, 0f), m);
                effect.Parameters["XViewProjection"].SetValue(m);
                foreach (var pass in effect.CurrentTechnique.Passes) {
                    pass.Apply();
                    g.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _lightMapVertices, 0, _lightMapVertices.Length, _lightMapIndices, 0, _lightMapIndices.Length / 3);
                }

                //sb.Begin();
                //for (int i = 0; i < _lightMapIndices.Length / 12; i++) {
                //    int index = i * 6;
                //    Vector2 v1 = new Vector2(_lightMapVertices[_lightMapIndices[index]].Position.X, _lightMapVertices[_lightMapIndices[index]].Position.Y);
                //    Vector2 v2 = new Vector2(_lightMapVertices[_lightMapIndices[index + 1]].Position.X, _lightMapVertices[_lightMapIndices[index + 1]].Position.Y);
                //    Vector2 v3 = new Vector2(_lightMapVertices[_lightMapIndices[index + 2]].Position.X, _lightMapVertices[_lightMapIndices[index + 2]].Position.Y);
                //    DrawHelper.DrawLine(v1, v2, 2f, Color.Red);
                //    DrawHelper.DrawLine(v2, v3, 2f, Color.Green);
                //    DrawHelper.DrawLine(v3, v1, 2f, Color.Blue);
                //}
                //sb.End();
            }
            catch {
            }
            finally {
            }

            TargetAvailable = true;
            DrawHelper.gdHelper.DequeueRenderTargetBindings(g);
        }
    }

    void SetIndices(int width, int height) {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                int vertexIndex = i * height + j;
                int index = vertexIndex * 6;
                vertexIndex += i * 1;
                _lightMapIndices[index] = (short)(vertexIndex + height + 2);
                _lightMapIndices[index + 1] = (short)(vertexIndex + 1);
                _lightMapIndices[index + 2] = (short)vertexIndex;
                _lightMapIndices[index + 5] = (short)vertexIndex;
                _lightMapIndices[index + 3] = (short)(vertexIndex + height + 1);
                _lightMapIndices[index + 4] = (short)(vertexIndex + height + 2);
            }
        }
    }

    void SetVertexPositions(int width, int height) {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                _lightMapVertices[i * height + j].Position = new Vector3(i * 16f, j * 16f, 0f);
            }
        }
    }

    void SampleLighting(int width, int height) {
        LightmapX = (int)(Main.screenPosition.X / 16) - 1;
        LightmapY = (int)(Main.screenPosition.Y / 16) - 1;
        LightmapScreenPosition = new Vector2(LightmapX, LightmapY) * 16f;

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                int x = LightmapX + i;
                int y = LightmapY + j;

                _lightMapVertices[i * height + j].Color = Lighting.GetColor(x - 2, y - 2);
            }
        }
    }

    protected override void Load() {
        Main.OnPreDraw += Main_OnPreDraw;
    }

    protected override void Unload() {
        Main.OnPreDraw -= Main_OnPreDraw;
    }
}
