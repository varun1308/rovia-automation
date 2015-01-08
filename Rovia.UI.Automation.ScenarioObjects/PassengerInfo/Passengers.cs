

using System.Collections.Generic;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class Passengers
    {
        protected bool Equals(Passengers other)
        {
            return Infants == other.Infants && Adults == other.Adults && _childCount == other._childCount;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Infants;
                hashCode = (hashCode*397) ^ Adults;
                hashCode = (hashCode*397) ^ Children;
                return hashCode;
            }
        }

        public int Infants { get; set; }
        public int Adults { get; set; }
        private int _childCount;

        public Passengers(string pax)
        {
            foreach (var passenger in pax.Split(','))
            {
                if (passenger.Contains("Adult"))
                    Adults = int.Parse(passenger.Trim()[0].ToString());
                if (passenger.Contains("Child"))
                    Children = int.Parse(passenger.Trim()[0].ToString());
                if (passenger.Contains("Infant"))
                    Infants = int.Parse(passenger.Trim()[0].ToString());
            }
        }

        public Passengers()
        {
            // TODO: Complete member initialization
        }
        public int Children {
            get { return ChildrenAges == null ? _childCount : ChildrenAges.Count; }
            set
            {
                _childCount = value;
                ChildrenAges = null;
            }
        }
        public List<string> ChildrenAges { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Passengers;
            if (other == null)
                return false;
            return Infants == other.Infants && Adults == other.Adults && Children == other.Children;
        }

        public override string ToString()
        {
            return string.Format("{0} Adults, {1} Children, {2} Infants", Adults, Children, Infants);
        }
    }
}
