using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using VposApi.Models;
using VposApi.Services;

[ApiController]
[Route("api/[controller]")]
public class SecureController : ControllerBase
{
    private readonly ISecureService _secureService;

    public SecureController(ISecureService SecureService)
    {
        _secureService = SecureService;
    }

    [HttpPost]
    public async Task<IActionResult> Process(
        [FromBody] SecureRequest request,
        [FromServices] IValidator<SecureRequest> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }

        var result = await _secureService.ProcessAsync(request);
        return Ok(result);
    }

}