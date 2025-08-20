using Azure;
using RERPAPI.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Common
{
    public static class ApiResponseFactory
    {
        public static APIResponse Success(object? data, string? message)
        {                                                                                                               
            return new APIResponse
            {
                status = 1,
                message = message ?? "",
                data = data
            };
        }

        public static APIResponse Fail(Exception? ex,string message)
        {
            return new APIResponse
            {
                status = 0,
                message = message,
                error = ex?.ToString()
            };
        }

    }
}
