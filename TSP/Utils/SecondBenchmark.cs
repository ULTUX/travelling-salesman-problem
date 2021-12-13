using System;
using System.Threading;
using TSP.Algorithms;

namespace TSP.Utils
{
    public class SecondBenchmark
    {
        private int trueCost;
        private int[] timeConstraints;
        private int repeats;
        private string inputFileName;
        private Graph graph;
        private AnnealMethod _annealMethod;
        private SwapMethod _swapMethod;

        public SecondBenchmark(int trueCost, int[] timeConstraints, int repeats, string inputFileName, AnnealMethod annealMethod, SwapMethod swapMethod)
        {
            this.trueCost = trueCost;
            this.timeConstraints = timeConstraints;
            this.repeats = repeats;
            this.inputFileName = inputFileName;
            _annealMethod = annealMethod;
            _swapMethod = swapMethod;
            Console.WriteLine("Starting benchmark...");
            startBenchmark();
        }

        private void startBenchmark()
        { 
            Console.WriteLine("Iteration count: {0}.", repeats);
            Thread.Sleep(1000);
            graph = inputFileName.Split(".")[1] == "atsp" ? new ATSPReader(inputFileName).ReadFile() 
                : new GraphFileReader(inputFileName).ReadFile();
            
            int arrIterator = 0;
            float[,] tabuResults = new float[timeConstraints.Length, 4];
            float[,] annealResults = new float[timeConstraints.Length, 4];
            string[] csvHeaders = {"Czas wykonania [s]", "Najmniejsze rozwiązanie", "Największe rozwiązanie", "Średni błąd względny"};
            foreach (var timeConstraint in timeConstraints)
            {
                Console.WriteLine("Testing for time: {0}s", timeConstraint/1000);
                int annealAvg = 0;
                int tabuAvg = 0;
                
                int minCostTabu = int.MaxValue;
                int maxCostTabu = 0;
                
                int minCostAnneal = int.MaxValue;
                int maxCostAnneal = 0;
                
                for (int i = 0; i < repeats; i++)
                {
                    Console.WriteLine("{0:0.00}% complete", (float) i / repeats * 100);
                
                    //Tabu search benchmark
                    TabuSearch tabu = new TabuSearch(graph,  timeConstraint, SwapMethod.TwoOperatorSwap, true);
                    tabu.Start();

                    var cost = tabu.GetResults().costFound;
                    minCostTabu = Math.Min(minCostTabu, cost);
                    maxCostTabu = Math.Max(maxCostTabu, cost);
                    tabuAvg += cost;
                
                    //Simulated annealing
                    SimulatedAnnealing annealing = new SimulatedAnnealing(graph, timeConstraint, AnnealMethod.Geometric, null, true);
                    annealing.Start();
                    cost = annealing.GetResults().costFound;
                    minCostAnneal = Math.Min(minCostAnneal, cost);
                    maxCostAnneal = Math.Max(maxCostAnneal, cost);
                    annealAvg += cost;

                }

                annealAvg /= repeats;
                tabuAvg /= repeats;

                tabuResults[arrIterator, 0] = timeConstraint;
                tabuResults[arrIterator, 1] = minCostTabu;
                tabuResults[arrIterator, 2] = maxCostTabu;
                tabuResults[arrIterator, 3] = ((float) tabuAvg - trueCost) / trueCost;
                
                annealResults[arrIterator, 0] = timeConstraint;
                annealResults[arrIterator, 1] = minCostAnneal;
                annealResults[arrIterator, 2] = maxCostAnneal;
                annealResults[arrIterator, 3] = ((float) annealAvg - trueCost) / trueCost;

                arrIterator++;

            }
            new CsvWriter("t_" + inputFileName.Split(".")[0]+"_"+_swapMethod+".csv", csvHeaders,
                tabuResults).WriteFile();
            new CsvWriter("a_" + inputFileName.Split(".")[0]+"_"+_annealMethod+".csv", csvHeaders,
                annealResults).WriteFile();
            
        }
    }
}