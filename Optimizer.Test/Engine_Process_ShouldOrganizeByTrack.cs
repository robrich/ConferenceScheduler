using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceScheduler.Optimizer;
using ConferenceScheduler.Entities;
using ConferenceScheduler.Interfaces;

namespace ConferenceScheduler.Optimizer.Test
{
    [TestFixture]
    public class Engine_Process_ShouldOrganizeByTrack
    {
        [Test]
        public void PuttingSessionsInTheSameTrackIntoTheSameRoom_4Sessions1Track()
        {
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

            var engine = (null as IConferenceOptimizer).Create();

            var assignments = engine.Process(sessions, rooms, timeslots);
            assignments.WriteSchedule();

            var s1RoomId = assignments.Where(a => a.SessionId == 1).Single().RoomId;
            var s3RoomId = assignments.Where(a => a.SessionId == 3).Single().RoomId;
            Assert.That(s1RoomId, Is.EqualTo(s3RoomId), "Sessions with the same TopicId should be in the same room whenever possible.");
        }

        [Test]
        public void PuttingSessionsInTheSameTrackIntoTheSameRoom_5Sessions2Tracks()
        {
            var sessions = new SessionsCollection();
            sessions.Add(1, 1, Presenter.Create(1));
            sessions.Add(2, 2, Presenter.Create(2));
            sessions.Add(3, 1, Presenter.Create(3));
            sessions.Add(4, 1, Presenter.Create(4));
            sessions.Add(5, 2, Presenter.Create(5));

            var rooms = new List<Room>();
            rooms.Add(Room.Create(1, 10));
            rooms.Add(Room.Create(2, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(Timeslot.Create(1, 9.0));
            timeslots.Add(Timeslot.Create(2, 10.25));
            timeslots.Add(Timeslot.Create(3, 11.5));

            var engine = (null as IConferenceOptimizer).Create();

            var assignments = engine.Process(sessions, rooms, timeslots);
            assignments.WriteSchedule();

            var s1RoomId = assignments.Where(a => a.SessionId == 1).Single().RoomId;
            var s3RoomId = assignments.Where(a => a.SessionId == 3).Single().RoomId;
            var s4RoomId = assignments.Where(a => a.SessionId == 4).Single().RoomId;

            var s2RoomId = assignments.Where(a => a.SessionId == 2).Single().RoomId;
            var s5RoomId = assignments.Where(a => a.SessionId == 5).Single().RoomId;

            Assert.That(s1RoomId, Is.EqualTo(s3RoomId), "Sessions with the same TopicId should be in the same room whenever possible (S1 & S3).");
            Assert.That(s1RoomId, Is.EqualTo(s4RoomId), "Sessions with the same TopicId should be in the same room whenever possible (S1 & S4).");
            Assert.That(s2RoomId, Is.EqualTo(s5RoomId), "Sessions with the same TopicId should be in the same room whenever possible (S2 & S5).");
        }

        //[Test]
        //public void SeparatingSessionsInTheSameTrackIntoDifferentTimslots_4Sessions1Track()
        //{
        //    var sessions = new SessionsCollection();
        //    sessions.Add(1, null, Presenter.Create(1));
        //    sessions.Add(2, 1, Presenter.Create(2));
        //    sessions.Add(3, null, Presenter.Create(3));
        //    sessions.Add(4, 1, Presenter.Create(4));

        //    var rooms = new List<Room>();
        //    rooms.Add(Room.Create(1, 10));
        //    rooms.Add(Room.Create(2, 10));

        //    var timeslots = new List<Timeslot>();
        //    timeslots.Add(Timeslot.Create(1, 9.0));
        //    timeslots.Add(Timeslot.Create(2, 10.25));

        //    var engine = (null as IConferenceOptimizer).Create();

        //    var assignments = engine.Process(sessions, rooms, timeslots);
        //    assignments.WriteSchedule();

        //    var s2TimeslotId = assignments.Where(a => a.SessionId == 2).Single().TimeslotId;
        //    var s4TimeslotId = assignments.Where(a => a.SessionId == 4).Single().TimeslotId;
        //    Assert.That(s2TimeslotId, Is.Not.EqualTo(s4TimeslotId), "Sessions with the same TopicId should not be in the same timeslot.");
        //}

        //[Test]
        //public void SeparatingSessionsInTheSameTrackIntoDifferentTimslots_6Sessions3Tracks()
        //{
        //    var engine = (null as IConferenceOptimizer).Create();

        //    var sessions = new SessionsCollection();
        //    sessions.Add(1, 1, Presenter.Create(1));
        //    sessions.Add(2, 2, Presenter.Create(2));
        //    sessions.Add(3, 1, Presenter.Create(3));
        //    sessions.Add(4, 1, Presenter.Create(4));
        //    sessions.Add(5, null, Presenter.Create(5));
        //    sessions.Add(6, 3, Presenter.Create(6));

        //    var rooms = new List<Room>();
        //    rooms.Add(Room.Create(1, 10));
        //    rooms.Add(Room.Create(2, 10));

        //    var timeslots = new List<Timeslot>();
        //    timeslots.Add(Timeslot.Create(1, 9.0));
        //    timeslots.Add(Timeslot.Create(2, 10.25));
        //    timeslots.Add(Timeslot.Create(3, 11.5));

        //    var assignments = engine.Process(sessions, rooms, timeslots);
        //    assignments.WriteSchedule();

        //    var s1TimeslotId = assignments.Where(a => a.SessionId == 1).Single().TimeslotId;
        //    var s3TimeslotId = assignments.Where(a => a.SessionId == 3).Single().TimeslotId;
        //    var s4TimeslotId = assignments.Where(a => a.SessionId == 4).Single().TimeslotId;
        //    var slotsAreDifferent = ((s1TimeslotId != s3TimeslotId) && (s1TimeslotId != s4TimeslotId) && (s3TimeslotId != s4TimeslotId));

        //    Assert.True(slotsAreDifferent, "Sessions with the same TopicId should not be in the same timeslot.");
        //}

        //[Test]
        //public void AssigningAllSessionsToDifferentTimslotRoomCombinations()
        //{
        //    var engine = (null as IConferenceOptimizer).Create();

        //    var sessions = new SessionsCollection();
        //    sessions.Add(1, 1, Presenter.Create(1));
        //    sessions.Add(2, 2, Presenter.Create(2));
        //    sessions.Add(3, 1, Presenter.Create(3));
        //    sessions.Add(4, 1, Presenter.Create(4));
        //    sessions.Add(5, null, Presenter.Create(5));
        //    sessions.Add(6, 3, Presenter.Create(6));

        //    var rooms = new List<Room>();
        //    rooms.Add(Room.Create(1, 10));
        //    rooms.Add(Room.Create(2, 10));

        //    var timeslots = new List<Timeslot>();
        //    timeslots.Add(Timeslot.Create(1, 9.0));
        //    timeslots.Add(Timeslot.Create(2, 10.25));
        //    timeslots.Add(Timeslot.Create(3, 11.5));

        //    var assignments = engine.Process(sessions, rooms, timeslots);
        //    assignments.WriteSchedule();

        //    bool hasDuplicate = false;
        //    for (int r = 0; r < rooms.Count; r++)
        //        for (int t = 0; t < timeslots.Count; t++)
        //            hasDuplicate = (hasDuplicate && (assignments.Count(a => (a.RoomId == r) && (a.TimeslotId == t)) > 1));

        //    Assert.False(hasDuplicate, "No 2 sessions should be in the same room-timeslot combination.");
        //}


    }
}
