using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Helpers.Common
{
    public  class TaxHelper
    {
        private  ExceedDbContext db = new ExceedDbContext();
        public  string GetTaxName(int taxid)
        {
            string taxName = "";
            var tax = db.GLTaxRates.Find(taxid);
            if (tax != null) taxName = tax.TaxName;

            return taxName;
        }
    }
}
