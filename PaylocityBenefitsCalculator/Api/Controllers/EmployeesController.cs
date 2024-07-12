using Api.Dtos.Employee;
using Api.Models;
using Api.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
    }

    /// <summary>
    /// Retrieves an employee by ID.
    /// </summary>
    /// <param name="id">The ID of the employee.</param>
    /// <returns>An ActionResult containing the ApiResponse with employee details.</returns>
    [SwaggerOperation(Summary = "Get employee by id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Get(int id)
    {
        try
        {
            var employee = await _employeeService.GetEmployee(id);
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(new ApiResponse<GetEmployeeDto>
            {
                Data = employee,
                Success = true
            });
        }
        catch (Exception ex)
        {
            // Log unexpected exceptions

            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
        }
    }

    /// <summary>
    /// Retrieves all employees.
    /// </summary>
    /// <returns>An ActionResult containing the ApiResponse with a list of employees.</returns>
    [SwaggerOperation(Summary = "Get all employees")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> GetAll()
    {
        try
        {
            var employees = await _employeeService.GetAllEmployees();
            if (employees == null || employees.Count == 0)
            {
                return NotFound();
            }

            return Ok(new ApiResponse<List<GetEmployeeDto>>
            {
                Data = employees,
                Success = true
            });
        }
        catch (Exception ex)
        {
            // Log unexpected exceptions

            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
        }
    }

    /// <summary>
    /// Retrieves the paycheck details for an employee by ID.
    /// </summary>
    /// <param name="id">The ID of the employee.</param>
    /// <returns>An ActionResult containing the ApiResponse with paycheck details.</returns>
    [SwaggerOperation(Summary = "Get employee paycheck")]
    [HttpGet("{id}/paycheck")]
    public async Task<ActionResult<ApiResponse<PayCheck>>> GetPaycheck(int id)
    {
        try
        {
            var paycheck = await _employeeService.GetPaycheck(id);
            if (paycheck == null)
                return NotFound();

            return Ok(new ApiResponse<PayCheck>
            {
                Data = paycheck,
                Success = true
            });
        }
        catch (Exception ex)
        {
            // Log unexpected exceptions

            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
        }
    }

    /// <summary>
    /// Adds or updates an employee.
    /// </summary>
    /// <param name="employee">The employee object to add or update.</param>
    /// <returns>An ActionResult indicating success or failure.</returns>
    [SwaggerOperation(Summary = "Adds or updates an employee")]
    [HttpPost("AddOrUpdateEmployee")]
    public async Task<ActionResult> AddOrUpdateEmployee([FromBody] Employee employee)
    {
        if (employee == null)
        {
            return BadRequest("Employee data is null.");
        }

        try
        {
            var result = await _employeeService.AddOrUpdateEmployee(employee);
            if (result)
            {
                return Ok("Employee added or updated successfully.");
            }
            else {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding or updating the employee.");
            }
        }
        catch (InvalidOperationException ex)
        {
            // Log exception
            
            // Return a bad request response with the exception message
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            // Log unexpected exceptions
            
            // Return a 500 Internal Server Error response
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
