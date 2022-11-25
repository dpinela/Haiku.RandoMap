using BepInEx.Configuration;
using Modding;

namespace RandoMap
{
    internal class Settings
    {
        public ConfigEntry<bool> ShowMap;

        private const string MainGroup = "";

        public Settings(ConfigFile config)
        {
            ShowMap = config.Bind(MainGroup, "Show Map", false);
            ConfigManagerUtil.createButton(config, () => RandoMapPlugin.LogError("Spoiler log not implemented yet"), MainGroup, "Show Spoiler Log", "A list of all checks and the items they contain");
        }
    }
}