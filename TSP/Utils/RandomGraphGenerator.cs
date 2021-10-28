using System;

namespace TSP
{
    public class RandomGraphGenerator
    {
        private readonly bool _isSymmetric;
        private int _minCost;
        private int _maxCost;
        private int _size;
        private Random _randomGenerator;

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
            int[,] adjMatrix = new int[_size,_size];
            if (_isSymmetric)
            {
                for (int i = 0; i < _size; i++)
                {
                    for (int j = i; j < _size; j++)
                    {
                        if (i == j)
                        {
                            adjMatrix[i,j] = -1;
                            continue;
                        }
                        adjMatrix[i, j] = _randomGenerator.Next(_minCost, _maxCost);
                        adjMatrix[j, i] = adjMatrix[i, j];
                    }
                }
            }
            else
            {
                for (int i = 0; i < _size; i++)
                {
                    for (int j = 0; j < _size; j++)
                    {
                        if (i == j)
                        {
                            adjMatrix[i,j] = -1;
                            continue;
                        }
                        adjMatrix[i, j] = _randomGenerator.Next(_minCost, _maxCost);
                    }
                }
            }

            Graph graph = new Graph(_size, adjMatrix);
            return graph;
        }
        
    }
}