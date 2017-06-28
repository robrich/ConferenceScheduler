using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConferenceScheduler.Entities;

namespace ConferenceScheduler.Optimizer.Test
{
    public static class ExtensionMethods
    {
        public static Interfaces.IConferenceOptimizer Create(this Interfaces.IConferenceOptimizer ignore)
        {
            return ignore.Create(new EventHandler());
        }

        public static Interfaces.IConferenceOptimizer Create(this Interfaces.IConferenceOptimizer ignore, EventHandler eventHandlers)
        {
            // return new Engine(eventHandlers.EngineUpdateEventHandler);
            // return new Gurobi.Engine(eventHandlers.EngineUpdateEventHandler);
            return new Glop.Engine(eventHandlers.EngineUpdateEventHandler);
        }

        public static void Append(this StringBuilder sb, string format, params object[] data)
        {
            sb.Append(string.Format(format, data));
        }

        public static void WriteSchedule(this IEnumerable<Assignment> assignments)
        {
            var timeslots = assignments.Select(a => a.TimeslotId).Distinct().OrderBy(a => a);
            var rooms = assignments.Select(a => a.RoomId).Distinct().OrderBy(a => a);

            var result = new StringBuilder();

            result.Append("R\\T\t|\t");

            foreach (var timeslot in timeslots)
                result.Append("{0}\t", timeslot);

            result.AppendLine();
            result.AppendLine("----------------------------------------------------------------------------------------------");

            foreach (var room in rooms)
            {
                result.Append("{0}\t|\t", room);
                foreach (var timeslot in timeslots)
                {
                    var session = assignments.Where(a => a.RoomId == room && a.TimeslotId == timeslot).SingleOrDefault();
                    if (session == null)
                        result.Append("\t");
                    else
                        result.Append("{0}\t", session.SessionId);
                }
                result.AppendLine();
            }

            Console.WriteLine(result.ToString());
        }

        public static string Serialize(this IEnumerable<Assignment> assignments)
        {
            var timeslots = assignments.Select(a => a.TimeslotId).Distinct().OrderBy(a => a);
            var rooms = assignments.Select(a => a.RoomId).Distinct().OrderBy(a => a);

            var result = new StringBuilder();
            foreach (var room in rooms)
            {
                foreach (var timeslot in timeslots)
                {
                    var session = assignments.Where(a => a.RoomId == room && a.TimeslotId == timeslot).SingleOrDefault();
                    if (session == null)
                        result.Append("  ");
                    else
                        result.Append("{0:00}", session.SessionId);
                }
            }

            return result.ToString();
        }
    }
}
