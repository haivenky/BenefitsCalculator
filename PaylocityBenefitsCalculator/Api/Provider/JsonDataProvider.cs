using Api.Configuration;
using Api.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api.Provider
{
    /// <summary>
    /// Provides methods to interact with JSON data related to employees.
    /// </summary>
    public class JsonDataProvider : IJsonDataProvider
    {
        private readonly string _jsonDataPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDataProvider"/> class.
        /// </summary>
        /// <param name="config">Configuration options for JSON data path.</param>
        /// <param name="env">Provides information about the web hosting environment.</param>
        public JsonDataProvider(IOptions<JsonDataConfig> config, IWebHostEnvironment env)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (env == null)
                throw new ArgumentNullException(nameof(env));

            // Combine the content root path and JSON data path from configuration
            _jsonDataPath = Path.Combine(env.ContentRootPath, config.Value.JsonDataPath);
        }

        /// <summary>
        /// Retrieves a list of employees from the JSON data file.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of employees.</returns>
        public async Task<List<Employee>> GetEmployees()
        {
            try
            {
                if (File.Exists(_jsonDataPath))
                {
                    // Read JSON data from the file
                    var json = await File.ReadAllTextAsync(_jsonDataPath);
                    var serializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new JsonStringEnumConverter() }
                    };
                    return JsonSerializer.Deserialize<List<Employee>>(json, serializerOptions);
                }
                else
                {
                    // Handle file does not exist scenario
                    Console.WriteLine("Employees data file does not exist.");
                    throw new FileNotFoundException("Employees data file does not exist.", _jsonDataPath);
                }
            }
            catch (IOException ex)
            {
                // Handle IO exception when reading JSON file
                Console.WriteLine($"IO Exception reading JSON data from {_jsonDataPath}: {ex.Message}");
                throw; // Re-throw the exception to propagate it up the call stack
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Error reading JSON data from {_jsonDataPath}: {ex.Message}");
                throw; // Re-throw the exception to propagate it up the call stack
            }
        }

        /// <summary>
        /// Saves the list of employees to the JSON data file.
        /// </summary>
        /// <param name="employees">The list of employees to save.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SaveEmployees(List<Employee> employees)
        {
            try
            {
                // Backup the existing JSON data file
                if (File.Exists(_jsonDataPath))
                {
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string backupFileName = Path.GetFileNameWithoutExtension(_jsonDataPath) + $"_{timestamp}.json";
                    string backupFilePath = Path.Combine(Path.GetDirectoryName(_jsonDataPath), backupFileName);
                    File.Move(_jsonDataPath, backupFilePath);
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                };
                var json = JsonSerializer.Serialize(employees, options);
                // Write JSON data to file asynchronously
                await File.WriteAllTextAsync(_jsonDataPath, json);
            }
            catch (IOException ex)
            {
                // Handle IO exception when saving JSON file
                Console.WriteLine($"IO Exception saving employees to {_jsonDataPath}: {ex.Message}");
                throw; // Re-throw the exception to propagate it up the call stack
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Error saving employees to {_jsonDataPath}: {ex.Message}");
                throw; // Re-throw the exception to propagate it up the call stack
            }
        }

        /// <summary>
        /// Retrieves a list of dependents for all employees.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of dependents.</returns>
        public async Task<List<Dependent>> GetDependents()
        {
            var employees = await GetEmployees();
            return employees.SelectMany(e => e.Dependents).ToList();
        }
    }
}
