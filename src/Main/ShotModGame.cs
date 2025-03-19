using System.Runtime.InteropServices;
using ImGuiNET;
using ImGuizmoNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Shot.GUI;
using Shot.Rendering.Objects;
using Shot.ShotMath;
using StbImageSharp;
using Vector2 = System.Numerics.Vector2;

namespace Shot.Main;

public class ShotGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
    : GameWindow(gameWindowSettings, nativeWindowSettings)
{
    private readonly ShotCamera _camera = new ShotCamera();
    private ImGuiController _controller = null!;
    private MODE _gizmoMode = MODE.WORLD;
    private OPERATION _gizmoOperation = OPERATION.TRANSLATE;

    private readonly float[] _vertices =
    [
        // positions          // normals           // texture coords
        -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f,
        0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f,
        0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f,
        0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f,
        -0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f,

        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
        0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f,
        0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f,
        -0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,

        -0.5f, 0.5f, 0.5f, -1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
        -0.5f, 0.5f, -0.5f, -1.0f, 0.0f, 0.0f, 1.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, -1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, -1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
        -0.5f, -0.5f, 0.5f, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f,
        -0.5f, 0.5f, 0.5f, -1.0f, 0.0f, 0.0f, 1.0f, 0.0f,

        0.5f, 0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f,
        0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
        0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
        0.5f, -0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,

        -0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, 0.0f, 1.0f,
        0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, 1.0f, 1.0f,
        0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, 1.0f, 0.0f,
        0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, 1.0f, 0.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, 0.0f, 0.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, 0.0f, 1.0f,

        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
        0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f,
        0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f,
        -0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f,
        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f
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

        _camera.AspectRatio = (float)FramebufferSize.X / FramebufferSize.Y;
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
            _cubeTransforms[i].EularRotation =
                Quaternion.FromAxisAngle(new Vector3(1.0f, 0.3f, 0.5f), rot).ToEulerAngles();
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

        _camera.UpdateCamera();
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

        _controller.Update(this, (float)e.Time);

        var matrix = Matrix4.Identity;
        ImGuizmo.DrawGrid(ref _camera.View.Row0.X, ref _camera.Projection.Row0.X, ref matrix.Row0.X, 20);
        ImGuizmo.ViewManipulate(ref _camera.View.Row0.X, 1, Vector2.Zero, new Vector2(100, 100), 255);

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
            var objectModel = Matrix4.Identity;

            ImGuizmo.RecomposeMatrixFromComponents(ref _cubeTransforms[i].Position.X,
                ref _cubeTransforms[i].EularRotation.X, ref _cubeTransforms[i].Scale.X, ref objectModel.Row0.X);

            _objectMat.SetMatrix4Uniform("model", objectModel);
            
            var rayDir = ScreenPosToWorldRay();
            if (!ImGuizmo.IsOver() && MouseState.IsButtonPressed(MouseButton.Left) && RayCastObject(_camera.Position, rayDir, -Vector3.One, Vector3.One, objectModel))
            {
                _selectedObject = i;
            }

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
            Matrix4 selectedObjectModel = Matrix4.Identity;
            ImGuizmo.RecomposeMatrixFromComponents(ref _cubeTransforms[_selectedObject].Position.X,
                ref _cubeTransforms[_selectedObject].EularRotation.X, ref _cubeTransforms[_selectedObject].Scale.X,
                ref selectedObjectModel.Row0.X);

            ImGuizmo.Manipulate(ref _camera.View.Row0.X, ref _camera.Projection.Row0.X, _gizmoOperation, _gizmoMode,
                ref selectedObjectModel.Row0.X);


            ImGuizmo.DecomposeMatrixToComponents(ref selectedObjectModel.Row0.X,
                ref _cubeTransforms[_selectedObject].Position.X,
                ref _cubeTransforms[_selectedObject].EularRotation.X, ref _cubeTransforms[_selectedObject].Scale.X);

            var selectedPosition = _cubeTransforms[_selectedObject].Position.OpenTk2Sys();
            ImGui.DragFloat3("Position", ref selectedPosition, 0.01f);
            _cubeTransforms[_selectedObject].Position = selectedPosition.Sys2OpenTk();
            var selectedRotation = _cubeTransforms[_selectedObject].EularRotation.OpenTk2Sys();
            ImGui.DragFloat3("Rotation", ref selectedRotation, 1.0f, -180, 180);
            _cubeTransforms[_selectedObject].EularRotation = selectedRotation.Sys2OpenTk();
            var selectedScale = _cubeTransforms[_selectedObject].Scale.OpenTk2Sys();
            ImGui.DragFloat3("Scale", ref selectedScale, 0.1f);
            _cubeTransforms[_selectedObject].Scale = selectedScale.Sys2OpenTk();

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

        _camera.AspectRatio = e.Width / (float)e.Height;
        _controller.WindowResized(e.Width, e.Height);
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        base.OnKeyDown(e);

        switch (e.Key)
        {
            case Keys.D1:
                _gizmoOperation = OPERATION.TRANSLATE;
                break;
            case Keys.D2:
                _gizmoOperation = OPERATION.ROTATE;
                break;
            case Keys.D3:
                _gizmoOperation = OPERATION.SCALE;
                break;
            case Keys.K:
                _gizmoMode = _gizmoMode == MODE.LOCAL ? MODE.WORLD : MODE.LOCAL;
                break;
            case Keys.J:
                CursorState = _camera.LockCamera();
                break;
            case Keys.Escape:
                Close();
                break;
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
        var format = PixelFormat.Red;

        if (image.Comp == ColorComponents.RedGreenBlue)
            format = PixelFormat.Rgb;
        else if (image.Comp == ColorComponents.RedGreenBlueAlpha)
            format = PixelFormat.Rgba;

        GL.BindTexture(TextureTarget.Texture2D, textureId);
        GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)format, image.Width, image.Height, 0, format,
            PixelType.UnsignedByte, image.Data);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        return textureId;
    }

    private Vector3 ScreenPosToWorldRay()
    {
        Vector4 rayStartNdc = new Vector4(
            (MouseState.X / ClientSize.X - 0.5f) * 2.0f,
            (MouseState.Y / ClientSize.Y - 0.5f) * 2.0f,
            -1.0f, // The near plane maps to Z=-1 in Normalized Device Coordinates
            1.0f
        );
        Vector4 rayEndNdc = new Vector4(
            (MouseState.X / ClientSize.X - 0.5f) * 2.0f,
            (MouseState.Y / ClientSize.Y - 0.5f) * 2.0f,
            0.0f,
            1.0f
        );

        var inverseProjectionMatrix = _camera.Projection.Transposed().Inverted();

        var inverseViewMatrix = _camera.View.Transposed().Inverted();

        //var rayStartCamera = inverseProjectionMatrix * rayStartNdc;
        //rayStartCamera /= rayStartCamera.W;
        //var rayStartWorld = inverseViewMatrix * rayStartCamera;
        //rayStartWorld /= rayStartWorld.W;
        //var rayEndCamera = inverseProjectionMatrix * rayEndNdc;
        //rayEndCamera /= rayEndCamera.W;
        //var rayEndWorld = inverseViewMatrix * rayEndCamera;
        //rayEndWorld /= rayEndWorld.W;


        // Faster way (just one inverse)
        Matrix4 M = Matrix4.Invert(_camera.Projection * _camera.View);
        Vector4 rayStartWorld = M * rayStartNdc; rayStartWorld/=rayStartWorld.W;
        Vector4 rayEndWorld   = M * rayEndNdc  ; rayEndWorld  /=rayEndWorld.W;


        Vector3 rayDirWorld = new Vector3(rayEndWorld - rayStartWorld).Normalized();

        return rayDirWorld;
    }

    private bool RayCastObject(Vector3 rayOrigin,
        Vector3 rayDirection,
        Vector3 aabbMin,
        Vector3 aabbMax,
        Matrix4 modelMatrix)
    {
        var tMin = 0.0f;
        var tMax = 100000.0f;

        var oobPositionWorldspace = new Vector3(modelMatrix.Row3.X, modelMatrix.Row3.Y, modelMatrix.Row3.Z);

        var delta = oobPositionWorldspace - rayOrigin;

        var xAxis = new Vector3(modelMatrix.Row0.X, modelMatrix.Row0.Y, modelMatrix.Row0.Z);
        var e = Vector3.Dot(xAxis, delta);
        var f = Vector3.Dot(rayDirection, xAxis);

        var t1 = (e + aabbMin.X) / f;
        var t2 = (e + aabbMax.X) / f;

        if (t1 > t2)
        {
            (t1, t2) = (t2, t1);
        }

        if (t2 < tMax)
        {
            tMax = t2;
        }

        if (t1 > tMin)
        {
            tMin = t1;
        }

        return tMax > tMin;
    }
}