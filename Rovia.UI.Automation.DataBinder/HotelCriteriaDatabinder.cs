﻿namespace Rovia.UI.Automation.DataBinder
{
    using Criteria;
    using Exceptions;
    using ScenarioObjects;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    /// <summary>
    /// Hotel Search Criteria data binder
    /// </summary>
    public class HotelCriteriaDatabinder : ICriteriaDataBinder
    {
        #region Private Members

        private static PaymentMode GetPaymentMode(string paymentMode)
        {
            return string.IsNullOrEmpty(paymentMode) ? PaymentMode.CreditCard : StringToEnum<PaymentMode>(paymentMode);
        }

        private static HotelPostSearchFilters GetPostSearchFilters(string filters, string value)
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
                    case "RATING":
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
                    case "MATRIX":
                        filterCriteria.Matrix = new HotelMatrix()
                        {
                            Rating = int.Parse(valueList[i])
                        };
                        break;
                    case "SORT":
                        filterCriteria.SortBy = StringToEnum<SortBy>(valueList[i]);
                        break;
                    default: throw new InvalidInputException(valueList[i] + " to HotelDataBinder");
                }
                i++;
            }
            return filterCriteria;
        }

        private static T StringToEnum<T>(string name)
        {
            return (T)Enum.Parse(typeof(T), name, true);
        }

        #endregion

        #region Public Members
        /// <summary>
        /// Hotel Product Search Criteria Data Binder
        /// </summary>
        /// <param name="dataRow">Input datasheet row</param>
        /// <returns>Hotel Search Criteria Object</returns>
        public SearchCriteria GetCriteria(DataRow dataRow)
        {
            return new HotelSearchCriteria()
                {
                    Description = dataRow["Description"].ToString().Replace("..", ","),
                    Pipeline = dataRow["ExecutionPipeLine"].ToString(),
                    UserType = StringToEnum<UserType>((string)dataRow["UserType"]),
                    ShortLocation = dataRow["ShortLocation"].ToString().Replace("..", ","),
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
                                    AdditionalPreferences = string.IsNullOrEmpty(dataRow["AdditionalPreferences"].ToString()) ? null : dataRow["AdditionalPreferences"].ToString().Split('|').ToList(),
                                    HotelName = dataRow["HotelName"].ToString().Replace("..", ","),
                                    StarRating = dataRow["StarRating"].ToString()
                                },
                            PostSearchFilters = GetPostSearchFilters(dataRow["PostSearchFilters"].ToString(), dataRow["PostSearchFilterValues"].ToString())
                        },
                    Supplier = dataRow["Supplier"].ToString(),
                    PaymentMode = GetPaymentMode(dataRow["PaymentMode"].ToString())
                };
        }

        #endregion

    }
}
