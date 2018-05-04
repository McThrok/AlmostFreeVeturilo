using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlmostFreeVeturilo.Logic;
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
        private PathFinder _pathFinder = new PathFinder();

        [HttpGet("{lat}/{lng}")]
        public async Task<List<PathPart>> Get(float lat, float lng)
        {
            return await _pathFinder.GetStartStations(lat, lng);
        }

        [HttpGet("{uid}/{lat}/{lng}")]
        public async Task<List<PathPart>> Get(int uid, float lat, float lng)
        {
            return await _pathFinder.GetPath(uid, lat, lng);
        }
    }
}
