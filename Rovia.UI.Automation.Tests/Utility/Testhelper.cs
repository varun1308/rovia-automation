using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Criteria;
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

        public static string TripsErrorUI { get; set; }
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
            var i = 0;
            while (true)
            {
                try
                {
                    GoToHomePage();
                    break;
                }
                catch (PageLoadFailed)
                {
                    _app.SaveScreenshot("SiteLoadFailed" + i++);
                }
            }
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
            catch (Exception)
            {
                _logger.LogInformation("GoToLoginPage Failed");
                throw;
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
            catch (Exception)
            {
                _logger.LogInformation("LogOut Failed");
                throw;
            }

        }

        private static PassengerDetails GetPassengerDetails()
        {
            switch (TripProductType)
            {
                case TripProductType.Hotel:
                case TripProductType.Car:
                    return new PassengerDetails(new Passengers("1 Adult"));
                case TripProductType.Activity:
                    var activitySearchCriteria = _criteria as ActivitySearchCriteria;
                    return new PassengerDetails(activitySearchCriteria.Passengers,activitySearchCriteria.AdultAgeGroup,activitySearchCriteria.ChildrenAgeGroup,activitySearchCriteria.InfantAgeGroup);
                case TripProductType.Air:
                    return new PassengerDetails(_criteria.Passengers);
                default:
                    return null;
            }
        }

        internal static void GoToHomePage()
        {
            _app.Launch(ApplicationSettings.Url);
            ResolveProductionSitePopup();
            _app.HomePage.WaitForHomePage();
            _app.State.CurrentPage = "HomePage";
        }

        private static void ResolveProductionSitePopup()
        {
            if (ApplicationSettings.Environment == "PROD")
            {
                _app.ConfirmAlert();
                Thread.Sleep(2000);
            }
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
            catch (Exception)
            {
                _logger.LogStatus("EditPassengerInfoAndContinue", "Failed");
                throw;
            }
        }

        internal static void Search()
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("HomePage"))
                    throw new InvalidOperationException("Search", _app.State.CurrentPage);
                _app.HomePage.Search(_criteria);
                _app.ResultsPage.WaitForResultLoad();
                TripsErrorUI = _app.HomePage.GetTripsErrorUri();
                //_app.ResultsPage.ValidateSearch(_criteria);
                _app.State.CurrentPage = "ResultsPage";
                _app.ResultsPage.VerifyPreSearchFilters(_criteria.Filters.PreSearchFilters);
                _logger.LogStatus("Search", "Passed");
            }
            catch (Exception)
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
                        _app.LoginDetailsPage.LogIn(ApplicationSettings.PreferredCustomer.Username, ApplicationSettings.PreferredCustomer.Password);
                        _app.State.CurrentUser.UserName = "RegisteredUserUserName";
                        _app.State.CurrentUser.Type = _criteria.UserType;
                        _app.State.CurrentUser.IsLoggedIn = true;
                        break;
                    case UserType.Guest:
                        _app.LoginDetailsPage.ContinueAsGuest("vikul", "rathod", "vrathod@tavisca.com");
                        _app.State.CurrentUser.ResetUser();
                        break;
                }
                OnSuccessLogin(requestingPage);
                _logger.LogStatus("Login", "Passed");
            }
            catch (Exception)
            {
                _logger.LogStatus("Login", "Failed");
                throw;
            }
        }

        private static void OnSuccessLogin(string requestingPage)
        {
            if (requestingPage.Equals("HomePage"))
            {
                _app.HomePage.WaitForHomePage();
                _app.State.CurrentPage = "HomePage";
            }
            else
            {
                if (ApplicationSettings.Environment == "PROD")
                    if (_app.TripFolderPage.IsLeavePopupVisible())
                        _app.ConfirmAlert();
                _app.PassengerInfoPage.WaitForPageLoad();
                _app.PassengerInfoPage.ValidateTripDetails(_selectedItineary);
                _app.State.CurrentPage = "PassengerInfoPage";
            }
        }

        internal static void AddToCart()
        {
            try
            {
                if (!_app.State.CurrentPage.EndsWith("ResultsPage"))
                    throw new InvalidOperationException("AddToCart", _app.State.CurrentPage);
                _selectedItineary = _app.ResultsPage.AddToCart(_criteria);
                if (_selectedItineary == null)
                    throw new AddToCartFailedException();
                _app.State.CurrentPage = "TripFolderPage";
                _app.TripFolderPage.ValidateTripFolder(_selectedItineary);
                _logger.LogStatus("AddToCart", "Passed");
            }
            catch (Exception)
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
            catch (Exception)
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
                _app.CheckoutPage.ValidateTripDetails(_selectedItineary);
                _app.State.CurrentPage = "CheckOutPage";
                _logger.LogStatus("ConfirmPassengerDetails", "Passed");
            }
            catch (Exception)
            {
                _logger.LogStatus("ConfirmPassengerDetails", "Failed");
                throw;
            }
        }


        #region TripFolder Calls

        public static void SaveTrip()
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("TripFolderPage"))
                    throw new InvalidOperationException("SaveTrip", _app.State.CurrentPage);
                // to implement
                _app.TripFolderPage.SaveTrip();
                //2 cases
                //1. If already logged in directly save the trip
                //2. If not, ask for login and then save the trip
                _logger.LogStatus("SaveTrip", "Passed");
            }
            catch (Exception)
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
                _app.TripFolderPage.TripStartOver();
                _logger.LogStatus("TripStartOver", "Passed");
            }
            catch (Exception)
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
                _app.TripFolderPage.EditTripName();
                _logger.LogStatus("EditTripName", "Passed");
            }
            catch (Exception)
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
            catch (Exception)
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
            catch (Exception)
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

                _app.TripFolderPage.CheckoutTrip();

                if (_app.State.CurrentUser.Type != UserType.Guest)
                {
                    _app.PassengerInfoPage.WaitForPageLoad();
                    _app.PassengerInfoPage.ValidateTripDetails(_selectedItineary);
                    _app.State.CurrentPage = "PassengerInfoPage";
                }
                else
                {
                    _app.State.CurrentPage = "LoginDetailsPage";
                }
                _logger.LogStatus("CheckoutTrip", "Passed");
            }
            catch (Exception)
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
                _app.TripFolderPage.ContinueShopping();
                _logger.LogStatus("ContinueShopping", "Passed");
            }
            catch (Exception)
            {
                _logger.LogStatus("ContinueShopping", "Failed");
                throw;
            }
        }

        #endregion


        public static void SetFilters()
        {
            try
            {
                if (!_app.State.CurrentPage.EndsWith("ResultsPage"))
                    throw new InvalidOperationException("SetFilters", _app.State.CurrentPage);
                _app.ResultsPage.SetAndValidatePostSearchFilters(_criteria.Filters.PostSearchFilters);
            }
            catch (Exception)
            {
                _logger.LogStatus("SetFilters", "Failed");
                throw;
            }

        }

        public static void PayNow()
        {
            try
            {
                if (!_app.State.CurrentPage.Equals("CheckOutPage"))
                    throw new InvalidOperationException("PayNow", _app.State.CurrentPage);

                if (!TripProductType.Equals(TripProductType.Car))
                {
                    if (_criteria.PaymentMode == PaymentMode.RoviaBucks)
                        _app.CheckoutPage.PayNow(new PaymentInfo(_criteria.PaymentMode));
                    else
                    {
                        _app.CheckoutPage.PayNow(_criteria.PaymentMode);
                        _app.BFCPaymentPage.WaitForLoad();
                        _app.State.CurrentPage = "BFCPaymentPage";
                        _app.BFCPaymentPage.PayNow(new PaymentInfo(_criteria.PaymentMode, _criteria.CardType));
                    }
                }
                else
                    _app.CheckoutPage.BookNow();
                _app.CheckoutPage.CheckPaymentStatus();
                _app.CheckoutPage.ValidateBookedProductDetails(_selectedItineary);
                _logger.LogStatus("PayNow", "Passed");
            }
            catch (Exception)
            {
                _logger.LogStatus("PayNow", "Failed");
                throw;
            }
        }

        internal static void InitializeTest()
        {
            _app.ClearBrowserCache();
            _app.State.CurrentUser.ResetUser();
            GoToHomePage();
        }

        public static void SaveScreenShot(TestContext context)
        {
            _app.SaveScreenshot(context);
        }
    }
}
