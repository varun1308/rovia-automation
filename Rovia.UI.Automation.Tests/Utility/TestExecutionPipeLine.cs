using System;
using System.Linq;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using InvalidOperationException = Rovia.UI.Automation.Exceptions.InvalidOperationException;

namespace Rovia.UI.Automation.Tests.Utility
{
    public static class TestExecutionPipeLine
    {
        public static void Execute( string pipeline)
        {
            try
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
                                case "PAYNOW":
                                    TestHelper.PayNow();
                                    break;
                                default: throw new InvalidOperationException(string.Format("{0} in Pipeline", x));
                            }
                        });
            }
            catch (Exception exception)
            {
                LogManager.GetInstance().LogError(exception);
                throw;
            }
        }
    }
}
