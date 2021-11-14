using System;

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