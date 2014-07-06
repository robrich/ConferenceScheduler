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
    public class Engine_Process_ShouldRespectPresentersWishesBy
    {
        [Test, ExpectedException(typeof(Exceptions.NoFeasibleSolutionsException))]
        public void ThrowingNoFeasibleSolutionIfSpeakerIsUnavailableForAllTimeslots()
        {
            var sessions = new SessionsCollection();
            sessions.Add(1, 1, 1);

            var rooms = new List<Room>();
            rooms.Add(TestHelper.CreateRoom(1, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(new Timeslot() { Id = 1 });

            Engine engine = new Engine();
            var assignments = engine.Process(sessions, rooms, timeslots, null);
        }

        [Test]
        public void ReturningTheCorrectAssignmentIfOneSpeakerIsAvailableForOnlyOneSlot()
        {
            Engine engine = new Engine();
            var sessions = new SessionsCollection();
            sessions.Add(1, 1);
            sessions.Add(2, 2, 2); // Only available for slot 1

            var rooms = new List<Room>();
            rooms.Add(TestHelper.CreateRoom(1, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(TestHelper.CreateTimeslot(1));
            timeslots.Add(TestHelper.CreateTimeslot(2));

            var assignments = engine.Process(sessions, rooms, timeslots, null);
            var checkAssignment = assignments.Where(a => a.SessionId == 2).Single();

            Assert.That(checkAssignment.TimeslotId, Is.EqualTo(1), "Session 2 should have been assigned to slot 1.");
        }

        [Test]
        public void ReturningTheCorrectAssignmentIfTwoSpeakersAreAvailableForTwoOfTheThreeSlots()
        {
            Engine engine = new Engine();
            var sessions = new SessionsCollection();
            sessions.Add(1, 1, 2);    // Not available for slot 2
            sessions.Add(2, 2, 2);    // Not available for slot 2
            sessions.Add(3, 3);       // Available for all but must be assigned to slot 2

            var rooms = new List<Room>();
            rooms.Add(TestHelper.CreateRoom(1, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(TestHelper.CreateTimeslot(1));
            timeslots.Add(TestHelper.CreateTimeslot(2));
            timeslots.Add(TestHelper.CreateTimeslot(3));

            var assignments = engine.Process(sessions, rooms, timeslots, null);
            var checkAssignment = assignments.Where(a => a.SessionId == 3).Single();

            Assert.That(checkAssignment.TimeslotId, Is.EqualTo(2), "Session 3 should have been assigned to slot 2.");
        }
    }
}
