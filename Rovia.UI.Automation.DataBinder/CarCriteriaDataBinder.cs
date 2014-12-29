using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.DataBinder
{
    public class CarCriteriaDataBinder : ICriteriaDataBinder
    {
        #region Private Members

        private PickUp ParsePickUpDetails(string pickUp, string location, string travelDates)
        {
            try
            {
                var pick = pickUp.Split('-');
                var pickUpDateTimeTime = travelDates.Split('|')[0].Split(',');
                return new PickUp()
                {
                    PickUpType = StringToEnum<PickUpType>(pick[0]),
                    PickUpLocCode = pick[1],
                    PickUpDate = DateTime.Now.AddDays(int.Parse(pickUpDateTimeTime[0])),
                    PickUpTime = pickUpDateTimeTime.Length == 2 ? pickUpDateTimeTime[1] : "Anytime",
                    PickUpLocation = location
                };
            }
            catch (Exception exception)
            {
                throw new InvalidInputException("Input PickUp details not in format ", exception);
            }
        }

        private DropOff ParseDropOffDetails(string dropOff, string location, string travelDates)
        {
            try
            {
                var drop = dropOff.Split('-');
                var dropoffDateTime = travelDates.Split('|')[1].Split(',');
                return new DropOff()
                {
                    DropOffType = StringToEnum<DropOffType>(drop[0]),
                    DropOffLocCode = drop.Length > 1 && !string.IsNullOrEmpty(drop[1]) ? drop[1] : string.Empty,
                    DropOffDate = DateTime.Now.AddDays(int.Parse(dropoffDateTime[0])),
                    DropOffTime = dropoffDateTime.Length == 2 ? dropoffDateTime[1] : "Anytime",
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

        private List<CorporateDiscount> ParseCorporateDiscount(string rentalAgency, string corpDiscCode, string promotionalCode)
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
                        RentalAgency = string.IsNullOrEmpty(rentalAgencies[i]) ? string.Empty : rentalAgencies[i],
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
            if (!string.IsNullOrEmpty(transmission))
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
                        },
                        PostSearchFilters = GetPostSearchFilters(dataRow["PostFilters"].ToString(), dataRow["PostFiltersValues"].ToString())
                    },
                    Passengers = new Passengers() { Adults = 1 }
                };
            }
            catch (Exception exception)
            {
                throw new InvalidInputException("DataRow to CarCriteriaDataBinder.GetCriteria", exception);
            }
        }

        private CarPostSearchFilters GetPostSearchFilters(string filter, string value)
        {
            if (string.IsNullOrEmpty(filter))
                return null;
            var filterList = filter.Split('|');
            var valueList = value.Split('|');
            var i = 0;
            var carfilterCriteria = new CarPostSearchFilters();
            while (i < filterList.Length)
            {
                switch (filterList[i].ToUpper())
                {
                    case "PRICE":
                        carfilterCriteria.PriceRange = new PriceRange()
                            {
                                Min = int.Parse(valueList[i].Split('-')[0]),
                                Max = int.Parse(valueList[i].Split('-')[1])
                            };
                        break;
                    case "LOCATIONS":
                        carfilterCriteria.LocationValues = new List<string>(valueList[i].Split('/'));
                        break;
                    case "CARTYPE":
                        carfilterCriteria.CarTypes = new List<string>(valueList[i].Split('/'));
                        break;
                    case "RENTALAGENCY":
                        carfilterCriteria.RentalAgency = new List<string>(valueList[i].Split('/'));
                        break;
                    case "CAROPTIONS":
                        carfilterCriteria.CarOptions = new List<string>(valueList[i].Split('/'));
                        break;
                    case "MATRIX":
                        carfilterCriteria.Matrix = new CarMatrix() { CheckMatrix = true };
                        break;
                    default: throw new InvalidInputException("Invalid filter keyword : " + filterList[i]);
                }
                i++;
            }
            return carfilterCriteria;
        }
    }
}
