using System;
using System.Collections.Generic;
using System.Linq;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using System.Configuration;
using System.Data.SqlClient;
using ExceedERP.Core.Domain.Admin;
using System.IO;
using System.Data;

namespace ExceedERP.Helpers.Common
{

    public static class UserHelper
    {
        
       

        #region general

        //public static List<string> GetUserBranchs(string user)
        // {
        //    using (ExceedDbContext db = new ExceedDbContext())
        //    {
        //        var branchesUserBelongsTo = db.OrgBranchUserMappings.Where(x => x.User == user).Select(x => x.Branch).ToList();
        //        return branchesUserBelongsTo;
        //    }
        // }
         public static string GetUserDefaultBranch(string user)
         {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                string defaultBranch = "";//for create
                var branchObj = db.OrgBranchUserMappings.Where(x => x.User == user).Where(x => x.IsDefault == true).ToList();
                defaultBranch = branchObj != null ? branchObj.Select(x => x.Branch).FirstOrDefault() : "";
                return defaultBranch;
            }
         }
        //public static string UserCanManipulateRecord(string currentuser, string recorduser, string recordbranch)
        //{
        //    using (ExceedDbContext db = new ExceedDbContext())
        //    {
        //        string errOrSuccesMsg = "";//edit and delet
        //                                   //check if the data was created by the user
        //        if (currentuser != recorduser)
        //        {
        //            errOrSuccesMsg = "You can not manipulate this record";
        //        }
        //        else
        //        {
        //            var branchObj = db.OrgBranchUserMappings.Where(x => x.User == currentuser)
        //                                                .Where(x => x.Branch == recordbranch).ToList();
        //            if (branchObj == null)
        //                errOrSuccesMsg = "You can not manipulate this record";
        //        }

        //        return errOrSuccesMsg;
        //    }
        //}

        //public static bool SetDefaultUserBranch(string userid, string branchId)
        //{
        //    using (ExceedDbContext db = new ExceedDbContext())
        //    {
        //        DefaultBranch userDefaultBranch = db.DefaultBranches.Find(userid);
        //        if (branchId != null && userDefaultBranch != null)
        //        {

        //            userDefaultBranch.BranchId = branchId;
        //            db.Entry(userDefaultBranch).State = EntityState.Modified;
        //            db.SaveChanges();
        //            return true;
        //        }

