using BepInEx;

namespace RandoMap
{
    [BepInPlugin("haiku.randomap", "Haiku Rando Map", "1.0.0.0")]
    [BepInDependency("haiku.mapi", "1.0")]
    [BepInDependency("haiku.rando", "0.2.0.0")]
    public class RandoMapPlugin : BaseUnityPlugin
    {
        public void Start()
        {
            Logger.LogInfo("Rando Map - Under Construction");
        }
    }
}