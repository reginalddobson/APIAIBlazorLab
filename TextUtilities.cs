namespace ApiAiBlazorLab
{
    public static class TextUtilities
    {
        public static string NormalizeFact(string? fact)
        {
            if (string.IsNullOrWhiteSpace(fact))
                return "No fact available.";

            fact = fact.Trim();

            return fact.EndsWith(".") ? fact : fact + ".";
        }
    }
}
