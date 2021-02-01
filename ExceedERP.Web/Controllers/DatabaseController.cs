using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Controllers
{
    [Authorize(Roles = Admin)]
    public class DatabaseController : Controller
    {
        private const string Admin = "Admin";
        private ExceedDbContext _db = new ExceedDbContext();

        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["ExceedDbContext"].ConnectionString;
        private string _backupFolderFullPath = "D:BackUps";
        private readonly string _databaseName = "";
        private readonly string[] _systemDatabaseNames = { "master", "tempdb", "model", "msdb" };

        private List<string> backupFiles = new List<string>();
        // GET: Database
        public ActionResult Index()
        {
            ViewBag.DatabaseList = new SelectList(GetAllUserDatabases());

            var configuration = _db.Configurations
                .Where(x => x.Reference == "BACKUP_AND_RESTORE")
                .FirstOrDefault(x=>x.Attribute== "DATABASE_BACKUP_PATH");
            if (configuration != null)
            {
                _backupFolderFullPath = configuration.CurrentValue;
            }
            else
            {
                ViewBag.Error = "Please configure backup path first.";
            }

            try
            {
                if (!Directory.Exists(_backupFolderFullPath)) Directory.CreateDirectory(_backupFolderFullPath);
            }
            catch (Exception e)
            {
                Logger.Error("Messsage: "+e.Message+ "\n StackTrace: " + e.StackTrace + "\n TargetSite: " + e.TargetSite);
            }
            
            try
            {
                backupFiles.AddRange(Directory.GetFiles(_backupFolderFullPath).Select(file => Path.GetFileNameWithoutExtension(file)).ToArray());
                ViewBag.File = backupFiles;
            }
            catch (Exception e)
            {
               Logger.Error("Messsage: "+e.Message+ "\n StackTrace: " + e.StackTrace + "\n TargetSite: " + e.TargetSite);
            }

            return View(backupFiles);
        }

        [HttpPost]
        public async Task<ActionResult> Index(string databaseName)
        {
            if (!String.IsNullOrEmpty(databaseName))
            {
                var configuration = _db.Configurations
                    .Where(x => x.Reference == "BACKUP_AND_RESTORE")
                    .FirstOrDefault(x => x.Attribute == "DATABASE_BACKUP_PATH");
                if (configuration != null)
                {
                    _backupFolderFullPath = configuration.CurrentValue;
                }
                else

                {
                    ViewBag.DatabaseList = new SelectList(GetAllUserDatabases());
                    ViewBag.Error = "Please configure backup path first.";
                    return View(backupFiles);
                }

                bool status =await BackupDatabase(databaseName);
                ViewBag.BakcUpStatus =status? "Database backup successfully." : "Database backup failed.";

                UserHelper.OperationLog(Request.UserHostAddress, "Database backup", status ? "Success" : "Failure",User.Identity.Name, "Database ID " + databaseName,Modules.Administration);
                return RedirectToAction("BackUp");
            }
            else
            {
                ViewBag.Note = "Please select database to restore";
                return View("Error");
            }
        }

        public ActionResult BackUp()
        {
            return View();
        }
        public ActionResult Restore(string id)
        {
            ViewBag.RestoreStatus = "";
            if (!String.IsNullOrEmpty(id))
            {

                string databasename = id.Substring(0,id.Length - 11);
                string fullbackupName = id + ".bak";
                string fullBackpPath=Path.Combine(_backupFolderFullPath, fullbackupName);
                bool status = RestoreDatabase(databasename, fullBackpPath);
                ViewBag.RestoreStatus = status ? "Database restored successfully." : "Database restore failed.";

                UserHelper.OperationLog(Request.UserHostAddress, "Database restore", status ? "Success" : "Failure",
                    User.Identity.Name, "Database ID " + id, Modules.Administration);
            }
            return View();
        }

        public async void BackupAllUserDatabases()
        {
            foreach (string databaseName in GetAllUserDatabases())
            {
               await BackupDatabase(databaseName);
            }
        }

        public async Task<bool>  BackupDatabase(string databaseName)
        {
            bool backupsuccess = false;
            string filePath = BuildBackupPathWithFilename(databaseName);

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = String.Format("BACKUP DATABASE [{0}] TO DISK='{1}'", databaseName, filePath);

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                       // command.ExecuteNonQuery();
                        command.CommandTimeout = 0;
                       await command.ExecuteNonQueryAsync();
                    }
                }
                backupsuccess = true;
            }
            catch (Exception e)
            {
               Logger.Error("Messsage: "+e.Message+ "\n StackTrace: " + e.StackTrace + "\n TargetSite: " + e.TargetSite);
                backupsuccess = false;
            }

            return backupsuccess;
        }
        public bool RestoreDatabase(string databaseName, string filePath)
        {
            bool restored = false;
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = String.Format("RESTORE DATABASE [{0}] FROM DISK '{1}' WITH RECOVERY, STATS = 10;", databaseName, filePath);

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        command.CommandTimeout = 0;
                        command.ExecuteNonQuery();
                    }
                }
                restored = true;

            }
            catch (Exception e)
            {
               Logger.Error("Messsage: "+e.Message+ "\n StackTrace: " + e.StackTrace + "\n TargetSite: " + e.TargetSite);
                restored = false;
            }

            return restored;
        }

        private IEnumerable<string> GetAllUserDatabases()
        {
            var databases = new List<String>();

            DataTable databasesTable;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                databasesTable = connection.GetSchema("Databases");

                connection.Close();
            }

            foreach (DataRow row in databasesTable.Rows)
            {
                string databaseName = row["database_name"].ToString();

                if (_systemDatabaseNames.Contains(databaseName))
                    continue;

                databases.Add(databaseName);
            }

            return databases;
        }

        private string BuildBackupPathWithFilename(string databaseName)
        {
            string filename = string.Format("{0}-{1}.bak", databaseName, DateTime.Now.ToString("yyyy-MM-dd"));

            return Path.Combine(_backupFolderFullPath, filename);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}