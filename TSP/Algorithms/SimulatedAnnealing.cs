using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TSP.Utils;

namespace TSP.Algorithms
{
    public class SimulatedAnnealing: TspAlgorithm
    {
        private int size;
        
        private float temp;
        private float tempModifier;
        private float initialTemp;
        private float endingTemp;

        private int maxIterations;
        private int currentIteration;
        
        private int[] bestSolution;
        private float _bestFitness = int.MaxValue;

        private int[] _currSolution;
        private float _currentFitness;

        private readonly Random _randGen = new Random();
        private int[] GetNewRandomSolution(int[] currSolution)
        {
            var bestRandomSolution = new int[currSolution.Length];
            bestRandomSolution = bestRandomSolution.OrderBy(item => _randGen.Next()).ToArray();
            var bestRandomSolutionCost = _graph.GetCost(bestRandomSolution);

            for (var i = 0; i < 19; i++)
            {
                currSolution = currSolution.OrderBy(item => _randGen.Next()).ToArray();
                var currSolCost = _graph.GetCost(currSolution);
                if (currSolCost < bestRandomSolutionCost)
                {
                    Array.Copy(currSolution, bestRandomSolution, currSolution.Length);
                    bestRandomSolutionCost = currSolCost;
                }
            }

            return bestRandomSolution;
        }
        public override void Start()
        {
            size = _graph.GetSize();
            tempModifier = 0.999995f;
            endingTemp = 0.000000000000001f;
            maxIterations = 100000000;
            currentIteration = 0;
            _currSolution = GetFirstSolution();
            // _currSolution = GetNewRandomSolution(_currSolution);
            _currentFitness = _graph.GetCost(_currSolution);
            bestSolution = _currSolution.Clone() as int[];
            temp = 1000000;
            
            
            Console.WriteLine("Starting anneal...");
            while (temp > endingTemp && currentIteration < maxIterations)
            {
                float startFitness = _bestFitness;
                var candidate = _currSolution.Clone() as int[];
                var firstSwapPoint = _randGen.Next(2, size);
                var secondSwapPoint = _randGen.Next(0, size - firstSwapPoint);
                SwapToNeighbour2(candidate, firstSwapPoint, secondSwapPoint);
                SetNewSolution(candidate);

                if (_bestFitness != startFitness)
                {
                    Console.WriteLine("Changed solution to: {0}", _bestFitness);
                }
                
                currentIteration++;
                temp *= tempModifier;
            }
            
            Console.WriteLine("Best solution found: {0}, solution: ", _bestFitness);
            PrintSolution(bestSolution);
        }
        
        private void PrintSolution(IEnumerable<int> solution)
        {
            var prev = 0;
            foreach (var i1 in solution) Console.Write("{0} ", i1);
            Console.WriteLine();
        }
        
        private void SwapToNeighbour2(int[] solution, int i, int j)
        {
            var min = Math.Min(i, j);
            var max = Math.Max(i, j);
            Array.Reverse(solution, min, max - min + 1);
        }

        public SimulatedAnnealing(Graph graph, int startVertex) : base(graph, startVertex)
        {
        }

        private double GetAcceptProbability(float fitness)
        {
            return Math.Exp(-1*Math.Abs(fitness - _currentFitness) / temp);
        }

        private void SetNewSolution(int[] solutionCandidate)
        {
            int solutionCandidateFitness = _graph.GetCost(solutionCandidate);
            if (solutionCandidateFitness < _currentFitness)
            {
                _currentFitness = solutionCandidateFitness;
                Array.Copy(solutionCandidate, _currSolution, size);
                if (!(_currentFitness < _bestFitness)) return;
                _bestFitness = _currentFitness;
                Array.Copy(_currSolution, bestSolution, size);
            }

            else if (_randGen.NextDouble() < GetAcceptProbability(solutionCandidateFitness))
            {
                _currentFitness = solutionCandidateFitness;
                Array.Copy(solutionCandidate, _currSolution, size);
            }

        }
        
        

        public SimulatedAnnealing(int[] currSolution, int[] bestSolution)
        {
            this._currSolution = currSolution;
            this.bestSolution = bestSolution;
        }
        
        private int[] GetFirstSolution()
        {
            var solution = new List<int>();
            var searchSpace = new List<int>();

            for (int i = 0; i < size; i++)
            {
                searchSpace.Add(i);
            }
            
            //var currDist = 0;
            var graph = _graph.GetGraph();
            int startVertex = _randGen.Next(0, size);
            solution.Add(startVertex);
            searchSpace.Remove(startVertex);
            var prevNode = startVertex;
            while (searchSpace.Count != 0)
            {
                var min = (idx: 0, val: int.MaxValue);
                for (var i = 0; i < searchSpace.Count; i++)
                    if (graph[prevNode, searchSpace[i]] < min.val)
                        min = (searchSpace[i], graph[prevNode, searchSpace[i]]);
                if (min.val == int.MaxValue) continue;
                solution.Add(min.idx);
                prevNode = min.idx;
                searchSpace.Remove(prevNode);
            }

            return solution.ToArray();
        }
    }
}