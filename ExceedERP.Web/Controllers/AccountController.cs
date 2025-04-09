using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
 

namespace ExceedERP.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        ApplicationBranchManager branchManager = new ApplicationBranchManager();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return ApplicationRoleManager.RoleManager;
            }
            private set
            {
                _roleManager = value;
            }
        }
        
        
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (DateTime.Now > new DateTime(2028, 4, 1))
            {
                return null;
            }
           
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //this block is to avoid multiple login
            // check if username/password pair match.
            var loggedinUser = await UserManager.FindAsync(model.UserName, model.Password);
            if (loggedinUser != null)
            {
                // Now user have entered correct username and password.
                // Time to change the security stamp
                await UserManager.UpdateSecurityStampAsync(loggedinUser.Id);
            }
            //until this
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    #region access log
                    UserHelper.AccessLog(Request.UserHostAddress, "Login", "Success", model.UserName, "");
                    #endregion

                    #region check default branch

                    if(branchManager.GetUserDefaultBranchByName(model.UserName)==null)
                        return RedirectToAction("Index","UserProfile");
                    #endregion
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    #region access log
                    UserHelper.AccessLog(Request.UserHostAddress, "Login", "Failed", User.Identity.Name, "Lockout");
                    #endregion
                    return View("Lockout");
                   
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    #region access log
                    UserHelper.AccessLog(Request.UserHostAddress, "Login", "Failed", User.Identity.Name, "");
                    #endregion
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }
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

        [Authorize(Roles = "Admin,Admin_Regional")]
        public ActionResult Register()
        {
            return View();
        }

        public bool  DeleteConfirmed(string id)
        {
            
                
                var user =  UserManager.FindById(id);
                if (user != null)
                {
                   this.GroupManager.ClearUserGroups(id);
                    var result =  UserManager.Delete(user);
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete user", "Success", User.Identity.Name, "Deleted user" + user.UserName);
                    #endregion
                    return true; 
                }


            return false;
        }

        public List<ApplicationGroup> GetApplicationGroups()
        {
            return GroupManager.Groups.ToList(); 
        }

        [Authorize(Roles = "Admin,Admin_Regional")]
        public  bool Register(RegisterViewModel model)
        {
            

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email ,
                    MiddleName = model.MiddleName ,
                    LastName = model.LastName ,
                    PhoneNo = model.PhoneNo,
                    IsEnabled = true ,
                    FirstName = model.FirstName,
                    EmployeeId = model.EmployeeId,
                    OrgStructureId=model.OrgStructureId,
                
                };
                var result =  UserManager.Create(user, model.Password);
                if (result.Succeeded)
                {
                    //auto sign
                     //SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                     #region access log
                    // UserHelper.AccessLog("", "Create user", "Success", User.Identity.Name, "Created user "+model.UserName);
                     #endregion
                    return true;
                }
               
            }

            // If we got this far, something failed, redisplay form
            return false;
        }
        public  bool Create(ApplicationGroup applicationgroup)
        {
            
            if (ModelState.IsValid)
            {
                 
                var result =  this.GroupManager.CreateGroup(applicationgroup);
                
                return true;
            }

            return false;
        }
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                     
                    return View("ForgotPasswordConfirmation");
                }
 
            }

          
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            #region access log

            string ip = Request.UserHostAddress ?? "";
            UserHelper.AccessLog(ip, "Log Off", "Success", User.Identity.Name, "");
            #endregion
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        public List<ApplicationUser> GetUsers()
        {
            return UserManager.Users.ToList(); 
        }
        public  bool Edit(RegisterViewModel editUser)
        {
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                var user =   UserManager.FindById(editUser.Id);
                if (user == null)
                {
                    return false;
                }


                user.UserName = editUser.UserName;
                user.FirstName = editUser.FirstName;
                user.MiddleName = editUser.MiddleName;
                user.LastName = editUser.LastName;
                user.PhoneNo = editUser.PhoneNo;
                user.Email = editUser.Email;
                user.IsEnabled = editUser.IsEnabled;
                user.EmployeeId = editUser.EmployeeId;
                this.UserManager.Update(user);
                return true;

            }
            return false; 
        }
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

       

        public List<ApplicationRole> GetRoles()
        {
              
               return RoleManager.Roles.ToList();
        }

        [Authorize(Roles = "Admin,Admin_Regional")]
        public ActionResult ChangePasswordAdmin(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ResetPasswordViewModel model = new ResetPasswordViewModel() { UserName = id };
            return View(model);
        }

        [Authorize(Roles = "Admin,Admin_Regional")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePasswordAdmin(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                #region access log
                UserHelper.AccessLog(Request.UserHostAddress, "Reset Password", "Failure", User.Identity.Name, "Reset password for user " + model.UserName);
                #endregion
                return View(model);
            }

            ApplicationDbContext context = new ApplicationDbContext();
           // UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
            //UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
            String hashedNewPassword = UserManager.PasswordHasher.HashPassword(model.Password);
            ApplicationUser cUser = await UserManager.FindByIdAsync(model.UserName);
            await UserManager.RemovePasswordAsync(cUser.Id);
            await UserManager.AddPasswordAsync(cUser.Id, model.Password);
            await UserManager.UpdateAsync(cUser);
            #region access log
            UserHelper.AccessLog(Request.UserHostAddress, "Reset Password", "Success", User.Identity.Name, "Reset password for user " + cUser.UserName);
            #endregion
            return View("PasswordResetConfirm");
        }
    }
}