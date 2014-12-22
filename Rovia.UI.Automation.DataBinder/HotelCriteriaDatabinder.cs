using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.DataBinder
{
    public class HotelCriteriaDatabinder : ICriteriaDataBinder
    {
        public SearchCriteria GetCriteria(DataRow dataRow)
        {
            return new HotelSearchCriteria()
                {
                    Description = dataRow["Description"].ToString().Replace("..", ","),
                    Pipeline = dataRow["ExecutionPipeLine"].ToString(),
                    UserType = StringToEnum<UserType>((string)dataRow["UserType"]),
                    ShortLocation = dataRow["ShortLocation"].ToString(),
                    Location = dataRow["Location"].ToString().Replace("..", ","),
                    StayPeriod = new StayPeriod()
                        {
                            CheckInDate = DateTime.Now.AddDays(int.Parse(dataRow["CheckInDate"].ToString())),
                            CheckOutDate = DateTime.Now.AddDays(int.Parse(dataRow["CheckOutDate"].ToString()))
                        },
                    Passengers = new Passengers()
                        {
                            Adults = int.Parse(dataRow["Adults"].ToString()),
                            ChildrenAges = string.IsNullOrEmpty(dataRow["ChildrenAges"].ToString()) ? null : dataRow["ChildrenAges"].ToString().Split('|').ToList()
                        },
                    Filters = new Filters()
                        {
                            PreSearchFilters = new HotelPreSearchFilters()
                                {
                                    AdditionalPreferences = string.IsNullOrEmpty(dataRow["ChildrenAges"].ToString()) ? null : dataRow["AdditionalPreferences"].ToString().Split('|').ToList(),
                                    HotelName = dataRow["HotelName"].ToString().Replace("..", ","),
                                    StarRating = dataRow["StarRating"].ToString()
                                },
                            PostSearchFilters = GetPostSearchFilters(dataRow["PostSearchFilters"].ToString(), dataRow["PostSearchFilterValues"].ToString())
                        },
                    Supplier = dataRow["Supplier"].ToString()
                };
        }

        private HotelPostSearchFilters GetPostSearchFilters(string filters, string value)
        {
            if (string.IsNullOrEmpty(filters))
                return null;
            var filterList = filters.Split('|');
            var valueList = value.Split('|');
            var i = 0;
            var filterCriteria = new HotelPostSearchFilters();
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
                    case "HOTELNAME":
                        filterCriteria.HotelName = valueList[i];
                        break;
                    case "DURATION":
                        var ratings = Array.ConvertAll(valueList[i].Split('-'), int.Parse);
                        filterCriteria.RatingRange = new RatingRange()
                            {
                                From = ratings[0],
                                To = ratings[1]
                            };
                        break;
                    case "AMENITIES":
                        filterCriteria.Amenities = new List<string>(valueList[i].Split('/'));
                        break;
                    case "PREFFEREDLOCATION":
                        var prefLocation = valueList[i].Split('-');
                        filterCriteria.PreferredLocation = new Tuple<string, string>(prefLocation[0], prefLocation[1]);
                        break;
                    case "DISTANCERANGE":
                        var distRange = Array.ConvertAll(valueList[i].Split('-'), int.Parse);
                        filterCriteria.DistanceRange = new DistanceRange()
                        {
                            Min = distRange[0],
                            Max = distRange[1]
                        };
                        break;
                }
                i++;
            }
            return filterCriteria;
        }

        private static T StringToEnum<T>(string name)
        {
            return (T)Enum.Parse(typeof(T), name, true);
        }
    }
}
