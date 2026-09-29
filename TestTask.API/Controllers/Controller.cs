using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace TestTask.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FunctionsController : ControllerBase
{
    private readonly TestService service;
    private readonly IValidator<RequestModel> validator;

    public FunctionsController(TestService service, IValidator<RequestModel> validator)
    {
        this.service = service;
        this.validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> Process([FromBody] RequestModel request)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errorResponse = new ResponseModel
            {
                Is_error = 1,
                Error_code = "VALIDATION_ERROR",
                Error_message = string.Join("; ", 
                                validationResult.Errors.Select(e => e.ErrorMessage))
            };
            return BadRequest(errorResponse);
        }

        var response = await service.ProcessAsync(request);
        return Ok(response);
    }
}