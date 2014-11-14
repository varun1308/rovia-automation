using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.DataBinder;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.Ui.Automation.ScenarioObjects;
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
        public static TripFolder Trip { get; set; }

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

        private static List<Results> ApplySpecialCriteria()
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
                App.State.CurrentPage = "TripFolderPage";
            }
            catch (Exception exception)
            {
                throw new Exception("AddToCart Failed",exception);
            }
        }

        #region TripFolder Calls

        public static void ParseTripFolder()
        {
            try
            {
                if (!App.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Trip can not be parse on " + App.State.CurrentPage);
               Trip =  App.TripFolderPage.ParseTripFolder();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in parsing trip details.", exception);
            }
        }

        public static void SaveTrip()
        {
            try
            {
                if (!App.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Trip can not be save on " + App.State.CurrentPage);
                // to implement
                Trip.TripSettingsButton.Click();
                Trip.SaveTripButton.Click();

                //2 cases
                //1. If already logged in directly save the trip
                //2. If not, ask for login and then save the trip
            }
            catch (Exception exception)
            {
                throw new Exception("Error in saving trip.", exception);
            }
        }

        public static void TripStartOver()
        {
            try
            {
                if (!App.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Trip can not be start over on " + App.State.CurrentPage);
                Trip.TripSettingsButton.Click();
                Trip.StartoverButton.Click();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in trip start over", exception);
            }
        }

        public static void EditTripName()
        {
            try
            {
                if (!App.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Trip name can not be edit on " + App.State.CurrentPage);
                Trip.TripSettingsButton.Click();
                App.TripFolderPage.EditTripName();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in editing trip name.", exception);
            }
        }

        public static void ModifyProduct(int index)
        {
            try
            {
                if (!App.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Trip product can not be modified on " + App.State.CurrentPage);
                Trip.TripProducts[index].ModifyProductButton.Click();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in modifying trip product.", exception);
            }
        }

        public static void RemoveProduct(int index)
        {
            try
            {
                if (!App.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Trip product can not be removed on " + App.State.CurrentPage);
                Trip.TripProducts[index].RemoveProductButton.Click();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in removing trip product.", exception);
            }
        }

        public static void CheckoutTrip()
        {
            try
            {
                if (!App.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Trip can not be checkout on " + App.State.CurrentPage);
                Trip.CheckoutTripButton.Click();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in trip checkout.", exception);
            }
        }

        public static void ContinueShopping()
        {
            try
            {
                if (!App.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Can not able to continue shopping on " + App.State.CurrentPage);
                Trip.ContinueShoppingButton.Click();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in continue shopping", exception);
            }
        }

        #endregion
    }

}
