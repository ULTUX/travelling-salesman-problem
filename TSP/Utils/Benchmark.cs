using System;
using System.Diagnostics;
using System.Threading;
using TSP.Algorithms;

namespace TSP.Utils
{
    public class Benchmark
    {
        private readonly TspAlgorithm _algorithm;
        private readonly int _endSize;
        private readonly int _iterationCount;
        private readonly int _repeats;
        private readonly int _startSize;
        private readonly int _step;
        private readonly float _stepMultiplier;
        private readonly Stopwatch _timer = new();
        private int _currSize;

        public Benchmark(int startSize, int endSize, int step, float stepMultiplier, TspAlgorithm algorithm,
            int repeats)
        {
            _startSize = startSize;
            _endSize = endSize;
            _step = step;
            _stepMultiplier = stepMultiplier;
            _algorithm = algorithm;
            _currSize = startSize;
            _repeats = repeats;

            //calculate iteration count
            var n = 0;
            var size = _startSize;


            while (size < endSize)
            {
                size = (int) (size * _stepMultiplier);
                size += _step;
                n++;
            }

            _iterationCount = n;
        }


        public void Start()
        {
            Console.WriteLine("Starting bechmark, starting size: {0}, ending size: {1}, step: {2}, multiplier: {3}...",
                _startSize, _endSize, _step, _stepMultiplier);
            Console.WriteLine("Calculated iteration count: {0}.", _iterationCount);
            Thread.Sleep(1000);

            var data = new float[_iterationCount, 2];
            string[] headers = {"Size", "Elapsed time[ms]"};
            var i = 0;

            while (_currSize < _endSize)
            {
                Console.WriteLine("{0:0.00}% Testing for size: {1}...", (float) i / _iterationCount * 100, _currSize);
                //Start benchmark:
                double avg = 0;
                for (var j = 0; j < _repeats; j++)
                {
                    Console.WriteLine("\t{0:0.00}% total completed, current size: {1}.",
                        (float) (j + i * _repeats) / (_repeats * _iterationCount) * 100, _currSize);
                    GenerateRandomGraphData(_currSize);
                    _timer.Start();
                    _algorithm.Start();
                    _timer.Stop();
                    avg += _timer.Elapsed.TotalMilliseconds;
                }

                avg /= _repeats;
                data[i, 0] = _currSize;
                data[i, 1] = (float) avg;

                //Change iteration state:                
                _currSize = (int) (_currSize * _stepMultiplier);
                _currSize += _step;
                i++;
            }

            var fileWriter = new CsvWriter(DateTime.Now.ToString("yy-MM-dd-HH-mm-ss") + ".csv", headers, data);
            fileWriter.WriteFile();
        }

        private void GenerateRandomGraphData(int size)
        {
            var generator = new RandomGraphGenerator(false, 10, 20, size, null);
            var newGraph = generator.GenerateRandomGraph();
            _algorithm.Graph = newGraph;
        }
    }
}