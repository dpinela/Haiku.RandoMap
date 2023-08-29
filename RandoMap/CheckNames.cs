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

        private static string[] InitRoomNameTable()
        {
            var t = new string[272];
            t[11] = "Abandoned Wastes-Coyote Key";
            t[91] = "Pinion's Expanse-Above Mischevious";
            t[98] = "Pinion's Expanse-Mischevious";
            t[121] = "Water Ducts-Electrified Corridor";
            t[128] = "Water Ducts-Helm Crawlers";
            t[216] = "Blazing Furnace-Corkscrews";
            t[81] = "Central Core-Bulb Town";
            t[54] = "Last Bunker-Bomb Floor";
            t[232] = "Blazing Furnace-Disruptor";
            t[212] = "Factory Facility-Big Brother";
            t[108] = "Incinerator Burner-Disruptor by Flamethrower Tower";
            t[102] = "Incinerator Burner-Entrance from Transformers";
            t[104] = "Incinerator Burner-Spinning Flamethrowers";
            t[118] = "Water Ducts-Zigzag Waterfalls";
            t[143] = "Forgotten Ruins-Overgrown Computer";
            t[138] = "Forgotten Ruins-First Tree";
            t[200] = "Blazing Furnace-Proton";
            t[223] = "Water Ducts-Elder Snailbot";
            t[213] = "Blazing Furnace-Entrance";
            t[76] = "Central Core-Former Core";
            t[210] = "Water Ducts-Wheel";
            t[134] = "Forgotten Ruins-Tower";
            t[45] = "Last Bunker-Spike Gauntlet";
            t[56] = "Last Bunker-Bell";
            t[160] = "Sunken Wastes-Entrance from Research Lab";
            t[162] = "Sunken Wastes-Past Big Drill";
            t[130] = "Water Ducts-Power Cell Monument";
            t[132] = "Forgotten Ruins-Below Lune";
            t[186] = "Factory Facility-Lava Cistern";
            t[126] = "Water Ducts-Pipe Entrance";
            t[156] = "Sunken Wastes-Suspended Saw Station";
            t[192] = "Factory Facility-Frog Entrance";
            t[228] = "Central Core-Power Cell Monument";
            t[43] = "Last Bunker-Electrified Floor";
            t[141] = "Forgotten Ruins-Blink Gauntlet";
            t[175] = "Factory Facility-Saw Tunnel";
            t[170] = "Central Core-Quatern's Project";
            t[50] = "Last Bunker-World Map Monitor";
            t[159] = "Research Lab-Coolant Pipes";
            t[182] = "Factory Facility-Twin Levers";
            t[34] = "Last Bunker-Scrapfall";
            t[26] = "Abandoned Wastes-Outside Train";
            t[181] = "Factory Facility-Lunging Saws";
            t[189] = "Factory Facility-Core Pillar";
            t[168] = "Central Core-Past Echo's Shop";
            t[68] = "Central Core-Past Car Battery";
            t[82] = "Central Core-Donut Transformers";
            t[183] = "Factory Facility-Right of Floor 02";
            t[80] = "Central Core-Mainframe to Bulb Town";
            t[25] = "Abandoned Wastes-Past Tire Mother";
            t[88] = "Pinion's Expanse-Clock";
            t[21] = "Abandoned Wastes-Gauntlet Past Magnet";
            t[198] = "Ruined Surface-Factory Chimneys";
            t[83] = "Central Core-Bulb Hive";
            t[103] = "Incinerator Burner-Bunsen Burner";
            t[105] = "Incinerator Burner-Flame Lore";
            t[117] = "Water Ducts-Elevator";
            t[127] = "Water Ducts-Lower Repair Station";
            t[139] = "Forgotten Ruins-Entrance to Lost Archives";
            t[140] = "Forgotten Ruins-Repair Station by Blink Gauntlet";
            t[152] = "Sunken Wastes-Amplifying Transputer";
            t[191] = "Factory Facility-Money Shrine Bot";
            t[206] = "Water Ducts-Helm-Crawlers Refuge";
            t[230] = "Factory Facility-Big Brother Entrance";
            t[58] = "Last Bunker-Bunk Beds";
            t[59] = "Last Bunker-Basketball Court";
            t[30] = "Last Bunker-Candles";
            t[32] = "Last Bunker-Hand Print Monument";
            t[144] = "Lost Archives";
            t[164] = "Forgotten Ruins-Research Lab";
            t[221] = "Factory Facility-Slate";
            t[73] = "Central Core-Bomb Pickup";
            t[147] = "Forgotten Ruins-Left Entrance";
            t[10] = "Abandoned Wastes-Wake Area";
            t[12] = "Abandoned Wastes-Key Gate";
            t[13] = "Abandoned Wastes-Nondescript Room";
            t[14] = "Abandoned Wastes-Flyer Climb";
            t[16] = "Abandoned Wastes-Pinion";
            t[23] = "Abandoned Wastes-Barrier Right of Wake Area";
            t[31] = "Last Bunker-Broken Pillars";
            t[33] = "Last Bunker-Traps";
            t[46] = "Last Bunker-Kitchen Door";
            t[61] = "Central Core-Bomb Floor to Bunker";
            t[63] = "Central Core-Bulbs Left of Mainframe";
            t[64] = "Central Core-Cell Towers";
            t[65] = "Central Core-Bulbs and Lasers";
            t[67] = "Central Core-Train";
            t[70] = "Central Core-Laser Exit to Ducts";
            t[74] = "Central Core-Bomb Floor Stack";
            t[75] = "Central Core-Electron Repair Station";
            t[101] = "Incinerator Burner-Gyro-Accelerator";
            t[106] = "Incinerator Burner-Tunnels";
            t[109] = "Incinerator Burner-Top Exit";
            t[113] = "Water Ducts-Light Repair Station";
            t[114] = "Water Ducts-Entrance to Train";
            t[133] = "Forgotten Ruins-Above First Tree";
            t[151] = "Sunken Wastes-Tunnel to Elevator Shaft";
            t[154] = "Sunken Wastes-Derelict Hole";
            t[165] = "Sunken Wastes-Elevator";
            t[173] = "Factory Facility-Floor 00 Entrance";
            t[177] = "Factory Facility-Auto Repair";
            t[185] = "Factory Facility-Walls of Saws";
            t[187] = "Factory Facility-Floor 03 Frog Hall";
            t[201] = "Last Bunker-Neutron";
            t[207] = "Water Ducts-Helm-Crawlers Refuge Exit Shaft";
            t[229] = "Factory Facility-Frogs and Saws";
            t[234] = "Last Bunker-Door to Old Arcadia";
            t[246] = "Old Arcadia-Upload Machine";
            t[252] = "Old Arcadia-Reactor Core";
            t[259] = "Old Arcadia-Elevator to Abandoned Wastes";
            t[269] = "Old Arcadia-Rock Saw";
            t[84] = "Central Core-Electron";
            t[122] = "Water Ducts-Right of Bomb Floor";
            return t;
        }
        
        private static readonly string[] roomNames = InitRoomNameTable();

        private static string RoomName(int sceneId) =>
            sceneId >= 0 && sceneId < roomNames.Length && roomNames[sceneId] != null ?
                roomNames[sceneId] :
                $"Scene {sceneId}";
    }
}