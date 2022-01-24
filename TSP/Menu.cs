using System;
using System.Threading;
using TSP.Algorithms;
using TSP.Algorithms.BranchNBound;
using TSP.Utils;

namespace TSP
{
    public class Menu
    {
        private Graph _currentGraph;
        private int? _timeConstraint;
        private AnnealMethod? _annealMethod;
        private SwapMethod? _swapMethod;
        private CoMethod? _coMethod;
        private int? _populationSize;
        private float? _cxRate;
        private float? _mtRate;

        public Menu()
        {
            PrintMainMenu();
        }

        private void PrintMainMenu()
        {
            var exit = false;
            while (!exit)
            {
                Console.WriteLine(
                    "Które zadanie projektowe należy włączyć?\n\t1. Zadanie 1\n\t2. Zadanie 2\n\t3. Zadanie 3\n\t0. Wyłączyć program");

                var key = Console.ReadKey();
                Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        PrintFirstPartMenu();
                        break;
                    case ConsoleKey.D2:
                        PrintSecondPartMenu();
                        break;
                    case ConsoleKey.D3:
                        PrintThirdPartMenu();
                        break;
                    case ConsoleKey.D0:
                        exit = true;
                        break;
                }
            }
        }

        private void PrintThirdPartMenu()
        {
            var exit = false;
            while (!exit)
            {
                Console.WriteLine("Co chcesz zrobić?");
                Console.WriteLine("\t1. Wczytać graf z pliku.\n\t2. Wygenerować losowy graf\n\t3. Przeprowadzić testy");
                if (_currentGraph != null)
                    Console.WriteLine(
                        "\t4. Uruchomić algorytm genetyczny");
                Console.WriteLine("\n\tR. Reset parametrów\n\t0. Powrót");
                var key = Console.ReadKey();
                Console.WriteLine();
                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        ReadGraphFromFile();
                        break;
                    case ConsoleKey.D2:
                        GenerateRandomGraph();
                        break;
                    case ConsoleKey.R:
                        _timeConstraint = null;
                        _annealMethod = null;
                        _swapMethod = null;
                        Console.WriteLine("Parametry zostały zresetowane.");
                        break;
                    case ConsoleKey.D0:
                        exit = true;
                        break;
                    default:
                    {
                        if (_currentGraph != null)
                            switch (key.Key)
                            {
                                case ConsoleKey.D4:
                                    RunGeneticAlg();
                                    break;
                                default:
                                    Console.WriteLine("Zły wybór, spróbuj jeszcze raz.");
                                    break;
                            }
                        else
                            Console.WriteLine("Zły wybór, spróbuj jeszcze raz.");

                        break;
                    }
                }

            }
        }

        private void RunGeneticAlg()
        {
            Console.WriteLine("Wybrano algorytm genetyczny");
            if (_currentGraph == null) return;
            try
            {
                if (_timeConstraint is null)
                {
                    Console.WriteLine("Podaj czas działania algorytmu: ");
                    var timeRead = Console.ReadLine();
                    _timeConstraint = ParseFromString(timeRead);
                }

                if (_coMethod is null)
                {
                    Console.WriteLine("Wybierz metodę krzyżowania:\n\t1. Partially matched crossover (PMX)\n\t" +
                                      "2. Order crossover (OX)\n");
                    var key = Console.ReadKey();
                    _coMethod = key.Key switch
                    {
                        (ConsoleKey.D1) => CoMethod.PartiallyMappedCo,
                        (ConsoleKey.D2) => CoMethod.OrderedCo,
                        _ => CoMethod.OrderedCo
                    };
                }

                if (_populationSize is null)
                {
                    Console.WriteLine("\nPodaj wielkość populacji: ");
                    var popSize = Console.ReadLine();
                    _populationSize = ParseFromString(popSize);
                }

                if (_cxRate is null)
                {
                    Console.WriteLine("Podaj współczynnik krzyżowania: ");
                    var cx = Console.ReadLine();
                    _cxRate = float.Parse(cx ?? throw new InvalidOperationException("Nie podano żadnej wartości."));
                }

                if (_mtRate is null)
                {
                    Console.WriteLine("Podaj współczynnik mutacji: ");
                    var mt = Console.ReadLine();
                    _mtRate = float.Parse(mt ?? throw new InvalidOperationException("Nie podano żadnej wartości."));
                }
                Console.WriteLine("\nUruchamianie algorytmu...");
                Thread.Sleep(1000);
                new GeneticAlgorithm(_currentGraph, (int) _timeConstraint, (int) _populationSize, (float) _cxRate, (float) _mtRate, (CoMethod) _coMethod).Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void PrintSecondPartMenu()
        {
            var exit = false;
            while (!exit)
            {
                Console.WriteLine("Co chcesz zrobić?");
                Console.WriteLine("\t1. Wczytać graf z pliku.\n\t2. Wygenerować losowy graf\n\t3. Przeprowadzić testy");
                if (_currentGraph != null)
                    Console.WriteLine(
                        "\t4. Uruchomić algorytm Tabu Search\n\t5. Uruchomić algorytm Symulowanego wyżarzania");
                Console.WriteLine("\n\tR. Reset parametrów\n\t0. Powrót");
                var key = Console.ReadKey();
                Console.WriteLine();
                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        ReadGraphFromFile();
                        break;
                    case ConsoleKey.D2:
                        GenerateRandomGraph();
                        break;
                    case ConsoleKey.D3:
                        throw new NotImplementedException("Do implementacji"); //TODO: Implement this.
                        break;
                    case ConsoleKey.R:
                        _timeConstraint = null;
                        _annealMethod = null;
                        _swapMethod = null;
                        Console.WriteLine("Parametry zostały zresetowane.");
                        break;
                    case ConsoleKey.D0:
                        exit = true;
                        break;
                    default:
                    {
                        if (_currentGraph != null)
                            switch (key.Key)
                            {
                                case ConsoleKey.D4:
                                    RunTabuSearchAlg();
                                    break;
                                case ConsoleKey.D5:
                                    RunSimAnnealingAlg();
                                    break;
                                default:
                                    Console.WriteLine("Zły wybór, spróbuj jeszcze raz.");
                                    break;
                            }
                        else
                            Console.WriteLine("Zły wybór, spróbuj jeszcze raz.");

                        break;
                    }
                }

            }
        }
        private void RunTabuSearchAlg()
        {
            Console.WriteLine("Wybrano Tabu search");
            if (_currentGraph != null)
            {
                try
                {
                    if (_timeConstraint is null)
                    {
                        Console.WriteLine("Podaj czas działania algorytmu: ");
                        var timeRead = Console.ReadLine();
                        _timeConstraint = ParseFromString(timeRead);
                    }

                    if (_swapMethod is null)
                    {
                        Console.WriteLine("Podaj rodzaj wyboru sąsiedztwa: \n\t1. Zamiana 2 wierzchołków (2-opt swap)\n\t" +
                                          "2. Zamiana 2 krawędzi (2 edge exchange)\n\t3. Zamiana przez wstawienie (insertion)");
                        var key = Console.ReadKey();
                        switch (key.Key)
                        {
                            case (ConsoleKey.D1):
                                _swapMethod = SwapMethod.TwoOperatorSwap;
                                break;
                            case (ConsoleKey.D2):
                                _swapMethod = SwapMethod.TwoEdgeSwap;
                                break;
                            case (ConsoleKey.D3):
                                _swapMethod = SwapMethod.TwoOperatorSwap;
                                break;
                            default:
                                _swapMethod = SwapMethod.TwoOperatorSwap;
                                break;
                            
                        }
                    }
                    Console.WriteLine("\nUruchamianie algorytmu...");
                    new TabuSearch(_currentGraph, (int) _timeConstraint, (SwapMethod) _swapMethod, false).Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        
        private void RunSimAnnealingAlg()
        {
            Console.WriteLine("Wybrano Symulwane wyżarzanie");
            if (_currentGraph != null)
            {
                try
                {
                    if (_timeConstraint is null)
                    {
                        Console.WriteLine("Podaj czas działania algorytmu: ");
                        var timeRead = Console.ReadLine();
                        _timeConstraint = ParseFromString(timeRead);
                    }

                    if (_annealMethod is null)
                    {
                        Console.WriteLine("Podaj schemat schładzania:\n\t1. Geometryczny [t=t*\u03B1]\n\t" +
                                          "2. Linearny [t=t-\u03B1]\n\t3. Wolnego spadku [t=1/(\u03B1*t)]");
                        var key = Console.ReadKey();
                        switch (key.Key)
                        {
                            case (ConsoleKey.D1):
                                _annealMethod = AnnealMethod.Geometric;
                                break;
                            case (ConsoleKey.D2):
                                _annealMethod = AnnealMethod.Linear;
                                break;
                            case (ConsoleKey.D3):
                                _annealMethod = AnnealMethod.SlowDecrease;
                                break;
                            default:
                                _annealMethod = AnnealMethod.Geometric;
                                break;
                            
                        }
                    }
                    
                    Console.WriteLine("\nPodaj modyfikator temperatury (puste dla dynamicznej): ");
                    var readModifier = Console.ReadLine();
                    if (readModifier == null)
                        throw new ArgumentNullException(null, "Wystąpił błąd przy pobieraniu danych z klawiatury.");
                    float? modifier;
                    if (readModifier == "")
                    {
                        modifier = null;
                    }
                    else
                    {
                        modifier = int.Parse(readModifier);
                    }
                    Console.WriteLine("Uruchamianie algorytmu...");
                    new SimulatedAnnealing(_currentGraph, (int) _timeConstraint, (AnnealMethod) _annealMethod, modifier, false).Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }


        private void PrintFirstPartMenu()
        {
            var exit = false;
            while (!exit)
            {
                Console.WriteLine("Co chcesz zrobić?");
                Console.WriteLine("\t1. Wczytać graf z pliku.\n\t2. Wygenerować losowy graf\n\t3. Przeprowadzić testy");
                if (_currentGraph != null)
                    Console.WriteLine("\t4. Rozwiązać problem komiwojażera algorytmem brute-force\n\t" +
                                      "5. Rozwiazać problem komiwojażera algorytmem DP\n\t6. Rozwiązać proglem komiwojadżera algorytmem B&B (min)\n\t7. Rozwiązać proglem komiwojadżera algorytmem B&B (DFS)");
                Console.WriteLine("\t0. Powrót");
                var key = Console.ReadKey();
                Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        ReadGraphFromFile();
                        break;
                    case ConsoleKey.D2:
                        GenerateRandomGraph();
                        break;
                    case ConsoleKey.D3:
                        InitBenchmark();
                        break;
                    case ConsoleKey.D0:
                        exit = true;
                        break;
                    default:
                    {
                        if (_currentGraph != null)
                            switch (key.Key)
                            {
                                case ConsoleKey.D4:
                                    RunBruteForceAlg();
                                    break;
                                case ConsoleKey.D5:
                                    RunDpAlg();
                                    break;
                                case ConsoleKey.D6:
                                    RunBnbMinAlg();
                                    break;
                                case ConsoleKey.D7:
                                    RunBnbDfsAlg();
                                    break;
                                default:
                                    Console.WriteLine("Zły wybór, spróbuj jeszcze raz.");
                                    break;
                            }
                        else
                            Console.WriteLine("Zły wybór, spróbuj jeszcze raz.");

                        break;
                    }
                }
            }

            Console.WriteLine("Zatrzymywanie programu...");
            Thread.Sleep(500);
        }

        private void InitBenchmark()
        {
            Console.Write("Wybrano przeprowadzenie testów.\nPodaj wielkość początkową: ");
            try
            {
                var data = Console.ReadLine();
                var startVal = ParseFromString(data);
                Console.Write("Podaj wielkość końcową: ");
                data = Console.ReadLine();
                var endVal = ParseFromString(data);
                Console.Write("Podaj krok: ");
                data = Console.ReadLine();
                var step = ParseFromString(data);
                Console.Write("Podaj ilość powtórzeń: ");
                data = Console.ReadLine();
                var repeats = ParseFromString(data);
                Console.Write("Podaj mnożnik (domyślnie 1.0): ");
                data = Console.ReadLine();
                float mult;
                if (data == "") mult = 1.0f;
                else mult = ParseFromString(data);

                Console.WriteLine(
                    "Wybierz algorytm:\n\t1. Przegląd zupełny\n\t2. Programowanie dynamiczne\n\t3. Branch and Bound metodą minimalną" +
                    "\n\t4. Branch and Bound przy użyciu DFS");
                Console.Write("Wybór: ");
                var key = Console.ReadKey().Key;
                TspAlgorithm algorithm;
                switch (key)
                {
                    case ConsoleKey.D1:
                        algorithm = new BruteForce();
                        break;
                    case ConsoleKey.D2:
                        algorithm = new DynamicProgrammingTsp();
                        break;
                    case ConsoleKey.D3:
                        algorithm = new BranchNBoundMin();
                        break;
                    case ConsoleKey.D4:
                        algorithm = new BranchNBoundDfs();
                        break;
                    default:
                        throw new Exception("Algorithm not specified.");
                }

                Console.WriteLine("Uruchamianie algorytmu...");
                new FirstBenchmark(startVal, endVal, step, mult, algorithm, repeats).Start();
                Console.WriteLine("Testy zostały przeprowadzone, plik .csv został zapisany.");
            }
            catch
            {
                Console.WriteLine("Nieprawidłowe dane.");
            }
        }

        private void RunBnbDfsAlg()
        {
            Console.WriteLine("Uruchamianie algorytmu...");
            var bNbAlg = new BranchNBoundDfs(_currentGraph, 0);
            bNbAlg.Start();
    }

        private void RunBnbMinAlg()
        {
            Console.WriteLine("Uruchamianie algorytmu...");
            var bNbAlg = new BranchNBoundMin(_currentGraph, 0);
            bNbAlg.Start();
            }

        private void RunDpAlg()
        {
            Console.WriteLine("Uruchamianie algorytmu...");
            var dynamicProgrammingTsp = new DynamicProgrammingTsp(_currentGraph, 0);
            dynamicProgrammingTsp.Start();
        }

        private void RunBruteForceAlg()
        {
            Console.WriteLine("Uruchamianie algorytmu...");
            var bfAlg = new BruteForce(_currentGraph, 0);
            bfAlg.Start();
        }

        private void GenerateRandomGraph()
        {
            try
            {
                Console.WriteLine("Generowanie losowego grafu.");
                Console.Write("Podaj wielkość grafu: ");
                var readVal = Console.ReadLine();

                var size = ParseFromString(readVal);

                Console.Write("Graf symetryczny(S)/asymetryczny(A)? ");
                var key = Console.ReadKey();
                if (!key.Key.Equals(ConsoleKey.S) && !key.Key.Equals(ConsoleKey.A))
                {
                    Console.WriteLine("Podano złe dane!");
                    return;
                }

                Console.WriteLine();
                var isSym = key.Key.Equals(ConsoleKey.S);

                Console.Write("Minimalna waga: ");

                readVal = Console.ReadLine();
                var minVal = ParseFromString(readVal);

                Console.Write("Maksymalna waga: ");

                readVal = Console.ReadLine();
                var maxVal = ParseFromString(readVal);

                Console.Write("Seed (opcjonalne): ");
                readVal = Console.ReadLine();

                int? seed = null;
                if (readVal != null && !readVal.Equals("")) seed = ParseFromString(readVal);

                Console.WriteLine("Generowanie grafu...");

                var generator = new RandomGraphGenerator(isSym, minVal, maxVal, size, seed);

                _currentGraph = generator.GenerateRandomGraph();
                Console.WriteLine("Wygenerowano graf: ");
                _currentGraph.Print();
            }
            catch
            {
                Console.WriteLine("Podano zły argument!");
            }
        }

        private void ReadGraphFromFile()
        {
            Console.WriteLine("Odczyt grafu z pliku.");
            Console.Write("Podaj nazwę pliku: ");
            var fileName = Console.ReadLine();
            Console.WriteLine("Odczyt z pliku: {0}", fileName);
            if (fileName is null) return;
            if (fileName.Split(".")[1] == "atsp")
            {
                var graphFileReader = new ATSPReader(fileName);
                try
                {
                    _currentGraph = graphFileReader.ReadFile();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Wystąpił błąd podczas próby odczytania danych z plku.");
                    Console.WriteLine("Błąd: {0}", e.Message);
                }
            }
            else
            {
                var graphFileReader = new GraphFileReader(fileName);
                try
                {
                    _currentGraph = graphFileReader.ReadFile();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Wystąpił błąd podczas próby odczytania danych z plku.");
                    Console.WriteLine("Błąd: {0}", e.Message);
                }
            }
        }

        private static int ParseFromString(string data)
        {
            if (data == null || !int.TryParse(data, out var n))
                throw new ArgumentException("Podano złe dane wejściowe!");
            return n;
        }
    }
}