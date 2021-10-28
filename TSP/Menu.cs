using System;
using System.Threading;

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
                Console.WriteLine("\t1. Wczytać graf z pliku.\n\t2. Wygenerować losowy graf");
                if (_currentGraph != null) 
                    Console.WriteLine("\t3. Rozwiązać problem komiwojażera algorytmem brute-force\n\t" +
                                      "4. Rozwiazać problem komiwojażera algorytmem DP");
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
                    case ConsoleKey.D0:
                        exit = true;
                        break;
                    default:
                    {
                        if (_currentGraph != null)
                        {
                            switch (key.Key)
                            {
                                case(ConsoleKey.D3):
                                    RunBruteForceAlg();
                                    break;
                                case (ConsoleKey.D4):
                                    RunDpAlg();
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Zły wybór, spróbuj jeszcze raz.");
                        }
                        break;
                    }
                }
            }
            
            Console.WriteLine("Zatrzymywanie programu...");
            Thread.Sleep(500);
            
        }

        private void RunDpAlg()
        {
            Console.WriteLine("Uruchamianie algorytmu...");
            Console.Write("Wierzchołek początkowy: ");
            var readVal = Console.ReadLine();
            try
            {
                var startVertex = ParseFromString(readVal);
                var DynamicProgrammingTsp = new DynamicProgrammingTsp(_currentGraph, startVertex);
                DynamicProgrammingTsp.Start();
            }
            catch
            {
                Console.WriteLine("Nieprawidłowe dane.");
            }
        }

        private void RunBruteForceAlg()
        {
            Console.WriteLine("Uruchamianie algorytmu...");
            Console.Write("Wierzchołek początkowy: ");
            var readVal = Console.ReadLine();
            try
            {
                var startVertex = ParseFromString(readVal);
                var bfAlg = new BruteForce(_currentGraph, startVertex);
                bfAlg.Start();
            }
            catch
            {
                Console.WriteLine("Nieprawidłowe dane.");
            }
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

                Console.Write("Minimalna wielkość: ");

                readVal = Console.ReadLine();
                var minVal = ParseFromString(readVal);
                
                Console.Write("Maksymalna wielkość: ");

                readVal = Console.ReadLine();
                var maxVal = ParseFromString(readVal);
                
                Console.Write("Seed (opcjonalne): ");
                readVal = Console.ReadLine();

                int? seed = null;
                if (readVal != null && !readVal.Equals(""))
                {
                    seed = ParseFromString(readVal);
                }
                
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
            if (data == null || !int.TryParse((string) data, out var n)) throw new ArgumentException("Podano złe dane wejściowe!");
            return n;
        }
    }
}