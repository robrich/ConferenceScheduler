using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceScheduler.Entities;

namespace ConferenceScheduler.Optimizer
{
    internal static class AssignmentExtensions
    {
        internal static int CompletedAssignmentCount(this IEnumerable<Assignment> assignments)
        {
            return assignments.Count(a => a.SessionId.HasValue);
        }

        internal static Session GetUnassignedSessionWithFewestAvailableOptions(this IEnumerable<Assignment> assignments, IEnumerable<Session> sessions, SessionAvailabilityCollection sessionMatrix)
        {
            Session result = null;
            var assignedSessionIds = assignments.Where(a => a.SessionId != null).Select(a => a.SessionId);
            
            var sessionDictionary = new Dictionary<int, int>();
            foreach (var session in sessions.Where(s => !assignedSessionIds.Contains(s.Id)))
            {
                sessionDictionary.Add(session.Id, sessionMatrix.GetAvailableAssignmentCount(session.Id));
            }

            if (sessionDictionary.Count() > 0)
            {
                var min = sessionDictionary.Min(s => s.Value);
                var keys = sessionDictionary.Where(sd => sd.Value == min).Select(a => a.Key);
                result = sessions.Where(a => keys.Contains(a.Id)).OrderByDescending(b => b.GetDependentDepth(sessions)).ThenByDescending(c => c.GetDependentCount(sessions)).FirstOrDefault();
            }

            return result;
        }

        internal static Assignment GetAssignment(this IEnumerable<Assignment> assignments, int roomId, int timeslotId)
        {
            return assignments.Where(a => a.RoomId == roomId && a.TimeslotId == timeslotId).SingleOrDefault();
        }

        internal static Assignment GetAssignment(this IEnumerable<Assignment> assignments, int sessionId)
        {
            return assignments.Where(a => a.SessionId.HasValue && a.SessionId.Value == sessionId).SingleOrDefault();
        }

        internal static IEnumerable<Assignment> GetAssignmentsInTimeslot(this IEnumerable<Assignment> assignments, int timeslotId)
        {
            return assignments.Where(a => a.TimeslotId == timeslotId && a.SessionId.HasValue);
        }

    }
}
