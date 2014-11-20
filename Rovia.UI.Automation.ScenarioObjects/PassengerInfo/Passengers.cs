

using System.Collections.Generic;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class Passengers
    {
        public int Infants { get; set; }
        public int Adults { get; set; }
        private int _childCount;
        public int Children {
            get { return ChildrenAges == null ? _childCount : ChildrenAges.Count; }
            set
            {
                _childCount = value;
                ChildrenAges = null;
            }
        }
        public List<string> ChildrenAges { get; set; }
    }
}
