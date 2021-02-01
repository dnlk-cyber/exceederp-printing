using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Models
{
    public class ApplicationBranchManager
    {
        private ApplicationBranchStore _branchStore;
        private ApplicationDbContext _db;
        private ExceedDbContext _dbContext; 
        private ApplicationUserManager _userManager;
       
        public ApplicationBranchManager()
        {
            _db = HttpContext.Current
                   .GetOwinContext().Get<ApplicationDbContext>();
            _dbContext = new ExceedDbContext();
            _userManager = HttpContext.Current
                .GetOwinContext().GetUserManager<ApplicationUserManager>();
            _branchStore = new ApplicationBranchStore(_dbContext);
            
        }
        public IQueryable<Branch> Branches
        {
            get
            {
                return _branchStore.Branches;
            }
        }


        //public IdentityResult SetUsersBranch(string userId, BranchVm result)
        //{
           
        //    var currentBranches = GetUsersBranch(userId);
        //    foreach (var branch in currentBranches)
        //    {
        //        string branchid = branch.BranchId.ToString();
        //        var userB = _db.UserBranches.FirstOrDefault(x => x.ApplicationUserId == userId && x.UserBranchId == result.BranchId);
        //        if (userB != null)
        //        {
        //            _db.UserBranches.Remove(userB);
        //            _db.SaveChanges();
        //        }
        //    }
        //    _db.SaveChanges();
        //    int defaultBranch = 0;
        //    var usersDb= new ApplicationDbContext();
        //    var user = usersDb.Users.Where(x => x.Id == userId).FirstOrDefault();
        //    if (user != null)
        //    {
        //        _dbContext = new ExceedDbContext();
        //        List<UserBranchGroup> userBranchGroups = new List<UserBranchGroup>();
        //        var employeeId = UserInformationHelper.GetEmployeeId(user.UserName);

        //        var placment = _dbContext.Placements.Where(x => x.EmployeeId == employeeId && x.isCurrent).FirstOrDefault();
        //        if (placment != null)
        //        {
        //            UserInformationHelper.GetEmployeeBranch(user.UserName);
        //            defaultBranch = placment.BranchId;
        //        }
        //        else
        //        {

        //        }
        //        foreach (string branchID in branchIds)
        //        {
        //            var newBranches = FindById(branchID);

        //            if (newBranches != null)
        //            {
        //                UserBranchGroup branchGroup = new UserBranchGroup();
        //                branchGroup.ApplicationUserId = userId;
        //                branchGroup.UserBranchId = branchID;
        //                if (defaultBranch.ToString() == branchID)
        //                {
        //                    branchGroup.IsDefault = true;
        //                }
        //                //else
        //                //{
        //                //     branchGroup.IsDeafult =
        //                //}
        //                _db.UserBranches.Add(branchGroup);
        //                _db.SaveChanges();
        //            }


        //        }
        //    }
           
        //    if (branchIds.Length != 0 && userId != null)
        //    {
            
        //    UserHelper.SetDefaultUserBranch(userId, branchIds[0]);  
        //    }
         

           
        //    return IdentityResult.Success;
        //}

        public  IEnumerable<Branch> GetUsersBranch(string userId)
        {
            var result = new List<Branch>();
            _db = new ApplicationDbContext();
            var userBranch =   _db.UserBranches.Where(x => x.ApplicationUserId == userId);
            if (userBranch.Any())
            {
                foreach (UserBranchGroup userBranchGroup in userBranch)
                {
                    int branchId = int.Parse(userBranchGroup.UserBranchId);
                    Branch branch = _dbContext.Branch.Find(branchId);
                    result.Add(branch);

                }
            }
            return result;
        }
        
        public IEnumerable<Branch> GetUsersBranchByName(string userName)
        {
            var user = _userManager.FindByName(userName);

            var result = new List<Branch>();
            var userBranch = _db.UserBranches.Where(x => x.ApplicationUserId == user.Id);
            foreach (UserBranchGroup userBranchGroup in userBranch)
            {
                int branchId = int.Parse(userBranchGroup.UserBranchId);
                Branch branch = _dbContext.Branch.Find(branchId);
                result.Add(branch);

            }
          
            return result;
        }

        public Branch GetUserDefaultBranchByName(string userName)
        {
            var user = _userManager.FindByName(userName);
            var userBranch = _db.UserBranches.Where(x=>x.IsDefault).FirstOrDefault(x => x.ApplicationUserId == user.Id);

            if (userBranch != null)
            {
                var branch= _dbContext.Branch.Find(int.Parse(userBranch.UserBranchId));
                if (branch != null)
                    return branch;
            }
           

            return null;
        }
        public Branch FindById(string id)
        {
            return _branchStore.FindById(id);
        }
    }
}