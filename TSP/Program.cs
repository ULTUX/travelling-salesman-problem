using TSP.Algorithms;
using TSP.Utils;

namespace TSP
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            int[] times = {100, 200, 300, 400, 500, 600, 700, 800, 900, 1000};
            float[] tempMods = {0.7f, 0.75f, 0.80f, 0.85f, 0.9f, 0.95f, 0.99f, 0.999f, 0.9999f, 0.99999f};
            // new Menu();
            new SecondBenchmark(39, 1000, 10, "ftv47.atsp", AnnealMethod.Geometric, SwapMethod.TwoOperatorSwap, times,
                tempMods);
        }


        // var alg = new TabuSearch(new ATSPReader("ftv170.atsp").ReadFile(), 360000, SwapMethod.TwoOperatorSwap);
        // alg.Starkt();
        // var results = alg.GetResults();
        // Console.WriteLine("Results got from algorithm: best found: {0}", results.costFound);
        // Graph.PrintSolution(results.solutionFound);
        // new SimulatedAnnealing(new ATSPReader("br17.atsp").ReadFile(),  5000).Start();
    }
}