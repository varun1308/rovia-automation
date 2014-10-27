using System;
using System.Drawing.Imaging;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Rovia.UI.Automation.Tests.Pages;

namespace Rovia.UI.Automation.Tests.Application
{
    public class RoviaApp : UIApplication
    {
        public HomePage HomePage
        {
            get { return InitializePage<HomePage>("HomeControls"); }
        }

        public AirResultsPage AirResultsPage
        {
            get { return InitializePage<AirResultsPage>("AirResultsControls"); }
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
