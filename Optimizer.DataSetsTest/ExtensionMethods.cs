using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConferenceScheduler.Interfaces;
using ConferenceScheduler.Entities;

namespace Optimizer.DataSetsTest
{
    public static class ExtensionMethods
    {
        public static IConferenceOptimizer Create(this IConferenceOptimizer ignore)
        {
            return ignore.Create(new EventHandler());
        }

        public static ConferenceScheduler.Interfaces.IConferenceOptimizer Create(this IConferenceOptimizer ignore, EventHandler eventHandlers)
        {
            // return new Engine(eventHandlers.EngineUpdateEventHandler);
            // return new Gurobi.Engine(eventHandlers.EngineUpdateEventHandler);
            return new ConferenceScheduler.Optimizer.Glop.Engine(eventHandlers.EngineUpdateEventHandler);
        }

        public static void WriteSchedule(this IEnumerable<Assignment> assignments)
        {

        }

        public static void WriteSchedule(this IEnumerable<Assignment> assignments, IEnumerable<Session> sessions, IDictionary<int, string> names)
        {
            var timeslots = assignments.Select(a => a.TimeslotId).Distinct().OrderBy(a => a);
            var rooms = assignments.Select(a => a.RoomId).Distinct().OrderBy(a => a);

            var result = new StringBuilder();

            result.Append("R\\T\t|\t");

            foreach (var timeslot in timeslots)
                result.Append($"{timeslot.ToString().PadRight(15, ' ')}\t");

            result.AppendLine();
            result.AppendLine("---------------------------------------------------------------------------");

            foreach (var room in rooms)
            {
                result.Append($"{room}\t|\t");
                foreach (var timeslot in timeslots)
                {
                    if (assignments.Count(a => a.RoomId == room && a.TimeslotId == timeslot) > 1)
                        throw new ArgumentException($"Multiple assignments to room {room} and timeslot {timeslot}.");
                    else
                    {
                        var session = assignments.Where(a => a.RoomId == room && a.TimeslotId == timeslot).SingleOrDefault();
                        if (session == null)
                            result.Append("\t".PadLeft(15, ' '));
                        else
                        {
                            string name;
                            if (names == null)
                                name = session.SessionId.ToString().PadRight(15);
                            else
                            {
                                var fullName = names.Single(n => n.Key == session.SessionId).Value;
                                var currentSession = sessions?.Single(s => s.Id == session.SessionId);
                                name = $"{fullName.Substring(0, Math.Min(fullName.Length, 10)).PadRight(10)} ({ (currentSession == null ? session.SessionId.Value : currentSession.TopicId)})";
                            }
                            result.Append($"{name}\t");
                        }
                    }
                }
                result.AppendLine();
            }

            Console.WriteLine(result.ToString());
        }


    }
}
