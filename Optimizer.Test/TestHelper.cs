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

        internal static Session CreateSession(int id, Int16 presenterId, params Int16[] presenterUnavailableForTimeslots)
        {
            return new Session()
            {
                Id = id,
                Presenters = CreatePresenter(presenterId, presenterUnavailableForTimeslots)
            };
        }

        internal static IEnumerable<Presenter> CreatePresenter(Int16 id, Int16[] presenterUnavailableForTimeslots)
        {
            return new List<Presenter>() 
            { 
                new Presenter() 
                    { 
                        Id = id,  
                        UnavailableForTimeslots = presenterUnavailableForTimeslots.AsEnumerable()
                    } 
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

        internal static Timeslot CreateTimeslot(Int16 id)
        {
            return new Timeslot()
            {
                Id = id
            };
        }


    }
}
