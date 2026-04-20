using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeTypeObjectDataSource
    {

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetEmployeeTypess()
        {
            List<string> sourceList = Enum.GetValues(typeof(EmployeeStatus))
                                          .Cast<EmployeeStatus>()
                                          .Where(status => status != EmployeeStatus.NOTSELECTED)
                                          .Select(status => status.ToString())
                                          .ToList();


            return sourceList;
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetEmployeeAssignmentType()
        {
            List<string> sourceList = Enum.GetValues(typeof(AssignmentType))
                                         .Cast<AssignmentType>()
                                         .Where(status => status != AssignmentType.NOTSELECTED)
                                         .Select(status => status.ToString())
                                         .ToList();
            

            return sourceList;
        }



    }
}