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
using Rovia.UI.Automation.Logger;
using InvalidOperationException = Rovia.UI.Automation.Exceptions.InvalidOperationException;

namespace Rovia.UI.Automation.Tests.Utility
{
    [TestClass]
    public class TestHelper
    {
        #region Datafields

        public static string SessionId { get { return  _app.HomePage.GetSessionId(); }}
        private static RoviaApp _app;
        private static LogManager _logger;
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

        #endregion

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            LogManager.Initialise();
            _logger = LogManager.GetInstance();
            _app = new RoviaApp();
            GoToHomePage();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            _app.Dispose();
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
                _logger.LogInformation("GoToLoginPage Failed");
                throw; // new Exception("Login page failed to Load", exception);
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
                _logger.LogInformation("LogOut Failed");
                throw;// new Exception("LogOut Failed", exception);
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

        internal static void GoToHomePage()
        {

            _app.Launch(ApplicationSettings.Url);
            _app.HomePage.WaitForHomePage();
            _app.State.CurrentPage = "HomePage";
            //_logger.Log("OnHomePage");
        }

        internal static void SetCriteria(SearchCriteria criteria)
        {
            _criteria = criteria;
        }

        internal static void EditPassengerInfoAndContinue()
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("PassengerDetails-ConfirmationPage"))
                    throw new InvalidOperationException("EditPassengerInfoAndContinue", _app.State.CurrentPage);
                _app.PassengerInfoPage.EditPassengerInfo();
                _logger.LogStatus("EditPassengerInfoAndContinue", "Passed");
            }
            catch (Exception exception)
            {
                _logger.LogStatus("EditPassengerInfoAndContinue", "Failed");
                throw; // new Exception("LogIn Failed", exception);
            }
        }

        internal static void Search()
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("HomePage"))
                    throw new InvalidOperationException("Search",_app.State.CurrentPage);
                _app.HomePage.Search(_criteria);
                _app.ResultsPage.WaitForResultLoad();
                _app.State.CurrentPage = "ResultsPage";
               Assert.IsTrue(_app.ResultsPage.VerifyPreSearchFilters(_criteria.Filters.PreSearchFilters),"Addtional search filters not applied.");
                //    Results = _app.ResultsPage.ParseResults();
                //if(!Validator.ValidatePreSearchFilters(_criteria.Filters.PreSearchFilters,Results))
                //     throw new ValidationException("Results ");
                _logger.LogStatus("Search", "Passed");
            }
            catch (Exception exception)
            {
                _logger.LogStatus("Search", "Failed");
                throw;
            }
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
                        _app.LoginDetailsPage.LogIn(ApplicationSettings.RegisteredCustomer.Username, ApplicationSettings.RegisteredCustomer.Password);
                        _app.State.CurrentUser.UserName = "vrathod@tavisca.com";
                        _app.State.CurrentUser.Type = _criteria.UserType;
                        _app.State.CurrentUser.IsLoggedIn = true;
                        break;
                    case UserType.Preferred:
                        _app.LoginDetailsPage.LogIn(ApplicationSettings.PreferredCustomer.Username,ApplicationSettings.PreferredCustomer.Password);
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
                _logger.LogStatus("Login", "Passed");
            }
            catch (Exception exception)
            {
                _logger.LogStatus("Login","Failed");
                throw; // new Exception("LogIn Failed", exception);
            }
        }

        internal static void AddToCart()
        {
            try
            {
                if (!_app.State.CurrentPage.EndsWith("ResultsPage"))
                    throw new InvalidOperationException("AddToCart", _app.State.CurrentPage);
                _selectedItineary = _app.ResultsPage.AddToCart(_criteria.Supplier);
                if (_selectedItineary == null)
                    throw new AddToCartFailedException();
                _app.State.CurrentPage = "TripFolderPage";
                ParseTripFolder();
                _logger.LogStatus("AddToCart", "Passed");
            }
            catch (Exception exception)
            {
                _logger.LogStatus("AddToCart", "Failed");
                throw;
            }
        }

        internal static void EnterPassengerDetails()
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("PassengerInfoPage"))
                    throw new InvalidOperationException("EnterPassengerDetails", _app.State.CurrentPage);
                _app.PassengerInfoPage.SubmitPassengerDetails(GetPassengerDetails());
                _app.State.CurrentPage = "PassengerDetails-ConfirmationPage";
                _logger.LogStatus("EnterPassengerDetails", "Passed");
            }
            catch (Exception exception)
            {
                _logger.LogStatus("EnterPassengerDetails", "Failed");
                throw;
            }
        }

        internal static void ConfirmPassengerDetails()
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("PassengerDetails-ConfirmationPage"))
                    throw new InvalidOperationException("ConfirmPassengerDetails", _app.State.CurrentPage);
                _app.PassengerInfoPage.ConfirmPassengers();
                _app.CheckoutPage.WaitForLoad();
                _app.State.CurrentPage = "CheckOutPage";
                _logger.LogStatus("ConfirmPassengerDetails", "Passed");
            }
            catch (Exception exception)
            {
                _logger.LogStatus("ConfirmPassengerDetails", "Failed");
                throw;
            }
        }

        internal static void CleanUp()
        {
            _app.ClearBrowserCache();
            _app.State.CurrentUser.ResetUser();
            GoToHomePage();
        }

        #region TripFolder Calls

        private static void ParseTripFolder()
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("TripFolderPage"))
                    throw new InvalidOperationException("ParseTripFolder", _app.State.CurrentPage);
                Trip = _app.TripFolderPage.ParseTripFolder();
            }
            catch (Exception exception)
            {
                _logger.LogInformation("TripFolder Parsing Failed");
                throw;
            }
        }

        public static void SaveTrip()
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("TripFolderPage"))
                    throw new InvalidOperationException("SaveTrip", _app.State.CurrentPage);
                // to implement
                Trip.TripSettingsButton.Click();
                Trip.SaveTripButton.Click();

                //2 cases
                //1. If already logged in directly save the trip
                //2. If not, ask for login and then save the trip
                _logger.LogStatus("SaveTrip", "Passed");
            }
            catch (Exception exception)
            {
                _logger.LogStatus("SaveTrip", "Failed");
                throw;
            }
        }

        public static void TripStartOver()
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("TripFolderPage"))
                    throw new InvalidOperationException("TripStartOver", _app.State.CurrentPage);
                Trip.TripSettingsButton.Click();
                Trip.StartoverButton.Click();
                _logger.LogStatus("TripStartOver", "Passed");
            }
            catch (Exception exception)
            {
                _logger.LogStatus("TripStartOver", "Failed");
                throw;
            }
        }

        public static void EditTripName()
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("TripFolderPage"))
                    throw new InvalidOperationException("EditTripName", _app.State.CurrentPage);
                Trip.TripSettingsButton.Click();
                _app.TripFolderPage.EditTripName();
                _logger.LogStatus("EditTripName", "Passed");
            }
            catch (Exception exception)
            {
                _logger.LogStatus("EditTripName", "Failed");
                throw;
            }
        }

        public static void ModifyProduct(int index)
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("TripFolderPage"))
                    throw new InvalidOperationException("ModifyProduct", _app.State.CurrentPage);
                Trip.TripProducts[index].ModifyProductButton.Click();
                _logger.LogStatus("ModifyProduct", "Passed");
            }
            catch (Exception exception)
            {
                _logger.LogStatus("ModifyProduct", "Failed");
                throw;
            }
        }

        public static void RemoveProduct(int index)
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("TripFolderPage"))
                    throw new InvalidOperationException("RemoveProduct", _app.State.CurrentPage);
                Trip.TripProducts[index].RemoveProductButton.Click();
                _logger.LogStatus("RemoveProduct", "Passed");
            }
            catch (Exception exception)
            {
                _logger.LogStatus("RemoveProduct", "Failed");
                throw;
            }
        }

        public static void CheckoutTrip()
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("TripFolderPage"))
                    throw new InvalidOperationException("CheckoutTrip", _app.State.CurrentPage);
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
                _logger.LogStatus("CheckoutTrip", "Passed");
            }
            catch (Exception exception)
            {
                _logger.LogStatus("CheckoutTrip", "Failed");
                throw;
            }
        }

        public static void ContinueShopping()
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("TripFolderPage"))
                    throw new InvalidOperationException("ContinueShopping", _app.State.CurrentPage);
                Trip.ContinueShoppingButton.Click();
                _logger.LogStatus("ContinueShopping", "Passed");
            }
            catch (Exception exception)
            {
                _logger.LogStatus("ContinueShopping", "Failed");
                throw; // new Exception("LogIn Failed", exception);
            }
        }

        #endregion

        public static bool SetFilters()
        {
            return _app.ResultsPage.SetPostSearchFilters(_criteria.Filters.PostSearchFilters);
        }

        public static void PayNow()
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("CheckOutPage"))
                    throw new InvalidOperationException("PayNow", _app.State.CurrentPage);
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
                _logger.LogStatus("PayNow", "Passed");
            }
            catch (Exception exception)
            {
                _logger.LogStatus("PayNow", "Failed");
                throw; // new Exception("LogIn Failed", exception);
            }
        }

        public static void SaveScreenShot(TestContext context)
        {
            _app.SaveScreenshot(context);
        }

        public static void SaveSessionId(TestContext context)
        {
            _app.SaveSessionId(context,_app.HomePage.GetSessionId());
        }
    }
}
