﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceScheduler.Entities;

namespace ConferenceScheduler.Optimizer
{
    internal class SessionAvailabilityCollection : List<SessionAvailability>
    {
        IEnumerable<Session> _sessions;
        IOrderedEnumerable<Timeslot> _orderedTimeslots;

        internal SessionAvailabilityCollection(IEnumerable<Entities.Session> sessions, IEnumerable<Entities.Room> rooms, IEnumerable<Entities.Timeslot> timeslots)
        {
            Load(sessions, rooms, timeslots);
        }

        internal SessionAvailabilityCollection(SessionAvailabilityCollection sac)
        {
            Load(sac);
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
            _sessions = sessions;
            _orderedTimeslots = timeslots.Sort();
            var lastTimeslotIndex = _orderedTimeslots.IndexOf(_orderedTimeslots.Last());

            foreach (var room in rooms)
                foreach (var timeslot in timeslots)
                {
                    if (room.AvailableInTimeslot(timeslot.Id))
                    {
                        var timeslotIndex = _orderedTimeslots.IndexOf(timeslot);
                        this.Add(new SessionAvailability(timeslot.Id, timeslotIndex, lastTimeslotIndex, room.Id, rooms.Count(), sessions));
                    }
                }
        }

        private void Load(SessionAvailabilityCollection sac)
        {
            _sessions = sac._sessions.ToList();
            IEnumerable<Timeslot> ts = sac._orderedTimeslots.ToList();
            _orderedTimeslots = ts.Sort();
            this.AddRange(sac.ToList());
        }

        internal IEnumerable<SessionAvailability> GetUnassignedItemsWithOnlyOneOption()
        {
            return this.Where(sa => !sa.Assigned && sa.AvailableSessionIds.Count() == 1);
        }

        internal void UpdateConstraints(Assignment assignment, Session session)
        {
            // Remove this session from the available list for all room-timeslot combinations
            var items = this.Where(i => i.AvailableSessionIds.Contains(session.Id));
            foreach (var item in items)
            {
                item.AvailableSessionIds.Remove(session.Id);
                if (item.RoomId == assignment.RoomId && item.TimeslotId == assignment.TimeslotId)
                    item.Assigned = true;
            }

            int thisTimeslotIndex = _orderedTimeslots.IndexOf(assignment.TimeslotId);

            // If Session has dependencies, eliminate this or earlier timeslots for those sessions
            if (session.HasDependencies())
            {
                foreach (var dependency in session.Dependencies)
                {
                    var possibleAssignments = this.Where(i => i.AvailableSessionIds.Contains(dependency.Id));
                    foreach (var possibleAssignment in possibleAssignments)
                    {
                        if (_orderedTimeslots.IndexOf(possibleAssignment.TimeslotId) >= thisTimeslotIndex)
                            possibleAssignment.AvailableSessionIds.Remove(dependency.Id);
                    }
                }
            }

            // If Session is dependent on other sessions, eliminate this or later timeslots for those sessions
            if (session.HasDependents(_sessions))
            {
                foreach (var dependentSession in session.GetDependents(_sessions))
                {
                    var possibleAssignments = this.Where(i => i.AvailableSessionIds.Contains(dependentSession.Id));
                    foreach (var possibleAssignment in possibleAssignments)
                    {
                        if (_orderedTimeslots.IndexOf(possibleAssignment.TimeslotId) <= thisTimeslotIndex)
                            possibleAssignment.AvailableSessionIds.Remove(dependentSession.Id);
                    }
                }
            }

        }

        internal int GetAvailableAssignmentCount(int sessionId)
        {
            return this.Count(a => a.AvailableSessionIds.Contains(sessionId));
        }

        internal IEnumerable<int> GetAvailableTimeslotIds(int sessionId)
        {
            return this.Where(a => a.AvailableSessionIds.Contains(sessionId)).Select(s => s.TimeslotId).Distinct();
        }

        internal SessionAvailabilityCollection Clone()
        {
            return new SessionAvailabilityCollection(this);
        }
    }
}
