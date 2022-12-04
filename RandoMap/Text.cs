namespace RandoMap
{
    internal static class Text
    {
        public const string _RANDO_CHECK = "_RANDO_CHECK";

        private static void Load(On.LocalizationSystem.orig_Init orig)
        {
            orig();
            LocalizationSystem.localizedEN[_RANDO_CHECK] = "Rando Check";
        }

        public static void Hook()
        {
            On.LocalizationSystem.Init += Load;
        }
    }
}