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

        internal SessionAvailability(int timeslotId, bool isFirstTimeslot, int roomId, IEnumerable<Session> sessions)
        {
            this.TimeslotId = timeslotId;
            this.RoomId = roomId;
            this.Assigned = false;

            this.AvailableSessionIds = sessions.GetSessionIds().ToList();
            foreach (var session in sessions)
            {
                // A session with dependencies cannot be in the 1st timeslot
                if (isFirstTimeslot && session.HasDependencies())
                    this.AvailableSessionIds.Remove(session.Id);

                foreach (var presenter in session.Presenters)
                {
                    if (presenter.IsUnavailableInTimeslot(timeslotId))
                        this.AvailableSessionIds.Remove(session.Id);
                }
            }
        }

    }
}
