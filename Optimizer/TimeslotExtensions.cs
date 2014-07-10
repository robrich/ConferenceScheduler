using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceScheduler.Entities;

namespace ConferenceScheduler.Optimizer
{
    internal static class TimeslotExtensions
    {
        internal static int GetFirstTimeslotId(this IEnumerable<Timeslot> timeslots)
        {
            return timeslots.OrderBy(x => x).First().Id;
        }
    }
}
