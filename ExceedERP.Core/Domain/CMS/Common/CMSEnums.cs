using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CMS
{
    public enum TraineeType
    {
        Individual,
        Company
    }
    public enum TrainingProgramStatus
    {
        Registered = 0,
        OnTraining = 1,
        Completed = 2,
    }
    public enum ISOChecklistValueType
    {
        Yes_Or_No,
        Numeric_Value
    }
}
