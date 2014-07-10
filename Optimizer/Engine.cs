using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConferenceScheduler.Entities;

namespace ConferenceScheduler.Optimizer
{
    /// <summary>
    /// Holds methods used to perform optimizations to determine conference schedules.
    /// </summary>
    public class Engine
    {
        /// <summary>
        /// Create an instance of the object
        /// </summary>
        public Engine() { }

        /// <summary>
        /// Returns an optimized conference schedule based on the inputs.
        /// </summary>
        /// <param name="sessions">A list of sessions and their associated attributes.</param>
        /// <param name="rooms">A list of rooms that sessions can be held in along with their associated attributes.</param>
        /// <param name="timeslots">A list of time slots during which sessions can be delivered.</param>
        /// <returns>A collection of assignments representing the room and timeslot in which each session will be delivered.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public IEnumerable<Assignment> Process(IEnumerable<Session> sessions, IEnumerable<Room> rooms, IEnumerable<Timeslot> timeslots)
        {
            var solution = new Solution(sessions, rooms, timeslots);

            while (solution.AssignmentsCompleted < sessions.Count())
            {
                solution.AssignSessionsWithOnlyOneOption();
                solution.AssignMostConstrainedSession();
            }

            if (!solution.IsFeasible)
                throw new Exceptions.NoFeasibleSolutionsException();
            else
                return solution.Results;
        }

    }
}
