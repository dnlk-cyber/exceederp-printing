using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
 using ExceedERP.Web.Models;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Controllers
{
    public class BranchUserController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApplicationBranch([DataSourceRequest]DataSourceRequest request, string id)
        {
            ApplicationBranchManager branchManager = new ApplicationBranchManager();
            
            List<BranchVm> branchVms = new List<BranchVm>();
           
            var usersDb = new ApplicationDbContext();
            UserBranchGroup userBranches = new UserBranchGroup();
            var user = usersDb.Users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                var employeeId = UserInformationHelper.GetEmployeeId(user.UserName);
                int defaultBranch = 0;
              
                var checkDefBranch = usersDb.UserBranches.FirstOrDefault(x => x.ApplicationUserId == id);
                if (checkDefBranch == null)
                {
                    var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == employeeId && x.isCurrent);
                    if (placment != null)
                    {
                        defaultBranch = placment.BranchId;
                        userBranches.ApplicationUserId = id;
                        userBranches.UserBranchId = defaultBranch.ToString();
                        userBranches.IsDefault = true;
                        usersDb.UserBranches.Add(userBranches);
                        usersDb.SaveChanges();
                    }

                }
                var userBranchs = branchManager.GetUsersBranch(id).ToList();
                List<int> userbranchId = userBranchs.Select(x => x.BranchId).ToList();
                List<Branch> applicationbranches = db.Branch.ToList();

                foreach (Branch branch in applicationbranches)
                {
                    BranchVm registerViewModel = new BranchVm();
                    if (userbranchId.Contains(branch.BranchId))
                    {
                        registerViewModel.IsAllowed = true;
                    }
                    else
                    {
                        registerViewModel.IsAllowed = false;
                    }

                  
                        var checkDefBranchh = usersDb.UserBranches.FirstOrDefault(x => x.UserBranchId == branch.BranchId.ToString() && x.ApplicationUserId == id);
                        if (checkDefBranchh != null)
                        {
                            registerViewModel.IsDefault = checkDefBranchh.IsDefault;
                        }
                    
                   
                    registerViewModel.BranchId = branch.BranchId.ToString();
                    registerViewModel.BranchName = branch.Name;
                    registerViewModel.Remark = branch.Remark;

                    branchVms.Add(registerViewModel);
                }
            }


            IQueryable<BranchVm> groups = branchVms.AsQueryable();
            DataSourceResult result = groups.ToDataSourceResult(request, group => new
            {
                BranchId = group.BranchId,
                IsAllowed = group.IsAllowed,
                IsDefault = group.IsDefault,
                BranchName = group.BranchName,
                Remark = group.Remark,

            });

            return Json(result);
        }
        
        //public ActionResult ApplicationBranch([DataSourceRequest]DataSourceRequest request, string id)
        //{
        //    ApplicationBranchManager branchManager = new ApplicationBranchManager();
        //    var userBranchs = branchManager.GetUsersBranch(id).ToList();
        //    List<BranchVm> branchVms = new List<BranchVm>();
        //    List<int> userbranchId = userBranchs.Select(x => x.BranchId).ToList();
        //    var usersDb= new ApplicationDbContext();
        //    var user= usersDb.Users .Where (x=> x.Id == id).FirstOrDefault ();
        //    if (user != null)
        //    {
        //        var employeeId = UserInformationHelper.GetEmployeeId(user.UserName );
        //        int defaultBranch=0;
        //        var placment = db.Placements.Where(x => x.EmployeeId == employeeId && x.isCurrent).FirstOrDefault ();
        //        if (placment!=null)
        //        {
        //            defaultBranch = placment.BranchId;
        //        }
        //        List<Branch> applicationbranches = db.Branch.ToList();
               
        //        foreach (Branch branch in applicationbranches)
        //        {
        //            BranchVm registerViewModel = new BranchVm();
        //            if (userbranchId.Contains(branch.BranchId))
        //            {
        //                registerViewModel.IsAllowed = true;
        //            }
        //            else
        //            {
        //                registerViewModel.IsAllowed = false;
        //            }
                    
        //            if (placment ==null)
        //            {
        //                var checkDefBranch = usersDb.UserBranches.Where(x => x.UserBranchId == branch.BranchId.ToString ()).FirstOrDefault();
        //                if (checkDefBranch != null)
        //                {
        //                    registerViewModel.IsDefault = checkDefBranch.IsDefault;
        //                }
        //            }
        //            else
        //            {
        //                if (branch.BranchId == defaultBranch)
        //                {
        //                    registerViewModel.IsDefault = true;
        //                }
        //            }
                  
        //            registerViewModel.BranchId = branch.BranchId.ToString();
        //            registerViewModel.BranchName = branch.Name;
        //            registerViewModel.Remark = branch.Remark;

        //            branchVms.Add(registerViewModel);
        //        }
        //    }

        
        //    IQueryable<BranchVm> groups = branchVms.AsQueryable();
        //    DataSourceResult result = groups.ToDataSourceResult(request, group => new
        //    {
        //        BranchId = group.BranchId,
        //        IsAllowed = group.IsAllowed,
        //        IsDefault = group.IsDefault,
        //        BranchName = group.BranchName,
        //        Remark = group.Remark,

        //    });

        //    return Json(result);
        //}

        public ActionResult Branches_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Branch> branches = db.Branch;
            DataSourceResult result = branches.ToDataSourceResult(request, branch => new {
                BranchId = branch.BranchId,
                Name = branch.Name,
                Remark = branch.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Branches_Create([DataSourceRequest]DataSourceRequest request, Branch branch)
        {
            if (ModelState.IsValid)
            {
                var entity = new Branch
                {
                    Name = branch.Name,
                    Remark = branch.Remark
                };

                db.Branch.Add(entity);
                db.SaveChanges();
                branch.BranchId = entity.BranchId;
            }

            return Json(new[] { branch }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Branches_Update([DataSourceRequest]DataSourceRequest request, Branch branch)
        {
            if (ModelState.IsValid)
            {
                var entity = new Branch
                {
                    BranchId = branch.BranchId,
                    Name = branch.Name,
                    Remark = branch.Remark
                };

                db.Branch.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { branch }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Branches_Destroy([DataSourceRequest]DataSourceRequest request, Branch branch)
        {
            if (ModelState.IsValid)
            {
                var entity = new Branch
                {
                    BranchId = branch.BranchId,
                    Name = branch.Name,
                    Remark = branch.Remark
                };

                db.Branch.Attach(entity);
                db.Branch.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { branch }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
