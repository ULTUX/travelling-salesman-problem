using System;
using System.Collections.Generic;
using Priority_Queue;
using TSP.Utils;

namespace TSP.Algorithms.BranchNBound
{
    public class BranchNBoundMin : BranchNBound
    {
        private SimplePriorityQueue<BranchNBoundNode, BranchNBoundNode> _queue;

        public BranchNBoundMin(Graph graph, int startVertex) : base(graph, startVertex)
        {
        }

        public BranchNBoundMin()
        {
        }


        public override void Start()
        {
            _queue = new SimplePriorityQueue<BranchNBoundNode, BranchNBoundNode>(Comparer<BranchNBoundNode>.Create(
                (x, y) =>
                    x.Cost > y.Cost ? 1 : x.Cost < y.Cost ? -1 : 0));
            var path = Solve();
            if (path == null)
            {
                if (!IsBenchmark) Console.WriteLine("Bad graph, could not calculate minimum path cost.");
                return;
            }

            var cost = 0;

            if (!IsBenchmark) Console.Write(_startVertex + " ");
            path.ForEach(tuple =>
            {
                if (!IsBenchmark) Console.Write(tuple.Item2 + " ");
                cost += _graph.GetWeight(tuple.Item1, tuple.Item2);
            });
            if (!IsBenchmark) Console.WriteLine();

            if (!IsBenchmark) Console.WriteLine("Path cost: {0}", cost);
        }

        private List<(int, int)> Solve()
        {
            var upperBound = int.MaxValue;
            BranchNBoundNode minNode = null;
            var (cost, reduced) = MinimizeMatrix(_graph.GetGraph());

            var root = new BranchNBoundNode(reduced, cost, _startVertex, 0, new List<(int, int)>(), null);


            _queue.Enqueue(root, root);

            while (_queue.Count > 0)
            {
                var node = _queue.Dequeue();
                if (node.Cost > upperBound) continue;
                var v = node.Vertex;
                if (node.Level == _graph.GetSize() - 1)
                {
                    //Reached leaf node, update upper bound.
                    node.Path.Add((v, 0));
                    upperBound = node.Cost;
                    minNode = node;
                }

                var children = FindPossibleChildren(node);
                children.ForEach(boundNode => _queue.Enqueue(boundNode, boundNode));
            }

            return minNode?.Path;
        }
    }
}