using System.Collections.Generic;
using TSP.Utils;

namespace TSP.Algorithms.BranchNBound
{
    public abstract class BranchNBound : TspAlgorithm
    {
        protected BranchNBound(Graph graph, int startVertex) : base(graph, startVertex)
        {
        }

        protected BranchNBound()
        {
        }

        protected static (int, int[,]) MinimizeMatrix(int[,] graph)
        {
            var (minR, minGraphR) = MinimizeColsOrRows(graph, true);
            var (minC, minGraphC) = MinimizeColsOrRows(minGraphR, false);
            return (minR + minC, minGraphC);
        }

        protected List<BranchNBoundNode> FindPossibleChildren(BranchNBoundNode parent)
        {
            var children = new List<BranchNBoundNode>();

            for (var i = 0; i < _graph.GetSize(); i++)
            {
                if (parent.Reduced[parent.Vertex, i] == -1) continue;

                var (cost, childReduced) = MinimizeMatrix(parent.Reduced, parent.Vertex, i);
                var childNode =
                    new BranchNBoundNode(childReduced, cost + parent.Cost + parent.Reduced[parent.Vertex, i], i,
                        parent.Level + 1, parent.Path, parent.Vertex);
                children.Add(childNode);
            }

            return children;
        }

        private (int, int[,]) MinimizeMatrix(int[,] graph, int parent, int child)
        {
            var graph2 = (int[,]) graph.Clone();

            for (var i = 0; i < graph2.GetLength(0); i++)
            {
                graph2[parent, i] = -1;
                graph2[i, child] = -1;
            }

            graph2[child, parent] = -1;
            graph2[child, 0] = -1;

            return MinimizeMatrix(graph2);
        }

        private static (int, int[,]) MinimizeColsOrRows(int[,] graph, bool isRow)
        {
            var minimized = (int[,]) graph.Clone();
            var min = 0;

            for (var i = 0; i < graph.GetLength(0); i++)
            {
                var minVal = int.MaxValue;

                for (var j = 0; j < graph.GetLength(0); j++)
                {
                    var k = isRow ? i : j;
                    var l = isRow ? j : i;
                    if (graph[k, l] < minVal && graph[k, l] != -1) minVal = graph[k, l];
                }

                if (minVal is not 0 and not int.MaxValue)
                {
                    min += minVal;
                    for (var j = 0; j < minimized.GetLength(0); j++)
                    {
                        var k = isRow ? i : j;
                        var l = isRow ? j : i;
                        if (minimized[k, l] is not 0 and not -1 and not int.MaxValue) minimized[k, l] -= minVal;
                    }
                }
            }

            return (min, minimized);
        }

        protected class BranchNBoundNode
        {
            public readonly int Cost;
            public readonly int Level;
            public readonly List<(int, int)> Path;
            public readonly int[,] Reduced;
            public readonly int Vertex;

            public BranchNBoundNode(int[,] reduced, int cost, int vertex, int level, List<(int, int)> path, int? parent)
            {
                Path = new List<(int, int)>(path);
                if (level != 0 && parent != null) Path.Add(((int) parent, vertex));
                Reduced = (int[,]) reduced.Clone();
                Level = level;
                Vertex = vertex;
                Cost = cost;
            }
        }
    }
}