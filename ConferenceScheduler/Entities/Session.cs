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
        /// The primary-key identifier of the Topic (Track) of the session
        /// </summary>
        public int? TopicId { get; set; }

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
        /// Recursively searches the dependency tree to see if the
        /// current session is dependent on the specified session.
        /// </summary>
        /// <param name="dependencySessionId">The ID of the session 
        /// that we want to determine whether or not it is in 
        /// the dependency chain of the current session.</param>
        /// <returns>A boolean indicating whether or not the current
        /// session is dependent on the specified session.</returns>
        public bool IsDependentUpon(int dependencySessionId)
        {
            bool result = false;
            var i = 0;

            if ((this != null) && (this.Dependencies != null))
            {
                var dependencyArray = this.Dependencies.ToArray();
                while (i < this.Dependencies.Count())
                {
                    var dependency = dependencyArray[i];
                    if (dependency.Id == dependencySessionId || dependency.IsDependentUpon(dependencySessionId))
                        result = true;
                    i++;
                }
            }

            return result;
        }

        // The following methods are specific to the local (non-Gurobi)
        // optimizer and should probably be made into extension 
        // methods of the Session or Sessions objects in that library

        /// <summary>
        /// Add a dependent session to the list of dependencies for this session
        /// </summary>
        /// <param name="session2"></param>
        public void AddDependency(Session session2)
        {
            _dependentSessions.Add(session2);
        }

        // TODO: Document
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasDependencies()
        {
            return (this.Dependencies != null && this.Dependencies.Count() > 0);
        }

        // TODO: Document
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessions"></param>
        /// <returns></returns>
        public bool HasDependents(IEnumerable<Session> sessions)
        {
            return (this.GetDependents(sessions).Count() > 0);
        }

        // TODO: Document
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public int GetDependencyCount()
        {
            int result = 0;
            if (this.Dependencies != null)
                result = this.Dependencies.Count();
            return result;
        }

        // TODO: Document
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessions"></param>
        /// <returns></returns>
        public int GetDependentCount(IEnumerable<Session> sessions)
        {
            return sessions.Where(s => s.IsDependentUpon(this.Id)).Count();
        }

        // Returns the depth of the dependent chain. That is, how many sessions
        // depend on this session that are also depended on by other sessions.
        // TODO: Document properly
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessions"></param>
        /// <returns></returns>
        public int GetDependentDepth(IEnumerable<Session> sessions)
        {
            int result = 0;
            if (this.HasDependents(sessions))
                foreach (var dependency in this.GetDependents(sessions))
                    result = System.Math.Max(result, dependency.GetDependentDepth(sessions) + 1);
            return result;
        }

        // Returns the depth of the dependency chain. That is, how many sessions
        // this session is dependent upon that are also dependent on other sessions.
        // TODO: Document
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessions"></param>
        /// <param name="roomCount"></param>
        /// <returns></returns>
        public int GetDependencyDepth(IEnumerable<Session> sessions, int roomCount)
        {
            int result = 0;
            if (this.HasDependencies())
                foreach (var dependency in this.Dependencies)
                    result = System.Math.Max(result, dependency.GetDependencyDepth(sessions, roomCount) + 1);

            double dependencyCountF = Convert.ToDouble(this.GetDependencyCount());
            double roomCountF = Convert.ToDouble(roomCount);
            int timeslotsPerDependency = Convert.ToInt32(System.Math.Ceiling(dependencyCountF / roomCountF));
            result = System.Math.Max(result, timeslotsPerDependency);

            return result;
        }

        // TODO: Document
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessions"></param>
        /// <returns></returns>
        public IEnumerable<Session> GetDependents(IEnumerable<Session> sessions)
        {
            return sessions.Where(s => s.Dependencies != null && s.Dependencies.Count(d => d.Id == this.Id) > 0);
        }

    }
}
