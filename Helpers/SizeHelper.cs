using Projekt.Models;

namespace Projekt.Helpers
{
    public static class SizeHelper
    {
        public static List<string> GetSizes()
        {
            return new List<string> { "XS", "S", "M", "L", "XL", "XXL" };
        }

        public static string GetFirst(List<string> sizes)
        {
            return GetSizes().Where(sizes.Contains).FirstOrDefault() ?? string.Empty;
        }
    }
}
