using System;
using System.Collections.Generic;
using System.Linq;
using TSP.Utils;

namespace TSP.Algorithms
{
    /**
     * Algorithm based on: https://www.youtube.com/watch?v=cY4HiiFHO1o
     */
    public class DynamicProgrammingTsp
    {
        private readonly Graph _graph;
        private readonly int _startVertex;

        public DynamicProgrammingTsp(Graph graph, int startVertex)
        {
            _graph = graph;
            _startVertex = startVertex;
        }

        // Start the algorithm
        public void Start()
        {
            var size = _graph.GetSize();

            //Store all subpaths as binary numbers representation
            var pathCost = new int[size, (int) Math.Pow(2, size)];

            // Initialize memory array (this array will store all sub-paths).
            // Second dimension of this array will be used as binary number that stores current state of every node.
            // Prepare the array by filling it with infinity.
            for (var i = 0; i < size; i++)
            for (var j = 0; j < pathCost.GetLength(1); j++)
                pathCost[i, j] = int.MaxValue;

            // Calculate and write to array cost to get to every node from start node
            for (var i = 0; i < size; i++)
            {
                if (i == _startVertex) continue;

                // Store cost of getting from start to each node i
                // 1 << _startVertex | 1 << i means that binary representation of second argument should be equal 
                // to binary number with only bits _startVertex and i set to 1.
                pathCost[i, (1 << _startVertex) | (1 << i)] = _graph.GetWeight(_startVertex, i);
            }

            for (var i = 3; i <= size; i++)
            {
                var combinationList = new List<int>();
                FindCombinations(0, 0, i, size, combinationList);
                foreach (var combination in combinationList.Where(combination =>
                        !IsNotPresent(_startVertex, combination)))
                    // Find node that was added using combination
                    for (var n = 0; n < size; n++)
                    {
                        // Find next state that was not present before and was generated using combination function
                        if (n == _startVertex || IsNotPresent(n, combination)) continue;

                        // Previous state, without next node that we just found
                        var oldState = combination ^ (1 << n);
                        var minPath = int.MaxValue;


                        for (var p = 0; p < size; p++)
                        {
                            if (p == _startVertex || p == n || IsNotPresent(p, combination)) continue;
                            var tempDist = pathCost[p, oldState] + _graph.GetWeight(p, n);
                            minPath = Math.Min(tempDist, minPath);
                        }

                        pathCost[n, combination] = minPath;
                    }
            }

            // Finished algorithm, find min cost in pathCost array
            var endStateMask = (1 << size) - 1;

            var minCost = int.MaxValue;

            for (var i = 0; i < size; i++)
            {
                if (i == _startVertex) continue;

                var currCost = pathCost[i, endStateMask] + _graph.GetWeight(i, _startVertex);
                minCost = Math.Min(currCost, minCost);
            }

            Console.WriteLine("Min cost found is: {0}.", minCost);

            // Display the path
            var prevIndex = _startVertex;
            var path = new int[size + 1];
            endStateMask = (1 << size) - 1;

            for (var i = size - 1; i >= 1; i--)
            {
                int? index = null;
                for (var j = 0; j < size; j++)
                {
                    // Find the index that is closest to end node
                    if (j == _startVertex || IsNotPresent(j, endStateMask)) continue;
                    index ??= j;

                    // Check what is closer to last index: j node or last node
                    if (pathCost[(int) index, endStateMask] + _graph.GetWeight((int) index, prevIndex) >
                        pathCost[j, endStateMask] + _graph.GetWeight(j, prevIndex)) index = j;
                }

                path[i] = (int) index;

                // Remove found index from mask
                endStateMask ^= 1 << (int) index;
                prevIndex = (int) index;
            }

            path[0] = _startVertex;
            path[size] = _startVertex;

            Console.WriteLine("Path:");

            path.ToList().ForEach(i => { Console.Write(i + " "); });

            Console.WriteLine("\nFinished DP Traveling Salesman algorithm.");
        }

        // Check whether given element has its corresponding bit switched in binary representation of second argument (One-hot representation).
        private static bool IsNotPresent(int a, int set)
        {
            //Check is a bit is switched on in set
            var isPresent = (1 << a) & set;

            //If is number is equal to 0 ==> bit is not switched
            return isPresent == 0;
        }

        // Generate a set of different numbers (of length len) whose binary representation only contain pre-set number of bits (setN argument).
        private static void FindCombinations(int set, int at, int setN, int len, ICollection<int> subsets)
        {
            //If all bits were added, add set into subsets list
            if (setN == 0)
            {
                subsets.Add(set);
                return;
            }

            for (var i = at; i < len; i++)
            {
                //Set this i-th bit in the set 
                set ^= 1 << i;
                FindCombinations(set, i + 1, setN - 1, len, subsets);

                // Revert variable back and try different bit
                set ^= 1 << i;
            }
        }
    }
}