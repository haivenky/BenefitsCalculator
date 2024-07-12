using Api.Dtos.Employee;
using Api.Models;

namespace Api.Services.Interface
{
    /// <summary>
    /// Interface defining operations related to employees.
    /// </summary>
    public interface IEmployeeService
    {
        /// <summary>
        /// Retrieves the details of a specific employee by ID.
        /// </summary>
        /// <param name="id">The ID of the employee.</param>
        /// <returns>An Employee DTO if found, otherwise null.</returns>
        Task<GetEmployeeDto> GetEmployee(int id);

        /// <summary>
        /// Retrieves the list of all employees.
        /// </summary>
        /// <returns>A list of all employees.</returns>
        Task<List<GetEmployeeDto>> GetAllEmployees();

        /// <summary>
        /// Adds or updates an employee.
        /// </summary>
        /// <param name="employee">The employee object to add or update.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        Task<bool> AddOrUpdateEmployee(Employee employee);

        /// <summary>
        /// Calculates the paycheck details for an employee based on their ID.
        /// </summary>
        /// <param name="id">The ID of the employee.</param>
        /// <returns>A PayCheck object containing the paycheck details or null if the employee is not found.</returns>
        Task<PayCheck> GetPaycheck(int id);
    }
}
