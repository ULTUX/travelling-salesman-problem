using TSP.Algorithms;
using TSP.Utils;

namespace TSP
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // new Menu();

            // new TabuSearch(new ATSPReader("ftv47.atsp").ReadFile(), 0).Start();
            new SimulatedAnnealing(new ATSPReader("ftv47.atsp").ReadFile(), 0).Start();

            // new BranchNBoundMin(new ATSPReader("rbg403.atsp").ReadFile(), 0).Start();
        }
    }
}