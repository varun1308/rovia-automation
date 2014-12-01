using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.DataBinder;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Validator;

namespace Rovia.UI.Automation.Tests.Utility
{
    [TestClass]
    public class TestHelper
    {
        private static RoviaApp _app;
        private static ILogger _logger;
        private static SearchCriteria _criteria;
        private static Results _selectedItineary;
        public static TripFolder Trip { get; set; }
        public static IValidator Validator { get; set; }

        public static List<Results> Results; 

        public static TripProductType TripProductType
        {
            get { return _app.State.CurrentProduct; }
            set { _app.State.CurrentProduct = value; }
        }

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            _app = new RoviaApp();
            _logger=new ConsoleLogger();
            GoToHomePage();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            _app.Dispose();
        }

        internal static void GoToHomePage()
        {

            _app.Launch(ApplicationSettings.Url);
            _app.HomePage.WaitForHomePage();
            _app.State.CurrentPage = "HomePage";
            _logger.Log("OnHomePage");
        }

        internal static void SetCriteria(SearchCriteria criteria)
        {
            _criteria = criteria;
        }

        //private static List<Results> ApplySpecialCriteria()
        //{
        //    var selectedResults = Results;
        //    if (_criteria != null)
        //    {
        //        var criteria = "";
        //        foreach (var criterium in _criteria.SpecialCriteria)
        //        {

        //            if (criterium.Name.Equals("Supplier"))
        //            {
        //                selectedResults = selectedResults.Where(x => x.Supplier.SupplierName.Equals(criterium.Value)).ToList();
        //                criteria += " | Supplier:" + criterium.Value;
        //                if (selectedResults.Count == 0)
        //                    throw new Exception("No Results found for " + criteria);
        //            }
        //            //todo add handling for other criteria
        //        }
        //    }
        //    return selectedResults;
        //}

