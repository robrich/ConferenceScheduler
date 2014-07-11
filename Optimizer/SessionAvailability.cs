using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceScheduler.Entities;

namespace ConferenceScheduler.Optimizer
{
    internal class SessionAvailability
    {
        internal int TimeslotId { get; set; }

        internal int RoomId { get; set; }

        internal bool Assigned { get; set; }

        internal ICollection<int> AvailableSessionIds { get; private set; }

        internal int AvailableSessionCount { get { return this.AvailableSessionIds.Count(); } }

        internal SessionAvailability(int timeslotId, int timeslotIndex, int lastTimeslotIndex, int roomId, IEnumerable<Session> sessions)
        {
            Load(timeslotId, timeslotIndex, lastTimeslotIndex, roomId, sessions);
        }

        private void Load(int timeslotId, int timeslotIndex, int lastTimeslotIndex, int roomId, IEnumerable<Session> sessions)
        {
            this.TimeslotId = timeslotId;
            this.RoomId = roomId;
            this.Assigned = false;

            this.AvailableSessionIds = sessions.GetSessionIds().ToList();
            foreach (var session in sessions)
            {
                var dependencyDepth = session.GetDependencyDepth(sessions);
                var dependentDepth = session.GetDependentDepth(sessions);

                // A session with dependencies cannot be in any of the 1st n timeslots
                // where n is the depth of the dependency chain
                if (timeslotIndex < dependencyDepth)
                {
                    this.AvailableSessionIds.Remove(session.Id);
                }

                // A session that is another session's dependency cannot be in the last n timeslots
                // where n is the depth of the dependent chain
                if (timeslotIndex > lastTimeslotIndex - dependentDepth)
                {
                    this.AvailableSessionIds.Remove(session.Id);
                }

                // If the presenter is not available in the timeslot
                // the session cannot be given in that slot
                foreach (var presenter in session.Presenters)
                {
                    if (presenter.IsUnavailableInTimeslot(timeslotId))
                        this.AvailableSessionIds.Remove(session.Id);
                }
            }
        }

    }
}
