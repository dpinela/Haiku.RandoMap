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
            var initRoom = rando.StartScene ?? Rando.SpecialScenes.GameStart;
            var initNode = rando.Topology.Scenes[initRoom].Nodes.First(
                n => n is RTopology.TransitionNode tn &&
                (tn.Type == TType.RepairStation || tn.Type == TType.HaikuWake));
            reachableNodes = Flood(initNode, new(rando.Logic)
            {
                Context = new PlayerInventoryRandoContext(rando.Topology.Checks)
            });
            obtainedChecks = new Collections.HashSet<RTopology.RandoCheck>(rando.CheckMapping
                .Where(m => RChecks.CheckManager.AlreadyGotCheck(m.Value))
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
            RandoMapPlugin.LogInfo("obtained checks: " + string.Join(", ", obtainedChecks));
            foreach (var c in ReachableUnvisitedChecks())
            {
                RandoMapPlugin.LogInfo("helperlogging " + c.ToString());
                w.WriteLine(c.LocationName());
            }
        }
    }
}