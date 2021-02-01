using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UnitOfMeasurement = ExceedERP.Core.Domain.Construction.UnitOfMeasurement;

namespace ExceedERP.Web.Areas.Construction.Controllers
{
    public class DataProviderController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        private readonly ApplicationDbContext userDb = new ApplicationDbContext();

        public static int EmployeeId
        {
            get;
            set;
        }
        public static string Owner
        {
            get;
            set;
        }
        public static int SelectedFile { get; set; }
        public static int RequestId { get; set; }
        public static string ReferenceType { get; set; }
        public static string LetterId { get; set; }
        public static int CosignId { get; set; }

        public string GetUserFullName(string userName)
        {
            var name = "";
            var userDb = new ApplicationDbContext();
            var currentUser = userDb.Users.FirstOrDefault(u => u.UserName == userName);
            if (currentUser != null)
            {
                name = $"{currentUser.FirstName} {currentUser.MiddleName} {currentUser.LastName}";
            }
            return name;

        }
        /// <summary>
        /// Returns setting attributes under a give category (or reference)
        /// </summary>
        /// <param name="Reference"></param>
        /// <returns></returns>
        public JsonResult GetCascadeAttributes(string Reference)
        {
            List<string> attributes = new List<string>();
            Reference = "Construction";
            switch (Reference)
            {
                case "Construction":

                    attributes =
                          Enum.GetValues(typeof(ConstructionSettingCategories))
                              .Cast<ConstructionSettingCategories>()
                              .Select(v => v.ToString())
                              .ToList();
                    break;
            }

            return Json(attributes.Select(s => new { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// returns the setting categories(or types) in general setting. 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCategories()
        {
            List<string> attributes = new List<string>();

            attributes =
                  Enum.GetValues(typeof(ConstructionSettingCategories))
                      .Cast<ConstructionSettingCategories>()
                      .Select(v => v.ToString())
                      .ToList();


            return Json(attributes.Select(s => new { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Returns the structure types (WBS types defined in the system)
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllStructureTypesForMapping()
        {
            var types = db.Configurations
                .Where(x => x.Reference == "STRUCTURE_TYPE")
                .Select(c => new { c.ConfigurationId, c.CurrentValue }).ToList();
            return Json(types, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllUsers()
        {
            var users = userDb.Users;

            return Json(users.Select(u => new { UserName = u.UserName, FullName = u.FirstName + " " + u.MiddleName + " " + u.LastName + " | @" + u.UserName }), JsonRequestBehavior.AllowGet);
        }
        public List<EmployeeIdView> GetEmployees(string user)
        {
            return GetPlacmentEmployees(user);
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(user);
            //List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            //EmployeeIdView EmployeeIdView = new EmployeeIdView();
            //var employeeList = db.Person_Employee.Where(x => mybraches.Contains(x.OrgBranchName));
            ////var placments = db.Placements.Where(x => mybraches.Contains(x.BranchId.ToString()) && x.isCurrent);
            ////if (employeeList.Any())
            ////{
            ////    employeeList = employeeList.AsQueryable();
            ////}
            //foreach (var emp in employeeList)
            //{
            //    //var check = placments.Where(x => x.EmployeeId == emp.EmployeeId).FirstOrDefault();
            //    //if (check != null)
            //    //{
            //        EmployeeIdView = new EmployeeIdView();
            //        EmployeeIdView.PersonId = emp.EmployeeId;
            //        EmployeeIdView.EmployeeCode = emp.EmployeeCode;
            //        EmployeeIdView.FullNameWithOutCode = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
            //        EmployeeIdView.FullName = emp.EmployeeCode + "- " + emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
            //        EmployeeIdView.IsActive = emp.IsActive;
            //        EmployeeIdViews.Add(EmployeeIdView);
            //    //}
            //}
            //var list = EmployeeIdViews != null ? EmployeeIdViews.ToList() : new List<EmployeeIdView>();
            //return list;
        }
        public List<EmployeeIdView> GetEmployeesForPlacment(string user)
        {

            List<string> mybraches = UserInformationHelper.GetUserBranchs(user);
            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            var employeeList = db.Person_Employee.Where(x => mybraches.Contains(x.OrgBranchName));

            foreach (var emp in employeeList)
            {

                EmployeeIdView = new EmployeeIdView();
                EmployeeIdView.PersonId = emp.EmployeeId;
                EmployeeIdView.EmployeeCode = emp.EmployeeCode;
                EmployeeIdView.FullNameWithOutCode = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                EmployeeIdView.FullName = emp.EmployeeCode + "- " + emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                EmployeeIdView.IsActive = emp.IsActive;
                EmployeeIdViews.Add(EmployeeIdView);

            }
            var list = EmployeeIdViews != null ? EmployeeIdViews.ToList() : new List<EmployeeIdView>();
            return list;
        }
        public List<EmployeeIdView> GetPayrollPlacmentEmployees(string user)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(user);
            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            var employeeList = db.Person_Employee.Where(x => x.IsActive);
            var placments = db.Placements.Where(x => mybraches.Contains(x.BranchId.ToString()) && x.isCurrent);
            if (employeeList.Any())
            {
                employeeList = employeeList.AsQueryable();
            }
            var lists = from e in employeeList
                        join p in placments
                            on e.EmployeeId equals p.EmployeeId
                        select (new
                        {
                            EmployeeId = e.EmployeeId,
                            EmployeeCode = e.EmployeeCode,
                            FullName = e.FirstNameEnglish + " " + e.MiddleNameEnglish + " " + e.LastNameEnglish,
                            FullNameCode = e.EmployeeCode + "- " + e.FirstNameEnglish + " " + e.MiddleNameEnglish + " " + e.LastNameEnglish,
                            IsActive = e.IsActive,
                        });
            lists = lists.Distinct();
            foreach (var emp in lists)
            {

                EmployeeIdView = new EmployeeIdView();
                EmployeeIdView.PersonId = emp.EmployeeId;
                EmployeeIdView.EmployeeCode = emp.EmployeeCode;
                EmployeeIdView.FullNameWithOutCode = emp.FullName;
                EmployeeIdView.FullName = emp.FullNameCode;
                EmployeeIdView.IsActive = emp.IsActive;
                EmployeeIdViews.Add(EmployeeIdView);

            }
            var list = EmployeeIdViews != null ? EmployeeIdViews.ToList() : new List<EmployeeIdView>();
            return list;
        }
        public List<string> GetUserBranchEmployees(string user)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(user);
            var placments = from p in db.Placements
                            join
                                e in db.Person_Employee
                                on p.EmployeeId equals e.EmployeeId
                            where mybraches.Contains(p.BranchId.ToString()) && p.isCurrent && e.IsActive
                            select (new
                            {
                                EmployeeId = p.EmployeeId
                            });
            if (placments.Any())
            {
                var listOfEmployees = placments.Select(x => x.EmployeeId.ToString()).ToList();
                return listOfEmployees;
            }
            return new List<string>();
        }
        public List<EmployeeIdView> GetPlacmentEmployees(string user)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(user);
            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            var employeeList = db.Person_Employee.Where(x => x.IsActive);
            var placments = db.Placements.Where(x => mybraches.Contains(x.BranchId.ToString()) && x.isCurrent);
            if (employeeList.Any())
            {
                employeeList = employeeList.AsQueryable();
            }
            var lists = from e in employeeList
                        join p in placments
                            on e.EmployeeId equals p.EmployeeId
                        select (new
                        {
                            EmployeeId = e.EmployeeId,
                            EmployeeCode = e.EmployeeCode,
                            FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName,
                            FullNameCode = e.EmployeeCode + "- " + e.FirstName + " " + e.MiddleName + " " + e.LastName,
                            IsActive = e.IsActive,
                        });
            lists = lists.Distinct();
            foreach (var emp in lists)
            {

                EmployeeIdView = new EmployeeIdView();
                EmployeeIdView.PersonId = emp.EmployeeId;
                EmployeeIdView.EmployeeCode = emp.EmployeeCode;
                EmployeeIdView.FullNameWithOutCode = emp.FullName;
                EmployeeIdView.FullName = emp.FullNameCode;
                EmployeeIdView.IsActive = emp.IsActive;
                EmployeeIdViews.Add(EmployeeIdView);

            }
            var list = EmployeeIdViews != null ? EmployeeIdViews.ToList() : new List<EmployeeIdView>();
            return list;
        }
        public List<EmployeeIdView> GetAllPlacmentEmployees(string user)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(user);
            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            var employeeList = db.Person_Employee.AsQueryable();
            var placments = db.Placements.Where(x => mybraches.Contains(x.BranchId.ToString()) && x.isCurrent);

            var lists = from e in employeeList
                        join p in placments
                            on e.EmployeeId equals p.EmployeeId
                        select (new
                        {
                            EmployeeId = e.EmployeeId,
                            EmployeeCode = e.EmployeeCode,
                            FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName,
                            FullNameCode = e.EmployeeCode + "- " + e.FirstName + " " + e.MiddleName + " " + e.LastName,
                            IsActive = e.IsActive,
                        });
            lists = lists.Distinct();
            foreach (var emp in lists)
            {

                EmployeeIdView = new EmployeeIdView();
                EmployeeIdView.PersonId = emp.EmployeeId;
                EmployeeIdView.EmployeeCode = emp.EmployeeCode;
                EmployeeIdView.FullNameWithOutCode = emp.FullName;
                EmployeeIdView.FullName = emp.FullNameCode;
                EmployeeIdView.IsActive = emp.IsActive;
                EmployeeIdViews.Add(EmployeeIdView);

            }
            var list = EmployeeIdViews != null ? EmployeeIdViews.ToList() : new List<EmployeeIdView>();
            return list;
        }
        public JsonResult GetFiscalYear()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public List<EmployeeIdView> GetEmployees()
        {

            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            var employeeList = db.Person_Employee.AsQueryable();

            if (employeeList.Any())
            {
                employeeList = employeeList.AsQueryable();
            }
            foreach (var emp in employeeList)
            {

                EmployeeIdView = new EmployeeIdView();
                EmployeeIdView.PersonId = emp.EmployeeId;
                EmployeeIdView.EmployeeCode = emp.EmployeeCode;
                EmployeeIdView.FullNameWithOutCode = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                EmployeeIdView.FullName = emp.EmployeeCode + "- " + emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                EmployeeIdView.IsActive = emp.IsActive;
                EmployeeIdViews.Add(EmployeeIdView);

            }

            var list = EmployeeIdViews != null ? EmployeeIdViews.ToList() : new List<EmployeeIdView>();
            return list;
        }

        public JsonResult GetUom()
        {
            var uom = db.UnitOfMeasurement;

            return Json(uom.Select(u => new { u.Abbreviation, u.UnitOfMeasurementId}), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// this returns the projects with status equal to estimation ....
        /// </summary>
        /// <returns></returns>
        public JsonResult GetProjectsListToCopyEstimationTo()
        {
            var db = new ExceedDbContext();
            var projectList = db.ProjectInformation.Where(x => x.ProjectStatus == ProjectStatus.ESTIMATION).OrderBy(x => x.Name);

            return Json(projectList.Select(p => new { ProjectInformationId = p.ProjectInformationId, Name = p.Name }), JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// get approved projects to copy estimation from
        /// </summary>
        /// <param name="ProjectInformationIdTwo"></param>
        /// <returns></returns>
        public JsonResult GetApprovedProjects(int ProjectInformationIdTwo)
        {
            var items = new List<ProjectInformation>();
            try
            {
                var currentProject = db.ProjectInformation.Where(x => x.ProjectInformationId == ProjectInformationIdTwo).FirstOrDefault();

                if (currentProject != null)
                {
                    items = db.ProjectInformation.Where(x => x.ProjectStatus == ProjectStatus.APPROVED &&
                                                        x.ProjectType == currentProject.ProjectType)
                                                        .ToList();
                }

                return Json(items.Select(p => new { Name = p.Name, ProjectInformationIdTwo = p.ProjectInformationId }),
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
            }
            return Json(items.Select(p => new { Name = p.Name, ProjectInformationId = p.ProjectInformationId }),
                JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// This returns all the projects 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllProjects()
        {
            var db = new ExceedDbContext();
            var projectList = db.ProjectInformation.OrderBy(x => x.Name);

            return Json(projectList.Select(p => new { ProjectInformationId = p.ProjectInformationId, Name = p.Name }), JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// this returns the sub-projects under a given project
        /// if project type is Building, subprojects are blocks
        /// if the project type is Road, subprojects are roads and road segments
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public JsonResult GetBlocks(int projectId)
        {
            var blocks = db.Blocks.Where(x => x.ProjectInformationId == projectId).ToList();
            return Json(blocks.Select(p => new { Name = p.Name, BlockId = p.BlockId }),
              JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSubProjectTypes(int projectId)
        {
            var blocks = db.SubProjectType.Where(x => x.ProjectInformationId == projectId).ToList();
            return Json(blocks.Select(p => new { p.Name, p.SubProjectTypeId }),
              JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStructureTypeMappingsWithinGivenSubType(int sourceSubTypeId, int destinationSubTypeMappingId)
        {
            var targetMapping = db.SubProjectStructureTypeMapping.Find(destinationSubTypeMappingId);
            if (targetMapping != null)
            {
                var mappings = db.SubProjectStructureTypeMapping.Where(m => m.SubProjectTypeId == sourceSubTypeId && m.ConfigurationId == targetMapping.ConfigurationId);
                return Json(mappings.Select(m => new { SubProjectStructureTypeMappingId = m.SubProjectStructureTypeMappingId, Name = m.Configuration.CurrentValue }), JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult GetWorkCategoryUnderStructureMapping(int sourceSubProjTypeMappingId)
        {

            var strMapping = db.SubProjectStructureTypeMapping.Find(sourceSubProjTypeMappingId);
            if (strMapping != null)
            {
                var catIds = new HashSet<int>(
                db.GenericWorkItemPerProject.Where(w => w.SubProjectTypeId == strMapping.SubProjectTypeId && w.WorkCategory.ConfigurationId == strMapping.ConfigurationId)
                .ToList()
                .Select(w => w.WorkCategoryId));
                var categories = db.WorkCategory.Where(c => catIds.Contains(c.WorkCategoryId)).ToList();
                return Json(categories.Select(m => new { m.WorkCategoryId, m.Name }), JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        
        public JsonResult GetWorkCategoryUnderStructureAndSpecificBlockMapping(int blockId, int configId)
        {

            var strMapping = db.BlockMappings.FirstOrDefault(m => m.BlockId == blockId && m.ConfigurationId == configId);
            if (strMapping != null)
            {
                var catIds = new HashSet<int>(
                db.WorkItemPerProject.Where(w => w.BlockId == blockId && w.WorkCategory.ConfigurationId == strMapping.ConfigurationId)
                .ToList()
                .Select(w => w.WorkCategoryId));
                var categories = db.WorkCategory.Where(c => catIds.Contains(c.WorkCategoryId)).ToList();
                return Json(categories.Select(m => new { m.WorkCategoryId, m.Name }), JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult GetConstructionPeriods()
        {
            var periods = db.ConstructionPeriod.ToList();

            return Json(periods.Select(p => new { ConstructionPeriodId = p.ConstructionPeriodId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetConstructionPeriodsWithinProjectContractTime(int projectId)
        {
            var mainContract = ContractMgmtHelper.GetProjectMainContract(projectId);
            //var periods = db.ConstructionPeriod.Where(p => p.Status == FiscalYearStatus.Open && p.DateTo > mainContract.CommencementDate);
            var periods = db.ConstructionPeriod.Where(p => p.Status == FiscalYearStatus.Open);

            return Json(periods.Select(p => new { ConstructionPeriodId = p.ConstructionPeriodId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        //
        // for populating the drop down in material price index
        public JsonResult GetItemCategory()
        {
            var items = db.ConstructionItemCategories.ToList();
            return Json(items.Select(p => new { ItemCategoryCode = p.ItemCategoryCode, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult AllItemCategoryFromInventory()
        {
            var items = db.ItemCategorys.Where(c => !c.HaveParent).Select(p => new { p.ItemCategoryCode, p.Name }).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItems(string ItemCategoryCode, string filterString)
        {
            var items = db.InventoryItems.Where(x => x.ItemCategoryCode == ItemCategoryCode).Select(p => new { p.ItemCode, p.Name });
            if (!string.IsNullOrEmpty(filterString))
            {
                filterString = filterString.ToLower();
                items = items.Where(i => i.Name.ToLower().Contains(filterString) || i.ItemCode.Contains(filterString));
            }

            return Json(items.Select(p => new { ItemCode = p.ItemCode, Name = p.Name + "  " + p.ItemCode }), JsonRequestBehavior.AllowGet);

        }
        public LargeJsonResult GetAllMaterials()
        {
            //db.Configuration.ProxyCreationEnabled = false;
            var items = db.InventoryItems.Select(item => new { item.Name, item.ItemCode });
            return new LargeJsonResult
            {
                MaxJsonLength = int.MaxValue,
                JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet,
                Data = items

            };
            //return Json(items.Select(p => new { ItemCode = p.ItemCode, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        #region Project Material Expense 
        //
        //  summary:
        //      Returns the category of items that are in the Good Receive Items table 
        public JsonResult GetItemCategoryOfReceivedItems(int selectedProjectId)
        {
            int branchId = 0;
            var currentProject = db.ProjectInformation.Find(selectedProjectId);
            if (currentProject != null)
            {
                branchId = currentProject.BranchId;
            }
            var categoriesInGoodRecieve = (from c in db.ItemCategorys
                                           join g in db.GoodReceiveItems
                                           on c.ItemCategoryCode equals g.ItemCategoryCode
                                           where g.GoodReceive.BranchId == branchId
                                           select new
                                           {
                                               ItemCategoryCode = c.ItemCategoryCode,
                                               Name = c.Name
                                           }).ToList();
            var categoryCodes = new HashSet<string>(categoriesInGoodRecieve.Select(ct => ct.ItemCategoryCode));
            var filteredCategories = db.ItemCategorys.Where(ctg => (categoryCodes.Contains(ctg.ItemCategoryCode)));
            return Json(filteredCategories.Select(p => new { ItemCategoryCode = p.ItemCategoryCode, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        // 
        //  summary:
        //      Returns the itesm under the given category and that are received 
        public JsonResult GetItemsReceived(string itemCategoryCode, int currentProjectId)
        {
            int branchId = 0;
            var currentProject = db.ProjectInformation.Find(currentProjectId);
            if (currentProject != null)
            {
                branchId = currentProject.BranchId;
            }
            var itemsInGoodRecieve = (from c in db.InventoryItems
                                      join g in db.GoodReceiveItems
                                      on c.ItemCode equals g.ItemCode
                                      where c.ItemCategoryCode == itemCategoryCode && g.GoodReceive.BranchId == currentProject.BranchId
                                      select new
                                      {
                                          ItemCode = c.ItemCode,
                                          Name = c.Name
                                      }).ToList();
            var itemCodes = new HashSet<string>(itemsInGoodRecieve.Select(item => item.ItemCode));
            var filteredItems = db.InventoryItems.Where(itm => (itemCodes.Contains(itm.ItemCode)));
            return Json(filteredItems.Select(p => new { ItemCode = p.ItemCode, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }

        // 
        //  summary:
        //      returns the Periods that are available in the Good Receive Table
        //public JsonResult GetGoodRecievePeriods(int goodReciveId)
        //{
        //    (from p in db.GLPeriods
        //        join g in db.GoodReceives
        //        on p. equals g.ItemCode
        //        where c.ItemCategoryCode == itemCategoryCode && g.GoodReceive.BranchId == currentProject.BranchId
        //        select new
        //        {
        //            ItemCode = c.ItemCode,
        //            Name = c.Name
        //        }).ToList();
        //    var periods = db.ConstructionPeriod.ToList();

        //    return Json(periods.Select(p => new { ConstructionPeriodId = p.ConstructionPeriodId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        //}
        #endregion
        //
        //
        // Equpments in Equipment cost --- Estimation template
        public JsonResult GetEquipmentCategory()
        {
            var items = db.EquipmentIdentity.ToList();
            return Json(items.Select(p => new { EquipmentCategoryName = p.EquipmentCategoryName, EquipmentIdentityID = p.EquipmentIdentityID }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEquipmentType(Guid EquipmentIdentityID)
        {
            var items = db.EquipmentType.Where(x => x.EquipmentIdentityID == EquipmentIdentityID).ToList();
            return Json(items.Select(p => new { EquipmentTypeName = p.EquipmentTypeName, EquipmentTypeID = p.EquipmentTypeID }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEquipment()
        {
            var internalEquipment = db.InternalMachinaryRent.ToList();
            var externalEquipment = db.MachinaryRent.ToList();
            var internalIds = new HashSet<Guid>(internalEquipment.Select(ic => ic.FleetsID));
            var externalIds = new HashSet<Guid>(externalEquipment.Select(ee => ee.FleetsID));
            var combinedEquipments = db.Fleets.Where(flt => (internalIds.Contains(flt.FleetsID)) || (externalIds.Contains(flt.FleetsID)));

            return Json(combinedEquipments.Select(eq => new { FleetsID = eq.FleetsID, EquipmentName = eq.EquipmentName }), JsonRequestBehavior.AllowGet);
        }
        //
        //public JsonResult GetInternalEquipments()
        //{
        //    var fleetItems = db.Fleets.Where(x => !x.IsExternal);
        //    return Json(fleetItems.Select(eq => new { FleetsID = eq.FleetsID, EquipmentName = eq.EquipmentName +"-" + eq.SerialNo }), JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetExternalEquipment()
        //{
        //    var fleetItems = db.Fleets.Where(x => x.IsExternal);
        //    return Json(fleetItems.Select(eq => new { FleetsID = eq.FleetsID, EquipmentName = eq.EquipmentName }), JsonRequestBehavior.AllowGet);
        //}
        // 
        public JsonResult GetScheduleTasksForDelayAnalysis(int projectId, int revisionId)
        {
            var currentScheduleVersion = db.ScheduleRevisions.Find(revisionId);
            if (currentScheduleVersion != null)
            {
                var tasks = db.Tasks.Where(x => x.WorkItemDuration.ProjectInformationId == projectId && x.Version == currentScheduleVersion.Version).ToList();
                return Json(tasks.Select(p => new { Title = p.Title, ID = p.ID }), JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult GetDelayTypes()
        {
            var delays = db.DelayTypes.ToList();
            return Json(delays.Select(p => new { Abbriviation = p.Abbriviation, DelayTypeId = p.DelayTypeId }), JsonRequestBehavior.AllowGet);
        }

        public decimal GetPlannedToDate(decimal plannedAmntForTheMonth, decimal planndedTillPrevMonth)
        {
            return planndedTillPrevMonth + plannedAmntForTheMonth;
        }
        /// <summary>
        /// Get all equipments ,external and internal, from fleets
        /// </summary>
        /// <returns></returns>
        public JsonResult GetExternalAndInternalEquipments()
        {
            var internalEquipment = db.InternalMachinaryRent.ToList();
            var externalEquipment = db.MachinaryRent.ToList();
            var internalIds = new HashSet<Guid>(internalEquipment.Select(ic => ic.FleetsID));
            var externalIds = new HashSet<Guid>(externalEquipment.Select(ee => ee.FleetsID));
            var combinedEquipments = db.Fleets.Where(flt => (internalIds.Contains(flt.FleetsID)) || (externalIds.Contains(flt.FleetsID)));

            return Json(combinedEquipments.Select(eq => new { FleetsID = eq.FleetsID, EquipmentName = eq.EquipmentName }), JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// Get only the external equipments from fleets
        /// </summary>
        /// <returns></returns>
        public JsonResult GetExternalEquipments()
        {
            var externalEquipments = db.Fleets.Where(fl => fl.IsExternal);
            return Json(externalEquipments.Select(eq => new { FleetsID = eq.FleetsID, EquipmentName = eq.EquipmentName }), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get only the internal equipments from fleets
        /// </summary>
        /// <returns></returns>
        public JsonResult GetInternalEquipments()
        {
            var internalEquipments = db.Fleets.Where(x => !x.IsExternal);
            return Json(internalEquipments.Select(eq => new { FleetsID = eq.FleetsID, EquipmentName = eq.EquipmentName + "-" + eq.SerialNo }), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get the work itesm under a given project 
        /// the work item name is returned by concatnating with the blcok name
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult GetProjectWorkItems(int projectId)
        {
            var db = new ExceedDbContext();
            var workItems = db.WorkItemPerProject.Where(x => x.ProjectInformationId == projectId).OrderBy(x => x.ItemName).ToList();
            var workItemsWithConcatnatedValue =
                (from w in db.WorkItemPerProject.OrderBy(w => w.ItemCode)
                 join b in db.Blocks
                 on w.BlockId equals b.BlockId
                 //join sp in db.SubProjectType
                 //on b.SubProjectTypeId equals sp.SubProjectTypeId
                 where w.ProjectInformationId == projectId
                 select new
                 {
                     WorkItemPerProjectId = w.WorkItemPerProjectId,
                     ItemName = w.ItemName + " - " + b.Name
                 }).ToList();
            //var workItemsWithConcatnatedValue = new List<WorkItemPerProject>();
            //var workItem = new WorkItemPerProject();
            //foreach (var item in workItems)
            //{
            //    var tempWorkItem = new WorkItemPerProject();
            //    if (item.BlockId != null)
            //    {
            //        tempWorkItem = item;
            //        var workItemBlock = db.Blocks.Find(item.BlockId);
            //        if (workItemBlock != null)
            //        {
            //            tempWorkItem.ItemName = item.ItemName + " of " + workItemBlock.Name;
            //        }
            //    }
            //    workItemsWithConcatnatedValue.Add(tempWorkItem);
            //}
            return Json(workItemsWithConcatnatedValue.Select(x => new { ItemName = x.ItemName, WorkItemPerProjectId = x.WorkItemPerProjectId }), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetAllWorkCategories()
        //{
        //    var workItemCategories = from w in db.WorkCategory
        //                             join p in db.ProjectTypeMappings
        //                                 on w.ConfigurationId equals p.ConfigurationId

        //                             where p.ProjectTypeId == ProjectId
        //                             select (new { w.WorkCategoryId, w.ConfigurationId, w.Description, w.Name, w.Index });
        //    ViewData["Categories"] = workItemCategories.Distinct().ToList();
        //    return Json(internalEquipments.Select(eq => new { FleetsID = eq.FleetsID, EquipmentName = eq.EquipmentName }), JsonRequestBehavior.AllowGet);

        //}
        /// <summary>
        /// This returns the only Item Categories that are used in
        /// Material price index
        /// </summary>
        /// <returns></returns>
        public JsonResult GetItemCategoryOfMaterial()
        {

            var joinedCategories = (from cIC in db.ConstructionItemCategories
                                    join mPI in db.MaterialPriceIndexIndices
                                    on cIC.ItemCategoryCode equals mPI.ItemCategoryCode
                                    //where wi.ProjectInformationId == currentProjectIdR
                                    select new
                                    {
                                        categoryCode = mPI.ItemCategoryCode,
                                        name = cIC.Name
                                    }).ToList();
            var codes = new HashSet<string>(joinedCategories.Select(ic => ic.categoryCode));

            var items = db.ConstructionItemCategories.Where(ic => (codes.Contains(ic.ItemCategoryCode)));
            return Json(items.Select(p => new { ItemCategoryCode = p.ItemCategoryCode, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This returns only the Materials, from inventory, that has been used in
        /// material price index
        /// </summary>
        /// <param name="ItemCategoryCode"></param>
        /// <returns></returns>
        public JsonResult GetItemsUnderCategory(string ItemCategoryCode, string filterString)
        {
            //
            // find only the items that exist in the material price index calculation
            var joinedItems = (from ii in db.InventoryItems
                               join mPI in db.MaterialPriceIndexIndices
                               on ii.ItemCode equals mPI.ItemCode
                               where mPI.ItemCategoryCode == ItemCategoryCode
                               select new
                               {
                                   itemCode = mPI.ItemCode
                               }).ToList();
            var codes = new HashSet<string>(joinedItems.Select(ic => ic.itemCode));
            var items = db.InventoryItems.Where(item => (codes.Contains(item.ItemCode)));
            if (!string.IsNullOrEmpty(filterString))
            {
                filterString = filterString.ToLower();
                items = items.Where(i => i.Name.ToLower().Contains(filterString) || i.ItemCode.Contains(filterString));
            }
            ViewData["Items"] = items.Select(p => new { ItemCode = p.ItemCode, Name = p.Name });

            return Json(items.Select(p => new { ItemCode = p.ItemCode, Name = p.Name + " --- "+p.ItemCode }), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// available job titles
        /// </summary>
        /// <returns></returns>
        public JsonResult GetProjectJobTitles()
        {
            var jobTitle = db.ProjectEstimationIndexLabour.ToList();

            var titles =
                jobTitle.Select(
                    x =>
                        new
                        {
                            ProjectEstimationIndexLabourId = x.ProjectEstimationIndexLabourId,
                            Title = GetTitle(x.ProjectJobTitleId)
                        }).ToList();
            return Json(titles.Select(p => new { ProjectEstimationIndexLabourId = p.ProjectEstimationIndexLabourId, Title = p.Title }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetProjectTakeOffSheets(int currentProjectId)
        {
            var allWorkItems = db.WorkItemPerProject.Where(w => w.ProjectInformationId == currentProjectId).ToList();
            var workItemIds = new HashSet<int>(allWorkItems.Select(wi => wi.WorkItemPerProjectId));
            var takeOffSheetsUnderProject = db.TakeOffPerProject.Where(takeoff => (workItemIds.Contains(takeoff.WorkItemPerProjectId))).ToList();
            //var takeOffSheetsUnderProject = db.TakeOffPerProject.Where(t => t.ProjectInformationId == currentProjectId);
            return Json(takeOffSheetsUnderProject.Select(ts => new { TakeOffNo = ts.TakeOffNo, TakeOffPerProjectId = ts.TakeOffPerProjectId }), JsonRequestBehavior.AllowGet);

        }
        private string GetTitle(int id)
        {
            var pJTitle = db.ProjectJobTitles.Where(x => x.ProjectJobTitleId == id).FirstOrDefault();
            if (pJTitle != null)
            {
                return pJTitle.JobTitle;
            }
            return "";
        }

        /// <summary>
        /// Get GLPeriods ... 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetGLPeriods()
        {
            var periods = db.InventoryPeriods.ToList();
            return Json(periods.Select(prd => new { GLPeriodId = prd.InventoryPeriodId, Name = prd.Name }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetInventoryPeriods()
        {
            var joinedOpenPeriods = (from iv in db.InventoryPeriods
                                     join cn in db.ConstructionPeriod
                                     on iv.Name equals cn.Name
                                     where cn.Status == FiscalYearStatus.Open
                                     select new
                                     {
                                         InventoryPeriodId = iv.InventoryPeriodId,
                                         Name = cn.Name
                                     }).ToList();
            //var periods = db.InventoryPeriods.ToList();
            return Json(joinedOpenPeriods.Select(prd => new { GLPeriodId = prd.InventoryPeriodId, Name = prd.Name }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetAvailableDeformedBars()
        {
            var bars = (from item in db.InventoryItems
                        join category in db.ConstructionItemCategories
                        on item.ItemCategoryCode equals category.ItemCategoryCode
                        where category.IsReinforcementBar
                        //join justPriced in db.MaterialPriceIndexIndices
                        //on item.ItemCode equals justPriced.ItemCode
                        select new
                        {
                            ItemCode = item.ItemCode,
                            Name = item.Name
                        }).ToList();
            return Json(bars.Select(b => new { ItemCode = b.ItemCode, Name = b.Name }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetAllProjectNames()
        {
            var items = db.ProjectInformation;
            return Json(items.Select(p => new { ProjectInformationId = p.ProjectInformationId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllProjectsAllowedForCurrentUser()
        {
            var currentUser = User.Identity.Name;
            var assignments = db.ConstructionProjectUserAssignments.Where(assgn => assgn.UserName == currentUser).ToList();
            var ids = new HashSet<int>(assignments.Select(a => a.ProjectInformationId));

            var items = db.ProjectInformation.Where(p => ids.Contains(p.ProjectInformationId));
            return Json(items.Select(p => new { ProjectInformationId = p.ProjectInformationId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProjectWorkBreakDownUnderBlock(int? projId, int? blockId)
        {
            var wbsToReturn = new List<ProjectWorkBreakDown>();
            //.ToList();
            if (projId != null && blockId != null)
            {
                var strUnderBlock = db.BlockMappings.Where(bm => bm.BlockId == blockId).ToList();
                var ids = new HashSet<int>(strUnderBlock.Select(s => s.ConfigurationId));
                var wbsUnderCurrentBlock =
                    db.ProjectWorkBreakDown
                    .Where(wbs => (ids.Contains(wbs.ConfigurationId)) &&
                    wbs.ProjectInformationId == projId).ToList();
                if (wbsUnderCurrentBlock.Any())
                {
                    wbsToReturn.AddRange(wbsUnderCurrentBlock);
                    //foreach(var wbs in wbsUnderCurrentBlock)
                    //{
                    //    wbsUnderCurrentBlock.Add()
                    //}
                }
                //items = db.ProjectWorkBreakDown.Where(wbd => wbd.ProjectInformationId == projId);

            }
            return Json(wbsToReturn.Select(pwd => new { ProjectWorkBreakDownId = pwd.ProjectWorkBreakDownId, Description = pwd.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProjectWorkBreakDownStructures(int? projId)
        {
            var wbsesUnderProject = new List<ProjectWorkBreakDown>();
            if (projId != null)
            {
                var wbses = db.ProjectWorkBreakDown.Where(wbs => wbs.ProjectInformationId == projId).ToList();
                if (wbses.Any())
                {
                    wbsesUnderProject.AddRange(wbses);
                }
            }
            return Json(wbsesUnderProject.Select(wbs => new { wbs.ProjectWorkBreakDownId, wbs.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetScheduleVersionsOfProject(int currentProjectId)
        {
            var revisions = db.ScheduleRevisions.Where(sv => sv.ProjectInformationId == currentProjectId);
            //return Json(revisions.Select(p => new { ScheduleRevisionId = p.ScheduleRevisionId, Version = "Ver. " + p.Version + ".00" }), JsonRequestBehavior.AllowGet);
            return Json(revisions.Select(p => new { Version = p.Version, VersionTitle = "Ver. " + p.Version + ".00" }), JsonRequestBehavior.AllowGet);

        }
        //public JsonResult GetWorkItemDuration(int currentProjectIdR)
        //{
        //    var wdUnderTheProject = (from wd in db.WorkItemDuration
        //                             join wi in db.WorkItemPerProject
        //                             on wd.WorkItemPerProjectId equals wi.WorkItemPerProjectId
        //                             where wi.ProjectInformationId == currentProjectIdR
        //                             select new
        //                             {
        //                                 ID = wd.WorkItemDurationId
        //                             }).ToList();
        //    var ids = new HashSet<int>(wdUnderTheProject.Select(wd => wd.ID));
        //    IQueryable<WorkItemDuration> workItemDurations = db.WorkItemDuration.Where(wd => (ids.Contains(wd.WorkItemDurationId)));

        //    return Json(workItemDurations.Select(wid => new { WorkItemDurationId = wid.WorkItemDurationId, WorkDescription = wid.WorkDescription }), JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetWorkItemsUnderCurrentProject(int wbsId)
        //{
        //    var currentWBS = db.ProjectWorkBreakDown.Find(wbsId);
        //    if (currentWBS != null)
        //    {
        //        var currentProject = db.ProjectInformation.Find(currentWBS.ProjectInformationId);
        //        if (currentProject != null)
        //        {
        //            var wdUnderTheProject = (from wd in db.WorkItemDuration
        //                                     join wi in db.WorkItemPerProject
        //                                     on wd.WorkItemPerProjectId equals wi.WorkItemPerProjectId
        //                                     where wi.ProjectInformationId == currentProject.ProjectInformationId
        //                                     select new
        //                                     {
        //                                         ID = wd.WorkItemDurationId
        //                                     }).ToList();
        //            var ids = new HashSet<int>(wdUnderTheProject.Select(wd => wd.ID));
        //            IQueryable<WorkItemDuration> workItemDurations = db.WorkItemDuration.Where(wd => (ids.Contains(wd.WorkItemDurationId)));

        //            return Json(workItemDurations.Select(wid => new { WorkItemDurationId = wid.WorkItemDurationId, WorkDescription = wid.WorkDescription }), JsonRequestBehavior.AllowGet);
        //        }
        //    }

        //    return null;
        //}
        public JsonResult GetTasksUnderProject(int currentProjectId)
        {
            var tasks = db.Tasks.Where(t => t.WorkItemDuration.ProjectInformationId == currentProjectId);
            return Json(tasks.Select(t => new { t.ID, Title = t.Title + "( V " + t.Version + " )" }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStructureTypes(int subProjectId)
        {
            var currentSubProject = db.SubProjectType.Find(subProjectId);
            List<Configuration> targetStructures = new List<Configuration>();
            var allStructureTypes = db.Configurations.Where(x => x.Reference == "STRUCTURE_TYPE");
            if (currentSubProject != null)
            {
                var currentProject = db.ProjectInformation.Where(p => p.ProjectInformationId == currentSubProject.ProjectInformationId).FirstOrDefault();
                if (currentProject != null)
                {
                    var joinedValue = (from c in allStructureTypes
                                       join m in db.ProjectTypeMappings
                                       on c.ConfigurationId equals m.ConfigurationId
                                       where m.ProjectType == currentProject.ProjectType
                                       select new
                                       {
                                           ConfigurationId = c.ConfigurationId,
                                           CurrentValue = c.CurrentValue
                                       }).ToList();
                    var ids = new HashSet<int>(joinedValue.Select(v => v.ConfigurationId));
                    var filteredStructures = db.Configurations.Where(c => (ids.Contains(c.ConfigurationId)));

                    return Json(filteredStructures.Select(st => new { ConfigurationId = st.ConfigurationId, CurrentValue = st.CurrentValue }), JsonRequestBehavior.AllowGet);
                }

            }
            return null;
        }

        public JsonResult GetTemplateWorkItems(int categoryId, int type)
        {
            var projectType = (AvailableProjectTypes)type;
            var workItems = db.ProjectEstimationWorkItem
                .Where(w => w.WorkCategoryId == categoryId && w.ProjectType == projectType &&
                w.ParentId == 0 && !w.ContainsWorkItemAsInput
                );
            return Json(workItems.Select(w => new { ProjectEstimationWorkItemId = w.ProjectEstimationWorkItemId, ItemName = w.ItemName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTemplateWorkItemsToUseAsInputs(int categoryId, int workItemId)
        {
            var currentWorkItem = db.GenericWorkItemPerProject.Find(workItemId);
            if (currentWorkItem != null)
            {
                var workItems = db.ProjectEstimationWorkItem
                    .Where(w => w.WorkCategoryId == categoryId &&
                        w.ProjectType == currentWorkItem.ProjectInformation.ProjectType &&
                        w.ParentId == 0 &&
                        w.ProjectEstimationWorkItemId != currentWorkItem.ProjectEstimationWorkItemId &&
                        !w.ContainsWorkItemAsInput);
                return Json(workItems.Select(w => new { ProjectEstimationWorkItemId = w.ProjectEstimationWorkItemId, ItemName = w.ItemName }), JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult GetAllStructureTypes()
        {
            var list = db.Configurations.Where(x => x.Reference == ConstructionSettingCategories.STRUCTURE_TYPE.ToString()).ToList();
            List<Configuration> listToReturn = new List<Configuration>();
            if (list.Any())
            {
                foreach (var config in list)
                {
                    var strToReturn = new Configuration
                    {
                        ConfigurationId = config.ConfigurationId,
                        CurrentValue = config.CurrentValue
                    };
                    var currentProjectTypeMapping = db.ProjectTypeMappings.FirstOrDefault(m => m.ConfigurationId == config.ConfigurationId);

                    if (currentProjectTypeMapping != null && currentProjectTypeMapping.ProjectType == AvailableProjectTypes.Road)
                    {
                        strToReturn.CurrentValue = strToReturn.CurrentValue + " (Series " + currentProjectTypeMapping.CodeOrSeries + " )";
                    }
                    listToReturn.Add(strToReturn);
                }
            }
            return Json(listToReturn.Select(s => new { ConfigurationId = s.ConfigurationId, CurrentValue = s.CurrentValue }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetRoadStructureTypes()
        {

            var projectTypeMapping = db.ProjectTypeMappings.Where(p => p.ProjectType == AvailableProjectTypes.Road);
            var ids = new HashSet<int>(projectTypeMapping.Select(p => p.ConfigurationId));
            var list = db.Configurations.Where(x => (ids.Contains(x.ConfigurationId)) && x.Reference == ConstructionSettingCategories.STRUCTURE_TYPE.ToString()).ToList();
            List<Configuration> listToReturn = new List<Configuration>();
            if (list.Any())
            {
                foreach (var config in list)
                {
                    var strToReturn = new Configuration
                    {
                        ConfigurationId = config.ConfigurationId,
                        CurrentValue = config.CurrentValue
                    };
                    var currentProjectTypeMapping = db.ProjectTypeMappings.FirstOrDefault(m => m.ConfigurationId == config.ConfigurationId);

                    if (currentProjectTypeMapping != null && currentProjectTypeMapping.ProjectType == AvailableProjectTypes.Road)
                    {
                        strToReturn.CurrentValue = strToReturn.CurrentValue + " (Series " + currentProjectTypeMapping.CodeOrSeries + " )";
                    }
                    listToReturn.Add(strToReturn);
                }
            }
            return Json(listToReturn.Select(s => new { ConfigurationId = s.ConfigurationId, CurrentValue = s.CurrentValue }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetBranches()
        {
            var branchList = (from inv in db.Branch
                              join ip in db.ItemPriceIndexes
                                  on inv.BranchId equals ip.BranchId
                              select (new { inv.BranchId, inv.Name })).Distinct();
            return Json(branchList.Select(b => new { BranchId = b.BranchId, Name = b.Name }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetPeriodsWithinTimeRange(int theCurrentTaskId)
        {
            var task = db.Tasks.Find(theCurrentTaskId);
            if (task != null)
            {
                DateTime start = task.Start;
                DateTime end = task.End;

                var periodsInTimeRange = db.ConstructionPeriod.Where(p => p.DateFrom <= start && p.DateTo >= start ||
                                                                          p.DateFrom >= start && p.DateTo <= end ||
                                                                          p.DateFrom <= end && p.DateTo >= end
                                                                    ).ToList();
                return Json(periodsInTimeRange.Select(p => new { ConstructionPeriodId = p.ConstructionPeriodId, Name = p.Name }), JsonRequestBehavior.AllowGet);

            }
            return null;
        }
        public JsonResult GetConstructionSubPeriods(int id)
        {
            var subPeriods = db.ConstructionSubPeriod.Where(sp => sp.ConstructionPeriodId == id);
            return Json(subPeriods.Select(sp => new { sp.ConstructionSubPeriodId, sp.Name }), JsonRequestBehavior.AllowGet);
        }
        // 
        //  Summary:
        //      get the GL Tax Rates which are for receivable accounts
        public JsonResult GetGLTaxRates()
        {
            var taxTypes = db.GLTaxRates.Where(tr => tr.ApplyToAr);
            return Json(taxTypes.Select(p => new { GLTaxRateID = p.GLTaxRateID, TaxName = p.TaxName }), JsonRequestBehavior.AllowGet);
        }
        // 
        //  Summary:
        //      get the withholding tax rates only
        public JsonResult GetWitholdingtaxRates()
        {
            var withholdingTaxes = db.GLTaxRates.Where(r => r.ApplyToWithholding);
            return Json(withholdingTaxes.Select(r => new { r.GLTaxRateID, r.TaxName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProjectWorkInProgressNumbers(int projectId, int sourctType, string filterString)
        {
            var wipToReturn = new List<WorkInProgress>();
            var wips = db.WorkInProgress.Where(w => w.ProjectInformationId == projectId && w.IsOnlineApproved);

            if ((ConstructionInvoiceSource)sourctType == ConstructionInvoiceSource.Manual)
            {
                return null;
            }
            if (wips.Any() && (ConstructionInvoiceSource)sourctType == ConstructionInvoiceSource.WIP)
            {
                foreach (WorkInProgress workInProgress in wips)
                {
                    // find the invoice where this work in progress is 
                    // referenced from
                    var wipIsUsed = false;
                    var invoice =
                        db.ConsReceivableInvoices.Where(
                            inv => inv.Reference == workInProgress.WorkInProgressNumber).FirstOrDefault();
                    if (invoice != null)
                    {
                        if (invoice.IsOnlineApproved)
                        {
                            wipIsUsed = true;
                        }
                    }
                    if (!wipIsUsed)
                    {
                        wipToReturn.Add(workInProgress);
                    }
                }
            }
            if (!string.IsNullOrEmpty(filterString))
            {
                filterString = filterString.ToLower();
                wipToReturn = wipToReturn.Where(w => w.WorkInProgressNumber.ToLower().Contains(filterString)).ToList();
            }
            return Json(wipToReturn.AsQueryable().Select(w => new { WorkInProgressId = w.WorkInProgressId, WorkInProgressNumber = w.WorkInProgressNumber }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaymentTerms()
        {
            var pTerms = db.PaymentTerms;
            return Json(pTerms.Select(t => new { PaymentTermsID = t.PaymentTermsID, PaymentTermsDays = t.PaymentTermsDays }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetPaymentCerificates(int projectId)
        {
            List<PaymentCertificateViewModel> list = new List<PaymentCertificateViewModel>();
            var paymentCeritificates = db.PaymentCertificates.Where(pc => pc.ProjectInformationId == projectId).ToList();
            if (paymentCeritificates.Any())
            {
                foreach (var certificate in paymentCeritificates)
                {
                    var record = new PaymentCertificateViewModel
                    {
                        ProjectPaymentCertificateId = certificate.ProjectPaymentCertificateId,
                        PaymentNumber = certificate.PaymentNumber + GenericHelper.ToOrdinal(certificate.PaymentNumber) + " pay"
                    };
                    list.Add(record);
                }
            }
            return Json(list.Select(t => new { ProjectPaymentCertificateId = t.ProjectPaymentCertificateId, PaymentNumber = t.PaymentNumber }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInvoiceNumbersForCollection(int collectionId)
        {
            var currentCollection = db.ConsARStandardCollections.Find(collectionId);
            var invoices = new List<ConsReceivableInvoice>().AsQueryable();

            if (currentCollection != null)
            {
                invoices = db.ConsReceivableInvoices.Where(inv => inv.ProjectInformationId == currentCollection.ProjectInformationId && inv.IsOnlineApproved && inv.ConstructionInvoiceStatus != ConstructionInvoiceStatus.Collected);
                if (currentCollection.ReceiptType == StandardCollectionReceiptType.Prepayment)
                {
                    return null;
                }
            }
            return Json(invoices.Select(t => new { ReceivableInvoiceId = t.ConsReceivableInvoiceId, InvoiceNumber = t.InvoiceNumber }), JsonRequestBehavior.AllowGet);
        }

        public decimal GetApplyAmountFromInvoice(string invoiceNumber)
        {
            var currentInvoice = db.ConsReceivableInvoices.FirstOrDefault(inv => inv.InvoiceNumber == invoiceNumber);
            var amnt = new decimal();
            if (currentInvoice != null)
            {
                //var invoiceDetails = db.ReceivableInvoiceDetails
                //    .Where(d => d.ReceivableInvoiceId == currentInvoice.ReceivableInvoiceId).ToList();
                //if (invoiceDetails.Any())
                //{
                //    amnt = invoiceDetails.Sum(d => d.NetAmount);
                //}
                amnt = currentInvoice.TotalAmount - currentInvoice.CollectedAmount;
            }
            return amnt;
        }

        public JsonResult GetAvailableDelaysOfTasks(int projectId, int scheduleVersion)
        {
            var delays = db.WorkDelay
                .Where(d => d.Task.WorkItemDuration.ProjectInformationId == projectId &&
                            d.Task.Version == scheduleVersion);
            //if (delays.Any())
            //{
            //    foreach (WorkDelay workDelay in delays)
            //    {
            //        var currentDelay = new WorkDelay { WorkDelayId = workDelay.WorkDelayId, Or}
            //    }
            //}
            return Json(delays.Select(d => new { WorkDelayId = d.WorkDelayId, Order = "Delay " + d.Order }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetPaymentMethods()
        {
            var paymentMethods = db.PaymentMethods;
            return Json(paymentMethods.Select(pt => new { pt.PaymentMethodID, pt.Method }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMainContractVariationOrderTypes()
        {
            var variationTypes = db.VariationOrderType.Where(vot => vot.Type == VariationOrderTypeCategory.MainContract);
            return Json(variationTypes.Select(ordt => new { ordt.VariationOrderTypeId, ordt.Name }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubContractVariationOrderTypes()
        {
            var variationTypes = db.VariationOrderType.Where(vot => vot.Type == VariationOrderTypeCategory.SubContract);
            return Json(variationTypes.Select(ordt => new { ordt.VariationOrderTypeId, ordt.Name }), JsonRequestBehavior.AllowGet);
        }
        public string GetTaskTitleFromWorkItem(int workItemDurationId)
        {
            var title = "";
            var workItemDuration = db.WorkItemDuration.Find(workItemDurationId);
            if (workItemDuration != null)
            {
                title = workItemDuration.WorkDescription;
            }
            return title;
        }

        public JsonResult GetProjectBlocks(int subProjectTypeId)
        {

            var blocks = db.Blocks.Where(b => b.SubProjectTypeId == subProjectTypeId);

            return Json(blocks.Select(bl => new { bl.BlockId, bl.Name }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetBlocksUnderTypeAndProject(int subProjectTypeId, int projectId)
        {

            var blocks = db.Blocks.Where(b => b.SubProjectTypeId == subProjectTypeId && b.ProjectInformationId == projectId);

            return Json(blocks.Select(bl => new { bl.BlockId, bl.Name }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetWorkCategories(int type)
        {
            //var allCategories = db.WorkCategory.ToList();
            //List<WorkCategory> categoriesOfCurrentProjectType = new List<WorkCategory>();
            //var typeMappings = db.ProjectTypeMappings.Where(tm => tm.ProjectTypeId == ProjectId).ToList();
            //if (typeMappings.Any())
            //{
            //    foreach (ProjectTypeMapping mapping in typeMappings)
            //    {
            //        var categoriesOfCurrentStructureType = allCategories.Where(c => c.ConfigurationId == mapping.ConfigurationId).ToList();
            //        categoriesOfCurrentProjectType.AddRange(categoriesOfCurrentStructureType);
            //    }
            //}
            var projectType = (AvailableProjectTypes)type;
            #region using joined tables
            //var workItemCategories = db.WorkCategory;
            var workItemCategories = (from w in db.WorkCategory
                                      join p in db.ProjectTypeMappings
                                          on w.ConfigurationId equals p.ConfigurationId
                                      where p.ProjectType == projectType
                                      select (new { w.WorkCategoryId, w.ConfigurationId, w.Description, w.Name, w.Index })).ToList();
            #endregion
            var listToReturn = new List<WorkCategory>();
            var category = new WorkCategory();
            if (projectType == AvailableProjectTypes.Road)
            {
                foreach (var c in workItemCategories)
                {
                    category = new WorkCategory
                    {
                        WorkCategoryId = c.WorkCategoryId,
                        Name = c.Name + " ( Ser. " + c.Index + " )"
                    };
                    listToReturn.Add(category);
                }
            }
            else
            {
                foreach (var c in workItemCategories)
                {
                    category = new WorkCategory
                    {
                        WorkCategoryId = c.WorkCategoryId,
                        Name = c.Name
                    };
                    listToReturn.Add(category);
                }
            }

            return Json(listToReturn.Select(c => new { Name = c.Name, WorkCategoryId = c.WorkCategoryId }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStructureTypesUnderBlock(int blockId)
        {
            //var bMapping = db.BlockMappings.Where(b => b.BlockId == blockId);
            //var ids = new HashSet<int>(bMapping.Select(m  => m.ConfigurationId))
            var joinedInfo = from bm in db.BlockMappings
                             join c in db.Configurations
                             on bm.ConfigurationId equals c.ConfigurationId
                             where bm.BlockId == blockId
                             select (new
                             {
                                 ConfigurationId = c.ConfigurationId,
                                 CurrentValue = c.CurrentValue,
                             });
            return Json(joinedInfo.Select(st => new { st.ConfigurationId, st.CurrentValue }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWorkCategoryUnderStructure(int configId, int blockId)
        {
            var taskUnderBlockAndStructure = db.Tasks.Where(t => t.BlockId == blockId && t.ConfigurationId == configId);
            var categoryIds = new HashSet<int>(taskUnderBlockAndStructure.Select(t => t.WorkCategoryId));
            var categorys = db.WorkCategory.Where(wc => (categoryIds.Contains(wc.WorkCategoryId)));
            return Json(categorys.Select(wc => new { wc.WorkCategoryId, wc.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProjectBoQ(int projectId)
        {
            var boqs = db.BoQOfExcutedWork.Where(b => b.ProjectInformationId == projectId);
            return Json(boqs.Select(b => new { b.BillNumber, b.BillReference, b.BoQOfExecutedWorkId }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPeriodsInCashFlowBudgetYear(int budgetYearId)
        {
            var currentYear = db.CashFlowYear.Find(budgetYearId);
            if (currentYear != null)
            {
                var joinedPeriods = (from gl in db.GLPeriods
                                     join cp in db.ConstructionPeriod
                                     on gl.Name equals cp.Name
                                     where gl.GlFiscalYearId == currentYear.GlFiscalYearId
                                     select new
                                     {
                                         GlFiscalYearId = gl.GlFiscalYearId,
                                         GLPeriodId = gl.GLPeriodId,
                                         Name = cp.Name,
                                         ConstructionPeriodId = cp.ConstructionPeriodId
                                     }).ToList();
                return Json(joinedPeriods.Select(p => new { p.ConstructionPeriodId, p.Name }), JsonRequestBehavior.AllowGet);

            }
            return null;
        }

        public JsonResult GetTasksPossibleForBeingPrecessor(int taskId)
        {

            var taskToBePredecessor = new List<Task>();
            var selectedTask = db.Tasks.Find(taskId);
            if (selectedTask != null)
            {
                var allPossiblePredecessors = db.Tasks
                    .Where(t => t.ID != taskId &&
                        t.Version == selectedTask.Version &&
                        t.BlockId <= selectedTask.BlockId &&
                        t.ID != selectedTask.ParentID)
                    .OrderBy(t => t.OrderID)
                    .ToList();

                foreach (Task task in allPossiblePredecessors)
                {
                    if (!IsAnExistingPredecessor(taskId, task.ID))
                    {
                        taskToBePredecessor.Add(task);
                    }
                }
            }
            return Json(taskToBePredecessor.Select(t => new { ID = t.ID, Title = t.ProjectSpecificId + " " + t.Title }), JsonRequestBehavior.AllowGet);
        }
        private bool IsAnExistingPredecessor(int targetTaskId, int predId)
        {
            var IsAPredessor = false;

            var allPredecessors = (from d in db.Dependencies
                                   join t in db.Tasks
                                   on d.PredecessorID equals t.ID
                                   where d.SuccessorID == targetTaskId
                                   select new
                                   {
                                       ID = t.ID,
                                       Title = t.Title
                                   }).ToList();
            var idOfExistingPredecessors = new HashSet<int>(allPredecessors.Select(t => t.ID));
            if (idOfExistingPredecessors.Contains(predId))
            {
                IsAPredessor = true;
            }
            return IsAPredessor;
        }
        public JsonResult GetWorkItemCodes()
        {
            var codes = db.ProjectEstimationWorkItem.
                Where(w => !string.IsNullOrEmpty(w.ItemCode)).Select(w => new { w.ItemName, w.ItemCode }).ToList();
            return Json(codes, JsonRequestBehavior.AllowGet);
        }
    }
}