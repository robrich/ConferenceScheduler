using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceScheduler.Exceptions
{
    /// <summary>
    /// Indicates that the solution as specified has no feasible solutions
    /// </summary>
    public class NoFeasibleSolutionsException: Exception
    {
        const string _defaultMessage = "No feasible solutions were found";

        /// <summary>
        /// Creates an instance of the exception with the default message.
        /// </summary>
        public NoFeasibleSolutionsException()
            : this(_defaultMessage)
        {
        }

        /// <summary>
        /// Creates an instance of the exception with the specified message.
        /// </summary>
        /// <param name="message">The message to deliver to the downstream client.</param>
        public NoFeasibleSolutionsException(string message):base(message)
        {
        }

        /// <summary>
        /// Creates an instance of the exception with the specified message and innerException.
        /// </summary>
        /// <param name="message">The message to deliver to the downstream client.</param>
        /// <param name="innerException">The exception that triggered this error.</param>
        public NoFeasibleSolutionsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }


    }
}
