namespace Rovia.UI.Automation.Logger
{
    using System.Collections.Generic;
    /// <summary>
    /// Class for Details log file
    /// </summary>
    class DetailEntry
    {
        #region private fileds
        private readonly List<string> _details;
        #endregion

        #region Public Properties
        public string Id { get; set; }
        public string Description { get; set; }
        #endregion

        #region public Members
        public DetailEntry()
        {
            _details = new List<string>();
        }

        public void Append(string s)
        {
            _details.Add(s);
        }

        public override string ToString()
        {
            return "<< " + Id + " >> " + Description + "\n" + string.Join("\n", _details) +
                "\n---------------------------------------------------------------------------------";
        }
        #endregion
    }
}
