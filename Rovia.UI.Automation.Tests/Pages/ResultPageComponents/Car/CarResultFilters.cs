using System;
using System.Collections.Generic;
using System.Linq;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public class CarResultFilters:UIPage,IResultFilters
    {
        private bool IsPreSearchFilterApplied(string filterType)
        {
            var isFiltered = GetUIElements("appliedFilters").ToList();
            return isFiltered.Exists(x => filterType.Contains(x.GetAttribute("data-fid")));
        }

        private IEnumerable<string> GetAppliedFilters()
        {
            return GetUIElements("appliedFilters").Select(x => x.Text.Trim());
        }

        private void SetPriceRange(PriceRange priceRange)
        {
            var minPrice =
                float.Parse(WaitAndGetBySelector("minPrice", ApplicationSettings.TimeOut.Fast).Text.Split(' ')[0].TrimStart('$'));
            var maxPrice =
               float.Parse(WaitAndGetBySelector("maxPrice", ApplicationSettings.TimeOut.Fast).Text.Split(' ')[0].TrimStart('$'));

            minPrice += minPrice * priceRange.Min / 100;
            maxPrice -= maxPrice * priceRange.Max / 100;

            ExecuteJavascript("$('#sliderRangePrice').trigger({type:'slideStop',value:[" + (minPrice * 100) + "," + (maxPrice * 100) + "]})");
        }

        private void SetLocationsFilter(List<string> locations)
        {
            var cabinTypeList = GetUIElements("cabinTypeFilter").ToList();
            cabinTypeList[0].Click();

            cabinTypeList.ForEach(x =>
            {
                if (locations.Contains(x.GetAttribute("data-name")) && x.Displayed)
                    x.Click();
            });
        }

        public bool VerifyPreSearchFilters(PreSearchFilters preSearchFilters)
        {
            var carSearchFilters = preSearchFilters as CarPreSearchFilters;
            bool carType = true, rentalAgency = true, carOptions = true;
            if (!string.IsNullOrEmpty(carSearchFilters.CarType)) carType= IsPreSearchFilterApplied("Car type");
            if (!string.IsNullOrEmpty(carSearchFilters.RentalAgency)) rentalAgency= IsPreSearchFilterApplied("Rental Agency");
            //if (carSearchFilters.AirConditioning > 0 || carSearchFilters.Transmission > 0) carOptions= IsPreSearchFilterApplied("Car options");
            return carType && rentalAgency && carOptions;
        }

        public void SetPostSearchFilters(PostSearchFilters postSearchFilters)
        {
            var carPostSearchFilters = postSearchFilters as CarPostSearchFilters;
            if (carPostSearchFilters == null)
                throw new InvalidInputException("PostSearchFilters");
            var appliedFilters = new List<string>();
            if (carPostSearchFilters.PriceRange != null)
            {
                SetPriceRange(carPostSearchFilters.PriceRange);
                appliedFilters.Add("Price");
            }
            if(carPostSearchFilters.LocationValues!=null)
            {
                SetLocationsFilter(carPostSearchFilters.LocationValues);
                appliedFilters.Add("Locations");
            }
           // SetMatrix();
            var unAppliedFilters = appliedFilters.Except(GetAppliedFilters()).ToList();

            if (unAppliedFilters.Any())
                throw new ValidationException("Following Filters were not applied : " + string.Join(",", unAppliedFilters));
        }

        public void ValidateFilters(PostSearchFilters postSearchFilters, List<Results> list)
        {
            throw new NotImplementedException();
        }
    }
}
