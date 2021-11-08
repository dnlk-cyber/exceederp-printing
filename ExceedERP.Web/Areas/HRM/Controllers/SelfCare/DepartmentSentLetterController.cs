using System.Web.Mvc;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    public class DepartmentSentLetterController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        //public static DateTime LetterDate { get; set; }
        //public static int RecivedLettersId { get; set; }
        //public static LettersStatus LettersStatus { get; set; }
        //public static string LetterAmharic { get; set; }
        //public static DateTime RecivedDate { get; set; }
        //public static string RecivedDateAmharic { get; set; }
        //public static DateTime OutgoingLetterDate { get; set; }
        //public static string OutgoingLetterDateAm { get; set; }
        //// GET: HRM/DepartmentSentLetter
        //public ActionResult Index()
        //{
        //    ViewData["InstuType"] = db.InstitutionType.AsQueryable();
        //    var context = new ApplicationDbContext();

        //    var allUsers = context.Users.ToList();
        //    ViewData["Users"] = allUsers;
        //    GetDepartments();
        //    return View();
        //}
        //public void GetDepartments()
        //{
        //    List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();
        //    var departments = db.MainCompanyStructures.ToList();

        //    ViewData["Departments"] = departments;
        //}
        //public void SetDate(string name, string value)
        //{
        //    if (name == "RecivedDateAmharic")
        //    {
        //        RecivedDateAmharic = value;
        //        var splitDate = RecivedDateAmharic.Split('/');
        //        RecivedDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
        //    }
        //    else if (name == "OutgoingLetterDateAm")
        //    {
        //        OutgoingLetterDateAm = value;
        //        var splitDate = OutgoingLetterDateAm.Split('/');
        //        OutgoingLetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
        //    }
        //    else if (name == "LetterAmharic")
        //    {
        //        LetterAmharic = value;
        //        var splitDate = LetterAmharic.Split('/');
        //        LetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
        //    }



        //}

        //public ActionResult DepartmentSentLetter_Read([DataSourceRequest]DataSourceRequest request)
        //{
        //    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
        //    int orgId = -1;
        //    var place = db.Placements.FirstOrDefault(x=>x.EmployeeId == empId && x.isCurrent);
        //    if(place != null)
        //    {
        //        orgId = place.OrgStructureId;
        //    }
        //    var lists = db.LetterSender.Where(x => x.Receiver == orgId).OrderBy(x => x.Date).Select(x => x.BookNo);
        //    IQueryable<IncomingLetterRegistration> newincommingletter = db.NewIncommingLetter.Where(x => lists.Contains(x.BookNo) && x.RefernceType == "Institution");
        //    DataSourceResult result = newincommingletter.ToDataSourceResult(request, incommingLetterRegistration => new
        //    {
        //        BookNo = incommingLetterRegistration.BookNo,
        //        LetterDate = incommingLetterRegistration.LetterDate,
        //        LetterReferenceNo = incommingLetterRegistration.LetterReferenceNo,
        //        RecivedDate = incommingLetterRegistration.RecivedDate,
        //        OwnerName = incommingLetterRegistration.OwnerName,
        //        Subject = incommingLetterRegistration.Subject,
        //        Shelve = incommingLetterRegistration.Shelve,
        //        Row = incommingLetterRegistration.Row,
        //        FolderNo = incommingLetterRegistration.FolderNo,
        //        FileNo = incommingLetterRegistration.FileNo,
        //        NoOfAttachment = incommingLetterRegistration.NoOfAttachment,
        //        OrgStructureId = incommingLetterRegistration.OrgStructureId,
        //        RefernceType = "Institution",
        //        Remark = incommingLetterRegistration.Remark,
        //        OutgoingLetterDate = incommingLetterRegistration.OutgoingLetterDate,
        //        OutgoingLetterReferenceNo = incommingLetterRegistration.OutgoingLetterReferenceNo,
        //        OutgoingOwnerName = incommingLetterRegistration.OutgoingOwnerName,
        //        OutgoingInstitutionTypeId = incommingLetterRegistration.OutgoingInstitutionTypeId,
        //        OutgoingSubject = incommingLetterRegistration.OutgoingSubject,
        //        OutgoingShelve = incommingLetterRegistration.OutgoingShelve,
        //        OutgoingRow = incommingLetterRegistration.OutgoingRow,
        //        OutgoingFolderNo = incommingLetterRegistration.OutgoingFolderNo,
        //        OutgoingFileNo = incommingLetterRegistration.OutgoingFileNo,
        //        OutgoingNoOfAttachment = incommingLetterRegistration.OutgoingNoOfAttachment,
        //        OutgoingSerialNo = incommingLetterRegistration.OutgoingSerialNo,
        //        OutgoingRefernceType = incommingLetterRegistration.OutgoingRefernceType,
        //        OutgoingRemark = incommingLetterRegistration.OutgoingRemark,
        //        InstitutionTypeId = incommingLetterRegistration.InstitutionTypeId,
        //        VolumeNo = incommingLetterRegistration.VolumeNo,
        //        OutgoingVolumeNo = incommingLetterRegistration.OutgoingVolumeNo,
        //        LetterAmharic = incommingLetterRegistration.LetterAmharic,
        //        OutgoingLetterDateAm = incommingLetterRegistration.OutgoingLetterDateAm,
        //        RecivedDateAmharic = incommingLetterRegistration.RecivedDateAmharic,
        //        EnableDisbale = incommingLetterRegistration.EnableDisbale,
        //        LastModified = incommingLetterRegistration.LastModified,
        //        DateCreated = incommingLetterRegistration.DateCreated,
        //        CreatedBy = incommingLetterRegistration.CreatedBy,
        //        ModifiedBy = incommingLetterRegistration.ModifiedBy,
        //        OrgBranchName = incommingLetterRegistration.OrgBranchName,
        //        Status = CheckStatus(incommingLetterRegistration.BookNo),
        //        RecivedTime = incommingLetterRegistration.RecivedTime,
        //        OutgoingTime = incommingLetterRegistration.OutgoingTime,

        //    });

        //    return Json(result);
        //}

        //[AcceptVerbs(HttpVerbs.Post)]

        //public ActionResult DepartmentSentLetter_Create([DataSourceRequest]DataSourceRequest request, EmployeeIncomingLetter incomingLetterRegistration)
        //{
        //    ModelState.Clear();
        //    var checkexistance = db.EmployeeIncommingLetter.Where(x => x.BookNo == incomingLetterRegistration.BookNo).FirstOrDefault();
        //    if (checkexistance != null)
        //    {
        //        ModelState.AddModelError("", "This Book No is already exist!!");
        //    }
        //    else if (LetterAmharic == null || RecivedDateAmharic == null)
        //    {
        //        ModelState.AddModelError("", "Incoming Date values are mandatory!!");
        //    }
        //    else if (LetterDate.Date > RecivedDate.Date)
        //    {
        //        ModelState.AddModelError("", "Letter date sholud be less or equal to recived date!!");
        //    }
        //    else if (LetterDate.Date > DateTime.Now.Date)
        //    {
        //        ModelState.AddModelError("", "Letter date sholud be less or equal to current date!!");
        //    }
        //    else if (RecivedDate.Date > DateTime.Now.Date)
        //    {
        //        ModelState.AddModelError("", "Recived date sholud be less or equal to current date!!");
        //    }
        //    if (OutgoingLetterDateAm != null)
        //    {
        //        if (OutgoingLetterDate.Date > DateTime.Now.Date)
        //        {
        //            ModelState.AddModelError("", "Outgoing Letter date sholud be less or equal to current date!!");
        //        }
        //    }
        //    else
        //    {
        //        OutgoingLetterDateAm = RecivedDateAmharic;
        //        OutgoingLetterDate = RecivedDate;
        //    }
        //    if (ModelState.IsValid)
        //    {


        //        var entity = new EmployeeIncomingLetter
        //        {
        //            BookNo = incomingLetterRegistration.BookNo,
        //            LetterDate = LetterDate,
        //            LetterReferenceNo = incomingLetterRegistration.LetterReferenceNo,
        //            RecivedDate = RecivedDate,
        //            OwnerName = incomingLetterRegistration.OwnerName,
        //            Subject = incomingLetterRegistration.Subject,
        //            Shelve = incomingLetterRegistration.Shelve,
        //            Row = incomingLetterRegistration.Row,
        //            FolderNo = incomingLetterRegistration.FolderNo,
        //            FileNo = incomingLetterRegistration.FileNo,
        //            NoOfAttachment = incomingLetterRegistration.NoOfAttachment,
        //            RefersTo = incomingLetterRegistration.RefersTo,
        //            RefernceType = "Employee",
        //            Remark = incomingLetterRegistration.Remark,
        //            OutgoingLetterDate = Convert.ToDateTime(OutgoingLetterDate.Date),
        //            OutgoingLetterReferenceNo = incomingLetterRegistration.OutgoingLetterReferenceNo,
        //            OutgoingOwnerName = incomingLetterRegistration.OutgoingOwnerName,
        //            //OutgoingInstitutionTypeId = incommingLetterRegistration.OutgoingInstitutionTypeId,
        //            OutgoingSubject = incomingLetterRegistration.OutgoingSubject,
        //            OutgoingShelve = incomingLetterRegistration.OutgoingShelve,
        //            OutgoingRow = incomingLetterRegistration.OutgoingRow,
        //            OutgoingFolderNo = incomingLetterRegistration.OutgoingFolderNo,
        //            OutgoingFileNo = incomingLetterRegistration.OutgoingFileNo,
        //            OutgoingNoOfAttachment = incomingLetterRegistration.OutgoingNoOfAttachment,
        //            OutgoingSerialNo = incomingLetterRegistration.OutgoingSerialNo,
        //            OutgoingRefernceType = "Employee",
        //            OutgoingRemark = incomingLetterRegistration.OutgoingRemark,
        //            //  InstitutionTypeId = incommingLetterRegistration.InstitutionTypeId,
        //            VolumeNo = incomingLetterRegistration.VolumeNo,
        //            OutgoingVolumeNo = incomingLetterRegistration.OutgoingVolumeNo,
        //            LetterAmharic = LetterAmharic,
        //            OutgoingLetterDateAm = OutgoingLetterDateAm,
        //            RecivedDateAmharic = RecivedDateAmharic,
        //            EnableDisbale = true,
        //            LastModified = incomingLetterRegistration.LastModified,
        //            DateCreated = DateTime.Now,
        //            CreatedBy = User.Identity.Name,
        //            ModifiedBy = incomingLetterRegistration.ModifiedBy,
        //            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),

        //        };

        //        db.EmployeeIncommingLetter.Add(entity);
        //        db.SaveChanges();
        //        incomingLetterRegistration.BookNo = entity.BookNo;
        //        db = new ExceedDbContext();
        //        var entityy = new EmployeeLetterSender
        //        {
        //            Date = DateTime.Now,
        //            DateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
        //            Receiver = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == incomingLetterRegistration.RefersTo)?.FullNameaEnglish,// incomingLetterRegistration.RefersTo,
        //            Description = incomingLetterRegistration.Subject,
        //            Sender = User.Identity.Name,
        //            BookNo = incomingLetterRegistration.BookNo,
        //            OwnerName = GetOwener(incomingLetterRegistration.BookNo),
        //            Status = LettersStatus.NEW,
        //            FileType = "",
        //            FileName = "-",
        //            File = null
        //        };

        //        db.EmployeeLetterSender.Add(entityy);
        //        db.SaveChanges();

        //    }



        //    return Json(new[] { incomingLetterRegistration }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]

        //public ActionResult DepartmentSentLetter_Update([DataSourceRequest]DataSourceRequest request, EmployeeIncomingLetter incomingLetterRegistration)
        //{
        //    ModelState.Clear();
        //    if (ModelState.IsValid)
        //    {
        //        if (RecivedDateAmharic == null)
        //        {
        //            RecivedDateAmharic = incomingLetterRegistration.RecivedDateAmharic;
        //            var splitDate = RecivedDateAmharic.Split('/');
        //            RecivedDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
        //        }
        //        if (OutgoingLetterDateAm == null)
        //        {
        //            OutgoingLetterDateAm = incomingLetterRegistration.OutgoingLetterDateAm;
        //            var splitDate = OutgoingLetterDateAm.Split('/');
        //            OutgoingLetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
        //        }
        //        if (LetterAmharic == null)
        //        {
        //            LetterAmharic = incomingLetterRegistration.LetterAmharic;
        //            var splitDate = LetterAmharic.Split('/');
        //            LetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
        //        }

        //        var entity = new EmployeeIncomingLetter
        //        {
        //            BookNo = incomingLetterRegistration.BookNo,
        //            LetterDate = LetterDate,
        //            LetterReferenceNo = incomingLetterRegistration.LetterReferenceNo,
        //            RecivedDate = RecivedDate,
        //            OwnerName = incomingLetterRegistration.OwnerName,
        //            Subject = incomingLetterRegistration.Subject,
        //            Shelve = incomingLetterRegistration.Shelve,
        //            Row = incomingLetterRegistration.Row,
        //            FolderNo = incomingLetterRegistration.FolderNo,
        //            FileNo = incomingLetterRegistration.FileNo,
        //            NoOfAttachment = incomingLetterRegistration.NoOfAttachment,
        //            RefersTo = incomingLetterRegistration.RefersTo,
        //            RefernceType = "Employee",
        //            Remark = incomingLetterRegistration.Remark,
        //            OutgoingLetterDate = OutgoingLetterDate,
        //            OutgoingLetterReferenceNo = incomingLetterRegistration.OutgoingLetterReferenceNo,
        //            OutgoingOwnerName = incomingLetterRegistration.OutgoingOwnerName,
        //            //  OutgoingInstitutionTypeId = incommingLetterRegistration.OutgoingInstitutionTypeId,
        //            OutgoingSubject = incomingLetterRegistration.OutgoingSubject,
        //            OutgoingShelve = incomingLetterRegistration.OutgoingShelve,
        //            OutgoingRow = incomingLetterRegistration.OutgoingRow,
        //            OutgoingFolderNo = incomingLetterRegistration.OutgoingFolderNo,
        //            OutgoingFileNo = incomingLetterRegistration.OutgoingFileNo,
        //            OutgoingNoOfAttachment = incomingLetterRegistration.OutgoingNoOfAttachment,
        //            OutgoingSerialNo = incomingLetterRegistration.OutgoingSerialNo,
        //            OutgoingRefernceType = "Employee",
        //            OutgoingRemark = incomingLetterRegistration.OutgoingRemark,
        //            // InstitutionTypeId = incommingLetterRegistration.InstitutionTypeId,
        //            VolumeNo = incomingLetterRegistration.VolumeNo,
        //            OutgoingVolumeNo = incomingLetterRegistration.OutgoingVolumeNo,
        //            LetterAmharic = incomingLetterRegistration.LetterAmharic,
        //            OutgoingLetterDateAm = incomingLetterRegistration.OutgoingLetterDateAm,
        //            RecivedDateAmharic = incomingLetterRegistration.RecivedDateAmharic,
        //            EnableDisbale = true,
        //            LastModified = DateTime.Now,
        //            DateCreated = incomingLetterRegistration.DateCreated,
        //            CreatedBy = incomingLetterRegistration.CreatedBy,
        //            ModifiedBy = User.Identity.Name,
        //            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
        //        };

        //        db.EmployeeIncommingLetter.Attach(entity);
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }

        //    return Json(new[] { incomingLetterRegistration }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]

        //public ActionResult DepartmentSentLetter_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeIncomingLetter incomingLetterRegistration)
        //{
        //    ModelState.Clear();
        //    var newincommingletter = db.EmployeeIncommingLetter.Where(x => x.RefernceType == incomingLetterRegistration.BookNo).FirstOrDefault();
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new EmployeeIncomingLetter
        //        {
        //            BookNo = incomingLetterRegistration.BookNo,
        //            LetterDate = incomingLetterRegistration.LetterDate,
        //            LetterReferenceNo = incomingLetterRegistration.LetterReferenceNo,
        //            RecivedDate = incomingLetterRegistration.RecivedDate,
        //            OwnerName = incomingLetterRegistration.OwnerName,
        //            Subject = incomingLetterRegistration.Subject,
        //            Shelve = incomingLetterRegistration.Shelve,
        //            Row = incomingLetterRegistration.Row,
        //            FolderNo = incomingLetterRegistration.FolderNo,
        //            FileNo = incomingLetterRegistration.FileNo,
        //            NoOfAttachment = incomingLetterRegistration.NoOfAttachment,
        //            RefersTo = incomingLetterRegistration.RefersTo,
        //            RefernceType = "Employee",
        //            Remark = incomingLetterRegistration.Remark,
        //            OutgoingLetterDate = incomingLetterRegistration.OutgoingLetterDate,
        //            OutgoingLetterReferenceNo = incomingLetterRegistration.OutgoingLetterReferenceNo,
        //            OutgoingOwnerName = incomingLetterRegistration.OutgoingOwnerName,
        //            //OutgoingInstitutionTypeId = incommingLetterRegistration.OutgoingInstitutionTypeId,
        //            OutgoingSubject = incomingLetterRegistration.OutgoingSubject,
        //            OutgoingShelve = incomingLetterRegistration.OutgoingShelve,
        //            OutgoingRow = incomingLetterRegistration.OutgoingRow,
        //            OutgoingFolderNo = incomingLetterRegistration.OutgoingFolderNo,
        //            OutgoingFileNo = incomingLetterRegistration.OutgoingFileNo,
        //            OutgoingNoOfAttachment = incomingLetterRegistration.OutgoingNoOfAttachment,
        //            OutgoingSerialNo = incomingLetterRegistration.OutgoingSerialNo,
        //            OutgoingRefernceType = incomingLetterRegistration.OutgoingRefernceType,
        //            OutgoingRemark = incomingLetterRegistration.OutgoingRemark,
        //            //  InstitutionTypeId = incommingLetterRegistration.InstitutionTypeId,
        //            VolumeNo = incomingLetterRegistration.VolumeNo,
        //            OutgoingVolumeNo = incomingLetterRegistration.OutgoingVolumeNo,
        //            LetterAmharic = incomingLetterRegistration.LetterAmharic,
        //            OutgoingLetterDateAm = incomingLetterRegistration.OutgoingLetterDateAm,
        //            RecivedDateAmharic = incomingLetterRegistration.RecivedDateAmharic
        //        };
        //        #region delete log
        //        UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted incomming letter of book no" + incomingLetterRegistration.BookNo);
        //        #endregion
        //        if (incomingLetterRegistration.EnableDisbale)
        //        {
        //            incomingLetterRegistration.EnableDisbale = false;
        //        }
        //        else
        //        {
        //            incomingLetterRegistration.EnableDisbale = true;
        //        }
        //        db.EmployeeIncommingLetter.Attach(entity);
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }

        //    return Json(new[] { incomingLetterRegistration }.ToDataSourceResult(request, ModelState));
        //}
        //public ActionResult UploadDocument(HttpPostedFileBase file, string url, string txtValue, string txtDecsion, string id)
        //{
        //    try
        //    {
        //        HRMInputs.LetterId = id;
        //        HRMInputs.Owner = "Employee";

        //        SetAnswer(id, "", file);



        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        ViewBag.Message = "Upload failed";
        //        return RedirectToAction("Upload");
        //    }
        //}
        //public ActionResult DownloadFile(string filename)
        //{
        //    if (filename == "")
        //    {
        //        string messages = "Please select the row first!!";
        //        return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //    }
        //    HRMInputs.LetterId = filename;
        //    HRMInputs.Owner = "Employee";
        //    db = new ExceedDbContext();
        //    try
        //    {
        //        var attachment =
        //                db.EmployeeLetterSender
        //                    .FirstOrDefault(x => x.LetterSenderId.ToString() == filename);
        //        if (attachment != null)
        //        {
        //            WebClient req = new WebClient();
        //            HttpResponse response = System.Web.HttpContext.Current.Response;
        //            response.Clear();
        //            response.ClearContent();
        //            response.ClearHeaders();
        //            response.Buffer = true;
        //            response.AddHeader("Content-Disposition",
        //                "attachment;filename=\"" + attachment.FileName + "\"");
        //            byte[] datas = attachment.File;
        //            response.BinaryWrite(datas);
        //            response.End();
        //            HRMInputs.SelectedFile = 0;
        //            return File(attachment.FileName, attachment.FileType);
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        // throw;
        //    }
        //    HRMInputs.SelectedFile = 0;


        //    // RedirectToAction("Index");
        //    return new JavaScriptResult() { Script = "alert('" + "There is no attached File!!" + "');" };
        //}
        //public void SetAnswer(string id, string description, HttpPostedFileBase file)
        //{
        //    db = new ExceedDbContext();
        //    LettersStatus = LettersStatus.ANSWERED;
        //    HRMInputs.LetterId = id;
        //    HRMInputs.Owner = "Employee";
        //    string fileName = "";
        //    byte[] image = null;
        //    if (file.ContentLength > 0)
        //    {

        //        fileName = Path.GetFileName(file.FileName);
        //        var path =
        //            Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.LetterId.ToString() + "-" +
        //                               AttachmentType.DOCUMENT), fileName);
        //        var mainPath =
        //            Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.LetterId.ToString() + "-" +
        //                               AttachmentType.DOCUMENT));
        //        if (!Directory.Exists(mainPath))
        //        {
        //            Directory.CreateDirectory(mainPath);
        //        }
        //        if (!System.IO.File.Exists(path))
        //        {



        //            file.SaveAs(path);
        //        }

        //        image = System.IO.File.ReadAllBytes(path);
        //    }
        //    var recivedLetters = db.EmployeeIncommingLetter.Where(x => x.BookNo == id).FirstOrDefault();
        //    if (recivedLetters != null)
        //    {

        //        var status =
        //            db.EmployeeLetterSender.Where(x => x.BookNo == id)
        //                .OrderByDescending(x => x.LetterSenderId)
        //                .FirstOrDefault();
        //        var entity = new EmployeeLetterSender();


        //        entity.OwnerName = recivedLetters.OwnerName;
        //        if (status != null)
        //        {
        //            entity.Receiver = status.Receiver;
        //        }
        //        var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
        //        var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
        //        //if (placment != null)
        //        //{
        //        //    entity.SenderDepartment = placment.OrgStructureId;
        //        //}
        //        entity.Sender = HrHelper.GetEmployeeFullName(empId);
        //        entity.Description = "Answered";
        //        entity.Date = DateTime.Now;
        //        entity.DateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month,
        //            DateTime.Now.Year);

        //        entity.Status = LettersStatus.ANSWERED;
        //        entity.Date = DateTime.Now;
        //        entity.BookNo = id;
        //        entity.FileType = file.ContentType;
        //        entity.File = image;
        //        entity.FileName = fileName;
        //        db = new ExceedDbContext();
        //        db.EmployeeLetterSender.Add(entity);
        //        //db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //    RedirectToAction("Index");
        //}
        //public ActionResult Upload(HttpPostedFileBase file)
        //{
        //    try
        //    {
        //        if (file.ContentLength > 0)
        //        {
        //            db = new ExceedDbContext();
        //            //int letterid = Convert.ToInt32(HRMInputs.LetterId);
        //            //var recivedLetters =
        //            //    db.RecivedLetters.Where(
        //            //        x => x.RecivedLetterId == letterid).FirstOrDefault();
        //            //if (recivedLetters != null)
        //            //{
        //            //    recivedLetters.FileName = file.FileName;
        //            //    var entityy = new RecivedLetters
        //            //    {
        //            //        RecivedLetterId = recivedLetters.RecivedLetterId,
        //            //        InstnPerson = recivedLetters.InstnPerson,
        //            //        Receiver = recivedLetters.Receiver,
        //            //        Sender = recivedLetters.Sender,
        //            //        Description = recivedLetters.Description,
        //            //        FileName = recivedLetters.FileName,
        //            //        LetterStatus = recivedLetters.LetterStatus,
        //            //        RecivedDate = recivedLetters.RecivedDate,
        //            //        BookNo = recivedLetters.BookNo
        //            //    };
        //            //    db = new ExceedDbContext();
        //            //    db.RecivedLetters.Attach(entityy);
        //            //    db.RecivedLetters.Remove(entityy);
        //            //    db.SaveChanges();
        //            //    db = new ExceedDbContext();
        //            //    entityy.LetterStatus = LettersStatus.NEW;
        //            //    db.RecivedLetters.Add(entityy);

        //            //    db.SaveChanges();
        //            //    recivedLetters.RecivedLetterId = entityy.RecivedLetterId;
        //            //    HRMInputs.LetterId = recivedLetters.RecivedLetterId.ToString();
        //            var fileName = Path.GetFileName(file.FileName);
        //            var path = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.LetterId + "-" + AttachmentType.LETTER), fileName);
        //            var mainPath =
        //                Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.LetterId + "-" + AttachmentType.LETTER));
        //            if (!Directory.Exists(mainPath))
        //            {
        //                Directory.CreateDirectory(mainPath);
        //            }
        //            file.SaveAs(path);

        //            var entity = new Attachment
        //            {

        //                Reference = HRMInputs.LetterId.ToString(),
        //                Description = file.FileName,
        //                FileType = file.ContentType,
        //                ReferenceType = AttachmentType.LETTER,
        //                Index = 0,
        //                URL = path,
        //                Remark = HRMInputs.Owner

        //            };

        //            //entity.Reference = recivedLetters.RecivedLetterId.ToString();
        //            //entity.Remark = recivedLetters.RecivedLetterId.ToString();
        //            db.Attachments.Add(entity);

        //            db.SaveChanges();

        //            //}
        //        }
        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        ViewBag.Message = "Upload failed";
        //        return RedirectToAction("Index");
        //    }
        //}
        //public void SetTransfer(string id, string reciver)
        //{
        //    db = new ExceedDbContext();
        //    LettersStatus = LettersStatus.TRANSFERED;
        //    //  RecivedLettersId = id;
        //    var recivedLetters = db.EmployeeIncommingLetter.FirstOrDefault(x => x.BookNo == id);
        //    if (recivedLetters != null)
        //    {
        //        var attachment =
        //            db.LetterSender
        //                .FirstOrDefault(x => x.BookNo == id);
        //        var entity = new EmployeeLetterSender();
        //        var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
        //        var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
        //        //if (placment != null)
        //        //{
        //        //    entity.SenderDepartment = placment.OrgStructureId;
        //        //}
        //        // LetterSenderId = id,
        //        entity.OwnerName = recivedLetters.OwnerName;
        //        entity.Receiver = reciver;
        //        entity.Sender = HrHelper.GetEmployeeFullName(empId);
        //        entity.Description = "Transfred";
        //        entity.Date = DateTime.Now;
        //        entity.DateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month,
        //            DateTime.Now.Year);
        //        if (attachment != null)
        //        {
        //            entity.FileName = attachment.FileName;
        //        }
        //        else
        //        {
        //            entity.FileName = "";
        //        }
        //        entity.Status = LettersStatus.TRANSFERED;
        //        entity.Date = DateTime.Now;
        //        entity.BookNo = id;

        //        db = new ExceedDbContext();
        //        db.EmployeeLetterSender.Add(entity);
        //        //db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //    RedirectToAction("Index");
        //}
        //public ActionResult Attachments_Destroy([DataSourceRequest]DataSourceRequest request, Attachment attachment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new Attachment
        //        {
        //            AttachmentId = attachment.AttachmentId,
        //            ReferenceType = attachment.ReferenceType,
        //            Reference = attachment.Reference,
        //            Description = attachment.Description,
        //            URL = attachment.URL,
        //            BinaryFile = attachment.BinaryFile,
        //            FileType = attachment.FileType,
        //            Index = attachment.Index,
        //            Remark = attachment.Remark
        //        };

        //        db.Attachments.Attach(entity);
        //        db.Attachments.Remove(entity);
        //        db.SaveChanges();
        //        RemoveFiles(attachment.Description);
        //    }

        //    return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        //}
        //public ActionResult RemoveFiles(string fileName)
        //{
        //    if (fileName != null)
        //    {

        //        var physicalPath = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.LetterId + "-" + AttachmentType.LETTER), fileName);

        //        if (System.IO.File.Exists(physicalPath))
        //        {
        //            // Deleting the File from the Location
        //            System.IO.File.Delete(physicalPath);
        //        }
        //    }

        //    return Content("");
        //}
        //public void Download(int id)
        //{
        //    HRMInputs.SelectedFile = id;

        //}
        //public ActionResult UpdateStatus(string BookNo)
        //{
        //    var incommingLetterRegistration = db.EmployeeIncommingLetter.Where(x => x.BookNo == BookNo).FirstOrDefault();
        //    if (incommingLetterRegistration != null)
        //    {

        //        var entity = new EmployeeIncomingLetter
        //        {
        //            BookNo = incommingLetterRegistration.BookNo,
        //            LetterDate = incommingLetterRegistration.LetterDate,
        //            LetterReferenceNo = incommingLetterRegistration.LetterReferenceNo,
        //            RecivedDate = incommingLetterRegistration.RecivedDate,
        //            OwnerName = incommingLetterRegistration.OwnerName,
        //            Subject = incommingLetterRegistration.Subject,
        //            Shelve = incommingLetterRegistration.Shelve,
        //            Row = incommingLetterRegistration.Row,
        //            FolderNo = incommingLetterRegistration.FolderNo,
        //            FileNo = incommingLetterRegistration.FileNo,
        //            NoOfAttachment = incommingLetterRegistration.NoOfAttachment,
        //            RefersTo = incommingLetterRegistration.RefersTo,
        //            RefernceType = "Employee",
        //            Remark = incommingLetterRegistration.Remark,
        //            OutgoingLetterDate = incommingLetterRegistration.OutgoingLetterDate,
        //            OutgoingLetterReferenceNo = incommingLetterRegistration.OutgoingLetterReferenceNo,
        //            OutgoingOwnerName = incommingLetterRegistration.OutgoingOwnerName,
        //            //OutgoingInstitutionTypeId = incommingLetterRegistration.OutgoingInstitutionTypeId,
        //            OutgoingSubject = incommingLetterRegistration.OutgoingSubject,
        //            OutgoingShelve = incommingLetterRegistration.OutgoingShelve,
        //            OutgoingRow = incommingLetterRegistration.OutgoingRow,
        //            OutgoingFolderNo = incommingLetterRegistration.OutgoingFolderNo,
        //            OutgoingFileNo = incommingLetterRegistration.OutgoingFileNo,
        //            OutgoingNoOfAttachment = incommingLetterRegistration.OutgoingNoOfAttachment,
        //            OutgoingSerialNo = incommingLetterRegistration.OutgoingSerialNo,
        //            OutgoingRefernceType = "Employee",
        //            OutgoingRemark = incommingLetterRegistration.OutgoingRemark,
        //            // InstitutionTypeId = incommingLetterRegistration.InstitutionTypeId,
        //            VolumeNo = incommingLetterRegistration.VolumeNo,
        //            OutgoingVolumeNo = incommingLetterRegistration.OutgoingVolumeNo,
        //            LetterAmharic = incommingLetterRegistration.LetterAmharic,
        //            OutgoingLetterDateAm = incommingLetterRegistration.OutgoingLetterDateAm,
        //            RecivedDateAmharic = incommingLetterRegistration.RecivedDateAmharic,


        //            EnableDisbale = true,
        //            LastModified = DateTime.Now,
        //            DateCreated = incommingLetterRegistration.DateCreated,
        //            CreatedBy = incommingLetterRegistration.CreatedBy,
        //            ModifiedBy = User.Identity.Name,
        //            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
        //        };
        //        if (incommingLetterRegistration.EnableDisbale)
        //        {
        //            entity.EnableDisbale = false;
        //        }
        //        else
        //        {
        //            entity.EnableDisbale = true;
        //        }
        //        db = new ExceedDbContext();
        //        db.EmployeeIncommingLetter.Attach(entity);
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();


        //    }

        //    return null;
        //}
        //public int SetStatus(string statusName)
        //{
        //    int status = 0;
        //    if (statusName == "ANSWERED" || statusName == "TRANSFERED")
        //    {
        //        status = 1;
        //    }
        //    return status;
        //}
        //private string GetOwener(string bookno)
        //{
        //    var book = db.EmployeeIncommingLetter.Where(x => x.BookNo == bookno).FirstOrDefault();
        //    if (book != null)
        //    {
        //        return book.OwnerName;
        //    }
        //    return "";

        //}
        //public ActionResult LetterSender_Read([DataSourceRequest]DataSourceRequest request, string bookId)
        //{
        //    IQueryable<EmployeeLetterSender> LetterSender = db.EmployeeLetterSender.Where(x => x.BookNo == bookId);
        //    DataSourceResult result = LetterSender.ToDataSourceResult(request, letterSender => new
        //    {
        //        LetterSenderId = letterSender.LetterSenderId,
        //        Date = letterSender.Date,
        //        DateAmharic = letterSender.DateAmharic,
        //        Receiver = letterSender.Receiver,
        //        Description = letterSender.Description,
        //        Sender = letterSender.Sender,
        //        BookNo = letterSender.BookNo,
        //        OwnerName = letterSender.OwnerName,// GetOwener(bookId),
        //        Status = letterSender.Status,
        //        StatusName = letterSender.StatusName,
        //        FileName = letterSender.FileName,// SetStatus(letterSender.Status.ToString())
        //        CurrentStatus = CheckStatus(letterSender.BookNo),
        //    });

        //    return Json(result);
        //}
        //protected override void Dispose(bool disposing)
        //{
        //    db.Dispose();
        //    base.Dispose(disposing);
        //}
        //public int CheckStatus(string id)
        //{
        //    int result = 0;
        //    var book = db.EmployeeIncommingLetter.FirstOrDefault(x => x.BookNo == id);

        //    if (book != null)
        //    {
        //        var newstat =
        //            db.EmployeeLetterSender.Where(x => x.BookNo == id);

        //        if (newstat.Any())
        //        {
        //            var answer = newstat.FirstOrDefault(x => x.Status == LettersStatus.ANSWERED);
        //            if (answer != null)
        //            {
        //                result = 1;
        //            }
        //            var transfer = newstat.FirstOrDefault(x => x.Status == LettersStatus.TRANSFERED);
        //            if (transfer != null)
        //            {
        //                result = 2;
        //            }
        //        }
        //    }
        //    return result;
        //}
    }
}