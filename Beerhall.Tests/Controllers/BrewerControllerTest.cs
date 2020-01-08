using Beerhall.Controllers;
using Beerhall.Models.Domain;
using Beerhall.Models.ViewModels;
using Beerhall.Tests.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Beerhall.Tests.Controllers
{
    public class BrewerControllerTest
    {
        private BrewerController _controller;
        private Mock<IBrewerRepository> _brewerRepository;
        private Mock<ILocationRepository> _locationRepository;
        private readonly DummyApplicationDbContext _dummyContext;

        public BrewerControllerTest()
        {
            _dummyContext = new DummyApplicationDbContext();
            _brewerRepository = new Mock<IBrewerRepository>();
            _locationRepository = new Mock<ILocationRepository>();
            _controller = new BrewerController(_brewerRepository.Object, _locationRepository.Object);
            _controller.TempData = new Mock<ITempDataDictionary>().Object;
        }

        [Fact]
        public void Index_PassesOrderedListOfBrewersInViewResultModelAndStoresTotalTurnoverInViewData()
        {
            //Arrange
            Brewer bavik = new Brewer("Bavik") { BrewerId = 1 };
            Brewer moortgat = new Brewer("Duvel Moortgat") { BrewerId = 2 };
            _brewerRepository.Setup(m => m.GetAll()).Returns(new List<Brewer>() { bavik, moortgat });

            //Act
            var result = Assert.IsType<ViewResult>(_controller.Index());

            //Assert
            var brewersInModel = Assert.IsType<List<Brewer>>(result.Model);
            Assert.Equal(3, brewersInModel.Count);
            Assert.Equal("Bavik", brewersInModel[0].Name);
            Assert.Equal("De Leeuw", brewersInModel[1].Name);
            Assert.Equal("Duvel Moortgat", brewersInModel[2].Name);
            Assert.Equal(20050000, result.ViewData["TotalTurnover"]);
        }

        [Fact]
        public void Edit_ValidEdit_UpdatesAndPersistsBrewerAndRedirectsToActionIndex()
        {
            _brewerRepository.Setup(m => m.GetBy(1)).Returns(_dummyContext.Bavik);
            var brewerEvm = new BrewerEditViewModel(_dummyContext.Bavik)
            {
                Street = "nieuwe straat 1"
            };
            var result = Assert.IsType<RedirectToActionResult>(_controller.Edit(brewerEvm, 1));
            var bavik = _dummyContext.Bavik;
            Assert.Equal("Index", result?.ActionName);
            Assert.Equal("Bavik", bavik.Name);
            Assert.Equal("nieuwe straat 1", bavik.Street);
            _brewerRepository.Verify(m => m.SaveChanges(), Times.Once());
        }
    }
}
