using System;
using TSP.Algorithms;
using TSP.Utils;

namespace TSP
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            new Menu();
            int[] time = new[] {100, 200, 300, 400, 500, 600, 700, 800, 1000, 2000, 4000, 8000, 10000};
            // new SecondBenchmark(39, time, 10, "ftv47.atsp", AnnealMethod.Geometric, SwapMethod.InsertSwap);
            // var alg = new TabuSearch(new ATSPReader("ftv170.atsp").ReadFile(), 360000, SwapMethod.TwoOperatorSwap);
            // alg.Starkt();
            // var results = alg.GetResults();
            // Console.WriteLine("Results got from algorithm: best found: {0}", results.costFound);
            // Graph.PrintSolution(results.solutionFound);
            // new SimulatedAnnealing(new ATSPReader("br17.atsp").ReadFile(),  5000).Start();
        }
    }
}