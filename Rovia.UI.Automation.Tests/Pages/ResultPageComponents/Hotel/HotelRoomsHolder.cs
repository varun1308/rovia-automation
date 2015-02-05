namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    using System;
    using System.Linq;
    using System.Threading;
    using AppacitiveAutomationFramework;
    using Exceptions;
    using Logger;
    using ScenarioObjects;
    using Configuration;
    using ScenarioObjects.Hotel;

    /// <summary>
    /// Hotel rooms details page
    /// </summary>
    internal class HotelRoomsHolder : UIPage
    {
        private HotelResult _addedResult;

        #region Private Members

        private bool AddToCart(IUIWebElement btnAddToCart)
        {
            btnAddToCart.Click();
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
                _addedResult = ParseResultFromCart();
                btnCheckOut.Click();
                return true;
            }
            WaitAndGetBySelector("btnCancel", ApplicationSettings.TimeOut.Slow).Click();
            return false;
        }

        private HotelResult ParseResultFromCart()
        {
            var stayInfo = GetUIElements("stayInfo").Select(x => x.Text).ToArray();
            return new HotelResult()
            {
                Amount = new Amount(WaitAndGetBySelector("cartPrice", ApplicationSettings.TimeOut.Fast).Text),
                Passengers = new Passengers(WaitAndGetBySelector("passengerInfo", ApplicationSettings.TimeOut.Fast).Text.Replace("For", "")),
                HotelName = WaitAndGetBySelector("hotelName", ApplicationSettings.TimeOut.Fast).Text,
                HotelAddress = WaitAndGetBySelector("hotelAddress", ApplicationSettings.TimeOut.Fast).Text,
                StayPeriod = new StayPeriod()
                {
                    CheckInDate = DateTime.Parse(stayInfo[0]),
                    CheckOutDate = DateTime.Parse(stayInfo[1])
                },
                SelectedRoom = new HotelRoom()
                {
                    NoOfRooms = int.Parse(stayInfo[2].Trim()[0].ToString()),
                    Descriptions = WaitAndGetBySelector("roomDescription", ApplicationSettings.TimeOut.Fast).Text
                }
            };
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Wait for hotel rooms page to load
        /// </summary>
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
                if (webElement != null && webElement.Displayed)
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

        /// <summary>
        /// Back to Hotel result page from hotel details page
        /// </summary>
        public void BackToResults()
        {
            WaitAndGetBySelector("backToResults", ApplicationSettings.TimeOut.Slow).Click();
        }

        /// <summary>
        /// Add hotel room to cart as per search criteria
        /// </summary>
        /// <param name="supplier">supplier name</param>
        /// <returns></returns>
        internal Results AddToCart(string supplier)
        {
            try
            {
                var moreRooms = WaitAndGetBySelector("showMoreRooms", ApplicationSettings.TimeOut.Fast);
                if (moreRooms != null && moreRooms.Displayed)
                    moreRooms.Click();
                var suppliers = GetUIElements("suppliers")
                       .Select(x => x.GetAttribute("title")).ToArray();
                var validIndices = Enumerable.Range(0, suppliers.Count());
                if (supplier != null)
                    validIndices = validIndices.TakeWhile(x => suppliers[x].Equals(supplier));
                var addToCartBtn = GetUIElements("btnAddToCart");
                var hotelRating = GetUIElements("hotelRating").Count;
                var roomDetatils = GetUIElements("roomDetails").Select(x => x.Text).ToArray();
                var roomType = roomDetatils.Where((x, i) => i % 2 == 0);
                var roomPrice = roomDetatils.Where((x, i) => i % 2 == 1);
                var selectedRoomIndex = validIndices.First(i => AddToCart(addToCartBtn.ElementAt(i)));
                _addedResult.RoomSupplier = suppliers[selectedRoomIndex];
                _addedResult.HotelRating = hotelRating;
                _addedResult.SelectedRoom.RoomPrice = new Amount(roomPrice.ElementAt(selectedRoomIndex));
                _addedResult.SelectedRoom.Descriptions = roomType.ElementAt(selectedRoomIndex);
                return _addedResult;
            }
            catch (System.InvalidOperationException)
            {
                LogManager.GetInstance().LogWarning("No Room Could Be Added");
                throw new AddToCartFailedException();
            }
        }

        #endregion
    }
}
