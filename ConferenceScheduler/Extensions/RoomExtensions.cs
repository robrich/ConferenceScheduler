using ConferenceScheduler.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceScheduler.Extensions
{
    // TODO: Document
    /// <summary>
    /// 
    /// </summary>
    public static class RoomExtensions
    {
        // TODO: Document
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rooms"></param>
        public static void Validate(this IEnumerable<Room> rooms)
        {
            if (rooms == null)
                throw new ArgumentNullException(nameof(rooms));

            if (rooms.Count() == 0)
                throw new ArgumentException("You must have at least one room");

            if (rooms.Select(r => r.Id).Distinct().Count() != rooms.Count())
                throw new ArgumentException("Room Ids must be unique");
        }
    }
}
