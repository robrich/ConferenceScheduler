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
            rooms.Add(Room.Create(1, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(Timeslot.Create(1));

            Engine engine = new Engine();
            var assignments = engine.Process(sessions, rooms, timeslots);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ThrowingArgumentNullExceptionIfSessionsIsNull()
        {
            IEnumerable<Session> sessions = null;

            var rooms = new List<Room>();
            rooms.Add(Room.Create(1, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(Timeslot.Create(1));

            Engine engine = new Engine();
            var assignments = engine.Process(sessions, rooms, timeslots);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ThrowingArgumentExceptionIfNoRoomsSupplied()
        {
            var sessions = new SessionsCollection();
            sessions.Add(1, 1);

            var rooms = new List<Room>();

            var timeslots = new List<Timeslot>();
            timeslots.Add(Timeslot.Create(1));

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
            timeslots.Add(Timeslot.Create(1));

            Engine engine = new Engine();
            var assignments = engine.Process(sessions, rooms, timeslots);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ThrowingArgumentExceptionIfNoTimeslotsSupplied()
        {
            var sessions = new SessionsCollection();
            sessions.Add(1, 1);

            var rooms = new List<Room>();
            rooms.Add(Room.Create(1, 10));

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
            rooms.Add(Room.Create(1, 10));

            IEnumerable<Timeslot> timeslots = null;

            Engine engine = new Engine();
            var assignments = engine.Process(sessions, rooms, timeslots);
        }

        [Test, ExpectedException(typeof(Exceptions.NoFeasibleSolutionsException))]
        public void ThrowingNoFeasibleSolutionIfThereAreMoreSessionsThanSlotsAndRooms()
        {
            Engine engine = new Engine();
            var sessions = new SessionsCollection();
            sessions.Add(1, null, Presenter.Create(1));
            sessions.Add(2, null, Presenter.Create(2));

            var rooms = new List<Room>();
            rooms.Add(Room.Create(1, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(Timeslot.Create(1));

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
            rooms.Add(Room.Create(1, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(Timeslot.Create(1));
            timeslots.Add(Timeslot.Create(2));

            var assignments = engine.Process(sessions, rooms, timeslots);
        }

        [Test, ExpectedException(typeof(Exceptions.NoFeasibleSolutionsException))]
        public void ThrowingNoFeasibleSolutionIfAvailableTimeslotsForAMultiPresenterSessionDontIntersect()
        {
            // 2 presenters for one session where neither
            // is available to present when the other is available

            var presenter1 = Presenter.Create(1, 2);
            var presenter2 = Presenter.Create(2, 1);

            var sessions = new SessionsCollection();
            sessions.Add(1, null, presenter1, presenter2);

            var rooms = new List<Room>() { Room.Create(1, 10) };

            var timeslots = new List<Timeslot>();
            timeslots.Add(Timeslot.Create(1));
            timeslots.Add(Timeslot.Create(2));

            Engine engine = new Engine();
            var assignments = engine.Process(sessions, rooms, timeslots);
        }

        [Test, ExpectedException(typeof(Exceptions.DependencyException))]
        public void ThrowingDependencyExceptionIfCircularDependenciesExist()
        {

            var presenter1 = Presenter.Create(1, 2);

            var sessions = new SessionsCollection();

            var session1 = sessions.Add(1, 1, Presenter.Create(1));
            var session2 = sessions.Add(2, 1, Presenter.Create(2));
            var session3 = sessions.Add(3, 1, Presenter.Create(3));

            session1.AddDependency(session2);
            session2.AddDependency(session3);
            session3.AddDependency(session1);

            var rooms = new List<Room>();
            rooms.Add(Room.Create(1, 10));
            rooms.Add(Room.Create(2, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(Timeslot.Create(1));
            timeslots.Add(Timeslot.Create(2));

            Engine engine = new Engine();
            var assignments = engine.Process(sessions, rooms, timeslots);

        }
    }
}
