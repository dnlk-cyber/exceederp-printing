using ExceedERP.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace ExceedERP.Web
{

	public class ApplicationUserManager : UserManager<ApplicationUser, string>
	{
		public ApplicationUserManager(IUserStore<ApplicationUser, string> store)
			: base(store)
		{
		}

		public static ApplicationUserManager Usermgr;

		public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
		{
			var manager = new ApplicationUserManager(new UserStore<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>(context.Get<ApplicationDbContext>()));
			// Configure validation logic for usernames
			manager.UserValidator = new UserValidator<ApplicationUser>(manager)
			{
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = false
			};
			// Configure validation logic for passwords
			manager.PasswordValidator = new PasswordValidator
			{
				RequiredLength = 6,
				RequireNonLetterOrDigit = false,
				RequireDigit = false,
				RequireLowercase = false,
				RequireUppercase = false,
			};
			// Configure user lockout defaults
			manager.UserLockoutEnabledByDefault = true;
			manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromHours(2);
			manager.MaxFailedAccessAttemptsBeforeLockout = 3;
			// Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
			// You can write your own provider and plug in here.
			manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
			{
				MessageFormat = "Your security code is: {0}"
			});
			manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
			{
				Subject = "SecurityCode",
				BodyFormat = "Your security code is {0}"
			});
			manager.EmailService = new EmailService();
			manager.SmsService = new SmsService();
			var dataProtectionProvider = options.DataProtectionProvider;
			if (dataProtectionProvider != null)
			{
				manager.UserTokenProvider =
					new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
			}
			Usermgr = manager;
			return manager;
		}
	}

	// Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
	public class ApplicationRoleManager : RoleManager<ApplicationRole>
	{
		public static ApplicationRoleManager RoleManager;
		public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
			: base(roleStore)
		{
		}

		public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
		{
			RoleManager = new ApplicationRoleManager(new ApplicationRoleStore(context.Get<ApplicationDbContext>()));
			return RoleManager;
		}
	}

	//This is useful if you do not want to tear down the database each time you run the application.
	//public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
	//This example shows you how to create a new database if the Model changes
	//public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
	//{
	//    protected override void Seed(ApplicationDbContext context)
	//    {
	//        InitializeIdentityForEf(context);
	//        base.Seed(context);
	//    }
	//    private const string InitialUserName = "tester";
	//    private const string InitialUserFirstName = "TestFirstName";
	//    private const string InitialUserLastName = "TestLastName";
	//    private const string InitialUserEmail = "test@test.com";
	//    private const string InitialUserPassword = "Password1";

	//    private const string VaniallaUserName = "VanillaUser";
	//    private const string VaniallaUserFirstName = "VanillaFirstName";
	//    private const string VaniallaUserLastName = "VanillaLastName";
	//    private const string VaniallaUserEmail = "vanilla@test.com";
	//    private const string VaniallaUserPassword = "Password1";
	//    private static readonly ApplicationDbContext _db = new ApplicationDbContext();

	//    private static readonly string[] _groupAdminRoleNames = { "CanEditUser", "CanEditGroup", "User" };


	//    private static readonly string[] _initialGroupNames = { "SuperAdmins", "GroupAdmins", "UserAdmins", "Users" };


	//    private static readonly string[] _superAdminRoleNames = { "Admin", "CanEditUser", "CanEditGroup", "CanEditRole", "User" };
	//    private static readonly string[] _userAdminRoleNames = { "CanEditUser", "User" };
	//    private static readonly string[] _userRoleNames = { "User" };

	//    public static void InitializeIdentityForEf(ApplicationDbContext db)
	//    {
	//        db.Configuration.LazyLoadingEnabled = true;
	//        ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
	//        ApplicationRoleManager roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();


	//        AddRoles(roleManager);

	//        AddUsers(userManager);
	//        var groupManager = new ApplicationGroupManager();

	//        AddGroups(groupManager);
	//        AddRolesToGroups(groupManager);
	//        AddUsersToGroups(groupManager);

	//    }
	//    public static void AddGroups(ApplicationGroupManager _idManager)
	//    {
	//        foreach (var groupName in _initialGroupNames)
	//        {
	//            ApplicationGroup applicationGroup = new ApplicationGroup(groupName);

	//            _idManager.CreateGroup(applicationGroup);

	//        }
	//    }

	//    private static void AddRoles(ApplicationRoleManager roleManager)
	//    {
	//        List<ApplicationRole> applicationRoles = new List<ApplicationRole>
	//        {
	//        #region Finance
	//        new ApplicationRole("Finance_Admin","To define common finance setup","Finance"),
	//        new ApplicationRole("Finance_Report","To generate common finance report","Finance"),
	//        new ApplicationRole("FinanceConstants.GeneralLedgerUser","User will record, post, reverse, send for approval, edit, and delete diffrernt type of transaction entries","Finance"),
	//        new ApplicationRole("FinanceConstants.GeneralLedgerApprover","User will approve/reject different type of transaction ","Finance"),
	//        new ApplicationRole("FinanceConstants.GeneralLedgerView","User will view only transaction.","Finance"),
	//        new ApplicationRole("FinanceConstants.GeneralLedgerAdmin","To define chart of account segment, and journal category ","Finance"),
	//        new ApplicationRole("General_Ledger_Report","To generate GL report","Finance"),
	//        new ApplicationRole("FinanceConstants.AccountPayableView","User will view only payment and invoice transaction ","Finance"),
	//        new ApplicationRole("FinanceConstants.AccountPayableInvoice","User will record,  send for approval, transfer to GL, edit, and delete different type of supplier invoices ","Finance"),
	//        new ApplicationRole("FinanceConstants.AccountPayableApprover","User will approve supplier invoice, and payment amount ","Finance"),
	//        new ApplicationRole("FinanceConstants.AccountPayablePayment","User will record,  send for approval, transfer to GL, edit, and delete different type of supplier payment ","Finance"),
	//        new ApplicationRole("FinanceConstants.AccountPayableAdmin","To define payable setup","Finance"),
	//        new ApplicationRole("Account_Payable_Report","To generate payable invoice, and payment report","Finance"),
	//        new ApplicationRole("Account_Receivable_Report","To generate receivable report","Finance"),
	//        new ApplicationRole("Account_Receivable_Admin","To define receivable setup","Finance"),
	//        new ApplicationRole("FinanceConstants.AccountReceivableApprover","User will approve customer  invoice, and collection amount ","Finance"),
	//        new ApplicationRole("Account_Receivable_Collection","User will record, send for approval, transfer to GL, edit, and delete different type of customer collection or receipts","Finance"),
	//        new ApplicationRole("FinanceConstants.AccountReceivableInvoice","User will record, send for approval, transfer to GL, edit, and delete different type of customer invoices","Finance"),
	//        new ApplicationRole("Account_Receivable_view","User will view only receivable activities ","Finance"),
	//        new ApplicationRole("Bank_Reconciliation_View","User will view only bank reconciled and unreconciled transaction  entries ","Finance"),
	//        new ApplicationRole("Bank_Reconciliation_Admin","To define bank setup","Finance"),
	//        new ApplicationRole("Bank_Reconciliation_report","To generate different bank reconciliation report","Finance"),
	//        new ApplicationRole("Bank_Reconciliation_User ","User will reconcile ledger transaction with bank statement transaction on monthly","Finance"),
	//       #endregion

	//        #region payroll
	//       new ApplicationRole("OverTime_Actual_Record_User ", "Overtime user", "Payroll"),
	//       new ApplicationRole("Payroll_Input", "Manage Payroll input functionalities", "Payroll"),
	//       new ApplicationRole("OverTime_Request_Approver", "Approve,Manage pend or reject all Over time requests", "Payroll"),
	//       new ApplicationRole("OverTime_Request_User", "Request Over Time requests", "Payroll"),
	//       new ApplicationRole("Payroll_Admin", "Manage Payroll Settings and configurations functionalities", "Payroll"),
	//       new ApplicationRole("Payroll_Process", "Manage Payroll Process functionalities", "Payroll"),
	//       new ApplicationRole("Payroll_Distribute", "Manage Payroll Distribution functionalities", "Payroll"),
	//       new ApplicationRole("Payroll_Close", "Manage Payroll Closing functionalities", "Payroll"),
	//       new ApplicationRole("Payroll_Transfer", "Manage Payroll Transfer functionalities", "Payroll"),
	//       new ApplicationRole("Payroll_Approver", "Approve,Manage pend or reject all payroll requests", "Payroll"),
	//       new ApplicationRole("Payroll_Report", "View payroll reports", "Payroll"),
	//       new ApplicationRole("Payroll_View", "", "Payroll"),
	//       new ApplicationRole("OverTime_Actual_Record_User", "Maintain Actual Employee over time record", "Payroll"),
	//       new ApplicationRole("Payroll_Input","User will enter different type of payroll input like loan, overtime, fixed contribution…","Payroll"),
	//       new ApplicationRole("Payroll_Admin","To define payroll setup","Payroll"),
	//       new ApplicationRole("Payroll_Process","User will process payroll on monthly base, reverse process payroll, transfer transaction to GL, close period, and view transaction.","Payroll"),
	//       new ApplicationRole("Payroll_Approver","User will approve processed payroll before issued to payment ","Payroll"),
	//       new ApplicationRole("Payroll_Report","To generate different type of payroll report","Payroll"),
	//       new ApplicationRole("Payroll_View","User will view only payroll input data, process payroll status, and report.","Payroll"),
	//       new ApplicationRole("Payroll_Distribute","Manage Payroll Distribution functionalities","Payroll"),
	//       new ApplicationRole("Payroll_Close","Manage Payroll Closing functionalities","Payroll"),
	//       new ApplicationRole("Payroll_Transfer","Manage Payroll Transfer functionalities","Payroll"),
	//       new ApplicationRole("OverTime_Request_Approver","Approve,Manage pend or reject all Over time requests","Payroll"),
	//       new ApplicationRole("OverTime_Request_User","Request Over Time requests","Payroll"),
	//        #endregion

	//       #region Procurement
	//       new ApplicationRole("Procurment_UserDepartment_User", "User departments role", "Procurement"),
	//       new ApplicationRole("Procurment_UserDepartment_Approver", "Approver of user departments", "Procurement"),
	//       new ApplicationRole("Procurement_Bond_User", "Guarantee,bond and cpo", "Procurement"),
	//       new ApplicationRole("Procurement_Bond_Approver", "Guarantee,bond and cpo approver", "Procurement"),
	//       new ApplicationRole("Procurement_Supplier_User", "Supplier management user", "Procurement"),
	//       new ApplicationRole("Procurement_Supplie_Approver", "Supplier management approver", "Procurement"),
	//       new ApplicationRole("Procurement_Contract_User", "Contract management user", "Procurement"),
	//       new ApplicationRole("Procurement_Contract_Approver", "Contract management approver", "Procurement"),
	//       new ApplicationRole("Procurement_Plan_User", "Procurement need and plan management user", "Procurement"),
	//       new ApplicationRole("Procurement_Plan_Approver", "Procurement need and plan management approver", "Procurement"),
	//       new ApplicationRole("Procurement_Tender_User", "Tender,proforma and PO management user", "Procurement"),
	//       new ApplicationRole("Procurement_Tender_Approver", "Tender,proforma and PO management user approver", "Procurement"),
	//       #endregion

	//       #region Property
	//         new ApplicationRole("User_Department", "Approve store requisitions", "Property"),
	//         new ApplicationRole("Property_Count_User", "Physical count", "Property"),
	//         new ApplicationRole("Property_Count_Approver", "Physical count Approver", "Property"),
	//         new ApplicationRole("Property_Requisition_User","All users that request items from store ","Property"),
	//         new ApplicationRole("Property_User","Property Staff that handles property transactions ","Property"),
	//         new ApplicationRole("User_Department","Approver of work unit requisitions ","Property"),
	//         new ApplicationRole("Property_Approver","Approve property transactions ","Property"),
	//         new ApplicationRole("Property_Authorizer","Authorize property transactions after approval","Property"),
	//         new ApplicationRole("Property_Count_Approver","Property Physical Count Update approver","Property"),
	//         new ApplicationRole("Property_Count_User","Property Physical Count user","Property"),
	//         new ApplicationRole("Property_Admin","Setting Property lookups ","Property"),
	//         new ApplicationRole("Property_Report","View, Export and print Property related reports","Property"),
	//         new ApplicationRole("PropertyImport","Import data","Property"),
	//         new ApplicationRole("Property_StoreKeeper","Store transactions","Property"),
	//       #endregion
	//         #region Fixed Asset
	//         new ApplicationRole("FixedAsset_Requisition_User", "Fixed asset requisition user", "Fixed Asset"),
	//         new ApplicationRole("FixedAsset_Requisition_Approver", "Fixed asset requisition approver", "Fixed Asset"),
	//         new ApplicationRole("FixedAsset_User", "Fixed asset user", "Fixed Asset"),
	//         new ApplicationRole("FixedAsset_Approver", "Fixed asset approver", "Fixed Asset"),
	//         new ApplicationRole("FixedAsset_Authorizer", "Fixed Asset Authorizer", "Fixed Asset"),
	//         new ApplicationRole("FixedAsset_Count_User", "Fixed asset count user", "Fixed Asset"),
	//         new ApplicationRole("FixedAsset_Count_Approver", "Fixed Asset count approver", "Fixed Asset"),
	//         new ApplicationRole("FixedAsset_Admin", "Fixed asset settings administrator", "Fixed Asset"),
	//         new ApplicationRole("FixedAsset_Report", "Fixed Asset report user", "Fixed Asset"),
	//         new ApplicationRole("FixedAsset_Import", "Fixed asset import", "Fixed Asset"),
	//         new ApplicationRole("FixedAsset_StoreKeeper", "Fixed Asset store keeper", "Fixed Asset"),
	//         new ApplicationRole("FixedAsset_Void", "Fixed asset void", "Fixed Asset"),
	//         new ApplicationRole("FixedAsset_User","Manage Fixed Asset AssetRegistration and user card ","Fixed Asset"),
	//         new ApplicationRole("FixedAsset_Approver","Approve Fixed asset user card and assetRegistration ","Fixed Asset"),
	//         new ApplicationRole("FixedAsset_Admin","Setting Fixed Asset lookups","Fixed Asset"),
	//         new ApplicationRole("FixedAsset_Depreciation_User","Run fixed Asset AnnualDepreciation ","Fixed Asset"),
	//         new ApplicationRole("FixedAsset_Depreciation_Approver","Approve Fixed Asset AnnualDepreciation","Fixed Asset"),
	//         new ApplicationRole("FixedAsset_Report","View, Export and print,Fixed Asset Reports ","Fixed Asset"),
	//        #endregion
	//        #region HR roles
	//        new ApplicationRole("HR_Admin","HR Configuration and settings","HR"),
	//        new ApplicationRole("HR_Report","HR Reports view and print","HR"),
	//        new ApplicationRole("HR_Leave_User","Manage leave functionality by HR personnel","HR"),
	//        new ApplicationRole("HR_Leave_Admin","Manage leave setting functionality by HR personnel administrator","HR"),
	//        new ApplicationRole("HR_Leave_Approver","Approve , pend or reject all leave requests","HR"),
	//        new ApplicationRole("HR_Leave_Report","View leave reports","HR"),
	//        new ApplicationRole("HR_Employee_SelfService","Employees can view information or fill request","HR"),
	//        new ApplicationRole("HR_Manager_SelfService","Managers can view information or request from employees","HR"),
	//        new ApplicationRole("HR_Company_Admin","Manage company structure setting functionalities","HR"),
	//        new ApplicationRole("HR_Company_User","Manage company stricture functionalities by HR personnel","HR"),
	//        new ApplicationRole("HR_Profile_Admin","Manage basic employee profile  setting functionalities","HR"),
	//        new ApplicationRole("HR_Profile_User","Manage basic employee profile  setting functionalities","HR"),
	//        new ApplicationRole("HR_Recruitment_Admin","Manage recruitment settings of HR recruitment functionalities","HR"),
	//        new ApplicationRole("HR_Recruitment_User","Manage recruitment functionalities by HR personnel","HR"),
	//        new ApplicationRole("HR_Recruitment_Approver","Approve , pend or reject all recruitment requests","HR"),
	//        new ApplicationRole("HR_Recruitment_Report","View recruitment  reports","HR"),
	//        new ApplicationRole("HR_Placement_Admin","Manage placement functionalities by HR personnel","HR"),
	//        new ApplicationRole("HR_Placement_User","Manage placement functionalities by HR personnel","HR"),
	//        new ApplicationRole("HR_Attendance_User","Manage attendance functionalities by HR personnel","HR"),
	//        new ApplicationRole("HR_Benefit_Admin","Manage benefit functionalities by HR personnel","HR"),
	//        new ApplicationRole("HR_Benefit_User","Manage benefit functionalities by HR personnel","HR"),
	//        new ApplicationRole("HR_Training_User","Manage training functionalities bby HR personnel","HR"),
	//        new ApplicationRole("HR_Training_Approver","Approve , pend or reject all training  requests","HR"),
	//        new ApplicationRole("HR_Performance_Admin","Manage all performance settings and functionalities","HR"),
	//        new ApplicationRole("HR_Performance_User","Manage performance functionalities by HR personnel","HR"),
	//        new ApplicationRole("HR_Performance_Report","View performance  reports","HR"),
	//        new ApplicationRole("HR_Discipline_User","Manage discipline functionalities by HR personnel","HR"),
	//        new ApplicationRole("HR_Termination_User","Manage termination functionalities by HR personnel","HR"),
	//        new ApplicationRole("HR_Letters_User","Manage letters functionalities by HR archiver","HR"),
	//        new ApplicationRole("HR_PrintableDocuments_User","Manage printable documents functionalities by HR personnel","HR"),
	//        new ApplicationRole("HR_Recruitment_Requester", "Request Recruitment", "HR"),
	//        new ApplicationRole("HR_Training_Requester", "Request Training", "HR"),
	//         #endregion

	//        #region Archive
	//        new ApplicationRole("Archive_Admin","Manage archive Settings and configurations by Archive administrator","Archive "),
	//        new ApplicationRole("Archive_User","Manage archive functionalities by Archiver","Archive "),
	//        new ApplicationRole("Archive_Report","View archive  reports","Archive "),
	//          #endregion

	//          #region Health
	//        new ApplicationRole("Health_Admin","Manage settings and configurations","Health"),
	//        new ApplicationRole("Health_User","Manage health and insurance functionalities","Health"),
	//        new ApplicationRole("Health_Approver","Approve , pend or reject all health  requests","Health"),
	//        #endregion

	//        #region insurance
	//         new ApplicationRole("Insurance_Admin", "Manage settings and configurations", "Insurance"),
	//         new ApplicationRole("Insurance_User", "Manage Insurance functionalities", "Insurance"),
	//        #endregion
			  
	//        #region Budget
	//        new ApplicationRole("Budget_Admin", "Budget Administrator", "Budget"),
	//        new ApplicationRole("Budget_Approver", "Budget Approver", "Budget"),
	//        new ApplicationRole("Budget_Report", "Budget Report", "Budget"),
	//        new ApplicationRole("Budget_Requestor", "Budget Requester", "Budget"),
	//        new ApplicationRole("Budget_User", "Budget User", "Budget"),
	//        #endregion

	//        #region CRM
	//         new ApplicationRole("CRM_Admin", "CRM Administrator", "CRM"),
	//         new ApplicationRole("Sales_Report", "Sales Report", "CRM"),
	//         new ApplicationRole("Marketing_Report", "Marketing Report", "CRM"),
	//         new ApplicationRole("Service_Support_report", "Service Support_report", "CRM"),
	//         new ApplicationRole("Sales_Quote_User", "Sales Quote User", "CRM"),
	//         new ApplicationRole("Sales_Order_User", "Sales Order User", "CRM"),
	//         new ApplicationRole("Cash_Sales_User", "Cash Sales User", "CRM"),
	//         new ApplicationRole("Sales_Quote_Approver", "Sales Quote Approver", "CRM"),
	//         new ApplicationRole("Sales_Order_Approver", "Sales Order Approver", "CRM"),
	//         new ApplicationRole("Cash_Sales_Approver", "Cash Sales Approver", "CRM"),
	//         new ApplicationRole("Marketing_User", "Marketing User", "CRM"),
	//         new ApplicationRole("Service_Support_User", "Service_Support_User", "CRM"),
	//         new ApplicationRole("Inventory_Issue_User", "Inventory Issue User", "CRM"),
	//         new ApplicationRole("Inventory_Issue_Approver", "Inventory Issue Approver", "CRM"),
	//         new ApplicationRole("Customer_Service_User", "Customer Service User", "CRM"),
	//         new ApplicationRole("Customer_Service_Approver", "Customer Service Approver", "CRM"),
	//         new ApplicationRole("Customer_Service_Report", "Customer Service Report", "CRM"),
	//         new ApplicationRole("Credit_Settlement_User", "Credit Settlement User", "CRM"),
	//        #endregion

	//         #region FLEET
	//          new ApplicationRole("Fleet_Admin", "Fleet Administrator", "FLEET"),
	//          new ApplicationRole("Registration_User", "Registration User", "FLEET"),
	//          new ApplicationRole("Registration_Approver", "Registration Approver", "FLEET"),
	//          new ApplicationRole("Rent_Requestor", "Rent Requester", "FLEET"),
	//          new ApplicationRole("Rent_User", "Rent User", "FLEET"),
	//          new ApplicationRole("Rent_Approver", "Rent Approver", "FLEET"),
	//          new ApplicationRole("Maintenance_Requestor", "Maintenance Requester", "FLEET"),
	//          new ApplicationRole("Maintenance_Request_Approver", "Maintenance Request Approver", "FLEET"),
	//          new ApplicationRole("Maintenance_Order_User", "Maintenance Order User", "FLEET"),
	//          new ApplicationRole("Maintenance_Service_Approver", "Maintenance Service Approver", "FLEET"),
	//          new ApplicationRole("Service_User", "Service User", "FLEET"),
	//          new ApplicationRole("Fuel_Requestor", "Fuel Requester", "FLEET"),
	//          new ApplicationRole("Fuel_Approver", "Fuel Approver", "FLEET"),
	//          new ApplicationRole("Fuel_User", "Fuel User", "FLEET"),
	//          new ApplicationRole("Inspection_User", "Inspection User", "FLEET"),
	//          new ApplicationRole("Inspection_Approver", "Inspection Approver", "FLEET"),
	//          new ApplicationRole("Resource_Requestor", "Resource Requester", "FLEET"),
	//          new ApplicationRole("Resource_Distributor", "Resource Distributor", "FLEET"),
	//          new ApplicationRole("Resource_Return_User", "Resource Return_User", "FLEET"),
	//          new ApplicationRole("Resource_User", "Resource User", "FLEET"),
	//          new ApplicationRole("Resource_Request_Approver", "Resource Request Approver", "FLEET"),
	//          new ApplicationRole("Resource_Approver", "Resource Approver", "FLEET"),
	//          new ApplicationRole("Tyre_Requestor", "Tyre Requester", "FLEET"),
	//          new ApplicationRole("Tyre_Approver", "Tyre Approver", "FLEET"),
	//          new ApplicationRole("Tyre_User", "Tyre User", "FLEET"),
	//          new ApplicationRole("Fleet_Training_user", "Fleet Training user", "FLEET"),
	//          new ApplicationRole("Fleet_Maintenance_Report", "Fleet Maintenance Report", "FLEET"),
	//          new ApplicationRole("Fleet_Report", "Fleet Report", "FLEET"),
	//          new ApplicationRole("Fleet_Resource_Report", "Fleet Resource Report", "FLEET"),
	//          new ApplicationRole("Equipment_Plan_User", "Equipment Plan User", "FLEET"),
	//          new ApplicationRole("Equipment_Plan_Approver", "Equipment Plan Approver", "FLEET"),
	//          new ApplicationRole("Salvage_Store_Issue_User", "Salvage Store Issue User", "FLEET"),
	//          new ApplicationRole("Salvage_Store_Issue_Approver", "Salvage Store Issue Approver", "FLEET"),
	//          new ApplicationRole("Store_Good_Receiving_User", "Store Good Receiving User", "FLEET"),
	//          new ApplicationRole("Store_Good_Receiving_Approver", "Store Good Receiving Approver", "FLEET"),
	//         new ApplicationRole("Fleet_Registration_Report", "Fleet Registration Report", "FLEET"),

	//       #endregion


	//        };

	//        foreach (ApplicationRole role in applicationRoles)
	//        {
	//            roleManager.Create(role);
	//        }


	//    }

	//    private static void AddRolesToGroups(ApplicationGroupManager groupManager)
	//    {
	//        // Add the Super-Admin Roles to the Super-Admin Group:
	//        IDbSet<ApplicationGroup> allGroups = _db.ApplicationGroups;
	//        ApplicationGroup superAdmins = allGroups.First(g => g.Name == "SuperAdmins");
	//        groupManager.SetGroupRoles(superAdmins.Id, _groupAdminRoleNames);


	//        // Add the Group-Admin Roles to the Group-Admin Group:
	//        ApplicationGroup groupAdmins = _db.ApplicationGroups.First(g => g.Name == "GroupAdmins");
	//        groupManager.SetGroupRoles(groupAdmins.Id, _groupAdminRoleNames);


	//        // Add the User-Admin Roles to the User-Admin Group:
	//        ApplicationGroup userAdmins = _db.ApplicationGroups.First(g => g.Name == "UserAdmins");
	//        groupManager.SetGroupRoles(userAdmins.Id, _userAdminRoleNames);


	//        // Add the User Roles to the Users Group:
	//        ApplicationGroup users = _db.ApplicationGroups.First(g => g.Name == "Users");
	//        groupManager.SetGroupRoles(users.Id, _userRoleNames);

	//    }

	//    // Change these to your own:

	//    private static void AddUsers(ApplicationUserManager userManager)
	//    {
	//        var newUser = new ApplicationUser
	//        {
	//            UserName = InitialUserName,
	//            FirstName = InitialUserFirstName,
	//            LastName = InitialUserLastName,
	//            Email = InitialUserEmail
	//        };


	//        var userCreationResult = userManager.Create(newUser, InitialUserPassword);
	//        if (!userCreationResult.Succeeded)
	//        {
	//            throw new DbEntityValidationException("Could not create InitialUser because: " + String.Join(", ", userCreationResult.Errors));
	//        }

	//        var newVaniallaUser = new ApplicationUser
	//        {
	//            UserName = VaniallaUserName,
	//            FirstName = VaniallaUserFirstName,
	//            LastName = VaniallaUserLastName,
	//            Email = VaniallaUserEmail
	//        };

	//        // Be careful here - you  will need to use a password which will 
	//        // be valid under the password rules for the application, 
	//        // or the process will abort:
	//        userCreationResult = userManager.Create(newVaniallaUser, VaniallaUserPassword);
	//        if (!userCreationResult.Succeeded)
	//        {
	//            // warn the user that it's seeding went wrong
	//            throw new DbEntityValidationException("Could not create VaniallaUser because: " + String.Join(", ", userCreationResult.Errors));
	//        }
	//    }

	//    // Configure the initial Super-Admin user:
	//    private static void AddUsersToGroups(ApplicationGroupManager groupManager)
	//    {

	//        ApplicationUser user = _db.Users.First(u => u.UserName == InitialUserName);
	//        IDbSet<ApplicationGroup> allGroups = _db.ApplicationGroups;
	//        foreach (ApplicationGroup group in allGroups)
	//        {
	//            groupManager.SetUserGroups(user.Id, group.Id);
	//        }

	//        user = _db.Users.FirstOrDefault(u => u.UserName == VaniallaUserName);
	//        var plainUsers = allGroups.FirstOrDefault(g => g.Name == "Users");
	//        groupManager.SetUserGroups(user.Id, plainUsers.Id);
	//    }

	//}

	public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
	{
		public static ApplicationSignInManager SignInManager;

		public override Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool rememberMe, bool shouldLockout)
		{
			var user = UserManager.FindByNameAsync(userName).Result;
			if (user != null)
			{
				if (!user.IsEnabled)
				{
					return Task.FromResult<SignInStatus>(SignInStatus.LockedOut);
				}
			}


			return base.PasswordSignInAsync(userName, password, rememberMe, shouldLockout);
		}

		public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) :
			base(userManager, authenticationManager) { }

		public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
		{
			return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
		}

		public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
		{
			var signinmgr = new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
			SignInManager = signinmgr;
			return signinmgr;
		}
	}
}