using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceScheduler.Optimizer
{
    internal class PresenterAvailability
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal int TimeslotId { get; set; }

        internal int PresenterId { get; set; }

        internal bool IsAvailable { get; set; }

    }
}
