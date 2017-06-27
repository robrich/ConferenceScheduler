using ConferenceScheduler.Entities;
using ConferenceScheduler.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Optimizer.DataSetsTest
{
    public enum Topic
    {
        Other = 0,
        SoftSkills = 1,
        IOT = 2,
        Python = 3,
        Agile = 4,
        AI = 5,
        Mobile = 6,
        Cloud = 7,
        Blockchain = 8,
        DevOps = 9,
        Angular = 10,
        Javascript = 11,
        DotNetCore = 12,
        BI = 13,
        ReactRedux = 14
    }

    [TestFixture]
    public class SoCalCodeCampSanDiego2017
    {
        [Test]
        public void ScheduleWithoutPreferences()
        {
            var engine = (null as IConferenceOptimizer).Create();
            var sessions = new SessionsCollection();
            var rooms = new List<Room>();
            var timeslots = new List<Timeslot>();


            // Presenters
            var presenterRichClingman = Presenter.Create(1);
            var presenterTreyHunner = Presenter.Create(2);
            var presenterMichaelLebo = Presenter.Create(3);
            var presenterAaronRuckman = Presenter.Create(4);
            var presenterDanielEgan = Presenter.Create(5);
            var presenterLorraineKan = Presenter.Create(6);
            var presenterChrisLucian = Presenter.Create(7);
            var presenterBretStateham = Presenter.Create(8);
            var presenterRobinShahan = Presenter.Create(9);
            var presenterJustinJames = Presenter.Create(10);
            var presenterTobiasHughes = Presenter.Create(11);
            var presenterWendySteinman = Presenter.Create(12);
            var presenterOgunTigli = Presenter.Create(13);
            var presenterHibaMughal = Presenter.Create(14);
            var presenterEricFloe = Presenter.Create(15);
            var presenterHattanShobokshi = Presenter.Create(16);
            var presenterJonBachelor = Presenter.Create(17);
            var presenterPaulWhitmer = Presenter.Create(18);
            var presenterDavidFord = Presenter.Create(19);
            var presenterVaziOkhandiar = Presenter.Create(20);
            var presenterChrisStead = Presenter.Create(21);
            var presenterRyanMilbourne = Presenter.Create(22);
            var presenterMaxNodland = Presenter.Create(23);
            var presenterBarryStahl = Presenter.Create(24);
            var presenterJustineCocci = Presenter.Create(25);
            var presenterChrisGriffith = Presenter.Create(26);
            var presenterIotLaboratory = Presenter.Create(27);



            // Sessions
            var session01 = sessions.Add(1, (int)Topic.SoftSkills, presenterRichClingman);
            var session02 = sessions.Add(2, (int)Topic.IOT, presenterRichClingman);

            var session03 = sessions.Add(3, (int)Topic.Python, presenterTreyHunner);
            var session04 = sessions.Add(4, (int)Topic.Python, presenterTreyHunner);

            var session05 = sessions.Add(5, (int)Topic.Other, presenterMichaelLebo);

            var session06 = sessions.Add(6, (int)Topic.Agile, presenterAaronRuckman);

            var session07 = sessions.Add(7, (int)Topic.AI, presenterDanielEgan);

            var session08 = sessions.Add(8, (int)Topic.Mobile, presenterLorraineKan);

            var session09 = sessions.Add(9, (int)Topic.Agile, presenterChrisLucian);

            var session10 = sessions.Add(10, (int)Topic.IOT, presenterBretStateham);

            var session11 = sessions.Add(11, (int)Topic.Cloud, presenterRobinShahan);

            var session12 = sessions.Add(12, (int)Topic.SoftSkills, presenterJustinJames);

            var session13 = sessions.Add(13, (int)Topic.Blockchain, presenterTobiasHughes);

            var session14 = sessions.Add(14, (int)Topic.Other, presenterWendySteinman);

            var session15 = sessions.Add(15, (int)Topic.Angular, presenterOgunTigli);

            var session16 = sessions.Add(16, (int)Topic.Mobile, presenterHibaMughal);

            var session17 = sessions.Add(17, (int)Topic.Mobile, presenterEricFloe);

            var session18 = sessions.Add(18, (int)Topic.Javascript, presenterHattanShobokshi);

            var session19 = sessions.Add(19, (int)Topic.Mobile, presenterJonBachelor);

            var session20 = sessions.Add(20, (int)Topic.DotNetCore, presenterPaulWhitmer);

            var session21 = sessions.Add(21, (int)Topic.Javascript, presenterDavidFord);

            var session22 = sessions.Add(22, (int)Topic.BI, presenterVaziOkhandiar);

            var session23 = sessions.Add(23, (int)Topic.Javascript, presenterChrisStead);

            var session24 = sessions.Add(24, (int)Topic.Blockchain, presenterRyanMilbourne);
            var session25 = sessions.Add(25, (int)Topic.Blockchain, presenterRyanMilbourne);

            var session26 = sessions.Add(26, (int)Topic.Javascript, presenterJustinJames);
            var session27 = sessions.Add(27, (int)Topic.Mobile, presenterJustinJames);

            var session28 = sessions.Add(28, (int)Topic.ReactRedux, presenterMaxNodland);
            var session29 = sessions.Add(29, (int)Topic.ReactRedux, presenterMaxNodland);

            var session30 = sessions.Add(30, (int)Topic.AI, presenterBarryStahl);

            var session31 = sessions.Add(31, (int)Topic.AI, presenterJustineCocci);
            var session32 = sessions.Add(32, (int)Topic.AI, presenterJustineCocci);

            var session33 = sessions.Add(33, (int)Topic.DevOps, presenterHattanShobokshi);

            var session34 = sessions.Add(34, (int)Topic.Mobile, presenterChrisGriffith);

            var session35 = sessions.Add(35, (int)Topic.IOT, presenterIotLaboratory);
            var session36 = sessions.Add(36, (int)Topic.IOT, presenterIotLaboratory);
            var session37 = sessions.Add(37, (int)Topic.IOT, presenterIotLaboratory);
            var session38 = sessions.Add(38, (int)Topic.IOT, presenterIotLaboratory);
            var session39 = sessions.Add(39, (int)Topic.IOT, presenterIotLaboratory);
            var session40 = sessions.Add(40, (int)Topic.IOT, presenterIotLaboratory);


            // Session dependencies
            session25.AddDependency(session24);

            session36.AddDependency(session35);
            session37.AddDependency(session36);
            session38.AddDependency(session37);
            session39.AddDependency(session38);
            session40.AddDependency(session39);


            // Timeslots
            timeslots.Add(Timeslot.Create(1, 8.75));
            timeslots.Add(Timeslot.Create(2, 10));
            timeslots.Add(Timeslot.Create(3, 11.25));
            timeslots.Add(Timeslot.Create(4, 13.5));
            timeslots.Add(Timeslot.Create(5, 14.75));
            timeslots.Add(Timeslot.Create(6, 16));


            // Rooms
            rooms.Add(Room.Create(1, 10)); // Unex 127
            rooms.Add(Room.Create(2, 10)); // Unex 126
            rooms.Add(Room.Create(3, 10)); // Unex 110
            rooms.Add(Room.Create(4, 10)); // Unex 107
            rooms.Add(Room.Create(5, 10)); // Unex 106
            rooms.Add(Room.Create(6, 10)); // Unex 104
            rooms.Add(Room.Create(7, 10)); // Unex 101
            rooms.Add(Room.Create(8, 10, new int[] { 1, 2, 3 })); // Unex 129 -- Only available in PM


            // Create the schedule
            var assignments = engine.Process(sessions, rooms, timeslots);
            assignments.WriteSchedule();

        }
    }
}
