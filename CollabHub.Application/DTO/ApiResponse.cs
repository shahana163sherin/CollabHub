using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO
{
    public class ApiResponse<T>
    {
        //public bool Success { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public T? Data { get; set; }
        public ApiError Error { get; set; }

        public static ApiResponse<T> Success(int statusCode, T data, string message="Success")
        {
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                Message = message,
                Data = data,
                Error=null
            };
        }

        public static ApiResponse<T> Fail(int statusCode, string message,string type,string details=null)
        {
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                Message = message,
                Data = default,
                Error = new ApiError
                {
                    Type = type,
                    Details = details
                }
            };
        }
    }
        public class ApiError
        {
            public string Type { get; set; }
            public string Details { get; set; }
        }
    
}
