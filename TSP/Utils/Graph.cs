using System;
using System.Collections.Generic;

namespace TSP.Utils
{
    public class Graph
    {
        private readonly int _size;
        private int[,] _graph;

        public Graph(int size, int[,] graph)
        {
            _graph = graph;
            _size = size;
        }

        public void SetGraph(int[,] graph)
        {
            _graph = graph;
        }

        public int GetCost(int[] solution)
        {
            var cost = 0;
            for (var i = 0; i < solution.Length - 1; i++) cost += GetWeight(solution[i], solution[i + 1]);
            cost += GetWeight(solution[^1], solution[0]);
            return cost;
        }

        public int[,] GetGraph()
        {
            return _graph;
        }

        public int GetSize()
        {
            return _size;
        }

        public int GetWeight(int a, int b)
        {
            return _graph[a, b];
        }
        
        public static void PrintSolution(IEnumerable<int> solution)
        {
            foreach (var i1 in solution) Console.Write("{0} ", i1);
            Console.WriteLine();
        }


        public void Print()
        {
            for (var i = 0; i < _size; i++)
            {
                for (var j = 0; j < _size; j++) Console.Write(_graph[i, j] + " ");
                Console.WriteLine();
            }
        }
    }
}