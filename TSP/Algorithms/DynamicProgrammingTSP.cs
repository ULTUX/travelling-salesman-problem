using System;
using System.Collections.Generic;
using System.Linq;

namespace TSP
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
            var adjMatrix = _graph.GetGraph();
            var size = adjMatrix.GetLength(0);

            var pathCost = new int[size, (int) Math.Pow(2, size)];
                
            // Initialize memo array (this array will store all sub-paths).
            // Second dimension of this array will be used as binary number that stores current state of every node.
            // Prepare the array by filling it with infinity.
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < pathCost.GetLength(1); j++)
                {
                    pathCost[i,j] = int.MaxValue;
                }
            }
                
            // Calculate and write to array cost to get to every node from start node
            for (var i = 0; i < size; i++)
            {
                if (i==_startVertex) continue;
                    
                // Store cost of getting from start to each node i
                // 1 << _startVertex | 1 << i means that binary representation of second argument should be equal 
                // to binary number with only bits _startVertex and i set to 1.
                pathCost[i, (1 << _startVertex) | (1 << i)] = adjMatrix[_startVertex, i];
            }

            for (var i = 3; i <= size; i++)
            {

                var combinationList = new List<int>();
                Combinations(0, 0, i, size, combinationList);
                foreach (var combination in combinationList.Where(combination => !IsNotPresent(_startVertex, combination)))
                {
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
                            if (p==_startVertex || p == n || IsNotPresent(p, combination)) continue;
                            var tempDist = pathCost[p, oldState] + adjMatrix[p, n];
                            minPath = Math.Min(tempDist, minPath);
                        }
                        pathCost[n, combination] = minPath;
                    }
                }

            }
            
            // Finished algorithm, find min cost in memo table
            var endStateMask = (1 << size) - 1;
            
            var minCost = int.MaxValue;

            for (var i = 0; i < size; i++)
            {
                if (i == _startVertex) continue;

                var currCost = pathCost[i, endStateMask] + adjMatrix[i,_startVertex];
                minCost = Math.Min(currCost, minCost);
            }
            
            Console.WriteLine("Min cost found is: {0}.",minCost);
            
            // Display the path
            var prevIndex = _startVertex;
            var path = new int[size + 1];

            for (var i = size - 1; i >= 1; i--)
            {
                var index = -1;
                for (var j = 0; j < size; j++)
                {
                    // Find the index that is closest to end node
                    if (j == _startVertex || IsNotPresent(j, endStateMask)) continue;
                    if (index == -1) index = j;
                    
                    // Check what is closer to last index: j node or last node
                    if (pathCost[index,endStateMask] + adjMatrix[index,prevIndex] <
                        pathCost[j,endStateMask] + adjMatrix[j,prevIndex]) index = j;
                }

                path[i] = index;
                
                // Remove found index from mask
                endStateMask ^= (1 << index);
                prevIndex = index;
            }
            
            Console.WriteLine("Path:");
            
            path.ToList().ForEach(i =>
            {
                Console.Write(i+" ");
            });
            Console.WriteLine("\nFinished DP Traveling Salesman algorithm.");
        }

        // Check whether given element has its corresponding bit switched in binary representation of second argument (One-hot representation).
        private static bool IsNotPresent(int a, int set)
        {
            return ((1 << a) & set) == 0;
        }

        // Generate a set of different numbers (of length len) whose binary representation only contain pre-set number of bits (setN argument).
        private static void Combinations(int set, int at, int setN, int len, ICollection<int> subsets)
        {
            var leftElem = len - at;
            if (leftElem < setN) return;

            if (setN == 0)
            {
                subsets.Add(set);
            }
            else
            {
                for (int i = at; i < len; i++)
                {
                    set ^= (1 << i);
                    Combinations(set, i+1, setN - 1, len, subsets);
                    
                    // Revert variable back and try different bit
                    set ^= (1 << i);

                }
            }
        }
    }
}