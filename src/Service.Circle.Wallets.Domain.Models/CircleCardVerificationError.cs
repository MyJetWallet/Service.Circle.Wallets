namespace Service.Circle.Wallets.Domain.Models
{
    public enum CircleCardVerificationError
    {
        // VerificationFailed,
        // VerificationFraudDetected,
        // VerificationDenied,
        // VerificationNotSupportedByIssuer,
        // VerificationStoppedByIssuer,
        CardFailed,

        // CardInvalid,
        CardAddressMismatch,
        CardZipMismatch,
        CardCvvInvalid,

        CardExpired
        // CardLimitViolated,
        // CardNotHonored,
        // CardCvvRequired,
        // CreditCardNotAllowed,
        // CardAccountIneligible,
        // CardNetworkUnsupported
    }
}