using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using TSP.Utils;

namespace TSP.Algorithms
{
    public class TabuSearch : TspAlgorithm
    {
        private readonly Random _random = new();
        private readonly Action<int[], int, int> _swapMethod;
        private readonly Collection<(int, int)> _tabuList = new();
        private readonly int _timeConstraint;
        private int[] _bestSolution;
        private int _bestSolutionCost;
        private int _tabuListSize;
        private double _timeTookMillis = -1;

        public TabuSearch(Graph graph, int timeInMillis, SwapMethod method, bool isBench) : base(graph, 0)
        {
            _timeConstraint = timeInMillis;

            _swapMethod = method switch
            {
                (SwapMethod.TwoOperatorSwap) => SwapToNeighbourVertexSwap,
                (SwapMethod.TwoEdgeSwap) => SwapToNeighbourEdgeSwap,
                (SwapMethod.InsertSwap) => SwapToNeighbourInsert,
                _ => SwapToNeighbourVertexSwap
            };

            IsBenchmark = isBench;
        }

        /**
         * Empty constructor for Benchmark class
         */
        public TabuSearch()
        {
        }


        public override void Start()
        {
            var currentSol = GetFirstSolution();
            _tabuListSize = 20;
            _bestSolutionCost = _graph.GetCost(currentSol);
            _bestSolution = new int[currentSol.Length];
            Array.Copy(currentSol, _bestSolution, currentSol.Length);

            var numIterationsNotChanged = 0;
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            while (stopWatch.Elapsed.TotalMilliseconds <= _timeConstraint)
            {
                var hasChanged = FindNextNeighbour(currentSol);
                if (hasChanged)
                {
                    var currSolCost = _graph.GetCost(currentSol);

                    if (currSolCost < _bestSolutionCost)
                    {
                        _bestSolutionCost = currSolCost;
                        Array.Copy(currentSol, _bestSolution, currentSol.Length);
                        if (!IsBenchmark) Console.WriteLine("Found new best solution: " + _bestSolutionCost);
                        _timeTookMillis = stopWatch.Elapsed.TotalMilliseconds;
                        numIterationsNotChanged = 0;
                    }
                    else
                    {
                        numIterationsNotChanged++;
                    }
                }


                if (numIterationsNotChanged >= 10 * _graph.GetSize())
                {
                    currentSol = GetFirstSolution();
                    numIterationsNotChanged = 0;
                }
            }

            stopWatch.Stop();
            if (!IsBenchmark)
            {
                Console.WriteLine("Solution cost: " + _bestSolutionCost);
                Graph.PrintSolution(_bestSolution);
            }
        }

        public (int costFound, int[] solutionFound, double timeTookMillis) GetResults()
        {
            return (_bestSolutionCost, _bestSolution, _timeTookMillis);
        }

        private bool FindNextNeighbour(int[] solution)
        {
            var bestLocalCost = int.MaxValue;
            var bestLocalSolution = new int[solution.Length];
            var currSol = new int[solution.Length];
            Array.Copy(solution, currSol, solution.Length);
            var bestSolution = _bestSolutionCost;
            var minVal = 0;

            var tabuIndexes = (0, 0);

            var solutionFound = false;

            for (var i = 0; i < _graph.GetSize() - 1; i++)
            for (var j = i + 1; j < _graph.GetSize(); j++)
            {
                _swapMethod(currSol, i, j);
                var currCost = _graph.GetCost(currSol);

                if (bestLocalCost - currCost > minVal)
                    if (!IsPresentInTabu(i, j) || currCost < bestSolution)
                    {
                        minVal = bestLocalCost - currCost;
                        bestSolution = currCost;
                        bestLocalCost = currCost;
                        tabuIndexes = (i, j);
                        Array.Copy(currSol, bestLocalSolution, currSol.Length);
                        solutionFound = true;
                    }

                SwapToNeighbourVertexSwap(currSol, i, j);
            }

            if (!solutionFound) return false;
            PushToTabu(tabuIndexes.Item1, tabuIndexes.Item2);
            Array.Copy(bestLocalSolution, solution, bestLocalSolution.Length);
            return true;
        }


        private int[] FindBestNeighbourhoodMethod(int[] currSol, int i, int j)
        {
            var secondSol = new int[currSol.Length];
            var newSolution = new int[currSol.Length];
            Array.Copy(currSol, secondSol, currSol.Length);
            Array.Copy(currSol, newSolution, currSol.Length);


            SwapToNeighbourVertexSwap(newSolution, i, j);
            var minNeighbour = new int[currSol.Length];
            Array.Copy(newSolution, minNeighbour, minNeighbour.Length);
            var minNeighbourCost = _graph.GetCost(minNeighbour);

            Array.Copy(currSol, secondSol, currSol.Length);
            SwapToNeighbourEdgeSwap(secondSol, i, j);
            return _graph.GetCost(secondSol) < minNeighbourCost ? secondSol : minNeighbour;
        }

        private void SwapToNeighbourVertexSwap(int[] solution, int i, int j)
        {
            (solution[i], solution[j]) = (solution[j], solution[i]);
        }

        private void SwapToNeighbourEdgeSwap(int[] solution, int i, int j)
        {
            var min = Math.Min(i, j);
            var max = Math.Max(i, j);
            Array.Reverse(solution, min, max - min + 1);
        }

        private void SwapToNeighbourInsert(int[] solution, int i, int j)
        {
            var listSol = solution.ToList();
            int temp = listSol[i];
            listSol.RemoveAt(i);
            listSol.Insert(j, temp);
            Array.Copy(listSol.ToArray(), solution, solution.Length);
        }

        private bool CheckIfPathIsValid(int[] solution)
        {
            for (var i = 1; i < solution.Length; i++)
                if (_graph.GetWeight(i - 1, i) == 0)
                {
                    if (!IsBenchmark) Console.WriteLine("NOT VALID PATH FOUND!");
                    return false;
                }

            return true;
        }

        private void PushToTabu(int val1, int val2)
        {
            _tabuList.Add((val1, val2));
            if (_tabuList.Count > _tabuListSize)
                while (_tabuList.Count > _tabuListSize)
                    _tabuList.RemoveAt(0);
        }


        private bool IsPresentInTabu(int val1, int val2)
        {
            for (var i = 0; i < _tabuList.Count; i++)
                if (_tabuList[i].Item1 == val1 || _tabuList[i].Item1 == val2 || _tabuList[i].Item2 == val1 ||
                    _tabuList[i].Item2 == val2)
                    return true;
            return _tabuList.Contains((val1, val2)) || _tabuList.Contains((val2, val1));
        }


        private int[] GetFirstSolution()
        {
            var solution = new List<int>();
            var searchSpace = new List<int>();

            for (var i = 0; i < _graph.GetSize(); i++)
            {
                searchSpace.Add(i);
            }

            //var currDist = 0;
            var graph = _graph.GetGraph();
            var startVertex = _random.Next(0, _graph.GetSize());
            solution.Add(startVertex);
            searchSpace.Remove(startVertex);
            var prevNode = startVertex;
            while (searchSpace.Count != 0)
            {
                var min = (idx: 0, val: int.MaxValue);
                foreach (var t in searchSpace.Where(t => graph[prevNode, t] < min.val))
                    min = (t, graph[prevNode, t]);

                if (min.val == int.MaxValue) continue;
                solution.Add(min.idx);
                prevNode = min.idx;
                searchSpace.Remove(prevNode);
            }

            return solution.ToArray();
        }
    }

    public enum SwapMethod
    {
        TwoOperatorSwap,
        TwoEdgeSwap,
        InsertSwap
    }
}