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

        public Menu()
        {
            PrintMainMenu();
        }

        private void PrintMainMenu()
        {
            var exit = false;
            while (!exit)
            {
                Console.WriteLine("Co chcesz zrobić?");
                Console.WriteLine("\t1. Wczytać graf z pliku.\n\t2. Wygenerować losowy graf\n\t3. Przeprowadzić testy");
                if (_currentGraph != null)
                    Console.WriteLine("\t4. Rozwiązać problem komiwojażera algorytmem brute-force\n\t" +
                                      "5. Rozwiazać problem komiwojażera algorytmem DP\n\t6. Rozwiązać proglem komiwojadżera algorytmem B&B (min)\n\t7. Rozwiązać proglem komiwojadżera algorytmem B&B (DFS)");
                Console.WriteLine("\t0. Wyłączyć program");
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
                new Benchmark(startVal, endVal, step, mult, algorithm, repeats).Start();
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

        private static int ParseFromString(string data)
        {
            if (data == null || !int.TryParse(data, out var n))
                throw new ArgumentException("Podano złe dane wejściowe!");
            return n;
        }
    }
}