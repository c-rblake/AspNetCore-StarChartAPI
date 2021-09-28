using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarChart.Models
{
    public class CelestialObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? OrbitedObjectId { get; set; }

        public List<CelestialObject> Satellites { get; set; }

        public TimeSpan OribtalPeriod { get; set; }
    }
}
