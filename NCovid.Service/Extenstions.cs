using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace NCovid.Service
{
    public static class Extenstions
    {
        /// <summary>
        /// The ToInt
        /// </summary>
        /// <param name="input">The input<see cref="string"/></param>
        /// <returns>The <see cref="int"/></returns>
        public static int ToInt(this string input)
        {
            return input.IsInt() ? int.Parse(input, NumberStyles.Any) : 0;
        }
        /// <summary>
        /// Check string IsInt
        /// </summary>
        /// <param name="input">The input<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public static bool IsInt(this string input)
        {
            return input.IsSet() && int.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        }
        public static bool IsSet(this string input)
        {
            return !(string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input));
        }
        /// <summary>
        /// Check string  IsDecimal
        /// </summary>
        /// <param name="input">The input<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public static bool IsDecimal(this string input)
        {
            return input.IsSet() && decimal.TryParse(input, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out _);
        }
        /// <summary>
        /// Converts string to  ToDecimal
        /// </summary>
        /// <param name="input">The input<see cref="string"/></param>
        /// <returns>The <see cref="decimal"/></returns>
        public static decimal ToDecimal(this string input)
        {
            return input.IsDecimal() ? decimal.Parse(input) : 0;
        }
        public static decimal ToDecimal(this string input, string format = "{0:F2}")
        {
            return !input.IsDecimal() ? 0m : Convert.ToDecimal(string.Format(format, decimal.Parse(input)));
        }
    }
}
