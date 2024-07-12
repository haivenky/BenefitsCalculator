using Api.Models;
using Api.Provider;
using Api.Services;
using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ApiTests.UnitTests.Services
{
    public class DependentServiceTests
    {
        private readonly IJsonDataProvider _mockJsonDataProvider;
        private readonly DependentService _dependentService;

        public DependentServiceTests()
        {
            _mockJsonDataProvider = Substitute.For<IJsonDataProvider>();
            _dependentService = new DependentService(_mockJsonDataProvider);
        }

        [Fact]
        public async Task GetDependent_ExistingDependent_ReturnsDto()
        {
            // Arrange
            var dependents = new List<Dependent>
            {
                new Dependent { Id = 1, FirstName = "Alice", LastName = "Morant", Relationship = Relationship.Child }
            };

            _mockJsonDataProvider.GetDependents().Returns(Task.FromResult(dependents));

            // Act
            var result = await _dependentService.GetDependent(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Alice", result.FirstName);
            Assert.Equal("Morant", result.LastName);
        }

        [Fact]
        public async Task GetAllDependents_ReturnsListOfDtos()
        {
            // Arrange
            var dependents = new List<Dependent>
            {
                new Dependent { Id = 1, FirstName = "Alice", LastName = "Morant", Relationship = Relationship.Spouse },
                new Dependent { Id = 2, FirstName = "Rex", LastName = "Morant", Relationship = Relationship.Child }
            };

            _mockJsonDataProvider.GetDependents().Returns(Task.FromResult(dependents));

            // Act
            var results = await _dependentService.GetAllDependents();

            // Assert
            Assert.NotNull(results);
            Assert.Equal(2, results.Count);
            Assert.Equal("Alice", results[0].FirstName);
            Assert.Equal("Morant", results[0].LastName);
            Assert.Equal("Rex", results[1].FirstName);
            Assert.Equal("Morant", results[1].LastName);
        }
    }
}
