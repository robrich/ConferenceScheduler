using ConferenceScheduler.Entities;
using ConferenceScheduler.Exceptions;
using ConferenceScheduler.Extensions;
using Google.OrTools.LinearSolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceScheduler.Optimizer.Glop
{
    public class Engine : ConferenceScheduler.Interfaces.IConferenceOptimizer
    {
        Action<ProcessUpdateEventArgs> _updateEventHandler;

        Solver _model;
        Variable[,,] _v;
        // Variable[] _s;

        int[] _timeslotIds;
        int[] _sessionIds;
        int[] _roomIds;

        /// <summary>
        /// Create an instance of the object
        /// </summary>
        /// <param name="updateEventHandler">A method to call to handle an update event.</param>
        public Engine(Action<ProcessUpdateEventArgs> updateEventHandler)
        {
            _updateEventHandler = updateEventHandler;
            _model = CreateMixedIntegerProgrammingSolver();
            // _model = CreateLinearProgrammingSolver();
        }

        public IEnumerable<Assignment> Process(IEnumerable<Session> sessions, IEnumerable<Room> rooms, IEnumerable<Timeslot> timeslots)
        {
            Validate(sessions, rooms, timeslots);

            _timeslotIds = new int[timeslots.Count()];
            int index = 0;
            foreach (var timeslot in timeslots)
            {
                _timeslotIds[index] = timeslot.Id;
                index++;
            }

            _sessionIds = new int[sessions.Count()];
            index = 0;
            foreach (var session in sessions)
            {
                _sessionIds[index] = session.Id;
                index++;
            }

            _roomIds = new int[rooms.Count()];
            index = 0;
            foreach (var room in rooms)
            {
                _roomIds[index] = room.Id;
                index++;
            }

            CreateVariables(sessions.Count(), rooms.Count(), timeslots.Count());
            CreateConstraints(sessions, rooms, timeslots.Count());

            int status = _model.Solve();
            if (status != Solver.OPTIMAL)
                throw new NoFeasibleSolutionsException();

            // var v = _model.Get(GRB.DoubleAttr.X, _v);
            //var p = _model.Get(GRB.DoubleAttr.X, _s);

            //for (int i = 0; i < sessions.Count(); i++)
            //    Console.WriteLine($"s[{i}] = {p[i]}");

            var results = new List<Assignment>();
            for (int s = 0; s < sessions.Count(); s++)
                for (int r = 0; r < rooms.Count(); r++)
                    for (int t = 0; t < timeslots.Count(); t++)
                    {
                        if (_v[s, r, t].SolutionValue() == 1.0)
                            results.Add(new Assignment(_roomIds[r], _timeslotIds[t], _sessionIds[s]));
                    }

            return results;
        }

        private void CreateVariables(int sessionCount, int roomCount, int timeslotCount)
        {
            _v = new Variable[sessionCount, roomCount, timeslotCount];
            for (int s = 0; s < sessionCount; s++)
                for (int r = 0; r < roomCount; r++)
                    for (int t = 0; t < timeslotCount; t++)
                    {
                        _v[s, r, t] = _model.MakeBoolVar($"x[{s},{r},{t}]");
                        Console.WriteLine($"Variable: x[{s},{r},{t}]");
                    }

            //_s = new Variable[sessionCount];
            //for (int s = 0; s < sessionCount; s++)
            //{
            //    _s[s] = _model.MakeIntVar(0.0, Convert.ToDouble(timeslotCount), $"s[{s}]");
            //    Console.WriteLine($"Variable: s[{s}]");
            //}
        }

        private void CreateConstraints(IEnumerable<Session> sessions, IEnumerable<Room> rooms, int timeslotCount)
        {
            int sessionCount = sessions.Count();
            int roomCount = rooms.Count();

            // Each room can have no more than 1 session per timeslot
            for (int r = 0; r < roomCount; r++)
                for (int t = 0; t < timeslotCount; t++)
                {
                    Constraint expr = _model.MakeConstraint(0, 1, $"x[*,{r},{t}]_LessEqual_1");
                    for (int s = 0; s < sessionCount; s++)
                        expr.SetCoefficient(_v[s, r, t], 1);

                    Console.WriteLine($"x[*,{r},{t}]_LessEqual_1");
                }

            // Each session must be assigned to exactly 1 room/timeslot combination
            for (int s = 0; s < sessionCount; s++)
            {
                Constraint expr = _model.MakeConstraint(1.0, 1.0, $"x[{s},*,*]_Equals_1");
                for (int r = 0; r < roomCount; r++)
                    for (int t = 0; t < timeslotCount; t++)
                        expr.SetCoefficient(_v[s, r, t], 1.0);

                Console.WriteLine($"x[{s},*,*]_Equals_1");
            }

            // No room can be assigned to a session in a timeslot 
            // during which it is not available
            foreach (var room in rooms)
            {
                int roomIndex = _roomIds.IndexOfValue(room.Id).Value;
                foreach (var uts in room.UnavailableForTimeslots)
                {
                    int utsi = _timeslotIds.IndexOfValue(uts).Value;
                    Constraint expr = _model.MakeConstraint(0.0, 0.0, $"x[*,{roomIndex},{utsi}]_Equals_0");
                    for (int s = 0; s < sessionCount; s++)
                        expr.SetCoefficient(_v[s, roomIndex, utsi], 1.0);
                    Console.WriteLine($"x[*,{roomIndex},{utsi}]_Equals_0");
                }
            }

            // Sessions cannot be assigned to a timeslot during which
            // any presenter is unavailable
            foreach (var session in sessions)
            {
                int sessionIndex = _sessionIds.IndexOfValue(session.Id).Value;

                List<int> unavailableTimeslotIndexes = new List<int>();
                foreach (var presenter in session.Presenters)
                {
                    foreach (var unavailableTimeslot in presenter.UnavailableForTimeslots)
                    {
                        int timeslotIndex = _timeslotIds.IndexOfValue(unavailableTimeslot).Value;
                        unavailableTimeslotIndexes.Add(timeslotIndex);
                    }
                }

                if (unavailableTimeslotIndexes.Any())
                {
                    Constraint expr = _model.MakeConstraint(0.0, 0.0, $"PresentersUnavailable_Session[{sessionIndex}");
                    foreach (var utsi in unavailableTimeslotIndexes.Distinct())
                        for (int r = 0; r < roomCount; r++)
                            expr.SetCoefficient(_v[sessionIndex, r, utsi], 1.0);
                    Console.WriteLine($"PresentersUnavailable_Session[{sessionIndex}");
                }
            }

            // A speaker can only be involved with 1 session per timeslot
            var speakerIds = sessions.SelectMany(s => s.Presenters.Select(p => p.Id)).Distinct();
            foreach (int speakerId in speakerIds)
            {
                var pIds = sessions.Where(s => s.Presenters.Select(p => p.Id).Contains(speakerId)).Select(s => s.Id).ToArray();
                for (int i = 0; i < pIds.Length - 1; i++)
                    for (int j = i + 1; j < pIds.Length; j++)
                    {
                        int session1Index = _sessionIds.IndexOfValue(pIds[i]).Value;
                        int session2Index = _sessionIds.IndexOfValue(pIds[j]).Value;
                        CreateConstraintSessionsMustBeInDifferentTimeslots(session1Index, session2Index, timeslotCount, roomCount);
                    }
            }

            // A timeslot should have no more sessions in a particular 
            // topicId than absolutely necessary.
            var topicIds = sessions.Where(s => s.TopicId.HasValue).Select(s => s.TopicId.Value).Distinct();
            foreach (var topicId in topicIds)
            {
                double topicCount = sessions.Count(s => s.TopicId == topicId);
                double maxTopicCount = System.Math.Ceiling(topicCount / Convert.ToDouble(timeslotCount));

                for (int t = 0; t < timeslotCount; t++)
                {
                    var expr = _model.MakeConstraint(0.0, maxTopicCount, $"x[(topicId={topicId}),*,{t}]_LessEqual_{maxTopicCount}");
                    foreach (var session in sessions.Where(s => s.TopicId == topicId))
                    {
                        int sessionIndex = _sessionIds.IndexOfValue(session.Id).Value;
                        for (int r = 0; r < roomCount; r++)
                            expr.SetCoefficient(_v[sessionIndex, r, t], 1.0);
                    }
                    Console.WriteLine($"x[(topicId={topicId}),*,{t}]_LessEqual_{maxTopicCount}");
                }
            }

            // All sessions with dependencies on a session must be scheduled
            // later (with a higher timeslot index value) than that session S
            foreach (var session in sessions)
            {
                int sessionIndex = _sessionIds.IndexOfValue(session.Id).Value;
                foreach (var dependentSession in session.Dependencies)
                {
                    int dependentSessionIndex = _sessionIds.IndexOfValue(dependentSession.Id).Value;
                    LinearExpr dExpr = new LinearExpr();
                    LinearExpr sExpr = new LinearExpr();

                    for (int r = 0; r < roomCount; r++)
                        for (int t = 0; t < timeslotCount; t++)
                        {
                            dExpr += (_v[dependentSessionIndex, r, t] * (t + 1));
                            sExpr += (_v[sessionIndex, r, t] * t);
                        }

                    _model.Add(dExpr <= sExpr);
                    Console.WriteLine($"s[{sessionIndex}]_GreaterThan_s[{dependentSessionIndex}]");
                }
            }

        }

        private void CreateConstraintSessionsMustBeInDifferentTimeslots(int session1Index, int session2Index, int timeslotCount, int roomCount)
        {
            for (int t = 0; t < timeslotCount; t++)
            {
                Constraint expr = _model.MakeConstraint(0.0, 1.0, $"x[{session1Index},*,{t}]_NotEqual_x[{session2Index},*,{t}]");
                for (int r = 0; r < roomCount; r++)
                {
                    expr.SetCoefficient(_v[session1Index, r, t], 1.0);
                    expr.SetCoefficient(_v[session2Index, r, t], 1.0);
                }
                Console.WriteLine($"x[{session1Index},*,{t}]_NotEqual_x[{session2Index},*,{t}]");
            }
        }

        private static void Validate(IEnumerable<Session> sessions, IEnumerable<Room> rooms, IEnumerable<Timeslot> timeslots)
        {
            rooms.Validate();
            timeslots.Validate();
            sessions.Validate();

            sessions.ValidateAgainstRoomsAndTimeslots(rooms, timeslots);
        }


        private static Solver CreateLinearProgrammingSolver()
        {
            var solver = new Solver("LinearProgramming", Solver.GLOP_LINEAR_PROGRAMMING);
            if (solver == null)
                throw new InvalidOperationException("Could not create solver");
            return solver;
        }

        private static Solver CreateMixedIntegerProgrammingSolver()
        {
            var solver = new Solver("MIP", Solver.CBC_MIXED_INTEGER_PROGRAMMING);
            if (solver == null)
                throw new InvalidOperationException("Could not create solver");
            return solver;
        }

    }
}
