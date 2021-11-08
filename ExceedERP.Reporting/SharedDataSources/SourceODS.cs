using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
   public class SourceODS
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetSource()
        {
            List<string> sourceList = new List<string>();

            sourceList.Add(DataSource.Profile.ToString());
            sourceList.Add(DataSource.Placement.ToString());

            return sourceList;
        }
    }
}
