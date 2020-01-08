using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beerhall.Models.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Beerhall.Controllers
{
    public class BrewerController : Controller
    {
        private readonly IBrewerRepository _brewerRepository;

        public BrewerController(IBrewerRepository brewerRepository)
        {
            _brewerRepository = brewerRepository;
        }
        public IActionResult Index()
        {
            IEnumerable<Brewer> brewers = _brewerRepository.GetAll();
            return View(brewers);
        }
    }
}