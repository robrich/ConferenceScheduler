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
    public class Engine_Process_ShouldRespectVenueNeedsBy
    {
        [Test]
        public void NotAssigningASessionToARoomWhenItIsNotAvailable()
        {
            Engine engine = new Engine();
            var sessions = new SessionsCollection();
            sessions.Add(1, 1);
            sessions.Add(2, 2);
            sessions.Add(3, 3);

            var rooms = new List<Room>();
            rooms.Add(Room.Create(1, 10));
            rooms.Add(Room.Create(2, 10, 1));

            var timeslots = new List<Timeslot>();
            timeslots.Add(Timeslot.Create(1));
            timeslots.Add(Timeslot.Create(2));

            var assignments = engine.Process(sessions, rooms, timeslots);
            var checkAssignment = assignments.Where(a => a.RoomId == 2 && a.TimeslotId == 1).SingleOrDefault();

            if (checkAssignment == null)
                Assert.Null(checkAssignment);
            else
                Assert.Null(checkAssignment.SessionId, "No session should have been assigned to room 2 during timeslot 1.");
        }

    }
}
