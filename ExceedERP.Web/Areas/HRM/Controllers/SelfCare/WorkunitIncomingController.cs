using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [Authorize(Roles = HR_Employee_SelfService + "," + HR_Admin)]
    public class WorkunitIncomingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Employee_SelfService = "HR_Employee_SelfService";
        private const string HR_Admin = "HR_Admin";
    //    public static DateTime LetterDate { get; set; }
    //    public static int RecivedLettersId { get; set; }
    //    public static LettersStatus LettersStatus { get; set; }
    //    public static string LetterAmharic{ get; set; }
    //    public static DateTime RecivedDate { get; set; }
    //    public static string RecivedDateAmharic { get; set; }
    //    public static DateTime OutgoingLetterDate { get; set; }
    //    public static string OutgoingLetterDateAm { get; set; }
    //    string ErrorMessage = "";
      
    //    public ActionResult Index()
    //    {
    //        ViewData["InstuType"] = db.InstitutionType.AsQueryable();
    //        var context = new ApplicationDbContext();

    //        var allUsers = context.Users.ToList();
    //        ViewData["Users"] = allUsers;
    //        GetDepartments();
    //        return View();
    //    }
    //    public void GetDepartments()
    //    {
    //        List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();
    //        var departments = db.MainCompanyStructures.ToList();
           
    //        ViewData["Departments"] = departments;
    //    }
    //    public int CheckStatus(string id)
    //    {
    //        int result = 0;
    //        int empID = UserInformationHelper.GetEmployeeId(User.Identity.Name);
    //        var palcment = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empID);
    //        var book = db.NewIncommingLetter.FirstOrDefault(x => x.BookNo == id);

    //        if (book != null)
    //        {
    //            var newstat =
    //                db.LetterSender.Where(x => x.BookNo == id && x.SenderDepartment ==palcment .OrgStructureId  ).ToList();

    //            if (newstat.Any())
    //            {
    //                var answer = newstat.FirstOrDefault(x => x.Status == LettersStatus.ANSWERED);
    //                if (answer != null)
    //                {
    //                    result = 1;
    //                }
    //                var transfer = newstat.FirstOrDefault(x => x.Status == LettersStatus.TRANSFERED);
    //                if (transfer != null)
    //                {
    //                    result = 2;
    //                }
    //            }
    //        }
    //        return result;
    //    }
    //    public void SetDate(string name, string value)
    //    {
    //        if (name == "RecivedDateAmharic")
    //        {
    //            RecivedDateAmharic = value;
    //            var splitDate = RecivedDateAmharic.Split('/');
    //            RecivedDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
    //        }
    //        else if (name == "OutgoingLetterDateAm")
    //        {
    //            OutgoingLetterDateAm = value;
    //            var splitDate = OutgoingLetterDateAm.Split('/');
    //            OutgoingLetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
    //        }
    //        else if (name == "LetterAmharic")
    //        {
    //            LetterAmharic = value;
    //            var splitDate = LetterAmharic.Split('/');
    //            LetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
    //        }



    //    }
    //        [Authorize(Roles = HR_Admin+","+HR_Employee_SelfService )]
    //    public ActionResult NewIncommingLetter_Read([DataSourceRequest]DataSourceRequest request)
    //    {
    //        int empID = UserInformationHelper.GetEmployeeId(User.Identity.Name);
    //        var palcment = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empID);
    //        var lists = db.LetterSender.Where(x => x.Receiver == palcment.OrgStructureId).OrderBy(x => x.Date).Select(x => x.BookNo).ToList();
    //        IQueryable<IncomingLetterRegistration> newincommingletter = db.NewIncommingLetter.Where(x => x.RefernceType == "Institution" && lists.Contains(x.BookNo));
    //        DataSourceResult result = newincommingletter.ToDataSourceResult(request, incommingLetterRegistration => new {
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
    //            OrgStructureId = incommingLetterRegistration.OrgStructureId,
    //            RefernceType = "Institution",
    //            Remark = incommingLetterRegistration.Remark,
    //            OutgoingLetterDate = incommingLetterRegistration.OutgoingLetterDate,
    //            OutgoingLetterReferenceNo = incommingLetterRegistration.OutgoingLetterReferenceNo,
    //            OutgoingOwnerName = incommingLetterRegistration.OutgoingOwnerName,
    //            OutgoingInstitutionTypeId = incommingLetterRegistration.OutgoingInstitutionTypeId,
    //            OutgoingSubject = incommingLetterRegistration.OutgoingSubject,
    //            OutgoingShelve = incommingLetterRegistration.OutgoingShelve,
    //            OutgoingRow = incommingLetterRegistration.OutgoingRow,
    //            OutgoingFolderNo = incommingLetterRegistration.OutgoingFolderNo,
    //            OutgoingFileNo = incommingLetterRegistration.OutgoingFileNo,
    //            OutgoingNoOfAttachment = incommingLetterRegistration.OutgoingNoOfAttachment,
    //            OutgoingSerialNo = incommingLetterRegistration.OutgoingSerialNo,
    //            OutgoingRefernceType = incommingLetterRegistration.OutgoingRefernceType,
    //            OutgoingRemark = incommingLetterRegistration.OutgoingRemark,
    //            InstitutionTypeId = incommingLetterRegistration.InstitutionTypeId,
    //            VolumeNo = incommingLetterRegistration.VolumeNo,
    //            OutgoingVolumeNo = incommingLetterRegistration.OutgoingVolumeNo,
    //            LetterAmharic = incommingLetterRegistration.LetterAmharic,
    //            OutgoingLetterDateAm = incommingLetterRegistration.OutgoingLetterDateAm,
    //            RecivedDateAmharic = incommingLetterRegistration.RecivedDateAmharic,
    //            EnableDisbale = incommingLetterRegistration.EnableDisbale,
    //            LastModified = incommingLetterRegistration.LastModified,
    //            DateCreated = incommingLetterRegistration.DateCreated,
    //            CreatedBy = incommingLetterRegistration.CreatedBy,
    //            ModifiedBy = incommingLetterRegistration.ModifiedBy,
    //            OrgBranchName = incommingLetterRegistration.OrgBranchName,
    //            Status = CheckStatus(incommingLetterRegistration.BookNo),
                
    //        });

    //        return Json(result);
    //    }

    //      [AcceptVerbs(HttpVerbs.Post)]
    //      [Authorize(Roles = HR_Admin)]
    //      public ActionResult NewIncommingLetter_Create([DataSourceRequest] DataSourceRequest request,
    //          IncomingLetterRegistration incommingLetterRegistration)
    //      {
    //          ModelState.Clear();

    //          var checkexistance =
    //              db.NewIncommingLetter.Where(x => x.BookNo == incommingLetterRegistration.BookNo).FirstOrDefault();
    //          if (checkexistance != null)
    //          {
    //              ModelState.AddModelError("", "This Book No is already exist!!");
    //          }
    //          else if (LetterAmharic == null)
    //          {
    //              ModelState.AddModelError("", "Incoming Letter Date value is mandatory!!");
    //          }

    //          else if (RecivedDateAmharic == null)
    //          {
    //              ModelState.AddModelError("", "Incoming Received Date value is mandatory!!");
    //          }
    //          else if (LetterDate.Date > RecivedDate.Date)
    //          {
    //              ModelState.AddModelError("", "Letter date sholud be less or equal to recived date!!");
    //          }
    //          else if (LetterDate.Date > DateTime.Now.Date)
    //          {
    //              ModelState.AddModelError("", "Letter date sholud be less or equal to current date!!");
    //          }
    //          else if (RecivedDate.Date > DateTime.Now.Date)
    //          {
    //              ModelState.AddModelError("", "Recived date sholud be less or equal to current date!!");
    //          }
    //          else if (OutgoingLetterDateAm != null)
    //          {
    //              if (OutgoingLetterDate.Date > DateTime.Now.Date)
    //              {
    //                  ModelState.AddModelError("", "Outgoing Letter date sholud be less or equal to current date!!");
    //              }
    //          }
    //          else if (OutgoingLetterDateAm == null)
    //          {
    //              OutgoingLetterDateAm = RecivedDateAmharic;
    //              OutgoingLetterDate = RecivedDate;
    //          }
    //          if (ModelState.IsValid)
    //              {
    //                  var entity = new IncomingLetterRegistration
    //                  {
    //                      BookNo = incommingLetterRegistration.BookNo,
    //                      LetterDate = LetterDate,
    //                      LetterReferenceNo = incommingLetterRegistration.LetterReferenceNo,
    //                      RecivedDate = RecivedDate,
    //                      OwnerName = incommingLetterRegistration.OwnerName,
    //                      Subject = incommingLetterRegistration.Subject,
    //                      Shelve = incommingLetterRegistration.Shelve,
    //                      Row = incommingLetterRegistration.Row,
    //                      FolderNo = incommingLetterRegistration.FolderNo,
    //                      FileNo = incommingLetterRegistration.FileNo,
    //                      NoOfAttachment = incommingLetterRegistration.NoOfAttachment,
    //                      OrgStructureId = incommingLetterRegistration.OrgStructureId,
    //                      RefernceType = "Institution",
    //                      Remark = incommingLetterRegistration.Remark,
    //                      OutgoingLetterDate = OutgoingLetterDate,
    //                      OutgoingLetterReferenceNo = incommingLetterRegistration.OutgoingLetterReferenceNo,
    //                      OutgoingOwnerName = incommingLetterRegistration.OutgoingOwnerName,
    //                      OutgoingInstitutionTypeId = incommingLetterRegistration.OutgoingInstitutionTypeId,
    //                      OutgoingSubject = incommingLetterRegistration.OutgoingSubject,
    //                      OutgoingShelve = incommingLetterRegistration.OutgoingShelve,
    //                      OutgoingRow = incommingLetterRegistration.OutgoingRow,
    //                      OutgoingFolderNo = incommingLetterRegistration.OutgoingFolderNo,
    //                      OutgoingFileNo = incommingLetterRegistration.OutgoingFileNo,
    //                      OutgoingNoOfAttachment = incommingLetterRegistration.OutgoingNoOfAttachment,
    //                      OutgoingSerialNo = incommingLetterRegistration.OutgoingSerialNo,
    //                      OutgoingRefernceType = "Institution",
    //                      OutgoingRemark = incommingLetterRegistration.OutgoingRemark,
    //                      InstitutionTypeId = incommingLetterRegistration.InstitutionTypeId,
    //                      VolumeNo = incommingLetterRegistration.VolumeNo,
    //                      OutgoingVolumeNo = incommingLetterRegistration.OutgoingVolumeNo,
    //                      LetterAmharic = LetterAmharic,
    //                      OutgoingLetterDateAm = OutgoingLetterDateAm,
    //                      RecivedDateAmharic = RecivedDateAmharic,
    //                      EnableDisbale = true,
    //                      LastModified = incommingLetterRegistration.LastModified,
    //                      DateCreated = DateTime.Now,
    //                      CreatedBy = User.Identity.Name,
    //                      ModifiedBy = incommingLetterRegistration.ModifiedBy,
    //                      OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
    //                  };

    //                  db.NewIncommingLetter.Add(entity);
    //                  db.SaveChanges();
    //                  incommingLetterRegistration.BookNo = entity.BookNo;

    //                  db = new ExceedDbContext();
    //                  var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
    //                  var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == id && x.isCurrent);
    //                  var entityy = new LetterSender
    //                  {
    //                      Date = DateTime.Now,
    //                      DateAmharic =
    //                          XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month,
    //                              DateTime.Now.Year),
    //                      Receiver = incommingLetterRegistration.OrgStructureId,
    //                      Description = incommingLetterRegistration.Subject,
    //                      Sender = HrHelper.GetEmployeeFullName(id),

    //                      BookNo = incommingLetterRegistration.BookNo,
    //                      OwnerName = GetOwener(incommingLetterRegistration.BookNo),
    //                      Status = LettersStatus.NEW,
    //                      FileName = ""
    //                  };
    //                  if (placment != null)
    //                  {
    //                      entityy.SenderDepartment = placment.OrgStructureId;

    //                  }
    //                  db.LetterSender.Add(entityy);
    //                  db.SaveChanges();

    //              }
              
          

    //return Json(new[] { incommingLetterRegistration }.ToDataSourceResult(request, ModelState));
    //    }
    //    public ActionResult LetterSender_Read([DataSourceRequest]DataSourceRequest request, string bookId)
    //    {
    //        IQueryable<LetterSender> LetterSender = db.LetterSender.Where(x => x.BookNo == bookId);
    //        DataSourceResult result = LetterSender.ToDataSourceResult(request, letterSender => new
    //        {
    //            LetterSenderId = letterSender.LetterSenderId,
    //            Date = letterSender.Date,
    //            Receiver = letterSender.Receiver,
    //            Description = letterSender.Description,
    //            Sender = letterSender.Sender,
    //            BookNo = letterSender.BookNo,
    //            OwnerName =letterSender.OwnerName,// GetOwener(bookId),
    //            Status = letterSender.Status,
    //            StatusName = letterSender.StatusName,
    //            DateAmharic = letterSender.DateAmharic,
    //            FileName = letterSender.FileName,
    //            SenderDepartment = letterSender.SenderDepartment,
    //            CurrentStatus = CheckStatus(letterSender.BookNo),
    //        });

    //        return Json(result);
    //    }
    //    public void Download(int id)
    //    {
    //        HRMInputs.SelectedFile = id;

    //    }
    //    //public ActionResult DownloadFile(string filename)
    //    //{
    //    //    if (filename == "")
    //    //    {
    //    //        string messages = "Please select the row first!!";
    //    //        return new JavaScriptResult() { Script = "alert('" + messages + "');" };
    //    //    }
    //    //    HRMInputs.LetterId = filename;
    //    //    HRMInputs.Owner = filename;
    //    //    db = new ExceedDbContext();
    //    //    try
    //    //    {
    //    //        var attachment =
    //    //                db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.LETTER && x.Remark == HRMInputs.Owner)
    //    //                    .FirstOrDefault();
    //    //        if (attachment != null)
    //    //        {
    //    //            WebClient req = new WebClient();
    //    //            HttpResponse response = System.Web.HttpContext.Current.Response;
    //    //            response.Clear();
    //    //            response.ClearContent();
    //    //            response.ClearHeaders();
    //    //            response.Buffer = true;
    //    //            response.AddHeader("Content-Disposition",
    //    //                "attachment;filename=\"" + attachment.Description + "\"");
    //    //            byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.LetterId.ToString() + "-" + AttachmentType.LETTER + "/" + attachment.Description));
    //    //            response.BinaryWrite(datas);
    //    //            response.End();
    //    //            HRMInputs.SelectedFile = 0;
    //    //            return File(filename, attachment.FileType);
    //    //        }
    //    //    }
    //    //    catch (Exception)
    //    //    {

    //    //        // throw;
    //    //    }
    //    //    HRMInputs.SelectedFile = 0;
    //    //    return null;
    //    //}
    //    public int SetStatus(string statusName)
    //    {
    //        int status = 0;
    //        if (statusName == "ANSWERED" || statusName == "TRANSFERED")
    //        {
    //            status = 1;
    //        }
    //        return status;
    //    }
    //    private string GetOwener(string bookno)
    //    {
    //        var book = db.NewIncommingLetter.Where(x => x.BookNo == bookno).FirstOrDefault();
    //        if (book != null)
    //        {
    //            return book.OwnerName;
    //        }
    //        return "";

    //    }
    //    [AcceptVerbs(HttpVerbs.Post)]
    //    [Authorize(Roles = HR_Admin)]
    //    public ActionResult NewIncommingLetter_Update([DataSourceRequest]DataSourceRequest request, IncomingLetterRegistration incommingLetterRegistration)
    //    {
    //        ModelState.Clear();
    //        if (ModelState.IsValid)
    //        {
    //            if (RecivedDateAmharic==null)
    //            {
    //                RecivedDateAmharic = incommingLetterRegistration.RecivedDateAmharic;
    //                var splitDate = RecivedDateAmharic.Split('/');
    //                RecivedDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
    //            }
    //             if (OutgoingLetterDateAm==null)
    //            {
    //                OutgoingLetterDateAm = incommingLetterRegistration.OutgoingLetterDateAm ;
    //                var splitDate = OutgoingLetterDateAm.Split('/');
    //                OutgoingLetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
    //            }
    //             if (LetterAmharic== null)
    //            {
    //                LetterAmharic = incommingLetterRegistration.LetterAmharic ;
    //                var splitDate = LetterAmharic.Split('/');
    //                LetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
    //            }

    //            var entity = new IncomingLetterRegistration
    //            {
    //                BookNo = incommingLetterRegistration.BookNo,
    //                LetterDate = LetterDate,
    //                LetterReferenceNo = incommingLetterRegistration.LetterReferenceNo,
    //                RecivedDate = RecivedDate,
    //                OwnerName = incommingLetterRegistration.OwnerName,
    //                Subject = incommingLetterRegistration.Subject,
    //                Shelve = incommingLetterRegistration.Shelve,
    //                Row = incommingLetterRegistration.Row,
    //                FolderNo = incommingLetterRegistration.FolderNo,
    //                FileNo = incommingLetterRegistration.FileNo,
    //                NoOfAttachment = incommingLetterRegistration.NoOfAttachment,
    //                OrgStructureId = incommingLetterRegistration.OrgStructureId,
    //                RefernceType = "Institution",
    //                Remark = incommingLetterRegistration.Remark,
    //                OutgoingLetterDate = OutgoingLetterDate,
    //                OutgoingLetterReferenceNo = incommingLetterRegistration.OutgoingLetterReferenceNo,
    //                OutgoingOwnerName = incommingLetterRegistration.OutgoingOwnerName,
    //                OutgoingInstitutionTypeId = incommingLetterRegistration.OutgoingInstitutionTypeId,
    //                OutgoingSubject = incommingLetterRegistration.OutgoingSubject,
    //                OutgoingShelve = incommingLetterRegistration.OutgoingShelve,
    //                OutgoingRow = incommingLetterRegistration.OutgoingRow,
    //                OutgoingFolderNo = incommingLetterRegistration.OutgoingFolderNo,
    //                OutgoingFileNo = incommingLetterRegistration.OutgoingFileNo,
    //                OutgoingNoOfAttachment = incommingLetterRegistration.OutgoingNoOfAttachment,
    //                OutgoingSerialNo = incommingLetterRegistration.OutgoingSerialNo,
    //                OutgoingRefernceType = "Institution",
    //                OutgoingRemark = incommingLetterRegistration.OutgoingRemark,
    //                InstitutionTypeId = incommingLetterRegistration.InstitutionTypeId,
    //                VolumeNo = incommingLetterRegistration.VolumeNo,
    //                OutgoingVolumeNo = incommingLetterRegistration.OutgoingVolumeNo,
    //                LetterAmharic = LetterAmharic,
    //                OutgoingLetterDateAm = OutgoingLetterDateAm,
    //                RecivedDateAmharic = RecivedDateAmharic,
    //                EnableDisbale = true,
    //                LastModified = DateTime.Now ,
    //                DateCreated = incommingLetterRegistration.DateCreated,
    //                CreatedBy = incommingLetterRegistration.CreatedBy,
    //                ModifiedBy = User .Identity .Name ,
    //                OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
    //            };

    //            db.NewIncommingLetter.Attach(entity);
    //            db.Entry(entity).State = EntityState.Modified;
    //            db.SaveChanges();
    //        }

    //        return Json(new[] { incommingLetterRegistration }.ToDataSourceResult(request, ModelState));
    //    }
    //    private bool CheckValdationToDelete(string id)
    //    {
    //        ErrorMessage = "";
    //        #region LetterSender
    //        var sendrs = db.LetterSender.Where(x => x.BookNo == id.ToString()).FirstOrDefault();
    //        if (sendrs != null)
    //        {
    //            ErrorMessage = "You cannot delete this incomming letter. Please Delete letter sender information first.";
    //            return false;
    //        }
    //        #endregion
    //        #region LetterStatus
    //        var status = db.LetterStatus.Where(x => x.BookNo == id.ToString()).FirstOrDefault();
    //        if (status != null)
    //        {
    //            ErrorMessage = "You cannot delete this incomming letter. Please Delete letter status information first.";
    //            return false;
    //        }
    //        #endregion
    //        return true;
    //    }
    //    [AcceptVerbs(HttpVerbs.Post)]
    //    [Authorize(Roles = HR_Employee_SelfService)]
    //    public ActionResult NewIncommingLetter_Destroy([DataSourceRequest]DataSourceRequest request, IncomingLetterRegistration incommingLetterRegistration)
    //    {
    //        ModelState.Clear();
    //        if (ModelState.IsValid)
    //        {
    //            if (CheckValdationToDelete(incommingLetterRegistration.BookNo))
    //            {
    //                ModelState.AddModelError("", ErrorMessage );
    //            }
    //            else
    //            {
    //                var entity = new IncomingLetterRegistration
    //                {
    //                    BookNo = incommingLetterRegistration.BookNo,
    //                    LetterDate = incommingLetterRegistration.LetterDate,
    //                    LetterReferenceNo = incommingLetterRegistration.LetterReferenceNo,
    //                    RecivedDate = incommingLetterRegistration.RecivedDate,
    //                    OwnerName = incommingLetterRegistration.OwnerName,
    //                    Subject = incommingLetterRegistration.Subject,
    //                    Shelve = incommingLetterRegistration.Shelve,
    //                    Row = incommingLetterRegistration.Row,
    //                    FolderNo = incommingLetterRegistration.FolderNo,
    //                    FileNo = incommingLetterRegistration.FileNo,
    //                    NoOfAttachment = incommingLetterRegistration.NoOfAttachment,
    //                    OrgStructureId = incommingLetterRegistration.OrgStructureId,
    //                    RefernceType = incommingLetterRegistration.RefernceType,
    //                    Remark = incommingLetterRegistration.Remark,
    //                    OutgoingLetterDate = incommingLetterRegistration.OutgoingLetterDate,
    //                    OutgoingLetterReferenceNo = incommingLetterRegistration.OutgoingLetterReferenceNo,
    //                    OutgoingOwnerName = incommingLetterRegistration.OutgoingOwnerName,
    //                    OutgoingInstitutionTypeId = incommingLetterRegistration.OutgoingInstitutionTypeId,
    //                    OutgoingSubject = incommingLetterRegistration.OutgoingSubject,
    //                    OutgoingShelve = incommingLetterRegistration.OutgoingShelve,
    //                    OutgoingRow = incommingLetterRegistration.OutgoingRow,
    //                    OutgoingFolderNo = incommingLetterRegistration.OutgoingFolderNo,
    //                    OutgoingFileNo = incommingLetterRegistration.OutgoingFileNo,
    //                    OutgoingNoOfAttachment = incommingLetterRegistration.OutgoingNoOfAttachment,
    //                    OutgoingSerialNo = incommingLetterRegistration.OutgoingSerialNo,
    //                    OutgoingRefernceType = incommingLetterRegistration.OutgoingRefernceType,
    //                    OutgoingRemark = incommingLetterRegistration.OutgoingRemark,
    //                    InstitutionTypeId = incommingLetterRegistration.InstitutionTypeId,
    //                    VolumeNo = incommingLetterRegistration.VolumeNo,
    //                    OutgoingVolumeNo = incommingLetterRegistration.OutgoingVolumeNo,
    //                    LetterAmharic = incommingLetterRegistration.LetterAmharic,
    //                    OutgoingLetterDateAm = incommingLetterRegistration.OutgoingLetterDateAm,
    //                    RecivedDateAmharic = incommingLetterRegistration.RecivedDateAmharic
    //                };

    //                if (incommingLetterRegistration.EnableDisbale)
    //                {
    //                    incommingLetterRegistration.EnableDisbale = false;
    //                }
    //                else
    //                {
    //                    incommingLetterRegistration.EnableDisbale = true;
    //                }
    //                #region delete log
    //                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted institution incomming letter of book no" + incommingLetterRegistration.BookNo);
    //                #endregion
    //                db.NewIncommingLetter.Attach(entity);
    //                db.Entry(entity).State = EntityState.Modified;
    //                db.SaveChanges();
    //            }
    //        }

    //        return Json(new[] { incommingLetterRegistration }.ToDataSourceResult(request, ModelState));
    //    }
    //    public ActionResult UpdateStatus(string BookNo)
    //    {
    //        var incommingLetterRegistration = db.NewIncommingLetter.Where(x => x.BookNo == BookNo).FirstOrDefault();
    //        if (incommingLetterRegistration != null)
    //        {
                
    //                var entity = new IncomingLetterRegistration
    //                {
    //                    BookNo = incommingLetterRegistration.BookNo,
    //                    LetterDate = incommingLetterRegistration.LetterDate,
    //                    LetterReferenceNo = incommingLetterRegistration.LetterReferenceNo,
    //                    RecivedDate = incommingLetterRegistration.RecivedDate,
    //                    OwnerName = incommingLetterRegistration.OwnerName,
    //                    Subject = incommingLetterRegistration.Subject,
    //                    Shelve = incommingLetterRegistration.Shelve,
    //                    Row = incommingLetterRegistration.Row,
    //                    FolderNo = incommingLetterRegistration.FolderNo,
    //                    FileNo = incommingLetterRegistration.FileNo,
    //                    NoOfAttachment = incommingLetterRegistration.NoOfAttachment,
    //                    OrgStructureId = incommingLetterRegistration.OrgStructureId,
    //                    RefernceType = incommingLetterRegistration.RefernceType,
    //                    Remark = incommingLetterRegistration.Remark,
    //                    OutgoingLetterDate = incommingLetterRegistration.OutgoingLetterDate,
    //                    OutgoingLetterReferenceNo = incommingLetterRegistration.OutgoingLetterReferenceNo,
    //                    OutgoingOwnerName = incommingLetterRegistration.OutgoingOwnerName,
    //                    OutgoingInstitutionTypeId = incommingLetterRegistration.OutgoingInstitutionTypeId,
    //                    OutgoingSubject = incommingLetterRegistration.OutgoingSubject,
    //                    OutgoingShelve = incommingLetterRegistration.OutgoingShelve,
    //                    OutgoingRow = incommingLetterRegistration.OutgoingRow,
    //                    OutgoingFolderNo = incommingLetterRegistration.OutgoingFolderNo,
    //                    OutgoingFileNo = incommingLetterRegistration.OutgoingFileNo,
    //                    OutgoingNoOfAttachment = incommingLetterRegistration.OutgoingNoOfAttachment,
    //                    OutgoingSerialNo = incommingLetterRegistration.OutgoingSerialNo,
    //                    OutgoingRefernceType = incommingLetterRegistration.OutgoingRefernceType,
    //                    OutgoingRemark = incommingLetterRegistration.OutgoingRemark,
    //                    InstitutionTypeId = incommingLetterRegistration.InstitutionTypeId,
    //                    VolumeNo = incommingLetterRegistration.VolumeNo,
    //                    OutgoingVolumeNo = incommingLetterRegistration.OutgoingVolumeNo,
    //                    LetterAmharic = incommingLetterRegistration.LetterAmharic,
    //                    OutgoingLetterDateAm = incommingLetterRegistration.OutgoingLetterDateAm,
    //                    RecivedDateAmharic = incommingLetterRegistration.RecivedDateAmharic,
    //                    LastModified = DateTime.Now ,
    //                    DateCreated = incommingLetterRegistration.DateCreated,
    //                    CreatedBy = incommingLetterRegistration.CreatedBy,
    //                    ModifiedBy = User.Identity .Name ,
    //                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
    //                };

    //                if (incommingLetterRegistration.EnableDisbale)
    //                {
    //                    entity.EnableDisbale = false;
    //                }
    //                else
    //                {
    //                    entity.EnableDisbale = true;
    //                }
    //                db = new ExceedDbContext();
    //                db.NewIncommingLetter.Attach(entity);
    //                db.Entry(entity).State = EntityState.Modified;
    //                db.SaveChanges();

              
    //        }

    //        return null;
    //    }
    //    public ActionResult Upload(HttpPostedFileBase file)
    //    {
    //        try
    //        {
    //            if (file.ContentLength > 0)
    //            {
    //                db = new ExceedDbContext();
    //                //int letterid = Convert.ToInt32(HRMInputs.LetterId);
    //                //var recivedLetters =
    //                //    db.RecivedLetters.Where(
    //                //        x => x.RecivedLetterId == letterid).FirstOrDefault();
    //                //if (recivedLetters != null)
    //                //{
    //                //    recivedLetters.FileName = file.FileName;
    //                //    var entityy = new RecivedLetters
    //                //    {
    //                //        RecivedLetterId = recivedLetters.RecivedLetterId,
    //                //        InstnPerson = recivedLetters.InstnPerson,
    //                //        Receiver = recivedLetters.Receiver,
    //                //        Sender = recivedLetters.Sender,
    //                //        Description = recivedLetters.Description,
    //                //        FileName = recivedLetters.FileName,
    //                //        LetterStatus = recivedLetters.LetterStatus,
    //                //        RecivedDate = recivedLetters.RecivedDate,
    //                //        BookNo = recivedLetters.BookNo
    //                //    };
    //                //    db = new ExceedDbContext();
    //                //    db.RecivedLetters.Attach(entityy);
    //                //    db.RecivedLetters.Remove(entityy);
    //                //    db.SaveChanges();
    //                //    db = new ExceedDbContext();
    //                //    entityy.LetterStatus = LettersStatus.NEW;
    //                //    db.RecivedLetters.Add(entityy);

    //                //    db.SaveChanges();
    //                //    recivedLetters.RecivedLetterId = entityy.RecivedLetterId;
    //                //    HRMInputs.LetterId = recivedLetters.RecivedLetterId.ToString();
    //                    var fileName = Path.GetFileName(file.FileName);
    //                    var path = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.LetterId + "-" + AttachmentType.LETTER), fileName);
    //                    var mainPath =
    //                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.LetterId + "-" + AttachmentType.LETTER));
    //                    if (!Directory.Exists(mainPath))
    //                    {
    //                        Directory.CreateDirectory(mainPath);
    //                    }
    //                    file.SaveAs(path);

    //                    var entity = new Attachment
    //                    {

    //                        Reference = HRMInputs.LetterId.ToString(),
    //                        Description = file.FileName,
    //                        FileType = file.ContentType,
    //                        ReferenceType = AttachmentType.LETTER,
    //                        Index = 0,
    //                        URL = path,
    //                        Remark = HRMInputs.Owner

    //                    };

    //                    //entity.Reference = recivedLetters.RecivedLetterId.ToString();
    //                    //entity.Remark = recivedLetters.RecivedLetterId.ToString();
    //                    db.Attachments.Add(entity);

    //                    db.SaveChanges();

    //                //}
    //            }
    //            return RedirectToAction("Index");
    //        }
    //        catch
    //        {
    //            ViewBag.Message = "Upload failed";
    //            return RedirectToAction("Index");
    //        }
    //    }



    //    public ActionResult Attachments_Destroy([DataSourceRequest]DataSourceRequest request, Attachment attachment)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            var entity = new Attachment
    //            {
    //                AttachmentId = attachment.AttachmentId,
    //                ReferenceType = attachment.ReferenceType,
    //                Reference = attachment.Reference,
    //                Description = attachment.Description,
    //                URL = attachment.URL,
    //                BinaryFile = attachment.BinaryFile,
    //                FileType = attachment.FileType,
    //                Index = attachment.Index,
    //                Remark = attachment.Remark
    //            };

    //            db.Attachments.Attach(entity);
    //            db.Attachments.Remove(entity);
    //            db.SaveChanges();
    //            RemoveFiles(attachment.Description);
    //        }

    //        return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
    //    }
    //    public ActionResult RemoveFiles(string fileName)
    //    {
    //        if (fileName != null)
    //        {

    //            var physicalPath = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.LetterId + "-" + AttachmentType.LETTER), fileName);

    //            if (System.IO.File.Exists(physicalPath))
    //            {
    //                // Deleting the File from the Location
    //                System.IO.File.Delete(physicalPath);
    //            }
    //        }

    //        return Content("");
    //    }
      
    //   public ActionResult UploadDocument(HttpPostedFileBase file,string url,  string txtValue, string txtDecsion, string id)
    //    {
    //        try
    //        {
    //           HRMInputs.LetterId= id;
    //            HRMInputs.Owner = "Institution";
              
    
    //                    SetAnswer(id, "",file);
                       
                    

    //            return RedirectToAction("Index");
    //        }
    //        catch (Exception ex)
    //        {
    //            ViewBag.Message = "Upload failed";
    //            return RedirectToAction("Upload");
    //        }
    //    }
    //    public ActionResult DownloadFile(string filename)
    //    {
    //        if (filename == "")
    //        {
    //            string messages = "Please select the row first!!";
    //            return new JavaScriptResult() { Script = "alert('" + messages + "');" };
    //        }
    //        HRMInputs.LetterId = filename;
    //        HRMInputs.Owner = "Institution";
    //        db = new ExceedDbContext();
    //        try
    //        {
    //            var attachment =
    //                    db.LetterSender
    //                        .FirstOrDefault(x => x.LetterSenderId.ToString() == filename);
    //            if (attachment != null)
    //            {
    //                WebClient req = new WebClient();
    //                HttpResponse response = System.Web.HttpContext.Current.Response;
    //                response.Clear();
    //                response.ClearContent();
    //                response.ClearHeaders();
    //                response.Buffer = true;
    //                response.AddHeader("Content-Disposition",
    //                    "attachment;filename=\"" + attachment.FileName + "\"");
    //                byte[] datas = attachment.File;
    //                response.BinaryWrite(datas);
    //                response.End();
    //                HRMInputs.SelectedFile = 0;
    //                return File(attachment.FileName, attachment.FileType);
    //            }
    //        }
    //        catch (Exception)
    //        {

    //            // throw;
    //        }
    //        HRMInputs.SelectedFile = 0;


    //    return    RedirectToAction("Index");
    //        ////return new JavaScriptResult() { Script = "alert('" + "There is no attached File!!" + "');" };
    //    }
    //    public void SetAnswer(string id, string description, HttpPostedFileBase file)
    //    {
    //        db = new ExceedDbContext();
    //        LettersStatus = LettersStatus.ANSWERED;
    //        HRMInputs.LetterId = id;
    //        HRMInputs.Owner = "Institution";
    //       string fileName = "";
    //         byte[] image= null;
    //        if (file.ContentLength > 0)
    //        {

    //             fileName = Path.GetFileName(file.FileName);
    //            var path =
    //                Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.LetterId.ToString() + "-" +
    //                                   AttachmentType.DOCUMENT), fileName);
    //            var mainPath =
    //                Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.LetterId.ToString() + "-" +
    //                                   AttachmentType.DOCUMENT));
    //            if (!Directory.Exists(mainPath))
    //            {
    //                Directory.CreateDirectory(mainPath);
    //            }
    //            if (!System.IO.File.Exists(path))
    //            {



    //                file.SaveAs(path);
    //            }

    //            image = System.IO.File.ReadAllBytes(path);
    //        }
    //        var recivedLetters = db.NewIncommingLetter.Where(x => x.BookNo == id).FirstOrDefault();
    //            if (recivedLetters != null)
    //            {
                  
    //                var status =
    //                    db.LetterSender.Where(x => x.BookNo == id)
    //                        .OrderByDescending(x => x.LetterSenderId)
    //                        .FirstOrDefault();
    //                var entity = new LetterSender();

                      
    //                entity.OwnerName = recivedLetters.OwnerName;
    //                if (status != null)
    //                {
    //                    entity.Receiver = status.Receiver;
    //                }
    //                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
    //                var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
    //                if (placment != null)
    //                {
    //                    entity.SenderDepartment = placment.OrgStructureId;
    //                }
    //                entity.Sender = HrHelper.GetEmployeeFullName(empId);
    //                entity.Description = "Answered";
    //                entity.Date = DateTime.Now;
    //                entity.DateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month,
    //                    DateTime.Now.Year);

    //                entity.Status = LettersStatus.ANSWERED;
    //                entity.Date = DateTime.Now;
    //                entity.BookNo = id;
    //                entity.FileType = file.ContentType;
    //                entity.File = image;
    //                entity.FileName = fileName;
    //                db = new ExceedDbContext();
    //                db.LetterSender.Add(entity);
    //                //db.Entry(entity).State = EntityState.Modified;
    //                db.SaveChanges();
    //            }
            
    //        RedirectToAction("Index");
    //    }
    //    public void SetTransfer(string id, int reciver)
    //    {
    //        db = new ExceedDbContext();
    //        LettersStatus = LettersStatus.TRANSFERED;
    //      //  RecivedLettersId = id;
    //        var recivedLetters = db.NewIncommingLetter.FirstOrDefault(x => x.BookNo == id);
    //        if (recivedLetters != null)
    //        {
    //            var attachment =
    //                db.LetterSender
    //                    .FirstOrDefault(x =>x.BookNo  == id);
    //            var entity = new LetterSender();
    //            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
    //            var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
    //            if (placment != null)
    //            {
    //                entity.SenderDepartment = placment.OrgStructureId;
    //            }
    //                // LetterSenderId = id,
    //            entity.OwnerName = recivedLetters.OwnerName;
    //            entity.Receiver = reciver;
    //            entity.Sender = HrHelper.GetEmployeeFullName(empId);
    //            entity.Description = "Transfred";
    //            entity.Date = DateTime.Now;
    //            entity.DateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month,
    //                DateTime.Now.Year);
    //            if (attachment != null)
    //            {
    //                entity.FileName = attachment.FileName;
    //            }
    //            else
    //            {
    //                entity.FileName = "";
    //            }
    //            entity.Status = LettersStatus.TRANSFERED;
    //            entity.Date = DateTime.Now;
    //            entity.BookNo = id;
              
    //            db = new ExceedDbContext();
    //            db.LetterSender.Add(entity);
    //            //db.Entry(entity).State = EntityState.Modified;
    //            db.SaveChanges();
    //        }
    //        RedirectToAction("Index");
    //    }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
