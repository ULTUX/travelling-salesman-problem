using TSP.Algorithms;
using TSP.Utils;

namespace TSP
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // new Menu();

            new GeneticAlgorithm(new ATSPReader("ftv47.atsp").ReadFile(), 1000000, 1000, 0.8f, 0.01f,
                CoMethod.OrderedCo).Start();

            // new TabuSearch(new ATSPReader("ftv47.atsp").ReadFile(),
            //     1000, SwapMethod.InsertSwap, false).Start();
            // int[] times = {1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000};
            // float[] tempMods = {0.7f, 0.75f, 0.80f, 0.85f, 0.9f, 0.95f, 0.99f, 0.999f, 0.9999f, 0.99999f};
            // new SecondBenchmark(39, 1000, 10, "rbg403.atsp", AnnealMethod.SlowDecrease, SwapMethod.InsertSwap, times,
            // tempMods);
        }


        // var alg = new TabuSearch(new ATSPReader("ftv170.atsp").ReadFile(), 360000, SwapMethod.TwoOperatorSwap);
        // alg.Start();
        // var results = alg.GetResults();
        // Console.WriteLine("Results got from algorithm: best found: {0}", results.costFound);
        // Graph.PrintSolution(results.solutionFound);
        // new SimulatedAnnealing(new ATSPReader("br17.atsp").ReadFile(),  5000).Start();
    }
}