using System;

namespace TSP.Utils
{
    public class RandomGraphGenerator
    {
        private readonly bool _isSymmetric;
        private readonly int _maxCost;
        private readonly int _minCost;
        private readonly Random _randomGenerator;
        private readonly int _size;

        public RandomGraphGenerator(bool isSymmetric, int minCost, int maxCost, int size, int? seed)
        {
            _isSymmetric = isSymmetric;
            _minCost = minCost;
            _maxCost = maxCost;
            _size = size;
            _randomGenerator = seed != null ? new Random((int) seed) : new Random();
        }

        public Graph GenerateRandomGraph()
        {
            var adjMatrix = new int[_size, _size];
            if (_isSymmetric)
                for (var i = 0; i < _size; i++)
                for (var j = i; j < _size; j++)
                {
                    if (i == j)
                    {
                        adjMatrix[i, j] = -1;
                        continue;
                    }

                    adjMatrix[i, j] = _randomGenerator.Next(_minCost, _maxCost);
                    adjMatrix[j, i] = adjMatrix[i, j];
                }
            else
                for (var i = 0; i < _size; i++)
                for (var j = 0; j < _size; j++)
                {
                    if (i == j)
                    {
                        adjMatrix[i, j] = -1;
                        continue;
                    }

                    adjMatrix[i, j] = _randomGenerator.Next(_minCost, _maxCost);
                }

            var graph = new Graph(_size, adjMatrix);
            return graph;
        }
    }
}