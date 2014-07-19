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
            sessions.Add(1, null, Presenter.Create(1));

            var rooms = new List<Room>();
            rooms.Add(Room.Create(1, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(Timeslot.Create(1));

            var assignments = engine.Process(sessions, rooms, timeslots);
            assignments.WriteSchedule();
            Assert.That(assignments.Count(), Is.EqualTo(1), "The wrong number of assignments were returned.");
        }

        [Test]
        public void AssigningAllSessions()
        {
            Engine engine = new Engine();
            var sessions = new SessionsCollection();
            sessions.Add(1, null, Presenter.Create(1));
            sessions.Add(2, null, Presenter.Create(2));
            sessions.Add(3, null, Presenter.Create(2));
            sessions.Add(4, null, Presenter.Create(3));
            sessions.Add(5, null, Presenter.Create(3));
            sessions.Add(6, null, Presenter.Create(3));
            sessions.Add(7, null, Presenter.Create(3));

            var rooms = new List<Room>();
            rooms.Add(Room.Create(1, 10));
            rooms.Add(Room.Create(2, 10));
            rooms.Add(Room.Create(3, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(Timeslot.Create(1));
            timeslots.Add(Timeslot.Create(2));
            timeslots.Add(Timeslot.Create(3));
            timeslots.Add(Timeslot.Create(4));
            timeslots.Add(Timeslot.Create(5));

            var assignments = engine.Process(sessions, rooms, timeslots);
            var assignmentsWithSessions = assignments.Where(a => a.SessionId.HasValue);
            assignments.WriteSchedule();
            Assert.That(assignmentsWithSessions.Count(), Is.EqualTo(sessions.Count()), "The wrong number of assignments were returned.");
        }

        [Test]
        public void SeparatingSessionsInTheSameTrackIntoDifferentTimslots()
        {
            Engine engine = new Engine();
            var sessions = new SessionsCollection();
            sessions.Add(1, 1, Presenter.Create(1));
            sessions.Add(2, 2, Presenter.Create(2));
            sessions.Add(3, 1, Presenter.Create(3));
            sessions.Add(4, 3, Presenter.Create(4));

            var rooms = new List<Room>();
            rooms.Add(Room.Create(1, 10));
            rooms.Add(Room.Create(2, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(Timeslot.Create(1, 9.0));
            timeslots.Add(Timeslot.Create(2, 10.25));

            var assignments = engine.Process(sessions, rooms, timeslots);
            assignments.WriteSchedule();

            var s1TimeslotId = assignments.Where(a => a.SessionId == 1).Single().TimeslotId;
            var s3TimeslotId = assignments.Where(a => a.SessionId == 3).Single().TimeslotId;
            Assert.That(s1TimeslotId, Is.Not.EqualTo(s3TimeslotId), "Sessions with the same TopicId should not be in the same timeslot.");
        }

    }
}
