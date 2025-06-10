using Projekt.Models;

namespace Projekt.Helpers
{
    public static class ColorHelper
    {
        public static Colors GetColorByValue(int value)
        {
            return GetColors().FirstOrDefault(color => (int)color == value);
        }

        public static int GetColorValue(Colors color)
        {
            return (int)color;
        }

        public static string GetColorName(Colors color)
        {
            string colorName = color.ToString();

            return colorName switch
            {
                "Black" => "Sort",
                "White" => "Hvid",
                "Gray" => "Grå",
                "Red" => "Rød",
                "Blue" => "Blå",
                "Green" => "Grøn",
                "Yellow" => "Gul",
                "Purple" => "Lilla",
                "Pink" => "Lyserød",
                "Orange" => "Orange",
                "Brown" => "Brun",
                _ => colorName
            };
        }

        public static string GetColorClass(Colors color)
        {
            return color.ToString().ToLowerInvariant();
        }

        public static List<Colors> GetColors()
        {
            return Enum.GetValues(typeof(Colors)).Cast<Colors>().OrderBy(GetColorName).ToList();
        }

        public static Colors GetFirst(List<Colors> colors)
        {
            return GetColors().Where(colors.Contains).FirstOrDefault();
        }
    }
}
