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
            var sessions = new List<Session>();
            sessions.Add(TestHelper.CreateSession(1, 1));

            var rooms = new List<Room>();
            rooms.Add(TestHelper.CreateRoom(1, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(TestHelper.CreateTimeslot(1));

            var assignments = engine.Process(sessions, rooms, timeslots, null);
            Assert.That(assignments.Count(), Is.EqualTo(1), "The wrong number of assignments were returned.");
        }


    }
}
