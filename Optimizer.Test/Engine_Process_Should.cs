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
    public class Engine_Process_Should
    {
        [Test]
        public void RunSuccessfullyIfNoDataObjectsSupplied()
        {
            Engine engine = new Engine();
            var assignments = engine.Process(null, null, null, null);
            // Assert.That(true, Is.EqualTo(true), "It didn't blow up"); // TODO: when Process() returns something build a real assert
            Assert.True(true, "An error occurred when no data objects were supplied.");
        }

        [Test]
        public void RunSuccessfullyIfNoDataValuesSupplied()
        {
            Engine engine = new Engine();
            var sessions = new List<Session>();
            var rooms = new List<Room>();
            var timeslots = new List<Timeslot>();
            var settings = new Dictionary<string, string>();

            var assignments = engine.Process(sessions, rooms, timeslots, settings);
            Assert.True(true, "An error occurred when no data was supplied.");
        }

        [Test]
        public void RunSuccessfullyIfNoSettingsSupplied()
        {
            Engine engine = new Engine();
            var sessions = new List<Session>();
            sessions.Add(CreateSession(1, 1));

            var rooms = new List<Room>();
            rooms.Add(new Room() { Id = 1 });

            var timeslots = new List<Timeslot>();
            timeslots.Add(new Timeslot() { Id = 1 });

            var assignments = engine.Process(sessions, rooms, timeslots, null);
            Assert.True(true, "An error occurred when no settings were supplied.");
        }

        [Test]
        public void ReturnTheOnlyPossibleAssignmentIfOneSessionRoomAndSlotAreSupplied()
        {
            Engine engine = new Engine();
            var sessions = new List<Session>();
            sessions.Add(CreateSession(1,1));

            var rooms = new List<Room>();
            rooms.Add(new Room() { Id = 1 });

            var timeslots = new List<Timeslot>();
            timeslots.Add(new Timeslot() { Id = 1 });

            var assignments = engine.Process(sessions, rooms, timeslots, null);
            Assert.That(assignments.Count(), Is.EqualTo(1), "The wrong number of assignments were returned.");
        }

        [Test, ExpectedException(typeof(Exceptions.NoFeasibleSolutionsException))]
        public void ThrowNoFeasibleSolutionIfSpeakerIsUnavailableForAllTimeslots()
        {
            var sessions = new List<Session>();
            sessions.Add(CreateSession(1, 1, 1));

            var rooms = new List<Room>();
            rooms.Add(new Room() { Id = 1 });

            var timeslots = new List<Timeslot>();
            timeslots.Add(new Timeslot() { Id = 1 });

            Engine engine = new Engine();
            var assignments = engine.Process(sessions, rooms, timeslots, null);
        }

        [Test, ExpectedException(typeof(Exceptions.NoFeasibleSolutionsException))]
        public void ThrowNoFeasibleSolutionIfThereAreMoreSessionsThanSlotsAndRooms()
        {
            Engine engine = new Engine();
            var sessions = new List<Session>();
            sessions.Add(CreateSession(1, 1));
            sessions.Add(CreateSession(2, 2));

            var rooms = new List<Room>();
            rooms.Add(new Room() { Id = 1 });

            var timeslots = new List<Timeslot>();
            timeslots.Add(new Timeslot() { Id = 1 });

            var assignments = engine.Process(sessions, rooms, timeslots, null);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ThrowArgumentExceptionIfThereIsntAtLeastOnePresenterForEachSession()
        {
            Engine engine = new Engine();
            var sessions = new List<Session>();
            sessions.Add(CreateSession(1, 1));
            sessions.Add(new Session() { Id = 2 });

            var rooms = new List<Room>();
            rooms.Add(new Room() { Id = 1 });

            var timeslots = new List<Timeslot>();
            timeslots.Add(new Timeslot() { Id = 1 });
            timeslots.Add(new Timeslot() { Id = 2 });

            var assignments = engine.Process(sessions, rooms, timeslots, null);
        }



        private static Session CreateSession(int id, Int16 presenterId, params Int16[] presenterUnavailableForTimeslots)
        {
            return new Session()
            {
                Id = id,
                Presenters = new List<Presenter>() 
                        { 
                            new Presenter() 
                                { 
                                    Id = presenterId,  
                                    UnavailableForTimeslots = presenterUnavailableForTimeslots.AsEnumerable()
                                } 
                        }
            };
        }

    }
}
