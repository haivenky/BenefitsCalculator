using Api.Dtos.Dependent;

namespace Api.Services.Interface
{
    /// <summary>
    /// Interface defining operations related to dependents.
    /// </summary>
    public interface IDependentService
    {
        /// <summary>
        /// Retrieves the details of a specific dependent by ID.
        /// </summary>
        /// <param name="id">The ID of the dependent.</param>
        /// <returns>A Dependent DTO if found, otherwise null.</returns>
        Task<GetDependentDto> GetDependent(int id);

        /// <summary>
        /// Retrieves the list of all dependents.
        /// </summary>
        /// <returns>A list of all dependents.</returns>
        Task<List<GetDependentDto>> GetAllDependents();
    }
}
