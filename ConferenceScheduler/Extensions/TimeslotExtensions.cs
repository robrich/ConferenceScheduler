using ConferenceScheduler.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceScheduler.Extensions
{

    // TODO: Document
    /// <summary>
    /// 
    /// </summary>
    public static class TimeslotExtensions
    {
        // TODO: Document
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeslots"></param>
        public static void Validate(this IEnumerable<Timeslot> timeslots)
        {
            if (timeslots == null)
                throw new ArgumentNullException(nameof(timeslots));

            if (timeslots.Count() == 0)
                throw new ArgumentException("You must have at least one timeslot");

            if (timeslots.Select(t => t.Id).Distinct().Count() != timeslots.Count())
                throw new ArgumentException("Timeslot Ids must be unique");

        }
    }
}
