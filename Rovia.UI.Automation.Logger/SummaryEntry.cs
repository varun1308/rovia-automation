namespace Rovia.UI.Automation.Logger
{
    using System.Collections.Generic;
    /// <summary>
    /// Class for summary log file
    /// </summary>
    class SummaryEntry
    {
        #region private fileds
        private readonly List<string> _passedSteps;
        private readonly List<string> _failedSteps;
        #endregion

        #region Public Properties
        public string Id { get; set; }
        #endregion

        #region public Members
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
            return "<< "+ Id + " >> " + Description + ',' + string.Join("|", _passedSteps) + ',' + string.Join("|", _failedSteps) +
                   ',' + (_failedSteps.Count > 0
                       ? "FAILED"
                       : "PASSED");
        }
        #endregion
    }
}
