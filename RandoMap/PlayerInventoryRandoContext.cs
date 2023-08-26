using RLogic = Haiku.Rando.Logic;
using RChecks = Haiku.Rando.Checks;
using RTopology = Haiku.Rando.Topology;
using Collections = System.Collections.Generic;
using static System.Linq.Enumerable;

namespace RandoMap
{
    // A ICheckRandoContext that answers queries based on the items currently held by
    // the player, as determined by rando itself.
    internal class PlayerInventoryRandoContext : RLogic.ICheckRandoContext
    {
        private const int NumSymbols = (int)RLogic.LogicSymbol.False;

        private readonly Collections.List<RTopology.RandoCheck>[] checksBySymbol;
        private readonly ushort[] startingSymbols;

        public PlayerInventoryRandoContext(Collections.IReadOnlyList<RTopology.RandoCheck> allChecks, ushort[] startingSymbols)
        {
            checksBySymbol = new Collections.List<RTopology.RandoCheck>[NumSymbols];
            this.startingSymbols = startingSymbols;
            for (var i = 0; i < NumSymbols; i++)
            {
                checksBySymbol[i] = new();
            }
            foreach (var check in allChecks)
            {
                var sym = RLogic.LogicEvaluator.SymbolForCheck(check);
                checksBySymbol[(int)sym].Add(check);
            }
        }

        public int GetCount(RLogic.LogicSymbol state) =>
            checksBySymbol[(int)state].Count(RChecks.CheckManager.AlreadyGotCheck) +
            startingSymbols[(int)state];
    }
}