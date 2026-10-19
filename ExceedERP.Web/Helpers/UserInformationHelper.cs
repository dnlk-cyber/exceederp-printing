using System;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Hubs;
using ExceedERP.Helpers.HR;
using ExceedERP.Core.Domain.HRM;
using System.Text;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Helpers
{
	public class EmployeeVm
	{
		public int EmployeeId { get; set; }
		public string EmployeeCode { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string FirstNameEnglish { get; set; }
		public string MiddleNameEnglish { get; set; }
		public string LastNameEnglish { get; set; }
	}
	public class EmployeeCacheVm
	{
		public int PersonId { get; set; }
		public string FullName { get; set; }
		public string EmployeeCode { get; set; }
	}
	public class EmployeeUseMapCacheVm
	{
		public int EmpId { get; set; }
		public string FullName { get; set; }
		public string UserName { get; set; }
		public string UserId { get; set; }
	}

	public class UserInformationHelper
	{
        //ExceedDbContext db = new ExceedDbContext();
        //ApplicationDbContext _dbApplication = new ApplicationDbContext();
        #region User Infromation
        public static string GetEmployeeUserNameByEmployeeId(int empId)
        {
            using (var _dbApplication = new ApplicationDbContext())
            {
                return _dbApplication.Users
                    .Where(x => x.EmployeeId == empId)
                    .Select(x => x.UserName)
                    .FirstOrDefault() ?? string.Empty;
            }
        }
        public static int GetEmployeeWorkUnitById(int employeeid)
		{
			ExceedDbContext db = new ExceedDbContext();
			int branch = 0;

			var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == employeeid && x.isCurrent);
			if (placment != null)
			{
				branch = placment.OrgStructureId;
			}

			return branch;

		}
		public static int GetEmployeeBranchById(int employeeid)
		{
			ExceedDbContext db = new ExceedDbContext();
			int branch = 0;

			var placment = db.Placements.FirstOrDefault(x => x.EmployeeId ==employeeid && x.isCurrent);
			if (placment != null)
			{
				branch = placment.BranchId;
			}

			return branch;

		}
		public static int GetEmployeeBusinessUnitById(int employeeid)
		{
			ExceedDbContext db = new ExceedDbContext();
			int bizUnitId = 0;

			var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == employeeid && x.isCurrent);
			if (placment != null)
			{
				bizUnitId = placment.BusinessUnitId;
			}

			return bizUnitId;

		}
		public static string GetEmployeeBranch(string userName)
		{

			ExceedDbContext db = new ExceedDbContext();
			string branch = "";

			var userdetail = new ApplicationDbContext();
			var userempId = userdetail.Users.Where(x => x.UserName == userName).FirstOrDefault();
			if (userempId != null)
			{

				var placment = db.Placements.Where(x => x.EmployeeId == userempId.EmployeeId && x.isCurrent).FirstOrDefault();
				if (placment != null)
				{
					var br = db.Branch.Find(placment.BranchId);
					if(br != null)
					{
						branch = br.Name;
					}
				}
			}

			return branch;

		}
		public static string GetEmployeeDepartment(string userName)
		{
			ExceedDbContext db = new ExceedDbContext();
			string department = "";

			var userdetail = new ApplicationDbContext();
			var userempId = userdetail.Users.Where(x => x.UserName == userName).FirstOrDefault();
			if (userempId != null)
			{

				var placment = db.Placements.Where(x => x.EmployeeId == userempId.EmployeeId && x.isCurrent).FirstOrDefault();
				if (placment != null)
				{
					var str = db.MainCompanyStructures.Find(placment.OrgStructureId);
					if(str != null)
					{
						department = str.Name;
					}
				}
			}

			return department;

		}
		public static string GetEmployeeCostCenter(string userName)
		{
			ExceedDbContext db = new ExceedDbContext();
			int orgId = 0;
			string CostCenter = "";

			var userdetail = new ApplicationDbContext();
			var userempId = userdetail.Users.Where(x => x.UserName == userName).FirstOrDefault();
			if (userempId != null)
			{

				var placment = db.Placements.Where(x => x.EmployeeId == userempId.EmployeeId && x.isCurrent).FirstOrDefault();
				if (placment != null)
				{
					orgId = placment.OrgStructureId;
					var allStructure = db.MainCompanyStructures;

				xx: var branchCostcenter = db.BranchCostCenter.Where(x => x.BranchId == placment.BranchId && x.OrgStructureId == orgId).FirstOrDefault();
					if (branchCostcenter != null)
					{
						return branchCostcenter.Values;
					}
					else
					{
						var check = allStructure.Where(x => x.OrgStructureId == orgId).FirstOrDefault();
						if (check != null)
						{
							if (check.Parent != 0)
							{
								orgId = check.Parent;
								goto xx;
							}
							else
							{
								return "";
							}
						}

					}

				}
			}

			return CostCenter;

		}
		public static string GetEmployeeFullName(string userName)
		{

			string fullName = "";

			//var userdetail = new ApplicationDbContext();
			//var userempId = userdetail.Users.FirstOrDefault(x => x.UserName == userName);
			//if (userempId != null)
			//{

			//    var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == userempId.EmployeeId);
			//    if (employee != null)
			//    {
			//        fullName = employee.FirstName + " " + employee.MiddleName + " " + employee.LastName;
			//    }

			//}
			fullName = GetEmployeeFullNameFromCache(userName);

			return fullName;

		}
		public static List<EmployeeCacheVm> GetEmployeeDetailFromCache()
		{
			ExceedDbContext db = new ExceedDbContext();           

			var employeeUseMapCacheVms = (List<EmployeeCacheVm>)MemoryCache.Default["EmployeeMap"];
			if (employeeUseMapCacheVms == null)
			{
				List<EmployeeCacheVm> list = new List<EmployeeCacheVm>();
				var employees = db.Person_Employee                  
					.Select(x => new { x.EmployeeId, x.EmployeeCode, x.FirstNameEnglish, x.MiddleNameEnglish, x.LastNameEnglish }).ToList();
				foreach (var emp in employees)
				{
					var record = new EmployeeCacheVm
					{
						FullName = emp.FirstNameEnglish + " " + emp.MiddleNameEnglish + " " + emp.LastNameEnglish,
						PersonId = emp.EmployeeId,
						EmployeeCode = emp.EmployeeCode
					};
					list.Add(record);
				}

				MemoryCache.Default["EmployeeMap"] = list;

			 
				return list;

			}

			else
			{
				return employeeUseMapCacheVms; 
			}

		}
		
		private static string GetEmployeeFullNameFromCache(string userName)
		{
			ExceedDbContext db = new ExceedDbContext();
			var userdetail = new ApplicationDbContext();

			var employeeUseMapCacheVms = (List<EmployeeUseMapCacheVm>)MemoryCache.Default["EmployeeUseMap"];
			if (employeeUseMapCacheVms == null)
			{
				List<EmployeeUseMapCacheVm> list = new List<EmployeeUseMapCacheVm>();
				var users = userdetail.Users
					.Where(x => x.EmployeeId != 0)
					.Select(x => new { x.UserName, x.Id, x.EmployeeId }).ToList();

				var employees = db.Person_Employee.Select(x => new { x.EmployeeId, x.FirstName, x.MiddleName, x.LastName }).ToList();
				foreach (var user in users)
				{
					if (user.EmployeeId == 0)
						continue;

					int empid = user.EmployeeId;
					var employee = employees.FirstOrDefault(x => x.EmployeeId == empid);
					if (employee != null)
					{
						var record = new EmployeeUseMapCacheVm
						{
							FullName = employee.FirstName + " " + employee.MiddleName + " " + employee.LastName,
							EmpId = empid,
							UserName = user.UserName,
							UserId = user.Id
						};
						list.Add(record);
					}
				}

				MemoryCache.Default["EmployeeUseMap"] = list;

				var fullName = list.FirstOrDefault(x => x.UserName == userName)?.FullName;
				return fullName;

			}

			else
			{
				var fullName = employeeUseMapCacheVms.FirstOrDefault(x => x.UserName == userName)?.FullName;
				return fullName;
			}

		}
		public static async Task<string> GetEmployeeFullNameAsync(string userName)
		{
			ExceedDbContext db = new ExceedDbContext();
			string fullName = "";

			var userdetail = new ApplicationDbContext();
			var userempId = await userdetail.Users.Where(x => x.UserName == userName).FirstOrDefaultAsync();
			if (userempId != null)
			{

				var employee = await db.Person_Employee.Where(x => x.EmployeeId == userempId.EmployeeId)
					.FirstOrDefaultAsync();
				if (employee != null)
				{
					fullName = employee.FirstName + " " + employee.MiddleName + " " + employee.LastName;
				}

			}

			return fullName;

		}
		public static string GetEmployeeFullNameById(int id)
		{
			ExceedDbContext db = new ExceedDbContext();


			string fullName = "";




			var employee = db.Person_Employee.Where(x => x.EmployeeId == id).FirstOrDefault();
			if (employee != null)
			{
				fullName = employee.FirstName + "-" + employee.MiddleName + "-" + employee.LastName;
			}



			return fullName;

		}

		public static int GetEmployeeId(string userName)
		{
			using (var _dbApplication = new ApplicationDbContext())
			{
				// Fetch the EmployeeId directly from the Users table without tracking
				var employeeId = _dbApplication.Users
					.AsNoTracking()
					.Where(x => x.UserName == userName)
					.Select(x => (int?)x.EmployeeId) // Cast EmployeeId to int? for nullable type
					.FirstOrDefault();

				return employeeId ?? 0; // If EmployeeId is null, return 0
			}
		}


		//public static int GetEmployeeId(string userName)
		//{
		//	ExceedDbContext db = new ExceedDbContext();

		//	int EmployeeId = 0;

		//	var userdetail = new ApplicationDbContext();
		//	var userempId = userdetail.Users.Where(x => x.UserName == userName).FirstOrDefault();
		//	if (userempId != null)
		//	{

		//		var employee = db.Person_Employee.Where(x => x.EmployeeId == userempId.EmployeeId).FirstOrDefault();
		//		if (employee != null)
		//		{
		//			EmployeeId = employee.EmployeeId;
		//		}

		//	}

		//	return EmployeeId;

		//}

		public static string GetUserId(string userName)
		{          

			string userid = "";

			var db = new ApplicationDbContext();
			var user = db.Users.Where(x => x.UserName == userName).FirstOrDefault();
			if (user != null)
			{

				userid= user.Id;

			}

			return userid;

		}

		public static List<int> GetUserBusinessUnits(string userName)
		{
			ExceedDbContext db = new ExceedDbContext();
			var userid = GetUserId(userName);
			List<int> bizmap = null;

			bizmap = db.UserBusinessUnitMaps
								.Where(x => x.UserId == userid)
								.Select(x => x.BusinessUnitId).ToList();

			return bizmap;

		}
		public static List<EmployeeIdView> GetEmployeesByReportsTo(string user)
		{
			ExceedDbContext db = new ExceedDbContext(); db = new ExceedDbContext();
			var employees = new List<EmployeeIdView>();
			var employeeIdView = new EmployeeIdView();
			var allEmployee = db.Person_Employee.Where(x => x.IsActive).ToList();
			int bossId = UserInformationHelper.GetEmployeeId(user);

			var placments = db.Placements.Where(y => y.EmployeeId == bossId && y.isCurrent).FirstOrDefault();
			if (placments != null)
			{
				var deptid = placments.OrgStructureId;
				var list = GetEmployeesByReportsTo(deptid).Where(z => z.IsActive).ToList();
				//for other hr will be enabled
				//var userdetail = allEmployee.Where(x => x.EmployeeId == bossId).FirstOrDefault(); 
				var userdetail = allEmployee;
				//list.Add(userdetail);
				list.AddRange(userdetail);
				foreach (var emp in list)
				{

					employeeIdView = new EmployeeIdView();
					employeeIdView.PersonId = emp.EmployeeId;
					employeeIdView.EmployeeCode = emp.EmployeeCode;
					employeeIdView.FullNameWithOutCode = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
					employeeIdView.FullName = emp.EmployeeCode + "- " + emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
					employeeIdView.IsActive = emp.IsActive;
					employees.Add(employeeIdView);

				}
			}

			return employees;

		}
		public static List<Person_Employee> GetEmployeesByReportsTo(int reportstoId)
		{
			ExceedDbContext db = new ExceedDbContext();
			var list = new List<Person_Employee>();
			var allEmployee = db.Person_Employee.Where(x => x.IsActive).ToList();
			var placements = db.Placements.Where(x => x.ReportsTo == reportstoId && x.isCurrent).ToList();
			if (placements.Any())
			{
				foreach (var p in placements)
				{
					var check = allEmployee.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
					if (check != null)
					{
						list.Add(check);
					}
				}
			}
			return list;

		}

        #endregion

        public static void UpdateDefalutBranchAndBusinessunit(int empId, int branchId, int businessUnitId)
        {
            string userId = GetEmployeeUserId(empId);

            using (var _appDbContext = new ApplicationDbContext())
            using (var _exceedDbContext = new ExceedDbContext())
            {
                using (var transaction = _appDbContext.Database.BeginTransaction()) //  Use DbContextTransaction
                {
                    try
                    {
                        // Update User Branches
                        var existingUserBranch = _appDbContext.UserBranches
                            .FirstOrDefault(ub => ub.ApplicationUserId == userId && ub.UserBranchId == branchId.ToString());

                        var defaultUserBranches = _appDbContext.UserBranches
                            .Where(ub => ub.ApplicationUserId == userId && ub.IsDefault).ToList();

                        foreach (var ub in defaultUserBranches)
                        {
                            ub.IsDefault = false;
                        }

                        if (existingUserBranch == null)
                        {
                            _appDbContext.UserBranches.Add(new UserBranchGroup
                            {
                                ApplicationUserId = userId,
                                UserBranchId = branchId.ToString(),
                                IsDefault = true
                            });
                        }
                        else
                        {
                            existingUserBranch.IsDefault = true;
                        }
                        _appDbContext.SaveChanges();

                        // Update User Business Units
                        var existingUserBusinessUnit = _exceedDbContext.UserBusinessUnitMaps
                            .FirstOrDefault(ub => ub.UserId == userId && ub.BusinessUnitId == businessUnitId);

                        var defaultUserBusinessUnits = _exceedDbContext.UserBusinessUnitMaps
                            .Where(ub => ub.UserId == userId && ub.IsDefault).ToList();

                        foreach (var ub in defaultUserBusinessUnits)
                        {
                            ub.IsDefault = false;
                        }

                        if (existingUserBusinessUnit == null)
                        {
                            _exceedDbContext.UserBusinessUnitMaps.Add(new UserBusinessUnitMap
                            {
                                UserId = userId,
                                BusinessUnitId = businessUnitId,
                                IsDefault = true
                            });
                        }
                        else
                        {
                            existingUserBusinessUnit.IsDefault = true;
                        }
                        _exceedDbContext.SaveChanges();

                        transaction.Commit(); //  Commit transaction
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback(); //  Rollback on error
                        throw;
                    }
                }
            }
        }

        public static List<int> GetUserReportsToAndChildWorkUnits(string userName)
		{
			ExceedDbContext db = new ExceedDbContext();
			List<int> list = new List<int>();

			var userdetail = new ApplicationDbContext();
			var userempId = userdetail.Users.FirstOrDefault(x => x.UserName == userName);
			if (userempId != null)
			{

				var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == userempId.EmployeeId && x.isCurrent);
				if (placment != null)
				{
					list.Add(placment.ReportsTo);
					var childOrds = db.MainCompanyStructures.Where(x => x.Parent == placment.ReportsTo).Select(x => x.OrgStructureId).ToList();
					list.AddRange(childOrds);
				}
			}


			return list;

		}
		public static int GetEmployeeReportsTo(int empid)
		{
			ExceedDbContext db = new ExceedDbContext();
			int orgId = 0;

			var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empid && x.isCurrent);
			if (placement != null)
			{
				orgId = placement.ReportsTo;
			}


			return orgId;

		}
		//should return BranchId
		public static int GetEmployeeBranchId(string userName)
		{
			ApplicationDbContext _dbApplication = new ApplicationDbContext();
			int branchId = 0;
			string defaultBranch = "";//for create
			var userInfo = _dbApplication.Users.Where(x => x.UserName == userName).FirstOrDefault();
			if (userInfo != null)
			{
				var branchObj = _dbApplication.UserBranches.Where(x => x.ApplicationUserId == userInfo.Id).Where(x => x.IsDefault).ToList();
				branchId = branchObj.Select(x => Convert.ToInt32(x.UserBranchId)).FirstOrDefault();
			}

			return branchId;
		}
		//should return OrgStructureId
		public static int GetEmployeeCostCenterId(string userName)
		{
			ExceedDbContext db = new ExceedDbContext();
			//int department = 1;
			int orgId = 0;
			//string CostCenter = "";

			var userdetail = new ApplicationDbContext();
			var userempId = userdetail.Users.Where(x => x.UserName == userName).FirstOrDefault();
			if (userempId != null)
			{

				var placment = db.Placements.Where(x => x.EmployeeId == userempId.EmployeeId && x.isCurrent).FirstOrDefault();
				if (placment != null)
				{
					orgId = placment.OrgStructureId;
					var allStructure = db.MainCompanyStructures;

				xx: var branchCostcenter = db.BranchCostCenter.Where(x => x.BranchId == placment.BranchId && x.OrgStructureId == orgId).FirstOrDefault();
					if (branchCostcenter != null)
					{
						return branchCostcenter.BranchCostCenterId;
					}
					else
					{
						var check = allStructure.Where(x => x.OrgStructureId == orgId).FirstOrDefault();
						if (check != null)
						{
							if (check.Parent != 0)
							{
								orgId = check.Parent;
								goto xx;
							}
							else
							{
								return 0;
							}
						}

					}

				}
			}


			return orgId;

		}

		public static int GetEmployeeDepartmentId(string userName)
		{
			ExceedDbContext db = new ExceedDbContext();

			int department = 0;

			ApplicationDbContext userdetail = new ApplicationDbContext();
			var userempId = userdetail.Users.Where(x => x.UserName == userName).FirstOrDefault();
			if (userempId != null)
			{

				var placment = db.Placements.Where(x => x.EmployeeId == userempId.EmployeeId && x.isCurrent).FirstOrDefault();
				if (placment != null)
				{
					department = placment.OrgStructureId;
				}
			}

			return department;

		}

		public static int GetEmployeeDepartmentIdByEmpId(int empid)
		{
			ExceedDbContext db = new ExceedDbContext();

			int department = 0;

			var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == empid && x.isCurrent);
			if (placment != null)
			{
				department = placment.OrgStructureId;
			}

			return department;

		}

		public static List<string> GetUserBranchs(string user)
		{
			ApplicationDbContext _dbApplication = new ApplicationDbContext();
			var userInfo = _dbApplication.Users.FirstOrDefault(x => x.UserName == user);
			if (userInfo != null)
			{
				var branchesUserBelongsTo = _dbApplication.UserBranches.Where(x => x.ApplicationUserId == userInfo.Id).Select(x => x.UserBranchId).ToList();
				return branchesUserBelongsTo;
			}
			return new List<string>();
		}

		public static List<int> GetUserBranchsAsInt(string user)
		{
			ApplicationDbContext _dbApplication = new ApplicationDbContext();
			var userInfo = _dbApplication.Users.FirstOrDefault(x => x.UserName == user);
			if (userInfo != null)
			{
				var branchesUserBelongsTo = _dbApplication.UserBranches.Where(x => x.ApplicationUserId == userInfo.Id).Select(x => x.UserBranchId).ToList();
			   return branchesUserBelongsTo.Select(int.Parse).ToList();
			}
			return new List<int>();
		}

		public static int GetUserPlacementBusinessUnit(string user)
		{
			ExceedDbContext db = new ExceedDbContext();
			int bizUnit = 0;
			int empid = GetUserEmployeeId(user);
			var placement = db.Placements
				 .Where(x => x.EmployeeId == empid)
				 .Where(x => x.isCurrent).FirstOrDefault();

			if (placement != null)
				bizUnit = placement.BusinessUnitId;

			return bizUnit;
		}

		//for store keeper
		public static List<string> GetUserStores(string user)
		{
			ExceedDbContext db = new ExceedDbContext();
			ApplicationDbContext _dbApplication = new ApplicationDbContext();
			var userInfo = _dbApplication.Users.FirstOrDefault(x => x.UserName == user);
			if (userInfo != null)
			{
				var storesUserBelongsTo = db.StoreKeeperAssignments.Where(x => x.StoreKeeper == userInfo.Id).Select(x => x.StoreCode).ToList();
				return storesUserBelongsTo;
			}
			return new List<string>();
		}

		public static List<string> GetUserBanks(string user)
		{
			ExceedDbContext db = new ExceedDbContext();
			ApplicationDbContext _dbApplication = new ApplicationDbContext();
			var userInfo = _dbApplication.Users.FirstOrDefault(x => x.UserName == user);
			if (userInfo != null)
			{
				var banksUserBelongsTo = db.BankUserAssignments.Where(x => x.User == userInfo.Id).Select(x => x.BankAccount).ToList();
				return banksUserBelongsTo;
			}
			return new List<string>();
		}

		public static string GetUserDefaultBranch(string user)
		{
			using (var _dbApplication = new ApplicationDbContext())
			{
				// Fetch the User without tracking
				var defaultBranch = _dbApplication.Users                    
					.Where(x => x.UserName == user)
					.SelectMany(x => _dbApplication.UserBranches                       
						.Where(ub => ub.ApplicationUserId == x.Id && ub.IsDefault)
						.Select(ub => ub.UserBranchId)
					)
					.FirstOrDefault(); // Get the first default branch or null

				return defaultBranch ?? ""; // Return empty string if no default branch found
			}
		}

		// public static string GetUserDefaultBranch(string user)
		//{
		//	ApplicationDbContext _dbApplication = new ApplicationDbContext();
		//	string defaultBranch = "";//for create
		//	var userInfo = _dbApplication.Users.FirstOrDefault(x => x.UserName == user);
		//	if (userInfo != null)
		//	{
		//		var branchObj = _dbApplication.UserBranches.Where(x => x.ApplicationUserId == userInfo.Id).Where(x => x.IsDefault).ToList();
		//		defaultBranch = branchObj != null ? branchObj.Select(x => x.UserBranchId).FirstOrDefault() : "";
		//	}
		//	return defaultBranch;
		//}

		public static string GetUserDefaultBranchName(string user)
		{
			ExceedDbContext db = new ExceedDbContext();
			ApplicationDbContext _dbApplication = new ApplicationDbContext();
			string defaultBranch = "";//for create
			var userInfo = _dbApplication.Users.FirstOrDefault(x => x.UserName == user);
			if (userInfo != null)
			{
				var branchMapObj = _dbApplication.UserBranches
					.Where(x => x.ApplicationUserId == userInfo.Id)
					.Where(x => x.IsDefault).FirstOrDefault();
				if (branchMapObj != null)
				{
					int did = int.Parse(branchMapObj.UserBranchId);
					var branchObj = db.Branch.Find(did);
					defaultBranch = branchObj != null ? branchObj.Name : "";
				}
			}
			return defaultBranch;
		}
		public static string GetUserDefaultBusinessUnitName(string user)
		{
			if (!GlobalHelper.IsMultipleBusinessUnit())
				return "NA";
			
			ExceedDbContext db = new ExceedDbContext();
			ApplicationDbContext _dbApplication = new ApplicationDbContext();
			string defaultBusinessUnit = "";//for create
			var userInfo = _dbApplication.Users.FirstOrDefault(x => x.UserName == user);
			if (userInfo != null)
			{
				var businessUnit = db.UserBusinessUnitMaps
					.Where(x => x.UserId == userInfo.Id)
					.FirstOrDefault(x => x.IsDefault);
				if (businessUnit != null)
				{
					var businessUnitObj = db.BusinessUnits.Find(businessUnit.BusinessUnitId);
					defaultBusinessUnit = businessUnitObj != null ? businessUnitObj.Name : "";
				}
			}
			return defaultBusinessUnit;
		}

		public static int GetUserDefaultBranchId(string user)
		{
			ApplicationDbContext _dbApplication = new ApplicationDbContext();
			int defaultBranch =0;//for create
			var userInfo = _dbApplication.Users.FirstOrDefault(x => x.UserName == user);
			if (userInfo != null)
			{
				var branchObj = _dbApplication.UserBranches
					.Where(x => x.ApplicationUserId == userInfo.Id).FirstOrDefault(x => x.IsDefault);
				if(branchObj != null )
				{
					defaultBranch = int.Parse(branchObj.UserBranchId);
				}
				else
				{
					int empid=GetUserEmployeeId(user);
					defaultBranch = EmployeeHelper.GetEmployeePlacementBranch(empid);

				}
			}
			return defaultBranch;
		}

		public static int GetUserDefaultBusinessUnitId(string user)
		{
			ExceedDbContext db = new ExceedDbContext();
			ApplicationDbContext _dbApplication = new ApplicationDbContext();
			int defaultBusinessUnit = 0;//for create
			var userInfo = _dbApplication.Users.FirstOrDefault(x => x.UserName == user);
			if (userInfo != null)
			{
				var businessUnit = db.UserBusinessUnitMaps
					.Where(x => x.UserId == userInfo.Id).FirstOrDefault(x => x.IsDefault);
				if (businessUnit != null)
				{
					defaultBusinessUnit = businessUnit.BusinessUnitId;
				}
				else
				{
					int empid = GetUserEmployeeId(user);
					defaultBusinessUnit = EmployeeHelper.GetEmployeePlacementBusinessUnit(empid);
				}
			}
			return defaultBusinessUnit;
		}

		public static string GetUserFullNamebyId(string userid)
		{
			ApplicationDbContext _dbApplication = new ApplicationDbContext();
			string userFullName = "";
			var userInfo = _dbApplication.Users.Find(userid);
			if (userInfo != null)
			{
				userFullName = userInfo.FirstName + " " + userInfo.MiddleName + " " + userInfo.LastName;
			}
			return userFullName;
		}
		public static string GetUserFullName(string user)
		{
			ApplicationDbContext _dbApplication = new ApplicationDbContext();
			string userFullName = "";
			var userInfo = _dbApplication.Users.FirstOrDefault(x => x.UserName == user);
			if (userInfo != null)
			{
				userFullName = userInfo.FirstName + " " + userInfo.MiddleName + " " + userInfo.LastName;
			}
			return userFullName;
		}

		//new 
		public static List<string> GetEmployeeRoles(int empid)
		{
			string userName = GetEmployeeUserId(empid);
			List<string> roles = GetUserRoles(userName);
			return roles;
		}
		public static List<string> GetUserRoles(string userName)
		{
			ApplicationDbContext Db = new ApplicationDbContext();
			List<string> roles = new List<string>();

			var user = Db.Users.Where(x => x.UserName == userName).FirstOrDefault();
			if (user != null)
			{
				foreach(var r in user.Roles)
				{
					//roles.Add(r.RoleId);
					var role = Db.Roles.Find(r.RoleId);
					if (role != null)
						roles.Add(role.Name);
				}
			}
			return roles;

		}
		public static int GetUserDelegatorEmployeeId(string user)
		{
			
			int empId = GetUserEmployeeId(user);
			int delegatorEmpId = EmployeeHelper.GetDelegatorEmployeeId(empId);

			return delegatorEmpId;
		}
		public static int GetUserEmployeeId(string user)
		{
			int id = 0;
			ApplicationDbContext _dbApplication = new ApplicationDbContext();
			var userInfo = _dbApplication.Users.FirstOrDefault(x => x.UserName == user);
			if (userInfo != null)
			{
				try
				{
					id = userInfo.EmployeeId;
				}
				catch (Exception e)
				{
					;
				}
			}

			return id;
		}

		public static string GetEmployeeUserId(int empid)
		{
			using (var _dbApplication = new ApplicationDbContext())
			{
				// Fetch the user directly without tracking
				var employeeUserId = _dbApplication.Users
					.AsNoTracking()
					.Where(x => x.EmployeeId == empid)
					.Select(x => x.UserName)
					.FirstOrDefault(); // Get the UserName directly or null

				return employeeUserId ?? ""; // Return empty string if not found
			}
		}

		//public static string GetEmployeeUserId(int empid)
		//{
		//	string employeeUserId = "";
		//	ApplicationDbContext _dbApplication = new ApplicationDbContext();
		//	var userInfo = _dbApplication.Users.FirstOrDefault(x => x.EmployeeId == empid);
		//	if (userInfo != null)
		//	{
		//		try
		//		{
		//			employeeUserId = userInfo.UserName;
		//		}
		//		catch (Exception e)
		//		{
		//			;
		//		}
		//	}

		//	return employeeUserId;
		//}


		//public static int GetEmployeeReportsTo(int empid)
		//{
		//    ExceedDbContext db = new ExceedDbContext();
		//    int orgId = 0;

		//    var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empid && x.isCurrent);
		//    if (placement != null)
		//    {
		//        orgId = placement.ReportsTo;
		//    }


		//    return orgId;

		//}

		public static int GetUserReportsTo(string userName)
		{
			ExceedDbContext db = new ExceedDbContext();
			int orgId = 0;

			var userdetail = new ApplicationDbContext();
			var userempId = userdetail.Users.FirstOrDefault(x => x.UserName == userName);
			if (userempId != null)
			{

				var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == userempId.EmployeeId && x.isCurrent);
				if (placment != null)
				{
					orgId = placment.ReportsTo;

				}
			}


			return orgId;

		}
		public static int GetUserPlacementWorkUnit(string userName)
		{
			ExceedDbContext db = new ExceedDbContext();
			int orgId = 0;

			var userdetail = new ApplicationDbContext();
			var userempId = userdetail.Users.Where(x => x.UserName == userName).FirstOrDefault();
			if (userempId != null)
			{

				var placment = db.Placements.Where(x => x.EmployeeId == userempId.EmployeeId && x.isCurrent).FirstOrDefault();
				if (placment != null)
				{
					orgId = placment.OrgStructureId;

				}
			}


			return orgId;

		}

        //public static List<string> GetUsersInSameCostCenter(string user)
        //{
        //	ExceedDbContext db = new ExceedDbContext();
        //	List<string> usersinsameCostCenter = new List<string>();
        //	ApplicationDbContext _dbApplication = new ApplicationDbContext();
        //	var userInfo = _dbApplication.Users.FirstOrDefault(x => x.UserName == user);
        //	if (userInfo != null)
        //	{
        //		var userWorkUnit = db.Placements.Where(x=>x.isCurrent)
        //			.FirstOrDefault(x => x.EmployeeId == userInfo.EmployeeId);

        //		if (userWorkUnit != null)
        //		{
        //			int selectedWorkUnit = userWorkUnit.ReportsTo;
        //			usersinsameCostCenter.Add(user);
        //			foreach (var applicationUser in _dbApplication.Users.ToList())
        //			{
        //				if(GetUserCostCenter(applicationUser.UserName)==selectedWorkUnit)
        //					usersinsameCostCenter.Add(applicationUser.UserName);
        //			}
        //		}

        //	}

        //	return usersinsameCostCenter;
        //}

        //public static int GetUserCostCenter(string user)
        //{
        //	ExceedDbContext db = new ExceedDbContext();
        //	ApplicationDbContext _dbApplication = new ApplicationDbContext();
        //	var userInfo = _dbApplication.Users.FirstOrDefault(x => x.UserName == user);
        //	if (userInfo != null)
        //	{
        //		var userWorkUnit = db.Placements.Where(x => x.isCurrent)
        //			.FirstOrDefault(x => x.EmployeeId == userInfo.EmployeeId);

        //		if (userWorkUnit != null)
        //		{
        //			return userWorkUnit.ReportsTo;
        //		}
        //	}

        //	return 0;
        //}

        public static List<UserVM> GetUsersInRole(string roleName)
        {
            using (var _dbApplication = new ApplicationDbContext())
            {
                using (var db = new ExceedDbContext())
                {
                    // Fetch role ID directly
                    var role = _dbApplication.Roles
                        .AsNoTracking()
                        .FirstOrDefault(r => r.Name == roleName);

                    if (role == null)
                        return new List<UserVM>();

                    // Fetch users in the specified role
                    var userIds = _dbApplication.Users
                        .Where(u => u.Roles.Any(r => r.RoleId == role.Id))
                        .AsNoTracking()
                        .Select(u => new { u.Id, u.UserName, u.EmployeeId })
                        .ToList();

                    // Fetch employee details separately
                    var employeeIds = userIds.Select(u => u.EmployeeId).Distinct().ToList();
                    var employees = db.Person_Employee
                        .Where(e => employeeIds.Contains(e.EmployeeId))
                        .AsNoTracking()
                        .ToList();

                    // Combine data in memory
                    var userList = userIds
                        .Join(employees,
                              user => user.EmployeeId,
                              employee => employee.EmployeeId,
                              (user, employee) => new UserVM
                              {
                                  UserName = user.UserName,
                                  UserId = user.Id,
                                  FullName = string.Concat(
                                      employee.EmployeeCode, " ",
                                      employee.FirstNameEnglish, " ",
                                      employee.MiddleNameEnglish, " ",
                                      employee.LastNameEnglish)
                              })
                        .ToList();

                    return userList;
                }
            }
        }

        //public static List<UserVM> GetUsersInRole_old(string roleName)
        //{
        //	ApplicationDbContext _dbApplication = new ApplicationDbContext();
        //	ExceedDbContext db = new ExceedDbContext();
        //	List<UserVM> list = new List<UserVM>();
        //	var role = _dbApplication.Roles.FirstOrDefault(x => x.Name == roleName);

        //	var userroles = _dbApplication.Users.SelectMany(x => x.Roles).ToList();
        //	foreach (var userRole in userroles)
        //	{
        //		if (role != null && userRole.RoleId == role.Id)
        //		{
        //			var exists = list.Where(x => x.UserId == userRole.UserId);
        //			if (!exists.Any())
        //			{
        //				var userobj = _dbApplication.Users.Find(userRole.UserId);
        //				var employee = db.Person_Employee.Where(x => x.EmployeeId == userobj.EmployeeId).FirstOrDefault();
        //				if (employee != null)
        //				{
        //					var user = new UserVM
        //					{
        //						UserName = userobj.UserName,
        //						UserId = userobj.Id,
        //						FullName = employee.FullNameaEnglish,
        //					};
        //					list.Add(user);
        //				}
        //				else
        //				{
        //					var user = new UserVM
        //					{
        //						UserName = userobj.UserName,
        //						UserId = userobj.Id,
        //						FullName = userobj.FirstName + " " + userobj.MiddleName + " " + userobj.MiddleName,
        //					};
        //					list.Add(user);
        //				}
        //			}
        //		}

        //	}

        //	return list;
        //}
        public static bool CheckUserRole(string userName, string roleName)
		{
			using (var _dbApplication = new ApplicationDbContext())
			{
				// Fetch the role without tracking changes
				var role = _dbApplication.Roles.AsNoTracking().FirstOrDefault(x => x.Name == roleName);
				if (role == null) return false;

				// Fetch the user roles without tracking
				var userRoleExists = _dbApplication.Users
					.Where(x => x.UserName == userName)
					.SelectMany(x => x.Roles)
					.Any(r => r.RoleId == role.Id); // Check if the role exists directly

				return userRoleExists;
			}
		}

		//public static bool CheckUserRole_old(string userName, string roleName)
		//{
		//	ApplicationDbContext _dbApplication = new ApplicationDbContext();
		//	List<UserVM> list = new List<UserVM>();
		//	var role = _dbApplication.Roles.FirstOrDefault(x => x.Name == roleName);


		//	var userroles = _dbApplication.Users.Where(x => x.UserName == userName).SelectMany(x => x.Roles).ToList();
		//	foreach (var userRole in userroles)
		//	{
		//		if (role != null && userRole.RoleId == role.Id)
		//		{
		//			return true;
		//		}
		//	}

		//	return false;
		//}
		public static int GetUserBranchByUserName(string username)
		{
			ExceedDbContext db = new ExceedDbContext();
			ApplicationDbContext _dbApplication = new ApplicationDbContext();

			var user = _dbApplication.Users.FirstOrDefault(x => x.UserName == username);
			if (user != null)
			{
				int employeeId=user.EmployeeId;
				var place = db.Placements.FirstOrDefault(x => x.EmployeeId == employeeId);
				if (place != null)
				{
					return place.BranchId;
				}
			}

			return 0;
		}
		
		//public static List<int> GetUserReportsToAndChildWorkUnits(string userName)
		//{
		//    ExceedDbContext db = new ExceedDbContext();
		//    List<int> list = new List<int>();

		//    var userdetail = new ApplicationDbContext();
		//    var userempId = userdetail.Users.FirstOrDefault(x => x.UserName == userName);
		//    if (userempId != null)
		//    {

		//        var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == userempId.EmployeeId && x.isCurrent);
		//        if (placment != null)
		//        {
		//            list.Add(placment.ReportsTo);
		//            var childOrds = db.MainCompanyStructures.Where(x => x.Parent == placment.ReportsTo).Select(x => x.OrgStructureId).ToList();
		//            list.AddRange(childOrds);
		//        }
		//    }


		//    return list;

		//}
		public static List<int> GetUserHeadWorkUnitAndChildWorkUnits(string userName)
		{
			ExceedDbContext db = new ExceedDbContext();
			List<int> list = new List<int>();

			var userdetail = new ApplicationDbContext();
			var userempId = userdetail.Users.FirstOrDefault(x => x.UserName == userName);
			if (userempId != null)
			{

				var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == userempId.EmployeeId && x.isCurrent);
				if (placment != null)
				{
					if (placment.IsHead)
					{
						list.Add(placment.OrgStructureId);
						var childOrds = db.MainCompanyStructures.Where(x => x.Parent == placment.OrgStructureId).Select(x => x.OrgStructureId).ToList();
						list.AddRange(childOrds);
					}
					else
					{
						return list;
					}
				}
			}


			return list;

		}

		public static void ChangeDefaultBranch(int empId, int branchId)
		{
			var adb = new ApplicationDbContext();
			var username = GetEmployeeUserId(empId);
			var userId = GetUserId(username);

			var oldBranchs = adb.UserBranches.Where(x => x.ApplicationUserId == userId).ToList();
			foreach (var bra in oldBranchs)
			{
				bra.IsDefault = false;

				adb.Entry(bra).State = EntityState.Modified;
				adb.SaveChanges();
			}

			var checkBranch = adb.UserBranches.FirstOrDefault(x => x.ApplicationUserId == userId && x.UserBranchId == branchId.ToString());
			if (checkBranch != null)
			{
				checkBranch.IsDefault = true;

				adb.Entry(checkBranch).State = EntityState.Modified;
				adb.SaveChanges();
			}
			else
			{
				var branch = new UserBranchGroup
				{
					UserBranchId = branchId.ToString(),
					ApplicationUserId = userId,
					IsDefault = true
				};

				adb.UserBranches.Add(branch);
				adb.SaveChanges();
			}
		}

		//public static void HRAlertNotifications()
		//{
		//	ExceedDbContext db = new ExceedDbContext();

		//	var persons = db.Person_Employee.Where(x => x.IsActive).ToList();
		//	var head = db.RetirementSettings.FirstOrDefault(x => x.RetireSetting == RetireSetting.Heads);
		//	var performer = db.RetirementSettings.FirstOrDefault(x => x.RetireSetting == RetireSetting.Performers);
		//	var placements = db.Placements.Where(x => x.isCurrent);

		//	List<string> names = new List<string>();
		//	foreach (var per in persons)
		//	{

		//		var placement = placements.FirstOrDefault(x => x.EmployeeId == per.EmployeeId);
		//		var days = DateTime.Today.Date.Subtract(per.DateOfBirth.Date).Days;
		//		var month = days / 30;
		//		var year = (int)month / 12;

		//		if (placement != null && placement.IsHead && head != null && month == head.CurrentValue)
		//		{

		//			NotifModels notif = new NotifModels
		//			{
		//				UserId = per.FullNameaEnglish,
		//				Title = "Retiremtent Notification",
		//				Message = $" {per.FullNameaEnglish}  have left {month} months to retire",
		//				URL = "~/HRM/EmployeeProfile"
		//			};

		//			Notify.NotifyUser(notif);
		//		}
		//		else if (placement != null && !placement.IsHead && performer != null && month == performer.CurrentValue)
		//		{
		//			NotifModels notif = new NotifModels
		//			{
		//				UserId = per.FullNameaEnglish,
		//				Title = "Retiremtent Notification",
		//				Message = $" {per.FullNameaEnglish}  have left {month} months to retire",
		//				URL = "~/HRM/EmployeeProfile"
		//			};

		//			Notify.NotifyUser(notif);
		//		}

		//	}
		//	foreach (var per in persons.Where(x => x.Status == EmployeeStatus.PROBATION))
		//	{
		//		var config = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == ProbationSetting.Probation_Period.ToString());
		//		if (config != null)
		//		{
		//			// var dayss = DateTime.Today.Date.Subtract(per.HiredDate.Date).Days;
		//			var dayss = HrHelper.GetWorkingDays(per.HiredDate, DateTime.Today);
		//			var orginalday = Convert.ToInt32(config.CurrentValue);
		//			var day = Convert.ToInt32(config.CurrentValue) - 5;
		//			if (dayss >= day)
		//			{
		//				NotifModels notif = new NotifModels
		//				{
		//					UserId = per.FullNameaEnglish,
		//					Title = "Probation Notification",
		//					Message = $" {per.FullNameaEnglish}  have left few days for probation",
		//					URL = "~/HRM/EmployeeProfile"
		//				};

		//				Notify.NotifyUser(notif);
		//			}
		//		}
		//	}


		//}
	}
}