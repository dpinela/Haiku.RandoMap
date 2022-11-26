using Rando = Haiku.Rando;
using RTopology = Haiku.Rando.Topology;
using CType = Haiku.Rando.Topology.CheckType;

namespace RandoMap
{
    internal static class CheckNames
    {
        public static string ItemName(this RTopology.RandoCheck check)
        {
            var key = check.Type switch
            {
                CType.Wrench => "_HEALING_WRENCH_TITLE",
                CType.Bulblet => "_LIGHT_BULB_TITLE",
                CType.Ability => Rando.HaikuResources.RefUnlockTutorial.abilities[check.CheckId].title,
                CType.Item => InventoryManager.instance.items[check.CheckId].itemName,
                CType.Chip => GameManager.instance.chip[check.CheckId].title,
                CType.ChipSlot => "_CHIP_SLOT",
                CType.MapDisruptor => "_DISRUPTOR",
                CType.PowerCell => "_POWERCELL",
                CType.Coolant => "_COOLANT_TITLE",
                CType.TrainStation => GameManager.instance.trainStations[check.CheckId].title,
                CType.FireRes => "_FIRE_RES_TITLE",
                CType.WaterRes => "_WATER_RES_TITLE",
                _ => "???"
            };
            return LocalizationSystem.GetLocalizedValue(key);
        }

        public static string LocationName(this RTopology.RandoCheck check)
        {
            if (check.IsShopItem)
            {
                return check.SceneId switch
                {
                    220 => "Echo Shop",
                    28 => "Sonnet Shop (Abandoned Wastes)",
                    9 => "Sonnet Shop (Train)",
                    224 => "Reaper Shop",
                    _ => "??? Shop"
                };
            }
            var baseName = check.ItemName();
            if (!check.IsUnique())
            {
                baseName += $" (Room {check.SceneId})";
            }
            return baseName;
        }

        private const int RustedKey = 0;
        private const int CapsuleFragment = 3;

        public static bool IsUnique(this RTopology.RandoCheck check) =>
            check.Type switch
            {
                CType.Wrench or CType.Bulblet or CType.Ability or CType.Chip or
                CType.TrainStation or CType.FireRes or CType.WaterRes => true,
                CType.Item => !(check.CheckId == RustedKey || check.CheckId == CapsuleFragment),
                _ => false
            };
    }
}