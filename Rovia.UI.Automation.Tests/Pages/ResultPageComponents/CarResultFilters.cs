using System;
using System.Linq;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public class CarResultFilters:UIPage,IResultFilters
    {
        private bool IsPreSearchFilterApplied(string filterType)
        {
            var isFiltered = GetUIElements("appliedFilters").ToList();
            return isFiltered.Exists(x => filterType.Contains(x.GetAttribute("data-fid")));
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
            //throw new NotImplementedException();
        }
    }
}
