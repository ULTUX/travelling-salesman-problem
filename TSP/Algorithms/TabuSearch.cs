using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TSP.Utils;

namespace TSP.Algorithms
{
    public class TabuSearch : TspAlgorithm
    {
        private readonly Random random = new();
        private readonly Collection<(int, int)> tabuList = new();
        private int[] bestSolution;
        private int bestSolutionCost;
        private int tabuListSize;


        public TabuSearch(Graph graph, int startVertex) : base(graph, startVertex)
        {
        }

        public TabuSearch()
        {
        }


        public override void Start()
        {
            var currentSol = GetFirstSolution();
            // currentSol = GetNewRandomSolution(currentSol);
            tabuListSize = (int) (0.5 * _graph.GetSize());
            bestSolutionCost = _graph.GetCost(currentSol, _startVertex);
            bestSolution = new int[currentSol.Length];
            Array.Copy(currentSol, bestSolution, currentSol.Length);
            var swapFunc = new Action<int[], int, int>(SwapToNeighbour);

            var i = 0;
            Console.WriteLine("Current cost: {0}", bestSolutionCost);
            const int NUM_ITERATIONS = 10000;
            var numIterationsNotChanged = 0;
            while (i < NUM_ITERATIONS)
            {
                var hasChanged = FindNextNeighbour(currentSol, swapFunc);
                if (!hasChanged)
                {
                    numIterationsNotChanged++;
                }
                else
                {
                    var currSolCost = _graph.GetCost(currentSol, _startVertex);

                    if (currSolCost < bestSolutionCost)
                    {
                        bestSolutionCost = currSolCost;
                        Array.Copy(currentSol, bestSolution, currentSol.Length);
                        Console.WriteLine("Found new best solution: " + bestSolutionCost);
                    }
                }

                if (numIterationsNotChanged == _graph.GetSize())
                {
                    // currentSol = GetNewRandomSolution(currentSol);
                    numIterationsNotChanged = 0;
                    if (swapFunc == SwapToNeighbour)
                        swapFunc = SwapToNeighbour2;
                    else swapFunc = SwapToNeighbour;
                }

                i++;
            }

            Console.WriteLine("Solution cost: " + bestSolutionCost);
            printSolution(bestSolution);
        }

        //It is Tabu search algorithm for TSP problem. Write a function that finds best neighbor of the current solution and returns it.
        //The function should return new array that is a neighbor of the current solution. The array should have two elements swapped.
        private bool FindNextNeighbour(int[] solution, Action<int[], int, int> swapFunction)
        {
            var bestLocalCost = _graph.GetCost(solution, _startVertex);
            int[] bestLocalSolution = null;
            var currSol = new int[solution.Length];
            Array.Copy(solution, currSol, solution.Length);
            var bestSolution = bestSolutionCost;
            var minVal = 0;

            var tabuIndexes = (0, 0);

            for (var i = 1; i < _graph.GetSize() - 1; i++)
            for (var j = i + 1; j < _graph.GetSize(); j++)
            {
                swapFunction(currSol, i, j);
                var currCost = _graph.GetCost(currSol, _startVertex);

                if (bestLocalCost - currCost > minVal)
                    if (!IsPresentInTabu(i, j) || currCost < bestSolution)
                    {
                        minVal = currCost;
                        bestSolution = currCost;
                        bestLocalCost = currCost;
                        bestLocalSolution = new int[currSol.Length];
                        tabuIndexes = (i, j);
                        Array.Copy(currSol, bestLocalSolution, currSol.Length);
                    }

                SwapToNeighbour(currSol, i, j);
            }

            if (bestLocalSolution == null) return false;
            pushToTabu(tabuIndexes.Item1, tabuIndexes.Item2);
            Array.Copy(bestLocalSolution, solution, bestLocalSolution.Length);
            return true;
        }

