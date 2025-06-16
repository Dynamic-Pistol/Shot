
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Shot.Main;

var nativeWinSettings = NativeWindowSettings.Default;
nativeWinSettings.APIVersion = new Version(3, 3);
nativeWinSettings.ClientSize = new Vector2i(800, 600);
nativeWinSettings.Title = "Shot Game";


using var game = new ShotGame(GameWindowSettings.Default, nativeWinSettings);
game.Run();

