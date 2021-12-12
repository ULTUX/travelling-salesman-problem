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

            var alg = new TabuSearch(new ATSPReader("ftv170.atsp").ReadFile(), 360000, SwapMethod.TwoOperatorSwap);
            alg.Start();
            var results = alg.GetResults();
            Console.WriteLine("Results got from algorithm: best found: {0}", results.costFound);
            Graph.PrintSolution(results.solutionFound);
            // new SimulatedAnnealing(new ATSPReader("br17.atsp").ReadFile(),  5000).Start();
        }
    }
}