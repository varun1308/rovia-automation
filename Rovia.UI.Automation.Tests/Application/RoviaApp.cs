using System;
using System.Drawing.Imaging;
using System.Threading;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Pages;

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
                CurrentProduct = "AIR"
                
            };
        }

        public HomePage HomePage
        {
            get { return InitializePage<HomePage>("HomeControls"); }
        }

        public IResultsPage ResultsPage
        {
            get
            {
                switch (State.CurrentProduct)
                {
                    case "AIR": return InitializePage<AirResultsPage>("AirResultsControls");
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

        public void ClearBrowserCache()
        {
            Driver.Manage().Cookies.DeleteAllCookies(); //delete all cookies
            Thread.Sleep(5000); //wait 5 seconds to clear cookies.
        }
    }
}
