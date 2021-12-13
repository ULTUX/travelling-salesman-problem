using System;
using System.Collections.Generic;
using System.Diagnostics;
using TSP.Utils;

namespace TSP.Algorithms
{
    public class SimulatedAnnealing : TspAlgorithm
    {
        private readonly Random _randGen = new();

        private readonly Action _reduceTemperature;
        private readonly int _timeConstraint;
        private int _bestFitness = int.MaxValue;

        private int[] _bestSolution;
        private int _currentFitness;
        private int _currentIteration;

        private int[] _currSolution;
        private int _size;

        private double _temp;
        private double? _tempModifier;
        private double _timeTookMillis;

        public SimulatedAnnealing(Graph graph, int totalMillis, AnnealMethod method, float? tempModifier, bool isBench)
            : base(graph, 0)
        {
            _timeConstraint = totalMillis;
            _tempModifier = tempModifier;
            _reduceTemperature = method switch
            {
                AnnealMethod.Linear => DecreaseTempLinearly,
                AnnealMethod.Geometric => DecreaseTempGeometrically,
                AnnealMethod.SlowDecrease => DecreaseTempSlowDecrease,
                _ => DecreaseTempGeometrically
            };
            IsBenchmark = isBench;
        }


        public override void Start()
        {
            //Init fields, could not do that in constructor bcs one object of this class can be ran multiple times
            //with different input values (Benchmarking purposes).
            _size = _graph.GetSize();
            _tempModifier ??= 0.999999;
            _currentIteration = 0;
            _currSolution = GetFirstSolution();
            _currentFitness = _graph.GetCost(_currSolution);
            _bestSolution = _currSolution.Clone() as int[];
            _temp = 10000000;


            if (!IsBenchmark) Console.WriteLine("Starting anneal...");
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            while (stopWatch.Elapsed.TotalMilliseconds < _timeConstraint)
            {
                var startFitness = _bestFitness;
                var candidate = _currSolution.Clone() as int[];
                var firstSwapPoint = _randGen.Next(2, _size);
                var secondSwapPoint = _randGen.Next(0, _size - firstSwapPoint);
                SwapToNeighbour(candidate, firstSwapPoint, secondSwapPoint);
                SetNewSolution(candidate);

                if (_bestFitness < startFitness)
                {
                    if (!IsBenchmark) Console.WriteLine("Changed solution to: {0}", _bestFitness);
                    _timeTookMillis = stopWatch.Elapsed.TotalMilliseconds;
                }

                _currentIteration++;
                _reduceTemperature(); //Reduce temperature with selected annealing method.
            }

            stopWatch.Stop();

            if (!IsBenchmark)
            {
                Console.WriteLine("Best solution found: {0}, solution: ", _bestFitness);
                Graph.PrintSolution(_bestSolution);
            }
        }

        public (int costFound, int[] solutionFound, double timeTookMillis, double endTemperature) GetResults()
        {
            return (_bestFitness, _bestSolution, _timeTookMillis, _temp);
        }

        private void SwapToNeighbour(int[] solution, int i, int j)
        {
            (solution[i], solution[j]) = (solution[j], solution[i]);
        }

        private void SwapToNeighbour2(int[] solution, int i, int j)
        {
            var min = Math.Min(i, j);
            var max = Math.Max(i, j);
            Array.Reverse(solution, min, max - min + 1);
        }

        private double GetAcceptProbability(float fitness)
        {
            return Math.Exp(-1 * Math.Abs(fitness - _currentFitness) / _temp);
        }

        private void SetNewSolution(int[] solutionCandidate)
        {
            var solutionCandidateFitness = _graph.GetCost(solutionCandidate);
            if (solutionCandidateFitness < _currentFitness)
            {
                _currentFitness = solutionCandidateFitness;
                Array.Copy(solutionCandidate, _currSolution, _size);
                if (!(_currentFitness < _bestFitness)) return;
                _bestFitness = _currentFitness;
                Array.Copy(_currSolution, _bestSolution, _size);
            }

            else if (_randGen.NextDouble() < GetAcceptProbability(solutionCandidateFitness))
            {
                _currentFitness = solutionCandidateFitness;
                Array.Copy(solutionCandidate, _currSolution, _size);
            }
        }


        private int[] GetFirstSolution()
        {
            var solution = new List<int>();
            var searchSpace = new List<int>();

            for (int i = 0; i < _size; i++)
            {
                searchSpace.Add(i);
            }

            //var currDist = 0;
            var graph = _graph.GetGraph();
            var startVertex = _randGen.Next(0, _size);
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


        private void DecreaseTempLinearly()
        {
            if (_tempModifier != null) _temp -= 1 - (double) _tempModifier;
        }

        private void DecreaseTempSlowDecrease()
        {
            if (_tempModifier != null) _temp /= (1 + (double) _tempModifier * _temp);
        }

        private void DecreaseTempGeometrically()
        {
            if (_tempModifier != null) _temp *= (double) _tempModifier;
        }
    }

    public enum AnnealMethod
    {
        Linear,
        Geometric,
        SlowDecrease
    }
}