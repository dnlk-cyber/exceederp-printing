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
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Web.Areas.Common.Controllers;
using ExceedERP.Web.Areas.Inventory.Controllers;

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
         ExceedDbContext db = new ExceedDbContext();
         ApplicationDbContext _dbApplication = new ApplicationDbContext();
        #region User Infromation
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

        internal static List<int> GetUserBusinessUnits(string name)
        {
            throw new NotImplementedException();
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
                    .Where(x=>x.EmployeeId!=0)
                    .Select(x => new {x.UserName, x.Id, x.EmployeeId}).ToList();
                foreach (var user in users)
                {
                    if(user.EmployeeId==0)
                        continue;

                    int empid = user.EmployeeId;
                    var employee = db.Person_Employee.Select(x => new { x.EmployeeId,x.FirstName, x.MiddleName, x.LastName }).FirstOrDefault(x=>x.EmployeeId==empid);
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

        internal static int GetUserDefaultBusinessUnitId(string name)
        {
            return 0;
        }

        public static int GetEmployeeId(string userName)
        {
            ExceedDbContext db = new ExceedDbContext();

            int EmployeeId = 0;

            var userdetail = new ApplicationDbContext();
            var userempId = userdetail.Users.Where(x => x.UserName == userName).FirstOrDefault();
            if (userempId != null)
            {

                var employee = db.Person_Employee.Where(x => x.EmployeeId == userempId.EmployeeId).FirstOrDefault();
                if (employee != null)
                {
                    EmployeeId = employee.EmployeeId;
                }

            }

            return EmployeeId;

        }

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

        public static List<EmployeeIdView> GetEmployeesByReportsTo(string user)
        {
            ExceedDbContext db = new ExceedDbContext(); db = new ExceedDbContext();
            var employees = new List<EmployeeIdView>();
            var employeeIdView = new EmployeeIdView();
            var allEmployee = db.Person_Employee.ToList();
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
            var allEmployee = db.Person_Employee.ToList();
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

        public static int GetUserBusinessUnit(string user)
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
        public static string GetUserDefaultBranch(string user)
        {
            ApplicationDbContext _dbApplication = new ApplicationDbContext();
            string defaultBranch = "";//for create
            var userInfo = _dbApplication.Users.FirstOrDefault(x => x.UserName == user);
            if (userInfo != null)
            {
                var branchObj = _dbApplication.UserBranches.Where(x => x.ApplicationUserId == userInfo.Id).Where(x => x.IsDefault).ToList();
                defaultBranch = branchObj != null ? branchObj.Select(x => x.UserBranchId).FirstOrDefault() : "";
            }
            return defaultBranch;
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
            }
            return defaultBranch;
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
            string employeeUserId = "";
            ApplicationDbContext _dbApplication = new ApplicationDbContext();
            var userInfo = _dbApplication.Users.FirstOrDefault(x => x.EmployeeId == empid);
            if (userInfo != null)
            {
                try
                {
                    employeeUserId = userInfo.UserName;
                }
                catch (Exception e)
                {
                    ;
                }
            }

            return employeeUserId;
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
            var userempId = userdetail.Users.FirstOrDefault(x => x.UserName == userName);
            if (userempId != null)
            {

                var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == userempId.EmployeeId && x.isCurrent);
                if (placment != null)
                {
                    orgId = placment.OrgStructureId;

                }
            }


            return orgId;

        }
      
        public static List<string> GetUsersInSameCostCenter(string user)
        {
            ExceedDbContext db = new ExceedDbContext();
            List<string> usersinsameCostCenter = new List<string>();
            ApplicationDbContext _dbApplication = new ApplicationDbContext();
            var userInfo = _dbApplication.Users.FirstOrDefault(x => x.UserName == user);
            if (userInfo != null)
            {
                var userWorkUnit = db.Placements.Where(x=>x.isCurrent)
                    .FirstOrDefault(x => x.EmployeeId == userInfo.EmployeeId);
                   
                if (userWorkUnit != null)
                {
                    int selectedWorkUnit = userWorkUnit.ReportsTo;
                    usersinsameCostCenter.Add(user);
                    foreach (var applicationUser in _dbApplication.Users.ToList())
                    {
                        if(GetUserCostCenter(applicationUser.UserName)==selectedWorkUnit)
                            usersinsameCostCenter.Add(applicationUser.UserName);
                    }
                }
                
            }

            return usersinsameCostCenter;
        }

        public static int GetUserCostCenter(string user)
        {
            ExceedDbContext db = new ExceedDbContext();
            ApplicationDbContext _dbApplication = new ApplicationDbContext();
            var userInfo = _dbApplication.Users.FirstOrDefault(x => x.UserName == user);
            if (userInfo != null)
            {
                var userWorkUnit = db.Placements.Where(x => x.isCurrent)
                    .FirstOrDefault(x => x.EmployeeId == userInfo.EmployeeId);

                if (userWorkUnit != null)
                {
                    return userWorkUnit.ReportsTo;
                }
            }

            return 0;
        }

        public  static List<UserVM> GetUsersInRole(string roleName)
        {
            ApplicationDbContext _dbApplication = new ApplicationDbContext();
            List<UserVM> list = new List<UserVM>();
            var role = _dbApplication.Roles.FirstOrDefault(x => x.Name == roleName);

            var userroles = _dbApplication.Users.SelectMany(x => x.Roles).ToList();
            foreach (var userRole in userroles)
            {
                if (role != null && userRole.RoleId == role.Id)
                {
                    var exists = list.Where(x => x.UserId == userRole.UserId);
                    if (!exists.Any())
                    {
                        var userobj = _dbApplication.Users.Find(userRole.UserId);
                        var user = new UserVM
                        {
                            UserName = userobj.UserName,                           
                            UserId = userobj.Id,
                            FullName = userobj.FirstName + " " + userobj.MiddleName + " " + userobj.MiddleName,
                        };
                        list.Add(user);
                    }
                }

            }

            return list;
        }
        public static bool CheckUserRole(string userName, string roleName)
        {
            ApplicationDbContext _dbApplication = new ApplicationDbContext();
            List<UserVM> list = new List<UserVM>();
            var role = _dbApplication.Roles.FirstOrDefault(x => x.Name == roleName);


            var userroles = _dbApplication.Users.Where(x => x.UserName == userName).SelectMany(x => x.Roles).ToList();
            foreach (var userRole in userroles)
            {
                if (role != null && userRole.RoleId == role.Id)
                {
                    return true;
                }
            }

            return false;
        }
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
        /// <summary>
        /// get reports to workunit and its immediate children
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
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
    }
}