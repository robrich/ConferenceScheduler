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
    public class DependencyException: Exception
    {
        const string _defaultMessage = "A condition exists with the session dependencies that cannot be handled by the processor.";

        /// <summary>
        /// Creates an instance of the exception with the default message.
        /// </summary>
        public DependencyException()
            : this(_defaultMessage)
        {
        }

        /// <summary>
        /// Creates an instance of the exception with the specified message.
        /// </summary>
        /// <param name="message">The message to deliver to the downstream client.</param>
        public DependencyException(string message):base(message)
        {
        }

        /// <summary>
        /// Creates an instance of the exception with the specified message and innerException.
        /// </summary>
        /// <param name="message">The message to deliver to the downstream client.</param>
        /// <param name="innerException">The exception that triggered this error.</param>
        public DependencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }


    }
}
