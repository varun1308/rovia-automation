using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Pages;
using Rovia.UI.Automation.Tests.Pages.ResultPageComponents;
using Rovia.UI.Automation.Tests.Pages.SearchPanels;

namespace Rovia.UI.Automation.Tests.Application
{
    public class RoviaApp : UIApplication
    {
        public AppState State { get; set; }

        public HomePage HomePage
        {
            get 
            { 
                var homePage= InitializePage<HomePage>("HomeControls");
                switch (State.CurrentProduct)
                {
                    case TripProductType.Air: homePage.SearchPanel = InitializePage<AirSearchPanel>("AirSearchPanelControls");
                        break;
                    case TripProductType.Hotel: homePage.SearchPanel = InitializePage<HotelSearchPanel>("HotelSearchPanelControls");
                        break;
                    case TripProductType.Car: homePage.SearchPanel = InitializePage<CarSearchPanel>("CarSearchPanelControls");
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
                        var resultHolder=InitializePage<HotelResultsHolder>("HotelResultsHolderControls");
                        resultHolder.RoomsHolder= InitializePage<HotelRoomsHolder>("HotelRoomsHolderControls");
                        resultHolder.WaitingFunction = resultsPage.WaitForResultLoad;
                        resultsPage.ResultsHolder = resultHolder;
                        resultsPage.ResultFilters = InitializePage<HotelResultFilters>("HotelResultsFiltersControls");
                        resultsPage.ResultTitle = InitializePage<HotelResultsTitle>("HotelResultsTitleControls");
                        break;
                    case TripProductType.Car:
                        resultsPage.ResultsHolder = InitializePage<CarResultHolder>("CarResultsControls");
                        resultsPage.ResultFilters=InitializePage<CarResultFilters>("CarFiltersControls");
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
            get { return InitializePage<TripFolderPage>("TripFolderControls"); }
        }

        public PassengerInfoPage PassengerInfoPage
        {
            get { return InitializePage<PassengerInfoPage>("PassengerInfoControls"); }
        }

        public CheckoutPage CheckoutPage
        {
            get { return InitializePage<CheckoutPage>("CheckoutControls"); }
        }

        public BFCPaymentPage BFCPaymentPage
        {
            get { return InitializePage<BFCPaymentPage>("BFCPaymentControls"); }
        }

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

        public bool SaveScreenshot(TestContext context)
        {
            try
            {
                var driver = Driver as ITakesScreenshot;
                var testClass = context.FullyQualifiedTestClassName.Split('.');
                if (driver != null)
                {
                    var screenShot = driver.GetScreenshot();

                    var fileName = GetDirectoryPath() +"\\"+
                                   testClass[testClass.Length - 1] + "_" + context.TestName + "_" + (context.DataRow.Table.Rows.IndexOf(context.DataRow)+1) +
                                   ".jpg";
                    screenShot.SaveAsFile(fileName, ImageFormat.Jpeg);
                    LogManager.GetInstance().LogInformation("<< Img : "+fileName + " >>");
                }
            }
            catch
            {
            }
            return false;
        }

        private string GetDirectoryPath()
        {
            var directoryPath = ApplicationSettings.LoggedFilePath + "\\Dated_" +
                                        DateTime.Now.ToShortDateString().Replace('/', '_');
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            return directoryPath;
        }

        public void SaveSessionId(TestContext context,string sessionId)
        {
            context.Properties.Add(context.TestName + "_" + (context.DataRow.Table.Rows.IndexOf(context.DataRow)+1), sessionId);
        }

        public void ClearBrowserCache()
        {

            Driver.Manage().Cookies.DeleteAllCookies(); //delete all cookies
            Thread.Sleep(5000); //wait 5 seconds to clear cookies.
        }

        public void ConfirmAlert()
        {
            try
            {
                Driver.SwitchTo().Alert().Accept();
            }
            catch (NoAlertPresentException)
            {
               
            }
        }
    }
}
