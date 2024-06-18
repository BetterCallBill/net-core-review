using CRUDDemo.Entities;
using CRUDDemo.DTO;
using CRUDDemo.Enums;
using CRUDDemo.Services;
using CRUDDemo.Interfaces;
using AutoFixture;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit.Abstractions;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        //private fields
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        private readonly Mock<IPersonsRepository> _personRepositoryMock;
        private readonly IPersonsRepository _personsRepository;

        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;

        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        //constructor
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();
            _personRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personRepositoryMock.Object;
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;

            var countriesInitialData = new List<Country>() { };
            var personsInitialData = new List<Person>() { };

            //Craete mock for DbContext
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
              new DbContextOptionsBuilder<ApplicationDbContext>().Options
             );

            //Access Mock DbContext object
            ApplicationDbContext dbContext = dbContextMock.Object;

            //Create mocks for DbSets
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

            //Create services based on mocked DbContext object
            _countriesService = new CountriesService(_countriesRepository);
            _personsService = new PersonsService(_personsRepository);

            _testOutputHelper = testOutputHelper;
        }


        #region AddPerson

        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            Func<Task> action = async () =>
            {
                await _personsService.AddPersonAsync(personAddRequest);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddPerson_ValidPerson_Success()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(x => x.Email, "test@test.com").Create();
            Person person = personAddRequest.ToPerson();
            PersonResponse personResponseExpected = person.ToPersonResponse();

            _personRepositoryMock.Setup(x => x.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Act
            PersonResponse personResponseActual = await _personsService.AddPersonAsync(personAddRequest);

            //Assert
            personResponseActual.Should().Be(personResponseExpected);
        }


        //When we supply null value as PersonName, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull_ThrowsArgumentException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(x => x.PersonName, null as string).Create();
            Person person = personAddRequest.ToPerson();

            _personRepositoryMock.Setup(x => x.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Act
            Func<Task> action = async () => await _personsService.AddPersonAsync(personAddRequest);

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        #endregion

        #region GetPersonByPersonID

        //If we supply null as PersonID, it should return null as PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_PersonIDIsNull_ToBeNull()
        {
            //Arrange
            Guid? personID = null;

            //Act
            PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonIDAsync(personID);

            //Assert
            person_response_from_get.Should().BeNull();
        }


        //If we supply a valid person id, it should return the valid person details as PersonResponse object
        [Fact]
        public async Task GetPersonByPersonID_ValidPersonID_Success()
        {
            Person person = _fixture.Build<Person>().With(x => x.Email, "test@test.com").Create();
            PersonResponse person_response_expected = person.ToPersonResponse();

            // mock
            _personRepositoryMock.Setup(x => x.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            //Act
            PersonResponse person_response_actual = await _personsService.GetPersonByPersonIDAsync(person.PersonID);

            //Assert
            person_response_actual.Should().Be(person_response_expected);
            Assert.NotNull(person_response_actual);
        }

        #endregion

        #region GetAllPersons

        //The GetAllPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPersons_ReturnsEmptyList()
        {
            //Arrange
            var persons = new List<Person>();

            _personRepositoryMock
             .Setup(temp => temp.GetAllPersons())
             .ReturnsAsync(persons);

            //Act
            List<PersonResponse> persons_from_get = await _personsService.GetAllPersonsAsync();

            //Assert
            persons_from_get.Should().BeEmpty();
            Assert.Empty(persons_from_get);
        }

        //First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
        [Fact]
        public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>() {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();


            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            //Act
            List<PersonResponse> persons_list_from_get = await _personsService.GetAllPersonsAsync();

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            persons_list_from_get.Should().BeEquivalentTo(person_response_list_expected);
        }

        #endregion

        #region GetFilteredPersons

        //If the search text is empty and search by is "PersonName", it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_Success()
        {
            //Arrange
            List<Person> persons = new List<Person>() {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create()
            };
            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            //mock
            _personRepositoryMock
             .Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
             .ReturnsAsync(persons);


            //Act
            List<PersonResponse> persons_list_from_get = await _personsService.GetFilteredPersonsAsync(string.Empty, string.Empty);

            //Assert
            persons_list_from_get.Should().BeEquivalentTo(person_response_list_expected);
        }


        //First we will add few persons; and then we will search based on person name with some search string. It should return the matching persons
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_Success()
        {
            //Arrange
            List<Person> persons = new List<Person>() {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            _personRepositoryMock.Setup(temp => temp
                .GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);

            //Act
            List<PersonResponse> persons_list_from_search = await _personsService.GetFilteredPersonsAsync(nameof(Person.PersonName), "sa");

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }

        #endregion

        #region GetSortedPersons

        //When we sort based on PersonName in DESC, it should return persons list in descending on PersonName
        [Fact]
        public async Task GetSortedPersons_SortByPersonNameDesc_Success()
        {
            //Arrange
            List<Person> persons = new List<Person>() {
                _fixture.Build<Person>()
                    .With(temp => temp.PersonName, "A")
                    .With(temp => temp.Email, "someone_1@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),

                _fixture.Build<Person>()
                    .With(temp => temp.PersonName, "B")
                    .With(temp => temp.Email, "someone_2@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),

                _fixture.Build<Person>()
                    .With(temp => temp.PersonName, "C")
                    .With(temp => temp.Email, "someone_3@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            //mock
            _personRepositoryMock
                .Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(persons);

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            List<PersonResponse> allPersons = await _personsService.GetAllPersonsAsync();

            //Act
            List<PersonResponse> persons_list_from_sort = await _personsService.GetSortedPersonsAsync(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            persons_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
        }
        #endregion

        #region UpdatePerson

        //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_PersonIsNull_ThrowsArgumentNullException()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = null;

            //Act
            Func<Task> action = async () => await _personsService.UpdatePersonAsync(person_update_request);

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }


        //When we supply invalid person id, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID_ThrowsArgumentException()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = _fixture.Build<PersonUpdateRequest>().Create();

            //Act
            Func<Task> action = async () => await _personsService.UpdatePersonAsync(person_update_request);

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }


        //When PersonName is null, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull_ThrowsArgumentException()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = _fixture.Build<PersonUpdateRequest>()
                .With(temp => temp.PersonName, null as string)
                .With(temp => temp.Email, "abc@example.com")
                .Create();

            //Assert
            Func<Task> action = async () => await _personsService.UpdatePersonAsync(person_update_request);

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdatePerson_ValidPerson_Success()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "someone@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_expected.ToPersonUpdateRequest();

            _personRepositoryMock
                .Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            _personRepositoryMock
                .Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_update = await _personsService.UpdatePersonAsync(person_update_request);

            //Assert
            person_response_from_update.Should().Be(person_response_expected);
        }

        #endregion

        #region DeletePerson

        //If you supply an valid PersonID, it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonID_ReturnsTrue()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "someone@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Female")
                .Create();

            _personRepositoryMock.Setup(x => x.DeletePersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(true); //DELETE()

            //Act
            bool isDeleted = await _personsService.DeletePersonAsync(person.PersonID);

            //Assert
            isDeleted.Should().BeTrue();
        }


        //If you supply an invalid PersonID, it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID_ReturnsFalse()
        {
            //Act
            bool isDeleted = await _personsService.DeletePersonAsync(Guid.NewGuid());

            //Assert
            isDeleted.Should().BeFalse();
        }

        #endregion
    }
}