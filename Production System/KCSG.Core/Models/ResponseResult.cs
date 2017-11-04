using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Core.Models
{
    public class ResponseResult<T>
    {
        public T Data { get; set; }
        public bool IsSuccess { get; set; }
        public string[] ErrorMessages { get; set; }

        public ResponseResult(T data, bool isSuccess, params string[] errorMessages)
        {
            Data = data;
            IsSuccess = isSuccess;
            ErrorMessages = errorMessages;
        }
    }
    public class ResponseResult
    {
        public bool IsSuccess { get; set; }
        public string[] ErrorMessages { get; set; }

        public ResponseResult(bool isSuccess, params string[] errorMessages)
        {
            IsSuccess = isSuccess;
            ErrorMessages = errorMessages;
        }
    }
}
