using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Dashboard;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.CMS;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;

using ExceedERP.Core.Domain.printing.PrintingEstimation;
using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;
using ExceedERP.Core.Domain.Manufacturing.JobCosting;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.Core.Domain.Printing;

namespace ExceedERP.Web.Controllers
{

	public class InitialController : BaseController
	{

		private const string InitialUserName = "tester";
		private const string InitialUserFirstName = "TestFirstName";
		private const string InitialUserLastName = "TestLastName";
		private const string InitialUserEmail = "test@test.com";
		private const string InitialUserPassword = "Password1";

		private const string VaniallaUserName = "VanillaUser";
		private const string VaniallaUserFirstName = "VanillaFirstName";
		private const string VaniallaUserLastName = "VanillaLastName";
		private const string VaniallaUserEmail = "vanilla@test.com";
		private const string VaniallaUserPassword = "Password1";
		private static readonly ApplicationDbContext _db = new ApplicationDbContext();

		private static readonly string[] _groupAdminRoleNames = { "CanEditUser", "CanEditGroup", "User" };


		private static readonly string[] _initialGroupNames = { "SuperAdmins", "GroupAdmins", "UserAdmins", "Users" };


		private static readonly string[] _superAdminRoleNames = { "Admin", "CanEditUser", "CanEditGroup", "CanEditRole", "User" };
		private static readonly string[] _userAdminRoleNames = { "CanEditUser", "User" };
		private static readonly string[] _userRoleNames = { "User" };


		public ActionResult AddRoles()
		{
			ApplicationUserManager userManager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
			//ApplicationRoleManager roleManager = System.Web.HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();

			//AddRoles(roleManager);

			AddallRoles();



			return View("Success");
		}

		public ActionResult Initial()
		{
			ApplicationUserManager userManager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
			ApplicationRoleManager roleManager = System.Web.HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();

			//AddRoles(roleManager);

			AddUsers(userManager);
			var groupManager = new ApplicationGroupManager();

			AddGroups(groupManager);
			AddRolesToGroups(groupManager);
			AddUsersToGroups(groupManager);

			return View("Success");
		}

		public static void AddGroups(ApplicationGroupManager _idManager)
		{
			foreach (var groupName in _initialGroupNames)
			{
				ApplicationGroup applicationGroup = new ApplicationGroup(groupName);

				_idManager.CreateGroup(applicationGroup);

			}
		}

