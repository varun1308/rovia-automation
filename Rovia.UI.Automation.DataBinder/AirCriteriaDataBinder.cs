using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.DataBinder
{
    public class AirCriteriaDataBinder:ICriteriaDataBinder
    {
        public SearchCriteria GetCriteria(DataRow dataRow)
        {
            var searchType = StringToEnum<SearchType>((string) dataRow["TripType"]);
        
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
                            IncludeNearByAirPorts = (bool)dataRow["IncludeNearByAirPorts"],
                            CabinType = StringToEnum<CabinType>((string)dataRow["CabinType"]),
                            NonStopFlight = (bool)dataRow["NonStopFlight"],
                            AirLines = string.IsNullOrEmpty(dataRow["AirLines"].ToString()) ? null : new List<string>(((string)dataRow["AirLines"]).Split('|'))
                        }
                    },
                PaymentMode = StringToEnum<PaymentMode>(((string)dataRow["PaymentMode"]).Split('|')[0]),
                CardType = StringToEnum<CreditCardType>(((string)dataRow["PaymentMode"]).Contains("|")?((string)dataRow["PaymentMode"]).Split('|')[1]:"Visa"),
                SpecialCriteria = string.IsNullOrEmpty(dataRow["SpecialFilterName"].ToString()) ? null : ParseSpecialCriteria((string)dataRow["SpecialFilterName"], (string)dataRow["SpecialFilterValues"])
            };
        }

        private List<SpecialCriteria> ParseSpecialCriteria(string criteria, string value)
        {
            var criteriaList = criteria.Split('|');
            var values = value.Split('|');
            var i = 0;
            return criteriaList.Select(s => new SpecialCriteria()
                {
                    Name = s, Value = values[i++]
                }).ToList();
        }

        private List<AirportPair> ParseAirPorts(string airPorts, string dates, SearchType tripType)
        {
            var airports = airPorts.Split('|').Select(x => x.Split('-'));
            var traveldates = dates.Split('|');
            var i = 0;
            var airPortPairs = airports.Select(airport => new AirportPair()
                {
                    DepartureAirport = airport[0], ArrivalAirport = airport[1], DepartureDateTime = DateTime.Now.AddDays(int.Parse(traveldates[i++]))
                }).ToList();
            if (tripType==SearchType.RoundTrip)
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
