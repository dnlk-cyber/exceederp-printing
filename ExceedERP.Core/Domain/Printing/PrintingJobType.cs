using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintingJobType : TrackUserSettingOperation
    {
        [Key]
        public int PrintingJobTypeId { get; set; }
        public string JobTypeName { get; set; }
        public int NoOfPagesFactor { get; set; }
        public int DefaultNoOfPages { get; set; }
        public bool IsSpinRequired { get; set; }
        public bool IsBindingRequired { get; set; }
        public bool IsCoverRequired { get; set; }

    }
}
