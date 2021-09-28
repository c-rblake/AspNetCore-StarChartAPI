using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarChart.Data;

namespace StarChart.Controllers
{
    [Route(""), ApiController]
    public class CelestialObjectController : ControllerBase //Api Controller
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);

            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == id).ToList();

            return Ok(celestialObject); //Not Async
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(co=> co.Name == name);

            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == celestialObject.Id).ToList();

            return Ok(celestialObject);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.Include(co => co.Satellites);
            return Ok(celestialObjects);
        }

    }

}
