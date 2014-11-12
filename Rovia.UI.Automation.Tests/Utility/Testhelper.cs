using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.DataBinder;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Utility
{
    [TestClass]
    public class TestHelper
    {
        public static RoviaApp App { get; set; }
        private static SearchCriteria _criteria;
        public static IScenarioDataBinder DataBinder { get; set; }
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            App = new RoviaApp();
            App.Launch(ApplicationSettings.Url);
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            App.Dispose();
        }


        internal static void SetCriteria(DataRow dataRow)
        {
            _criteria = DataBinder.GetCriteria(dataRow);
        }


        internal static List<AirResult> ApplySpecialCriteria(IEnumerable<SpecialCriteria> criteria)
        {

            var selectedResults = App.AirResultsPage.ParseResults();
            if (criteria != null)
            {
                foreach (var criterium in criteria)
                {
                    if (criterium.Name.Equals("Supplier"))
                        selectedResults = selectedResults.Where(x => x.Supplier.SupplierName.Equals(criterium.Value)).ToList();
                    //todo add handling for other criteria
                }
            }
            return selectedResults;
        }



        internal static void Login()
        {
            if (App.State.CurrentUser.IsLoggedIn)
            {
                LogOut();
            }
            if (_criteria.UserType == UserType.Guest)
                App.State.CurrentUser.ReSetUser();
            else
            {
                GoToLoginPage();
                switch (_criteria.UserType)
                {
                    case UserType.Registered:
                        App.LoginDetailsPage.LogIn("vrathod@tavisca.com", "zaq1ZAQ!");
                        App.State.CurrentUser.UserName = "vrathod@tavisca.com";
                        break;
                    case UserType.Preferred:
                        App.LoginDetailsPage.LogIn("PreferredUser", "Password");
                        App.State.CurrentUser.UserName = "RegisteredUserUserName";
                        break;
                }
                App.State.CurrentUser.Type = _criteria.UserType;
                App.State.CurrentUser.IsLoggedIn = true;
            }
        }

        private static void GoToLoginPage()
        {
            try
            {
                switch (App.State.CurrentPage)
                {
                    case "HomePage": App.HomePage.GoToLoginPage();
                        break;
                }
                App.State.CurrentPage = "LogInPage";





            }
            catch (Exception exception)
            {

                throw new Exception("LogOutFailed", exception);
            }
        }

        private static void LogOut()
        {
            try
            {
                switch (App.State.CurrentPage)
                {
                    case "HomePage": App.HomePage.LogOut();
                        App.GoToHomePage();
                        break;
                }
            }
            catch (Exception exception)
            {

                throw new Exception("LogOutFailed", exception);
            }

        }

        internal static void Search()
        {
            var criteria=_criteria is AirSearchCriteria  ? _criteria as AirSearchCriteria :null;
            App.HomePage.Search(criteria);
        }
    }

}
