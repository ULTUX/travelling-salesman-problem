using System;
using System.Threading;
using TSP.Algorithms;

namespace TSP.Utils
{
    public class SecondBenchmark
    {
        private readonly float[] _tempModifiers;
        private readonly int[] _timeConstraints;
        private readonly int timeConstraint;
        private AnnealMethod _annealMethod;
        private SwapMethod _swapMethod;
        private Graph graph;
        private string inputFileName;
        private int repeats;
        private int trueCost;

        public SecondBenchmark(int trueCost, int timeConstraint, int repeats, string inputFileName,
            AnnealMethod annealMethod, SwapMethod swapMethod, int[] timeConstraints, float[] tempModifiers)
        {
            this.trueCost = trueCost;
            this.timeConstraint = timeConstraint;
            this.repeats = repeats;
            this.inputFileName = inputFileName;
            _annealMethod = annealMethod;
            _swapMethod = swapMethod;
            _timeConstraints = timeConstraints;
            _tempModifiers = tempModifiers;
            Console.WriteLine("Starting benchmark...");
            StartBenchmark();
        }

        private void StartBenchmark()
        {
            Console.WriteLine("Iteration count: {0}.", repeats);
            Thread.Sleep(1000);
            graph = inputFileName.Split(".")[1] == "atsp"
                ? new ATSPReader(inputFileName).ReadFile()
                : new GraphFileReader(inputFileName).ReadFile();

            var tabuResults = new float[repeats, 2];
            var annealResults = new float[repeats, 2];
            string[] csvHeaders = {"Czas wykonania [s]", "Znalezione rozwiązanie"};
            Console.WriteLine("Testing for time: {0}s", timeConstraint / 1000);

            for (var i = 0; i < repeats; i++)
            {
                Console.WriteLine("{0:0.00}% complete", (float) i / repeats * 100);

                //Tabu search benchmark
                var tabu = new TabuSearch(graph, timeConstraint, SwapMethod.TwoOperatorSwap, true);
                tabu.Start();
                var tabuResult = tabu.GetResults();

                //Simulated annealing
                var annealing = new SimulatedAnnealing(graph, timeConstraint, AnnealMethod.Geometric, null, true);
                annealing.Start();
                var annealResult = annealing.GetResults();

                annealResults[i, 0] = (float) annealResult.timeTookMillis;
                annealResults[i, 1] = annealResult.costFound;

                tabuResults[i, 0] = (float) tabuResult.timeTookMillis;
                tabuResults[i, 1] = tabuResult.costFound;
            }


            new CsvWriter("t_" + inputFileName.Split(".")[0] + "_" + _swapMethod + ".csv", csvHeaders,
                tabuResults).WriteFile();
            new CsvWriter("a_" + inputFileName.Split(".")[0] + "_" + _annealMethod + ".csv", csvHeaders,
                annealResults).WriteFile();


            string[] headers = {"Limit czasu", "Średni czas wykonania", "Średnia długość ścieżki"};

            var tabuData = new float[_timeConstraints.Length, 3];
            var annealData = new float[_timeConstraints.Length, 3];
            var it = 0;
            foreach (var constraint in _timeConstraints)
            {
                float avgRunTime = 0;
                float avgCost = 0;
                for (var i = 0; i < 10; i++)
                {
                    var tabu = new TabuSearch(graph, constraint, SwapMethod.TwoOperatorSwap, true);
                    tabu.Start();
                    var tabuResult = tabu.GetResults();
                    avgRunTime += Math.Max(0, (float) tabuResult.timeTookMillis);
                    avgCost += tabuResult.costFound;
                }

                avgRunTime /= 10;
                avgCost /= 10;
                tabuData[it, 0] = constraint;
                tabuData[it, 1] = avgRunTime;
                tabuData[it, 2] = avgCost;
                it++;
            }

            it = 0;
            foreach (var tempModifier in _tempModifiers)
            {
                float avgRunTime = 0;
                float avgCost = 0;
                for (var i = 0; i < 10; i++)
                {
                    var annealing = new SimulatedAnnealing(graph, timeConstraint, AnnealMethod.Geometric, tempModifier,
                        true);
                    annealing.Start();
                    var annealResult = annealing.GetResults();
                    avgRunTime += (float) annealResult.timeTookMillis;
                    avgCost += annealResult.costFound;
                }

                avgRunTime /= 10;
                avgCost /= 10;
                annealData[it, 0] = tempModifier;
                annealData[it, 1] = avgRunTime;
                annealData[it, 2] = avgCost;
                it++;
            }

            new CsvWriter("t_chart_" + inputFileName.Split(".")[0] + "_" + _swapMethod + ".csv", headers,
                tabuData).WriteFile();
            new CsvWriter("a_chart_" + inputFileName.Split(".")[0] + "_" + _annealMethod + ".csv", headers,
                annealData).WriteFile();
        }
    }
}