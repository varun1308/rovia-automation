﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.DataBinder
{
    public class AirCriteriaDataBinder : ICriteriaDataBinder
    {
        public SearchCriteria GetCriteria(DataRow dataRow)
        {
            var searchType = StringToEnum<SearchType>((string)dataRow["TripType"]);

            try
            {
                return new AirSearchCriteria()
                    {
                        Pipeline = (string)dataRow["ExecutionPipeline"],
                        UserType = StringToEnum<UserType>((string)dataRow["UserType"]),
                        Description = (string)dataRow["Description"],
                        AirportPairs = ParseAirPorts(dataRow["AirPortPairs"].ToString(), dataRow["TravelDates"].ToString(), searchType),
                        Passengers = new Passengers()
                            {
                                Adults = int.Parse(dataRow["Adults"].ToString()),
                                Infants = int.Parse(dataRow["Infants"].ToString()),
                                Children = int.Parse(dataRow["Children"].ToString())
                            },
                        SearchType = searchType,
                        Filters = new Filters()
                            {
                                PreSearchFilters = new AirPreSearchFilters()
                                {
                                    IncludeNearByAirPorts = bool.Parse(string.IsNullOrEmpty(dataRow["IncludeNearByAirPorts"].ToString()) ? "False" : dataRow["IncludeNearByAirPorts"].ToString()),
                                    CabinType = StringToEnum<CabinType>(string.IsNullOrEmpty(dataRow["CabinType"].ToString()) ? "Economy" : dataRow["CabinType"].ToString()),
                                    NonStopFlight = bool.Parse(string.IsNullOrEmpty(dataRow["NonStopFlight"].ToString()) ? "False" : dataRow["NonStopFlight"].ToString()),
                                    AirLines = string.IsNullOrEmpty(dataRow["AirLines"].ToString()) ? null : new List<string>(((string)dataRow["AirLines"]).Split('|'))
                                },
                                PostSearchFilters = GetPostSearchFilters(dataRow["PostFilters"].ToString(), dataRow["PostFiltersValues"].ToString())
                            },
                        PaymentMode = StringToEnum<PaymentMode>(((string)dataRow["PaymentMode"]).Split('|')[0]),
                        CardType = StringToEnum<CreditCardType>(((string)dataRow["PaymentMode"]).Contains("|") ? ((string)dataRow["PaymentMode"]).Split('|')[1] : "Visa"),
                        Supplier = dataRow["Supplier"].ToString()
                    };
            }
            catch (Exception exception)
            {
                throw new InvalidInputException("DataRow to AirCriteriaDataBinder.GetCriteria", exception);
            }
        }

        private AirPostSearchFilters GetPostSearchFilters(string filter, string value)
        {
            if (string.IsNullOrEmpty(filter))
                return null;
            var filterList = filter.Split('|');
            var valueList = value.Split('|');
            var i = 0;
            var filterCriteria = new AirPostSearchFilters();
            while (i < filterList.Length)
            {
                switch (filterList[i].ToUpper())
                {
                    case "PRICE":
                        filterCriteria.PriceRange = new PriceRange()
                        {
                            Min = int.Parse(valueList[i].Split('-')[0]),
                            Max = int.Parse(valueList[i].Split('-')[1])
                        };
                        break;

                    case "STOP":
                        filterCriteria.Stop =new List<string>(valueList[i].Split('/'));
                        break;
                    case "DURATION":
                        filterCriteria.MaxTimeDurationDiff = int.Parse(valueList[i]);
                        break;
                    case "CABIN":
                        filterCriteria.CabinTypes =new List<string>(valueList[i].Split('/')).ConvertAll(StringToEnum<CabinType>);
                        break;
                    case "AIRLINES":
                        filterCriteria.Airlines = new List<string>(valueList[i].Split('/')).ConvertAll(z => z.ToUpper());
                        break;
                    case "TAKEOFFTIME":
                        filterCriteria.TakeOffTimeRange = new TakeOffTimeRange()
                        {
                            Min = int.Parse(valueList[i].Split('-')[0]),
                            Max = int.Parse(valueList[i].Split('-')[1])
                        };
                        break;
                    case "LANDINGTIME":
                        filterCriteria.LandingTimeRange = new LandingTimeRange()
                        {
                            Min = int.Parse(valueList[i].Split('-')[0]),
                            Max = int.Parse(valueList[i].Split('-')[1])
                        };
                        break;
                    case "MATRIX":
                        filterCriteria.Matrix = new AirMatrix() { CheckMatrix = true };
                        break;
                }
                i++;
            }
            return filterCriteria;
        }

        //private List<SpecialCriteria> ParseSpecialCriteria(string criteria, string value)
        //{
        //    var criteriaList = criteria.Split('|');
        //    var values = value.Split('|');
        //    var i = 0;
        //    return criteriaList.Select(s => new AddToCartCriteria()
        //        {
        //            Name = s,
        //            Value = values[i++]
        //        }).ToList();
        //}

        private List<AirportPair> ParseAirPorts(string airPorts, string dates, SearchType tripType)
        {
            var airports = airPorts.Split('|').Select(x => x.Split('-'));
            var traveldates = dates.Split('|');
            var i = 0;
            var airPortPairs = airports.Select(airport => new AirportPair()
                {
                    DepartureAirport = airport[0],
                    ArrivalAirport = airport[1],
                    DepartureDateTime = DateTime.Now.AddDays(int.Parse(traveldates[i++]))
                }).ToList();
            if (tripType == SearchType.RoundTrip)
                airPortPairs.Add(new AirportPair()
                    {
                        DepartureDateTime = DateTime.Now.AddDays(int.Parse(traveldates[i]))
                    });
            return airPortPairs;
        }
        private static T StringToEnum<T>(string name)
        {
            return (T)Enum.Parse(typeof(T), name, true);
        }
    }
}
