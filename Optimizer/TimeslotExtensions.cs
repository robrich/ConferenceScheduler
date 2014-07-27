using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceScheduler.Entities;

namespace ConferenceScheduler.Optimizer
{
    internal static class TimeslotExtensions
    {
        internal static int IndexOf(this IOrderedEnumerable<Timeslot> timeslots, Timeslot timeslot)
        {
            return Array.IndexOf(timeslots.ToArray(), timeslot);
        }

        internal static int IndexOf(this IOrderedEnumerable<Timeslot> timeslots, int timeslotId)
        {
            var timeslot = timeslots.Where(t => t.Id == timeslotId).Single();
            return timeslots.IndexOf(timeslot);
        }

        internal static IOrderedEnumerable<Timeslot> Sort(this IEnumerable<Timeslot> timeslots)
        {
            return timeslots.OrderBy(t => (t.DayIndex * 24.0) + t.StartHour);
        }

        internal static int GetTopicScore(this Timeslot timeslot, IEnumerable<Assignment> assignments, IEnumerable<Session> sessions)
        {
            var result = 0;
            var assignmentsForSlot = assignments.Where(a => a.TimeslotId == timeslot.Id && a.SessionId.HasValue).ToList();
            var topicIds = assignmentsForSlot.Select(a => sessions.Where(s => s.Id == a.SessionId).Single().TopicId).Distinct().ToList();
            foreach (var topicId in topicIds)
            {
                if (topicId != null)
                {
                    var sessionIdsInTopic = sessions.Where(s => s.TopicId == topicId).Select(s => s.Id).ToList();
                    var sessionCount = assignmentsForSlot.Where(a => sessionIdsInTopic.Contains(a.SessionId.Value)).Count();
                    result += Convert.ToInt32(System.Math.Pow(2.0, Convert.ToDouble(sessionCount - 1)) - 1);
                }
            }

            return result;
        }

    }
}

