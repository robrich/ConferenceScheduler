using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceScheduler.Entities
{
    /// <summary>
    /// Represents a session (presentation) to be delivered at the conference
    /// </summary>
    public class Session
    {
        /// <summary>
        /// The primary-key identifier of the object
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Contains the list of presenters for the session
        /// </summary>
        public IEnumerable<Presenter> Presenters { get; set; }


        // internal int AvailableTimeslotCount { get; set; }
    }
}
