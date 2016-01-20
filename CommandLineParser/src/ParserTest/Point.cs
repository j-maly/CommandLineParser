using System;
using CommandLineParser.Exceptions;

namespace ParserTest
{
    /// <summary>
    /// example class used to demonstrate attributes with 
    /// custom values
    /// </summary>
    public class Point
    {
        public int x;
        public int y;

        public override string ToString()
        {
            return String.Format("Point[{0};{1}]", x, y);
        }

        public static Point Parse(string stringValue, System.Globalization.CultureInfo cultureInfo)
        {
            if (stringValue.StartsWith("[") && stringValue.EndsWith("]"))
            {
                string[] parts =
                    stringValue.Substring(1, stringValue.Length - 2).Split(';', ',');
                Point p = new Point();
                p.x = int.Parse(parts[0], cultureInfo);
                p.y = int.Parse(parts[1], cultureInfo);
                return p;
            }
            
            throw new CommandLineArgumentException("Bad point format", "point");
        }
    }
}