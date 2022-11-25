using BepInEx;

namespace RandoMap
{
    [BepInPlugin("haiku.randomap", "Haiku Rando Map", "1.0.0.0")]
    [BepInDependency("haiku.mapi", "1.0")]
    [BepInDependency("haiku.rando", "0.2.0.0")]
    public class RandoMapPlugin : BaseUnityPlugin
    {
        private static RandoMapPlugin Instance;

        internal static void LogError(string msg)
        {
            Instance.Logger.LogError(msg);
        }

        private Settings settings;

        public void Start()
        {
            Instance = this;
            Logger.LogInfo("Rando Map - Under Construction");
            settings = new(Config);
        }
    }
}