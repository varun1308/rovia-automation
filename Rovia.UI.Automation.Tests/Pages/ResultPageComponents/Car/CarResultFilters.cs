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
                if (locations.Contains(x.Text.Trim()) && x.Displayed)
                    x.Click();
            });
        }

        private void SetCarOptionsFilter(List<string> carOptions)
        {
            var carOptionsFilter = GetUIElements("carOptions").ToList();
            carOptionsFilter[0].Click();

            carOptionsFilter.ForEach(x =>
            {
                if (carOptions.Contains(x.GetAttribute("data-name")) && x.Displayed)
                    x.Click();
            });
        }

        private void SetRentalAgencyFilter(List<string> rentalAgency)
        {
            var rentalAgencyFilter = GetUIElements("carRentalAgencyFilter").ToList();
            rentalAgencyFilter[0].Click();

            rentalAgencyFilter.ForEach(x =>
            {
                if (rentalAgency.Contains(x.GetAttribute("data-name")) && x.Displayed)
                    x.Click();
            });
        }

        private void SetCarTypesFilter(List<string> carTypes)
        {
            var carTypesFilter = GetUIElements("carTypeFilter").ToList();
            carTypesFilter[0].Click();

            carTypesFilter.ForEach(x =>
            {
                if (carTypes.Contains(x.GetAttribute("data-name")) && x.Displayed)
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

        private void ValidateLocations(List<string> locationValues, IEnumerable<string> locationValueList)
        {
            if (!locationValueList.All(locationValues.Contains))
                _failedFilters.Add("Locations");
        }

        private void ValidateRentalAgency(List<string> rentalAgency, IEnumerable<string> carRentalAgency)
        {
            if (!carRentalAgency.All(rentalAgency.Contains))
                _failedFilters.Add("Rental Agency");
        }

        private void ValidateCarTypes(List<string> carTypes, IEnumerable<string> carTypeList)
        {
            if (!carTypeList.All(carTypes.Contains))
                _failedFilters.Add("Car type");
        }

        private void ValidatePriceRange(PriceRange priceRange, IEnumerable<double> amountList)
        {
            if (amountList.Any(x => x > priceRange.MaxPrice || x < priceRange.MinPrice))
                _failedFilters.Add("Price");
        }

        public void VerifyPreSearchFilters(PreSearchFilters preSearchFilters, Func<List<Results>> getParsedResults)
        {
            var carSearchFilters = preSearchFilters as CarPreSearchFilters;
            bool carType = true, rentalAgency = true, carOptions = true;
            if (!string.IsNullOrEmpty(carSearchFilters.CarType)) carType = IsPreSearchFilterApplied("Car type");
            if (!string.IsNullOrEmpty(carSearchFilters.RentalAgency)) rentalAgency = IsPreSearchFilterApplied("Rental Agency");
            //if (carSearchFilters.AirConditioning > 0 || carSearchFilters.Transmission > 0) carOptions= IsPreSearchFilterApplied("Car options");
            if(carType && rentalAgency && carOptions)
                return;
            throw new ValidationException("PostSEarchFilters");
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

        public void ValidateFilters(PostSearchFilters postSearchFilters, Func<List<Results>> getParsedResults)
        {
            var carPostSearchFilters = postSearchFilters as CarPostSearchFilters;
            var carResults = getParsedResults().Select(x => x as CarResult).ToList();
            _failedFilters = new List<string>();
            if (carPostSearchFilters == null)
                throw new InvalidInputException("PostSearchFilters");
            if (carPostSearchFilters.PriceRange != null)
                ValidatePriceRange(carPostSearchFilters.PriceRange, carResults.Select(x => x.TotalPrice.TotalAmount));
            if (carPostSearchFilters.LocationValues != null)
            {
                ValidateLocations(carPostSearchFilters.LocationValues.ConvertAll(d => d.ToLower()), carResults.Select(x => x.Location).ToList().ConvertAll(d => d.ToLower()));
            }
            if (carPostSearchFilters.CarTypes != null)
            {
                ValidateCarTypes(carPostSearchFilters.CarTypes.ConvertAll(d => d.ToLower()), carResults.Select(x => x.CarType).ToList().ConvertAll(d => d.ToLower()));
            }
            if (carPostSearchFilters.RentalAgency != null)
            {
                ValidateRentalAgency(carPostSearchFilters.RentalAgency.ConvertAll(d => d.ToLower()), carResults.Select(x => x.RentalAgency).ToList().ConvertAll(d => d.ToLower()));
            }
            if (carPostSearchFilters.CarOptions != null)
            {
                ValidateCarOptions(carPostSearchFilters.CarOptions, carResults);
            }
            if (carPostSearchFilters.Matrix != null)
                ValidateMatrix(_carMatrix, carResults);
            if (_failedFilters.Any())
                throw new ValidationException("Validation Failed for following filters : " + string.Join(",", _failedFilters));
        }

        private void ValidateCarOptions(List<string> carOptions, List<CarResult> carResults)
        {
            if (!(carResults.Select(x => x.AirConditioning).All(x => x.Equals(GetCarOptionsText(carOptions.Contains, "AC")))
                && carResults.Select(x => x.Transmission).All(x => x.Equals(GetCarOptionsText(carOptions.Contains, "AT")))))
                _failedFilters.Add("Car options");
        }

        private string GetCarOptionsText(Func<string, bool> contains, string ac)
        {
            if (ac.Equals("AC"))
                switch (contains.ToString())
                {
                    case "EAC":
                        return "Air Conditioning";
                    default: return "Air Conditioning";
                }
            else
                switch (contains.ToString())
                {
                    case "EAT":
                        return "Auto Transmission";
                    default: return "Auto Transmission";
                }
        }

        private void ValidateMatrix(CarMatrix carMatrix, List<CarResult> carResults)
        {
            if (!carResults.Select(x => x.TotalPrice.TotalAmount).All(x => x.Equals(carMatrix.TotalFare)))
                _failedFilters.Add("Price");
            if (!carResults.Select(x => x.CarType).All(x => x.Equals(carMatrix.CarType)))
                _failedFilters.Add("Car type");
            if (!carResults.Select(x => x.RentalAgency).All(x => x.Equals(carMatrix.RentalAgency)))
                _failedFilters.Add("Rental Agency");
        }
    }
}
