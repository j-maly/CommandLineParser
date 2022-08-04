using CommandLineParser.Exceptions;

namespace ParserTest
{
    /// <summary>
    /// example class used to demonstrate attributes with custom values
    /// </summary>
    public class Point
    {
        public int X;
        public int Y;

        public override string ToString()
        {
            return $"Point[{X};{Y}]";
        }

        public static Point Parse(string stringValue, System.Globalization.CultureInfo cultureInfo)
        {
            if (stringValue.StartsWith("[") && stringValue.EndsWith("]"))
            {
                string[] parts = stringValue.Substring(1, stringValue.Length - 2).Split(';', ',');
                var p = new Point
                {
                    X = int.Parse(parts[0], cultureInfo),
                    Y = int.Parse(parts[1], cultureInfo)
                };
                return p;
            }

            throw new CommandLineArgumentException("Bad point format", "point");
        }
    }
}