		public static void AddallRoles()
		{
			ApplicationRoleManager roleManager = System.Web.HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
			List<ApplicationRole> applicationRoles = new List<ApplicationRole>
			{
				#region General
				new ApplicationRole("Admin","Sytem Admin","Global"),
				new ApplicationRole("CanEditUser","Can Edit User","Global"),
				new ApplicationRole("CanEditGroup","Can Edit Group","Global"),
				new ApplicationRole("CanEditRole","Can Edit Role","Global"),
				new ApplicationRole("User","User", "Global"),

				#endregion
			
				#region APTS Roles
				new ApplicationRole("APTS_Admin","Approve and see","APTS"),
				new ApplicationRole("APTS_Despensary","Request Drugs","APTS"),
				new ApplicationRole("APTS_DrugStore","Manage Drugs","APTS"),
				new ApplicationRole("APTS_Accountant","Manage balance","APTS"),
				new ApplicationRole("APTS_User","Apts User","APTS"), 
				#endregion

				#region dashboard
				new ApplicationRole("Admin_Regional","To see dashboard reports","Administrator"),
                new ApplicationRole("Admin_Global_Setting","To to manage global settings","Administrator"),
				#endregion

				#region dashboard
				new ApplicationRole("Dashboard_HR","To see dashboard reports","Global"),
				new ApplicationRole("Dashboard_Finance","To see dashboard reports","Global"),
				new ApplicationRole("Dashboard_Inventory","To see dashboard reports","Global"),
				new ApplicationRole("Dashboard_Procurement","To see dashboard reports","Global"),
			   #endregion
                #region info desk
new ApplicationRole("HelpDeskAdmin", "HelpDeskAdmin", "HelpDesk"),
new ApplicationRole("OpenTicket", "OpenTicket", "HelpDesk"),
new ApplicationRole("AssignHandler", "AssignHandler", "HelpDesk"),
new ApplicationRole("Handler", "Handler", "HelpDesk"),
new ApplicationRole("HelpDesk_Report", "View Report", "HelpDesk"),
new ApplicationRole("HelpDesk_Setting", "Add Help desk settings", "HelpDesk"),
#endregion

			   #region Finance
			new ApplicationRole("Finance_Admin","To define common finance setup","Finance"),
			new ApplicationRole("Finance_Checker","To check and correct account transactions","Finance"),
			new ApplicationRole("Finance_Report","To generate common finance report","Finance"),
			new ApplicationRole("General_Ledger_Post_User","To post journal records to ledger","Finance"),
			new ApplicationRole(FinanceConstants.GeneralLedgerUnPostUser,"To un-post journal records","Finance"),
			new ApplicationRole(FinanceConstants.GeneralLedgerUser,"User will record, reverse, send for approval, edit, and delete diffrernt type of transaction entries","Finance"),
			new ApplicationRole(FinanceConstants.GeneralLedgerApprover,"User will approve/reject different type of transaction ","Finance"),
			new ApplicationRole(FinanceConstants.GeneralLedgerView,"User will view only transaction.","Finance"),
			new ApplicationRole(FinanceConstants.GeneralLedgerAdmin,"To define chart of account segment, and journal category ","Finance"),
			new ApplicationRole("General_Ledger_Beginning","To manage and update beginning balance entries ","Finance"),
			new ApplicationRole("General_Ledger_Report","To generate GL report","Finance"),
			new ApplicationRole("General_SubLedger_Transfer","To transfer sub ledger transactions to GL","Finance"),
			new ApplicationRole("General_SubLedger_Inventory_BatchTransfer","To batch transfer invventory sub ledger transactions to GL","Finance"),


			new ApplicationRole("Account_Payable_View","User will view only payment and invoice transaction ","Finance"),
			new ApplicationRole("Account_Payable_Invoice","User will record,  send for approval, transfer to GL, edit, and delete different type of supplier invoices ","Finance"),
			new ApplicationRole("Account_Payable_Approver","User will approve supplier invoice, and payment amount ","Finance"),
			new ApplicationRole("Account_Payable_Payment","User will record,  send for approval, transfer to GL, edit, and delete different type of supplier payment ","Finance"),
			new ApplicationRole("Account_Payable_Settlement","User will record,  send for approval, transfer to GL, edit, and delete different type of payment settlment ","Finance"),
			new ApplicationRole("Account_Payable_Admin","To define payable setup","Finance"),
			new ApplicationRole("Account_Payable_Report","To generate payable invoice, and payment report","Finance"),
			new ApplicationRole("Account_Payable_PettyCash_Approver","To approve petty cash transaction","Finance"),
			new ApplicationRole("Account_Payable_PettyCash","To do petty cash activity","Finance"),
			new ApplicationRole("Account_Payable_TravelOrder_Approver","To approve travel order transaction","Finance"),
			new ApplicationRole("Account_Payable_TravelOrder","To do travel order activity","Finance"),
			new ApplicationRole("Account_Payable_TravelExpenseClaim_Approver","To approve travel expense claim transaction","Finance"),
			new ApplicationRole("Account_Payable_TravelExpenseClaim","To do travel expense claim activity","Finance"),
			new ApplicationRole("Account_Payable_FundTransfer_Approver","To approve fund transfer request transaction","Finance"),
			new ApplicationRole("Account_Payable_FundTransfer","To do fund transfer request activity","Finance"),
			new ApplicationRole("Account_Payable_Void","To void AP transactions","Finance"),
			new ApplicationRole("AP_PaymentRequest_Authorizer","To Authorize payment requests that are approved by department","Finance"),


			new ApplicationRole("Account_Receivable_Report","To generate receivable report","Finance"),
			new ApplicationRole("Account_Receivable_Admin","To define receivable setup","Finance"),
			new ApplicationRole("Account_Receivable_Approver","User will approve customer  invoice, and collection amount ","Finance"),
			new ApplicationRole("Account_Receivable_Collection","User will record, send for approval, transfer to GL, edit, and delete different type of customer collection or receipts","Finance"),
			new ApplicationRole("Account_Receivable_Invoice","User will record, send for approval, transfer to GL, edit, and delete different type of customer invoices","Finance"),
			new ApplicationRole("Account_Receivable_View","User will view only receivable activities ","Finance"),
			new ApplicationRole("Account_Receivable_Void","Void AR transactions","Finance"),

			new ApplicationRole("AR_SalesOrder","User will record, send for approval, transfer to GL, edit, and delete different type of customer invoices","Finance"),
			new ApplicationRole("AR_SalesOrder_view","View sales order transactions ","Finance"),
			new ApplicationRole("AR_SalesOrder_Approver","Approve sales order transactions","Finance"),
			new ApplicationRole("AR_SalesOrder_Receipt","Print cash and credit receipts.","Finance"),
			new ApplicationRole("AR_SalesOrder_Void","void sales orders.","Finance"),


			new ApplicationRole("Bank_Reconciliation_View","User will view only bank reconciled and unreconciled transaction  entries ","Finance"),
			new ApplicationRole("Bank_Reconciliation_Admin","To define bank setup","Finance"),
			new ApplicationRole("Bank_Reconciliation_Approver","To approve bank reconciliation transactions","Finance"),
			new ApplicationRole("Bank_Reconciliation_Void","To void bank reconciliation transactions","Finance"),
			new ApplicationRole("Bank_Reconciliation_Report","To generate different bank reconciliation report","Finance"),
			new ApplicationRole("Bank_Reconciliation_User ","User will reconcile ledger transaction with bank statement transaction on monthly","Finance"),
		  
		   #endregion

				#region payroll
			   new ApplicationRole("OverTime_Actual_Record_User ", "Overtime user", "Payroll"),
			   new ApplicationRole("Payroll_Input", "Manage Payroll input functionalities", "Payroll"),
			   new ApplicationRole("OverTime_Request_Approver", "Approve,Manage pend or reject all Over time requests", "Payroll"),
			   new ApplicationRole("OverTime_Request_User", "Request Over Time requests", "Payroll"),
			   new ApplicationRole("Payroll_Admin", "Manage Payroll Settings and configurations functionalities", "Payroll"),
			   new ApplicationRole("Payroll_Process", "Manage Payroll Process functionalities", "Payroll"),
			   new ApplicationRole("Payroll_Distribute", "Manage Payroll Distribution functionalities", "Payroll"),
			   new ApplicationRole("Payroll_Close", "Manage Payroll Closing functionalities", "Payroll"),
			   new ApplicationRole("Payroll_Transfer", "Manage Payroll Transfer functionalities", "Payroll"),
			   new ApplicationRole("Payroll_Approver", "Approve,Manage pend or reject all payroll requests", "Payroll"),
			   new ApplicationRole("Payroll_Report", "View payroll reports", "Payroll"),
			   new ApplicationRole("Payroll_View", "", "Payroll"),
			   new ApplicationRole("OverTime_Actual_Record_User", "Maintain Actual Employee over time record", "Payroll"),
			   new ApplicationRole("Payroll_Input","User will enter different type of payroll input like loan, overtime, fixed contribution…","Payroll"),
			   new ApplicationRole("Payroll_Admin","To define payroll setup","Payroll"),
			   new ApplicationRole("Payroll_Process","User will process payroll on monthly base, reverse process payroll, transfer transaction to GL, close period, and view transaction.","Payroll"),
			   new ApplicationRole("Payroll_Approver","User will approve processed payroll before issued to payment ","Payroll"),
			   new ApplicationRole("Payroll_Report","To generate different type of payroll report","Payroll"),
			   new ApplicationRole("Payroll_View","User will view only payroll input data, process payroll status, and report.","Payroll"),
			   new ApplicationRole("Payroll_Distribute","Manage Payroll Distribution functionalities","Payroll"),
			   new ApplicationRole("Payroll_Close","Manage Payroll Closing functionalities","Payroll"),
			   new ApplicationRole("Payroll_Transfer","Manage Payroll Transfer functionalities","Payroll"),
			   new ApplicationRole("OverTime_Request_Approver","Approve,Manage pend or reject all Over time requests","Payroll"),
			   new ApplicationRole("OverTime_Request_User","Request Over Time requests","Payroll"),
			   new ApplicationRole("All_Branch_Payroll_Process","Process all branch","Payroll"),
			   new ApplicationRole("All_Branch_Payroll_Report","View all branch report","Payroll"),
				#endregion

			  #region Procurement
		   new ApplicationRole("Procurement_Admin", "Manage Procurement setting", "Procurement"),
		   new ApplicationRole("Procurement_User", "Procurement User", "Procurement"),
		   new ApplicationRole("Procurement_Approver", "Approver of procurement activities", "Procurement"),
		   new ApplicationRole("Procurement_UserDepartment_User", "User departments role", "Procurement"),
		   new ApplicationRole("Procurement_UserDepartment_Approver", "Approver of user departments", "Procurement"),
		   new ApplicationRole("PR_Requisition_Second_Approver", "Second Approver of PR requests", "Procurement"),
		   new ApplicationRole("Procurement_Bond_User", "Guarantee,bond and cpo", "Procurement"),
		   new ApplicationRole("Procurement_Bond_Approver", "Guarantee,bond and cpo approver", "Procurement"),

		   new ApplicationRole("Procurement_AdvanceBond_User", "Advance guarantee,bond and cpo", "Procurement"),
		   new ApplicationRole("Procurement_AdvanceBond_Approver", "Advance guarantee,bond and cpo approver", "Procurement"),

		   new ApplicationRole("Procurement_Supplier_User", "Supplier management user", "Procurement"),
		   new ApplicationRole("Procurement_Supplier_Approver", "Supplier management approver", "Procurement"),
		   new ApplicationRole("Procurement_Contract_User", "Contract management user", "Procurement"),
		   new ApplicationRole("Procurement_Contract_Manager", "Contract management manager", "Procurement"),
		   new ApplicationRole("Procurement_Contract_Approver", "Contract management approver", "Procurement"),
		   new ApplicationRole("Procurement_Plan_User", "Procurement need and plan management user", "Procurement"),
		   new ApplicationRole("Procurement_Plan_Approver", "Procurement need and plan management approver", "Procurement"),
		   new ApplicationRole("Procurement_Tender_User", "Tender,proforma and PO management user", "Procurement"),
		   new ApplicationRole("Procurement_Tender_Approver", "Tender,proforma and PO management user approver", "Procurement"),
		   new ApplicationRole("Procurement_Void", "To void procurement transactions", "Procurement"),
		   new ApplicationRole("Procurement_TeamLeader", "To distribute purchase requisions to team members", "Procurement"),
		   new ApplicationRole("Procurement_Officer", "To see asigned purchase requisions by procurement officers", "Procurement"),
		   new ApplicationRole("Procurement_ForeignPurchase_User", "Foreign purchase follow up activity", "Procurement"),
		   new ApplicationRole("Procurement_ForeignPurchase_Approver", "Foreign purchase follow up approver", "Procurement"),
		   new ApplicationRole("Procurement_Report", "View procurement reports", "Procurement"),
		   new ApplicationRole("Service_Requisition_Second_Approver", "Second approver", "Procurement"),
		   new ApplicationRole("Procurement_Payment_User", "payment request user", "Procurement"),
		   new ApplicationRole("Procurement_Payment_Approver", "payment request approver", "Procurement"),
				
		   #endregion

			#region Property
			 new ApplicationRole("User_Department", "Approve store requisitions", "Property"),
			 new ApplicationRole("Property_Requisition_Second_Approver", "Request second approver", "Property"),
			 new ApplicationRole("Property_Count_User", "Physical count", "Property"),
			 new ApplicationRole("Property_Count_Approver", "Physical count Approver", "Property"),
			 new ApplicationRole("Property_Requisition_User","All users that request items from store ","Property"),
			 new ApplicationRole("Property_User","Property Staff that handles property transactions ","Property"),
			 new ApplicationRole("User_Department","Approver of work unit requisitions ","Property"),
			 new ApplicationRole("Property_Approver","Approve property transactions ","Property"),
			 new ApplicationRole("Property_Authorizer","Authorize property transactions after approval","Property"),
			 new ApplicationRole("Property_Count_Approver","Property Physical Count Update approver","Property"),
			 new ApplicationRole("Property_Count_User","Property Physical Count user","Property"),
			 new ApplicationRole("Property_Admin","Setting Property lookups ","Property"),
			 new ApplicationRole("Property_Report","View, Export and print Property related reports","Property"),
			 new ApplicationRole("PropertyImport","Import data","Property"),
			 new ApplicationRole("Property_StoreKeeper","Store transactions","Property"),
			 new ApplicationRole("Property_Inspector","Good receive inspection role","Property"),
			 new ApplicationRole("Property_Void","Void Stock Transactions","Property"),
			 new ApplicationRole("Property_Bincard","Update or recalculate bincard transaction","Property"),
		   #endregion
			 #region Fixed Asset
			 new ApplicationRole("FixedAsset_Requisition_User", "Fixed asset requisition user", "Fixed Asset"),
			 new ApplicationRole("FixedAsset_Requisition_Approver", "Fixed asset requisition approver", "Fixed Asset"),
			 new ApplicationRole("FixedAsset_Requisition_Second_Approver", "Fixed asset requisition second approver", "Fixed Asset"),
			 new ApplicationRole("FixedAsset_User", "Fixed asset user", "Fixed Asset"),
			 new ApplicationRole("FixedAsset_Approver", "Fixed asset approver", "Fixed Asset"),
			 new ApplicationRole("FixedAsset_Authorizer", "Fixed Asset Authorizer", "Fixed Asset"),
			 new ApplicationRole("FixedAsset_Count_User", "Fixed asset count user", "Fixed Asset"),
			 new ApplicationRole("FixedAsset_Count_Approver", "Fixed Asset count approver", "Fixed Asset"),
			 new ApplicationRole("FixedAsset_Admin", "Fixed asset settings administrator", "Fixed Asset"),
			 new ApplicationRole("FixedAsset_Report", "Fixed Asset report user", "Fixed Asset"),
			 new ApplicationRole("FixedAsset_Import", "Fixed asset import", "Fixed Asset"),
			 new ApplicationRole("FixedAsset_StoreKeeper", "Fixed Asset store keeper", "Fixed Asset"),
			 new ApplicationRole("FixedAsset_Void", "Fixed asset void", "Fixed Asset"),
			 new ApplicationRole("FixedAsset_User","Manage Fixed Asset AssetRegistration and user card ","Fixed Asset"),
			 new ApplicationRole("FixedAsset_Approver","Approve Fixed asset user card and assetRegistration ","Fixed Asset"),
			 new ApplicationRole("FixedAsset_Admin","Setting Fixed Asset lookups","Fixed Asset"),
			 new ApplicationRole("FixedAsset_Depreciation_User","Run fixed Asset AnnualDepreciation ","Fixed Asset"),
			 new ApplicationRole("FixedAsset_Depreciation_Approver","Approve Fixed Asset AnnualDepreciation","Fixed Asset"),
			 new ApplicationRole("FixedAsset_Report","View, Export and print,Fixed Asset Reports ","Fixed Asset"),
			 new ApplicationRole("FixedAsset_Inspector","Fixed asset receive inspection role","Fixed Asset"),
			 new ApplicationRole("Dashboard_FixedAsset","View Fixed Asset Dashboard ","Fixed Asset"),
			#endregion
				#region HR roles
	 new ApplicationRole("GenerateDayAbsent", "GenerateDayAbsent", "HR"),
	 new ApplicationRole("SalaryStructure", "SalaryStructure", "HR"),
	 new ApplicationRole("Miscellaneous", "Miscellaneous", "HR"),
	 new ApplicationRole("ManPowerPlanning", "Man Power Planning", "HR"),
	new ApplicationRole("LeaveExpire", "Leave Expire", "HR"),
	new ApplicationRole("LeaveExpireReverse", "Leave Expire Reverse", "HR"),
	new ApplicationRole("HR_Performance_Approver","Approve employee performance","HR"),
	new ApplicationRole("HR_EmployeeArchive","Manage Employee Archive","HR"),
	new ApplicationRole("HR_InstitutionArchive","Manage Institution Archive","HR"),
	new ApplicationRole("HR_Admin","HR Configuration and settings","HR"),
	new ApplicationRole("HR_Department_Head","HR Department Heads","HR"),
	new ApplicationRole("HR_Report","HR Reports view and print","HR"),
	new ApplicationRole("HR_Leave_User","Manage leave functionality by HR personnel","HR"),
	new ApplicationRole("HR_Leave_Admin","Manage leave setting functionality by HR personnel administrator","HR"),
	new ApplicationRole("HR_Leave_Approver","Approve , pend or reject all leave requests","HR"),
	new ApplicationRole("HR_Leave_Report","View leave reports","HR"),
	new ApplicationRole("HR_Employee_SelfService","Employees can view information or fill request","HR"),
	new ApplicationRole("HR_Manager_SelfService","Managers can view information or request from employees","HR"),
	new ApplicationRole("HR_Company_Admin","Manage company structure setting functionalities","HR"),
	new ApplicationRole("HR_Company_User","Manage company stricture functionalities by HR personnel","HR"),
	new ApplicationRole("HR_Profile_Admin","Manage basic employee profile  setting functionalities","HR"),
	new ApplicationRole("HR_Rehire_Profile_Admin","Manage basic Rehireable employee profile   functionalities","HR"),
	new ApplicationRole("HR_Profile_User","Manage basic employee profile  setting functionalities","HR"),
	new ApplicationRole("HR_Recruitment_Admin","Manage recruitment settings of HR recruitment functionalities","HR"),
	new ApplicationRole("HR_Recruitment_User","Manage recruitment functionalities by HR personnel","HR"),
	new ApplicationRole("HR_Recruitment_Approver","Approve , pend or reject all recruitment requests","HR"),
	new ApplicationRole("HR_Recruitment_Report","View recruitment  reports","HR"),
	new ApplicationRole("HR_Placement_Admin","Manage placement functionalities by HR personnel","HR"),
	new ApplicationRole("HR_Temporary_Placement_Admin","Manage placement functionalities by HR personnel","HR"),
	new ApplicationRole("HR_Placement_User","Manage placement functionalities by HR personnel","HR"),
	new ApplicationRole("HR_Attendance_User","Manage attendance functionalities by HR personnel","HR"),
	new ApplicationRole("HR_Benefit_Admin","Manage benefit functionalities by HR personnel","HR"),
	new ApplicationRole("HR_Benefit_User","Manage benefit functionalities by HR personnel","HR"),
	new ApplicationRole("HR_Training_User","Manage training functionalities by HR personnel","HR"),
	new ApplicationRole("HR_Training_Need_Approver","approve training functionalities by HR training  depart Need","HR"),
	new ApplicationRole("HR_Training_Approver","Approve , pend or reject all training  requests","HR"),
	new ApplicationRole("HR_Performance_Admin","Manage all performance settings and functionalities","HR"),
	new ApplicationRole("HR_Performance_User","Manage performance functionalities by HR personnel","HR"),
	new ApplicationRole("HR_Performance_Report","View performance  reports","HR"),
	new ApplicationRole("HR_Discipline_User","Manage discipline functionalities by HR personnel","HR"),
	new ApplicationRole("HR_Termination_User","Manage termination functionalities by HR personnel","HR"),
	new ApplicationRole("HR_Letters_User","Manage letters functionalities by HR archiver","HR"),
	new ApplicationRole("HR_PrintableDocuments_User","Manage printable documents functionalities by HR personnel","HR"),
	new ApplicationRole("HR_Recruitment_Requester", "Request Recruitment", "HR"),
	new ApplicationRole("HR_Training_Requester", "Request Training", "HR"),
	new ApplicationRole("Announcement", "Announcement", "HR"),
	new ApplicationRole("PlacementDeActivateByBranch","De-Activate Employees Based on Branch","HRM"),


	 #endregion

				#region Archive
			new ApplicationRole("Archive_Admin","Manage archive Settings and configurations by Archive administrator","Archive "),
			new ApplicationRole("Archive_User","Manage archive functionalities by Archiver","Archive "),
			new ApplicationRole("Archive_Report","View archive  reports","Archive "),
			  #endregion

				
				#region Health
				new ApplicationRole("Health_Admin","Manage settings and configurations","Health"),
				new ApplicationRole("Health_User","Manage health and insurance functionalities","Health"),
				new ApplicationRole("Health_Approver","Approve , pend or reject all health  requests","Health"),
				#endregion

				#region insurance
				 new ApplicationRole("Insurance_Admin", "Manage settings and configurations", "Insurance"),
				 new ApplicationRole("Insurance_User", "Manage Insurance functionalities", "Insurance"),
					#endregion
			  
				#region Budget
			new ApplicationRole("Budget_Admin", "Budget Administrator", "Budget"),
			new ApplicationRole("Budget_Approver", "Budget Approver", "Budget"),
			new ApplicationRole("Budget_Report", "Budget Report", "Budget"),
			new ApplicationRole("Budget_Requestor", "Budget Requester", "Budget"),
			new ApplicationRole("Budget_User", "Budget User", "Budget"),
			new ApplicationRole("Budget_Transfer", "Budget Transfer", "Budget"),
			#endregion

				#region CRM
			new ApplicationRole(CRMRoles.CRMAdmin, "CRM Administrator - Manages all CRM Settings. Can Create, Edit and Delete CRM Settings", "CRM"),
			new ApplicationRole(CRMRoles.SalesReport, "Sales Report - Can see all CRM Reports.", "CRM"),
			new ApplicationRole(CRMRoles.SalesQuoteUser, "Sales Quote User - Create, Edit and Delete Sales Quote records", "CRM"),
			new ApplicationRole(CRMRoles.SalesQuoteFirstApprover, "Sales Quote first-level Approver", "CRM"),
			new ApplicationRole(CRMRoles.SalesQuoteSecondApprover, "Sales Quote second-level Approver", "CRM"),
			new ApplicationRole(CRMRoles.SalesQuoteApprover, "Sales Quote Approver - Approve and Reject Sales Quotes", "CRM"),
			new ApplicationRole(CRMRoles.SalesOrderUser, "Sales Order User - Create, Edit and Delete Sales Order records", "CRM"),
			new ApplicationRole(CRMRoles.SalesOrderApprover, "Sales Order Approver - Approve and Reject Sales Orders", "CRM"),
			new ApplicationRole(CRMRoles.CashSalesUser, "Cash Sales User - Create, Edit and Delete CASH SALES", "CRM"),
			new ApplicationRole(CRMRoles.CashSalesApprover, "Cash Sales Approver - Approve and Reject CASH SALES", "CRM"),
			new ApplicationRole(CRMRoles.CreditSalesUser, "Credit Sales User - Create, Edit and Delete CREDIT SALES", "CRM"),
			new ApplicationRole(CRMRoles.CreditSalesApprover, "Credit Sales Approver - Approve and Reject CREDIT SALES", "CRM"),
			new ApplicationRole(CRMRoles.CRMQueueSetting, "Responsible for Queue Related Settings (queue types, investment types and location mapping)", "CRM"),
			new ApplicationRole(CRMRoles.CRMQueueUser, "Creating and Modifying Queue Records", "CRM"),
			new ApplicationRole(CRMRoles.CRMQueueApprover, "Queue Records approver", "CRM"),
			new ApplicationRole(CRMRoles.CustomerRegistrationUser, "Add and Modify Organization Customers", "CRM"),
			new ApplicationRole(CRMRoles.ShipmentUser, "Shipment User. Can Create and Modify Shipment Records", "CRM"),
			new ApplicationRole(CRMRoles.ShipmentApprover, "Shipment Approver. Can Approver, Reject Shipment Records", "CRM"),
			new ApplicationRole(CRMRoles.CreditSettlementUser, "Credit Settlement User - Create, Edit and Delete Credit Settlements", "CRM"),
			new ApplicationRole(CRMRoles.CreditSettlementApprover, "Credit Settlement Approver - Approve and Reject Credit Settlements", "CRM"),
			new ApplicationRole(CRMRoles.AdvancePaymentUser, "Advance Payment User - Create, Edit and Delete Advance Payments", "CRM"),
			new ApplicationRole(CRMRoles.AdvancePaymentApprover, "Advance Payment Approver - Approve and Reject Advance Payments", "CRM"),
			new ApplicationRole(CRMRoles.CRMVoid, "Void sales quote, sales order, sales invoice, shipment and other vouchers", "CRM"),
			new ApplicationRole(CRMRoles.SalesPlanUser, "Sales plan user - Create, Edit and Delete Sales Plan", "CRM"),
			new ApplicationRole(CRMRoles.SalesPlanApprover, "Sales plan approver - Approve and Reject Sales Plan", "CRM"),
			new ApplicationRole(CRMRoles.CRMBatchTransfer, "CRM Batch Transfer - batch transfer Cash Sales, Credit Sales, Shipment, Credit Settlements and Deposit", "CRM"),
			new ApplicationRole(CRMRoles.IncompleteShipmentAdmin, "Incomplete and Partially Incomplete shipment admin", "CRM"),
			new ApplicationRole(CRMRoles.CRMTenderFollowup, "Tender Followup", "CRM"),
			new ApplicationRole(CRMRoles.PriceEstimationUser, "Maintains price estimation vouchers", "CRM"),
			new ApplicationRole(CRMRoles.PriceEstimationFirstApprover, "Price estimation first approver", "CRM"),
			new ApplicationRole(CRMRoles.PriceEstimationSecondApprover, "Price estimation second approver", "CRM"),
			new ApplicationRole(CRMRoles.PriceEstimationApprover, "Price estimation final approver", "CRM"),
			#endregion

				#region FLEET
  new ApplicationRole("Fleet_Admin", "Fleet Administrator", "FLEET"),
  new ApplicationRole("Registration_User", "Registration User", "FLEET"),
  new ApplicationRole("Registration_Approver", "Registration Approver", "FLEET"),
  new ApplicationRole("Rent_Requestor", "Rent Requester", "FLEET"),
  new ApplicationRole("Rent_User", "Rent User", "FLEET"),
  new ApplicationRole("Rent_Approver", "Rent Approver", "FLEET"),
  //new ApplicationRole("Maintenance_Requestor", "Maintenance Requester", "FLEET"),
  //new ApplicationRole("Maintenance_Request_Approver", "Maintenance Request Approver", "FLEET"),
  //new ApplicationRole("Maintenance_Order_User", "Maintenance Order User", "FLEET"),
  //new ApplicationRole("Maintenance_Service_Approver", "Maintenance Service Approver", "FLEET"),
  //new ApplicationRole("Service_User", "Service User", "FLEET"),
  new ApplicationRole("Fuel_Requestor", "Fuel Requester", "FLEET"),
  new ApplicationRole("Fuel_Approver", "Fuel Approver", "FLEET"),
  new ApplicationRole("Fuel_User", "Fuel User", "FLEET"),
  new ApplicationRole("Inspection_User", "Inspection User", "FLEET"),
  new ApplicationRole("Inspection_Approver", "Inspection Approver", "FLEET"),
  new ApplicationRole("Resource_Requestor", "Resource Requester", "FLEET"),
  new ApplicationRole("Resource_Distributor", "Resource Distributor", "FLEET"),
  new ApplicationRole("Resource_Return_User", "Resource Return_User", "FLEET"),
  new ApplicationRole("Resource_User", "Resource User", "FLEET"),
  new ApplicationRole("Resource_Request_Approver", "Resource Request Approver", "FLEET"),
  new ApplicationRole("Resource_Approver", "Resource Approver", "FLEET"),
  new ApplicationRole("Tyre_Requestor", "Tyre Requester", "FLEET"),
  new ApplicationRole("Tyre_Approver", "Tyre Approver", "FLEET"),
  new ApplicationRole("Tyre_User", "Tyre User", "FLEET"),
  new ApplicationRole("Fleet_Training_user", "Fleet Training user", "FLEET"),
  //new ApplicationRole("Fleet_Maintenance_Report", "Fleet Maintenance Report", "FLEET"),
  new ApplicationRole("Fleet_Report", "Fleet Report", "FLEET"),
  new ApplicationRole("Fleet_Resource_Report", "Fleet Resource Report", "FLEET"),
  new ApplicationRole("Equipment_Plan_User", "Equipment Plan User", "FLEET"),
  new ApplicationRole("Equipment_Plan_Approver", "Equipment Plan Approver", "FLEET"),
  new ApplicationRole("Salvage_Store_Issue_User", "Salvage Store Issue User", "FLEET"),
  new ApplicationRole("Salvage_Store_Issue_Approver", "Salvage Store Issue Approver", "FLEET"),
  new ApplicationRole("Store_Good_Receiving_User", "Store Good Receiving User", "FLEET"),
  new ApplicationRole("Store_Good_Receiving_Approver", "Store Good Receiving Approver", "FLEET"),
  new ApplicationRole("Fleet_Registration_Report", "Fleet Registration Report", "FLEET"),
   new ApplicationRole("Fleet_TimeSheet","Timesheet data entry","FLEET"),
	new ApplicationRole("Fleet_Requisition","Fleet Requisition","FLEET"),
	#endregion
	#region Maintenance
	new ApplicationRole("Maintenance_Void","users who can make void","Maintenance"),
	 new ApplicationRole(MaintenanceRoles.MaintenanceAdmin, "Maintenance Administrator", "Maintenance"),
	 new ApplicationRole(MaintenanceRoles.MaintenanceOrderUser, "Maintenance order user", "Maintenance"),
	 new ApplicationRole(MaintenanceRoles.ServiceUser, "Maintenance service user", "Maintenance"),
	 new ApplicationRole(MaintenanceRoles.MaintenanceRequestor, "Maintenance Request User", "Maintenance"),
	 new ApplicationRole(MaintenanceRoles.MaintenanceWorkShopUser, "Maintenance Work Shop User", "Maintenance"),
	 new ApplicationRole(MaintenanceRoles.MaintenanceRequestApprover, "Maintenance Request Approver", "Maintenance"),
	 new ApplicationRole(MaintenanceRoles.MaintenanceOrderUser, "Maintenance Job/Work Order user", "Maintenance"),
	 new ApplicationRole(MaintenanceRoles.MaintenanceServiceApprover, "Maintenance Service Approver", "Maintenance"),
	 new ApplicationRole(MaintenanceRoles.MaintenanceServiceRejecter, "Maintenance Service Rejecter", "Maintenance"),
	 new ApplicationRole(MaintenanceRoles.ServiceUser, "Service  User", "Maintenance"),
	 new ApplicationRole(MaintenanceRoles.MaintenanceReport, "Maintenance Report", "Maintenance"),
	 new ApplicationRole(MaintenanceRoles.MaintenanceEquipmentRegistrationUser, "Maintenance Equipment Registration User", "Maintenance"),
	 new ApplicationRole(MaintenanceRoles.MileageImportAndExport, "Mileage Import And Export", "Maintenance"),
	 new ApplicationRole(MaintenanceRoles.MaintenanceToolsStoreUser, "Maintenance Tools Store User", "Maintenance"),
	 new ApplicationRole(MaintenanceRoles.PeriodicEquipmentInspectionUser, "Periodic Equipment Inspection User", "Maintenance"),
	#endregion

				#region PRODUCTION
				new ApplicationRole("Production_Admin", "Production Administrator", "PRODUCTION"),
				new ApplicationRole("Production_Reporting", "Production Reporting", "PRODUCTION"),
				#endregion
				#region CMS
				new ApplicationRole(CMSRoles.CMSAdmin, "Certificate Management System Admin", "CMS"),
				new ApplicationRole(CMSRoles.TraineeRegistration, "Maintain Trainees - Add new Trainee and Company, Edit Trainee and Delete Trainee", "CMS"),
				new ApplicationRole(CMSRoles.Trainee, "Trainee (Student)", "CMS"),
				new ApplicationRole(CMSRoles.Trainer, "Trainer", "CMS"),
				new ApplicationRole(CMSRoles.Examiner, "Examiner", "CMS"),
				new ApplicationRole(CMSRoles.CMSReporting, "CMS Reports", "CMS"),
				#endregion
				 #region info desk
			new ApplicationRole("HelpDeskAdmin", "HelpDeskAdmin", "HelpDesk"),
			new ApplicationRole("OpenTicket", "OpenTicket", "HelpDesk"),
			new ApplicationRole("AssignHandler", "AssignHandler", "HelpDesk"),
			new ApplicationRole("Handler", "Handler", "HelpDesk"),
			new ApplicationRole("HelpDesk_Report", "View Report", "HelpDesk"),
			new ApplicationRole("HelpDesk_Setting", "Add Help desk settings", "HelpDesk")
			#endregion
				#region Printing
				#region Printing Estimation
                ,
             new ApplicationRole("PrintingEstimation_Officer", "", "Printing"),
                new ApplicationRole("PrintingEstimation_TeamLead", "", "Printing"),
                new ApplicationRole("PrintingEstimation_DepartmentHead", "", "Printing"),
                new ApplicationRole("PrintingProduction_Head", "", "Printing"),
                new ApplicationRole("PrintingProduction_TeamLead", "", "Printing"),
                new ApplicationRole("PrintingProduction_Officer", "", "Printing"),
                new ApplicationRole("Printing_Admin", "", "Printing"),

                new ApplicationRole("PrintingPrePress", "", "Printing"),
                new ApplicationRole("PrintingPress", "", "Printing"),
                new ApplicationRole("PrintingPostPress", "", "Printing"),
                new ApplicationRole("Print_QualityAssurance", "", "Printing"),
					#endregion
				#region Production FollowUp
					 new ApplicationRole(ProductionFollowUpRoles.PrintingProductionAdmin, "Production FollowUp Administrator", "Printing"),
					 new ApplicationRole(ProductionFollowUpRoles.PrintingProductionSetting, "Production FollowUp Setting", "Printing"),
					 new ApplicationRole(ProductionFollowUpRoles.PrintingProductionReport, "Production FollowUp Report", "Printing"),
					 new ApplicationRole(ProductionFollowUpRoles.PrintingProductionProformaUser, "Production FollowUp Proforma User", "Printing"),
					 new ApplicationRole(ProductionFollowUpRoles.PrintingProductionJobOrderUser, "Production FollowUp Job Order User", "Printing"),
					 new ApplicationRole(ProductionFollowUpRoles.PrintingProductionFinishedGoodUser, "Production FollowUp Finished Good User", "Printing"),
					 new ApplicationRole(ProductionFollowUpRoles.PrintingProductionDeliveryUser, "Production FollowUp Delivery User", "Printing"),
					#endregion
				#region Job Costing FollowUp
					 //new ApplicationRole(JobCostingRoles.JobCostingAdmin, "Job Costing Administrator", "Printing"),
					 //new ApplicationRole(JobCostingRoles.JobCostingJobCostUser, "Job Costing Job Cost User", "Printing"),
					 //new ApplicationRole(JobCostingRoles.JobCostingDailyLaborRateUser, "Job Costing Daily Labor RateUser", "Printing"),
					 //new ApplicationRole(JobCostingRoles.JobCostingIdleTimeUser, "JobCosting Idle Time User", "Printing"),
      //          new ApplicationRole("Printing_Admin", "", "Printing"),
				#endregion
			 #endregion
			   #region Manufucturing
				//#region Furniture Estimation
				//	 ,new ApplicationRole(FurnitureEstimationRoles.FurnitureEstimationAdmin, "Furniture Estimation Administrator", "Manufucturing"),
				//	 new ApplicationRole(FurnitureEstimationRoles.FurnitureEstimationUser, "Furniture Estimation Form User", "Manufucturing"),
				//	#endregion
                     #region Furniture
				new ApplicationRole("FurnitureEstimation_Officer", "", "Manufacturing"),
                new ApplicationRole("FurnitureEstimation_TeamLead", "", "Manufacturing"),
                new ApplicationRole("FurnitureEstimation_DepartmentHead", "", "Manufacturing"),
                new ApplicationRole("FurnitureProduction_Head", "", "Manufacturing"),
                new ApplicationRole("FurnitureProduction_TeamLead", "", "Manufacturing"),
                new ApplicationRole("FurnitureProduction_Officer", "", "Manufacturing"),
                new ApplicationRole("Furniture_Admin", "", "Manufacturing"),
                new ApplicationRole("Furniture_Metal", "", "Manufacturing"),
                new ApplicationRole("Furniture_Wood", "", "Manufacturing"),
                new ApplicationRole("Furniture_Assembly", "", "Manufacturing"),
				#endregion
				#region Production FollowUp
					 new ApplicationRole(ManufacturingProductionFollowUpRoles.ManufacturingProductionAdmin, "Production FollowUp Administrator", "Manufucturing"),
					 new ApplicationRole(ManufacturingProductionFollowUpRoles.ManufacturingProductionSetting, "Production FollowUp Setting", "Manufucturing"),
					 new ApplicationRole(ManufacturingProductionFollowUpRoles.ManufacturingProductionReport, "Production FollowUp Report", "Manufucturing"),
					 new ApplicationRole(ManufacturingProductionFollowUpRoles.ManufacturingProductionProformaUser, "Production FollowUp Proforma User", "Manufucturing"),
					 new ApplicationRole(ManufacturingProductionFollowUpRoles.ManufacturingProductionJobOrderUser, "Production FollowUp Job Order User", "Manufucturing"),
					 new ApplicationRole(ManufacturingProductionFollowUpRoles.ManufacturingProductionFinishedGoodUser, "Production FollowUp Finished Good User", "Manufucturing"),
					 new ApplicationRole(ManufacturingProductionFollowUpRoles.ManufacturingProductionDeliveryUser, "Production FollowUp Delivery User", "Manufucturing"),
					#endregion
				#region Job Costing FollowUp
					 new ApplicationRole(ManufacturingJobCostingRoles.ManufacturingJobCostingAdmin, "JobCosting_Admin", "Manufucturing"),
					 new ApplicationRole(ManufacturingJobCostingRoles.ManufacturingJobCostingJobCostUser, "JobCosting_JobCost_User", "Manufucturing"),
					 new ApplicationRole(ManufacturingJobCostingRoles.ManufacturingJobCostingIdleTimeUser, "JobCosting_IdleTime_User", "Manufucturing"),
					 new ApplicationRole(ManufacturingJobCostingRoles.ManufacturingJobCostingDailyLaborRateUser, "JobCosting_DailyLaborRate_User", "Manufucturing"),
				#endregion
			 #endregion
					  #region planning
 new ApplicationRole(PlanningRoles.Planning_Admin,"Create data, modify planning data","Planning"),
 new ApplicationRole(PlanningRoles.Planning_User,"See, update plan and actual","Planning"),
 new ApplicationRole(PlanningRoles.Planning_Risk_User,"See, update risk","Planning"),
 #endregion
			};

			foreach (ApplicationRole role in applicationRoles)
			{
				roleManager.Create(role);
			}


		}

