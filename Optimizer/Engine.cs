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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        IDictionary<string, string> _settings;

        /// <summary>
        /// Create an instance of the object
        /// </summary>
        public Engine() { }

        /// <summary>
        /// Create an instance of the object using the supplied settings
        /// </summary>
        /// <param name="settings">A collection of configuration settings</param>
        public Engine(IDictionary<string, string> settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Returns an optimized conference schedule based on the inputs.
        /// </summary>
        /// <param name="sessions">A list of sessions and their associated attributes.</param>
        /// <param name="rooms">A list of rooms that sessions can be held in along with their associated attributes.</param>
        /// <param name="timeslots">A list of time slots during which sessions can be delivered.</param>
        /// <returns>A collection of assignments representing the room and Timeslot in which each session will be delivered.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public IEnumerable<Assignment> Process(IEnumerable<Session> sessions, IEnumerable<Room> rooms, IEnumerable<Timeslot> timeslots)
        {
            var result = new AssignmentCollection();
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
                        if (room.AvailableInTimeslot(timeslot.Id))
                            result.Add(new Assignment(room.Id, timeslot.Id));

                while (result.AssignmentsCompleted < sessions.Count())
                {
                    // If any room-timeslot combinations only have 1 session available, assign them
                    var itemsWithOneOption = sessionMatrix.GetUnassignedItemsWithOnlyOneOption();
                    while (itemsWithOneOption.Count() > 0)
                    {
                        var item = itemsWithOneOption.First();
                        var assignment = result.GetAssignment(item.RoomId, item.TimeslotId);
                        sessionMatrix.Assign(assignment, item.SessionIds.Single());
                        presenterMatrix.RemovePresentersFromSlots(assignment, sessions.Where(s => s.Id == assignment.SessionId).Single());
                        itemsWithOneOption = sessionMatrix.GetUnassignedItemsWithOnlyOneOption();
                    }

                    var session = result.GetUnassignedSessionWithFewestAvailableSlots(sessions, presenterMatrix);
                    if (session != null)
                    {
                        var availableTimeslots = presenterMatrix.GetAvailableTimeslotIds(session.Presenters);
                        var unassignedMatrix = result.Where(a => a.SessionId == null && availableTimeslots.Contains(a.TimeslotId));
                        var assignment = unassignedMatrix.First();
                        sessionMatrix.Assign(assignment, session.Id);
                        session = result.GetUnassignedSessionWithFewestAvailableSlots(sessions, presenterMatrix);
                    }
                }

                //TODO: Add value 
            }

            return result.Where(a => a.SessionId.HasValue);
        }

    }
}
