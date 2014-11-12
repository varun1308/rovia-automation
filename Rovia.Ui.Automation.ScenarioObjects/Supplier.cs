using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class Supplier
    {
        protected bool Equals(Supplier other)
        {
            return SupplierId == other.SupplierId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (SupplierId*397) ^ (SupplierName != null ? SupplierName.GetHashCode() : 0);
            }
        }

        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        
    }
}
