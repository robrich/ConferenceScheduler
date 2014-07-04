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
    }
}
