using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceScheduler.Entities;

namespace ConferenceScheduler.Optimizer
{
    internal static class SessionExtensions
    {
        internal static IEnumerable<int> GetSessionIds(this IEnumerable<Session> sessions)
        {
            return sessions.Select(s => s.Id).Distinct().ToList();
        }

        internal static bool HasDependencies(this Session session)
        {
            return (session.Dependencies != null && session.Dependencies.Count() > 0);
        }

        internal static bool HasDependents(this Session session, IEnumerable<Session> sessions)
        {
            return (session.GetDependents(sessions).Count() > 0);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal static int GetDependencyCount(this Session session)
        {
            int result = 0;
            if (session.Dependencies != null)
                result = session.Dependencies.Count();
            return result;
        }

        internal static int GetDependentCount(this Session session, IEnumerable<Session> sessions)
        {
            return sessions.Where(s => s.IsDependentUpon(session.Id)).Count();
        }

        // Returns the depth of the dependency chain. That is, how many sessions
        // this session is dependent upon that are also dependent on other sessions.
        internal static int GetDependencyDepth(this Session session, IEnumerable<Session> sessions, int roomCount)
        {
            int result = 0;
            if (session.HasDependencies())
                foreach (var dependency in session.Dependencies)
                    result = System.Math.Max(result, dependency.GetDependencyDepth(sessions, roomCount) + 1);

            double dependencyCountF = Convert.ToDouble(session.GetDependencyCount());
            double roomCountF = Convert.ToDouble(roomCount);
            int timeslotsPerDependency = Convert.ToInt32(System.Math.Ceiling(dependencyCountF / roomCountF));
            result = System.Math.Max(result, timeslotsPerDependency);

            return result;
        }

        // Returns the depth of the dependent chain. That is, how many sessions
        // depend on this session that are also depended on by other sessions.
        internal static int GetDependentDepth(this Session session, IEnumerable<Session> sessions)
        {
            int result = 0;
            if (session.HasDependents(sessions))
                foreach (var dependency in session.GetDependents(sessions))
                    result = System.Math.Max(result, dependency.GetDependentDepth(sessions) + 1);
            return result;
        }


        internal static IEnumerable<Session> GetDependents(this Session session, IEnumerable<Session> sessions)
        {
            return sessions.Where(s => s.Dependencies != null && s.Dependencies.Count(d => d.Id == session.Id) > 0);
        }

        internal static IEnumerable<Presenter> GetPresenters(this IEnumerable<Session> sessions)
        {
            return sessions.SelectMany(s => s.Presenters).Distinct();
        }

        internal static bool HaveCircularDependencies(this IEnumerable<Session> sessions)
        {
            bool result = false;
            int i = 0;
            var sessionsArray = sessions.ToArray();

            while (!result && (i < sessions.Count()))
            {
                var session = sessionsArray[i];
                if (session.IsDependentUpon(session.Id))
                    result = true;
                i++;
            }

            return result;
        }

        private static bool IsDependentUpon(this Session session, int dependencySessionId)
        {
            bool result = false;
            var i = 0;

            if ((session != null) && (session.Dependencies != null))
            {
                var dependencyArray = session.Dependencies.ToArray();
                while (i < session.Dependencies.Count())
                {
                    var dependency = dependencyArray[i];
                    if (dependency.Id == dependencySessionId || dependency.IsDependentUpon(dependencySessionId))
                        result = true;
                    i++;
                }
            }

            return result;
        }

    }
}
