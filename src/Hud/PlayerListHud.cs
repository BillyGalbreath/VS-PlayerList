using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace PlayerList.Hud;

public class PlayerListHud : IRenderer {
    private readonly ICoreClientAPI api;
    private readonly Matrixf mvMatrix = new();
    private readonly MeshRef backgroundRef;
    private readonly Vec4f colorMask = new(1, 1, 1, 1);

    public PlayerListHud(ICoreClientAPI api) {
        this.api = api;

        api.Event.PlayerJoin += Update;
        api.Event.PlayerLeave += Update;

        backgroundRef = api.Render.UploadMesh(GetQuad(0x80000000));
    }

    private void Update(IClientPlayer player) {
        api.Gui.CreateCompo("playerlist:thelist", new ElementBounds() {
            Alignment = EnumDialogArea.CenterFixed,
            BothSizing = ElementSizing.Fixed,
            fixedWidth = 500,
            fixedHeight = 500
        });
    }

    public double RenderOrder {
        get {
            return 1.02;
        }
        private set { }
    }

    public int RenderRange {
        get {
            return 0;
        }
        private set { }
    }

    public void OnRenderFrame(float deltaTime, EnumRenderStage stage) {
        IShaderProgram shader = api.Render.CurrentActiveShader;

        // Render background
        shader.Uniform("rgbaIn", colorMask);
        shader.Uniform("extraGlow", 0);
        shader.Uniform("applyColor", 0);
        shader.Uniform("tex2d", 0);
        shader.Uniform("noTexture", 1f);

        mvMatrix
            .Set(api.Render.CurrentModelviewMatrix)
            .Translate(10, 10, 50)
            .Scale(100, 20, 0)
            .Translate(0.5f, 0.5f, 0)
            .Scale(0.5f, 0.5f, 0);

        shader.UniformMatrix("projectionMatrix", api.Render.CurrentProjectionMatrix);
        shader.UniformMatrix("modelViewMatrix", mvMatrix.Values);

        api.Render.RenderMesh(backgroundRef);

        // render player list
        // todo
    }

    public void Dispose() {
        api.Render.DeleteMesh(backgroundRef);
    }

    private static readonly int[] quadVertices = new int[12] { -1, -1, 0, 1, -1, 0, 1, 1, 0, -1, 1, 0 };
    private static readonly int[] quadTextureCoords = new int[8] { 0, 0, 1, 0, 1, 1, 0, 1 };
    private static readonly int[] quadVertexIndices = new int[6] { 0, 1, 2, 0, 2, 3 };

    private static MeshData GetQuad(uint argb) {
        float[] xyz = new float[12];
        for (int i = 0; i < 12; i++) {
            xyz[i] = quadVertices[i];
        }
        float[] uv = new float[8];
        for (int j = 0; j < uv.Length; j++) {
            uv[j] = quadTextureCoords[j];
        }
        byte[] rgba = new byte[16];
        for (int j = 0; j < 4; j++) {
            rgba[j * 4] = (byte)(argb >> 16 & 0xFF);
            rgba[j * 4 + 1] = (byte)(argb >> 8 & 0xFF);
            rgba[j * 4 + 2] = (byte)(argb & 0xFF);
            rgba[j * 4 + 3] = (byte)(argb >> 24 & 0xFF);
        }
        MeshData meshData = new();
        meshData.SetXyz(xyz);
        meshData.SetUv(uv);
        meshData.SetRgba(rgba);
        meshData.SetVerticesCount(4);
        meshData.SetIndices(quadVertexIndices);
        meshData.SetIndicesCount(6);
        return meshData;
    }
}
