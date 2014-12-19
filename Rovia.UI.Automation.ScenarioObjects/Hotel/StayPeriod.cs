using System;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class StayPeriod
    {
        public override int GetHashCode()
        {
            unchecked
            {
                return (CheckInDate.Date.GetHashCode()*397) ^ CheckOutDate.Date.GetHashCode();
            }
        }

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Duration {
            get { return (CheckOutDate - CheckInDate).Days; }
        }

        public override bool Equals(object obj)
        {
            var other = obj as StayPeriod;
            if (other == null)
                return false;
            return CheckInDate.Date.Equals(other.CheckInDate.Date) && CheckOutDate.Date.Equals(other.CheckOutDate.Date);
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", CheckInDate.ToShortDateString(), CheckOutDate.ToShortDateString());
        }
    }
}
