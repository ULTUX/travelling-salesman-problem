namespace TSP
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //  int[,] a = {{0, 5, 8, 4, 5}, {5, 0, 7, 4, 5}, {8, 7, 0, 8, 6}, {4, 4, 8, 0, 8}, {5, 5, 6, 8, 0}};
            // Graph graph = new Graph(5, a);
            // BruteForce alg = new BruteForce(graph, 0);
            // alg.Start();


            // GraphFileReader reader = new GraphFileReader("plik.txt");
            // Graph graph = reader.ReadFile();
            // BruteForce alg = new BruteForce(graph, 0);
            // alg.Start();
            // DynamicProgrammingTsp tsp = new DynamicProgrammingTsp(graph, 0);
            // tsp.Start();

            // RandomGraphGenerator graphGenerator = new RandomGraphGenerator(true, 10, 20, 20, null);
            // DynamicProgrammingTsp tsp = new DynamicProgrammingTsp(graphGenerator.GenerateRandomGraph(), 0);
            // tsp.Start();


            new Menu();

            // var benchmark = new Benchmark(5, 25, 1, 1.0f, new BranchNBoundDfs(), 100);
            // benchmark.Start();
            // int[,] graph =
            //     {{-1, 20, 30, 10, 11}, {15, -1, 16, 4, 2}, {3, 5, -1, 2, 4}, {19, 6, 18, -1, 3}, {16, 4, 7, 16, -1}};
            // Graph graphO = new Graph(5, graph);
            // var alg = new BranchNBoundDfs(graphO, 0);
            // alg.Start();
        }
    }
}