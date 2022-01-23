using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using TSP.Utils;

namespace TSP.Algorithms
{
    public class GeneticAlgorithm : TspAlgorithm
    {
        private readonly Random _randGen = new();

        private readonly int _timeConstraint;
        private int _bestCost = int.MaxValue;
        private int[] _bestSolution;

        private List<int[]> _population;
        private float[] _populationFitness;
        private int populationSize;
        private float mutationRate;
        private float crossoverRate;
        private CoMethod _coMethod;
        
        private double _timeTookMillis;
        
        
        public GeneticAlgorithm(Graph graph, int totalMillis, int populationSize, float crossoverRate, float mutationRate, CoMethod coMethod) : base(graph, 0)
        {
            this.populationSize = populationSize;
            _timeConstraint = totalMillis;
            this.mutationRate = mutationRate;
            this.crossoverRate = crossoverRate;
            _coMethod = coMethod;
            _populationFitness = new float[populationSize];
            _bestSolution = new int[graph.GetSize()];
            CreatePopulation();
        }

        public void test()
        {
            OrderedCo(new[] {1, 2, 3, 4, 5, 6, 7, 8, 9}, new[] {5, 3, 6, 7, 8, 1, 2, 9, 4});
        }
        public override void Start()
        {

            Stopwatch timeTaken = new Stopwatch();
            
            timeTaken.Start();
            CreatePopulation();
            CalcFitness();
            var generations = 1;
            while (timeTaken.Elapsed.TotalMilliseconds < _timeConstraint)
            {
                CalcFitness();
                for (int i = 0; i < populationSize; i++)
                {
                    if (_graph.GetCost(_population[populationSize-1]) < _bestCost)
                    {
                        _bestCost = _graph.GetCost(_population[populationSize-1]);
                        Console.WriteLine("Found new best solution with cost of {0}. Current population size: {1}", _bestCost, _population.Count);
                    }
                }
                NextGeneration();
                generations++;
            }
        }

        private void CreatePopulation()
        {
            var population = new List<int[]>();
            for (var i = 0; i < populationSize; i++)
            {
                IEnumerable<int> popMem = Enumerable.Range(0, _graph.GetSize()).OrderBy(_ => _randGen.Next());
                var enumerable = popMem.ToList();
                population.Add(enumerable.ToArray());
            }

            _population = population;
        }
        
        private void SwapToNeighbour2(int[] solution, int i, int j)
        {
            var min = Math.Min(i, j);
            var max = Math.Max(i, j);
            Array.Reverse(solution, min, max - min + 1);
        }

        private void CalcFitness()
        {
            
            for (var i = 0; i < _population.Count; i++)
            {
                _populationFitness[i] = (float) (1f/Math.Pow(_graph.GetCost(_population[i]), 4));
            }

            //Normalize fitness
            var fitSum = _populationFitness.Sum();
            for (var i = 0; i < _populationFitness.Length; i++)
            {
                _populationFitness[i] /= fitSum;
            }
            

            //Sort fitness ascending for weighted polling
            _population = _population.OrderBy(mem => _populationFitness[_population.IndexOf(mem)]).ToList();
            _populationFitness = _populationFitness.OrderBy((fitness) => fitness).ToArray();

            for (int i = 1; i < _populationFitness.Length; i++)
            {
                _populationFitness[i] += _populationFitness[i - 1];
            }

        }

        private void NextGeneration()
        {
            var newPop = new List<int[]>();
            for (var i = 0; i < populationSize; i++)
            {
                var rand = (float) _randGen.NextDouble();
                var pIndex = 0;
                for (var j = 0; j < _populationFitness.Length; j++)
                {
                    if (rand > _populationFitness[j]) continue;
                    pIndex = j;
                    break;
                }
                newPop.Add((int[]) _population[pIndex].Clone());
            }
            
            //Picked new population ancestors, now begin crossover

            _population.Clear();
            for (var i = 0; i < newPop.Count; i += 2)
            {
                if (i + 1 >= newPop.Count) break;
                if (_randGen.NextDouble() <= crossoverRate)
                {
                    var (item1, item2) = OrderedCo(newPop[i], newPop[i + 1]);
                    newPop.Remove(newPop[i]);
                    newPop.Remove(newPop[i]);
                    newPop.Add(item1);
                    newPop.Add(item2);
                }
            }

            foreach (var t in newPop)
            {
                if (_randGen.NextDouble() < mutationRate)
                {
                    int i1, i2;

                    do
                    {
                        i1 = _randGen.Next(_graph.GetSize());
                        i2 = _randGen.Next(_graph.GetSize());
                    } while (i1 == i2);
                
                    SwapToNeighbour2(t, i1, i2);
                }
            }
            _population = newPop;
        }


        private bool IsInSubArr(int[] arr, int start, int end, int val)
        {
            bool isIn = false;

            for (int i = start; i <= end; i++)
            {
                if (arr[i] == val) return true;
            }

            return false;
        }
        

        private (int[], int[]) OrderedCo(int[] p1, int[] p2)
        {
            var child = new int[p1.Length];
            var child2 = new int[p2.Length];
            var p1Cp = p1.ToList();
            var p2Cp = p2.ToList();

            var start = _randGen.Next(p1.Length);
            var end = _randGen.Next(p1.Length);
            
            while (start == end) start = _randGen.Next(p1.Length);

            var temp = start;
            start = start < end ? start : end;
            end = end > temp ? end : temp;

            for (var i = start; i <= end; i++)
            {
                child[i] = p1[i];
                child2[i] = p2[i];
            }

            for (var i = 0; i <= end; i++)
            {
                var t1 = p1Cp[0];
                p1Cp.Add(t1);
                p1Cp.RemoveAt(0);
            }

            for (var i = 0; i <= end; i++){
                var t2 = p2Cp[0];
                p2Cp.Add(t2);
                p2Cp.RemoveAt(0);
            }

            for (int i = 0; i < p1Cp.Count; i++)
            {
                if (!IsInSubArr(child2, start, end, p1Cp[i])) continue;
                p1Cp.RemoveAt(i);
                i--;
            }
            for (int i = 0; i < p2Cp.Count; i++)
            {
                if (!IsInSubArr(child, start, end, p2Cp[i])) continue;
                p2Cp.RemoveAt(i);
                i--;
            }

            using var en1 = p1Cp.GetEnumerator();
            en1.MoveNext();
            using var en2 = p2Cp.GetEnumerator();
            en2.MoveNext();

            for (int i = end+1; i%child.Length != start; i++)
            {
                int index = i % child.Length;
                child[index] = en2.Current;
                en2.MoveNext();

                child2[index] = en1.Current;
                en1.MoveNext();
            }
            


            return (child, child2);
        }
        
        
        
    }

    public enum CoMethod
    {
        OrderedCo,
        PartiallyMappedCo
    }
}