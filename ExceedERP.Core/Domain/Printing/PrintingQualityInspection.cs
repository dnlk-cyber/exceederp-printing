using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintingQualityInspection 
    {
        public int PrintingQualityInspectionId { get; set; }
        public string Description { get; set; }
        public PrintGradeQuality PrintQualityGrade { get; set; }
        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public string ModifiedBy { get; set; }
        //FK
        public int PrintingProductionFollowUpId { get; set; }
    }
    public enum PrintGradeQuality
    {
        Excellent,
        Vervy_Good,
        Good,
        Poor
    }
}
