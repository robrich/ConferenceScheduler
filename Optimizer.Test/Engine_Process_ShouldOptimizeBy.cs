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
    public class Engine_Process_ShouldOptimizeBy
    {
        [Test]
        public void ReturningTheOnlyPossibleAssignmentIfOneSessionRoomAndSlotAreSupplied()
        {
            Engine engine = new Engine();
            var sessions = new SessionsCollection();
            sessions.Add(1, 1);

            var rooms = new List<Room>();
            rooms.Add(TestHelper.CreateRoom(1, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(TestHelper.CreateTimeslot(1));

            var assignments = engine.Process(sessions, rooms, timeslots, null);
            Assert.That(assignments.Count(), Is.EqualTo(1), "The wrong number of assignments were returned.");
        }

        [Test]
        public void AssigningAllSessions()
        {
            Engine engine = new Engine();
            var sessions = new SessionsCollection();
            sessions.Add(1, 1);
            sessions.Add(2, 2);
            sessions.Add(3, 2);
            sessions.Add(4, 3);
            sessions.Add(5, 3);
            sessions.Add(6, 3);
            sessions.Add(7, 3);

            var rooms = new List<Room>();
            rooms.Add(TestHelper.CreateRoom(1, 10));
            rooms.Add(TestHelper.CreateRoom(2, 10));
            rooms.Add(TestHelper.CreateRoom(3, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(TestHelper.CreateTimeslot(1));
            timeslots.Add(TestHelper.CreateTimeslot(2));
            timeslots.Add(TestHelper.CreateTimeslot(3));
            timeslots.Add(TestHelper.CreateTimeslot(4));
            timeslots.Add(TestHelper.CreateTimeslot(5));

            var assignments = engine.Process(sessions, rooms, timeslots, null);
            var assignmentsWithSessions = assignments.Where(a => a.SessionId.HasValue);
            Assert.That(assignmentsWithSessions.Count(), Is.EqualTo(sessions.Count()), "The wrong number of assignments were returned.");
        }
    }
}
