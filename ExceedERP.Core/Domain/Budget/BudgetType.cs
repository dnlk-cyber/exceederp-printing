using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetType : TrackUserSettingOperation
    {


        public int BudgetTypeId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
   
    }
}
