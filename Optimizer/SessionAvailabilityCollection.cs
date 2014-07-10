using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceScheduler.Entities;

namespace ConferenceScheduler.Optimizer
{
    internal class SessionAvailabilityCollection : List<SessionAvailability>
    {
        internal SessionAvailabilityCollection(IEnumerable<Entities.Session> sessions, IEnumerable<Entities.Room> rooms, IEnumerable<Entities.Timeslot> timeslots)
        {
            Load(sessions, rooms, timeslots);
        }

        internal bool IsFeasible
        {
            get
            {
                return (this.Where(a => !a.Assigned && a.AvailableSessionCount == 0).Count() == 0);
            }
        }

        private void Load(IEnumerable<Entities.Session> sessions, IEnumerable<Entities.Room> rooms, IEnumerable<Entities.Timeslot> timeslots)
        {
            var firstTimeslotId = timeslots.GetFirstTimeslotId();
            foreach (var room in rooms)
                foreach (var timeslot in timeslots)
                {
                    if (room.AvailableInTimeslot(timeslot.Id))
                    {
                        var isFirstTimeslot = timeslot.Id == firstTimeslotId;
                        this.Add(new SessionAvailability(timeslot.Id, isFirstTimeslot, room.Id, sessions));
                    }
                }
        }

        internal IEnumerable<SessionAvailability> GetUnassignedItemsWithOnlyOneOption()
        {
            return this.Where(sa => !sa.Assigned && sa.AvailableSessionIds.Count() == 1);
        }

        internal void RemoveAssignedSessions(Assignment assignment, int sessionId)
        {
            var items = this.Where(i => i.AvailableSessionIds.Contains(sessionId));
            foreach (var item in items)
            {
                item.AvailableSessionIds.Remove(sessionId);
                if (item.RoomId == assignment.RoomId && item.TimeslotId == assignment.TimeslotId)
                    item.Assigned = true;
            }  
        }
    }
}
