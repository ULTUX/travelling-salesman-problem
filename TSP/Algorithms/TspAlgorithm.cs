using TSP.Utils;

namespace TSP.Algorithms
{
    public abstract class TspAlgorithm
    {
        protected Graph _graph;
        protected int _startVertex;
        protected bool IsBenchmark;

        protected TspAlgorithm(Graph graph, int startVertex)
        {
            _graph = graph;
            _startVertex = startVertex;
        }

        protected TspAlgorithm()
        {
            IsBenchmark = true;
        }

        public int StartVertex
        {
            get => _startVertex;
            set => _startVertex = value;
        }


        public Graph Graph
        {
            get => _graph;
            set => _graph = value;
        }

        public abstract void Start();
    }
}