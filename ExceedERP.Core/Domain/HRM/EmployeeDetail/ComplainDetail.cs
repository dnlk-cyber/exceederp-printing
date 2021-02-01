using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class ComplainDetail
    {
        [Key]
        public int ComplainDetailId { get; set; }
        public int ComplainId { get; set; }
        [DataType(DataType.MultilineText)]
        public string Response { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }
        [Display(Name ="Date")]
        public string DateAm { get; set; }
        public string Remark { get; set; }
    }
}
