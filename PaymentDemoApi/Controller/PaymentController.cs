using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using VposApi.Models;
using VposApi.Services;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<IActionResult> Process(
        [FromBody] PaymentRequest request,
        [FromServices] IValidator<PaymentRequest> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }

        var result = await _paymentService.ProcessAsync(request);
        return Ok(result);
    }

}