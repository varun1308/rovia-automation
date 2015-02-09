namespace Rovia.UI.Automation.Tests.Application
{
    using System;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Threading;
    using AppacitiveAutomationFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using Logger;
    using ScenarioObjects;
    using Configuration;
    using Pages;
    using Pages.Parsers;
    using Pages.ResultPageComponents;
    using Pages.ResultPageComponents.Activity;
    using Pages.SearchPanels;

    /// <summary>
    /// Class to intialize driver configuration and UI controls
    /// </summary>
    public class RoviaApp : UIApplication
    {
        #region Public Properties

        public AppState State { get; set; }

        #endregion

        #region Private Members

        private static string GetDirectoryPath()
        {
            var directoryPath = ApplicationSettings.LoggedFilePath + "\\Dated_" +
                                        DateTime.Now.ToShortDateString().Replace('/', '_');
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            return directoryPath;
        }

        #endregion

        #region Public Properties

        public virtual HomePage HomePage
        {
            get
            {
                var homePage = InitializePage<HomePage>("RoviaHomeControls");
                switch (State.CurrentProduct)
                {
                    case TripProductType.Air: homePage.SearchPanel = InitializePage<AirSearchPanel>("RoviaAirSearchPanelControls");
                        break;
                    case TripProductType.Hotel: homePage.SearchPanel = InitializePage<HotelSearchPanel>("RoviaHotelSearchPanelControls");
                        break;
                    case TripProductType.Car: homePage.SearchPanel = InitializePage<CarSearchPanel>("RoviaCarSearchPanelControls");
                        break;
                    case TripProductType.Activity: homePage.SearchPanel = InitializePage<ActivitySearchPanel>("RoviaActivitySearchPanelControls");
                        break;
                }
                return homePage;
            }
        }

        public ResultsPage ResultsPage
        {
            get
            {
                var resultsPage = InitializePage<ResultsPage>("ResultControls");
                switch (State.CurrentProduct)
                {
                    case TripProductType.Air:
                        resultsPage.ResultsHolder = InitializePage<AirResultsHolder>("AirResultsHolderControls");
                        resultsPage.ResultFilters = InitializePage<AirResultFilters>("AirResultsFiltersControls");
                        break;
                    case TripProductType.Hotel:
                        var hotelResultHolder = InitializePage<HotelResultsHolder>("HotelResultsHolderControls");
                        hotelResultHolder.RoomsHolder = InitializePage<HotelRoomsHolder>("HotelRoomsHolderControls");
                        resultsPage.ResultsHolder = hotelResultHolder;
                        resultsPage.ResultFilters = InitializePage<HotelResultFilters>("HotelResultsFiltersControls");
                        resultsPage.ResultTitle = InitializePage<HotelResultsTitle>("HotelResultsTitleControls");
                        break;
                    case TripProductType.Car:
                        resultsPage.ResultsHolder = InitializePage<CarResultHolder>("CarResultsHolderControls");
                        resultsPage.ResultFilters = InitializePage<CarResultFilters>("CarFiltersControls");
                        break;
                    case TripProductType.Activity:
                        var activityResultHolder = InitializePage<ActivityResultsHolder>("ActivityResultsHolderControls");
                        activityResultHolder.ActivityHolder = InitializePage<ActivityHolder>("ActivityHolderControls");
                        resultsPage.ResultsHolder = activityResultHolder;
                        resultsPage.ResultFilters = InitializePage<ActivityResultFilters>("ActivityResultsFiltersControls");
                        //resultsPage.ResultFilters = InitializePage<ActivityResultFilters>("CarFiltersControls");
                        break;
                    default:
                        return null;
                }
                return resultsPage;
            }
        }

        public LoginDetailsPage LoginDetailsPage
        {
            get { return InitializePage<LoginDetailsPage>("LoginDetailsControls"); }
        }

        public TripFolderPage TripFolderPage
        {
            get
            {
                var tripFolderPage = InitializePage<TripFolderPage>("TripFolderControls");
                tripFolderPage.TripProductParser = InitializePage<TripFolderParser>("TripFolderParserControls");
                return tripFolderPage;
            }
        }

        public PassengerInfoPage PassengerInfoPage
        {
            get
            {
                var paxInfoPage = InitializePage<PassengerInfoPage>("PassengerInfoControls");
                paxInfoPage.TripProductHolder = InitializePage<TripProductHolder>("PassengerInfoTripProductHolderControls");
                return paxInfoPage;
            }
        }

        public CheckoutPage CheckoutPage
        {
            get
            {
                var checkOutPage = InitializePage<CheckoutPage>("CheckoutControls");
                checkOutPage.TripProductHolder = InitializePage<TripProductHolder>("CheckoutTripProductHolderControls");
                return checkOutPage;
            }
        }

        public BFCPaymentPage BFCPaymentPage
        {
            get { return InitializePage<BFCPaymentPage>("BFCPaymentControls"); }
        }

        #endregion

        #region Public Members

        public RoviaApp()
        {
            State = new AppState()
            {
                CurrentUser = new User()
                {
                    Type = UserType.Guest,
                    IsLoggedIn = false
                },
                CurrentProduct = TripProductType.Air
            };
        }

        /// <summary>
        /// Save UI screen shot with default test names
        /// </summary>
        /// <param name="context">TestContext Instance</param>
        public void SaveScreenshot(TestContext context)
        {
            try
            {
                var driver = Driver as ITakesScreenshot;
                var testClass = context.FullyQualifiedTestClassName.Split('.');
                if (driver == null) return;
                var screenShot = driver.GetScreenshot();

                var fileName = GetDirectoryPath() + "\\" +
                               testClass[testClass.Length - 1] + "_" + context.TestName + "_" +
                               (context.DataRow.Table.Rows.IndexOf(context.DataRow) + 1) +
                               ".jpg";
                screenShot.SaveAsFile(fileName, ImageFormat.Jpeg);
                LogManager.GetInstance().LogInformation("$$ " + fileName + " $$");
            }
            catch (Exception ex)
            {
                LogManager.GetInstance().LogWarning("Error in taking screenshot: Driver freezed due to Rovia Award Popup.");
            }
        }

        /// <summary>
        /// Save UI scrren shot with custom image name
        /// </summary>
        /// <param name="name"></param>
        public void SaveScreenshot(string name)
        {
            var driver = Driver as ITakesScreenshot;
            if (driver == null) return;
            var screenShot = driver.GetScreenshot();

            var fileName = GetDirectoryPath() + "\\" + name + ".jpg";
            screenShot.SaveAsFile(fileName, ImageFormat.Jpeg);
        }

        /// <summary>
        /// Clear browser cache explicitly 
        /// </summary>
        public void ClearBrowserCache()
        {

            Driver.Manage().Cookies.DeleteAllCookies(); //delete all cookies
            Thread.Sleep(5000); //wait 5 seconds to clear cookies.
        }

        /// <summary>
        /// Confirm unwanted alert coming on page those are freezing browser
        /// </summary>
        public void ConfirmAlert()
        {
            try
            {
                Driver.SwitchTo().Alert().Accept();
            }
            catch (NoAlertPresentException) { }
        }

        #endregion
    }
}
