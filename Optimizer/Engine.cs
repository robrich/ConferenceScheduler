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
        /// Returns an optimized conference schedule based on the inputs.
        /// </summary>
        /// <param name="sessions">A list of sessions and their associated attributes.</param>
        /// <param name="rooms">A list of rooms that sessions can be held in along with their associated attributes.</param>
        /// <param name="timeslots">A list of time slots during which sessions can be delivered.</param>
        /// <param name="settings">A dictionary of configuration settings for the process.</param>
        /// <returns>A collection of assignments representing the room and Timeslot in which each session will be delivered.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "session"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "room"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "timeslot"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "settings")]
        public IEnumerable<Assignment> Process(IEnumerable<Session> sessions, IEnumerable<Room> rooms, IEnumerable<Timeslot> timeslots, IDictionary<string, string> settings)
        {
            var result = new List<Assignment>();
            if (sessions != null && rooms != null && timeslots != null)
            {
                // Make sure there are enough slots/rooms for all of the sessions
                // TODO: Subtract out any times that specific rooms are not available
                if (sessions.Count() > (rooms.Count() * timeslots.Count()))
                    throw new Exceptions.NoFeasibleSolutionsException();

                if (sessions.Count(s => s.Presenters == null || s.Presenters.Count() < 1) > 0)
                    throw new ArgumentException("Every session must have at least one presenter.");

                // Create the presenter availability matrix
                var presenters = sessions.SelectMany(s => s.Presenters).Distinct();
                var timeslotIds = timeslots.Select(ts => ts.Id);
                var presenterMatrix = new PresenterAvailablityCollection(presenters, timeslotIds);
                if (!presenterMatrix.IsFeasible)
                    throw new Exceptions.NoFeasibleSolutionsException();

                // Create the session availability matrix
                var sessionMatrix = new SessionAvailabilityCollection(sessions, rooms, timeslots);
                if (!sessionMatrix.IsFeasible)
                    throw new Exceptions.NoFeasibleSolutionsException();

                // Setup the empty assignment matrix
                foreach (var room in rooms)
                    foreach (var timeslot in timeslots)
                        result.Add(new Assignment(room.Id, timeslot.Id));

                var session = GetUnassignedSessionWithFewestAvailableSlots(result, sessions, presenterMatrix);
                while (session != null)
                {
                    var availableTimeslots = presenterMatrix.GetAvailableTimeslotIds(session.Presenters);
                    var unassignedMatrix = result.Where(a => a.SessionId == null && availableTimeslots.Contains(a.TimeslotId));
                    var target = unassignedMatrix.First();
                    target.SessionId = session.Id;
                    session = GetUnassignedSessionWithFewestAvailableSlots(result, sessions, presenterMatrix);
                }


                //TODO: Add value 
            }
            return result;
        }

        private static Session GetUnassignedSessionWithFewestAvailableSlots(IEnumerable<Assignment> assignments, IEnumerable<Session> sessions, PresenterAvailablityCollection presenterMatrix)
        {
            Session result = null;
            var assignedSessionIds = assignments.Where(a => a.SessionId != null).Select(a => a.SessionId);
            var sessionDictionary = new Dictionary<int, int>();
            foreach (var session in sessions.Where(s => !assignedSessionIds.Contains(s.Id)))
            {
                sessionDictionary.Add(session.Id, presenterMatrix.GetAvailableTimeslotIds(session.Presenters).Count());
            }

            if (sessionDictionary.Count() > 0)
            {
                var min = sessionDictionary.Min(s => s.Value);
                var key = sessionDictionary.FirstOrDefault(sd => sd.Value == min).Key;
                result = sessions.Where(s => s.Id == key).Single();
            }

            return result;
        }

    }
}
