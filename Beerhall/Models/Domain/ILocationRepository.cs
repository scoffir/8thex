using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beerhall.Models.Domain
{
    public interface ILocationRepository
    {
        Location GetBy(string postalCode);
        IEnumerable<Location> GetAll();
    }
}
