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
        private List<Session> _dependentSessions;

        /// <summary>
        /// The primary-key identifier of the object
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Contains the list of presenters for the session
        /// </summary>
        public IEnumerable<Presenter> Presenters { get; set; }

        /// <summary>
        /// Contains the list of sessions that this session is dependent upon.
        /// All dependencies must happen prior to this session.
        /// </summary>
        public IEnumerable<Session> Dependencies 
        {
            get { return _dependentSessions; } 
        }

        /// <summary>
        /// Create an instance of the object
        /// </summary>
        public Session()
        {
            _dependentSessions = new List<Session>();
        }

        /// <summary>
        /// Add a dependent session to the list of dependencies for this session
        /// </summary>
        /// <param name="session2"></param>
        public void AddDependency(Session session2)
        {
            _dependentSessions.Add(session2);
        }

    }
}
