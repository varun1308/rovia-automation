

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    using System;
    using System.Linq;
    using AppacitiveAutomationFramework;
    using Criteria;
    using Configuration;

    /// <summary>
    /// Class contains properties of results page title
    /// </summary>
    public class HotelResultsTitle : UIPage, IResultsTitle
    {
        #region Private Fields

        private int _adults;
        private int _children;
        private DateTime _checkInDate;
        private DateTime _checkOutDate;
        private string _location;

        #endregion

        private void ParseTitle()
        {
            _location = WaitAndGetBySelector("locationHeader", ApplicationSettings.TimeOut.Fast).Text;
            var subtitle = WaitAndGetBySelector("dateAndPasanngers", ApplicationSettings.TimeOut.Fast).Text.Split('|').Select(x => x.Trim()).ToList();
            var dates = subtitle[0].Split('-');
            _checkInDate = DateTime.Parse(dates[0]);
            _checkOutDate = DateTime.Parse(dates[1]);
            _adults = int.Parse(subtitle[1][0].ToString());
            if (subtitle.Count == 3)
                _children = int.Parse(subtitle[2][0].ToString());
        }

        /// <summary>
        /// Validate title fields if visible and valid
        /// </summary>
        /// <param name="criteria">hotel serach criteria object</param>
        /// <returns></returns>
        public bool ValidateTitle(SearchCriteria criteria)
        {
            ParseTitle();
            var hotelCriteria = criteria as HotelSearchCriteria;
            return _adults == hotelCriteria.Passengers.Adults
                   && _children == hotelCriteria.Passengers.Children
                //&& _location.Equals(hotelCriteria.Location)
                   && _checkInDate.Date == hotelCriteria.StayPeriod.CheckInDate.Date
                   && _checkOutDate.Date == hotelCriteria.StayPeriod.CheckOutDate.Date;
        }
    }
}
