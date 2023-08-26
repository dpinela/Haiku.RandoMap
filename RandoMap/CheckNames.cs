using Rando = Haiku.Rando;
using RChecks = Haiku.Rando.Checks;
using RTopology = Haiku.Rando.Topology;
using CType = Haiku.Rando.Topology.CheckType;
using Collections = System.Collections.Generic;

namespace RandoMap
{
    internal static class CheckNames
    {
        public static string ItemName(this RTopology.RandoCheck check)
        {
            var key = RChecks.UIDef.Of(check).Name;
            var baseName = string.IsNullOrEmpty(key) ? "???" : LocalizationSystem.GetLocalizedValue(key);
            return check.Type switch
            {
                CType.Item when check.CheckId == 7 => $"Green {baseName}",
                CType.Item when check.CheckId == 8 => $"Red {baseName}",
                CType.ChipSlot => $"{ChipSlotColor(check.CheckId)} {baseName}",
                CType.MapDisruptor => $"{baseName} ({RoomName(check.SceneId)})",
                _ => baseName
            };
        }

        private static string ChipSlotColor(int slotId)
        {
            var color = GameManager.instance.chipSlot[slotId].chipSlotColor;
            return string.IsNullOrEmpty(color) ? "???" : color.Substring(0, 1).ToUpper() + color.Substring(1).ToLower();
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
                baseName += $" ({RoomName(check.SceneId)})";
            }
            return baseName;
        }

        private const int RustedKey = 0;
        private const int CapsuleFragment = 3;

        public static bool IsUnique(this RTopology.RandoCheck check) =>
            check.Type switch
            {
                CType.Wrench or CType.Bulblet or CType.Ability or CType.Chip or
                CType.TrainStation or CType.FireRes or CType.WaterRes or
                CType.MapDisruptor or CType.MapMarker or CType.Lever => true,
                CType.Item => !(check.CheckId == RustedKey || check.CheckId == CapsuleFragment),
                _ => false
            };

        private static readonly Collections.Dictionary<int, string> roomNames = new()
        {
            {11, "Abandoned Wastes-Coyote Key"},
            {91, "Pinion's Expanse-Above Mischevious"},
            {98, "Pinion's Expanse-Mischevious"},
            {121, "Water Ducts-Electrified Corridor"},
            {128, "Water Ducts-Helm Crawlers"},
            {216, "Blazing Furnace-Corkscrews"},
            {81, "Central Core-Bulb Town"},
            {54, "Last Bunker-Bomb Floor"},
            {232, "Blazing Furnace-Disruptor"},
            {212, "Factory Facility-Big Brother"},
            {108, "Incinerator Burner-Disruptor by Flamethrower Tower"},
            {102, "Incinerator Burner-Entrance from Transformers"},
            {104, "Incinerator Burner-Spinning Flamethrowers"},
            {118, "Water Ducts-Zigzag Waterfalls"},
            {143, "Forgotten Ruins-Overgrown Computer"},
            {138, "Forgotten Ruins-First Tree"},
            {200, "Blazing Furnace-Proton"},
            {223, "Water Ducts-Elder Snailbot"},
            {213, "Blazing Furnace-Entrance"},
            {76, "Central Core-Former Core"},
            {210, "Water Ducts-Wheel"},
            {134, "Forgotten Ruins-Tower"},
            {45, "Last Bunker-Spike Gauntlet"},
            {56, "Last Bunker-Bell"},
            {160, "Sunken Wastes-Entrance from Research Lab"},
            {162, "Sunken Wastes-Past Big Drill"},
            {130, "Water Ducts-Power Cell Monument"},
            {132, "Forgotten Ruins-Below Lune"},
            {186, "Factory Facility-Lava Cistern"},
            {126, "Water Ducts-Pipe Entrance"},
            {156, "Sunken Wastes-Suspended Saw Station"},
            {192, "Factory Facility-Frog Entrance"},
            {228, "Central Core-Power Cell Monument"},
            {43, "Last Bunker-Electrified Floor"},
            {141, "Forgotten Ruins-Blink Gauntlet"},
            {175, "Factory Facility-Saw Tunnel"},
            {170, "Central Core-Quatern's Project"},
            {50, "Last Bunker-World Map Monitor"},
            {159, "Research Lab-Coolant Pipes"},
            {182, "Factory Facility-Twin Levers"},
            {34, "Last Bunker-Scrapfall"},
            {26, "Abandoned Wastes-Outside Train"},
            {181, "Factory Facility-Lunging Saws"},
            {189, "Factory Facility-Core Pillar"},
            {168, "Central Core-Past Echo's Shop"},
            {68, "Central Core-Past Car Battery"},
            {82, "Central Core-Donut Transformers"},
            {183, "Factory Facility-Right of Floor 02"},
            {80, "Central Core-Mainframe to Bulb Town"},
            {25, "Abandoned Wastes-Past Tire Mother"},
            {88, "Pinion's Expanse-Clock"},
            {21, "Abandoned Wastes-Gauntlet Past Magnet"},
            {198, "Ruined Surface-Factory Chimneys"},
            {83, "Central Core-Bulb Hive"},
            {103, "Incinerator Burner-Bunsen Burner"},
            {105, "Incinerator Burner-Flame Lore"},
            {117, "Water Ducts-Elevator"},
            {127, "Water Ducts-Lower Repair Station"},
            {139, "Forgotten Ruins-Entrance to Lost Archives"},
            {140, "Forgotten Ruins-Repair Station by Blink Gauntlet"},
            {152, "Sunken Wastes-Amplifying Transputer"},
            {191, "Factory Facility-Money Shrine Bot"},
            {206, "Water Ducts-Helm-Crawlers Refuge"},
            {230, "Factory Facility-Big Brother Entrance"},
            {58, "Last Bunker-Bunk Beds"},
            {59, "Last Bunker-Basketball Court"},
            {30, "Last Bunker-Candles"},
            {32, "Last Bunker-Hand Print Monument"},
            {144, "Lost Archives"},
            {164, "Forgotten Ruins-Research Lab"},
            {221, "Factory Facility-Slate"}
        };

        private static string RoomName(int sceneId) =>
            roomNames.TryGetValue(sceneId, out var name) ? name : "???";
    }
}