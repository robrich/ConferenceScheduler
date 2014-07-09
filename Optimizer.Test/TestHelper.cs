using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceScheduler.Entities;

namespace ConferenceScheduler.Optimizer.Test
{
    internal class TestHelper
    {

        internal static Presenter CreatePresenter(int id, params int[] presenterUnavailableForTimeslots)
        {
            return new Presenter()
            {
                Id = id,
                UnavailableForTimeslots = presenterUnavailableForTimeslots.AsEnumerable()
            };
        }

        internal static Room CreateRoom(int id, int capacity)
        {
            return new Room()
            {
                Id = id,
                Capacity = capacity
            };
        }

        internal static Room CreateRoom(int id, int capacity, params int[] roomUnavailableForTimeslots)
        {
            return new Room()
            {
                Id = id,
                Capacity = capacity,
                UnavailableForTimeslots = roomUnavailableForTimeslots.ToList()
            };
        }


        internal static Timeslot CreateTimeslot(int id)
        {
            return CreateTimeslot(id, 0);
        }

        internal static Timeslot CreateTimeslot(int id, Single startHour)
        {
            return new Timeslot()
            {
                Id = id,
                StartHour = startHour
            };
        }


    }
}
