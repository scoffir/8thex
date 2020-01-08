using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beerhall.Models.Domain;
using Beerhall.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Beerhall.Controllers
{
    public class BrewerController : Controller
    {
        private readonly IBrewerRepository _brewerRepository;
        private readonly ILocationRepository _locationRepository;

        public BrewerController(IBrewerRepository brewerRepository, ILocationRepository locationRepository)
        {
            _brewerRepository = brewerRepository;
            _locationRepository = locationRepository;
        }
        public IActionResult Index()
        {
            IEnumerable<Brewer> brewers = _brewerRepository.GetAll().OrderBy(b => b.Name).ToList();
            ViewData["TotalTurnover"] = brewers.Sum(b => b.Turnover);
            return View(brewers);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Brewer brewer = _brewerRepository.GetBy(id);
            ViewData["Locations"] = new SelectList(
                _locationRepository.GetAll().OrderBy(l => l.Name),
                nameof(Location.PostalCode),
                nameof(Location.Name));
            return View(new BrewerEditViewModel(brewer));
        }

        [HttpPost]
        public IActionResult Edit(BrewerEditViewModel brewerEditViewModel, int id)
        {
            Brewer brewer = null;
            try
            {
                brewer = _brewerRepository.GetBy(id);
                MapBrewerEditViewModelToBrewer(brewerEditViewModel, brewer);
                _brewerRepository.SaveChanges();
                TempData["message"] = $"You successfully edited brewer {brewer.Name}.";
            }
            catch
            {
                TempData["error"] = $"Sorry, something went wrong, brewer {brewer?.Name} was not edited...";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["IsEdit"] = false;
            ViewData["Locations"] = GetLocationsAsSelectList();
            return View(nameof(Edit), new BrewerEditViewModel());
        }

        [HttpPost]
        public IActionResult Create(BrewerEditViewModel brewerEditViewModel)
        {
            Brewer brewer = null;
            try
            {
                brewer = new Brewer(brewerEditViewModel.Name);
                MapBrewerEditViewModelToBrewer(brewerEditViewModel, brewer);
                _brewerRepository.Add(brewer);
                _brewerRepository.SaveChanges();
                TempData["message"] = $"You successfully created brewer {brewer.Name}.";
            }
            catch
            {
                TempData["error"] = $"Sorry, something went wrong, brewer {brewer?.Name} was not created...";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            ViewData[nameof(Brewer.Name)] = _brewerRepository.GetBy(id).Name;
            return View();
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            Brewer brewer = null;
            try
            {
                brewer = _brewerRepository.GetBy(id);
                _brewerRepository.Delete(brewer);
                _brewerRepository.SaveChanges();
                TempData["message"] = $"You successfully deleted brewer {brewer.Name}.";
            }
            catch
            {
                TempData["error"] = $"Sorry, something went wrong, brewer {brewer?.Name} was not deleted...";
            }
            return RedirectToAction(nameof(Index));
        }

        private void MapBrewerEditViewModelToBrewer(BrewerEditViewModel brewerEditViewModel, Brewer brewer)
        {
            brewer.Name = brewerEditViewModel.Name;
            brewer.Street = brewerEditViewModel.Street;
            brewer.Location = brewerEditViewModel.PostalCode == null
                ? null
                : _locationRepository.GetBy(brewerEditViewModel.PostalCode);
            brewer.Turnover = brewerEditViewModel.Turnover;
        }

        private object GetLocationsAsSelectList()
        {
            return new SelectList(
                _locationRepository.GetAll().OrderBy(l => l.Name),
                nameof(Location.PostalCode),
                nameof(Location.Name));
        }
    }
}