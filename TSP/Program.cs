﻿namespace TSP
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // new GeneticAlgorithm(new ATSPReader("rbg403.atsp").ReadFile(), 9000000, 10000, 0.8f, 0.03f,
            //     CoMethod.OrderedCo, false).Start();


            new Menu();


            // new TabuSearch(new ATSPReader("ftv47.atsp").ReadFile(),
            //     1000, SwapMethod.InsertSwap, false).Start();


            // int[] times = {500, 1000, 2000, 4000, 8000, 16000, 32000, 64000};
            // float[] tempMods = {0.7f, 0.75f, 0.80f, 0.85f, 0.9f, 0.95f, 0.99f, 0.999f, 0.9999f, 0.99999f};
            // int[] popSizes =
            // {
            //     5, 10, 20, 40, 50, 100, 200, 400
            // };
            // new SecondBenchmark(39, 1000, 10, "rbg403.atsp", null,
            //     null, CoMethod.PartiallyMappedCo, times, tempMods, popSizes, 0.8f, 0.01f);
        }
    }


    // var alg = new TabuSearch(new ATSPReader("ftv170.atsp").ReadFile(), 360000, SwapMethod.TwoOperatorSwap);
    // alg.Start();
    // var results = alg.GetResults();
    // Console.WriteLine("Results got from algorithm: best found: {0}", results.costFound);
    // Graph.PrintSolution(results.solutionFound);
    // new SimulatedAnnealing(new ATSPReader("br17.atsp").ReadFile(),  5000).Start();
}