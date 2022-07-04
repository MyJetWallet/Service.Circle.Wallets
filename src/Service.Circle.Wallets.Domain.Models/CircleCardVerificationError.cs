namespace Service.Circle.Wallets.Domain.Models
{
    public enum CircleCardVerificationError
    {
        CardFailed,
        CardAddressMismatch,
        CardZipMismatch,
        CardCvvInvalid,
        CardExpired,
        CardInvalid,
        VerificationFailed,
        VerificationNotSupportedByIssuer,
        CardLimitViolated,
        CardNotHonored,
        CardCvvRequired,
        CardAccountIneligible,
        ThreeDSecureNotSupported,
        ThreeDSecureActionExpired,
        ThreeDSecureInvalidRequest
        // VerificationFraudDetected,
        // VerificationDenied,
        // VerificationStoppedByIssuer,
        // CreditCardNotAllowed,
        // CardNetworkUnsupported
    }
}