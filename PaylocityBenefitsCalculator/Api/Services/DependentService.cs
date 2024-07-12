using Api.Dtos.Dependent;
using Api.Models;
using Api.Provider;
using Api.Services.Interface;

namespace Api.Services
{
    /// <summary>
    /// Service class that handles operations related to dependents.
    /// </summary>
    public class DependentService : IDependentService
    {
        private readonly IJsonDataProvider _jsonDataProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependentService"/> class.
        /// </summary>
        /// <param name="jsonDataProvider">The JSON data provider to retrieve dependents data.</param>
        public DependentService(IJsonDataProvider jsonDataProvider)
        {
            _jsonDataProvider = jsonDataProvider ?? throw new ArgumentNullException(nameof(jsonDataProvider));
        }


        /// <summary>
        /// Retrieves the details of a specific dependent by ID.
        /// </summary>
        /// <param name="id">The ID of the dependent.</param>
        /// <returns>A Dependent DTO if found, otherwise null.</returns>
        public async Task<GetDependentDto> GetDependent(int id)
        {
            try
            {
                var dependents = await _jsonDataProvider.GetDependents();
                var dependent = dependents.FirstOrDefault(d => d.Id == id);

                // Return null if dependent with specified ID is not found
                return dependent == null ? null : MapToGetDependentDto(dependent);
            }
            catch (Exception ex)
            {
                // Handle or log any exceptions that occur during retrieval
                throw new ApplicationException("Error occurred while retrieving dependent.", ex);
            }
        }

        /// <summary>
        /// Retrieves the list of all dependents.
        /// </summary>
        /// <returns>A list of all dependents.</returns>
        public async Task<List<GetDependentDto>> GetAllDependents()
        {
            try
            {
                var dependents = await _jsonDataProvider.GetDependents();
                return dependents.Select(MapToGetDependentDto).ToList();
            }
            catch (Exception ex)
            {
                // Handle or log any exceptions that occur during retrieval
                throw new ApplicationException("Error occurred while retrieving dependents.", ex);
            }
        }

        #region Private Methods
        /// <summary>
        /// Maps an Dependent object to a GetDependentDto object.
        /// </summary>
        /// <param name="dependent">The Dependent object to map.</param>
        /// <returns>A GetDependentDto object mapped from the Dependent.</returns>
        private GetDependentDto MapToGetDependentDto(Dependent dependent)
        {
            return new GetDependentDto
            {
                Id = dependent.Id,
                FirstName = dependent.FirstName,
                LastName = dependent.LastName,
                DateOfBirth = dependent.DateOfBirth.Date,
                Relationship = dependent.Relationship
            };
        } 
        #endregion
    }
}
