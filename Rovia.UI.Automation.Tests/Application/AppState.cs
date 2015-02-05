namespace Rovia.UI.Automation.Tests.Application
{
    using ScenarioObjects;

    /// <summary>
    /// Saves the state of the application
    /// </summary>
    public class AppState
    {
        public string CurrentPage { get; set; }
        public User CurrentUser { get; set; }
        public TripProductType CurrentProduct { get; set; }
    }
}