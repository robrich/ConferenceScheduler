using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConferenceScheduler.Optimizer
{
    internal class PresenterAvailablityCollection: List<PresenterAvailability>
    {
        ICollection<int> _presenterIds;
        ICollection<int> _timeslotIds;

        public PresenterAvailablityCollection(IEnumerable<Entities.Presenter> presenters, IEnumerable<int> timeslotIds)
        {
            Load(presenters, timeslotIds);
        }

        public bool IsFeasible
        {
            get { return GetFeasibility(); }
        }

        private void Load(IEnumerable<Entities.Presenter> presenters, IEnumerable<int> timeslotIds)
        {
            _presenterIds = new List<int>();
            _timeslotIds = new List<int>();
            foreach (var presenter in presenters)
            {
                _presenterIds.Add(presenter.Id);
                foreach (var timeslotId in timeslotIds)
                {
                    _timeslotIds.Add(timeslotId);
                    var available = !(presenter.UnavailableForTimeslots != null && presenter.UnavailableForTimeslots.Contains(timeslotId));
                    this.Add(new PresenterAvailability() 
                    { 
                        PresenterId = presenter.Id,
                        TimeslotId = timeslotId,
                        IsAvailable = available
                    });
                }
            }
        }

        private bool GetFeasibility()
        {
            bool result = true;
            foreach (var presenterId in _presenterIds)
            {
                if (this.Count(a => a.PresenterId == presenterId && a.IsAvailable) == 0)
                    result = false;
            }
            return result;
        }
    }
}
