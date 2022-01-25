using System;
using System.Linq;
using System.Threading;
using TSP.Algorithms;

namespace TSP.Utils
{
    public class SecondBenchmark
    {
        private readonly float[] _tempModifiers;
        private readonly int[] _timeConstraints;
        private readonly int timeConstraint;
        private AnnealMethod? _annealMethod;
        private SwapMethod? _swapMethod;
        private Graph graph;
        private string inputFileName;
        private int repeats;
        private int trueCost;
        private readonly CoMethod? _coMethod;
        private int[] _popSize;
        private float _coRate;
        private float _mtRate;

        public SecondBenchmark(int trueCost, int timeConstraint, int repeats, string inputFileName,
            AnnealMethod? annealMethod, SwapMethod? swapMethod, CoMethod? coMethod, int[] timeConstraints, float[] tempModifiers,
            int[] popSize, float coRate, float mtRate)
        {
            this.trueCost = trueCost;
            this.timeConstraint = timeConstraint;
            this.repeats = repeats;
            this.inputFileName = inputFileName;
            _annealMethod = annealMethod;
            _swapMethod = swapMethod;
            _timeConstraints = timeConstraints;
            _tempModifiers = tempModifiers;
            _coMethod = coMethod;
            _popSize = popSize;
            _coRate = coRate;
            _mtRate = mtRate;
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
            var geneticResults = new float[repeats, 2];


            string[] headers = {"Limit czasu", "Średni czas wykonania", "Średnia długość ścieżki"};

            var tabuData = new float[_timeConstraints.Length, 3];
            var annealData = new float[_timeConstraints.Length, 3];
            var geneticData = new float[_timeConstraints.Length, 3];
            var geneticDataPopSize = new float[_popSize.Length, 3];
            var it = 0;
            foreach (var constraint in _timeConstraints)
            {
                Console.WriteLine("{0:0.00}% complete",
                    (float) _timeConstraints.ToList().IndexOf(constraint) / repeats * 100);

                float avgRunTimeTabu = 0;
                float avgCostTabu = 0;

                float avgRunTimeGenetic = 0;
                float avgCostGenetic = 0;

                float avgRunTimeAnneal = 0;
                float avgCostAnneal = 0;

                for (var i = 0; i < 10; i++)
                {
                    if (_coMethod != null)
                    {
                        var genetic = new GeneticAlgorithm(graph, constraint, _popSize[0], _coRate, _mtRate,
                            (CoMethod) _coMethod, true);
                        genetic.Start();
                        var geneticResult = genetic.GetResults();
                        avgRunTimeGenetic += Math.Max(0, (float) geneticResult.timeTookMillis);
                        avgCostGenetic += geneticResult.costFound;
                    }

                    if (_swapMethod != null)
                    {
                        var tabu = new TabuSearch(graph, constraint, (SwapMethod) _swapMethod, true);
                        tabu.Start();
                        var tabuResult = tabu.GetResults();
                        avgRunTimeTabu += Math.Max(0, (float) tabuResult.timeTookMillis);
                        avgCostTabu += tabuResult.costFound;
                    }

                    if (_annealMethod == null) continue;
                    var anneal = new SimulatedAnnealing(graph, constraint, (AnnealMethod) _annealMethod, null, true);
                    anneal.Start();
                    var annealResult = anneal.GetResults();
                    avgRunTimeAnneal += Math.Max(0, (float) annealResult.timeTookMillis);
                    avgCostAnneal += annealResult.costFound;
                }


                avgRunTimeTabu /= 10;
                avgCostTabu /= 10;

                avgRunTimeAnneal /= 10;
                avgCostAnneal /= 10;

                avgRunTimeGenetic /= 10;
                avgCostGenetic /= 10;

                Console.WriteLine("Tabu: {0}", avgCostTabu);
                Console.WriteLine("Anneal: {0}", avgCostAnneal);
                Console.WriteLine("Genetic: {0}", avgCostGenetic);

                tabuData[it, 0] = constraint;
                tabuData[it, 1] = avgRunTimeTabu;
                tabuData[it, 2] = avgCostTabu;

                annealData[it, 0] = constraint;
                annealData[it, 1] = avgRunTimeAnneal;
                annealData[it, 2] = avgCostAnneal;
                
                geneticData[it, 0] = constraint;
                geneticData[it, 1] = avgRunTimeGenetic;
                geneticData[it, 2] = avgCostGenetic;
                it++;
            }

            it = 0;
            
            
            foreach (var popSize in _popSize)
            {
                float avgRunTime = 0;
                float avgCost = 0;
                for (var i = 0; i < 10; i++)
                {
                    var genetic = new GeneticAlgorithm(graph, timeConstraint, popSize, 0.8f, 0.01f, CoMethod.OrderedCo, false);
                    genetic.Start();
                    var geneticResult = genetic.GetResults();
                    avgRunTime += (float) geneticResult.timeTookMillis;
                    avgCost += geneticResult.costFound;
                }
            
                avgRunTime /= 10;
                avgCost /= 10;
                geneticDataPopSize[it, 0] = popSize;
                geneticDataPopSize[it, 1] = avgRunTime;
                geneticDataPopSize[it, 2] = avgCost;
                it++;
            }

            // new CsvWriter("t_chart_" + inputFileName.Split(".")[0] + "_" + _swapMethod + ".csv", headers,
            //     tabuData).WriteFile();
            // new CsvWriter("a_chart_" + inputFileName.Split(".")[0] + "_" + _annealMethod + ".csv", headers,
            //     annealData).WriteFile();
            new CsvWriter("g_chart_" + inputFileName.Split(".")[0] + "_" + _coMethod + ".csv", headers,
                geneticData).WriteFile();
            // new CsvWriter("a_chart_modifiers_" + inputFileName.Split(".")[0] + "_" + _annealMethod + ".csv", headers,
            //     annealDataTempModsData).WriteFile();
            new CsvWriter("g_chart_modifiers_" + inputFileName.Split(".")[0] + "_" + _coMethod + ".csv", headers,
                geneticDataPopSize).WriteFile();
        }
    }
}