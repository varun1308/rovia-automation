using System;
using System.Drawing.Imaging;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Pages;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Pages.SearchPanels;

namespace Rovia.UI.Automation.Tests.Application
{
    public class RoviaApp : UIApplication
    {
        public AppState State { get; set; }

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

        public HomePage HomePage
        {
            get 
            { 
                var homePage= InitializePage<HomePage>("HomeControls");
                switch (State.CurrentProduct)
                {
                    case TripProductType.Air: homePage.SearchPanel = InitializePage<AirSearchPanel>("AirSearchPanelControls");
                        break;
                }
                return homePage;
            }
        }

        public IResultsPage ResultsPage
        {
            get
            {
                switch (State.CurrentProduct)
                {
                    case TripProductType.Air: return InitializePage<AirResultsPage>("AirResultsControls");
                    case TripProductType.Hotel: return InitializePage<HotelResultsPage>("HotelResultsControls");
                    default:
                        return null;
                }
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

        public bool SaveScreenshot(TestContext context)
        {
            try
            {
                var driver = Driver as ITakesScreenshot;

                if (driver != null)
                {
                    var screenShot = driver.GetScreenshot();

                    var fileName = AppDomain.CurrentDomain.BaseDirectory + "\\" +
                                   context.FullyQualifiedTestClassName.Replace(".", "_") + "_" + context.TestName +
                                   ".jpg";
                    screenShot.SaveAsFile(fileName, ImageFormat.Jpeg);
                }
            }
            catch
            {
            }
            return false;
        }

        
    }
}