        internal static void EditPassengerInfoAndContinue()
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("PassengerDetails-ConfirmationPage"))
                    throw new Exception("PassengerDetails-Editing is not available on " + _app.State.CurrentPage);
                _app.PassengerInfoPage.EditPassengerInfo();
            }
            catch (Exception exception)
            {
                throw new Exception("PassengerDetail-Submission failed", exception);
            }
        }

        internal static void Search()
        {
                if (!_app.State.CurrentPage.Equals("HomePage"))
                    throw new PageNotFoundException("HomePage");
                _app.HomePage.Search(_criteria);
            _app.ResultsPage.WaitForResultLoad();
            //    Results = _app.ResultsPage.ParseResults();
            //if(!Validator.ValidatePreSearchFilters(_criteria.Filters.PreSearchFilters,Results))
            //    throw new Exception("Result validation failed");
        }

        internal static void Login()
        {
            try
            {
                var requestingPage = _app.State.CurrentPage;
                if (_app.State.CurrentUser.IsLoggedIn)
                    LogOut();
                GoToLoginPage();
                switch (_criteria.UserType)
                {
                    case UserType.Registered:
                        _app.LoginDetailsPage.LogIn("vrathod@tavisca.com", "zaq1ZAQ!");
                        _app.State.CurrentUser.UserName = "vrathod@tavisca.com";
                        _app.State.CurrentUser.Type = _criteria.UserType;
                        _app.State.CurrentUser.IsLoggedIn = true;
                        break;
                    case UserType.Preferred:
                        _app.LoginDetailsPage.LogIn("PreferredUser", "Password");
                        _app.State.CurrentUser.UserName = "RegisteredUserUserName";
                        _app.State.CurrentUser.Type = _criteria.UserType;
                        _app.State.CurrentUser.IsLoggedIn = true;
                        break;
                    case UserType.Guest:
                        _app.LoginDetailsPage.ContinueAsGuest("vikul", "rathod", "vrathod@tavisca.com");
                        _app.State.CurrentUser.ResetUser();
                        break;
                }
                if (requestingPage.Equals("HomePage"))
                {
                    _app.HomePage.WaitForHomePage();
                    _app.State.CurrentPage = "HomePage";
                }
                else
                {
                    _app.PassengerInfoPage.WaitForPageLoad();
                    _app.State.CurrentPage = "PassengerInfoPage";
                }
            }
            catch (Exception exception)
            {

                throw new Exception("LogIn Failed", exception);
            }
        }

        internal static void AddToCart()
        {
                if (!_app.State.CurrentPage.EndsWith("ResultsPage"))
                    throw new PageNotFoundException("ResultsPage");
                _selectedItineary=_app.ResultsPage.AddToCart(_criteria.Supplier);
            if (_selectedItineary==null)
                throw new Exception("NoValidResultFound");
                _app.State.CurrentPage = "TripFolderPage";
                ParseTripFolder();
        }

        internal static void EnterPassengerDetails()
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("PassengerInfoPage"))
                    throw new PageNotFoundException("PassengerInfoPage");
                _app.PassengerInfoPage.SubmitPassengerDetails(GetPassengerDetails());
                _app.State.CurrentPage = "PassengerDetails-ConfirmationPage";
            }
            catch (Exception exception)
            {
                throw new Exception("PassengerDetail-Submission failed", exception);
            }
        }

        internal static void ConfirmPassengerDetails()
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("PassengerDetails-ConfirmationPage"))
                    throw new Exception("PassengerDetails-Confirmation is not available on " + _app.State.CurrentPage);
                _app.PassengerInfoPage.ConfirmPassengers();
                _app.CheckoutPage.WaitForLoad();
                _app.State.CurrentPage = "CheckOutPage";
            }
            catch (Exception exception)
            {

                throw new Exception("Passenger Confirmation failed", exception);
            }
        }

        private static void GoToLoginPage()
        {
            try
            {
                switch (_app.State.CurrentPage)
                {
                    case "HomePage": _app.HomePage.GoToLoginPage();
                        break;
                    case "LoginDetailsPage":
                        break;
                }
                _app.State.CurrentPage = "LoginDetailsPage";
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
                if (_app.State.CurrentUser.Type == UserType.Guest)
                    return;
                switch (_app.State.CurrentPage)
                {
                    case "HomePage": _app.HomePage.LogOut();
                        _app.State.CurrentPage = "LogInPage";
                        _app.State.CurrentUser.ResetUser();
                        GoToHomePage();
                        break;
                }
            }
            catch (Exception exception)
            {
                throw new Exception("LogOut Failed", exception);
            }

        }

        private static PassengerDetails GetPassengerDetails()
        {
            return new PassengerDetails(_criteria.Passengers)
            {
                Country = "United States",
                IsInsuranceRequired = false
            };
        }

        #region TripFolder Calls

        private static void ParseTripFolder()
        {
                if (!_app.State.CurrentPage.Equals("TripFolderPage"))
                    throw new PageNotFoundException("TripFolderPage");
                Trip = _app.TripFolderPage.ParseTripFolder();
        }

        public static void SaveTrip()
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Trip can not be save on " + _app.State.CurrentPage);
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
                if (!_app.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Trip can not be start over on " + _app.State.CurrentPage);
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
                if (!_app.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Trip name can not be edit on " + _app.State.CurrentPage);
                Trip.TripSettingsButton.Click();
                _app.TripFolderPage.EditTripName();
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
                if (!_app.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Trip product can not be modified on " + _app.State.CurrentPage);
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
                if (!_app.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Trip product can not be removed on " + _app.State.CurrentPage);
                Trip.TripProducts[index].RemoveProductButton.Click();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in removing trip product.", exception);
            }
        }

        public static void CheckoutTrip()
        {
                if (!_app.State.CurrentPage.Equals("TripFolderPage"))
                    throw new PageNotFoundException("TripFolderPage");
                Trip.CheckoutTripButton.Click();

                if (_app.State.CurrentUser.Type != UserType.Guest)
                {
                    _app.PassengerInfoPage.WaitForPageLoad();
                    _app.State.CurrentPage = "PassengerInfoPage";
                }
                else
                {
                    //_app.LoginDetailsPage.WaitForLoad();
                    _app.State.CurrentPage = "LoginDetailsPage";
                }
        }

        public static void ContinueShopping()
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Can not continue shopping on " + _app.State.CurrentPage);
                Trip.ContinueShoppingButton.Click();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in continue shopping", exception);
            }
        }

        #endregion

        #region Air Filters Call

        public static bool SetFilters()
        {
            return _app.ResultsPage.SetPostSearchFilters(_criteria.Filters.PostSearchFilters);
        }

        #endregion

        public static void PayNow()
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("CheckOutPage"))
                    throw new Exception("Pay-Now is not available on " + _app.State.CurrentPage);
                if (_criteria.PaymentMode == PaymentMode.RoviaBucks)
                    _app.CheckoutPage.PayNow(new PaymentInfo(_criteria.PaymentMode));
                else
                {
                    _app.CheckoutPage.PayNow(_criteria.PaymentMode);
                    _app.BFCPaymentPage.WaitForLoad();
                    _app.State.CurrentPage = "BFCPaymentPage";
                    _app.BFCPaymentPage.PayNow(new PaymentInfo(_criteria.PaymentMode, _criteria.CardType));
                }
                _app.CheckoutPage.CheckPaymentStatus();

            }
            catch (Exception exception)
            {
                throw new Exception("Pay Now failed", exception);
            }
        }

        internal static void CleanUp()
        {
            _app.ClearBrowserCache();
        }

    }

}
