using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.Budget.Models
{
    public class BindingItem
    {
      public  int Value { get; set; }
      public string Text { get; set; }
    }

    public class BudgetUsageViewModel
    {
        public int BudgetUsageId { get; set; }
        public string BudgetCategory { get; set; }
        public string WorkUnit { get; set; }
        public string Unit { get; set; }
        public string AccountCode { get; set; }
            [DisplayName("1st")]
        public decimal Jan { get; set; }
         [DisplayName("2nd")]
            public decimal Feb { get; set; }
         [DisplayName("3rd")]

         public decimal Mar { get; set; }
         [DisplayName("4th")]
         public decimal Apr { get; set; }
         [DisplayName("5th")]
         public decimal May { get; set; }
         [DisplayName("6th")]
         public decimal Jun { get; set; }
         [DisplayName("7th")]
         public decimal Jul { get; set; }
         [DisplayName("8th")]
         public decimal Aug { get; set; }
         [DisplayName("9th")]
         public decimal Sep { get; set; }
         [DisplayName("10th")]
         public decimal Oct { get; set; }
         [DisplayName("11th")]
         public decimal Nov { get; set; }
         [DisplayName("12th")]
         public decimal Dec { get; set; }
   
        public string Remark { get; set; }

    }
}