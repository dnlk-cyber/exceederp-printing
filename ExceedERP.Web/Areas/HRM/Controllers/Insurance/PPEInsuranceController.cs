using ExceedERP.DataAccess.Context;
using System;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Insurance
{
    public class PPEInsuranceController : Controller
    {
        // GET: HRM/PPEInsurance
        ExceedDbContext db = new ExceedDbContext();
        public static DateTime RenewalDate { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            //ViewData["Equipment"] = db.Fleets.ToList();

            //var types = db.InsuranceType.ToList();
            //List<InsuranceType> insuranceList = new List<InsuranceType>();
            //foreach (var type in types)
            //{
            //    InsuranceType insurance = new InsuranceType();
            //    var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == type.LookupId && x.LookupType == Core.Domain.Common.LookUpTypes.INSURANCE_CLASS_OF_BUSINESS);
            //    if (lookup != null)
            //    {
            //        insurance.InsuranceTypeId = type.InsuranceTypeId;
            //        insurance.Name = lookup.Description;
            //    }
            //    insuranceList.Add(insurance);
            //}
            //ViewData["InsuranceType"] = insuranceList;
            //ViewData["InsuranceCatagory"] = db.InsuranceCategory.ToList();

        }

        //public JsonResult PPEInsurance_Read([DataSourceRequest] DataSourceRequest request)
        //{
        //    var accident = db.FleetInsurance.Where(x => x.IsSendForApproval).ToList();
        //    var accidents = accident.Select(a => new FleetInsurance
        //    {
        //        FleetsID = a.FleetsID,
        //        City = a.City,
        //        CreatedBy = a.CreatedBy,
        //        DateCreated = a.DateCreated,
        //        Description = a.Description,
        //        DueDate = a.DueDate,
        //        Email = a.Email,
        //        ExpireDate = a.ExpireDate,
        //        FleetInsuranceID = a.FleetInsuranceID,
        //        InsuranceCompany = a.InsuranceCompany,
        //        IsOnlineApproved = a.IsOnlineApproved,
        //        IsOnlineTransferred = a.IsOnlineTransferred,
        //        IsOnlineTransferredBy = a.IsOnlineTransferredBy,
        //        IsSendForApproval = a.IsSendForApproval,
        //        IsVoid = a.IsVoid,
        //        LastModified = a.LastModified,
        //        ModifiedBy = a.ModifiedBy,
        //        OnlineApprovedBy = a.OnlineApprovedBy,
        //        OnlineApprovedTime = a.OnlineApprovedTime,
        //        OnlineTransferredTime = a.OnlineTransferredTime,
        //        OrgBranchName = a.OrgBranchName,
        //        PhoneNo = a.PhoneNo,
        //        PremiumCost = a.PremiumCost,
        //        Remark = a.Remark,
        //        SendForApprovalBy = a.SendForApprovalBy,
        //        SendForApprovalTime = a.SendForApprovalTime,
        //        Status = a.Status,
        //        VoidBy = a.VoidBy,
        //        InsuranceTypeId = a.InsuranceTypeId,
        //        InsuranceCategoryId = a.InsuranceCategoryId,
        //        VoidTime = a.VoidTime,
        //        SerialNo = a.SerialNo
                



        //    }).ToList();

        //    return Json(accidents.ToDataSourceResult(request));
        //}
        //public JsonResult PPEInsurance_Create([DataSourceRequest] DataSourceRequest request, Accident Accident)
        //{

        //    ModelState.Remove("AccidentID");
        //    Accident.AccidentID = Guid.NewGuid();
        //    Accident.CreatedBy = User.Identity.Name;
        //    Accident.DateCreated = DateTime.Now;


        //    if (Accident != null)
        //    {
        //        //_Accidentervice.Insert(Accident);
        //    }

        //    return Json(new[] { Accident }.ToDataSourceResult(request));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public JsonResult PPEInsurance_Update([DataSourceRequest] DataSourceRequest request, FleetInsurance a)
        //{

        //    //var entity = db.Accident.FirstOrDefault(x => x.AccidentID == Accident.AccidentID);
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new FleetInsurance
        //        {
        //            FleetsID = a.FleetsID,
        //            City = a.City,
        //            CreatedBy = a.CreatedBy,
        //            DateCreated = a.DateCreated,
        //            Description = a.Description,
        //            DueDate = a.DueDate,
        //            Email = a.Email,
        //            ExpireDate = a.ExpireDate,
        //            FleetInsuranceID = a.FleetInsuranceID,
        //            InsuranceCompany = a.InsuranceCompany,
        //            IsOnlineApproved = a.IsOnlineApproved,
        //            IsOnlineTransferred = a.IsOnlineTransferred,
        //            IsOnlineTransferredBy = a.IsOnlineTransferredBy,
        //            IsSendForApproval = a.IsSendForApproval,
        //            IsVoid = a.IsVoid,
        //            LastModified = a.LastModified,
        //            ModifiedBy = a.ModifiedBy,
        //            OnlineApprovedBy = a.OnlineApprovedBy,
        //            OnlineApprovedTime = a.OnlineApprovedTime,
        //            OnlineTransferredTime = a.OnlineTransferredTime,
        //            OrgBranchName = a.OrgBranchName,
        //            PhoneNo = a.PhoneNo,
        //            PremiumCost = a.PremiumCost,
        //            Remark = a.Remark,
        //            SendForApprovalBy = a.SendForApprovalBy,
        //            SendForApprovalTime = a.SendForApprovalTime,
        //            Status = a.Status,
        //            VoidBy = a.VoidBy,
        //            InsuranceTypeId = a.InsuranceTypeId,
        //            InsuranceCategoryId = a.InsuranceCategoryId,
        //            VoidTime = a.VoidTime

        //        };
        //        db.FleetInsurance.Attach(entity);
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
           

        //    return Json(new[] { a }.ToDataSourceResult(request));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public JsonResult PPEInsurance_Destroy([DataSourceRequest] DataSourceRequest request, FleetInsurance a)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        var entity = new FleetInsurance
        //        {
        //            FleetsID = a.FleetsID,
        //            City = a.City,
        //            CreatedBy = a.CreatedBy,
        //            DateCreated = a.DateCreated,
        //            Description = a.Description,
        //            DueDate = a.DueDate,
        //            Email = a.Email,
        //            ExpireDate = a.ExpireDate,
        //            FleetInsuranceID = a.FleetInsuranceID,
        //            InsuranceCompany = a.InsuranceCompany,
        //            IsOnlineApproved = a.IsOnlineApproved,
        //            IsOnlineTransferred = a.IsOnlineTransferred,
        //            IsOnlineTransferredBy = a.IsOnlineTransferredBy,
        //            IsSendForApproval = a.IsSendForApproval,
        //            IsVoid = a.IsVoid,
        //            LastModified = a.LastModified,
        //            ModifiedBy = a.ModifiedBy,
        //            OnlineApprovedBy = a.OnlineApprovedBy,
        //            OnlineApprovedTime = a.OnlineApprovedTime,
        //            OnlineTransferredTime = a.OnlineTransferredTime,
        //            OrgBranchName = a.OrgBranchName,
        //            PhoneNo = a.PhoneNo,
        //            PremiumCost = a.PremiumCost,
        //            Remark = a.Remark,
        //            SendForApprovalBy = a.SendForApprovalBy,
        //            SendForApprovalTime = a.SendForApprovalTime,
        //            Status = a.Status,
        //            VoidBy = a.VoidBy,
        //            InsuranceTypeId = a.InsuranceTypeId,
        //            InsuranceCategoryId = a.InsuranceCategoryId,
        //            VoidTime = a.VoidTime

        //        };
        //        #region delete log
        //        UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted PPE insurance " + a.PlateNo);
        //        #endregion
        //        db.FleetInsurance.Attach(entity);
        //        db.FleetInsurance.Remove(entity);
        //        db.SaveChanges();

        //        //_Accidentervice.Update(Accident);
        //    }

        //    return Json(new[] { a }.ToDataSourceResult(request));
        //}
    }
}