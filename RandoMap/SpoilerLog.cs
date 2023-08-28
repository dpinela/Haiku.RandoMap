using IO = System.IO;
using Collections = System.Collections.Generic;
using static System.Linq.Enumerable;
using RChecks = Haiku.Rando.Checks;
using RTopology = Haiku.Rando.Topology;

namespace RandoMap
{
    internal class SpoilerLog
    {
        private Collections.List<(string Item, string Location)> entries;

        public static SpoilerLog? Generate()
        {
            var rando = RChecks.CheckManager.Instance.Randomizer;
            if (rando == null)
            {
                return null;
            }
            return new(rando.CheckMapping);
        }

        public SpoilerLog(Collections.IReadOnlyDictionary<RTopology.RandoCheck, RTopology.RandoCheck> mapping)
        {
            entries = mapping
                .Where(entry => !IsDeletedCheck(entry.Value))
                .Select(entry => (entry.Value.ItemName(), entry.Key.LocationName()))
                .ToList();
        }

        // Rando uses 999999 for consolidated scrap piles, but we can be a little more permissive.
        private static bool IsDeletedCheck(RTopology.RandoCheck rc) =>
            rc.Type == RTopology.CheckType.Filler && rc.CheckId > 900000;

        public void WriteToCSV(IO.TextWriter w)
        {
            foreach (var entry in entries)
            {
                w.WriteLine(entry.Item + "," + entry.Location);
            }
        }
    }
}