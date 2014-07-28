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

        internal Solution(Solution solution)
        {
            Load(solution);
        }

        internal int AssignSessionsWithOnlyOneOption()
        {
            // If any room-timeslot combinations only have 1 session available, assign them
            int changeCount = 0;
            var itemsWithOneOption = _sessionMatrix.GetUnassignedItemsWithOnlyOneOption();
            while (itemsWithOneOption.Count() > 0)
            {
                var item = itemsWithOneOption.First();
                var sessionId = item.AvailableSessionIds.Single();
                var assignment = this.Assignments.GetAssignment(item.RoomId, item.TimeslotId);
                var sessionToAssign = _sessions.Where(s => s.Id == sessionId).Single();
                Assign(assignment, sessionToAssign);
                changeCount++;
                itemsWithOneOption = _sessionMatrix.GetUnassignedItemsWithOnlyOneOption();
            }
            return changeCount;
        }

        internal int AssignMostConstrainedSession()
        {
            int changeCount = 0;
            var session = this.Assignments.GetUnassignedSessionWithFewestAvailableOptions(_sessions, _sessionMatrix);
            if (session != null)
            {
                var availableTimeslots = _sessionMatrix.GetAvailableTimeslotIds(session.Id);
                var unassignedMatrix = this.Assignments.Where(a => a.SessionId == null && availableTimeslots.Contains(a.TimeslotId));
                var assignment = unassignedMatrix.First();
                Assign(assignment, session);
                changeCount++;
                session = this.Assignments.GetUnassignedSessionWithFewestAvailableOptions(_sessions, _sessionMatrix);
            }
            return changeCount;
        }

        private void Assign(Assignment assignment, Session session)
        {
            assignment.SessionId = session.Id;
            _sessionMatrix.UpdateConstraints(assignment, session);
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

        internal int GetScore()
        {
            int result = 0;

            foreach (var timeslot in _timeslots)
            {
                result += timeslot.GetTopicScore(this.Assignments, _sessions);
            }

            return result;
        }

        internal Solution SwapAssignments()
        {
            var newSolution = this.Clone();
            if (newSolution.Assignments.Count() > 1)
            {
                var a1 = newSolution.Assignments.GetRandom();
                var a2 = newSolution.Assignments.GetRandom(a1.SessionId.Value);
                var temp = a2.SessionId;
                a2.SessionId = a1.SessionId;
                a1.SessionId = temp;
            }
            return newSolution;
        }

        private Solution Clone()
        {
            return new Solution(this);
        }

        internal Solution Optimize(Action<ProcessUpdateEventArgs> updateEventHandler)
        {
            var bestSolution = this;
            var bestScore = bestSolution.GetScore();

            var maxAttempts = Convert.ToInt32(System.Math.Pow(2.0, Convert.ToDouble(this.Assignments.Count() - 1)));
            var attemptCount = 0;
            
            Solution solution;
            do
            {
                // Try to improve the score
                solution = bestSolution.SwapAssignments();
                attemptCount++;
                var currentScore = solution.GetScore();
                if ((solution.IsFeasible) && (currentScore < bestScore))
                {
                    bestSolution = solution;
                    bestSolution.RaiseUpdateEvent(updateEventHandler);
                }
            } while (attemptCount < maxAttempts);

            return bestSolution;
        }

        internal void RaiseUpdateEvent(Action<ProcessUpdateEventArgs> updateEventHandler)
        {
            if (updateEventHandler != null)
            {
                var args = new ProcessUpdateEventArgs() { Assignments = this.Assignments.ToList() };
                updateEventHandler.Invoke(args);
            }
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

        private void Load(Solution solution)
        {
            _sessions = solution._sessions.ToList();
            _rooms = solution._rooms.ToList();
            _timeslots = solution._timeslots.ToList();
            _presenters = solution._presenters.ToList();
            _sessionMatrix = solution._sessionMatrix.Clone();
            this.Assignments = solution.Assignments.Clone();
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

            if (sessions.HaveCircularDependencies())
                throw new Exceptions.DependencyException("Sessions may not have circular dependencies.");
        }

    }
}
