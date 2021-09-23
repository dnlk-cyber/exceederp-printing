using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{


    public partial class OwningandOperatingCostConstant : TrackUserSettingOperation
    {
        public int OwningandOperatingCostConstantId { get; set; }
 
        public OwningandOperatingCostConstantType ConstantType { get; set; }

         public decimal Value { get; set; }
           public string Remark  { get; set; }






       
    }



}