using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceScheduler.Entities
{
    /// <summary>
    /// Represents a location for a session.
    /// </summary>
    public class Room
    {
        /// <summary>
        /// The primary-key identifier of the object
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The number of people that the room can comfortably and safely hold
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// The list of timeslot Ids during which the room is not available
        /// </summary>
        public IEnumerable<int> UnavailableForTimeslots { get; set; }

        /// <summary>
        /// Indicates if the room is available during the specified timeslot
        /// </summary>
        /// <param name="timeslotId">The unique identifier of the timeslot being tested</param>
        /// <returns>True if the room is available during the timeslot, false if not</returns>
        public bool AvailableInTimeslot(int timeslotId)
        {
            return (this.UnavailableForTimeslots == null || !this.UnavailableForTimeslots.Contains(timeslotId));
        }
    }
}
