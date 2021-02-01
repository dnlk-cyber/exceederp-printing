using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Construction
{
    public class ConstructionConstants
    {

        #region user roles
        public const string ConstructionSettingAdmin = "Construction_Setting_Admin";
        public const string EstimationUser = "Estimation_User";
        public const string CostingUser = "Costing_User";
        public const string ManpowerUser = "Manpower_User";
        public const string MaterialUtilization = "Material_Utilization";
        public const string EquipmentUtilization = "Equipment_Utilization";
        public const string SchedulingUser = "Scheduling_User";
        public const string ContractManagementUser = "Contract_Management_User";
        public const string ProjectBudget = " Project_Budget";
        public const string ProjectReporting = " Project_Reporting";
        public const string ProjectTracking = "Project_Tracking";


        public const string ScheduleApprover = "Schedule_Approver";
        public const string EstimationApprover = "Estimation_Approver";
        public const string CostingApprover = "Costing_Approver";
        public const string PaymentApprover = "Payment_Approver";
        #endregion

        #region Dashboard Constants
        public const string ProjectPlanAndAchivementStatusCategory = "CONS_PLAN_ACHIVEMENT";
        public const string ProjectPlan = "PLAN";
        public const string ProjectAchivement = "ACHIVEMENT";


        public const string ProjectCostStatusCategory = "CONS_COST_STATUS";
        //public const string ProjectContractAmount = "CONTRACT_AMOUNT";
        public const string ProjectCostTodate = "COST_TODATE";

        public const string ProjectTimeStatusCategory = "CONS_TIME_STATUS";
        public const string ProjectTimeElapsedToDate = "ELAPSED_TIME";

        public const string ProjectPaymentAmountCategory = "CONS_PROJECT_PAYMENT";
        public const string ProjectPaymentAmountTodate = "PAYMENT_TODATE";
        #endregion
    }
}
