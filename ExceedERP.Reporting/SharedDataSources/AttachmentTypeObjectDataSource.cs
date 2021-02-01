using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class AttachmentTypeObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetAttachmentType()
        {
            List<string> sourceList = new List<string>
            {

                "PHOTO",
                "EXPERIENCE",
                "TRAINING",
                "EDUCATION",
                "COMMITMENT",
                "INCOMING_LETTER",
                "OUTGOING_LETTER",
                "CLEARANCE",
                "LOGO",
                "OTHER"


            };

            return sourceList;
        }
    }
}