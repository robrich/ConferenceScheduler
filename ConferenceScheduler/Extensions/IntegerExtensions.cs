using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceScheduler.Extensions
{
    /// <summary>
    /// Contains extension methods to the Integer data type
    /// and data types that collect integers
    /// </summary>
    public static class IntegerExtensions
    {
        /// <summary>
        /// Locates the item in an array that holds the specified value
        /// </summary>
        /// <param name="values">The array of integers being inspected</param>
        /// <param name="valueToLocate">The value to locate in the specified array</param>
        /// <returns>A nullable integer containing the index of the located item or null if the value is not found in the array</returns>
        public static int? IndexOfValue(this int[] values, int valueToLocate)
        {
            int? result = null;

            if (values != null)
            {
                int i = 0;
                while (!result.HasValue && i < values.Length)
                {
                    if (values[i] == valueToLocate)
                        result = i;
                    i++;
                }
            }

            return result;
        }
    }
}
