namespace SimpleRulesEngine.Models
{
    public enum TextComparison
    {
        NotSet = 0,
        Equal = 1,
        NotEqual = 2,
        StartsWith = 3,
        EndsWith = 4,
        Contains = 5,
    }

    public static class TextComparisonExtensions
    {
        public static bool HasValue(this TextComparison textComparison)
        {
            return textComparison != TextComparison.NotSet;
        }
    }
}
