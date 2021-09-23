using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.APTS
{
    public class DrugReceivalDetail
    {
        public int DrugReceivalDetailId { get; set; }
        public int DrugReceivalModel19Id { get; set; }
        public int DrugListsId { get; set; }
        public int DrugUnitId { get; set; }
        public decimal QtyReceived { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public string BatchNo { get; set; }
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ExpiryDate { get; set; }
        public string ExpiryDateAmh { get; set; }
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public bool IsCurrent { get; set; }
        public decimal Balance { get; set; }
    }
}
