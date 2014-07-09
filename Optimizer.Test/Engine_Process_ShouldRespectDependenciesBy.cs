using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceScheduler.Optimizer;
using ConferenceScheduler.Entities;

namespace ConferenceScheduler.Optimizer.Test
{
    [TestFixture]
    public class Engine_Process_ShouldRespectDependenciesBy
    {
        [Test]
        public void ReturningTheOnlyPossibleAssignmentIfTheFirstSessionIsDependentOnTheSecond()
        {
            Engine engine = new Engine();
            var sessions = new SessionsCollection();
            var session1 = sessions.Add(1, 1);
            var session2 = sessions.Add(2, 1);
            session1.AddDependency(session2);

            var rooms = new List<Room>();
            rooms.Add(TestHelper.CreateRoom(1, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(TestHelper.CreateTimeslot(1, 10));
            timeslots.Add(TestHelper.CreateTimeslot(2, 11));

            var assignments = engine.Process(sessions, rooms, timeslots);
            var session1Assignment = assignments.Where(a => a.SessionId.Value == 1).Single();
            Assert.That(session1Assignment.TimeslotId, Is.EqualTo(2), "Session 1 must be assigned to timeslot 2 to satisfy the dependencies.");
        }

        [Test]
        public void ReturningTheOnlyPossibleAssignmentIfTheSecondSessionIsDependentOnTheFirst()
        {
            Engine engine = new Engine();
            var sessions = new SessionsCollection();
            var session1 = sessions.Add(1, 1);
            var session2 = sessions.Add(2, 1);
            session2.AddDependency(session1);

            var rooms = new List<Room>();
            rooms.Add(TestHelper.CreateRoom(1, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(TestHelper.CreateTimeslot(1, 10));
            timeslots.Add(TestHelper.CreateTimeslot(2, 11));

            var assignments = engine.Process(sessions, rooms, timeslots);
            var session2Assignment = assignments.Where(a => a.SessionId.Value == 2).Single();
            Assert.That(session2Assignment.TimeslotId, Is.EqualTo(2), "Session 2 must be assigned to timeslot 2 to satisfy the dependencies.");
        }
    }
}