		private static void AddRolesToGroups(ApplicationGroupManager groupManager)
		{
			// Add the Super-Admin Roles to the Super-Admin Group:
			IDbSet<ApplicationGroup> allGroups = _db.ApplicationGroups;
			ApplicationGroup superAdmins = allGroups.First(g => g.Name == "SuperAdmins");
			groupManager.SetGroupRoles(superAdmins.Id, _superAdminRoleNames);


			// Add the Group-Admin Roles to the Group-Admin Group:
			ApplicationGroup groupAdmins = _db.ApplicationGroups.First(g => g.Name == "GroupAdmins");
			groupManager.SetGroupRoles(groupAdmins.Id, _groupAdminRoleNames);


			// Add the User-Admin Roles to the User-Admin Group:
			ApplicationGroup userAdmins = _db.ApplicationGroups.First(g => g.Name == "UserAdmins");
			groupManager.SetGroupRoles(userAdmins.Id, _userAdminRoleNames);


			// Add the User Roles to the Users Group:
			ApplicationGroup users = _db.ApplicationGroups.First(g => g.Name == "Users");
			groupManager.SetGroupRoles(users.Id, _userRoleNames);

		}

		// Change these to your own:

		private static void AddUsers(ApplicationUserManager userManager)
		{
			var newUser = new ApplicationUser
			{
				UserName = InitialUserName,
				FirstName = InitialUserFirstName,
				LastName = InitialUserLastName,
				Email = InitialUserEmail
			};


			var userCreationResult = userManager.Create(newUser, InitialUserPassword);
			if (!userCreationResult.Succeeded)
			{
				throw new DbEntityValidationException("Could not create InitialUser because: " + String.Join(", ", userCreationResult.Errors));
			}

			var newVaniallaUser = new ApplicationUser
			{
				UserName = VaniallaUserName,
				FirstName = VaniallaUserFirstName,
				LastName = VaniallaUserLastName,
				Email = VaniallaUserEmail
			};

			// Be careful here - you  will need to use a password which will 
			// be valid under the password rules for the application, 
			// or the process will abort:
			userCreationResult = userManager.Create(newVaniallaUser, VaniallaUserPassword);
			if (!userCreationResult.Succeeded)
			{
				// warn the user that it's seeding went wrong
				throw new DbEntityValidationException("Could not create VaniallaUser because: " + String.Join(", ", userCreationResult.Errors));
			}
		}

		// Configure the initial Super-Admin user:
		private static void AddUsersToGroups(ApplicationGroupManager groupManager)
		{

			ApplicationUser user = _db.Users.First(u => u.UserName == InitialUserName);
			IDbSet<ApplicationGroup> allGroups = _db.ApplicationGroups;
			foreach (ApplicationGroup group in allGroups)
			{
				groupManager.SetUserGroups(user.Id, group.Id);
			}

			user = _db.Users.FirstOrDefault(u => u.UserName == VaniallaUserName);
			var plainUsers = allGroups.FirstOrDefault(g => g.Name == "Users");
			groupManager.SetUserGroups(user.Id, plainUsers.Id);
		}

	}
}