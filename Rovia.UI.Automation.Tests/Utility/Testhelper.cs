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
            GoToHomePage();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            App.Dispose();
        }

        internal static void GoToHomePage()
        {
            App.Launch(ApplicationSettings.Url);
            App.State.CurrentPage = "HomePage";
        }

        internal static void SetCriteria(DataRow dataRow)
        {
            _criteria = DataBinder.GetCriteria(dataRow);
        }

        internal static List<Results> ApplySpecialCriteria()
        {

            var selectedResults = App.ResultsPage.ParseResults();
            if (_criteria != null)
            {
                foreach (var criterium in _criteria.SpecialCriteria)
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
            try
            {
                if (App.State.CurrentUser.IsLoggedIn)
                    LogOut();
                GoToLoginPage();
                switch (_criteria.UserType)
                {
                    case UserType.Registered:
                        App.LoginDetailsPage.LogIn("vrathod@tavisca.com", "zaq1ZAQ!");
                        App.State.CurrentUser.UserName = "vrathod@tavisca.com";
                        App.State.CurrentUser.Type = _criteria.UserType;
                        App.State.CurrentUser.IsLoggedIn = true;
                        break;
                    case UserType.Preferred:
                        App.LoginDetailsPage.LogIn("PreferredUser", "Password");
                        App.State.CurrentUser.UserName = "RegisteredUserUserName";
                        App.State.CurrentUser.Type = _criteria.UserType;
                        App.State.CurrentUser.IsLoggedIn = true;
                        break;
                    case UserType.Guest:
                        App.LoginDetailsPage.ContinueAsGuest();
                        App.State.CurrentUser.ReSetUser();
                        break;
                }
                App.State.CurrentPage = "HomePage";

            }
            catch (Exception exception)
            {
                
                throw new Exception("LogIn Failed", exception);
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

                throw new Exception("Login page failed to Load", exception);
            }
        }

        private static void LogOut()
        {
            try
            {
                switch (App.State.CurrentPage)
                {
                    case "HomePage": App.HomePage.LogOut();
                        App.State.CurrentPage = "LogInPage";
                        GoToHomePage();
                        break;
                }
            }
            catch (Exception exception)
            {

                throw new Exception("LogOut Failed", exception);
            }

        }

        internal static void Search()
        {

            try
            {
                if (!App.State.CurrentPage.Equals("HomePage"))
                    throw new Exception("Search is not available on " + App.State.CurrentPage);
                var criteria = _criteria is AirSearchCriteria ? _criteria as AirSearchCriteria : null;
                App.HomePage.Search(criteria);
                WaitForResultLoad();

            }
            catch (Exception exception)
            {
                
                throw new Exception("SearchFailed",exception);
            }
        }

        private static void WaitForResultLoad()
        {
            switch (_criteria.ProductType)
            {
                    case ProductType.Air:App.ResultsPage.WaitForResultLoad();
                    App.State.CurrentPage = "AirResultsPage";
                    break;
            }
        }

        public static void AddToCart()
        {
            try
            {
                if (!App.State.CurrentPage.EndsWith("ResultsPage"))
                    throw new Exception("AddToCart is not available on "+App.State.CurrentPage);
                App.ResultsPage.AddToCart(ApplySpecialCriteria());
            }
            catch (Exception exception)
            {
                throw new Exception("AddToCart Failed",exception);
            }
        }
    }

}
