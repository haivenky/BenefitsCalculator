using Api.Models;

namespace Api.Provider
{
    /// <summary>
    /// Interface defining operations to interact with JSON data related to employees.
    /// </summary>
    public interface IJsonDataProvider
    {
        /// <summary>
        /// Retrieves a list of employees from the JSON data source asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of employees.</returns>
        Task<List<Employee>> GetEmployees();

        /// <summary>
        /// Retrieves a list of dependents from the JSON data source asynchronously, across all employees.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of dependents.</returns>
        Task<List<Dependent>> GetDependents();

        /// <summary>
        /// Saves the list of employees to the JSON data source asynchronously.
        /// </summary>
        /// <param name="employees">The list of employees to save.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SaveEmployees(List<Employee> employees);
    }
}
