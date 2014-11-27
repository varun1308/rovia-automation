using System;
using System.Linq;

namespace Rovia.UI.Automation.Tests.Utility
{
    public static class TestExecutionPipeLine
    {
        public static void Execute( string pipeline)
        {
            pipeline.Split('|').ToList().ForEach(x =>
                {
                    switch (x.ToUpper())
                    {
                        case "LOGIN":
                            TestHelper.Login();
                            break;
                        case "SEARCH":
                            TestHelper.Search();
                            break;
                        case "ADDTOCART":
                            TestHelper.AddToCart();
                            break;
                        case "CHECKOUTTRIP":
                            TestHelper.CheckoutTrip();
                            break;
                        case "EDITPAXINFO":
                            TestHelper.EditPassengerInfoAndContinue();
                            break;
                        case "ENTERPAXINFO":
                            TestHelper.EnterPassengerDetails();
                            break;
                        case "CONFIRMPAXINFO":
                            TestHelper.ConfirmPassengerDetails();
                            break;
                        case "SAVETRIP":
                            TestHelper.SaveTrip();
                            break;
                        case "TRIPSTARTOVER":
                            TestHelper.TripStartOver();
                            break; 
                        case "CONTINUESHOPPING":
                            TestHelper.ContinueShopping();
                            break;
                        case "SETFILTERS":
                            TestHelper.SetFilters();
                            break;
                        case "SETMATRIX":
                            TestHelper.SetMatrix();
                            break;
                        case "PAYNOW":
                            TestHelper.PayNow();
                            break;
                        default:throw new Exception(string.Format("Invalid Operation {0} in Pipeline",x));
                    }
                });
        }
    }
}
