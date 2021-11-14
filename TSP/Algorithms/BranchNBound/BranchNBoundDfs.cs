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

        public void Start()
        {
            var path = Solve();
            if (path == null)
            {
                Console.WriteLine("Bad graph, could not calculate minimum path cost.");
                return;
            }

            var cost = 0;

            Console.Write(StartVertex + " ");
            path.ForEach(tuple =>
            {
                Console.Write(tuple.Item2 + " ");
                cost += Graph.GetWeight(tuple.Item1, tuple.Item2);
            });
            Console.WriteLine();

            Console.WriteLine("Path cost: {0}", cost);
        }

        private void Dfs(BranchNBoundNode v)
        {
            if (v.Cost > _upperBound) return;
            if (v.Level == Graph.GetSize() - 1)
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
            var (cost, reduced) = MinimizeMatrix(Graph.GetGraph());

            var root = new BranchNBoundNode(reduced, cost, StartVertex, 0, new List<(int, int)>(), null);

            Dfs(root);
            return _minNode.Path;
        }
    }
}