namespace VposApi.Helpers
{
    public static class InternalResponseMapping
    {
        public static readonly Dictionary<string, string> InternalResponseMappingList = new Dictionary<string, string>
    {
        {"000000", "SUCCESSFUL"},
        {"000001", "REFER_TO_CARD_ISSUER"},
        {"000002", "REFER_TO_CARD_ISSUER_SPECIAL_CONDITION"},
        {"000003", "INVALID_MERCHANT"},
        {"000004", "PICKUP_CARD"},
        {"000005", "DO_NOT_HONOUR"},
        {"000006", "ERROR_IN_FILE_UPDATE"},
        {"000007", "PICKUP_CARD_SPECIAL_CONDITION"},
        {"000008", "HONOUR_WITH_ID"},
        {"000009", "TRY_AGAIN"},
        {"000011", "APPROVED_VIP"},
        {"000012", "INVALID_TRANSACTION"},
        {"000013", "INVALID_AMOUNT"},
        {"000014", "INVALID_ACCOUNT_NUMBER"},
        {"000015", "NO_SUCH_ISSUER"},
        {"000025", "UNABLE_TO_LOCATE_RECORD_ON_FILE"},
        {"000028", "ORIGINAL_IS_DENIED"},
        {"000029", "ORIGINAL_NOT_FOUND"},
        {"000030", "FORMAT_ERROR"},
        {"000033", "EXPIRED_CARD_PICK_UP"},
        {"000034", "COUNTERFEIT_CARD_PICK_UP"},
        {"000036", "RESTRICTED_CARD_PICK_UP"},
        {"000038", "PIN_TRIES_EXCEEDED_PICK_UP"},
        {"000039", "UNSUPPORTED_MESSAGE_SECURITY_CODE"},
        {"000041", "LOST_CARD"},
        {"000043", "STOLEN_CARD"},
        {"000046", "CLOSED_ACCOUNT"},
        {"000051", "INSUFFICIENT_FUNDS"},
        {"000052", "NO_CHECKING_ACCOUNT"},
        {"000053", "NO_SAVINGS_ACCOUNT"},
        {"000054", "EXPIRED_CARD"},
        {"000055", "INCORRECT_PIN"},
        {"000057", "TRANSACTION_NOT_PERMITTED_TO_CARDHOLDER"},
        {"000058", "TRANSACTION_NOT_PERMITTED_TO_TERMINAL"},
        {"000059", "SUSPECTED_FRAUD"},
        {"000061", "EXCEEDS_WITHDRAWAL_AMOUNT_LIMIT"},
        {"000062", "RESTRICTED_CARD"},
        {"000063", "SECURITY_VIOLATION"},
        {"000065", "EXCEEDS_WITHDRAWAL_FREQUENCY_LIMIT"},
        {"000070", "CONTACT_CARD_ISSUER"},
        {"000071", "PIN_NOT_CHANGED"},
        {"000075", "ALLOWABLE_NUMBER_OF_PIN_TRIES_EXCEEDED"},
        {"000076", "KEY_SYNCHRONISATION_ERROR"},
        {"000077", "NO_SCRIPT_AVAILABLE"},
        {"000078", "UNSAFE_PIN"},
        {"000079", "ARQC_FAILED"},
        {"000080", "BATCH_NUMBER_ERROR"},
        {"000085", "APPROVAL_OF_REQUEST"},
        {"000086", "CANNOT_VERIFY_PIN"},
        {"000087", "KEY_SYNCHRONISATION_ERROR_SWITCH"},
        {"000088", "CRYPTOGRAPHIC_FAILURE"},
        {"000091", "SWITCH_IS_INOPERATIVE"},
        {"000092", "FINANCIAL_INSTITUTION_UNKNOWN_FOR_ROUTING"},
        {"000093", "VIOLATION_OF_LAW"},
        {"000094", "DUPLICATE_TXN"},
        {"000095", "RECONCILE_ERROR"},
        {"000096", "SYSTEM_MALFUNCTION"},
        {"000098", "DUPLICATE_REVERSAL"},
        {"000099", "QR_DENIED"}
    };

        public static string GetInternalResponseMappingList(string code)
        {
            return InternalResponseMappingList.TryGetValue(code, out var message) ? message : "Bilinmeyen Hata";
        }
    }

}
