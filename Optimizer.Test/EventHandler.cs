using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceScheduler.Entities;

namespace ConferenceScheduler.Optimizer.Test
{
    public class EventHandler
    {
        public IEnumerable<Assignment> Assignments { get; private set; }

        public void EngineUpdateEventHandler(IEnumerable<Assignment> assignments)
        {
            this.Assignments = assignments;
            this.Assignments.WriteSchedule();
        }
    }
}
