using RLogic = Haiku.Rando.Logic;
using SNames = Haiku.Rando.Logic.LogicStateNames;
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
        private readonly Collections.Dictionary<string, Collections.List<RTopology.RandoCheck>> checksByStateName;
        private readonly Collections.ISet<RTopology.RandoCheck> startingChecks;

        public PlayerInventoryRandoContext(Collections.IReadOnlyList<RTopology.RandoCheck> allChecks, Collections.ISet<RTopology.RandoCheck> startingChecks)
        {
            checksByStateName = allChecks.GroupBy(RLogic.LogicEvaluator.GetStateName)
                .ToDictionary(g => g.Key, g => g.ToList());
            this.startingChecks = startingChecks;
        }

        public bool HasState(string state) => GetCount(state) > 0;

        public int GetCount(string state) =>
            checksByStateName.Where(e => e.Key.StartsWith(state))
                .SelectMany(e => e.Value)
                .Count(rc => RChecks.CheckManager.AlreadyGotCheck(rc) || startingChecks.Contains(rc));
    }
}