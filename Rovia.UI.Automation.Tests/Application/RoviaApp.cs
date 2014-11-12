using System;
using System.Drawing.Imaging;
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
                }
            };
        }

        public HomePage HomePage
        {
            get { return InitializePage<HomePage>("HomeControls"); }
        }

        public AirResultsPage AirResultsPage
        {
            get { return InitializePage<AirResultsPage>("AirResultsControls"); }
        }

        public LoginDetailsPage LoginDetailsPage
        {
            get { return InitializePage<LoginDetailsPage>("LoginDetailsControls"); }
        }

        public TripFolderPage TripFolderPage
        {
            get { return InitializePage<TripFolderPage>("AirResultsControls"); }
        }

        public PassengerInfoPage PassengerInfoPage
        {
            get { return InitializePage<PassengerInfoPage>("PassengerInfoControls"); }
        }

        public CheckoutPage CheckoutPage
        {
            get { return InitializePage<CheckoutPage>("CheckoutControls"); }
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

        public void GoToHomePage()
        {
            Launch(ApplicationSettings.Url);
        }
    }
}
