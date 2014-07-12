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
        internal static int IndexOf(this IOrderedEnumerable<Timeslot> timeslots, Timeslot timeslot)
        {
            return Array.IndexOf(timeslots.ToArray(), timeslot);
        }

        internal static int IndexOf(this IOrderedEnumerable<Timeslot> timeslots, int timeslotId)
        {
            var timeslot = timeslots.Where(t => t.Id == timeslotId).Single();
            return timeslots.IndexOf(timeslot);
        }

        internal static IOrderedEnumerable<Timeslot> Sort(this IEnumerable<Timeslot> timeslots)
        {
            return timeslots.OrderBy(t => (t.DayIndex * 24.0) + t.StartHour);
        }

    }
}
