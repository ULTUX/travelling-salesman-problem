using System;
using System.IO;
using System.Linq;

namespace TSP.Utils
{
    public class ATSPReader
    {
        private readonly string _fileName;
        private int _dimension;
        private string _graphName;
        private Graph _readGraph;
        private string _type;

        public ATSPReader(string fileName)
        {
            _fileName = fileName;
        }

        public Graph ReadFile()
        {
            try
            {
                Console.WriteLine("Reading data from file: {0}...", _fileName);
                var lines = File.ReadAllLines(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar +
                                              _fileName);
                _graphName = lines[0].Split(": ")[1];
                _type = lines[1].Split(": ")[1];
                _dimension = int.Parse(lines[3].Split(": ")[1]);
                var graphData = lines.Skip(7).ToArray();
                Console.WriteLine("Read metadata:\n\tgraphSize: {0}\n\tgraphName: {1}\n\ttype: {2}", _dimension, _graphName,
                    _type);
                var i = 0;
                var currRow = 0;
                var currCol = 0;
                var graph = new int[_dimension, _dimension];
                while (graphData[i] != "EOF")
                {
                    var lineData = graphData[i].Split(" ");
                    foreach (var s in lineData)
                    {
                        if (s == "") continue;
                        graph[currRow, currCol] = int.Parse(s);
                        currCol++;
                        if (currCol != _dimension) continue;
                        currCol = 0;
                        currRow++;
                    }

                    i++;
                }

                var graphObj = new Graph(_dimension, graph);
                graphObj.Print();
                return graphObj;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("Error was thrown: {0}.", exception.Message);
            }

            return null;
        }
    }
}