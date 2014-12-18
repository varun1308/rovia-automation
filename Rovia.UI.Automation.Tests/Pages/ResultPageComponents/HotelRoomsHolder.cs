using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    internal class HotelRoomsHolder : UIPage
    {
        public void WaitForLoad()
        {
            try
            {
                IUIWebElement webElement;
                do
                {
                    webElement = WaitAndGetBySelector("hotelDetailGallery", ApplicationSettings.TimeOut.Slow);
                } while (webElement == null || !webElement.Displayed);
                while (WaitAndGetBySelector("progressBar", ApplicationSettings.TimeOut.Fast).Displayed) ;
                webElement = WaitAndGetBySelector("divNoRoomsAvailable", ApplicationSettings.TimeOut.Fast);
                if (webElement!=null && webElement.Displayed)
                    throw new Alert(webElement.Text);
                webElement = WaitAndGetBySelector("tableHotelRooms", ApplicationSettings.TimeOut.Fast);
                if (webElement == null || !webElement.Displayed)
                    throw new Exception("Rooms Didn't Load");
            }
            catch (Exception exception)
            {
                throw new PageLoadFailed("HotelRoomPage", exception);
            }
        }

        public void BackToResults()
        {
            WaitAndGetBySelector("backToResults",Configuration.ApplicationSettings.TimeOut.Slow).Click();
        }

        internal Results AddToCart(string supplier)
        {
            try
            {
                var moreRooms = WaitAndGetBySelector("showMoreRooms", ApplicationSettings.TimeOut.Fast);
                if(moreRooms!=null && moreRooms.Displayed)
                    moreRooms.Click();
                var suppliers = GetUIElements("suppliers")
                       .Select(x=>x.GetAttribute("title") ).ToArray();
                var validIndices = Enumerable.Range(0, suppliers.Count());
                if(supplier!=null)
                    validIndices=validIndices.TakeWhile(x=>suppliers[x].Equals(supplier));
                var addToCartBtn = GetUIElements("btnAddToCart");
                var hotelTitle = WaitAndGetBySelector("hotelTitle", ApplicationSettings.TimeOut.Fast).Text;
                var hotelAddress = WaitAndGetBySelector("hotelAddress", ApplicationSettings.TimeOut.Fast).Text;
                var hotelRating = GetUIElements("hotelRating").Count;
                var roomDetatils = GetUIElements("roomDetails").Select(x => x.Text).ToArray();
                var roomType = roomDetatils.Where((x, i) => i % 2 == 0);
                var roomPrice = roomDetatils.Where((x, i) => i % 2 == 1);
                var selectedRoomIndex = validIndices.First(i => AddToCart(addToCartBtn.ElementAt(i)));
                return new HotelResult()
                    {
                        RoomSupplier = suppliers[selectedRoomIndex],
                        HotelName = hotelTitle,
                        HotelAddress = hotelAddress,
                        HotelRating = hotelRating,
                        RoomType = roomType.ElementAt(selectedRoomIndex),
                        RoomPrice = new Amount(roomPrice.ElementAt(selectedRoomIndex))
                    };
            }
            catch (System.InvalidOperationException exception)
            {
                LogManager.GetInstance().LogWarning("No Room Could Be Added");
                throw new AddToCartFailedException();
            }
        }

        private bool AddToCart(IUIWebElement btnAddToCart)
        {
            btnAddToCart.Click();
            var divloader = WaitAndGetBySelector("divLoader", ApplicationSettings.TimeOut.Fast);
            try
            {
                while (true)
                {
                    GetUIElements("alerts").ForEach(x =>
                    {
                        if (x.Displayed)
                            throw new Alert(x.Text);
                    });

                    Thread.Sleep(1000);
                }
            }
            catch (Exception exception)
            {
                LogManager.GetInstance().LogWarning(exception.Message);
            }
            var btnCheckOut = WaitAndGetBySelector("btnCheckOut", ApplicationSettings.TimeOut.Slow);
            if (btnCheckOut != null && btnCheckOut.Displayed)
            {
                btnCheckOut.Click();
                return true;
            }

            WaitAndGetBySelector("btnCancel", ApplicationSettings.TimeOut.Slow).Click();
            return false;
        }
    }
}
