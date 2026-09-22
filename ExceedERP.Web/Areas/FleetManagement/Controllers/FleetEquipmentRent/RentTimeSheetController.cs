using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.Web.Areas.FleetManagement.Models;
using System.IO;
using Excel;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Maintenance;
using System.Data.Entity.Validation;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{

    [AuthorizeRoles(FleetManagementRoles.RentUser, FleetManagementRoles.RentApprover)]
    public class RentTimeSheetController : Controller
    {


        ITimeSheetService _TimeSheetService;
        IFleetService _FleetService;
        private IRentFromExternalService _RentFromExternalService;
        public RentTimeSheetController()
        {
            _TimeSheetService = new TimeSheetService();
            _FleetService = new FleetService();
            _RentFromExternalService = new RentFromExternalService();
        }

        //
        // GET: /FleetManagement/TimeSheet/
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public string GetFleetName(Guid Id)
        {


            var fleet = _FleetService.GetById(Id);
            if (fleet != null)
            {
                return fleet.UnitNum + " - " + fleet.PlateNumber;
            }
            else
            {
                return string.Empty;
            }
        }
        public string GetName(Guid Id)
        {


            var fleet = _FleetService.GetById(Id);
            if (fleet != null)
            {
                return fleet.EquipmentName;
            }
            else
            {
                return string.Empty;
            }
        }

        public ActionResult TimeSheet_Import([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> file)
        {
            ExceedDbContext db = new ExceedDbContext();
            string errors = string.Empty;
            string errorsummary = " Successfully Imported TimeSheet ";

            string addtionalName = DateTime.Now.Year.ToString() +
                DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() +
                DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                DateTime.Now.Second.ToString();
            System.Data.DataSet ds = new System.Data.DataSet();


            foreach (HttpPostedFileBase fileBase in file)
            {
                Stream stream = fileBase.InputStream;
                // We return the interface, so that
                IExcelDataReader reader = null;
                string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);
                int t = 0;

                if (fileBase.FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    t = reader.AsDataSet().Tables.Count;
                }
                else if (fileBase.FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    t = reader.AsDataSet().Tables.Count;
                }
                else
                {
                    ModelState.AddModelError("File", "This file format is not supported");
                    return View();
                }

                int V = t;
                reader.IsFirstRowAsColumnNames = true;

                ds = reader.AsDataSet();

                errors = string.Empty;
                var dt = ds.Tables[0];

                for (int i = 1; i < dt.Rows.Count; i++)
                {

                    var AssetCode = dt.Rows[i][0].ToString().Trim();
                    var Fleet = db.Fleets.Where(x => x.UnitNum == AssetCode).FirstOrDefault();
                    while (reader.Read())
                    {
                        if (Fleet != null)
                        {
                            try
                            {

                                var Day = DateTime.Parse(reader.GetString(1));

                                var firstShiftfrom = DateTime.Parse(reader.GetString(2));
                                var firstShiftto = DateTime.Parse(reader.GetString(3));
                                var secondShiftfrom = DateTime.Parse(reader.GetString(4));
                                var secondShiftto = DateTime.Parse(reader.GetString(5));
                                var KmHrReading = int.Parse(reader.GetString(10));
                                var TimeSheet = new TimeSheet
                                {
                                    TimeSheetID = Guid.NewGuid(),
                                    FleetsID = Fleet.FleetsID,
                                    FleetTypeID = Fleet.EquipmentTypeID,
                                    WorkingHoursMorningFrom = firstShiftfrom,
                                    WorkingHoursMorningto = firstShiftto,
                                    WorkingHoursAfternoonFrom = secondShiftfrom,
                                    WorkingHoursAfternoonTo = secondShiftto,
                                    KMHrReading = KmHrReading

                                };



                                db.TimeSheet.Add(TimeSheet);
                                db.SaveChanges();


                            }
                            catch (Exception e)
                            {

                            }




                            if (errors != string.Empty)
                            {
                                errorsummary = errors + " Numbers Failed Import";
                            }



                        }
                    }
                }


            }
            return Json(new { data = errorsummary });

        }

        public ActionResult TimeSheet_Read([DataSourceRequest] DataSourceRequest request,string dataSource,string EquipmentId)
        {
            // var TimeSheet = _TimeSheetService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Where(x => x.Type == MaintenanceConst.RentalTimeSheet).Select(t => new TimeSheet
            var userBranch = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
            IQueryable<TimeSheet> TimeSheet;
            if (dataSource == "External")
            {
                TimeSheet = db.TimeSheet.Where(x => x.OrgBranchName == userBranch).Where(x => x.Type == MaintenanceConst.RentalTimeSheet && x.RentFromExternalEquipmentId == null && x.ExternalRentedEquipmentId.ToString() == EquipmentId).AsQueryable();
            }
            else if (dataSource == "Internal")
            {
                TimeSheet = db.TimeSheet.Where(x => x.OrgBranchName == userBranch).Where(x => x.Type == MaintenanceConst.RentalTimeSheet && x.ExternalRentedEquipmentId == null && x.RentFromExternalEquipmentId.ToString() == EquipmentId).AsQueryable();
            }
            else
            {
                TimeSheet = db.TimeSheet.Where(x => x.OrgBranchName == userBranch).Where(x => x.Type == MaintenanceConst.RentalTimeSheet&&x.RentFromExternalEquipmentId==null&&x.ExternalRentedEquipmentId==null).AsQueryable();
            }
            DataSourceResult Result=TimeSheet.ToDataSourceResult(request,t=>new

                {
                    Date = t.Date,
                    RentalRate = t.RentalRate,
                    IdleTime = t.IdleTime,
                    DownTime = t.DownTime,
                    CustomerName = t.CustomerName,
                    CubicValue = t.CubicValue,
                    PlateSerialNo = GetFleetName(t.FleetsID),
                    EquipmentAdministratorName = t.EquipmentAdministratorName,
                    EquipmentName = GetName(t.FleetsID),
                    FleetsID = t.FleetsID,
                KMHrReading = t.KMHrReading,
                FinalKMHrReading = t.FinalKMHrReading,
                WorkingHoursMorningFrom = t.WorkingHoursMorningFrom,
                WorkingHoursAfternoonTo = t.WorkingHoursAfternoonTo,
                IdealRentalRate = t.IdealRentalRate,
                    TimeSheetID = t.TimeSheetID,
                    WorkingHoursAfternoonFrom = t.WorkingHoursAfternoonFrom,
                    
                    WorkingHoursMorningto = t.WorkingHoursMorningto,
                    TypeofWork = t.TypeofWork,
                    ChargeMethod = t.ChargeMethod,
                    IsOnlineApproved = t.IsOnlineApproved,
                    IsSendForApproval = t.IsSendForApproval,
                    Status = t.Status,
                    DateCreated = t.DateCreated,
                    LastModified = t.LastModified,
                    CreatedBy = t.CreatedBy,
                    ModifiedBy = t.ModifiedBy

                });

            return Json(Result,JsonRequestBehavior.AllowGet);
        }
        public ActionResult TimeSheetForExternal_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {
            IQueryable<TimeSheet> TimeSheet = db.TimeSheet.Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Where(x => x.IsExternal == true && x.FleetsID.ToString() == Id).Select(t => new TimeSheet

            {
                Date = t.Date,
                RentalRate = t.RentalRate,
                IdleTime = t.IdleTime,
                DownTime = t.DownTime,
                CustomerName = t.CustomerName,
                EquipmentAdministratorName = t.EquipmentAdministratorName,
                EquipmentName = t.EquipmentName,
                PlateSerialNo = t.PlateSerialNo,
                FleetsID = t.FleetsID,
                KMHrReading = t.KMHrReading,
                TimeSheetID = t.TimeSheetID,
                WorkingHoursAfternoonFrom = t.WorkingHoursAfternoonFrom,
                WorkingHoursAfternoonTo = t.WorkingHoursAfternoonTo,
                WorkingHoursMorningFrom = t.WorkingHoursMorningFrom,
                WorkingHoursMorningto = t.WorkingHoursMorningto,
                TypeofWork = t.TypeofWork,
                IsOnlineApproved = t.IsOnlineApproved,
                IsSendForApproval = t.IsSendForApproval,
                Status = t.Status,
                RentType = t.RentType,
                DateCreated = t.DateCreated,
                LastModified = t.LastModified,
                CreatedBy = t.CreatedBy,
                ModifiedBy = t.ModifiedBy

            });

            return Json(TimeSheet.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TimeSheetForExternal_Create([DataSourceRequest] DataSourceRequest request, TimeSheet TimeSheet, Guid Id)
        {

            ModelState.Remove("TimeSheetID");
            ModelState.Remove("CustomerName");
            ModelState.Remove("FleetsID");
            ModelState.Remove("FleetTypeID");
            ModelState.Remove("FleetCategoryID");
            IFleetService fleetService = new FleetService();

            if (TimeSheet.WorkingHoursAfternoonFrom > TimeSheet.WorkingHoursAfternoonTo)
            {
                ModelState.AddModelError("InvalidTime", "Starting Time Is Must Be Less Than Ending Time");
            }
            else if (TimeSheet.WorkingHoursMorningFrom > TimeSheet.WorkingHoursMorningto)
            {
                ModelState.AddModelError("InvalidTime", "Starting Time Is Must Be Less Than Ending Time");

            }
            else
            {


                var RentFromExternal = _RentFromExternalService.GetById(Id);
                Fleets fleet = fleetService.GetById(TimeSheet.FleetsID);

                if (TimeSheet != null)
                {
                    TimeSheet.TimeSheetID = Guid.NewGuid();
                    TimeSheet.CustomerName = RentFromExternal.CompanyName;
                    TimeSheet.Date = DateTime.Now;
                    TimeSheet.RentType = fleet.RentType;
                    TimeSheet.Type = MaintenanceConst.RentalTimeSheet;
                    TimeSheet.RentalRate = TimeSheet.RentalRate;
                    TimeSheet.FleetTypeID = Guid.Empty;
                    TimeSheet.FleetCategoryID = Guid.Empty;
                    TimeSheet.EquipmentName = TimeSheet.EquipmentName;
                    TimeSheet.Type = MaintenanceConst.RentalTimeSheet;
                    TimeSheet.CreatedBy = User.Identity.Name;
                    TimeSheet.DateCreated = DateTime.Now;
                    TimeSheet.Status = MaintenanceConst.Pending;
                    TimeSheet.IsExternal = true;
                    TimeSheet.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    TimeSheet.FleetsID = Id;

                    _TimeSheetService.Insert(TimeSheet);
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Fleet timeSheet registration", "Success", User.Identity.Name, $"created timeSheet reg {TimeSheet.TimeSheetID}", Modules.FleetManagement);
                    #endregion
                }
            }

            return Json(new[] { TimeSheet }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TimeSheet_Create([DataSourceRequest] DataSourceRequest request, TimeSheet TimeSheet,string dataSource,string EquipmentId)
        {
            ModelState.Remove("TimeSheetID");
            ModelState.Remove("CustomerName");
            ModelState.Remove("FleetTypeID");
            ModelState.Remove("FleetCategoryID");

            IFleetService fleetService = new FleetService();
            Fleets fleet = fleetService.GetById(TimeSheet.FleetsID);
             if(!ModelState.IsValid)
            {
                ModelState.AddModelError("SetRental", "Set Proper Rental Rate");
            }
            //if (fleet.RentalRate < 1)
            //{
            //    ModelState.AddModelError("SetRental", "Set Proper Rental Rate");

            //}

            else if (TimeSheet.WorkingHoursAfternoonFrom > TimeSheet.WorkingHoursAfternoonTo)
            {
                ModelState.AddModelError("InvalidTime", "Starting Time Is Must Be Less Than Ending Time");
            }
            else if (TimeSheet.WorkingHoursMorningFrom > TimeSheet.WorkingHoursMorningto)
            {
                ModelState.AddModelError("InvalidTime", "Starting Time Is Must Be Less Than Ending Time");

            }

            else
            {

                if (dataSource == "External")
                {
                    Guid Eid = new Guid();
                   if(!string.IsNullOrWhiteSpace(EquipmentId)&&Guid.TryParse(EquipmentId, out Guid Pid)) {
                        Eid = Pid;
                    }
                    if (TimeSheet != null)
                    {
                        TimeSheet.TimeSheetID = Guid.NewGuid();
                        //TimeSheet.Date = DateTime.Now;
                        TimeSheet.RentalRate = TimeSheet.RentalRate;
                        TimeSheet.IdealRentalRate = TimeSheet.IdealRentalRate;
                        TimeSheet.WorkingHoursMorningFrom = TimeSheet.WorkingHoursMorningFrom;
                        TimeSheet.WorkingHoursMorningto = TimeSheet.WorkingHoursMorningto;
                        TimeSheet.WorkingHoursAfternoonFrom = TimeSheet.WorkingHoursAfternoonFrom;
                        TimeSheet.WorkingHoursAfternoonTo = TimeSheet.WorkingHoursAfternoonTo;
                        TimeSheet.IdleTime = TimeSheet.IdleTime;
                        TimeSheet.IdealRentalRate = TimeSheet.IdealRentalRate;
                        TimeSheet.Date = TimeSheet.Date;
                        TimeSheet.CubicValue = TimeSheet.CubicValue;
                        TimeSheet.ExternalRentedEquipmentId =Eid;
                        TimeSheet.BreakDownTime = TimeSheet.BreakDownTime;
                        TimeSheet.TypeofWork = TimeSheet.TypeofWork;
                        TimeSheet.DownTime = TimeSheet.DownTime;
                        TimeSheet.KMHrReading = TimeSheet.KMHrReading;
                        TimeSheet.FinalKMHrReading = TimeSheet.FinalKMHrReading;
                        TimeSheet.FleetsID = TimeSheet.FleetsID;
                        TimeSheet.ChargeMethod = TimeSheet.ChargeMethod;
                        TimeSheet.RentType = fleet.RentType;
                        TimeSheet.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                        TimeSheet.CustomerName = TimeSheet.CustomerName;
                        TimeSheet.FleetTypeID = fleet.EquipmentIdentityID;
                        TimeSheet.FleetCategoryID = fleet.EquipmentTypeID;
                        TimeSheet.EquipmentName = fleet.EquipmentName;
                        TimeSheet.PlateSerialNo = fleet.PlateNumber + "-" + fleet.SerialNo;
                        TimeSheet.IsExternal = fleet.IsExternal;
                        TimeSheet.Type = MaintenanceConst.RentalTimeSheet;
                        TimeSheet.CreatedBy = User.Identity.Name;
                        TimeSheet.DateCreated = DateTime.Now;
                        TimeSheet.Status = MaintenanceConst.Pending;
                        

                        _TimeSheetService.Insert(TimeSheet);
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "Fleet timeSheet service registration", "Success", User.Identity.Name, $"created timesheet service reg {TimeSheet.TimeSheetID}", Modules.FleetManagement);
                        #endregion
                    }
                }
                else if (dataSource == "Internal")
                {
                    int Eid = 0;
                     if(!string.IsNullOrWhiteSpace(EquipmentId)&&int.TryParse(EquipmentId,out int PiD))
                    {
                        Eid = PiD;
                    }
                    if (TimeSheet != null)
                    {
                        TimeSheet.TimeSheetID = Guid.NewGuid();
                        //TimeSheet.Date = DateTime.Now;
                        TimeSheet.RentFromExternalEquipmentId = Eid;
                        TimeSheet.ChargeMethod = TimeSheet.ChargeMethod;
                        TimeSheet.WorkingHoursMorningFrom = TimeSheet.WorkingHoursMorningFrom;
                        TimeSheet.WorkingHoursMorningto = TimeSheet.WorkingHoursMorningto;
                        TimeSheet.WorkingHoursAfternoonFrom = TimeSheet.WorkingHoursAfternoonFrom;
                        TimeSheet.WorkingHoursAfternoonTo = TimeSheet.WorkingHoursAfternoonTo;
                        TimeSheet.IdleTime = TimeSheet.IdleTime;
                        TimeSheet.IdealRentalRate = TimeSheet.IdealRentalRate;
                        TimeSheet.Date = TimeSheet.Date;
                        TimeSheet.CubicValue = TimeSheet.CubicValue;
                        TimeSheet.BreakDownTime = TimeSheet.BreakDownTime;
                        TimeSheet.TypeofWork = TimeSheet.TypeofWork;
                        TimeSheet.KMHrReading = TimeSheet.KMHrReading;
                        TimeSheet.FinalKMHrReading = TimeSheet.FinalKMHrReading;
                        TimeSheet.FleetsID = TimeSheet.FleetsID;
                        TimeSheet.RentType = fleet.RentType;
                        TimeSheet.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                        TimeSheet.CustomerName = fleet.SupplierCompany;
                        TimeSheet.FleetTypeID = fleet.EquipmentIdentityID;
                        TimeSheet.FleetCategoryID = fleet.EquipmentTypeID;
                        TimeSheet.EquipmentName = fleet.EquipmentName;
                        TimeSheet.PlateSerialNo = fleet.PlateNumber + "-" + fleet.SerialNo;
                        TimeSheet.IsExternal = fleet.IsExternal;
                        TimeSheet.Type = MaintenanceConst.RentalTimeSheet;
                        TimeSheet.CreatedBy = User.Identity.Name;
                        TimeSheet.DateCreated = DateTime.Now;
                        TimeSheet.Status = MaintenanceConst.Pending;
                        try
                        {
                            db.TimeSheet.Add(TimeSheet);
                            db.SaveChanges();
                        }
                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityError in ex.EntityValidationErrors)
                            {
                                foreach (var validationError in entityError.ValidationErrors)
                                {
                                    ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                                }
                            }

                            return Json(new[] { TimeSheet }.ToDataSourceResult(request, ModelState));
                        }
                        //_TimeSheetService.Insert(TimeSheet);
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "Fleet timeSheet service registration", "Success", User.Identity.Name, $"created timesheet service reg {TimeSheet.TimeSheetID}", Modules.FleetManagement);
                        #endregion
                    }
                }
                else
                {
                    if (TimeSheet != null)
                    {
                        TimeSheet.TimeSheetID = Guid.NewGuid();
                        //TimeSheet.Date = DateTime.Now;
                        //TimeSheet.ExternalRentedEquipmentId = Eid;
                        TimeSheet.ChargeMethod = TimeSheet.ChargeMethod;
                        TimeSheet.RentalRate = TimeSheet.RentalRate;
                        TimeSheet.IdealRentalRate = TimeSheet.IdealRentalRate;
                        TimeSheet.RentType = fleet.RentType;
                        TimeSheet.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                        TimeSheet.CustomerName = fleet.Location;
                        TimeSheet.FleetTypeID = fleet.EquipmentIdentityID;
                        TimeSheet.FleetCategoryID = fleet.EquipmentTypeID;
                        TimeSheet.EquipmentName = fleet.EquipmentName;
                        TimeSheet.PlateSerialNo = fleet.PlateNumber + "-" + fleet.SerialNo;
                        TimeSheet.IsExternal = fleet.IsExternal;
                        TimeSheet.Type = MaintenanceConst.RentalTimeSheet;
                        TimeSheet.CreatedBy = User.Identity.Name;
                        TimeSheet.DateCreated = DateTime.Now;
                        TimeSheet.Status = MaintenanceConst.Pending;

                        _TimeSheetService.Insert(TimeSheet);
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "Fleet timeSheet service registration", "Success", User.Identity.Name, $"created timesheet service reg {TimeSheet.TimeSheetID}", Modules.FleetManagement);
                        #endregion
                    }
                }
                
            }

            return Json(new[] { TimeSheet }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TimeSheet_Update([DataSourceRequest] DataSourceRequest request, TimeSheet TimeSheet)
        {
            IFleetService fleetService = new FleetService();
            Fleets fleet = fleetService.GetById(TimeSheet.FleetsID);

            if (TimeSheet != null)
            {
                if (fleet.RentalRate < 1)
                {
                    ModelState.AddModelError("SetRental", "Set Proper Rental Rate");

                }

                else if (TimeSheet.WorkingHoursAfternoonFrom > TimeSheet.WorkingHoursAfternoonTo)
                {
                    ModelState.AddModelError("InvalidTime", "Starting Time Is Must Be Less Than Ending Time");
                }
                else if (TimeSheet.WorkingHoursMorningFrom > TimeSheet.WorkingHoursMorningto)
                {
                    ModelState.AddModelError("InvalidTime", "Starting Time Is Must Be Less Than Ending Time");

                }

                else
                {

                    TimeSheet.Date = DateTime.Now;
                    TimeSheet.RentalRate = TimeSheet.RentalRate;
                    TimeSheet.IdealRentalRate = TimeSheet.IdealRentalRate;
                    TimeSheet.RentType = fleet.RentType;
                    TimeSheet.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    TimeSheet.CustomerName = fleet.Location;
                    TimeSheet.FleetTypeID = fleet.EquipmentIdentityID;
                    TimeSheet.FleetCategoryID = fleet.EquipmentTypeID;
                    TimeSheet.EquipmentName = fleet.EquipmentName;
                    TimeSheet.PlateSerialNo = fleet.PlateNumber + "-" + fleet.SerialNo;
                    TimeSheet.IsExternal = fleet.IsExternal;
                    TimeSheet.Type = MaintenanceConst.RentalTimeSheet;
                    TimeSheet.ModifiedBy = User.Identity.Name;
                    TimeSheet.LastModified = DateTime.Now;
                    TimeSheet.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    _TimeSheetService.Update(TimeSheet);
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Timesheet update", "Success", User.Identity.Name, $"updated timesheet reg {TimeSheet.TimeSheetID} ", Modules.FleetManagement);
                    #endregion
                }
            }

            return Json(new[] { TimeSheet }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult TimeSheet_UpdateExternal([DataSourceRequest] DataSourceRequest request, TimeSheet TimeSheet,string dataSource)
        {
            IFleetService fleetService = new FleetService();
            Fleets fleet = fleetService.GetById(TimeSheet.FleetsID);
            string customerName = null;
            if(dataSource=="External")
            {
                customerName = TimeSheet.CustomerName;
            }
            else
            {
                customerName = fleet.SupplierCompany;
            }
            if (TimeSheet != null)
            {
                if (fleet.RentalRate < 1)
                {
                    ModelState.AddModelError("SetRental", "Set Proper Rental Rate");

                }

                else if (TimeSheet.WorkingHoursAfternoonFrom > TimeSheet.WorkingHoursAfternoonTo)
                {
                    ModelState.AddModelError("InvalidTime", "Starting Time Is Must Be Less Than Ending Time");
                }
                else if (TimeSheet.WorkingHoursMorningFrom > TimeSheet.WorkingHoursMorningto)
                {
                    ModelState.AddModelError("InvalidTime", "Starting Time Is Must Be Less Than Ending Time");

                }

                else
                {
                    var ExistingTimeSheet = db.TimeSheet.FirstOrDefault(x => x.TimeSheetID == TimeSheet.TimeSheetID);
                    ExistingTimeSheet.ChargeMethod = TimeSheet.ChargeMethod;
                    ExistingTimeSheet.WorkingHoursMorningFrom = TimeSheet.WorkingHoursMorningFrom;
                    ExistingTimeSheet.WorkingHoursMorningto = TimeSheet.WorkingHoursMorningto;
                    ExistingTimeSheet.WorkingHoursAfternoonFrom = TimeSheet.WorkingHoursAfternoonFrom;
                    ExistingTimeSheet.WorkingHoursAfternoonTo = TimeSheet.WorkingHoursAfternoonTo;
                    ExistingTimeSheet.IdleTime = TimeSheet.IdleTime;
                    ExistingTimeSheet.IdealRentalRate = TimeSheet.IdealRentalRate;
                    ExistingTimeSheet.Date = TimeSheet.Date;
                    ExistingTimeSheet.CubicValue = TimeSheet.CubicValue;
                    ExistingTimeSheet.BreakDownTime = TimeSheet.BreakDownTime;
                    ExistingTimeSheet.TypeofWork = TimeSheet.TypeofWork;
                    ExistingTimeSheet.KMHrReading = TimeSheet.KMHrReading;
                    ExistingTimeSheet.FinalKMHrReading = TimeSheet.FinalKMHrReading;
                    ExistingTimeSheet.FleetsID = TimeSheet.FleetsID;
                    ExistingTimeSheet.RentType = fleet.RentType;
                    ExistingTimeSheet.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    ExistingTimeSheet.CustomerName =customerName;
                    ExistingTimeSheet.FleetTypeID = fleet.EquipmentIdentityID;
                    ExistingTimeSheet.FleetCategoryID = fleet.EquipmentTypeID;
                    ExistingTimeSheet.EquipmentName = fleet.EquipmentName;
                    ExistingTimeSheet.PlateSerialNo = fleet.PlateNumber + "-" + fleet.SerialNo;
                    ExistingTimeSheet.IsExternal = fleet.IsExternal;
                    ExistingTimeSheet.Type = MaintenanceConst.RentalTimeSheet;
                    db.SaveChanges();
                    try
                    {
                        db.Entry(ExistingTimeSheet).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var entityError in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in entityError.ValidationErrors)
                            {
                                ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                            }
                        }

                        return Json(new[] { TimeSheet }.ToDataSourceResult(request, ModelState));
                    }
                    //_TimeSheetService.Update(TimeSheet);
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Timesheet update", "Success", User.Identity.Name, $"updated timesheet reg {TimeSheet.TimeSheetID} ", Modules.FleetManagement);
                    #endregion
                }
            }

            return Json(new[] { TimeSheet }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TimeSheet_Destroy([DataSourceRequest] DataSourceRequest request, TimeSheet timeSheet)
        {
            var TimeSheet = _TimeSheetService.GetById(timeSheet.TimeSheetID);
            if (TimeSheet != null)
            {
                _TimeSheetService.Delete(TimeSheet);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, " Delete Time Sheet number " + timeSheet.TimeSheetID);
            }

            return Json(new[] { TimeSheet }.ToDataSourceResult(request));
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

