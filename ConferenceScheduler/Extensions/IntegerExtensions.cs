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
        // TODO: Document
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "target")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "source")]
        public static bool Matches(this IEnumerable<int> source, IEnumerable<int> target)
        {
            bool result = source.Count().Equals(target.Count());
            if (result)
            {
                var sourceArray = source.ToArray();
                var targetArray = target.ToArray();
                for (int i = 0; i < sourceArray.Length; i++)
                {
                    if (sourceArray[i] != targetArray[i])
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }


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
