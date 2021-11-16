using System;
using System.Collections.Generic;
using TSP.Utils;

namespace TSP.Algorithms.BranchNBound
{
    public class BranchNBoundDfs : BranchNBound
    {
        private BranchNBoundNode _minNode;
        private int _upperBound = int.MaxValue;


        public BranchNBoundDfs(Graph graph, int startVertex) : base(graph, startVertex)
        {
        }

        public BranchNBoundDfs()
        {
        }

        public override void Start()
        {
            _upperBound = int.MaxValue;
            _minNode = null;
            var path = Solve();
            if (path == null)
            {
                Console.WriteLine("Bad graph, could not calculate minimum path cost.");
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

        private void Dfs(BranchNBoundNode v)
        {
            if (v.Cost > _upperBound) return;
            if (v.Level == _graph.GetSize() - 1)
            {
                //Reached leaf node, update upper bound.
                v.Path.Add((v.Vertex, 0));
                _upperBound = v.Cost;
                _minNode = v;
            }

            var children = FindPossibleChildren(v);
            children.ForEach(Dfs);
        }

        private List<(int, int)> Solve()
        {
            var (cost, reduced) = MinimizeMatrix(_graph.GetGraph());

            var root = new BranchNBoundNode(reduced, cost, _startVertex, 0, new List<(int, int)>(), null);

            Dfs(root);
            return _minNode.Path;
        }
    }
}