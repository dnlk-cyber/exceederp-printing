using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class Freelancer
    {
        public int FreelancerId { get; set; }
        public int EmployeeId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime From { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime To { get; set; }
        public string FromAm { get; set; }
        public string ToAm { get; set; }
    }
}
