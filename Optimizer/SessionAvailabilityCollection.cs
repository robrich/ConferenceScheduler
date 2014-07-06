using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceScheduler.Optimizer
{
    internal class SessionAvailabilityCollection: List<SessionAvailability>
    {
        internal SessionAvailabilityCollection(IEnumerable<Entities.Session> sessions, IEnumerable<Entities.Room> rooms, IEnumerable<Entities.Timeslot> timeslots)
        {
            Load(sessions, rooms, timeslots);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        internal bool IsFeasible 
        { 
            get
            {
                return (this.Where(a => a.SessionIds.Count() == 0).Count() == 0);
            }
        }

        private void Load(IEnumerable<Entities.Session> sessions, IEnumerable<Entities.Room> rooms, IEnumerable<Entities.Timeslot> timeslots)
        {
            foreach (var room in rooms)
                foreach (var timeslot in timeslots)
                    this.Add(new SessionAvailability(timeslot.Id, room.Id, sessions));
        }
    }
}
