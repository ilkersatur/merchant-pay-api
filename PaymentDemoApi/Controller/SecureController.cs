using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using VposApi.Config;
using VposApi.Models;
using VposApi.Services;

[ApiController]
[Route("api/[controller]")]
public class SecureController : ControllerBase
{
    private readonly ISecureService _secureService;
    private readonly VposSettings _settings;

    public SecureController(ISecureService SecureService, IOptions<VposSettings> options)
    {
        _secureService = SecureService;
        _settings = options.Value;
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

    [HttpPost("SuccessResult")]
    public async Task<IActionResult> SuccessResult([FromForm] SecureSuccessModel secureModel)
    {
        var queryParams = $"?data={secureModel.ResultThreeDSResponse}";
        return Redirect($"{_settings.OkPage}{queryParams}");
    }

    [HttpPost("TDPaySuccessResult")]
    public async Task<IActionResult> TDPaySuccessResult([FromForm] SecureSuccessModel secureModel)
    {
        var queryParams = $"?data={secureModel.ResultThreeDSResponse}";
        return Redirect($"{_settings.TDPayOkPage}{queryParams}");
    }

    [HttpPost("Auth3DS")]
    public async Task<IActionResult> Auth3DS(
        [FromBody] Auth3DSModel auth3DSModel,
        [FromServices] IValidator<Auth3DSModel> validator)
    {
        var validationResult = await validator.ValidateAsync(auth3DSModel);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }

        var result = await _secureService.Auth3DS(auth3DSModel);
        return Ok(result);
    }

}