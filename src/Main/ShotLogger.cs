using Serilog;
using Serilog.Core;

namespace Shot.Main;

public static class ShotLogger
{
    private static readonly Logger Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File("log.txt").CreateLogger();

    public static void LogInfo(string msg, params object[] contexts) => Logger.Information(msg, contexts);
    public static void LogError(string msg, params object[] contexts) => Logger.Error(msg, contexts);
    public static void LogWarn(string msg, params object[] contexts) => Logger.Warning(msg, contexts);
}