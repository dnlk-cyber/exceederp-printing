using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Procurements.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{
   
    [DataObject]
    public class TranscriptWithSupplierObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TranscriptViewModel> GetTranscripts()
        {
            List<TranscriptViewModel> transcriptList = new List<TranscriptViewModel>();
            TranscriptViewModel transcript = new TranscriptViewModel();
            var workunits = db.MainCompanyStructures.ToList();
            foreach(var workunit in workunits)
            {
                transcript = new TranscriptViewModel();
                transcript.Name = workunit.Name;
                transcript.Id = workunit.OrgStructureId;
                transcriptList.Add(transcript);
            }
            var suppliers = db.BidPParticipatingSupplierses.ToList();
            foreach (var supplier in suppliers)
            {
                transcript = new TranscriptViewModel();
                transcript.Name = supplier.SupplierName;
                transcript.Id = Convert.ToInt32(supplier.SupplierId);
                transcriptList.Add(transcript);
            }
            return transcriptList;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TranscriptViewModel> GetTranscriptsWithSupplier(string id)
        {
            List<TranscriptViewModel> transcriptList = new List<TranscriptViewModel>();
            TranscriptViewModel transcript = new TranscriptViewModel();
            var workunits = db.MainCompanyStructures.FirstOrDefault(x=>x.OrgStructureId.ToString()==id);
            if(workunits!=null)
            {
                transcript.Name = workunits.Name;
            }
            var suppliers = db.BidPParticipatingSupplierses.FirstOrDefault(x=>x.BidPParticipatingSuppliersID.ToString() == id);
           if(suppliers!=null)
            {
                transcript.Name = suppliers.SupplierName;
            }
            transcriptList.Add(transcript);
            return transcriptList;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TranscriptViewModel> GetParticiipanSuppliers()
        {
            List<TranscriptViewModel> transcriptList = new List<TranscriptViewModel>();
            TranscriptViewModel transcript = new TranscriptViewModel();
            
            var suppliers = db.BidPParticipatingSupplierses.ToList();
            foreach(var supplier in suppliers)
            {
                transcript = new TranscriptViewModel()
                {
                    Name = supplier.SupplierName,
                    Id = supplier.BidPParticipatingSuppliersID
                };
                transcriptList.Add(transcript);
            }
           
            return transcriptList;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TranscriptViewModel> GetSupplierTranscript(string participantsupplier)
        {
            List<TranscriptViewModel> transcriptList = new List<TranscriptViewModel>();
            TranscriptViewModel transcript = new TranscriptViewModel();
            
            var suppliers = db.BidPParticipatingSupplierses.Where(x=>x.LotID.ToString()== participantsupplier);
            foreach(var supplier in suppliers)
            { 
                transcript = new TranscriptViewModel()
                {
                    Name = supplier.SupplierName,
                    Id = supplier.BidPParticipatingSuppliersID
                };
                transcriptList.Add(transcript);
            }

            return transcriptList;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TranscriptViewModel> GetSupplierForTranscript(object[] participantsupplier)
        {
            List<TranscriptViewModel> transcriptList = new List<TranscriptViewModel>();
            TranscriptViewModel transcript = new TranscriptViewModel();
            if (participantsupplier!=null)
            {
                foreach (var participant in participantsupplier)
                {
                    var supplier = db.BidPParticipatingSupplierses.FirstOrDefault(x => x.BidPParticipatingSuppliersID.ToString() == participant);
                    transcript = new TranscriptViewModel();

                    if (supplier != null) transcript.Name = supplier.SupplierName;
                    //transcript.Id = 0;
                    transcriptList.Add(transcript);
                }
            }
            return transcriptList;
        }
    }
}
        
