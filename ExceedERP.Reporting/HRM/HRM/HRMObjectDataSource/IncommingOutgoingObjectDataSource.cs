using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class IncommingOutgoingObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<IncommingOutgoingLetterReport> GetIncommingOutgoing(string type, DateTime from, DateTime to)
        {
            List<IncommingOutgoingLetterReport> letters = new List<IncommingOutgoingLetterReport>();
            IncommingOutgoingLetterReport letter = new IncommingOutgoingLetterReport();
            DateTime dateTow = new DateTime(to.Year, to.Month, to.Day);
            //DateTime dateOne = new DateTime(from.Year, from.Month, from.Day);
            //if (type != null)
            //{
            //    if (type.ToLower() == "incoming")
            //    {
            //        db = new ExceedDbContext();
            //        var incommings = db.NewIncommingLetter.ToList();
            //        if (incommings.Any())
            //        {
            //            foreach (var i in incommings)
            //            {
            //                var institution = db.InstitutionType.FirstOrDefault(x => x.InstitutionTypeId == i.InstitutionTypeId);
            //                if (institution != null)
            //                {
            //                    letter.InstEmpName = institution.InstitutionName;
            //                }

            //                letter = new IncommingOutgoingLetterReport();
            //                letter.Date = i.LetterDate.ToShortDateString();
            //                letter.FileNo = i.FileNo;
            //                letter.Folder = i.FolderNo;
            //                //letter.InstEmpName = i.RefernceType;
            //                letter.NoOfAttachment = i.NoOfAttachment.ToString();
            //                letter.OwnerName = i.OwnerName;
            //                letter.ReferenceNo = i.LetterReferenceNo;
            //                letter.Remark = i.Remark;
            //                letter.Row = i.Row;
            //                letter.Sheleve = i.Shelve;
            //                letter.Subject = i.Subject;
            //                letter.Type = "Incomming";
            //                letters.Add(letter);
            //            }
            //        }
            //    }
            //    else if (type.ToLower() == "outgoing")
            //    {
            //        db = new ExceedDbContext();
            //        var incommings = db.NewOutgoingLetter.ToList();
            //        if (incommings.Any())
            //        {
            //            foreach (var i in incommings)
            //            {
            //                letter = new IncommingOutgoingLetterReport();
            //                //  letter.Date = i.

            //                var institution = db.InstitutionType.FirstOrDefault(x=>x.InstitutionTypeId == i.InstitutionTypeId);
            //                if(institution != null)
            //                {
            //                    letter.InstEmpName = institution.InstitutionName;
            //                }
            //                letter.Date = i.OutgoingLetterDate.ToShortDateString();
            //                letter.FileNo = i.FileNo;
            //                letter.Folder = i.FolderNo;
                        
            //                letter.NoOfAttachment = i.NoOfAttachment.ToString();
            //                letter.OwnerName = i.OwnerName;
            //                letter.ReferenceNo = i.LetterReferenceNo;
            //                letter.Remark = i.Remark;
            //                letter.Row = i.Row;
            //                letter.Sheleve = i.Shelve;
            //                letter.Subject = i.Subject;
            //                letter.Type = "Outgoing";
            //                letters.Add(letter);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        db = new ExceedDbContext();
            //        var incommings = db.NewIncommingLetter.ToList();
            //        if (incommings.Any())
            //        {
            //            foreach (var i in incommings)
            //            {
            //                var institution = db.InstitutionType.FirstOrDefault(x => x.InstitutionTypeId == i.InstitutionTypeId);
            //                if (institution != null)
            //                {
            //                    letter.InstEmpName = institution.InstitutionName;
            //                }

            //                letter = new IncommingOutgoingLetterReport();
            //                letter.Date = i.LetterDate.ToShortDateString();
            //                letter.FileNo = i.FileNo;
            //                letter.Folder = i.FolderNo;
            //                //letter.InstEmpName = i.RefernceType;
            //                letter.NoOfAttachment = i.NoOfAttachment.ToString();
            //                letter.OwnerName = i.OwnerName;
            //                letter.ReferenceNo = i.LetterReferenceNo;
            //                letter.Remark = i.Remark;
            //                letter.Row = i.Row;
            //                letter.Sheleve = i.Shelve;
            //                letter.Subject = i.Subject;
            //                letter.Type = i.InstitutionType.InstitutionName;
            //                letters.Add(letter);
            //            }
            //        }
            //        db = new ExceedDbContext();
            //        var ougoings = db.NewOutgoingLetter.ToList();
            //        if (ougoings.Any())
            //        {
            //            foreach (var i in ougoings)
            //            {
            //                letter = new IncommingOutgoingLetterReport();

            //                var institution = db.InstitutionType.FirstOrDefault(x => x.InstitutionTypeId == i.InstitutionTypeId);
            //                if (institution != null)
            //                {
            //                    letter.InstEmpName = institution.InstitutionName;
            //                }
            //                letter.Date = i.OutgoingLetterDate.ToShortDateString();
            //                letter.FileNo = i.FileNo;
            //                letter.Folder = i.FolderNo;
            //                //letter.InstEmpName = i.RefernceType;
            //                letter.NoOfAttachment = i.NoOfAttachment.ToString();
            //                letter.OwnerName = i.OwnerName;
            //                letter.ReferenceNo = i.LetterReferenceNo;
            //                letter.Remark = i.Remark;
            //                letter.Row = i.Row;
            //                letter.Sheleve = i.Shelve;
            //                letter.Subject = i.Subject;
            //                letter.Type = "Outgoing";
            //                letters.Add(letter);
            //            }
            //        }
            //    }
            //}
            return letters;
        }
        public void Dispose()
        {
            db.Dispose();
        }

    }
}