using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarChart.Data;
using StarChart.Models;

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
        public IActionResult GetByName(string name) // Could be Many..
        {
            //var celestialObject = _context.CelestialObjects.FirstOrDefault(co=> co.Name == name);
            var celestialObjects = _context.CelestialObjects.Where(co=> co.Name == name);
            if (!celestialObjects.Any())
            {
                return NotFound();
            }
            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == celestialObject.Id).ToList();
            }
            return Ok(celestialObjects.ToList());
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            //Ef core solution needs new migration var celestialObjects = _context.CelestialObjects.Include(co => co.Satellites);
            var celestialObjects = _context.CelestialObjects.ToList();
            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == celestialObject.Id).ToList();
            }
            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new {id= celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var existingObject = _context.CelestialObjects.Find(id);
            if (existingObject == null)
                return NotFound();
            existingObject.Name = celestialObject.Name;
            existingObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
            existingObject.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.CelestialObjects.Update(existingObject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if(celestialObject == null)
            {
                return NotFound();
            }
            celestialObject.Name = name; //No model State update
            // _context.Update(vehicle);
            // _context.Entry(vehicle).Property(v => v.ArrivalTime).IsModified = false; // Not affected by save changes.
            _context.Update(celestialObject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            //Remove Satellites as well without EF core.
            var celestialObjects = _context.CelestialObjects.Where(co => co.Id == id || co.OrbitedObjectId == id).ToList();
            if(!celestialObjects.Any())
            {
                return NotFound();
            }
            _context.RemoveRange(celestialObjects);
            _context.SaveChanges();
            return NoContent();
        }

    }

}