        //        else if (branchId != null && userDefaultBranch == null)
        //        {
        //            userDefaultBranch = new DefaultBranch();
        //            userDefaultBranch.UserId = userid;
        //            userDefaultBranch.BranchId = branchId;
        //            db.DefaultBranches.Add(userDefaultBranch);
        //            db.SaveChanges();
        //            return true;
        //        }
        //        return false;
        //    }
        //}
        public static void AccessLog(string ip, string type, string status, string user, string description)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                GlobalUserAccessLog log = new GlobalUserAccessLog
                {
                    IPAddress = ip,
                    UserName = user,
                    TimeAccessed = DateTime.Now,
                    Type = type,
                    Status = status,
                    Description = description
                };
                //db.GlobalUserAccessLog.Add(log);
                //db.SaveChanges();
            }
        }

        public static void DeleteLog(string ip, string type, string status, string user, string description, Modules module)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                GlobalDeleteLog log = new GlobalDeleteLog
                {
                    IpAddress = ip,
                    UserName = user,
                    TimeAccessed = DateTime.Now,
                    Type = type,
                    Status = status,
                    Description = description
                };
                db.GlobalDeleteLogs.Add(log);
                db.SaveChanges();
            }
        }

        public static void DeleteLog(string ip, string type, string status, string user, string description)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                GlobalDeleteLog log = new GlobalDeleteLog
                {
                    IpAddress = ip,
                    UserName = user,
                    TimeAccessed = DateTime.Now,
                    Type = type,
                    Status = status,
                    Description = description
                };
                db.GlobalDeleteLogs.Add(log);
                db.SaveChanges();
            }
        }
        public static string GetUserNameByEmployeeId(int employeeId)
        {
            string userName = null;

            string strcon = ConfigurationManager.ConnectionStrings["ExceedERPUserManagment"].ConnectionString;

            using (SqlConnection con = new SqlConnection(strcon))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(
                        "SELECT UserName FROM [User].AspNetUsers WHERE EmployeeId = @empId",
                        con);

                    cmd.Parameters.AddWithValue("@empId", employeeId);

                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        userName = result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    // TODO: log error
                }
            }

            return userName; // returns null if not found
        }

        public static void OperationLog(string ip, string type, string status, string user, string description,Modules module)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                GlobalOperationLog log = new GlobalOperationLog
                {
                    IpAddress = ip,
                    UserName = user,
                    TimeAccessed = DateTime.Now,
                    Type = type,
                    Status = status,
                    Description = description,
                    Module = module
                };
                db.GlobalOperationLogs.Add(log);
                db.SaveChanges();
            }
        }

        public static void OperationLog12(string ip, string type, string status, string user, string description)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                GlobalOperationLog log = new GlobalOperationLog
                {
                    IpAddress = ip,
                    UserName = user,
                    TimeAccessed = DateTime.Now,
                    Type = type,
                    Status = status,
                    Description = description
                };
                db.GlobalOperationLogs.Add(log);
                db.SaveChanges();
            }
        }

        public static void ExportLog(string ip, string type, string status, string user, string description)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                GlobalExportLog log = new GlobalExportLog
                {
                    IpAddress = ip,
                    UserName = user,
                    TimeAccessed = DateTime.Now,
                    Type = type,
                    Status = status,
                    Description = description
                };
                db.GlobalExportLog.Add(log);
                db.SaveChanges();
            }
        }

       
        #endregion

        //new

        public static int  GetUserEmployeeId(string user)
        {
            int empid = 0;
           string strcon = ConfigurationManager.ConnectionStrings["ExceedERPUserManagment"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            SqlDataReader myReader = null;
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Select EmployeeId from  [User].AspNetUsers where UserName=@user", con);
                cmd.Parameters.AddWithValue("@user", user);
                myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    empid = int.Parse(myReader[0].ToString());
                }
            }
            catch (Exception exe)
            {
               //log
            }
            finally
            {
                if (myReader != null)
                {
                    myReader.Close();
                }
                if (con != null)
                {
                    con.Close();
                }
            }
            return empid;
        }

        public static string GetEmployeeUserName(int empid)
        {
            string userName = "";
            string strcon = ConfigurationManager.ConnectionStrings["ExceedERPUserManagment"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            SqlDataReader myReader = null;
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Select UserName from  [User].AspNetUsers where EmployeeId=@empid", con);
                cmd.Parameters.AddWithValue("@empid", empid);
                myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    userName =myReader[0].ToString();
                }
            }
            catch (Exception exe)
            {
                //log
            }
            finally
            {
                if (myReader != null)
                {
                    myReader.Close();
                }
                if (con != null)
                {
                    con.Close();
                }
            }
            return userName;
        }
        public static string GetUserId(string user)
        {
            string empid = "";
            string strcon = ConfigurationManager.ConnectionStrings["ExceedERPUserManagment"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            SqlDataReader myReader = null;
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Select Id from [User].AspNetUsers where UserName=@user", con);
                cmd.Parameters.AddWithValue("@user", user);
                myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    empid = (myReader[0].ToString());
                }
            }
            catch (Exception exe)
            {
                //log
            }
            finally
            {
                if (myReader != null)
                {
                    myReader.Close();
                }
                if (con != null)
                {
                    con.Close();
                }
            }
            return empid;
        }

        public static string GetUserName(string userId)
        {
            string empid = "";
            string strcon = ConfigurationManager.ConnectionStrings["ExceedERPUserManagment"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            SqlDataReader myReader = null;
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Select UserName from [User].AspNetUsers where Id=@userid", con);
                cmd.Parameters.AddWithValue("@userid", userId);
                myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    empid = (myReader[0].ToString());
                }
            }
            catch (Exception exe)
            {
                //log
            }
            finally
            {
                if (myReader != null)
                {
                    myReader.Close();
                }
                if (con != null)
                {
                    con.Close();
                }
            }
            return empid;
        }
        public static byte[] GetUserSignature(string userName)
        {
            string userid = "";
        //   string signatures = null;
            byte[] img = null;
            int bufferSize = 81392;
            // The BLOB byte[] buffer to be filled by GetBytes.  
            byte[] signatures = new byte[bufferSize];
            // The bytes returned from GetBytes.  
            long retval;
            // The starting position in the BLOB output.  
            long startIndex = 0;
            BinaryWriter writer;

            string strcon = ConfigurationManager.ConnectionStrings["ExceedERPUserManagment"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            SqlDataReader myReader = null;
            SqlDataReader myReaderTwo = null;

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Select Id from [User].AspNetUsers where UserName=@userName", con);
                cmd.Parameters.AddWithValue("@userName", userName);

                myReader = cmd.ExecuteReader();
                if (myReader.HasRows)
                {
                    while (myReader.Read())
                    {

                        userid = (myReader[0]?.ToString());
                        SqlCommand cmd1 = new SqlCommand("Select Sign from [User].ApplicationUserSignatures where UserId=@userid", con);
                        cmd1.Parameters.AddWithValue("@userid", userid);
                        myReaderTwo = cmd1.ExecuteReader();

                        if (myReaderTwo.HasRows)
                        {
                            while (myReaderTwo.Read())
                            {

                                myReaderTwo.GetBytes(0, 0, signatures, 0, bufferSize);
                            }

                        }
                        else
                        {
                            signatures = null;
                        }


                    }
                }
                else
                {
                    signatures = null;
                }
            }
            catch (Exception exe)
            {
                //log
            }
            finally
            {
                if (myReader != null)
                {
                    myReader.Close();
                }
                if (myReaderTwo != null)
                {
                    myReaderTwo.Close();
                }
                if (con != null)
                {
                    con.Close();
                }
            }
           // signature = Encoding.ASCII.GetBytes(signatures);
            return signatures;
        }

        public static List<string> GetUserBranchId(string user)
        {

            List<string> branchId = new List<string>();
            string strcon = ConfigurationManager.ConnectionStrings["ExceedERPUserManagment"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            SqlDataReader myReader = null;
            try
            {
                string userId = GetUserId(user);
                con.Open();
                SqlCommand cmd = new SqlCommand("Select UserBranchId from  [User].ApplicationUsersBranch where ApplicationUserId=@user", con);
                cmd.Parameters.AddWithValue("@user", userId);
                myReader = cmd.ExecuteReader();

                while (myReader.Read())
                {
                    branchId.Add(myReader[0].ToString());
                }
            }
            catch (Exception exe)
            {
                //log
            }
            finally
            {
                if (myReader != null)
                {
                    myReader.Close();
                }
                if (con != null)
                {
                    con.Close();
                }
            }
            return branchId;
        }

        //mekane
        public static string GetRoleId(string name)
        {
            string empid = "";
            string strcon = ConfigurationManager.ConnectionStrings["ExceedERPUserManagment"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            SqlDataReader myReader = null;
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Select Id from [User].AspNetRoles where Name=@name", con);
                cmd.Parameters.AddWithValue("@name", name);
                myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    empid = (myReader[0].ToString());
                }
            }
            catch (Exception exe)
            {
                //log
            }
            finally
            {
                if (myReader != null)
                {
                    myReader.Close();
                }
                if (con != null)
                {
                    con.Close();
                }
            }
            return empid;
        }
        public static bool CheckUserRole(string userName, string roleName)
        {
            List<string> branchId = new List<string>();
            string strcon = ConfigurationManager.ConnectionStrings["ExceedERPUserManagment"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            SqlDataReader myReader = null;
            try
            {
                string userId = GetUserId(userName);
                string roleId = GetRoleId(roleName);
                con.Open();
                SqlCommand cmd = new SqlCommand("Select UserId from  [User].AspNetUserRoles where UserId=@user and RoleId=@role", con);
                cmd.Parameters.AddWithValue("@user", userId);
                cmd.Parameters.AddWithValue("@role", roleId);
                myReader = cmd.ExecuteReader();

                while (myReader.Read())
                {
                    branchId.Add(myReader[0].ToString());
                }
            }
            catch (Exception exe)
            {
                //log
            }
            finally
            {
                if (myReader != null)
                {
                    myReader.Close();
                }
                if (con != null)
                {
                    con.Close();
                }
            }

            if (branchId.Any())
            {
                return true;
            }


            return false;
        }

        public static List<UserGroupNRoleVm> GetUserGroupsAndRoles(string user)
        {

            List<UserGroupNRoleVm> list = new List<UserGroupNRoleVm>();

            string strcon = ConfigurationManager.ConnectionStrings["ExceedERPUserManagment"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            SqlDataReader myReader = null;
            try
            {

                con.Open();
                SqlCommand cmd = new SqlCommand("Select EmployeeId,UserName,Id from [User].AspNetUsers where UserName=@user", con);
                cmd.Parameters.AddWithValue("@user", user);
                myReader = cmd.ExecuteReader();

                while (myReader.Read())
                {
                    var u = new UserGroupNRoleVm
                    {
                        UserName = myReader[1].ToString()
                    };
                    u.UserId = myReader[2].ToString();
                    u.UserName = myReader[1].ToString();
                    u.FullName = EmployeeHelper.GetEmployeeEnglishName(int.Parse(myReader[0].ToString()));
                    u.WorkUnit = EmployeeHelper.GetEmployeeWorkUnit(int.Parse(myReader[0].ToString()));

                    list.Add(u);
                }
            }
            catch (Exception exe)
            {
                //log
            }
            finally
            {
                if (myReader != null)
                {
                    myReader.Close();
                }
                if (con != null)
                {
                    con.Close();
                }
            }
            foreach (var u in list)
            {
                #region get user groups
                try
                {

                    con.Open();
                    SqlCommand cmd = new SqlCommand("Select Name from [User].ApplicationUserGroups inner join [User].ApplicationGroups on [User].ApplicationUserGroups.ApplicationGroupId=[User].ApplicationGroups.Id where [User].ApplicationUserGroups.ApplicationUserId=@userid", con);
                    cmd.Parameters.AddWithValue("@userid", u.UserId);
                    myReader = cmd.ExecuteReader();

                    while (myReader.Read())
                    {
                        var g = new UserGroupVm
                        {
                            GroupName = myReader[0].ToString()
                        };
                        u.Groups.Add(g);
                    }
                }
                catch (Exception exe)
                {
                    //log
                }
                finally
                {
                    if (myReader != null)
                    {
                        myReader.Close();
                    }
                    if (con != null)
                    {
                        con.Close();
                    }
                }
                #endregion
                #region get user roles
                try
                {

                    con.Open();
                    SqlCommand cmd = new SqlCommand("Select Name from [User].AspNetUserRoles inner join [User].AspNetRoles on [User].AspNetUserRoles.RoleId=[User].AspNetRoles.Id where [User].AspNetUserRoles.UserId=@userid", con);
                    cmd.Parameters.AddWithValue("@userid", u.UserId);
                    myReader = cmd.ExecuteReader();

                    while (myReader.Read())
                    {
                        var r = new UserRoleVm
                        {
                            RoleName = myReader[0].ToString()
                        };
                        u.Roles.Add(r);
                    }
                }
                catch (Exception exe)
                {
                    //log
                }
                finally
                {
                    if (myReader != null)
                    {
                        myReader.Close();
                    }
                    if (con != null)
                    {
                        con.Close();
                    }
                }
                #endregion
            }
            return list;
        }
        public static List<UsersInGroupAndRoleVm> GetUsersInGroupAndRoles(string groupid, string roleId)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {

                List<UsersInGroupAndRoleVm> list = new List<UsersInGroupAndRoleVm>();

                string strcon = ConfigurationManager.ConnectionStrings["ExceedERPUserManagment"].ConnectionString;
                SqlConnection con = new SqlConnection(strcon);
                SqlDataReader myReader = null;
                int empId = 0;
                if (!string.IsNullOrEmpty(roleId))
                {
                    #region get users
                    try
                    {

                        con.Open();
                        SqlCommand cmd = new SqlCommand("Select U.EmployeeId,U.UserName,U.PhoneNo from [User].AspNetUsers U JOIN [User].AspNetUserRoles UR ON U.Id=UR.Userid JOIN [User].AspNetRoles R ON UR.Roleid=R.Id where R.Name=@roleId", con);
                        cmd.Parameters.AddWithValue("@roleId", roleId);
                        myReader = cmd.ExecuteReader();

                        while (myReader.Read())
                        {
                            empId = int.Parse(myReader[0].ToString());
                            var u = new UsersInGroupAndRoleVm
                            {
                                UserName = myReader[1].ToString()
                            };

                            var currentPlacement = db.Placements.Where(x => x.isCurrent && x.EmployeeId == empId).FirstOrDefault();
                            if (currentPlacement != null)
                            {
                                var lookUp = db.Lookups.Find(currentPlacement.LookupId);
                                u.Title = lookUp.Description;
                            }

                            u.Phone = myReader[2].ToString();
                            u.UserName = myReader[1].ToString();
                            u.FullName = EmployeeHelper.GetEmployeeEnglishName(empId);
                            u.WorkUnit = EmployeeHelper.GetEmployeeWorkUnit(empId);

                            list.Add(u);
                        }
                    }
                    catch (Exception exe)
                    {
                        //log
                    }
                    finally
                    {
                        if (myReader != null)
                        {
                            myReader.Close();
                        }
                        if (con != null)
                        {
                            con.Close();
                        }
                    }
                    #endregion
                }
                else if (!string.IsNullOrEmpty(groupid))
                {
                    #region get users by group
                    try
                    {

                        con.Open();
                        SqlCommand cmd = new SqlCommand("Select U.EmployeeId,U.UserName,U.PhoneNo from [User].AspNetUsers U JOIN [User].ApplicationUserGroups UG ON U.Id=UG.ApplicationUserId JOIN [User].ApplicationGroups G ON UG.ApplicationGroupId=G.Id where G.Id=@groupid", con);
                        cmd.Parameters.AddWithValue("@groupid", groupid);
                        myReader = cmd.ExecuteReader();

                        while (myReader.Read())
                        {
                            empId = int.Parse(myReader[0].ToString());
                            var u = new UsersInGroupAndRoleVm
                            {
                                UserName = myReader[1].ToString()
                            };

                            var currentPlacement = db.Placements.Where(x => x.isCurrent && x.EmployeeId == empId).FirstOrDefault();
                            if (currentPlacement != null)
                            {
                                var lookUp = db.Lookups.Find(currentPlacement.LookupId);
                                u.Title = lookUp.Description;
                            }

                            u.Phone = myReader[2].ToString();
                            u.UserName = myReader[1].ToString();
                            u.FullName = EmployeeHelper.GetEmployeeEnglishName(int.Parse(myReader[0].ToString()));
                            u.WorkUnit = EmployeeHelper.GetEmployeeWorkUnit(int.Parse(myReader[0].ToString()));

                            list.Add(u);
                        }
                    }
                    catch (Exception exe)
                    {
                        //log
                    }
                    finally
                    {
                        if (myReader != null)
                        {
                            myReader.Close();
                        }
                        if (con != null)
                        {
                            con.Close();
                        }
                    }
                    #endregion
                }

                return list;
            }
        }
        public static List<RoleVm> GetRoles()
        {

            List<RoleVm> list = new List<RoleVm>();

            string strcon = ConfigurationManager.ConnectionStrings["ExceedERPUserManagment"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            SqlDataReader myReader = null;
            #region get users
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Select Name,Id from [User].AspNetRoles", con);
                
                myReader = cmd.ExecuteReader();

                while (myReader.Read())
                {
                    var u = new RoleVm
                    {
                        Name = myReader[0].ToString()
                    };
                    u.RoleId = myReader[1].ToString();
                    list.Add(u);
                }
            }
            catch (Exception exe)
            {
                //log
            }
            finally
            {
                if (myReader != null)
                {
                    myReader.Close();
                }
                if (con != null)
                {
                    con.Close();
                }
            }
            #endregion


            return list;
        }
        public static List<GroupVM> GetGroups()
        {

            List<GroupVM> list = new List<GroupVM>();

            string strcon = ConfigurationManager.ConnectionStrings["ExceedERPUserManagment"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            SqlDataReader myReader = null;
            #region get users
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Select Id,Name from [User].ApplicationGroups", con);

                myReader = cmd.ExecuteReader();

                while (myReader.Read())
                {
                    var u = new GroupVM
                    {
                        Name = myReader[1].ToString()
                    };
                    u.Id = myReader[0].ToString();
                    list.Add(u);
                }
            }
            catch (Exception exe)
            {
                //log
            }
            finally
            {
                if (myReader != null)
                {
                    myReader.Close();
                }
                if (con != null)
                {
                    con.Close();
                }
            }
            #endregion


            return list;
        }
        //public static void OperationLog(object userHostAddress, string v1, string v2, object name, string v3, object finance)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
