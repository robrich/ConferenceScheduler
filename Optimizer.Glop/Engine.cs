using ConferenceScheduler.Entities;
using ConferenceScheduler.Exceptions;
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

        //GRBEnv _env;
        //GRBModel _model;

        //GRBVar[,,] _v;
        //GRBVar[] _s;

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
            //_env = new GRBEnv("ConferenceOptimizer.log");
            //_model = new GRBModel(_env);
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

            // _model.Optimize();
            //var status = _model.Get(GRB.IntAttr.Status);
            //if (status == GRB.Status.INFEASIBLE)
            //    throw new NoFeasibleSolutionsException();

            //var v = _model.Get(GRB.DoubleAttr.X, _v);
            //var p = _model.Get(GRB.DoubleAttr.X, _s);

            //for (int i = 0; i < sessions.Count(); i++)
            //    Console.WriteLine($"s[{i}] = {p[i]}");

            var results = new List<Assignment>();
            //for (int s = 0; s < sessions.Count(); s++)
            //    for (int r = 0; r < rooms.Count(); r++)
            //        for (int t = 0; t < timeslots.Count(); t++)
            //        {
            //            if (v[s, r, t] == 1.0)
            //                results.Add(new Assignment(_roomIds[r], _timeslotIds[t], _sessionIds[s]));
            //        }

            return results;
        }

        private void CreateVariables(int sessionCount, int roomCount, int timeslotCount)
        {
            //_v = new GRBVar[sessionCount, roomCount, timeslotCount];
            //for (int s = 0; s < sessionCount; s++)
            //    for (int r = 0; r < roomCount; r++)
            //        for (int t = 0; t < timeslotCount; t++)
            //        {
            //            _v[s, r, t] = _model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, $"x[{s},{r},{t}]");
            //            Console.WriteLine($"x[{s},{r},{t}]");
            //        }

            //_s = new GRBVar[sessionCount];
            //for (int s = 0; s < sessionCount; s++)
            //{
            //    _s[s] = _model.AddVar(0.0, Convert.ToDouble(timeslotCount), 0.0, GRB.INTEGER, $"s[{s}]");
            //    Console.WriteLine($"s[{s}]");
            //}

            //_model.Update();
        }

        private void CreateConstraints(IEnumerable<Session> sessions, IEnumerable<Room> rooms, int timeslotCount)
        {
            //int sessionCount = sessions.Count();
            //int roomCount = rooms.Count();

            //// Each room can have no more than 1 session per timeslot
            //for (int r = 0; r < roomCount; r++)
            //    for (int t = 0; t < timeslotCount; t++)
            //    {
            //        GRBLinExpr expr = 0.0;
            //        for (int s = 0; s < sessionCount; s++)
            //            expr.AddTerm(1.0, _v[s, r, t]);

            //        _model.AddConstr(expr <= 1.0, $"x[*,{r},{t}]_LessEqual_1");
            //        Console.WriteLine($"x[*,{r},{t}]_LessEqual_1");
            //    }

            //// Each session must be assigned to exactly 1 room/timeslot combination
            //for (int s = 0; s < sessionCount; s++)
            //{
            //    GRBLinExpr expr = 0.0;
            //    for (int r = 0; r < roomCount; r++)
            //        for (int t = 0; t < timeslotCount; t++)
            //            expr.AddTerm(1.0, _v[s, r, t]);

            //    _model.AddConstr(expr == 1.0, $"x[{s},*,*]_Equals_1");
            //    Console.WriteLine($"x[{s},*,*]_Equals_1");
            //}

            //// No room can be assigned to a session in a timeslot 
            //// during which it is not available
            //foreach (var room in rooms)
            //{
            //    int roomIndex = _roomIds.IndexOfValue(room.Id).Value;
            //    foreach (var uts in room.UnavailableForTimeslots)
            //    {
            //        int utsi = _timeslotIds.IndexOfValue(uts).Value;
            //        GRBLinExpr expr = 0.0;
            //        for (int s = 0; s < sessionCount; s++)
            //            expr.AddTerm(1.0, _v[s, roomIndex, utsi]);
            //        _model.AddConstr(expr == 0.0, $"x[*,{roomIndex},{utsi}]_Equals_0");
            //        Console.WriteLine($"x[*,{roomIndex},{utsi}]_Equals_0");
            //    }
            //}

            //// Sessions cannot be assigned to a timeslot during which
            //// any presenter is unavailable
            //foreach (var session in sessions)
            //{
            //    int sessionIndex = _sessionIds.IndexOfValue(session.Id).Value;

            //    List<int> unavailableTimeslotIndexes = new List<int>();
            //    foreach (var presenter in session.Presenters)
            //    {
            //        foreach (var unavailableTimeslot in presenter.UnavailableForTimeslots)
            //        {
            //            int timeslotIndex = _timeslotIds.IndexOfValue(unavailableTimeslot).Value;
            //            unavailableTimeslotIndexes.Add(timeslotIndex);
            //        }
            //    }

            //    if (unavailableTimeslotIndexes.Any())
            //    {
            //        GRBLinExpr expr = 0.0;
            //        foreach (var utsi in unavailableTimeslotIndexes.Distinct())
            //            for (int r = 0; r < roomCount; r++)
            //                expr.AddTerm(1.0, _v[sessionIndex, r, utsi]);
            //        _model.AddConstr(expr == 0, $"PresentersUnavailable_Session[{sessionIndex}");
            //        Console.WriteLine($"PresentersUnavailable_Session[{sessionIndex}");
            //    }
            //}

            //// A speaker can only be involved with 1 session per timeslot
            //var speakerIds = sessions.SelectMany(s => s.Presenters.Select(p => p.Id)).Distinct();
            //foreach (int speakerId in speakerIds)
            //{
            //    var pIds = sessions.Where(s => s.Presenters.Select(p => p.Id).Contains(speakerId)).Select(s => s.Id).ToArray();
            //    for (int i = 0; i < pIds.Length - 1; i++)
            //        for (int j = i + 1; j < pIds.Length; j++)
            //        {
            //            int session1Index = _sessionIds.IndexOfValue(pIds[i]).Value;
            //            int session2Index = _sessionIds.IndexOfValue(pIds[j]).Value;
            //            CreateConstraintSessionsMustBeInDifferentTimeslots(session1Index, session2Index, timeslotCount, roomCount);
            //        }
            //}

            //// The value of s[i] must be equal to the index of the timeslot
            //// that session i is assigned to
            //for (int i = 0; i < sessionCount; i++)
            //{
            //    GRBLinExpr expr = 0.0;
            //    for (int t = 0; t < timeslotCount; t++)
            //        for (int r = 0; r < roomCount; r++)
            //            expr.AddTerm(t, _v[i, r, t]);
            //    _model.AddConstr(_s[i] == expr, $"s[{i}]_Equals_Timeslot_x[{i},*,*]");
            //    Console.WriteLine($"s[{i}]_Equals_Timeslot");
            //}

            //// All sessions with dependencies on session S must be scheduled
            //// later (with a higher index value) than session S
            //foreach (var session in sessions)
            //{
            //    int sessionIndex = _sessionIds.IndexOfValue(session.Id).Value;
            //    foreach (var dependentSession in session.Dependencies)
            //    {
            //        int dependentSessionIndex = _sessionIds.IndexOfValue(dependentSession.Id).Value;
            //        _model.AddConstr((_s[dependentSessionIndex] + 1) <= _s[sessionIndex], $"s[{dependentSessionIndex}]_GreaterThan_s[{sessionIndex}]");
            //        Console.WriteLine($"s[{dependentSessionIndex}]_GreaterThan_s[{sessionIndex}]");
            //    }
            //}

            //_model.Update();
        }

        private void CreateConstraintSessionsMustBeInDifferentTimeslots(int session1Index, int session2Index, int timeslotCount, int roomCount)
        {
            //for (int t = 0; t < timeslotCount; t++)
            //{
            //    GRBLinExpr expr = 0.0;
            //    for (int r = 0; r < roomCount; r++)
            //    {
            //        expr.AddTerm(1.0, _v[session1Index, r, t]);
            //        expr.AddTerm(1.0, _v[session2Index, r, t]);
            //    }
            //    _model.AddConstr(expr <= 1.0, $"x[{session1Index},*,{t}]_NotEqual_x[{session2Index},*,{t}]");
            //    Console.WriteLine($"x[{session1Index},*,{t}]_NotEqual_x[{session2Index},*,{t}]");
            //}
        }

        private static void Validate(IEnumerable<Session> sessions, IEnumerable<Room> rooms, IEnumerable<Timeslot> timeslots)
        {
            if (sessions == null)
                throw new ArgumentNullException(nameof(sessions));

            if (rooms == null)
                throw new ArgumentNullException(nameof(rooms));

            if (timeslots == null)
                throw new ArgumentNullException(nameof(timeslots));

            if (sessions.Count() == 0)
                throw new ArgumentException("You must have at least one session");

            if (timeslots.Count() == 0)
                throw new ArgumentException("You must have at least one timeslot");

            if (rooms.Count() == 0)
                throw new ArgumentException("You must have at least one room");

            if (sessions.Count(s => s.Presenters == null || s.Presenters.Count() < 1) > 0)
                throw new ArgumentException("Every session must have at least one presenter.");

            var presentations = new SessionsCollection(sessions);
            if (presentations.HaveCircularDependencies())
                throw new DependencyException("Sessions may not have circular dependencies.");
        }

    }
}
