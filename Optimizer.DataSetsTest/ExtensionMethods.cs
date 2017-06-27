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
            var timeslots = assignments.Select(a => a.TimeslotId).Distinct().OrderBy(a => a);
            var rooms = assignments.Select(a => a.RoomId).Distinct().OrderBy(a => a);

            var result = new StringBuilder();

            result.Append("R\\T\t|\t");

            foreach (var timeslot in timeslots)
                result.Append($"{timeslot}\t");

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
                            result.Append("\t");
                        else
                            result.Append($"{session.SessionId}\t");
                    }
                }
                result.AppendLine();
            }

            Console.WriteLine(result.ToString());
        }


    }
}
