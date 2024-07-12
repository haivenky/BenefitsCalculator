using Api.Models;
using Api.Provider;
using Api.Services;
using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ApiTests.UnitTests.Services
{
    public class EmployeeServiceTests
    {
        private readonly IJsonDataProvider _mockJsonDataProvider;
        private readonly EmployeeService _employeeService;

        public EmployeeServiceTests()
        {
            _mockJsonDataProvider = Substitute.For<IJsonDataProvider>();
            _employeeService = new EmployeeService(_mockJsonDataProvider);
        }

        [Fact]
        public async Task GetEmployee_ExistingEmployee_ReturnsDto()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = 1, FirstName = "LeBron", LastName = "James", Salary = 50000 }
            };

            _mockJsonDataProvider.GetEmployees().Returns(Task.FromResult(employees));

            // Act
            var result = await _employeeService.GetEmployee(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("LeBron", result.FirstName);
            Assert.Equal("James", result.LastName);
        }

        [Fact]
        public async Task GetAllEmployees_ReturnsListOfDtos()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = 1, FirstName = "LeBron", LastName = "James", Salary = 50000 },
                new Employee { Id = 2, FirstName = "Ja", LastName = "Morant", Salary = 60000 }
            };

            _mockJsonDataProvider.GetEmployees().Returns(Task.FromResult(employees));

            // Act
            var results = await _employeeService.GetAllEmployees();

            // Assert
            Assert.NotNull(results);
            Assert.Equal(2, results.Count);
            Assert.Equal("LeBron", results[0].FirstName);
            Assert.Equal("James", results[0].LastName);
            Assert.Equal("Ja", results[1].FirstName);
            Assert.Equal("Morant", results[1].LastName);
        }

        [Fact]
        public async Task GetPaycheck_ExistingEmployee_ReturnsPaycheck()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = 1, FirstName = "LeBron", LastName = "James", Salary = 50000 }
            };

            _mockJsonDataProvider.GetEmployees().Returns(Task.FromResult(employees));

            // Act
            var result = await _employeeService.GetPaycheck(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.EmployeeId);
        }

        [Fact]
        public async Task GetPaycheck_ExistingEmployeeWithDependent_ReturnsPaycheck()
        {
            // Arrange
            var employee = new Employee
            {
                Id = 1,
                FirstName = "LeBron",
                LastName = "James",
                Salary = 120000, // Annual salary
                Dependents = new List<Dependent>
                {
                    new Dependent { Id = 1, FirstName = "Jane", LastName = "James", Relationship = Relationship.Spouse }
                }
            };

            var employees = new List<Employee> { employee };

            _mockJsonDataProvider.GetEmployees().Returns(Task.FromResult(employees));

            // Act
            var paycheck = await _employeeService.GetPaycheck(1);

            // Assert
            Assert.NotNull(paycheck);
            Assert.Equal(1, paycheck.EmployeeId);
            Assert.Equal(4615.38m, paycheck.GrossPay);
            Assert.Equal(923.08m, paycheck.Deductions);
            Assert.Equal(3692.31m, paycheck.NetPay);
        }
    }
}
