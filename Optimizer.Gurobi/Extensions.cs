using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceScheduler.Optimizer.Gurobi
{
    public static class Extensions
    {
        public static int? IndexOfValue(this int[] values, int valueToLocate)
        {
            int? result = null;

            int i = 0;
            while (!result.HasValue && i < values.Length)
            {
                if (values[i] == valueToLocate)
                    result = i;
                i++;
            }

            return result;
        }
    }
}
