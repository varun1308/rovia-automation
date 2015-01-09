using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.DataBinder
{
    class ActivityCriteriaDataBinder : ICriteriaDataBinder
    {
        public SearchCriteria GetCriteria(DataRow dataRow)
        {
            try
            {
                return new ActivitySearchCriteria()
                {
                    Pipeline = (string)dataRow["ExecutionPipeline"],
                    UserType = StringToEnum<UserType>((string)dataRow["UserType"]),
                    Description = (string)dataRow["Description"],
                    ShortLocation = dataRow["ShortLocation"].ToString().Replace("..", ","),
                    Location = dataRow["Location"].ToString().Replace("..", ","),
                    Passengers = ParsePassengers(dataRow["Adults"].ToString(), dataRow["Children"].ToString(), dataRow["Infants"].ToString()),
                    Filters = GetFilters(dataRow["PostFilters"].ToString(), dataRow["PostFiltersValues"].ToString()),
                    PaymentMode = StringToEnum<PaymentMode>(((string)dataRow["PaymentMode"]).Split('|')[0]),
                    CardType = StringToEnum<CreditCardType>(((string)dataRow["PaymentMode"]).Contains("|") ? ((string)dataRow["PaymentMode"]).Split('|')[1] : "Visa"),
                    FromDate = DateTime.Now.AddDays(int.Parse(dataRow["StartDate"].ToString())),
                    ToDate = DateTime.Now.AddDays(int.Parse(dataRow["EndDate"].ToString())),
                };
            }
            catch (Exception exception)
            {
                throw new InvalidInputException("DataRow to AirCriteriaDataBinder.GetCriteria", exception);
            }
        }

        private Passengers ParsePassengers(string adults, string children, string infant)
        {
            return new Passengers()
                {
                    Adults = string.IsNullOrEmpty(adults) ? 0 : int.Parse(adults),
                    Children = string.IsNullOrEmpty(children) ? 0 : int.Parse(children),
                    Infants = string.IsNullOrEmpty(infant) ? 0 : int.Parse(infant)
                };
        }

        private Filters GetFilters(string filters, string value)
        {
            if (string.IsNullOrEmpty(filters))
                return null;
            var filterList = filters.Split('|');
            var valueList = value.Split('|');
            var i = 0;
            var filterCriteria = new ActivityPostSearchFilters();
            //Name|PriceRange|Categories|matrix|Sort
            while (i < filterList.Length)
            {
                switch (filterList[i].ToUpper())
                {
                    case "PriceRange":
                        filterCriteria.PriceRange = new PriceRange()
                        {
                            Min = int.Parse(valueList[i].Split('-')[0]),
                            Max = int.Parse(valueList[i].Split('-')[1])
                        };
                        break;
                    case "Name":
                        filterCriteria.ActivityName = valueList[i];
                        break;
                    case "Categories":
                        filterCriteria.Categories = new List<string>(valueList[i].Split('/'));
                        break;
                    case "MATRIX":
                        filterCriteria.Matrix = new ActivityMatrix()
                        {
                            Category = valueList[i]
                        };
                        break;
                    case "SORT":
                        filterCriteria.SortBy = StringToEnum<SortBy>(valueList[i]);
                        break;
                    default: throw new InvalidInputException(valueList[i] + " to ActivityDataBinder");
                }
                i++;
            }
            return new Filters()
                {
                    PostSearchFilters = filterCriteria
                };
        }

        private static T StringToEnum<T>(string name)
        {
            return (T)Enum.Parse(typeof(T), name, true);
        }

    }
}
