using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace PlayerList.Hud;

public class PlayerListHud : IRenderer {
    private readonly ICoreClientAPI api;
    private readonly Matrixf mvMatrix = new();
    private readonly MeshRef backgroundRef;
    private readonly Vec4f colorMask = new(0, 0, 0, 0.5F);

    public PlayerListHud(ICoreClientAPI api) {
        this.api = api;

        api.Event.PlayerJoin += Update;
        api.Event.PlayerLeave += Update;

        backgroundRef = api.Render.UploadMesh(QuadMeshUtil.GetQuad());
    }

    private void Update(IClientPlayer player) {
        //
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
}
