using ConferenceScheduler.Entities;
using ConferenceScheduler.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceScheduler.Extensions
{
    // TODO: Document
    /// <summary>
    /// 
    /// </summary>
    public static class SessionsExtensions
    {
        // TODO: Document
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessions"></param>
        public static void Validate(this IEnumerable<Entities.Session> sessions)
        {
            if (sessions == null)
                throw new ArgumentNullException(nameof(sessions));

            if (sessions.Count() == 0)
                throw new ArgumentException("You must have at least one session");

            if (sessions.Select(s => s.Id).Distinct().Count() != sessions.Count())
                throw new ArgumentException("Session Ids must be unique");

            if (sessions.Count(s => s.Presenters == null || s.Presenters.Count() < 1) > 0)
                throw new ArgumentException("Every session must have at least one presenter.");

            var presentations = new SessionsCollection(sessions);
            if (presentations.HaveCircularDependencies())
                throw new DependencyException("Sessions may not have circular dependencies.");

            var presenters = sessions.SelectMany(s => s.Presenters);
            var presenterIds = presenters.Select(p => p.Id).Distinct();

            foreach (var presenterId in presenterIds)
            {
                if (presenters.Count(p => p.Id == presenterId) > 1)
                {
                    var baseAvailability = presenters.First().UnavailableForTimeslots;
                    foreach (var presenter in presenters.Where(p => p.Id == presenterId))
                    {
                        if (!presenter.UnavailableForTimeslots.Matches(baseAvailability))
                            throw new ArgumentException("A presenters availability must be consistant across all instances");
                    }
                }
            }

        }


        // TODO: Document
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessions"></param>
        /// <param name="rooms"></param>
        /// <param name="timeslots"></param>
        public static void ValidateAgainstRoomsAndTimeslots(this IEnumerable<Session> sessions, IEnumerable<Room> rooms, IEnumerable<Timeslot> timeslots)
        {
            var roomSlotCombinations = (rooms.Count() * timeslots.Count()) - rooms.Sum(r => r.UnavailableForTimeslots.Count());
            if (roomSlotCombinations < sessions.Count())
                throw new ArgumentException($"You must have at least as many room-timeslot combinations ({roomSlotCombinations}) as you have sessions ({sessions.Count()}).");

        }
    }
}
