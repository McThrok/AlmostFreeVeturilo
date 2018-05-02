using System.Collections.Generic;

namespace AlmostFreeVeturilo.Logic
{
    public class EdgeCollections
    {
        private Dictionary<int, Dictionary<int, float>> _weights = new Dictionary<int, Dictionary<int, float>>();

        public void AddEdge(int from, int to, float weight)
        {
            Adjust(ref from, ref to);
            if (!_weights.ContainsKey(from))
                _weights[from] = new Dictionary<int, float>();
            _weights[from][to] = weight;
        }

        public float GetWeght(int from, int to)
        {
            Adjust(ref from, ref to);
            return _weights[from][to];
        }

        private void Adjust(ref int from, ref int to)
        {
            if (from > to)
            {
                var x = from;
                from = to;
                to = x;
            }
        }

    }
}
