using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{

    public class Beginning : Operations
    {
        [Key]
        public int BeginningID { get; set; }

        public string ReferenceType { get; set; }
        public string Reference { get; set; }
        [Required]
        public string Period { get; set; }
        [Required]
        public double Value { get; set; }

        public Boolean IsProvisional { get; set; }

        public string Remark { get; set; }
    }
}
