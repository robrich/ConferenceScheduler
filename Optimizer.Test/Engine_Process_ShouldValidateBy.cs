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
    public class Engine_Process_ShouldValidateBy
    {
        [Test]
        public void RunningSuccessfullyIfNoDataObjectsSupplied()
        {
            Engine engine = new Engine();
            var assignments = engine.Process(null, null, null, null);
            // Assert.That(true, Is.EqualTo(true), "It didn't blow up"); // TODO: when Process() returns something build a real assert
            Assert.True(true, "An error occurred when no data objects were supplied.");
        }

        [Test]
        public void RunningSuccessfullyIfNoDataValuesSupplied()
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
        public void RunningSuccessfullyIfNoSettingsSupplied()
        {
            Engine engine = new Engine();
            var sessions = new List<Session>();
            sessions.Add(TestHelper.CreateSession(1, 1));

            var rooms = new List<Room>();
            rooms.Add(TestHelper.CreateRoom(1, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(TestHelper.CreateTimeslot(1));

            var assignments = engine.Process(sessions, rooms, timeslots, null);
            Assert.True(true, "An error occurred when no settings were supplied.");
        }

        [Test, ExpectedException(typeof(Exceptions.NoFeasibleSolutionsException))]
        public void ThrowingNoFeasibleSolutionIfThereAreMoreSessionsThanSlotsAndRooms()
        {
            Engine engine = new Engine();
            var sessions = new List<Session>();
            sessions.Add(TestHelper.CreateSession(1, 1));
            sessions.Add(TestHelper.CreateSession(2, 2));

            var rooms = new List<Room>();
            rooms.Add(TestHelper.CreateRoom(1, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(TestHelper.CreateTimeslot(1));

            var assignments = engine.Process(sessions, rooms, timeslots, null);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ThrowingArgumentExceptionIfThereIsntAtLeastOnePresenterForEachSession()
        {
            Engine engine = new Engine();
            var sessions = new List<Session>();
            sessions.Add(TestHelper.CreateSession(1, 1));
            sessions.Add(new Session() { Id = 2 });

            var rooms = new List<Room>();
            rooms.Add(TestHelper.CreateRoom(1, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(TestHelper.CreateTimeslot(1));
            timeslots.Add(TestHelper.CreateTimeslot(2));

            var assignments = engine.Process(sessions, rooms, timeslots, null);
        }

        [Test, ExpectedException(typeof(Exceptions.NoFeasibleSolutionsException))]
        public void ThrowingNoFeasibleSolutionIfAvailableTimeslotsForAMultiPresenterSessionDontIntersect()
        {
            // 2 presenters for one session where neither
            // is available to present when the other is available

            var presenter1 = TestHelper.CreatePresenter(1, 2);
            var presenter2 = TestHelper.CreatePresenter(2, 1);
            var presenters = new List<Presenter>() { presenter1, presenter2 };

            var sessions = new List<Session>() { TestHelper.CreateSession(1, presenters) };
            var rooms = new List<Room>() { TestHelper.CreateRoom(1, 10) };

            var timeslots = new List<Timeslot>();
            timeslots.Add(TestHelper.CreateTimeslot(1));
            timeslots.Add(TestHelper.CreateTimeslot(2));

            Engine engine = new Engine();
            var assignments = engine.Process(sessions, rooms, timeslots, null);
        }
    }
}
