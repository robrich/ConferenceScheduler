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

        internal ICollection<int> SessionIds { get; private set; }

        internal SessionAvailability(int timeslotId, int roomId, IEnumerable<Session> sessions)
        {
            this.TimeslotId = timeslotId;
            this.RoomId = roomId;
            this.Assigned = false;

            this.SessionIds = new List<int>(sessions.Select(s => s.Id).Distinct());
            foreach (var session in sessions)
            {
                foreach (var presenter in session.Presenters)
                {
                    if (presenter.UnavailableForTimeslots.Contains(timeslotId))
                        this.SessionIds.Remove(session.Id);
                }
            }
        }

    }
}
