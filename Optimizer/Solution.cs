using ConferenceScheduler.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceScheduler.Optimizer
{
    internal class Solution
    {
        PresenterAvailablityCollection _presenterMatrix;
        SessionAvailabilityCollection _sessionMatrix;

        internal AssignmentCollection Assignments { get; set; }

        internal IEnumerable<Assignment> Results
        {
            get
            {
                return this.Assignments.Where(a => a.SessionId.HasValue);
            }
        }

        internal int AssignmentsCompleted { get { return this.Assignments.AssignmentsCompleted; } }

        internal Solution(IEnumerable<Session> sessions, IEnumerable<Room> rooms, IEnumerable<Timeslot> timeslots)
        {
            this.Assignments = new AssignmentCollection();
            Validate(sessions, rooms, timeslots);
            Load(sessions, rooms, timeslots);
        }

        internal void AssignSessionsWithOnlyOneOption(IEnumerable<Session> sessions)
        {
            // If any room-timeslot combinations only have 1 session available, assign them
            var itemsWithOneOption = _sessionMatrix.GetUnassignedItemsWithOnlyOneOption();
            while (itemsWithOneOption.Count() > 0)
            {
                var item = itemsWithOneOption.First();
                var assignment = this.Assignments.GetAssignment(item.RoomId, item.TimeslotId);
                _sessionMatrix.Assign(assignment, item.SessionIds.Single());
                _presenterMatrix.RemovePresentersFromSlots(assignment, sessions.Where(s => s.Id == assignment.SessionId).Single());
                itemsWithOneOption = _sessionMatrix.GetUnassignedItemsWithOnlyOneOption();
            }
        }

        internal void AssignMostConstrainedSession(IEnumerable<Session> sessions)
        {
            var session = this.Assignments.GetUnassignedSessionWithFewestAvailableSlots(sessions, _presenterMatrix);
            if (session != null)
            {
                var availableTimeslots = _presenterMatrix.GetAvailableTimeslotIds(session.Presenters);
                var unassignedMatrix = this.Assignments.Where(a => a.SessionId == null && availableTimeslots.Contains(a.TimeslotId));
                var assignment = unassignedMatrix.First();
                _sessionMatrix.Assign(assignment, session.Id);
                session = this.Assignments.GetUnassignedSessionWithFewestAvailableSlots(sessions, _presenterMatrix);
            }
        }

        private void Load(IEnumerable<Session> sessions, IEnumerable<Room> rooms, IEnumerable<Timeslot> timeslots)
        {
            // Create the presenter availability matrix
            var presenters = sessions.SelectMany(s => s.Presenters).Distinct();
            var timeslotIds = timeslots.Select(ts => ts.Id);
            _presenterMatrix = new PresenterAvailablityCollection(presenters, timeslotIds);
            if (!_presenterMatrix.IsFeasible)
                throw new Exceptions.NoFeasibleSolutionsException();

            // Create the session availability matrix
            _sessionMatrix = new SessionAvailabilityCollection(sessions, rooms, timeslots);
            if (!_sessionMatrix.IsFeasible)
                throw new Exceptions.NoFeasibleSolutionsException();

            // Setup the empty assignment matrix
            foreach (var room in rooms)
                foreach (var timeslot in timeslots)
                    if (room.AvailableInTimeslot(timeslot.Id))
                        this.Assignments.Add(new Assignment(room.Id, timeslot.Id));

        }

        private static void Validate(IEnumerable<Session> sessions, IEnumerable<Room> rooms, IEnumerable<Timeslot> timeslots)
        {
            if (sessions == null)
                throw new ArgumentNullException("sessions");

            if (rooms == null)
                throw new ArgumentNullException("rooms");

            if (timeslots == null)
                throw new ArgumentNullException("timeslots");

            // Make sure there are enough slots/rooms for all of the sessions
            // TODO: Subtract out any times that specific rooms are not available
            if (sessions.Count() > (rooms.Count() * timeslots.Count()))
                throw new Exceptions.NoFeasibleSolutionsException("There are not enough rooms and timeslots to accommodate all of the sessions.");

            if (sessions.Count(s => s.Presenters == null || s.Presenters.Count() < 1) > 0)
                throw new ArgumentException("Every session must have at least one presenter.");

        }
    }
}
