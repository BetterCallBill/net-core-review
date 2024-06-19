using AutoFixture;
using CRUDDemo.Controllers;
using CRUDDemo.DTO;
using CRUDDemo.Enums;
using CRUDDemo.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CRUDTests
{
    public class PersonsControllerTest
    {
        //private fields
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly Mock<IPersonsService> _personsServiceMock;
        private readonly IFixture _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();
            _personsServiceMock = new Mock<IPersonsService>();
            _personsService = _personsServiceMock.Object;
            _countriesServiceMock = new Mock<ICountriesService>();
            _countriesService = _countriesServiceMock.Object;
        }

        #region Index

        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            // Arrange
            List<PersonResponse> persons_response_list = _fixture.Create<List<PersonResponse>>();

            PersonsController personsController = new PersonsController(_personsService, _countriesService);

            _personsServiceMock
             .Setup(temp => temp.GetFilteredPersonsAsync(It.IsAny<string>(), It.IsAny<string>()))
             .ReturnsAsync(persons_response_list);

            _personsServiceMock
             .Setup(temp => temp.GetSortedPersonsAsync(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>()))
             .ReturnsAsync(persons_response_list);

            // Act
            // PersonsController.Index(string searchBy, string? searchString, [string sortBy = "PersonName"], [SortOrderOptions sortOrder = SortOrderOptions.ASC])
            IActionResult result = await personsController.Index(
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<SortOrderOptions>()
            );

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.ViewData.Model.Should().Be(persons_response_list);
        }

        #endregion

        #region create

        [Fact]
        public async void Create_IfModelErrors_ToReturnCreateView()
        {
            //Arrange
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();

            PersonResponse person_response = _fixture.Create<PersonResponse>();

            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock
             .Setup(temp => temp.GetAllCountriesAsync())
             .ReturnsAsync(countries);

            _personsServiceMock
             .Setup(temp => temp.AddPersonAsync(It.IsAny<PersonAddRequest>()))
             .ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(_personsService, _countriesService);


            //Act
            personsController.ModelState.AddModelError("PersonName", "Person Name can't be blank");

            IActionResult result = await personsController.Create(person_add_request);

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();
            viewResult.ViewData.Model.Should().Be(person_add_request);
        }


        [Fact]
        public async void Create_IfNoModelErrors_ToReturnRedirectToIndex()
        {
            //Arrange
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();

            PersonResponse person_response = _fixture.Create<PersonResponse>();

            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock
             .Setup(temp => temp.GetAllCountriesAsync())
             .ReturnsAsync(countries);

            _personsServiceMock
             .Setup(temp => temp.AddPersonAsync(It.IsAny<PersonAddRequest>()))
             .ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(_personsService, _countriesService);

            //Act
            IActionResult result = await personsController.Create(person_add_request);

            //Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            redirectResult.ActionName.Should().Be("Index");
        }

        #endregion
    }
}