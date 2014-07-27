using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConferenceScheduler.Entities;

namespace ConferenceScheduler.Optimizer
{
    /// <summary>
    /// Holds methods used to perform optimizations to determine conference schedules.
    /// </summary>
    public class Engine
    {
        Action<ProcessUpdateEventArgs> _updateEventHandler;

        /// <summary>
        /// Create an instance of the object
        /// </summary>
        public Engine() : this(null) { }

        /// <summary>
        /// Create an instance of the object
        /// </summary>
        /// <param name="updateEventHandler">A method to call to handle an update event.</param>
        public Engine(Action<ProcessUpdateEventArgs> updateEventHandler)
        {
            _updateEventHandler = updateEventHandler;
        }

        /// <summary>
        /// Returns an optimized conference schedule based on the inputs.
        /// </summary>
        /// <param name="sessions">A list of sessions and their associated attributes.</param>
        /// <param name="rooms">A list of rooms that sessions can be held in along with their associated attributes.</param>
        /// <param name="timeslots">A list of time slots during which sessions can be delivered.</param>
        /// <returns>A collection of assignments representing the room and timeslot in which each session will be delivered.</returns>
        public IEnumerable<Assignment> Process(IEnumerable<Session> sessions, IEnumerable<Room> rooms, IEnumerable<Timeslot> timeslots)
        {
            var solution = new Solution(sessions, rooms, timeslots);

            while (solution.AssignmentsCompleted < sessions.Count())
            {
                if (solution.AssignSessionsWithOnlyOneOption() > 0)
                    RaiseUpdateEvent(solution);

                if (solution.AssignMostConstrainedSession() > 0)
                    RaiseUpdateEvent(solution);
            }

            var bestSolution = solution;
            var bestScore = bestSolution.GetScore();

            if (!bestSolution.IsFeasible)
                throw new Exceptions.NoFeasibleSolutionsException();
            else
            {
                var maxAttempts = Convert.ToInt32(System.Math.Pow(2.0, Convert.ToDouble(solution.Assignments.Count() - 1)));
                var attemptCount = 0;

                do
                {
                    // Try to improve the score
                    solution = bestSolution.SwapAssignments();
                    attemptCount++;
                    var currentScore = solution.GetScore();
                    if ((solution.IsFeasible) && (currentScore < bestScore))
                    {
                        bestSolution = solution;
                        RaiseUpdateEvent(bestSolution);
                    }
                } while (attemptCount < maxAttempts);

            }

            return bestSolution.Results;
        }

        private void RaiseUpdateEvent(Solution solution)
        {
            if (_updateEventHandler != null)
            {
                var args = new ProcessUpdateEventArgs() { Assignments = solution.Assignments.ToList() };
                _updateEventHandler.Invoke(args);
            }
        }

    }
}
