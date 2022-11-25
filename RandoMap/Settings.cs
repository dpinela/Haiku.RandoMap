using IO = System.IO;
using BepConfig = BepInEx.Configuration;
using MAPI = Modding;

namespace RandoMap
{
    internal class Settings
    {
        public BepConfig.ConfigEntry<bool> ShowMap;

        private const string MainGroup = "";

        public Settings(BepConfig.ConfigFile config)
        {
            ShowMap = config.Bind(MainGroup, "Show Map", false);
            MAPI.ConfigManagerUtil.createButton(config, MakeSpoilerLog, MainGroup, "Show Spoiler Log", "A list of all checks and the items they contain");
        }

        private static void MakeSpoilerLog()
        {
            var log = SpoilerLog.Generate();
            if (log == null)
            {
                RandoMapPlugin.LogInfo("spoiler log requested, but randomizer not active");
                return;
            }
            var asmloc = typeof(Settings).Assembly.Location;
            var logloc = IO.Path.Combine(asmloc, "..", "..", "RandoSpoilerLog.csv");
            using (var w = IO.File.CreateText(logloc))
            {
                log.WriteToCSV(w);
            }
        }
    }
}