using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace AlmostFreeVeturilo.Logic
{
    public class Graph
    {
        private readonly EdgeCollection _edges;
        private readonly List<int> _vertices;

        public Graph(EdgeCollection edges)
        {
            _edges = edges;
            _vertices = _edges.GetVertices();
        }

        public (List<int> uids, float cost) GetShortestPath(int start, int end)
        {
            PrepareTables(out var distances, out var previous, start);

            var vertsToUse = Enumerable.Range(0, _vertices.Count).ToList();

            while (vertsToUse.Count > 0)
            {
                int minVert = vertsToUse[0];
                foreach (var vert in vertsToUse)
                {
                    if (distances[vert] < distances[minVert])
                        minVert = vert;
                }

                vertsToUse.Remove(minVert);

                foreach (var vert in vertsToUse)
                {
          
                    var newDistance = distances[minVert] + _edges.GetWeight(_vertices[minVert], _vertices[vert]);
                    if (distances[vert] > newDistance)
                    {
                        distances[vert] = newDistance;
                        previous[vert] = minVert;
                    }
                }
            }

            var path = GetPath(previous, end);
            return path;
        }

        private void PrepareTables(out float[] distances, out int[] previous, int start)
        {
            distances = new float[_vertices.Count];
            previous = new int[_vertices.Count];

            for (int i = 0; i < _vertices.Count; i++)
            {
                distances[i] = float.MaxValue;
                previous[i] = -1;
            }

            var startVert = _vertices.IndexOf(start);
            distances[startVert] = 0;
            //previous[startVert] = startVert;

        }

        private (List<int> uids, float cost) GetPath(int[] previous, int end)
        {
            var path = new List<int>();
            var v = _vertices.IndexOf(end);
            while (v != -1)
            {
                path.Add(v);
                v = previous[v];
            }
            path.Reverse();

            var cost = path.Skip(1).Select(vert => _edges.GetWeight(_vertices[previous[vert]], _vertices[vert])-Common.ChangeBikePenalty).Sum();
            var uids = path.Select(vert => _vertices[vert]).ToList();

            return (uids, cost);
        }
    }
}

