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
        SessionAvailabilityCollection _sessionMatrix;

        IEnumerable<Session> _sessions;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        IEnumerable<Room> _rooms;
        IEnumerable<Timeslot> _timeslots;

        IEnumerable<Presenter> _presenters;

        internal ICollection<Assignment> Assignments { get; set; }

        internal IEnumerable<Assignment> Results
        {
            get
            {
                return this.Assignments.Where(a => a.SessionId.HasValue);
            }
        }

        internal int AssignmentsCompleted { get { return this.Assignments.CompletedAssignmentCount(); } }

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
            this.Assignments = new List<Assignment>();
            Validate(sessions, rooms, timeslots);
            Load(sessions, rooms, timeslots);
        }

        internal void AssignSessionsWithOnlyOneOption()
        {
            // If any room-timeslot combinations only have 1 session available, assign them
            var itemsWithOneOption = _sessionMatrix.GetUnassignedItemsWithOnlyOneOption();
            while (itemsWithOneOption.Count() > 0)
            {
                var item = itemsWithOneOption.First();
                var sessionId = item.AvailableSessionIds.Single();
                var assignment = this.Assignments.GetAssignment(item.RoomId, item.TimeslotId);
                var sessionToAssign = _sessions.Where(s => s.Id == sessionId).Single();
                Assign(assignment, sessionToAssign);
                itemsWithOneOption = _sessionMatrix.GetUnassignedItemsWithOnlyOneOption();
            }
        }

        internal void AssignMostConstrainedSession()
        {
            var session = this.Assignments.GetUnassignedSessionWithFewestAvailableOptions(_sessions, _sessionMatrix);
            if (session != null)
            {
                var availableTimeslots = _sessionMatrix.GetAvailableTimeslotIds(session.Id);
                var unassignedMatrix = this.Assignments.Where(a => a.SessionId == null && availableTimeslots.Contains(a.TimeslotId));
                var assignment = unassignedMatrix.First();
                Assign(assignment, session);
                session = this.Assignments.GetUnassignedSessionWithFewestAvailableOptions(_sessions, _sessionMatrix);
            }
        }

        private void Assign(Assignment assignment, Session session)
        {
            assignment.SessionId = session.Id;
            _sessionMatrix.RemoveAssignedSessions(assignment, session.Id);
        }

        private bool AllConstraintsAreSatisfied()
        {
            return PresenterConstraintsSatisfied() && SessionDependencyConstraintsSatisfied();
        }

        private int SessionDependencyConstraintViolationCount()
        {
            int violationCount = 0;

            var sessionsWithDependencies = _sessions.Where(s => s.Dependencies != null && s.Dependencies.Count() > 0);
            foreach (var dependentSession in sessionsWithDependencies)
            {
                var dependencyAssignment = this.Assignments.GetAssignment(dependentSession.Id);
                if (dependencyAssignment != null)
                {
                    foreach (var dependency in dependentSession.Dependencies)
                    {
                        var dependentAssignment = this.Assignments.GetAssignment(dependency.Id);
                        if (dependentAssignment != null)
                        {
                            var dependentTimeslot = _timeslots.Where(t => t.Id == dependentAssignment.TimeslotId).Single();
                            var dependencyTimeslot = _timeslots.Where(t => t.Id == dependencyAssignment.TimeslotId).Single();
                            if (dependencyTimeslot.CompareTo(dependentTimeslot) <= 0)
                                violationCount++;
                        }
                    }
                }
            }

            return violationCount;
        }

        private bool SessionDependencyConstraintsSatisfied()
        {
            return (this.SessionDependencyConstraintViolationCount() == 0);
        }

        private int PresenterConstraintViolationCount()
        {
            int violationCount = 0;

            foreach (var timeslot in _timeslots)
            {
                foreach (var presenter in _presenters)
                {
                    var sessionsInTimeslot = this.Assignments.GetAssignmentsInTimeslot(timeslot.Id).SelectMany(b => _sessions.Where(c => c.Id == b.SessionId));
                    var presenterSessionCount = sessionsInTimeslot.Count(s => s.Presenters.Select(a => a.Id).Contains(presenter.Id));
                    if (presenterSessionCount > 1) // A presenter can't present in more than one session at a time
                        violationCount++;
                    else if (presenterSessionCount == 1 && presenter.IsUnavailableInTimeslot(timeslot.Id)) // Make sure the presenter is available in the timeslot
                        violationCount++;
                }
            }

            return violationCount;
        }

        private bool PresenterConstraintsSatisfied()
        {
            return (this.PresenterConstraintViolationCount() == 0);
        }

        private void Load(IEnumerable<Session> sessions, IEnumerable<Room> rooms, IEnumerable<Timeslot> timeslots)
        {
            _sessions = sessions;
            _rooms = rooms;
            _timeslots = timeslots;
            _presenters = sessions.GetPresenters();

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

            if (timeslots.Count() == 0)
                throw new ArgumentException("You must have at least one timeslot");

            if (rooms.Count() == 0)
                throw new ArgumentException("You must have at least one room");

            if (sessions.Count(s => s.Presenters == null || s.Presenters.Count() < 1) > 0)
                throw new ArgumentException("Every session must have at least one presenter.");
        }

    }
}
