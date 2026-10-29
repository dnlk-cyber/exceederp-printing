using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Planning
{
    public class ConsProjectType 
    {
       
        public int ConsProjectTypeId { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Temporary until existing data gets migrated to new tables
        /// then it will need to be deleted
        /// </summary>
        [Display(Name = "Sector")]
        public string BusinessUnit { get; set; }

        public string Remark { get; set; }

    }
}
