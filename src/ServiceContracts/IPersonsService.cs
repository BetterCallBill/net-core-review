using ServiceContracts.DTO;
using ServiceContracts.DTO.Enums;

namespace ServiceContracts;

/// <summary>
/// Represents business logic for manipulating Perosn entity
/// </summary>
public interface IPersonsService
{
    /// <summary>
    /// Addds a new person into the list of persons
    /// </summary>
    /// <param name="personAddRequest">Person to add</param>
    /// <returns>Returns the same person details, along with newly generated PersonID</returns>
    PersonResponse AddPerson(PersonAddRequest? personAddRequest);

    /// <summary>
    /// Returns all persons
    /// </summary>
    /// <returns>Returns a list of objects of PersonResponse type</returns>
    List<PersonResponse> GetAllPersons();

    PersonResponse? GetPersonByPersonID(Guid? personID);

    List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString);

    List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);
    PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest);
}