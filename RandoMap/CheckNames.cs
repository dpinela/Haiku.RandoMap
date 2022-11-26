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
    }
}