using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Api.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DependentsController : ControllerBase
{
    private readonly IDependentService _dependentService;

    public DependentsController(IDependentService dependentService)
    {
        _dependentService = dependentService ?? throw new ArgumentNullException(nameof(dependentService));
    }

    /// <summary>
    /// Retrieves a dependent by ID.
    /// </summary>
    /// <param name="id">The ID of the dependent.</param>
    /// <returns>An ActionResult containing the ApiResponse with dependent details.</returns>
    [SwaggerOperation(Summary = "Get dependent by id")]
    [HttpGet("{id}")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the employee details", typeof(ApiResponse<GetDependentDto>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Dependent not found")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "An unexpected error occurred")]
    public async Task<ActionResult<ApiResponse<GetDependentDto>>> Get(int id)
    {
        try
        {
            var dependent = await _dependentService.GetDependent(id);
            if (dependent == null)
            {
                return NotFound();
            }

            return Ok(new ApiResponse<GetDependentDto>
            {
                Data = dependent,
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
    /// Retrieves all dependents.
    /// </summary>
    /// <returns>An ActionResult containing the ApiResponse with a list of dependents.</returns>
    [SwaggerOperation(Summary = "Get all dependents")]
    [HttpGet("")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns list of dependent details", typeof(ApiResponse<List<GetDependentDto>>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Dependents not found")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "An unexpected error occurred")]
    public async Task<ActionResult<ApiResponse<List<GetDependentDto>>>> GetAll()
    {
        try
        {
            var dependents = await _dependentService.GetAllDependents();
            if (dependents == null || dependents.Count == 0)
            {
                return NotFound();
            }

            return Ok(new ApiResponse<List<GetDependentDto>>
            {
                Data = dependents,
                Success = true
            });
        }
        catch (Exception ex)
        {
            // Log unexpected exceptions

            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
        }
    }
}
