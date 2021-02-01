using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Inventory.BaseModel
{
    public class BinCardLineBase
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string ReferenceDoc { get; set; }
        public decimal ReceivedQTY { get; set; }
        public decimal ReceivedUPrice { get; set; }
        public decimal ReceivedTPrice { get; set; }
        public decimal IssuedQTY { get; set; }
        public decimal IssuedUPrice { get; set; }
        public decimal IssuedTPrice { get; set; }
        public decimal BalanceQTY { get; set; }
        public decimal BalanceUPrice { get; set; }
        public decimal BalanceTPrice { get; set; }
        [Display(Name = "Year")]
        public int GlFiscalYearId { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        public string Reference { get; set; }
        public DateTime? TimeStamp { get; set; }
        public bool IsVoid { get; set; }
        [Display(Name = "Purchased From")]
        public string SupplierName { get; set; }
        [Display(Name = "Issued To work Unit")]
        public string IssuedTo { get; set; }
        public string StoreCode { get; set; }//for transfer send/receive
        public BinCardTransactionTypes TransactionType { get; set; }
        public BinCardTransactionStatus StatusType { get; set; }
    }
}
