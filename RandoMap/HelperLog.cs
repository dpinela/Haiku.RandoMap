using Rando = Haiku.Rando;
using RChecks = Haiku.Rando.Checks;
using RLogic = Haiku.Rando.Logic;
using RTopology = Haiku.Rando.Topology;
using TType = Haiku.Rando.Topology.TransitionType;
using IO = System.IO;
using Collections = System.Collections.Generic;
using static System.Linq.Enumerable;

namespace RandoMap
{
    internal class HelperLog
    {
        public static HelperLog? Generate()
        {
            var rando = RChecks.CheckManager.Instance.Randomizer;
            if (rando == null)
            {
                return null;
            }
            try
            {
                return new(rando);
            }
            catch (System.Exception err)
            {
                RandoMapPlugin.LogError(err.ToString());
                return null;
            }
        }

        private Collections.ISet<RTopology.IRandoNode> reachableNodes;
        private Collections.ISet<RTopology.RandoCheck> obtainedChecks;

        public HelperLog(RLogic.CheckRandomizer rando)
        {
            var initNode = rando.StartScene is int sceneId ?
                rando.Topology.Scenes[sceneId].Nodes.First(
                    n => n is RTopology.TransitionNode tn && tn.Type == TType.RepairStation) :
                rando.Topology.Scenes[Rando.SpecialScenes.GameStart].Nodes.First(
                    n => n is RTopology.TransitionNode tn && tn.Type == TType.HaikuWake);
            // It may be necessary to run the search multiple times in order to take
            // into account reachable vanilla checks - for example, vanilla levers
            // that can be opened with current movement - so that it is possible to
            // search past edges gated by those checks.
            var startingSymbols = new ushort[(int)RLogic.LogicSymbol.False];
            int formerlyReachableNodes;
            do
            {
                formerlyReachableNodes = reachableNodes == null ? 0 : reachableNodes.Count();
                reachableNodes = Flood(initNode, new(rando.Logic)
                {
                    Context = new PlayerInventoryRandoContext(rando.Topology.Checks, startingSymbols)
                });

                // Look for vanilla checks and bosses that are reachable.
                var bossTransitions = RLogic.CheckRandomizer.BossTransitions();
                Array.Clear(startingSymbols, 0, startingSymbols.Length);
                foreach (var rn in reachableNodes)
                {
                    if (rn is RTopology.RandoCheck rc)
                    {
                        if (!rando.CheckMapping.ContainsKey(rc))
                        {
                            var sym = RLogic.LogicEvaluator.SymbolForCheck(rc);
                            startingSymbols[(int)sym]++;
                        }
                    }
                    else if (rn is RTopology.TransitionNode tn)
                    {
                        var k1 = (tn.SceneId1, tn.Alias1);
                        if (bossTransitions.TryGetValue(k1, out var sym1))
                        {
                            startingSymbols[(int)sym1]++;
                            bossTransitions.Remove(k1);
                        }
                        var k2 = (tn.SceneId2, tn.Alias2);
                        if (bossTransitions.TryGetValue(k2, out var sym2))
                        {
                            startingSymbols[(int)sym2]++;
                            bossTransitions.Remove(k2);
                        }
                    }
                }
            } while (reachableNodes.Count > formerlyReachableNodes);
            
            obtainedChecks = new Collections.HashSet<RTopology.RandoCheck>(rando.CheckMapping
                .Where(m => m.Value.Obtained())
                .Select(m => m.Key));
            obtainedChecks.UnionWith(rando.Topology.Checks
                .Where(rc => !rando.CheckMapping.ContainsKey(rc)));
        }

        private static Collections.ISet<RTopology.IRandoNode> Flood(RTopology.IRandoNode node, RLogic.LogicEvaluator logicEval)
        {
            var unexplored = new Collections.Queue<RTopology.IRandoNode>();
            unexplored.Enqueue(node);
            var reachable = new Collections.HashSet<RTopology.IRandoNode>();
            while (unexplored.Count > 0)
            {
                var currentNode = unexplored.Dequeue();
                reachable.Add(currentNode);
                foreach (var edge in currentNode.Outgoing)
                {
                    if (logicEval.CanTraverse(edge) && !reachable.Contains(edge.Destination) && !unexplored.Contains(edge.Destination))
                    {
                        unexplored.Enqueue(edge.Destination);
                    }
                }
            }
            return reachable;
        }

        public Collections.List<RTopology.RandoCheck> ReachableUnvisitedChecks()
        {
            return reachableNodes.OfType<RTopology.RandoCheck>().Except(obtainedChecks).ToList();
        }

        public void WriteToCSV(IO.TextWriter w)
        {
            foreach (var c in ReachableUnvisitedChecks())
            {
                w.WriteLine(c.LocationName());
            }
        }
    }
}