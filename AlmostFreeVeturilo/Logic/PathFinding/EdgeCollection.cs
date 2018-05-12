using System;
using System.Collections.Generic;
using System.Linq;

namespace AlmostFreeVeturilo.Logic
{
    public class EdgeCollection
    {
        private Dictionary<int, Dictionary<int, float>> _weights = new Dictionary<int, Dictionary<int, float>>();
        private List<int> _vertices = new List<int>();

        public void AddEdge(int from, int to, float weight)
        {
            Adjust(ref from, ref to);

            if (!_vertices.Contains(to))
                _vertices.Add(to);

            if (!_vertices.Contains(from))
                _vertices.Add(from);

            if (!_weights.ContainsKey(from))
                _weights[from] = new Dictionary<int, float>();
            _weights[from][to] = weight;
        }

        public List<int> GetVertices()
        {
            return _vertices.ToList();
        }

        public float GetWeight(int from, int to)
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
