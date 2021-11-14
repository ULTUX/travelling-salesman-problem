using System;
using System.Collections.Generic;
using Priority_Queue;

namespace TSP.Algorithms
{
    public class BranchNBound
    {
        private static readonly IComparer<BranchNBoundNode> nodeComparer =
            Comparer<BranchNBoundNode>.Create((x, y) => x.Cost > y.Cost ? 1 : x.Cost < y.Cost ? -1 : 0);

        private readonly Graph _graph;
        private readonly int _startVertex;

        private readonly SimplePriorityQueue<BranchNBoundNode, BranchNBoundNode> queue = new(nodeComparer);


        public BranchNBound(Graph graph, int startVertex)
        {
            _graph = graph;
            _startVertex = startVertex;
        }

        public void Start()
        {
            var path = Solve();
            if (path == null)
            {
                Console.WriteLine("Bad graph, could not calculate minimum path cost.");
                return;
            }

            var cost = 0;

            path.ForEach(tuple =>
            {
                Console.WriteLine(tuple.Item1 + " --- " + tuple.Item2);
                cost += _graph.GetWeight(tuple.Item1, tuple.Item2);
            });

            Console.WriteLine("Path cost: {0}", cost);
        }

        private List<(int, int)> Solve()
        {
            var (cost, reduced) = MinimizeMatrix(_graph.GetGraph());

            var root = new BranchNBoundNode(reduced, cost, _startVertex, 0, new List<(int, int)>(), null);


            queue.Enqueue(root, root);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                var v = node.Vertex;
                if (node.Level == _graph.GetSize() - 1)
                {
                    //Reached leaf node, update upper bound.
                    node.Path.Add((v, 0));
                    Console.WriteLine(node.Cost);
                    return node.Path;
                }

                var children = FindPossibleChildren(node);
                children.ForEach(boundNode => queue.Enqueue(boundNode, boundNode));
            }

            return null;
        }


        private (int, int[,]) MinimizeMatrix(int[,] graph)
        {
            (var minR, var minGraphR) = minimizeColsOrRows(graph, true);
            (var minC, var minGraphC) = minimizeColsOrRows(minGraphR, false);
            return (minR + minC, minGraphC);
        }


        private List<BranchNBoundNode> FindPossibleChildren(BranchNBoundNode parent)
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

        private (int, int[,]) minimizeColsOrRows(int[,] graph, bool isRow)
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

        private class BranchNBoundNode
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