using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement
{
   public class FleetManagementRoles
    {


        public const string FleetAdmin = "Fleet_Admin";
        public const string RegistrationUser = "Registration_User";
        public const string RegistrationApprover = "Registration_Approver";
        public const string RentRequestor = "Rent_Requestor";
        public const string RentUser = "Rent_User";
        public const string RentApprover = "Rent_Approver";
        public const string MaintenanceRequestor = "Maintenance_Requestor";
        public const string MaintenanceRequestApprover = "Maintenance_Request_Approver";
        public const string MaintenanceOrderUser = "Maintenance_Order_User";
        public const string MaintenanceServiceApprover = "Maintenance_Service_Approver";
        public const string ServiceUser = "Service_User";
        public const string FuelRequestor = "Fuel_Requestor";
        public const string FuelApprover = "Fuel_Approver";
        public const string FuelUser = "Fuel_User";
        public const string InspectionUser = "Inspection_User";
        public const string InspectionApprover = "Inspection_Approver";
        public const string ResourceRequestor = "Resource_Requestor";
        public const string ResourceDistributor = "Resource_Distributor";
        public const string ResourceReturnUser = "Resource_Return_User";
        public const string ResourceUser = "Resource_User";
        public const string ResourceRequestApprover = "Resource_Request_Approver";
        public const string ResourceApprover = "Resource_Approver";
        public const string TyreRequestor = "Tyre_Requestor";
        public const string TyreApprover = "Tyre_Approver";
        public const string TyreUser = "Tyre_User";
        public const string FleetTraininguser = "Fleet_Training_user";
        public const string FleetMaintenanceReport = "Fleet_Maintenance_Report";
        public const string FleetReport = "Fleet_Report";
        public const string FleetResourceReport = "Fleet_Resource_Report";
        public const string FleetRegistrationReport = "Fleet_Registration_Report";

       //Not Added

        public const string Equipment_Plan_User = "Equipment_Plan_User";
        public const string Equipment_Plan_Approver = "Equipment_Plan_Approver";

        public const string SalvageStoreIssueUser = "Salvage_Store_Issue_User";
        public const string SalvageStoreIssueApprover = "Salvage_Store_Issue_Approver";

        public const string StoreGoodReceivingUser = "Store_Good_Receiving_User";
        public const string SalvageGoodReceivingApprover = "Store_Good_Receiving_Approver";
    }
}
