namespace PaymentDemoApi.Helpers
{
    public static class CardBrandHelper
    {
        public static string GetCardBrand(string pan)
        {
            if (string.IsNullOrWhiteSpace(pan) || pan.Length < 6)
                return "UNKNOWN";

            if (pan.StartsWith("4"))
                return "M";
            if (pan.StartsWith("5"))
                return "M";
            if (pan.StartsWith("34") || pan.StartsWith("37"))
                return "AMEX";
            if (pan.StartsWith("9792"))
                return "TROY";

            return "UNKNOWN";
        }
    }

}
