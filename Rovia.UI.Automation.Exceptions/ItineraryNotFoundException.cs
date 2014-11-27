using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.Exceptions
{
    public class ItineraryNotAvailableException : Exception
    {
        public ItineraryNotAvailableException()
            : base("Selected itinerary not available")
        {
            
        }
        public ItineraryNotAvailableException(Exception exception)
            : base("Selected itinerary not available", exception)
        {
        }
    }
}
