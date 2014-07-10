using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceScheduler.Entities
{
    /// <summary>
    /// Represents an instance of a person who is presenting one or more sessions
    /// </summary>
    public class Presenter
    {
        /// <summary>
        /// The primary-key identifier of the object
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The list of timeslot Ids during which the person cannot present
        /// </summary>
        public IEnumerable<int> UnavailableForTimeslots { get; set; }

        /// <summary>
        /// Create an instance of the object based on the supplied data
        /// </summary>
        /// <param name="id">The unique Id of the presenter</param>
        /// <param name="presenterUnavailableForTimeslots">The list of timeslot Ids during which the person cannot present</param>
        /// <returns>A Presenter object</returns>
        public static Presenter Create(int id, params int[] presenterUnavailableForTimeslots)
        {
            return new Presenter()
            {
                Id = id,
                UnavailableForTimeslots = presenterUnavailableForTimeslots.AsEnumerable()
            };
        }

    }
}
