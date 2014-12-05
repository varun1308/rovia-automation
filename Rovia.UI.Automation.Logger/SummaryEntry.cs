using System.Collections.Generic;

namespace Rovia.UI.Automation.Logger
{
    class SummaryEntry
    {
        public int Id { get; set; }
        private readonly List<string> _passedSteps;
        private readonly List<string> _failedSteps;
        public SummaryEntry()
        {
            _passedSteps=new List<string>();
            _failedSteps=new List<string>();
        }

        internal void AddStep(string step, string status)
        {
            if(status.ToUpper().Equals("PASSED"))
                _passedSteps.Add(step);
            else
                _failedSteps.Add(step);
        }

        public string Description { get; set; }

        public override string ToString()
        {
            return Id.ToString() + ',' + Description + ',' + string.Join("|", _passedSteps) + ',' + string.Join("|", _failedSteps) +
                   ',' + (_failedSteps.Count > 0
                       ? "FAILED"
                       : "PASSED");
        }
    }
}
