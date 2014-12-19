using System;
using System.Collections.Generic;
using System.Data;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.DataBinder
{
    public class CarCriteriaDataBinder : ICriteriaDataBinder
    {
        #region Private Members

        private PickUp ParsePickUpDetails(string pickUp,string location, string travelDates)
        {
            try
            {
                var pick = pickUp.Split('-');
                var pickUpTime = DateTime.Now.AddDays(int.Parse(travelDates.Split('|')[0]));
                return new PickUp()
                {
                    PickUpType = StringToEnum<PickUpType>(pick[0]),
                    PickUpLocCode = pick[1],
                    PickUpTime = pickUpTime,
                    PickUpLocation = location
                };
            }
            catch (Exception exception)
            {
                throw new InvalidInputException("Input PickUp details not in format ", exception);
            }
        }

        private DropOff ParseDropOffDetails(string dropOff,string location, string travelDates)
        {
            try
            {
                var drop = dropOff.Split('-');
                var dropoffTime = DateTime.Now.AddDays(int.Parse(travelDates.Split('|')[1]));
                return new DropOff()
                {
                    DropOffType = StringToEnum<DropOffType>(drop[0]),
                    DropOffLocCode = drop.Length > 1 && !string.IsNullOrEmpty(drop[1]) ? drop[1]: string.Empty,
                    DropOffTime = dropoffTime,
                    DropOffLocation = location
                };
            }
            catch (Exception exception)
            {
                throw new InvalidInputException("Input PickUp details not in format ", exception);
            }
        }

        private static T StringToEnum<T>(string name)
        {
            return (T)Enum.Parse(typeof(T), name, true);
        }

        private List<CorporateDiscount> ParseCorporateDiscount(string rentalAgency,string corpDiscCode,string promotionalCode)
        {
            try
            {
                var rentalAgencies = rentalAgency.Split('|');
                var corpDiscCodes = corpDiscCode.Split('|');
                var promotionalCodes = promotionalCode.Split('|');
                var corpDiscountList = new List<CorporateDiscount>();
                var i = 0;
                while (i < rentalAgencies.Length)
                {
                    corpDiscountList.Add(new CorporateDiscount()
                    {
                        RentalAgency = string.IsNullOrEmpty(rentalAgencies[i])?string.Empty:rentalAgencies[i],
                        CorpDiscountCode = string.IsNullOrEmpty(corpDiscCodes[i]) ? string.Empty : corpDiscCodes[i],
                        PromotionalCode = string.IsNullOrEmpty(promotionalCodes[i]) ? string.Empty : promotionalCodes[i],
                    });
                    i++;
                }
                return corpDiscountList;
            }
            catch (Exception exception)
            {
                throw new InvalidInputException("CorporateDiscount details not in format ", exception);
            }
        }

        private int ParseTransmission(string transmission)
        {
            if(!string.IsNullOrEmpty(transmission))
            switch (transmission.ToUpper())
            {
                case "AUTOMATIC":
                    return 1;
                case "MANUAL":
                    return 2;
            }
            return 0;
        }

        private int ParseAirContioningPreference(string isairconditioned)
        {
            if (string.IsNullOrEmpty(isairconditioned)) return 0;
            return bool.Parse(isairconditioned) ? 1 : 2;
        }

        #endregion
        
        public SearchCriteria GetCriteria(DataRow dataRow)
        {
            try
            {
                return new CarSearchCriteria()
                {
                    Description = (string)dataRow["Description"],
                    Pipeline = (string)dataRow["ExecutionPipeline"],
                    UserType = StringToEnum<UserType>((string)dataRow["UserType"]),
                    PickUp = ParsePickUpDetails(dataRow["PickUpType-Location"].ToString(), dataRow["OriginLocation"].ToString(), dataRow["TravelDates"].ToString()),
                    DropOff = ParseDropOffDetails(dataRow["DropOffType-Location"].ToString(), dataRow["DestinationLocation"].ToString(), dataRow["TravelDates"].ToString()),
                    Filters = new Filters()
                    {
                        PreSearchFilters = new CarPreSearchFilters()
                        {
                            RentalAgency = dataRow["RentalAgency"].ToString(),
                            AirConditioning = ParseAirContioningPreference(dataRow["AirConditioning"].ToString()),
                            CarType = dataRow["CarType"].ToString(),
                            Transmission = ParseTransmission(dataRow["Transmission"].ToString()),
                            CorporateDiscount = ParseCorporateDiscount(dataRow["CorpDiscRentalAgency"].ToString(), dataRow["CorpDiscCode"].ToString(), dataRow["CorpDiscPromotionalCode"].ToString())
                        }
                    },
                    Passengers = new Passengers(){Adults = 1}
                };
            }
            catch (Exception exception)
            {
                throw new InvalidInputException("DataRow to CarCriteriaDataBinder.GetCriteria", exception);
            }
        }
    }
}
