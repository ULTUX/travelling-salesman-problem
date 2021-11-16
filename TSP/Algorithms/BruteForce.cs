using System;
using TSP.Utils;

namespace TSP.Algorithms
{
    /**
     * Simple brute force algorithm for finding best path in Travelling Salesman Problem.
     * Algorithm uses next permutation algorithm for finding every possible permutation of graph nodes.
     */
    internal class BruteForce : TspAlgorithm
    {
        private int[] _permutation;

        public BruteForce(Graph graph, int startVertex) : base(graph, startVertex)
        {
        }

        /**
         * Start the algorithm.
         */
        public override void Start()
        {
            var minPath = int.MaxValue;
            var minPermutation = new int[_graph.GetSize() - 1];

            _permutation = new int[_graph.GetSize() - 1];

            var curIndex = 0;
            for (var i = 0; i < _graph.GetSize(); i++)
            {
                if (i == _startVertex) continue;
                _permutation[curIndex] = i;
                curIndex++;
            }

            do
            {
                var currentPathLen = 0;

                var prev = _startVertex;

                foreach (var t in _permutation)
                {
                    currentPathLen += _graph.GetWeight(prev, t);
                    prev = t;
                }

                currentPathLen += _graph.GetWeight(prev, _startVertex);

                if (currentPathLen >= minPath) continue;
                minPath = currentPathLen;
                minPermutation = (int[]) _permutation.Clone();
            } while (GetNextPermutation());

            //Finished algorithm, print results

            if (!IsBenchmark) Console.Write("Found min path. Path: " + _startVertex + " ");
            if (!IsBenchmark)
                foreach (var t in minPermutation)
                    Console.Write(t + " ");
            if (!IsBenchmark) Console.WriteLine("\nPath length: " + minPath + ".");
        }

        /**
         * Get next permutation based on decreasing suffix in current permutation.
         */
        private bool GetNextPermutation()
        {
            if (_permutation is not {Length: > 1}) return false;

            //That will be index of last element before decreasing section (element to permute first)
            var lastIncreasing = _permutation.Length - 2;

            //Find this index
            while (lastIncreasing >= 0)
            {
                if (_permutation[lastIncreasing] < _permutation[lastIncreasing + 1]) break;
                lastIncreasing--;
            }

            if (lastIncreasing < 0) return false;

            //Now, when we have last increasing element, find its successor
            var successor = _permutation.Length - 1;

            for (var i = _permutation.Length - 1; i > lastIncreasing; i--)
            {
                if (_permutation[i] <= _permutation[lastIncreasing]) continue;
                successor = i;
                break;
            }

            //Swap successor with last increasing element
            (_permutation[successor], _permutation[lastIncreasing]) =
                (_permutation[lastIncreasing], _permutation[successor]);

            //Reverse order of decreasing elements (revert back to first permutation)
            //TODO: Verify whether this function works properly
            var l = 1;
            for (var i = _permutation.Length - 1;
                i > (_permutation.Length - 1 - lastIncreasing) / 2 + lastIncreasing;
                i--)
            {
                (_permutation[i], _permutation[lastIncreasing + l]) =
                    (_permutation[lastIncreasing + l], _permutation[i]);
                l++;
            }

            return true;
        }
    }
}