        private int[] FindBestNeighbourhoodMethod(int[] currSol, int i, int j)
        {
            var secondSol = new int[currSol.Length];
            var newSolution = new int[currSol.Length];
            Array.Copy(currSol, secondSol, currSol.Length);
            Array.Copy(currSol, newSolution, currSol.Length);


            SwapToNeighbour(newSolution, i, j);
            var minNeighbour = new int[currSol.Length];
            Array.Copy(newSolution, minNeighbour, minNeighbour.Length);
            var minNeighbourCost = _graph.GetCost(minNeighbour, _startVertex);

            Array.Copy(currSol, secondSol, currSol.Length);
            SwapToNeighbour2(secondSol, i, j);
            return _graph.GetCost(secondSol, _startVertex) < minNeighbourCost ? secondSol : minNeighbour;
        }

        private void SwapToNeighbour(int[] solution, int i, int j)
        {
            (solution[i], solution[j]) = (solution[j], solution[i]);
        }

        private void SwapToNeighbour3(int[] solution, int i, int j)
        {
            var alg = random.Next(0, 1);

            if (alg == 0)
            {
                (solution[i], solution[j]) = (solution[j], solution[i]);
            }
            else
            {
                var min = Math.Min(i, j);
                var max = Math.Max(i, j);
                Array.Reverse(solution, min, max - min + 1);
            }
        }

        private void SwapToNeighbour2(int[] solution, int i, int j)
        {
            var min = Math.Min(i, j);
            var max = Math.Max(i, j);
            Array.Reverse(solution, min, max - min + 1);
        }

        private bool CheckIfPathIsValid(int[] solution)
        {
            for (var i = 1; i < solution.Length; i++)
                if (_graph.GetWeight(i - 1, i) == 0)
                {
                    Console.WriteLine("NOT VALID PATH FOUND!");
                    return false;
                }

            return true;
        }

        private void printSolution(IEnumerable<int> solution)
        {
            var prev = 0;
            foreach (var i1 in solution) Console.Write("{0} ", i1);
            Console.WriteLine();
        }

        private void pushToTabu(int val1, int val2)
        {
            tabuList.Add((val1, val2));
            if (tabuList.Count > tabuListSize)
                while (tabuList.Count > tabuListSize)
                    tabuList.RemoveAt(0);
        }

        private bool IsPresentInTabu(int val1, int val2)
        {
            for (var i = 0; i < tabuList.Count; i++)
                if (tabuList[i].Item1 == val1 || tabuList[i].Item1 == val2 || tabuList[i].Item2 == val1 ||
                    tabuList[i].Item2 == val2)
                    return true;
            return tabuList.Contains((val1, val2)) || tabuList.Contains((val2, val1));
        }


        private int[] GetFirstSolution()
        {
            var solution = new Collection<int>();
            //var currDist = 0;
            var prevNode = 0;
            var graph = _graph.GetGraph();
            solution.Add(0);
            while (solution.Count != _graph.GetSize())
            {
                var min = (idx: 0, val: int.MaxValue);
                for (var i = 1; i < _graph.GetSize(); i++)
                    if (!solution.Contains(i) && graph[prevNode, i] < min.val && graph[prevNode, i] != 0)
                        min = (i, graph[prevNode, i]);
                if (min.val == int.MaxValue) continue;
                solution.Add(min.idx);
                prevNode = min.idx;
            }

            return solution.ToArray();
        }

        private int[] GetNewRandomSolution(int[] currSolution)
        {
            var bestRandomSolution = new int[currSolution.Length];
            bestRandomSolution = currSolution.Skip(1).ToArray();
            bestRandomSolution = bestRandomSolution.OrderBy(i => random.Next()).ToArray();
            bestRandomSolution = bestRandomSolution.Prepend(0).ToArray();
            var bestRandomSolutionCost = _graph.GetCost(bestRandomSolution, _startVertex);

            for (var i = 0; i < 190; i++)
            {
                currSolution = currSolution.Skip(1).ToArray();
                currSolution = currSolution.OrderBy(i => random.Next()).ToArray();
                currSolution = currSolution.Prepend(0).ToArray();
                var currSolCost = _graph.GetCost(currSolution, _startVertex);
                if (currSolCost < bestRandomSolutionCost)
                {
                    Array.Copy(currSolution, bestRandomSolution, currSolution.Length);
                    bestRandomSolutionCost = currSolCost;
                }
            }

            return bestRandomSolution;
        }
    }
}