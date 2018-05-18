using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlmostFreeVeturilo.Logic;
using AlmostFreeVeturilo.Logic.VeturiloApi;
using AlmostFreeVeturilo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;

namespace AlmostFreeVeturilo.Controllers
{
    //[Produces("application/json")]
    [Route("api/Path")]
    public class PathController : Controller
    {
        private readonly PathFinder _pathFinder = new PathFinder();
        private readonly StartStationsFinder _startStationsFinder = new StartStationsFinder();
        

        [HttpGet("{lat}/{lng}")]
        public async Task<List<PathPart>> GetStartStations(float lat, float lng)
        {
            return await _startStationsFinder.GetStartStations(lat, lng);
        }

        [HttpGet("{uid}/{lat}/{lng}")]
        public async Task<VeturiloPath> GetPath(int uid, float lat, float lng)
        {
            return await _pathFinder.GetPath(uid, lat, lng);
        }

        [HttpGet("")]
        public async Task<List<Station>> Get()
        {
            var places = await VeturiloProxy.Instance.GetVeturiloPlaces();
            var stations = places.Select(p => new Station(p)).ToList();
            return stations;
        }
    }
}
