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
            return (!this.UnavailableForTimeslots.Contains(timeslotId));
        }

        /// <summary>
        /// Creates an instance of the object
        /// </summary>
        /// <param name="id">The unique identifier of the room</param>
        /// <returns>An object representing a room in which sessions may be held</returns>
        public static Room Create(int id)
        {
            return Create(id, 0);
        }

        /// <summary>
        /// Creates an instance of the object
        /// </summary>
        /// <param name="id">The unique identifier of the room</param>
        /// <param name="capacity">The number of people that can be reasonably accommodated 
        /// as attendees of the session in the room.</param>
        /// <returns>An object representing a room in which sessions may be held</returns>
        public static Room Create(int id, int capacity)
        {
            return Create(id, capacity, new int[] { });
        }

        /// <summary>
        /// Creates an instance of the object
        /// </summary>
        /// <param name="id">The unique identifier of the room</param>
        /// <param name="capacity">The number of people that can be reasonably accommodated 
        /// as attendees of the session in the room.</param>
        /// <param name="roomUnavailableForTimeslots">The list of timeslot Ids during which the room is not available</param>
        /// <returns>An object representing a room in which sessions may be held</returns>
        public static Room Create(int id, int capacity, params int[] roomUnavailableForTimeslots)
        {
            return new Room()
            {
                Id = id,
                Capacity = capacity,
                UnavailableForTimeslots = roomUnavailableForTimeslots.ToList()
            };
        }

    }
}
