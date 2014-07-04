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
            var sessions = new List<Session>();
            sessions.Add(TestHelper.CreateSession(1, 1, 1));

            var rooms = new List<Room>();
            rooms.Add(TestHelper.CreateRoom(1, 10));

            var timeslots = new List<Timeslot>();
            timeslots.Add(new Timeslot() { Id = 1 });

            Engine engine = new Engine();
            var assignments = engine.Process(sessions, rooms, timeslots, null);
        }


    }
}
