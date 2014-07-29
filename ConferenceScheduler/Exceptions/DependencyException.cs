using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceScheduler.Exceptions
{
    /// <summary>
    /// Indicates that an un-handleable situation exists with session dependencies
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
    public class DependencyException: Exception
    {
        const string _defaultMessage = "A condition exists with the session dependencies that cannot be handled by the processor.";

        /// <summary>
        /// Creates an instance of the exception with the specified message.
        /// </summary>
        /// <param name="message">The message to deliver to the downstream client.</param>
        public DependencyException(string message):base(message)
        {
        }

    }
}
