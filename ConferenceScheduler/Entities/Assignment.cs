using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceScheduler.Entities
{
    /// <summary>
    /// An instance of a session assigned to a room and a timeslot
    /// </summary>
    public class Assignment
    {
        /// <summary>
        /// Create an instance of a room-timeslot node with a null session
        /// </summary>
        public Assignment(int roomId, int timeslotId)
        {
            this.TimeslotId = timeslotId;
            this.RoomId = roomId;
        }

        /// <summary>
        /// Create an instance of a session-room-timeslot assignment
        /// </summary>
        public Assignment(int roomId, int timeslotId, int sessionId)
        {
            this.TimeslotId = timeslotId;
            this.RoomId = roomId;
            this.SessionId = sessionId;
        }

        /// <summary>
        /// The Id of the session in this assignment
        /// </summary>
        public int? SessionId { get; set; }

        /// <summary>
        /// The Id of the room for this assignment
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// The Id of the timeslot of this assignment
        /// </summary>
        public int TimeslotId { get; set; }

    }
}
