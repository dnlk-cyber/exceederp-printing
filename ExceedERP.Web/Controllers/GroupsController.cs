﻿using System;
using System.Collections.Generic;
 using System.Linq;
 using System.Web.Mvc;
 using ExceedERP.Helpers.Common;
 using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
 using ExceedERP.Web.Models;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Controllers
{
    [Authorize(Roles = "Admin,Admin_Regional,Admin_Module")]
    public class GroupsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }
        private AccountController account;
        private ApplicationGroupManager _groupManager;
        public ApplicationGroupManager GroupManager
        {
            get
            {
                return _groupManager ?? new ApplicationGroupManager();
            }
            private set
            {
                _groupManager = value;
            }
        }

        public ActionResult Roles(RoleViewModel[] result )
        {
            var model = result[result.Length - 1];
            result = result.Where(x => x.RoleViewModelId != model.RoleViewModelId && x.Name != "masterGridParameter").ToArray(); 
            
            string[] roles = result.Select(x => x.Name).ToArray();
            var group =    GroupManager.FindById(model.RoleViewModelId);
            GroupManager.SetGroupRoles(model.RoleViewModelId , roles);
            #region access log

            UserHelper.AccessLog(Request.UserHostAddress, "Edit Group", "Success", User.Identity.Name, "Edited group  " + GroupManager.FindById(model.RoleViewModelId));

            #endregion
            return Json(result);

        }

        [Authorize(Roles = "Admin,Admin_Regional,Admin_Module")]
        public ActionResult ApplicationGroups_Read([DataSourceRequest]DataSourceRequest request)
        {
            account = new AccountController(ApplicationUserManager.Usermgr, ApplicationSignInManager.SignInManager);
            
            List<ApplicationGroup> applicationGroups = account.GetApplicationGroups();
            #region module admin filter
            if(AdminModeHelper.IsModuleMode())
            {
                List<string> modulelist = AdminModeHelper.GetModuleForAdmin(User.Identity.Name);
                applicationGroups = applicationGroups.Where(x => modulelist.Contains(x.GroupModule.ToString())).ToList();
            }
            #endregion
            List<GroupViewModel> groupList = new List<GroupViewModel>();
            foreach (ApplicationGroup applicationGroup in applicationGroups)
            {
                GroupViewModel registerViewModel = new GroupViewModel
                {
                    Id = applicationGroup.Id,
                    Name = applicationGroup.Name,
                    Description = applicationGroup.Description,
                    GroupModule = applicationGroup.GroupModule
                };

                groupList.Add(registerViewModel);
            }
            IQueryable<GroupViewModel> groups = groupList.AsQueryable();
            DataSourceResult result = groups.ToDataSourceResult(request, group => new {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                GroupModule = group.GroupModule

            });

            return Json(result);
        }
        [Authorize(Roles = "Admin,Admin_Regional,Admin_Module")]
        public ActionResult ApplicationUserGroups([DataSourceRequest] DataSourceRequest request, string id)
        {
            var userGroups =   GroupManager.GetUserGroups(id).ToList();
            List<string> userGroupId = userGroups.Select(x => x.Id).ToList(); 
             account = new AccountController(ApplicationUserManager.Usermgr, ApplicationSignInManager.SignInManager);
            List<ApplicationGroup> applicationGroups = account.GetApplicationGroups();

            #region module admin filter
            if (AdminModeHelper.IsModuleMode())
            {
                List<string> modulelist = AdminModeHelper.GetModuleForAdmin(User.Identity.Name);
                applicationGroups = applicationGroups.Where(x => modulelist.Contains(x.GroupModule.ToString())).ToList();
            }
            #endregion
            List<GroupViewModel> groupList = new List<GroupViewModel>();

            foreach (ApplicationGroup applicationGroup in applicationGroups)
            {
                GroupViewModel registerViewModel = new GroupViewModel
                {
                    IsAllowed = userGroupId.Contains(applicationGroup.Id), //true or false
                    Id = applicationGroup.Id,
                    Name = applicationGroup.Name,
                    Description = applicationGroup.Description
                };
               
                groupList.Add(registerViewModel);
            }

            IQueryable<GroupViewModel> groups = groupList.AsQueryable();
            DataSourceResult result = groups.ToDataSourceResult(request, group => new {
                Id = group.Id,
                IsAllowed = group.IsAllowed,
                Name = group.Name,
                Description = group.Description,
                 
            });
            return Json(result);          
        }

        [Authorize(Roles = "Admin,Admin_Module")]
        public ActionResult ApplicationGroups_Create([DataSourceRequest]DataSourceRequest request, GroupViewModel groupView)
        {
            account = new AccountController(ApplicationUserManager.Usermgr, ApplicationSignInManager.SignInManager);
            
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                ApplicationGroup applicationGroup = new ApplicationGroup
                {
                    Name = groupView.Name,
                    Description = groupView.Description,
                    GroupModule=groupView.GroupModule,
                };
                bool value = account.Create(applicationGroup);
                #region access log
                UserHelper.AccessLog(Request.UserHostAddress, "Create Group", "Success", User.Identity.Name, "Updated group  " + applicationGroup.Name);
                #endregion
                if (value)
                {
                    return Json(new[] { groupView }.ToDataSourceResult(request, ModelState));  
                }
            }

            return Json(new[] { groupView }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "Admin,Admin_Module")]
        public ActionResult ApplicationGroups_Update([DataSourceRequest]DataSourceRequest request, GroupViewModel groupView)
        {
            account = new AccountController(ApplicationUserManager.Usermgr, ApplicationSignInManager.SignInManager);

            if (ModelState.IsValid)
            {
                var group =    GroupManager.FindById(groupView.Id);
                if (group == null)
                {
                    return HttpNotFound();
                }
                group.Name = groupView.Name;
                group.Description = groupView.Description;
                group.GroupModule = groupView.GroupModule;
                GroupManager.UpdateGroup(group);
                #region access log
                UserHelper.AccessLog(Request.UserHostAddress, "Update Group", "Success", User.Identity.Name, "Updated group  " + group.Name);
                #endregion
            }

            return Json(new[] { groupView }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "Admin,Admin_Module")]
        public ActionResult ApplicationGroups_Destroy([DataSourceRequest]DataSourceRequest request, GroupViewModel groupView)
        {
            if (ModelState.IsValid)
            {
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted system role" + groupView.Name);
                #endregion
                GroupManager.DeleteGroup(groupView.Id);
            }

            return Json(new[] { groupView }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult RefreshApplicationRoles()
        {

            try
            {
                InitialController.AddallRoles();
                var formattedData = new
                {
                    Message = "Refresh Successful.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                var formattedData = new
                {
                    Message = $"Error Occured in refreshing roles.\n {ex.Message}",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
        
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
