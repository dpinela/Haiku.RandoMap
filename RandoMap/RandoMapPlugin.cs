using Bep = BepInEx;
using RChecks = Haiku.Rando.Checks;

namespace RandoMap
{
    [Bep.BepInPlugin("haiku.randomap", "Haiku Rando Map", "1.0.0.0")]
    [Bep.BepInDependency("haiku.mapi", "1.0")]
    [Bep.BepInDependency("haiku.rando", "0.2.0.0")]
    public class RandoMapPlugin : Bep.BaseUnityPlugin
    {
        private static RandoMapPlugin? Instance;

        internal static void LogError(string msg)
        {
            Instance!.Logger.LogError(msg);
        }

        internal static void LogInfo(string msg)
        {
            Instance!.Logger.LogInfo(msg);
        }

        private Settings? settings;

        public void Start()
        {
            Instance = this;
            Logger.LogInfo("Rando Map - Under Construction");
            settings = new(Config);
        }
    }
}