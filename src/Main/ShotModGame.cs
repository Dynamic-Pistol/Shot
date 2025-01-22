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
    // positions          // normals           // texture coords
    -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
     0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
    -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

    -0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 1.0f,
    -0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 0.0f,

    -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
    -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
    -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
    -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

     0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

    -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

    -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
    ];

    private readonly ShotTransform[] _cubeTransforms =
    [
        new ShotTransform(new Vector3(0.0f, 0.0f, 0.0f)),
        new ShotTransform(new Vector3(2.0f, 5.0f, -15.0f)),
        new ShotTransform(new Vector3(-1.5f, -2.2f, -2.5f)),
        new ShotTransform(new Vector3(-3.8f, -2.0f, -12.3f)),
        new ShotTransform(new Vector3(2.4f, -0.4f, -3.5f)),
        new ShotTransform(new Vector3(-1.7f, 3.0f, -7.5f)),
        new ShotTransform(new Vector3(1.3f, -2.0f, -2.5f)),
        new ShotTransform(new Vector3(1.5f, 2.0f, -2.5f)),
        new ShotTransform(new Vector3(1.5f, 0.2f, -1.5f)),
        new ShotTransform(new Vector3(-1.3f, 1.0f, -1.5f))
    ];
    
    private uint _cubeVao;
    private uint _lightVao;
    
    private uint _vbo;

    private ShotMaterial _lampMat = null!;
    private ShotMaterial _objectMat = null!;

    private Vector3 _lightPos = new Vector3(0, 0, 3);

    private uint _tex1;
    private uint _tex2;
    private int _selectedObject;

    protected override void OnLoad()
    {
        base.OnLoad();
        _camera.LockCamera();
        _controller = new ImGuiController(ClientSize.X, ClientSize.Y);

        GL.DebugMessageCallback(DebugMessages, IntPtr.Zero);
        GL.Enable(EnableCap.DebugOutput);
        GL.Enable(EnableCap.DebugOutputSynchronous);
        GL.Enable(EnableCap.DepthTest);

        _lampMat = new ShotMaterial(new Dictionary<ShaderType, string>
        {
            { ShaderType.VertexShader, File.ReadAllText("Assets/Shaders/cube.vert") },
            { ShaderType.FragmentShader, File.ReadAllText("Assets/Shaders/cube.frag") }
        });

        _objectMat = new ShotMaterial(new Dictionary<ShaderType, string>
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

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8,
            0);
        GL.EnableVertexAttribArray(0);
        
        GL.GenVertexArrays(1, out _lightVao);
        GL.BindVertexArray(_lightVao);
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8,
            0);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8,
            3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, sizeof(float) * 8,
            6 * sizeof(float));
        GL.EnableVertexAttribArray(2);
        
        _tex1 = LoadTexture("Assets/container2.png");
        _tex2 = LoadTexture("Assets/lighting_maps_specular_color.png");
        
        _objectMat.Use();

        _objectMat.SetIntUniform("material.diffuse", 0);
        _objectMat.SetIntUniform("material.specular", 1);
        for (int i = 0; i < _cubeTransforms.Length; i++)
        {
            var rot = MathHelper.DegreesToRadians(20f * i);
            _cubeTransforms[i].Rotation = Quaternion.FromAxisAngle(new Vector3(1.0f, 0.3f, 0.5f), rot);
        }
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        var moveInput = Vector3.Zero;
        moveInput.X = Convert.ToSingle(KeyboardState[Keys.D]) - Convert.ToSingle(KeyboardState[Keys.A]);
        moveInput.Y = Convert.ToSingle(KeyboardState[Keys.W]) - Convert.ToSingle(KeyboardState[Keys.S]);
        moveInput.Z = Convert.ToSingle(KeyboardState[Keys.E]) - Convert.ToSingle(KeyboardState[Keys.Q]);

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
        GL.ClearColor(new Color4(0.1f, 0.1f, 0.1f, 1.0f));
    this.
        _objectMat.Use();
        _objectMat.SetVector3Uniform("light.direction", new Vector3(-0.2f, -1.0f, -0.3f)); 	
        _objectMat.SetVector3Uniform("light.ambient", new Vector3(0.2f, 0.2f, 0.2f));
        _objectMat.SetVector3Uniform("light.diffuse", new Vector3(0.5f, 0.5f, 0.5f));
        _objectMat.SetVector3Uniform("light.specular", new Vector3(1.0f, 1.0f, 1.0f));
        
        _objectMat.SetMatrix4Uniform("projection", _camera.Projection);
        _objectMat.SetMatrix4Uniform("view", _camera.View);
        _objectMat.SetVector3Uniform("viewPos", _camera.Position);
        
        _objectMat.SetFloatUniform("material.shininess", 32.0f);

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _tex1);
        GL.ActiveTexture(TextureUnit.Texture1);
        GL.BindTexture(TextureTarget.Texture2D, _tex2);

        GL.BindVertexArray(_lightVao);
        for (int i = 0; i < _cubeTransforms.Length; i++)
        {
            var objectModel = Matrix4.Identity * Matrix4.CreateTranslation(_cubeTransforms[i].Position) * 
                              Matrix4.CreateFromQuaternion(_cubeTransforms[i].Rotation);
            _objectMat.SetMatrix4Uniform("model", objectModel);

            if (i == _selectedObject)
            {
                _objectMat.SetIntUniform("selected", 1);
            }
            else
            {
                _objectMat.SetIntUniform("selected", 0);
            }
        
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }
        
        var lampModel = Matrix4.Identity * Matrix4.CreateTranslation(_lightPos) * Matrix4.CreateScale(0.2f);
        
        _lampMat.Use();
        _lampMat.SetMatrix4Uniform("proj", _camera.Projection);
        _lampMat.SetMatrix4Uniform("view", _camera.View);
        _lampMat.SetMatrix4Uniform("model", lampModel);

        GL.BindVertexArray(_cubeVao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        
        
        if (ImGui.Begin("Test"))
        {
            ImGui.Text($"Camera Position:{_camera.Position}");
            ImGui.Text($"Camera Rotation:{_camera.Yaw}, {_camera.Pitch}");
            ImGui.Text($"Camera Zoom: {_camera.Fov}");
            var snLightPos = _lightPos.OpenTk2Sys();
            ImGui.DragFloat3("Light position", ref snLightPos, 0.01f);
            _lightPos = snLightPos.Sys2OpenTk();
            ImGui.End();
        }
        else
        {
            ImGui.End();
        }

        if (ImGui.Begin("Inspector"))
        {
            var selectedPosition = _cubeTransforms[_selectedObject].Position.OpenTk2Sys();
            ImGui.DragFloat3("Position", ref selectedPosition, 0.01f);
            _cubeTransforms[_selectedObject].Position = selectedPosition.Sys2OpenTk();
            var selectedRotation = _cubeTransforms[_selectedObject].Rotation.ToEulerAngles().OpenTk2Sys();
            ImGui.DragFloat3("Rotation", ref selectedRotation, 0.01f);
            _cubeTransforms[_selectedObject].Rotation = Quaternion.Identity * Quaternion.FromEulerAngles(selectedRotation.X,selectedRotation.Y,selectedRotation.Z);
            ImGui.End();
        }
        else
        {
            ImGui.End();
        }
        
        if (ImGui.Begin("Hierarchy"))
        {
            for (int i = 0; i < _cubeTransforms.Length; i++)
            {
                
                if (ImGui.Selectable($"Cube {i}", i == _selectedObject))
                {
                    _selectedObject = i;
                }
            }
            ImGui.End();
        }
        else
        {
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
        IntPtr message, IntPtr userParam)
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