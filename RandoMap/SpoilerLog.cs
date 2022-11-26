using IO = System.IO;
using Collections = System.Collections.Generic;
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
            entries = new();
            foreach (var entry in mapping)
            {
                entries.Add((entry.Value.ItemName(), entry.Key.ItemName()));
            }
        }

        public void WriteToCSV(IO.TextWriter w)
        {
            foreach (var entry in entries)
            {
                w.WriteLine(entry.Item + "," + entry.Location);
            }
        }
    }
}