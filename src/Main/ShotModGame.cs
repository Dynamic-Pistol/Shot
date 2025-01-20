using System.Drawing;
using System.Runtime.InteropServices;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Shot.GUI;
using Shot.Rendering.Objects;
using Shot.ShotMath;
using StbImageSharp;

namespace Shot.Main;

public class ShotGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
    : GameWindow(gameWindowSettings, nativeWindowSettings)
{
    private readonly ShotCamera _camera = new ShotCamera();
    private ImGuiController _controller = null!;

    private readonly float[] _vertices =
    [
        -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
        0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 
        0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 
        0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 
        -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 
        -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 

        -0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
        0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
        0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
        0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
        -0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
        -0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,

        -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
        -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
        -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
        -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
        -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
        -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,

        0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
        0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
        0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
        0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
        0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
        0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,

        -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
        0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
        0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
        0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
        -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
        -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,

        -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
        0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
        0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
        0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
        -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
        -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f
    ];

    private uint _cubeVao;
    private uint _lightVao;
    
    private uint _vbo;

    private ShotMaterial _cubeMat = null!;
    private ShotMaterial _lightMat = null!;

    private System.Numerics.Vector3 _lightPos = System.Numerics.Vector3.Zero;

    protected override void OnLoad()
    {
        base.OnLoad();
        _camera.LockCamera();
        _controller = new ImGuiController(ClientSize.X, ClientSize.Y);

        GL.DebugMessageCallback(DebugMessages, IntPtr.Zero);
        GL.Enable(EnableCap.DebugOutput);
        GL.Enable(EnableCap.DebugOutputSynchronous);
        GL.Enable(EnableCap.DepthTest);

        _cubeMat = new ShotMaterial(new Dictionary<ShaderType, string>
        {
            { ShaderType.VertexShader, File.ReadAllText("Assets/Shaders/cube.vert") },
            { ShaderType.FragmentShader, File.ReadAllText("Assets/Shaders/cube.frag") }
        });

        _lightMat = new ShotMaterial(new Dictionary<ShaderType, string>
        {
            { ShaderType.VertexShader, File.ReadAllText("Assets/Shaders/light.vert") },
            { ShaderType.FragmentShader, File.ReadAllText("Assets/Shaders/light.frag") }
        });

        GL.GenVertexArrays(1, out _cubeVao);
        GL.BindVertexArray(_cubeVao);

        GL.GenBuffers(1, out _vbo);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, Marshal.SizeOf<ShotVertex>() * _vertices.Length, _vertices,
            BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 6,
            0);
        GL.EnableVertexAttribArray(0);
        
        GL.GenVertexArrays(1, out _lightVao);
        GL.BindVertexArray(_lightVao);
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 6,
            0);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, sizeof(float) * 6,
            3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        var moveInput = Vector3.Zero;
        moveInput.X = Convert.ToInt32(KeyboardState[Keys.D]) - Convert.ToInt32(KeyboardState[Keys.A]);
        moveInput.Y = Convert.ToInt32(KeyboardState[Keys.W]) - Convert.ToInt32(KeyboardState[Keys.S]);
        moveInput.Z = Convert.ToInt32(KeyboardState[Keys.E]) - Convert.ToInt32(KeyboardState[Keys.Q]);

        _camera.MoveCamera(moveInput, (float)args.Time, MouseState.ScrollDelta.Y);
        _controller.Update(this, (float)args.Time);
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);
        const float sensitivity = 0.04f;
        float moveX = e.DeltaX * sensitivity;
        float moveY = e.DeltaY * sensitivity;
        _camera.RotateCamera(moveX, moveY);
            
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);


        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.ClearColor(Color.Black);

        
        // _lightMat.SetVector3Uniform("material.ambient", new Vector3(1.0f, 0.5f, 0.31f));
        // _lightMat.SetVector3Uniform("material.diffuse", new Vector3(1.0f, 0.5f, 0.31f));
        // _lightMat.SetVector3Uniform("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
        // _lightMat.SetFloatUniform("material.shininess", 32.0f);
        
        
        var lightModel = Matrix4.Identity;
        Vector3 lightColor;
        float time = DateTime.Now.Second + DateTime.Now.Millisecond / 1000f;
        lightColor.X = (MathF.Sin(time * 2.0f) + 1) / 2f;
        lightColor.Y = (MathF.Sin(time * 0.7f) + 1) / 2f;
        lightColor.Z = (MathF.Sin(time * 1.3f) + 1) / 2f;

        // The ambient light is less intensive than the diffuse light in order to make it less dominant
        // Vector3 ambientColor = lightColor * new Vector3(0.2f);
        // Vector3 diffuseColor = lightColor * new Vector3(0.5f);

        // _lightMat.SetVector3Uniform("light.position", _lightPos.Sys2OpenTk());
        // _lightMat.SetVector3Uniform("light.ambient", ambientColor);
        // _lightMat.SetVector3Uniform("light.diffuse", diffuseColor);
        // _lightMat.SetVector3Uniform("light.specular", new Vector3(1.0f, 1.0f, 1.0f));
        
        _lightMat.Use();
        _lightMat.SetVector3Uniform("lightPos", _lightPos.Sys2OpenTk());
        _lightMat.SetVector3Uniform("lightColor", new Vector3( 1.0f, 0.5f, 0.31f));
        _lightMat.SetVector3Uniform("objectColor", new Vector3(1.0f, 1.0f, 1.0f));
        _lightMat.SetMatrix4Uniform("projection", _camera.Projection);
        _lightMat.SetMatrix4Uniform("view", _camera.View);
        _lightMat.SetVector3Uniform("viewPos", _camera.Position);
        _lightMat.SetMatrix4Uniform("model", lightModel);
        
        GL.BindVertexArray(_lightVao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        
        var cubeModel = Matrix4.Identity * Matrix4.CreateTranslation(_lightPos.Sys2OpenTk()) * Matrix4.CreateScale(0.2f);
        
        _cubeMat.Use();
        _cubeMat.SetMatrix4Uniform("proj", _camera.Projection);
        _cubeMat.SetMatrix4Uniform("view", _camera.View);
        _cubeMat.SetMatrix4Uniform("model", cubeModel);

        GL.BindVertexArray(_cubeVao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        
        
        if (ImGui.Begin("Test"))
        {
            ImGui.Text($"Camera Position:{_camera.Position}");
            ImGui.Text($"Camera Rotation:{_camera.Yaw}, {_camera.Pitch}");
            ImGui.Text($"Camera Zoom: {_camera.Fov}");
            ImGui.DragFloat3("Light position", ref _lightPos, 0.01f);
            ImGui.End();
        }
        
        _controller.Render();

        SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);

        _controller.WindowResized(e.Width, e.Height);
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Keys.Escape)
        {
            Close();
        }
        if (e.Key == Keys.J)
        {
            CursorState = _camera.LockCamera();
        }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);
        _controller.MouseScroll(e.Offset);
    }

    protected override void OnTextInput(TextInputEventArgs e)
    {
        base.OnTextInput(e);

        _controller.PressChar((char)e.Unicode);
    }

    private void DebugMessages(DebugSource source, DebugType type, int id, DebugSeverity severity, int length,
        IntPtr message, IntPtr userparam)
    {
        var msg = Marshal.PtrToStringAnsi(message, length);

        switch (severity)
        {
            case DebugSeverity.DontCare:
            case DebugSeverity.DebugSeverityNotification:
                ShotLogger.LogInfo("{0}, {1}", type, msg);
                break;
            case DebugSeverity.DebugSeverityHigh:
            case DebugSeverity.DebugSeverityMedium:
            case DebugSeverity.DebugSeverityLow:
                ShotLogger.LogError("{0}, {1}", type, msg);
                break;
        }
    }

    private uint LoadTexture(string path)
    {
        GL.GenTextures(1, out uint textureId);

        using var fs = File.Open(path, FileMode.Open, FileAccess.Read);
        var image = ImageResult.FromStream(fs);
        PixelFormat format = PixelFormat.Red;
        
        if (image.Comp == ColorComponents.RedGreenBlue)
            format = PixelFormat.Rgb;
        else if (image.Comp == ColorComponents.RedGreenBlueAlpha)
            format = PixelFormat.Rgba;

        GL.BindTexture(TextureTarget.Texture2D, textureId);
        GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)format, image.Width, image.Height, 0, format, PixelType.UnsignedByte, image.Data);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat); 
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat); 
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear); 
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear); 
        
        return textureId;
    }
}