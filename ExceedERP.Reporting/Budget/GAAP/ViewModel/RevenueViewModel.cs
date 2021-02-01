using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Budget.GAAP.ViewModel
{
    public class RevenueViewModel
    {

        public string AccountCode { get; set; }
        public string AccountDesc { get; set; }
       
        public decimal First { get; set; }
        public decimal Second { get; set; }
        public decimal Third { get; set; }
        public decimal Fourth { get; set; }

        public decimal PlanFirst { get; set; }
        public decimal PlanSecond { get; set; }
        public decimal PlanThird { get; set; }
        public decimal PlanFourth { get; set; }

        public decimal DiffrenceFirst { get; set; }
        public decimal DiffrenceSecond { get; set; }
        public decimal DiffrenceThird { get; set; }
        public decimal DiffrenceFourth { get; set; }
        public decimal Total { get; set; }

        public decimal ActualTotal { get; set; }
        public decimal PlanTotal { get; set; }
        public decimal PercentageTotal { get; set; }

    }
}
