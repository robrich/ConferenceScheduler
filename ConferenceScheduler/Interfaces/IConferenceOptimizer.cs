using System.Collections.Generic;
using ConferenceScheduler.Entities;

namespace ConferenceScheduler.Interfaces
{
    /// <summary>
    /// Holds methods used to perform optimizations to determine conference schedules.
    /// </summary>
    public interface IConferenceOptimizer
    {
        /// <summary>
        /// Returns an optimized conference schedule based on the inputs.
        /// </summary>
        /// <param name="sessions">A list of sessions and their associated attributes.</param>
        /// <param name="rooms">A list of rooms that sessions can be held in along with their associated attributes.</param>
        /// <param name="timeslots">A list of time slots during which sessions can be delivered.</param>
        /// <returns>A collection of assignments representing the room and timeslot in which each session will be delivered.</returns>
        IEnumerable<Assignment> Process(IEnumerable<Session> sessions, IEnumerable<Room> rooms, IEnumerable<Timeslot> timeslots);
    }
}