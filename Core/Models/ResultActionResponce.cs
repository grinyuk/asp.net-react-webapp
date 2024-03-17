using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ResultActionResponce
    {
        public static ResultActionResponce Error => new ResultActionResponce(false, "error");
        public static ResultActionResponce Success => new ResultActionResponce(true, "success");
        public bool Result { get; }
        public string? Message { get; }

        public ResultActionResponce(bool success, string message)
        {
            Result = success;
            Message = message;
        }
    }
}
