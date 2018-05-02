using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AFVTry.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AFVTry.Logic
{
    public class CompleteGraph
    {
        public readonly float[,] _edges;
        private readonly int _count;

        public CompleteGraph(float[,] edges)
        {
            _edges = edges;
            _count = _edges.GetLength(0);
        }

        public float this[int start, int end]
        {
            get
            {
                return start <= end ? _edges[start, end] : _edges[end, start];
            }
            set
            {
                if (start <= end)
                    _edges[start, end] = value;
                else
                    _edges[end, start] = value;
            }
        }

        public float GetShortestPath(int start, int end)
        {
            var distances = new float[_count];
            var previous = new int[_count];

            for (int i = 0; i < _count; i++)
            {
                distances[i] = float.MaxValue;
                previous[i] = -1;
            }

            distances[start] = 0;
            previous[start] = start;

            var verticies = Enumerable.Range(0, _count).ToList();

            while (verticies.Count > 0)
            {
                int minVert = verticies[0];
                foreach (var vert in verticies)
                {
                    if (distances[vert] < distances[minVert])
                        minVert = vert;
                }

                verticies.Remove(minVert);
                foreach (var vert in verticies)
                {
                    var newDistance = distances[minVert] + this[minVert, vert];
                    if (distances[vert] > newDistance)
                        distances[vert] = newDistance;
                }
            }
            return distances[end];
        }
    }
}

