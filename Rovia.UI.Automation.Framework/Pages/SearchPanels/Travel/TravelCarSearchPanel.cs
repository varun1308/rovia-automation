using Rovia.UI.Automation.Framework.Configurations;
using System.Linq;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Framework.Pages
{
    /// <summary>
    /// Travel site specific car product search methods
    /// </summary>
    public class TravelCarSearchPanel : CarSearchPanel
    {
        /// <summary>
        /// Override pickup details with respect to travel site controls
        /// </summary>
        /// <param name="carSearchCriteria"></param>
        protected override void EnterPickUpDetails(CarSearchCriteria carSearchCriteria)
        {
            if (carSearchCriteria.PickUp.PickUpType.Equals(PickUpType.Airport))
            {
                WaitAndGetBySelector("pickUpLoc", ApplicationSettings.TimeOut.Slow).SendKeys(carSearchCriteria.PickUp.PickUpLocCode);
                if (!string.IsNullOrEmpty(carSearchCriteria.PickUp.PickUpLocation)) SetOriginLocation(carSearchCriteria.PickUp.PickUpLocation);
            }
            else
            {
                WaitAndGetBySelector("pickUpCity", ApplicationSettings.TimeOut.Slow).Click();
                WaitAndGetBySelector("pickUpCityLoc", ApplicationSettings.TimeOut.Slow).SendKeys(carSearchCriteria.PickUp.PickUpLocCode);
                if (!string.IsNullOrEmpty(carSearchCriteria.PickUp.PickUpLocation)) SetOriginLocation(carSearchCriteria.PickUp.PickUpLocation);
            }
            var pickUpDate = WaitAndGetBySelector("pickUpDate", ApplicationSettings.TimeOut.Slow);
            pickUpDate.SendKeys(carSearchCriteria.PickUp.PickUpDate.ToString("MM/dd/yyyy"));
            pickUpDate.Click();
            if (string.IsNullOrEmpty(carSearchCriteria.PickUp.PickUpTime)) return;
            WaitAndGetBySelector("pickUpTimeClick", ApplicationSettings.TimeOut.Fast).Click();
            GetUIElements("PickUpTimeSelect").FirstOrDefault(x => x.Text.Equals(carSearchCriteria.PickUp.PickUpTime))
                .Click();
        }

        /// <summary>
        /// Override dropoff details with respect to travel site controls
        /// </summary>
        /// <param name="carSearchCriteria"></param>
        protected override void EnterDropOffDetails(CarSearchCriteria carSearchCriteria)
        {
            if (!carSearchCriteria.DropOff.DropOffType.Equals(DropOffType.SameAsPickUp))
            {
                if (carSearchCriteria.DropOff.DropOffType.Equals(DropOffType.Airport))
                {
                    WaitAndGetBySelector("dropoffAirport", ApplicationSettings.TimeOut.Slow).Click();
                    WaitAndGetBySelector("dropoffLoc", ApplicationSettings.TimeOut.Slow)
                        .SendKeys(carSearchCriteria.DropOff.DropOffLocCode);
                }
                else
                {
                    WaitAndGetBySelector("dropoffCity", ApplicationSettings.TimeOut.Slow).Click();
                    WaitAndGetBySelector("dropoffCityLoc", ApplicationSettings.TimeOut.Slow)
                        .SendKeys(carSearchCriteria.DropOff.DropOffLocCode);
                }
                if (!string.IsNullOrEmpty(carSearchCriteria.DropOff.DropOffLocation))
                    SetDestinationLocation(carSearchCriteria.DropOff.DropOffLocation);
            }
            else
                WaitAndGetBySelector("dropoffSameAsPickUp", ApplicationSettings.TimeOut.Slow).Click();
            var dropoffDate = WaitAndGetBySelector("dropoffDate", ApplicationSettings.TimeOut.Slow);
            dropoffDate.SendKeys(carSearchCriteria.DropOff.DropOffDate.ToString("MM/dd/yyyy"));
            dropoffDate.Click();
            if (string.IsNullOrEmpty(carSearchCriteria.DropOff.DropOffTime))return;
            WaitAndGetBySelector("dropoffTimeClick", ApplicationSettings.TimeOut.Fast).Click();
            GetUIElements("dropoffTimeSelect").FirstOrDefault(x => x.Text.Equals(carSearchCriteria.DropOff.DropOffTime)).Click();
        }
    }
}
