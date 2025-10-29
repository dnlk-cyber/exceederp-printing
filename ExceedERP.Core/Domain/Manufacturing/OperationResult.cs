using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing
{
    public class OperationResult
    {

        public bool Success { get; set; }
        public string Message { get; set; }
        public static OperationResult Ok(string message = "") => new OperationResult { Message = message, Success = true };
        public static OperationResult Fail(string message)
        {
            return new OperationResult { Success = false, Message = message };
        }
    }
}
