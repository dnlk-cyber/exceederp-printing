using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public static OperationResult Ok(string message = "")
        {
            return new OperationResult { Success = true, Message = message };
        }

        public static OperationResult Fail(string message) =>
            new OperationResult { Success = false, Message = message };
    }
}
