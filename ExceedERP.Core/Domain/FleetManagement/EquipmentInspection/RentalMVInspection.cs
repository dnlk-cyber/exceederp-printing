using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.EquipmentInspection
{
    public enum GoodCondition
    {
        __,
        Normal,
        NoLeakage, 
        Good,
        Working
    }
    public enum PoorCondition
    {
        __,
        Knocking,
        Abnormal,
        ThereIsLeakage,
        HardStarting,
        OverHeat,
        Poor,
        WornOut,
        NotWorking
    }
    public class RentalMVInspection: OperationWithThirdValidation
    {
        public int RentalMVInspectionId { get; set; }

        public int RentalInspectionCriteriaId { get; set; }
        public int RentalInspectionId { get; set; }
        public int RentalInspectionCriteriaDetailId { get; set; }
        public decimal Index { get; set; }
        public GoodCondition HeadOfficeGoodCondtion { get; set; }
        public PoorCondition HeadOfficePoorCondtion { get; set; }
        public GoodCondition BranchOfficeGoodCondtion { get; set; }
        public PoorCondition BranchOfficePoorCondtion { get; set; }
        public virtual RentalInspectionCriteria RentalInspectionCriteria { get; set; }
        public virtual RentalInspection RentalInspection { get; set; }
    }
}
