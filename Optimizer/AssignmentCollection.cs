using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceScheduler.Entities;

namespace ConferenceScheduler.Optimizer
{
    internal class AssignmentCollection: List<Assignment>
    {
        public int AssignmentsCompleted 
        {
            get
            {
                return this.Count(a => a.SessionId.HasValue);
            }
        }

        internal Session GetUnassignedSessionWithFewestAvailableSlots(IEnumerable<Session> sessions, PresenterAvailablityCollection presenterMatrix)
        {
            Session result = null;
            var assignedSessionIds = this.Where(a => a.SessionId != null).Select(a => a.SessionId);
            var sessionDictionary = new Dictionary<int, int>();
            foreach (var session in sessions.Where(s => !assignedSessionIds.Contains(s.Id)))
            {
                sessionDictionary.Add(session.Id, presenterMatrix.GetAvailableTimeslotIds(session.Presenters).Count());
            }

            if (sessionDictionary.Count() > 0)
            {
                var min = sessionDictionary.Min(s => s.Value);
                var key = sessionDictionary.FirstOrDefault(sd => sd.Value == min).Key;
                result = sessions.Where(s => s.Id == key).Single();
            }

            return result;
        }

        internal Assignment GetAssignment(int roomId, int timeslotId)
        {
            return this.Where(a => a.RoomId == roomId && a.TimeslotId == timeslotId).Single();
        }
    }
}
