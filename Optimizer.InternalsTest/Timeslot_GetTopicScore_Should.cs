using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceScheduler.Entities;
using ConferenceScheduler.Optimizer;

namespace Optimizer.InternalsTest
{

    [TestFixture]
    public class Timeslot_GetTopicScore_Should
    {
        [Test]
        public void ReturnZeroIfThereAreNoConflictsInTheTimeslot()
        {
            // 2 Rooms, 3 timeslots, 6 Sessions

            var sessions = new SessionsCollection();
            sessions.Add(1, 1, Presenter.Create(1));
            sessions.Add(2, 2, Presenter.Create(2));
            sessions.Add(3, 3, Presenter.Create(2));
            sessions.Add(4, 4, Presenter.Create(3));
            sessions.Add(5, 2, Presenter.Create(3));
            sessions.Add(6, 6, Presenter.Create(3));

            var assignments = new List<Assignment>();
            assignments.Add(new Assignment(1, 1, 1));
            assignments.Add(new Assignment(1, 2, 2));
            assignments.Add(new Assignment(1, 3, 3));
            assignments.Add(new Assignment(2, 1, 4));
            assignments.Add(new Assignment(2, 2, 5));
            assignments.Add(new Assignment(2, 3, 6));

            var target = new Timeslot() { Id = 1, DayIndex = 0, StartHour = 10.0F };
            var result = target.GetTopicScore(assignments, sessions);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void ReturnOneIfThereIsOneConflictInTheTimeslot()
        {
            // 2 Rooms, 3 timeslots, 6 Sessions

            var sessions = new SessionsCollection();
            sessions.Add(1, 1, Presenter.Create(1));
            sessions.Add(2, 2, Presenter.Create(2));
            sessions.Add(3, 3, Presenter.Create(2));
            sessions.Add(4, 4, Presenter.Create(3));
            sessions.Add(5, 2, Presenter.Create(3));
            sessions.Add(6, 6, Presenter.Create(3));

            var assignments = new List<Assignment>();
            assignments.Add(new Assignment(1, 1, 1));
            assignments.Add(new Assignment(1, 2, 2));
            assignments.Add(new Assignment(1, 3, 3));
            assignments.Add(new Assignment(2, 1, 4));
            assignments.Add(new Assignment(2, 2, 5));
            assignments.Add(new Assignment(2, 3, 6));

            var target = new Timeslot() { Id = 2, DayIndex = 0, StartHour = 11.0F };
            var result = target.GetTopicScore(assignments, sessions);
            Assert.AreEqual(1, result);
        }

        [Test]
        public void ReturnThreeIfThereAreTwoConflictsInTheTimeslot()
        {
            // 3 Rooms, 2 timeslots, 6 Sessions

            var sessions = new SessionsCollection();
            sessions.Add(1, 1, Presenter.Create(1));
            sessions.Add(2, 1, Presenter.Create(2));
            sessions.Add(3, 1, Presenter.Create(2));
            sessions.Add(4, 2, Presenter.Create(3));
            sessions.Add(5, 3, Presenter.Create(3));
            sessions.Add(6, 4, Presenter.Create(3));

            var assignments = new List<Assignment>();
            assignments.Add(new Assignment(1, 1, 1));
            assignments.Add(new Assignment(1, 1, 2));
            assignments.Add(new Assignment(2, 1, 3));
            assignments.Add(new Assignment(2, 2, 4));
            assignments.Add(new Assignment(3, 2, 5));
            assignments.Add(new Assignment(3, 2, 6));

            var target = new Timeslot() { Id = 1, DayIndex = 0, StartHour = 11.0F };
            var result = target.GetTopicScore(assignments, sessions);
            Assert.AreEqual(3, result);
        }

        [Test]
        public void ReturnSevenIfThereAreThreeConflictsInTheTimeslot()
        {
            // 4 Rooms, 2 timeslots, 6 Sessions

            var sessions = new SessionsCollection();
            sessions.Add(1, 1, Presenter.Create(1));
            sessions.Add(2, 1, Presenter.Create(2));
            sessions.Add(3, 1, Presenter.Create(2));
            sessions.Add(4, 1, Presenter.Create(3));
            sessions.Add(5, 3, Presenter.Create(3));
            sessions.Add(6, 4, Presenter.Create(3));

            var assignments = new List<Assignment>();
            assignments.Add(new Assignment(1, 1, 1));
            assignments.Add(new Assignment(2, 1, 2));
            assignments.Add(new Assignment(3, 1, 3));
            assignments.Add(new Assignment(4, 1, 4));
            assignments.Add(new Assignment(1, 2, 5));
            assignments.Add(new Assignment(2, 2, 6));

            var target = new Timeslot() { Id = 1, DayIndex = 0, StartHour = 11.0F };
            var result = target.GetTopicScore(assignments, sessions);
            Assert.AreEqual(7, result);
        }
    }
}
