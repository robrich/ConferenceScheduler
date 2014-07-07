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

        [Test, ExpectedException(typeof(Exceptions.NoFeasibleSolutionsException))]
        public void ThrowingNoFeasibleSolutionsExceptionIfNoSessionsSupplied()
        {
            var sessions = new SessionsCollection();

            var rooms = new List<Room>();
            rooms.Add(TestHelper.CreateRoom(1, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(TestHelper.CreateTimeslot(1));

            Engine engine = new Engine();
            var assignments = engine.Process(sessions, rooms, timeslots);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ThrowingArgumentNullExceptionIfSessionsIsNull()
        {
            IEnumerable<Session> sessions = null;

            var rooms = new List<Room>();
            rooms.Add(TestHelper.CreateRoom(1, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(TestHelper.CreateTimeslot(1));

            Engine engine = new Engine();
            var assignments = engine.Process(sessions, rooms, timeslots);
        }

        [Test, ExpectedException(typeof(Exceptions.NoFeasibleSolutionsException))]
        public void ThrowingNoFeasibleSolutionsExceptionIfNoRoomsSupplied()
        {
            var sessions = new SessionsCollection();
            sessions.Add(1, 1);

            var rooms = new List<Room>();

            var timeslots = new List<Timeslot>();
            timeslots.Add(TestHelper.CreateTimeslot(1));

            Engine engine = new Engine();
            var assignments = engine.Process(sessions, rooms, timeslots);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ThrowingArgumentNullExceptionIfRoomsIsNull()
        {
            var sessions = new SessionsCollection();
            sessions.Add(1, 1);

            IEnumerable<Room> rooms = null;

            var timeslots = new List<Timeslot>();
            timeslots.Add(TestHelper.CreateTimeslot(1));

            Engine engine = new Engine();
            var assignments = engine.Process(sessions, rooms, timeslots);
        }

        [Test, ExpectedException(typeof(Exceptions.NoFeasibleSolutionsException))]
        public void ThrowingNoFeasibleSolutionsExceptionIfNoTimeslotsSupplied()
        {
            var sessions = new SessionsCollection();
            sessions.Add(1, 1);

            var rooms = new List<Room>();
            rooms.Add(TestHelper.CreateRoom(1, 10));

            var timeslots = new List<Timeslot>();

            Engine engine = new Engine();
            var assignments = engine.Process(sessions, rooms, timeslots);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ThrowingArgumentNullExceptionIfTimeslotsIsNull()
        {
            var sessions = new SessionsCollection();
            sessions.Add(1, 1);

            var rooms = new List<Room>();
            rooms.Add(TestHelper.CreateRoom(1, 10));

            IEnumerable<Timeslot> timeslots = null;

            Engine engine = new Engine();
            var assignments = engine.Process(sessions, rooms, timeslots);
        }

        [Test, ExpectedException(typeof(Exceptions.NoFeasibleSolutionsException))]
        public void ThrowingNoFeasibleSolutionIfThereAreMoreSessionsThanSlotsAndRooms()
        {
            Engine engine = new Engine();
            var sessions = new SessionsCollection();
            sessions.Add(1, 1);
            sessions.Add(2, 2);

            var rooms = new List<Room>();
            rooms.Add(TestHelper.CreateRoom(1, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(TestHelper.CreateTimeslot(1));

            var assignments = engine.Process(sessions, rooms, timeslots);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ThrowingArgumentExceptionIfThereIsntAtLeastOnePresenterForEachSession()
        {
            Engine engine = new Engine();
            var sessions = new SessionsCollection();
            sessions.Add(1, 1);
            sessions.Add(new Session() { Id = 2 });

            var rooms = new List<Room>();
            rooms.Add(TestHelper.CreateRoom(1, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(TestHelper.CreateTimeslot(1));
            timeslots.Add(TestHelper.CreateTimeslot(2));

            var assignments = engine.Process(sessions, rooms, timeslots);
        }

        [Test, ExpectedException(typeof(Exceptions.NoFeasibleSolutionsException))]
        public void ThrowingNoFeasibleSolutionIfAvailableTimeslotsForAMultiPresenterSessionDontIntersect()
        {
            // 2 presenters for one session where neither
            // is available to present when the other is available

            var presenter1 = TestHelper.CreatePresenter(1, 2);
            var presenter2 = TestHelper.CreatePresenter(2, 1);

            var sessions = new SessionsCollection();
            sessions.Add(1, presenter1, presenter2);

            var rooms = new List<Room>() { TestHelper.CreateRoom(1, 10) };

            var timeslots = new List<Timeslot>();
            timeslots.Add(TestHelper.CreateTimeslot(1));
            timeslots.Add(TestHelper.CreateTimeslot(2));

            Engine engine = new Engine();
            var assignments = engine.Process(sessions, rooms, timeslots);
        }
    }
}
