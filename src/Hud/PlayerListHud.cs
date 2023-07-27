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

    public PlayerListHud(ICoreClientAPI api) {
        this.api = api;

        api.Event.PlayerJoin += Update;
        api.Event.PlayerLeave += Update;

        MeshData rectangle = LineMeshUtil.GetRectangle(ColorUtil.ToRgba(128, 0, 0, 0));
        backgroundRef = api.Render.UploadMesh(rectangle);
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

        Vec4f color = new(1, 1, 1, 1);

        // Render background
        shader.Uniform("rgbaIn", color);
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
}
