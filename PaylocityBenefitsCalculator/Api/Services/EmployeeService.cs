using Api.Dtos.Employee;
using Api.Models;
using Api.Dtos.Dependent;
using Api.Services.Interface;
using Api.Provider;

namespace Api.Services
{
    /// <summary>
    /// Service class that handles operations related to employees.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly IJsonDataProvider _jsonDataProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeService"/> class.
        /// </summary>
        /// <param name="jsonDataProvider">The JSON data provider to retrieve and save employee data.</param>
        public EmployeeService(IJsonDataProvider jsonDataProvider)
        {
            _jsonDataProvider = jsonDataProvider ?? throw new ArgumentNullException(nameof(jsonDataProvider));
        }

        /// <summary>
        /// Retrieves the details of a specific employee by ID.
        /// </summary>
        /// <param name="id">The ID of the employee.</param>
        /// <returns>An Employee DTO if found, otherwise null.</returns>
        public async Task<GetEmployeeDto> GetEmployee(int id)
        {
            try
            {
                // Retrieve all employees from the data provider
                var employees = await _jsonDataProvider.GetEmployees();
                var employee = employees.FirstOrDefault(e => e.Id == id);

                // If employee not found, return null
                if (employee == null)
                    return null;

                // Map Employee to GetEmployeeDto
                return MapToGetEmployeeDto(employee);
            }
            catch (Exception ex)
            {
                // Handle or log any exceptions that occur during retrieval
                throw new ApplicationException("Error occurred while retrieving employee.", ex);
            }
            
        }

        /// <summary>
        /// Retrieves the list of all employees.
        /// </summary>
        /// <returns>A list of all employees.</returns>
        public async Task<List<GetEmployeeDto>> GetAllEmployees()
        {
            try
            {
                var employees = await _jsonDataProvider.GetEmployees();

                // Map employees to GetEmployeeDto list
                return employees.Select(MapToGetEmployeeDto).ToList();
            }
            catch (Exception ex)
            {
                // Handle or log any exceptions that occur during retrieval
                throw new ApplicationException("Error occurred while retrieving employees.", ex);
            }
            
        }

        /// <summary>
        /// Adds or updates an employee.
        /// </summary>
        /// <param name="employee">The employee object to add or update.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public async Task<bool> AddOrUpdateEmployee(Employee employee)
        {
            // Validate that the employee has valid dependents
            if (!employee.HasValidDependents())
            {
                throw new InvalidOperationException("An employee may only have one spouse or domestic partner, not both.");
            }

            try
            {
                var employees = await _jsonDataProvider.GetEmployees();
                var existingEmployee = employees.FirstOrDefault(e => e.Id == employee.Id);

                if (existingEmployee == null)
                {
                    employees.Add(employee);
                }
                else
                {
                    UpdateEmployeeDetails(existingEmployee, employee);
                }

                await _jsonDataProvider.SaveEmployees(employees);
                return true;
            }
            catch (Exception ex)
            {
                // Handle or log any exceptions that occur during add/update
                throw new ApplicationException("Error occurred while adding or updating employee.", ex);
            }
        }

        /// <summary>
        /// Calculates the paycheck details for an employee based on their ID.
        /// </summary>
        /// <param name="id">The ID of the employee.</param>
        /// <returns>A PayCheck object containing the paycheck details or null if the employee is not found.</returns>
        public async Task<PayCheck> GetPaycheck(int id)
        {
            try
            {
                var employees = await _jsonDataProvider.GetEmployees();
                var employee = employees.FirstOrDefault(e => e.Id == id);

                if (employee == null)
                {
                    return null;
                }

                return CalculatePaycheck(employee);
            }
            catch (Exception ex)
            {
                // Handle or log any exceptions that occur during paycheck calculation
                throw new ApplicationException("Error occurred while calculating paycheck.", ex);
            }
        }

        #region Private Methods
        /// <summary>
        /// Maps an Employee object to a GetEmployeeDto object.
        /// </summary>
        /// <param name="employee">The Employee object to map.</param>
        /// <returns>A GetEmployeeDto object mapped from the Employee.</returns>
        private GetEmployeeDto MapToGetEmployeeDto(Employee employee)
        {
            return new GetEmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Salary = employee.Salary,
                DateOfBirth = employee.DateOfBirth,
                Dependents = employee.Dependents.Select(MapToGetDependentDto).ToList()
            };
        }

        /// <summary>
        /// Maps a Dependent object to a GetDependentDto object.
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
                DateOfBirth = dependent.DateOfBirth,
                Relationship = dependent.Relationship
            };
        }

        /// <summary>
        /// Updates the details of an existing employee with new data.
        /// </summary>
        /// <param name="existingEmployee">The existing Employee object to update.</param>
        /// <param name="newEmployee">The new Employee object with updated data.</param>
        private void UpdateEmployeeDetails(Employee existingEmployee, Employee newEmployee)
        {
            existingEmployee.FirstName = newEmployee.FirstName;
            existingEmployee.LastName = newEmployee.LastName;
            existingEmployee.Salary = newEmployee.Salary;
            existingEmployee.DateOfBirth = newEmployee.DateOfBirth;
            existingEmployee.Dependents = newEmployee.Dependents;
        }

        /// <summary>
        /// Calculates the paycheck details for an employee.
        /// </summary>
        /// <param name="employee">The Employee object for which to calculate the paycheck.</param>
        /// <returns>A PayCheck object containing the calculated paycheck details.</returns>
        private PayCheck CalculatePaycheck(Employee employee)
        {
            // Calculate the base annual cost for the employee
            decimal baseAnnualCost = Constants.BaseCostPerMonth * Constants.MonthsInYear;

            // Calculate the annual cost for all dependents
            decimal dependentAnnualCost = employee.Dependents.Sum(d => (Constants.DependentCostPerMonth * Constants.MonthsInYear) +
                                                (d.Age > Constants.DependentAgeThreshold ? Constants.AdditionalCostForOldDependents * Constants.MonthsInYear : 0));
            
            // Calculate the additional salary cost for high earners
            decimal salaryCost = employee.Salary > Constants.HighSalaryThreshold ? employee.Salary * Constants.HighSalaryAdditionalCostRate : 0;

            // Calculate the total annual benefit cost
            decimal totalAnnualCost = baseAnnualCost + dependentAnnualCost + salaryCost;

            // Calculate the annual salary and the amount per paycheck
            decimal annualSalary = employee.Salary;
            decimal paycheckAmount = annualSalary / Constants.PaychecksPerYear;

            // Calculate the deductions per paycheck and the net pay
            decimal deductions = totalAnnualCost / Constants.PaychecksPerYear;
            decimal netPay = paycheckAmount - deductions;

            // Round the values to 2 decimal places
            paycheckAmount = Math.Round(paycheckAmount, 2);
            deductions = Math.Round(deductions, 2);
            netPay = Math.Round(netPay, 2);

            // Return the calculated paycheck details
            return new PayCheck
            {
                EmployeeId = employee.Id,
                GrossPay = paycheckAmount,
                Deductions = deductions,
                NetPay = netPay
            };
        }
        #endregion
    }
}
