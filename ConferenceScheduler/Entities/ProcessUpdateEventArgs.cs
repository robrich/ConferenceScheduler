using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceScheduler.Entities
{
    /// <summary>
    /// Represents the current status of the optimization processing
    /// </summary>
    public class ProcessUpdateEventArgs
    {
        /// <summary>
        /// The collection of assignments as they currently stand
        /// </summary>
        public IEnumerable<Assignment> Assignments { get; set; }


    }
}
