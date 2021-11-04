using System;
using System.IO;

namespace TSP.Utils
{
    /**
     * Simple class that reads graphs from file.
     */
    public class GraphFileReader
    {
        private readonly string _fileName;
        private int _graphSize;
        private Graph _readGraph;

        /**
         * Initialize new GraphFileReader object.
         */
        public GraphFileReader(string fileName)
        {
            _fileName = fileName;
        }

        /**
         * Start reading from file. Return graph object contained in this file.
         */
        public Graph ReadFile()
        {
            try
            {
                Console.WriteLine("Reading data from file...");
                var fileLines =
                    File.ReadAllLines(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + _fileName);
                Console.WriteLine("Number of lines read: {0}", fileLines.Length);
                _graphSize = Convert.ToInt32(fileLines[0]);
                var adjMatrix = new int[_graphSize, _graphSize];
                Console.WriteLine("Graph size: {0}", _graphSize);

                for (var i = 0; i < _graphSize; i++)
                {
                    var stringData = fileLines[i + 1].Split(" ");
                    var realIndex = 0;
                    foreach (var t in stringData)
                    {
                        if (t.Equals("")) continue;
                        adjMatrix[i, realIndex] = Convert.ToInt32(t);
                        realIndex++;
                    }
                }

                _readGraph = new Graph(_graphSize, adjMatrix);
                Console.WriteLine("Finished reading graph from file.\nGraph:");
                _readGraph.Print();

                return _readGraph;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("Error was thrown: {0}.", exception.Message);
            }

            return null;
        }
    }
}