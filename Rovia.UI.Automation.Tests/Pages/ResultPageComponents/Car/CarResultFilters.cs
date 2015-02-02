using System;
using System.Collections.Generic;
using System.Linq;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public class CarResultFilters : UIPage, IResultFilters
    {
        private List<string> _appliedFilters;
        private List<string> _failedFilters;
        private CarMatrix _carMatrix;
        
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

            priceRange.MinPrice = minPrice;
            priceRange.MaxPrice = maxPrice;

            ExecuteJavascript("$('#sliderRangePrice').trigger({type:'slideStop',value:[" + (minPrice * 100) + "," + (maxPrice * 100) + "]})");
        }

        private void SetLocationsFilter(List<string> locations)
        {
            var locationFilter = GetUIElements("locationFilter").ToList();
            locationFilter[0].Click();

            locationFilter.ForEach(x =>
            {
                if (locations.ConvertAll(y => y.ToLower()).Contains(x.Text.Trim().ToLower()) && x.Displayed)
                    x.Click();
            });
        }

        private void SetCarOptionsFilter(List<string> carOptions)
        {
            var carOptionsFilter = GetUIElements("carOptions").ToList();
            carOptionsFilter[0].Click();

            carOptionsFilter.ForEach(x =>
            {
                if (carOptions.ConvertAll(y => y.ToLower()).Contains(x.GetAttribute("data-name").ToLower()) && x.Displayed)
                    x.Click();
            });
        }

        private void SetRentalAgencyFilter(List<string> rentalAgency)
        {
            var rentalAgencyFilter = GetUIElements("carRentalAgencyFilter").ToList();
            rentalAgencyFilter[0].Click();

            rentalAgencyFilter.ForEach(x =>
            {
                if (rentalAgency.ConvertAll(y => y.ToLower()).Contains(x.GetAttribute("data-name").ToLower()) && x.Displayed)
                    x.Click();
            });
        }

        private void SetCarTypesFilter(List<string> carTypes)
        {
            var carTypesFilter = GetUIElements("carTypeFilter").ToList();
            carTypesFilter[0].Click();

            carTypesFilter.ForEach(x =>
            {
                if (carTypes.ConvertAll(y => y.ToLower()).Contains(x.GetAttribute("data-name").ToLower()) && x.Displayed)
                    x.Click();
            });
        }

        private void SetMatrix()
        {
            var matrix = GetUIElements("matrixSection").First();
            _carMatrix = new CarMatrix();
            var carAgencyType = matrix.GetAttribute("data-id").Split(',');

            _carMatrix.CarType = carAgencyType[0];
            _carMatrix.RentalAgency = carAgencyType[1];
            _carMatrix.TotalFare = double.Parse(matrix.WaitAndGetBySelector("matrixPrice", ApplicationSettings.TimeOut.Fast).Text.TrimStart('$'));

            matrix.Click();
        }

        private void ValidateLocations(ICollection<string> locationValues)
        {
            var locations = GetUIElements("locations").Select(x => x.Text.Split('-')[0].TrimEnd(' ')).Where((item, index) => index % 2 == 0);
            if (!locations.All(locationValues.Contains))
                _failedFilters.Add("Locations");
        }

        private void ValidateRentalAgency(ICollection<string> rentalAgency)
        {
            var carRentalAgencies = GetUIElements("rentalAgency").Select(x => x.GetAttribute("alt"));
            if (!carRentalAgencies.All(rentalAgency.Contains))
                _failedFilters.Add("Rental Agency");
        }

        private void ValidateCarTypes(ICollection<string> filterCarTypes)
        {
            var carTypes = GetUIElements("carType").Where((x, index) => index == 0 || (index % 3 == 0)).Select(x => x.Text.Split(' ')[0]);
            if (!carTypes.All(filterCarTypes.Contains))
                _failedFilters.Add("Car type");
        }

        private void ValidatePriceRange(PriceRange priceRange)
        {
            var prices = GetUIElements("price").Where((item, index) => index % 2 != 0).Select(x => new Amount(x.Text).TotalAmount);
            if (prices.Any(x => x > priceRange.MaxPrice || x < priceRange.MinPrice))
                _failedFilters.Add("Price");
        }

        private void ValidateCarOptions(IEnumerable<string> filterCarOptions)
        {
            var carOptions = GetUIElements("carOptions");
            var airconditioning = carOptions.Skip(1).Where((x, index) => index % 4 == 0).Select(x => x.Text);
            var transmission = carOptions.Skip(2).Where((x, index) => index % 4 == 0).Select(x => x.Text);
            var filterCode = GetCarOptionsText(filterCarOptions);
            if (!(airconditioning.All(filterCode.Contains)))
                _failedFilters.Add("Car options");
        }

        private List<string> GetCarOptionsText(IEnumerable<string> carOptionsCode)
        {
            var carOptionsText = new List<string>();
            foreach (string code in carOptionsCode)
            {
                switch (code)
                {
                    case "EAC":
                        carOptionsText.Add("Air Conditioning");
                        break;
                    case "EAT":
                        carOptionsText.Add("Auto Transmission");
                        break;
                }
            }
            return carOptionsText;
        }

        private void ValidateMatrix(CarMatrix carMatrix)
        {
            var prices = GetUIElements("price").Where((item, index) => index % 2 != 0).Select(x => new Amount(x.Text).TotalAmount);
            if (!prices.All(x => x.Equals(carMatrix.TotalFare)))
                _failedFilters.Add("Price");
            var carTypes = GetUIElements("carType").Where((x, index) => index == 0 || (index % 3 == 0)).Select(x => x.Text.Split(' ')[0]);
            if (!carTypes.All(x => x.Equals(carMatrix.CarType)))
                _failedFilters.Add("Car type");
            var carRentalAgencies = GetUIElements("rentalAgency").Select(x => x.GetAttribute("alt"));
            if (!carRentalAgencies.All(x => x.Equals(carMatrix.RentalAgency)))
                _failedFilters.Add("Rental Agency");
        }

        public void VerifyPreSearchFilters(PreSearchFilters preSearchFilters)
        {
            var carSearchFilters = preSearchFilters as CarPreSearchFilters;
            var appliedFilters = new List<string>();
            if (!string.IsNullOrEmpty(carSearchFilters.CarType)) appliedFilters.Add("Car type");
            if (!string.IsNullOrEmpty(carSearchFilters.RentalAgency)) appliedFilters.Add("Rental Agency");
            //if (carSearchFilters.AirConditioning > 0 || carSearchFilters.Transmission > 0) carOptions= IsPreSearchFilterApplied("Car options");
            var unAppliedFilters = appliedFilters.Except(GetAppliedFilters()).ToList();
            if (unAppliedFilters.Any())
                throw new ValidationException("Following Filters were not applied on search : " + string.Join(",", unAppliedFilters));
        }

        public void SetPostSearchFilters(PostSearchFilters postSearchFilters)
        {
            var carPostSearchFilters = postSearchFilters as CarPostSearchFilters;
            if (carPostSearchFilters == null)
                throw new InvalidInputException("PostSearchFilters");
            _appliedFilters = new List<string>();
            if (carPostSearchFilters.Matrix != null)
            {
                SetMatrix();
                _appliedFilters.AddRange(new[] { "Rental Agency", "Car type" });
            }
            if (carPostSearchFilters.PriceRange != null)
            {
                SetPriceRange(carPostSearchFilters.PriceRange);
                _appliedFilters.Add("Price");
            }
            if (carPostSearchFilters.LocationValues != null)
            {
                SetLocationsFilter(carPostSearchFilters.LocationValues);
                _appliedFilters.Add("Locations");
            }
            if (carPostSearchFilters.CarTypes != null)
            {
                SetCarTypesFilter(carPostSearchFilters.CarTypes);
                _appliedFilters.Add("Car type");
            }
            if (carPostSearchFilters.RentalAgency != null)
            {
                SetRentalAgencyFilter(carPostSearchFilters.RentalAgency);
                _appliedFilters.Add("Rental Agency");
            }
            if (carPostSearchFilters.CarOptions != null)
            {
                SetCarOptionsFilter(carPostSearchFilters.CarOptions);
                _appliedFilters.Add("Car options");
            }

            var unAppliedFilters = _appliedFilters.Except(GetAppliedFilters()).ToList();

            if (unAppliedFilters.Any())
                throw new ValidationException("Following Filters were not applied : " + string.Join(",", unAppliedFilters));
        }

        public void ValidateFilters(PostSearchFilters postSearchFilters)
        {
            var carPostSearchFilters = postSearchFilters as CarPostSearchFilters;
            _failedFilters = new List<string>();
            if (carPostSearchFilters == null)
                throw new InvalidInputException("PostSearchFilters");
            if (carPostSearchFilters.PriceRange != null)
                ValidatePriceRange(carPostSearchFilters.PriceRange);
            if (carPostSearchFilters.LocationValues != null)
            {
                ValidateLocations(carPostSearchFilters.LocationValues.ConvertAll(d => d.ToLower()));
            }
            if (carPostSearchFilters.CarTypes != null)
            {
                ValidateCarTypes(carPostSearchFilters.CarTypes.ConvertAll(d => d.ToLower()));
            }
            if (carPostSearchFilters.RentalAgency != null)
            {
                ValidateRentalAgency(carPostSearchFilters.RentalAgency.ConvertAll(d => d.ToLower()));
            }
            if (carPostSearchFilters.CarOptions != null)
            {
                ValidateCarOptions(carPostSearchFilters.CarOptions);
            }
            if (carPostSearchFilters.Matrix != null)
                ValidateMatrix(_carMatrix);
            if (_failedFilters.Any())
                throw new ValidationException("Validation Failed for following filters : " + string.Join(",", _failedFilters));
        }
    }
}
