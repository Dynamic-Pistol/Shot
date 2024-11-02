
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using Shot.Main;

var nativeWinSettings = NativeWindowSettings.Default;
nativeWinSettings.APIVersion = new Version(4, 6);
nativeWinSettings.ClientSize = new Vector2i(800, 600);
nativeWinSettings.Title = "VoxMod";
using var game = new ShotGame(GameWindowSettings.Default, nativeWinSettings);
game.Run();
