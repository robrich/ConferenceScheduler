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

        IEnumerable<Session> _sessions;
        IEnumerable<Room> _rooms;
        IEnumerable<Timeslot> _timeslots;

        IEnumerable<Presenter> _presenters;

        internal AssignmentCollection Assignments { get; set; }

        internal IEnumerable<Assignment> Results
        {
            get
            {
                return this.Assignments.Where(a => a.SessionId.HasValue);
            }
        }

        internal int AssignmentsCompleted { get { return this.Assignments.AssignmentsCompleted; } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        internal bool IsFeasible
        {
            get
            {
                return AllConstraintsAreSatisfied();
            }
        }

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
                var sessionId = item.SessionIds.Single();
                var assignment = this.Assignments.GetAssignment(item.RoomId, item.TimeslotId);
                var sessionToAssign = sessions.Where(s => s.Id == sessionId).Single();
                Assign(assignment, sessionToAssign);
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
                Assign(assignment, session);
                session = this.Assignments.GetUnassignedSessionWithFewestAvailableSlots(sessions, _presenterMatrix);
            }
        }

        private void Assign(Assignment assignment, Session session)
        {
            assignment.SessionId = session.Id;
            _sessionMatrix.RemoveAssignedSessions(assignment, session.Id);
            _presenterMatrix.RemovePresentersFromSlots(assignment, session);
        }

        private bool AllConstraintsAreSatisfied()
        {
            var result = true;

            foreach (var timeslot in _timeslots)
            {
                foreach (var presenter in _presenters)
                {
                    var sessionsInTimeslot = this.Assignments.Where(a => a.TimeslotId == timeslot.Id && a.SessionId.HasValue).SelectMany(b => _sessions.Where(c => c.Id == b.SessionId));
                    var presenterSessionCount = sessionsInTimeslot.Count(s => s.Presenters.Select(a => a.Id).Contains(presenter.Id));
                    if (presenterSessionCount > 1) // A presenter can't present in more than one session at a time
                        result = false;
                    else if (presenterSessionCount == 1 && presenter.UnavailableForTimeslots != null && presenter.UnavailableForTimeslots.Contains(timeslot.Id)) // Make sure the presenter is available in the timeslot
                        result = false;
                }
            }

            return result;
        }

        private void Load(IEnumerable<Session> sessions, IEnumerable<Room> rooms, IEnumerable<Timeslot> timeslots)
        {
            _sessions = sessions;
            _rooms = rooms;
            _timeslots = timeslots;
            _presenters = sessions.SelectMany(s => s.Presenters).Distinct();

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

            // Make sure there are enough slots/rooms for all of the sessions
            if (sessions.Count() > this.Assignments.Count())
                throw new Exceptions.NoFeasibleSolutionsException("There are not enough rooms and timeslots to accommodate all of the sessions.");
        }

        private static void Validate(IEnumerable<Session> sessions, IEnumerable<Room> rooms, IEnumerable<Timeslot> timeslots)
        {
            if (sessions == null)
                throw new ArgumentNullException("sessions");

            if (rooms == null)
                throw new ArgumentNullException("rooms");

            if (timeslots == null)
                throw new ArgumentNullException("timeslots");

            if (sessions.Count(s => s.Presenters == null || s.Presenters.Count() < 1) > 0)
                throw new ArgumentException("Every session must have at least one presenter.");

        }

    }
}
