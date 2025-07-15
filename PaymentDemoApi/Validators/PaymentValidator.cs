using FluentValidation;
using VposApi.Models;

public class PaymentValidator : AbstractValidator<PaymentRequest>
{
    public PaymentValidator()
    {
        // Genel zorunlu alanlar
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId alanı zorunludur.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password alanı zorunludur.");
        RuleFor(x => x.MerchantNumber).NotEmpty().WithMessage("MerchantNumber alanı zorunludur.");
        RuleFor(x => x.ShopCode).NotEmpty().WithMessage("ShopCode alanı zorunludur.");
        RuleFor(x => x.TransactionType).NotEmpty().WithMessage("TransactionType alanı zorunludur.");

        // İşlem tipine göre kurallar
        When(x => x.TransactionType != null, () =>
        {
            RuleFor(x => x.CardHolderName)
                .NotEmpty()
                .When(x => x.TransactionType == "SALEPOS" || x.TransactionType == "MAILORDER" || x.TransactionType == "PREAUTH")
                .WithMessage("CardHolderName alanı zorunludur.");

            RuleFor(x => Convert.ToInt32(x.Amount))
                .GreaterThan(0)
                .When(x => x.TransactionType == "SALEPOS" || x.TransactionType == "MAILORDER" || x.TransactionType == "PREAUTH" || x.TransactionType == "REFUND" || x.TransactionType == "POSTAUTH" || x.TransactionType == "MOTOINSURANCE")
                .WithMessage("Amount sıfırdan büyük olmalıdır.");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .When(x => x.TransactionType == "SALEPOS" || x.TransactionType == "MAILORDER" || x.TransactionType == "PREAUTH" || x.TransactionType == "MOTOINSURANCE")
                .WithMessage("Currency alanı zorunludur.");

            RuleFor(x => x.Pan)
                .NotEmpty()
                .When(x => x.TransactionType == "SALEPOS" || x.TransactionType == "MAILORDER" || x.TransactionType == "PREAUTH")
                .WithMessage("Pan alanı zorunludur.");

            RuleFor(x => x.Cvv2)
                .NotEmpty()
                .When(x => x.TransactionType == "SALEPOS" || x.TransactionType == "MAILORDER" || x.TransactionType == "PREAUTH")
                .WithMessage("Cvv2 alanı zorunludur.");

            RuleFor(x => x.ExpireDate)
                .NotEmpty()
                .When(x => x.TransactionType == "SALEPOS" || x.TransactionType == "MAILORDER" || x.TransactionType == "PREAUTH")
                .WithMessage("ExpireDate alanı zorunludur.");

            RuleFor(x => x.TransactionId)
                .NotEmpty()
                .When(x => x.TransactionType == "VOID" || x.TransactionType == "REFUND" || x.TransactionType == "POSTAUTH")
                .WithMessage("TransactionId alanı zorunludur.");

            RuleFor(x => x.Bin)
                .NotEmpty()
                .When(x => x.TransactionType == "MOTOINSURANCE")
                .WithMessage("Bin alanı zorunludur.");

            RuleFor(x => x.LastFourDigits)
                .NotEmpty()
                .When(x => x.TransactionType == "MOTOINSURANCE")
                .WithMessage("LastFourDigits alanı zorunludur.");

            RuleFor(x => x.TcknVkn)
                .NotEmpty()
                .When(x => x.TransactionType == "MOTOINSURANCE")
                .WithMessage("TcknVkn alanı zorunludur.");
        });
    }
}
