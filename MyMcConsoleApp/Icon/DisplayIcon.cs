using System.Numerics;
using MyMc.Models;
using Raylib_cs;

namespace MyMcConsoleApp.Icon;

public static class DisplayIcon
{
    public static void Render(IconFile iconFile, IconSys iconSys, string directoryName)
    {
        Raylib.InitWindow(960, 640, $"{directoryName} :: {iconSys.Title} Icon (Raylib)");
        Raylib.SetTargetFPS(60);

        var camera = new Camera3D
        {
            Position = new Vector3(0, 2.0f, 5.0f),
            Target = Vector3.Zero,
            Up = new Vector3(0, 1, 0),
            FovY = 45.0f,
            Projection = CameraProjection.Perspective
        };

        const string VertexShaderSrc = """
            #version 330
            in vec3 vertexPosition;
            in vec3 vertexNormal;
            in vec2 vertexTexCoord;
            in vec4 vertexColor;

            uniform mat4 u_mvp;

            out vec2 v_uv;
            out vec4 v_color;
            out vec3 v_normal;

            void main()
            {
                gl_Position = u_mvp * vec4(vertexPosition, 1.0);
                v_uv = vertexTexCoord;
                v_color = vertexColor;
                v_normal = vertexNormal;
            }
            """;

        const string FragmentShaderSrc = """
            #version 330
            in vec2 v_uv;
            in vec4 v_color;
            in vec3 v_normal;

            uniform sampler2D u_texture;
            uniform vec3 u_ambient;
            uniform vec3 u_lightDir1;
            uniform vec3 u_lightColor1;
            uniform vec3 u_lightDir2;
            uniform vec3 u_lightColor2;
            uniform vec3 u_lightDir3;
            uniform vec3 u_lightColor3;

            out vec4 finalColor;

            void main()
            {
                vec4 texColor = texture(u_texture, v_uv);
                if (texColor.a < 0.1) discard;

                vec4 baseColor = texColor * v_color;
                vec3 normal = normalize(v_normal);

                vec3 light = u_ambient;
                light += u_lightColor1 * max(dot(normal, normalize(u_lightDir1)), 0.0);
                light += u_lightColor2 * max(dot(normal, normalize(u_lightDir2)), 0.0);
                light += u_lightColor3 * max(dot(normal, normalize(u_lightDir3)), 0.0);

                finalColor = vec4(baseColor.rgb * light, baseColor.a);
            }
            """;

        var mesh = BuildIcon.BuildIconMesh((int)iconFile.VertexCount, iconFile.Positions[0], iconFile.Normals, iconFile.Uvs, iconFile.VertexColors);
        var texture = BuildIcon.BuildTexture(iconFile.Texture, 128, 128, false);

        Shader iconShader = Raylib.LoadShaderFromMemory(VertexShaderSrc, FragmentShaderSrc);
        Material material = Raylib.LoadMaterialDefault();
        material.Shader = iconShader;

        if (texture.Id != 0)
        {
            Raylib.SetMaterialTexture(ref material, MaterialMapIndex.Albedo, texture);
        }

        // Cache uniform locations once outside the loop
        int locMvp = Raylib.GetShaderLocation(iconShader, "u_mvp");
        int locTex = Raylib.GetShaderLocation(iconShader, "u_texture");
        int locAmbient = Raylib.GetShaderLocation(iconShader, "u_ambient");
        int locLightDir1 = Raylib.GetShaderLocation(iconShader, "u_lightDir1");
        int locLightCol1 = Raylib.GetShaderLocation(iconShader, "u_lightColor1");
        int locLightDir2 = Raylib.GetShaderLocation(iconShader, "u_lightDir2");
        int locLightCol2 = Raylib.GetShaderLocation(iconShader, "u_lightColor2");
        int locLightDir3 = Raylib.GetShaderLocation(iconShader, "u_lightDir3");
        int locLightCol3 = Raylib.GetShaderLocation(iconShader, "u_lightColor3");

        // 1. Calculate Bounding Box
        double minX = double.PositiveInfinity, minY = double.PositiveInfinity, minZ = double.PositiveInfinity;
        double maxX = double.NegativeInfinity, maxY = double.NegativeInfinity, maxZ = double.NegativeInfinity;

        var firstFrame = iconFile.Positions[0];
        for (int i = 0; i < iconFile.VertexCount; i++)
        {
            double x = firstFrame[i * 3];
            double y = firstFrame[i * 3 + 1];
            double z = firstFrame[i * 3 + 2];
            if (x < minX) minX = x; if (x > maxX) maxX = x;
            if (y < minY) minY = y; if (y > maxY) maxY = y;
            if (z < minZ) minZ = z; if (z > maxZ) maxZ = z;
        }

        double cx = (minX + maxX) / 2.0;
        double cy = (minY + maxY) / 2.0;
        double cz = (minZ + maxZ) / 2.0;
        double maxDim = Math.Max(maxX - minX, Math.Max(maxY - minY, maxZ - minZ));
        double scale = maxDim > 0 ? (1.3 / maxDim) : 1.0;

        float rotationAngle = 0f;
        int frameIndex = 0;
        int lastUploadedFrame = -1;
        bool paused = false;

        var bgColor = new Color(
            (byte)iconSys.BackgroundColors[0],
            (byte)iconSys.BackgroundColors[1],
            (byte)iconSys.BackgroundColors[2],
            (byte)255);

        int delay = 0;

        while (!Raylib.WindowShouldClose())
        {
            float dt = Raylib.GetFrameTime();

            if (Raylib.IsKeyPressed(KeyboardKey.Space))
                paused = !paused;

            if (!paused && iconFile.Positions.Length > 0 && delay % 6 == 0)
            {
                frameIndex = (frameIndex + 1) % iconFile.Positions.Length;
                delay = 0;
            }

            if (Raylib.IsKeyPressed(KeyboardKey.R))
                rotationAngle -= 10.0f;

            if (iconFile.Positions.Length > 0 && frameIndex != lastUploadedFrame)
            {
                BuildIcon.UpdateFramePositions(mesh, (int)iconFile.VertexCount, iconFile.Positions[frameIndex]);
                lastUploadedFrame = frameIndex;
            }

            float deg2Rad = MathF.PI / 180f;

            Matrix4x4 model =
                Matrix4x4.CreateTranslation((float)-cx, (float)-cy, (float)-cz) *
                Matrix4x4.CreateScale((float)scale, (float)-scale, (float)scale) *
                Matrix4x4.CreateRotationX(0f) *
                Matrix4x4.CreateRotationY((rotationAngle + 200f) * deg2Rad);

            Matrix4x4 view = Matrix4x4.CreateLookAt(new Vector3(0, 0, 2.5f), Vector3.Zero, Vector3.UnitY);
            Matrix4x4 proj = Matrix4x4.CreatePerspectiveFieldOfView(45f * deg2Rad, 1.0f, 0.1f, 100.0f);

            Matrix4x4 mvp = model * view * proj;

            Raylib.SetShaderValueMatrix(iconShader, locMvp, Matrix4x4.Transpose(mvp));
            Raylib.SetShaderValue(iconShader, locTex, 0, ShaderUniformDataType.Int);
            SetVec3(iconShader, locAmbient, iconSys.AmbientLightColor);
            SetVec3(iconShader, locLightDir1, iconSys.LightDirection1);
            SetVec3(iconShader, locLightCol1, iconSys.LightColor1);
            SetVec3(iconShader, locLightDir2, iconSys.LightDirection2);
            SetVec3(iconShader, locLightCol2, iconSys.LightColor2);
            SetVec3(iconShader, locLightDir3, iconSys.LightDirection3);
            SetVec3(iconShader, locLightCol3, iconSys.LightColor3);

            Raylib.BeginDrawing();
            Raylib.ClearBackground(bgColor);

            Raylib.BeginMode3D(camera);
            Raylib.DrawMesh(mesh, material, Matrix4x4.Identity);
            Raylib.EndMode3D();

            Raylib.DrawText($"Frame: {frameIndex} / {Math.Max(1, iconFile.Positions.Length)} | {(paused ? "PAUSED" : "PLAYING")}", 14, 14, 18, Color.RayWhite);
            Raylib.EndDrawing();

            delay++;
        }

        static void SetVec3(Shader shader, int loc, double[] v)
        {
            var vec = new Vector3((float)v[0], (float)v[1], (float)v[2]);
            Raylib.SetShaderValue(shader, loc, vec, ShaderUniformDataType.Vec3);
        }
    }
}