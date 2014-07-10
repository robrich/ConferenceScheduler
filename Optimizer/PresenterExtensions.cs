using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceScheduler.Entities;

namespace ConferenceScheduler.Optimizer
{
    internal static class PresenterExtensions
    {
        internal static bool IsUnavailableInTimeslot(this Presenter presenter, int timeslotId)
        {
            bool result = true;
            if (presenter.UnavailableForTimeslots != null)
                result = presenter.UnavailableForTimeslots.Contains(timeslotId);
            return result;
        }
    }
}
