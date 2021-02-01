using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.ArchiveManagement
{
   public class SubjectType
    {
        public int SubjectTypeId { get; set; }
        [Display(Name ="Subject Type")]
        public string SubjectTypeName { get; set; }
        public string Remark { get; set; }

    }
}
