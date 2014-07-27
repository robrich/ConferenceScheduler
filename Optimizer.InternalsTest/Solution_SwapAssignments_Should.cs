using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceScheduler.Entities;
using ConferenceScheduler.Optimizer;
using ConferenceScheduler.Optimizer.Test;

namespace ConferenceScheduler.Optimizer.InternalsTest
{
    [TestFixture]
    public class Solution_SwapAssignments_Should
    {
        [Test]
        public void ResultInTheSameNumberOfAssignments()
        {
            var sessions = new SessionsCollection();
            sessions.Add(1, null, Presenter.Create(1));
            sessions.Add(2, 1, Presenter.Create(2));
            sessions.Add(3, null, Presenter.Create(3));
            sessions.Add(4, 1, Presenter.Create(4));

            var rooms = new List<Room>();
            rooms.Add(Room.Create(1, 10));
            rooms.Add(Room.Create(2, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(Timeslot.Create(1, 9.0));
            timeslots.Add(Timeslot.Create(2, 10.25));

            var solution = new Solution(sessions, rooms, timeslots);
            while (solution.AssignmentsCompleted < sessions.Count())
            {
                solution.AssignSessionsWithOnlyOneOption();
                solution.AssignMostConstrainedSession();
            }

            var s2 = solution.SwapAssignments();

            Assert.AreEqual(solution.Assignments.Count(), s2.Assignments.Count());
        }
        
        [Test]
        public void ResultInTwoDifferencesInAssignments()
        {
            var sessions = new SessionsCollection();
            sessions.Add(1, null, Presenter.Create(1));
            sessions.Add(2, 1, Presenter.Create(2));
            sessions.Add(3, null, Presenter.Create(3));
            sessions.Add(4, 1, Presenter.Create(4));

            var rooms = new List<Room>();
            rooms.Add(Room.Create(1, 10));
            rooms.Add(Room.Create(2, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(Timeslot.Create(1, 9.0));
            timeslots.Add(Timeslot.Create(2, 10.25));

            var solution = new Solution(sessions, rooms, timeslots);
            while (solution.AssignmentsCompleted < sessions.Count())
            {
                solution.AssignSessionsWithOnlyOneOption();
                solution.AssignMostConstrainedSession();
            }

            var s2 = solution.SwapAssignments();

            int differenceCount = 0;
            foreach (var a1 in solution.Assignments)
            {
                var a2 = s2.Assignments.Where(a => a.RoomId == a1.RoomId && a.TimeslotId == a1.TimeslotId).Single();
                if (a1.SessionId != a2.SessionId)
                    differenceCount++;
            }

            Assert.AreEqual(2, differenceCount);
        }

        [Test]
        public void KeepTheOriginalAssignmentsUnchanged()
        {
            var sessions = new SessionsCollection();
            sessions.Add(1, null, Presenter.Create(1));
            sessions.Add(2, 1, Presenter.Create(2));
            sessions.Add(3, null, Presenter.Create(3));
            sessions.Add(4, 1, Presenter.Create(4));

            var rooms = new List<Room>();
            rooms.Add(Room.Create(1, 10));
            rooms.Add(Room.Create(2, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(Timeslot.Create(1, 9.0));
            timeslots.Add(Timeslot.Create(2, 10.25));

            var solution = new Solution(sessions, rooms, timeslots);
            while (solution.AssignmentsCompleted < sessions.Count())
            {
                solution.AssignSessionsWithOnlyOneOption();
                solution.AssignMostConstrainedSession();
            }

            var originalSolution = solution.Assignments.Serialize();
            solution.Assignments.WriteSchedule();

            var s2 = solution.SwapAssignments();
            var postSwapSolution = solution.Assignments.Serialize();
            solution.Assignments.WriteSchedule();

            Assert.AreEqual(originalSolution, postSwapSolution);
        }

    }
}
