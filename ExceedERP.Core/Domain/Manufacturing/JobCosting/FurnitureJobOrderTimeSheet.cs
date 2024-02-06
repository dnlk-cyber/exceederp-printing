using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.JobCosting
{
   public class FurnitureJobOrderTimeSheet
    {
        public int FurnitureJobOrderTimeSheetId { get; set; }
        public int FurnitureJobOrderTimeRegistrationId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string Work { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }
        [DataType(DataType.Time)]
        // [DisplayFormat(DataFormatString = "{0:hh:mm:ss}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime ComplationTime { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime TotalTime { get; set; }
        public int NoOfEmployee { get; set; }
        public string Remark { get; set; }
    }
}
