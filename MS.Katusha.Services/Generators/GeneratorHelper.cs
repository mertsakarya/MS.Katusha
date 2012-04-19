using System;
using System.Text;

namespace MS.Katusha.Services.Generators
{
    public static class GeneratorHelper
    {
        public static Random RND = new Random((int) DateTimeOffset.UtcNow.Ticks & 0x0000FFFF);

        public static int RandomNumber(int min, int max)
        {
            return RND.Next(min, max);
        }

        /// <summary>
        /// Generates a random string with the given length
        /// </summary>
        /// <param name="size">Size of the string</param>
        /// <param name="lowerCase">If true, generate lowercase string</param>
        /// <returns>Random string</returns>
        public static string RandomString(int size, bool lowerCase)
        {
            var builder = new StringBuilder();
            //Random random = new Random();
            for (int i = 0; i < size; i++) {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * RND.NextDouble() + 66)));
                if (ch == 91) ch = ' ';
                builder.Append(ch);
            }
            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
    }
}
