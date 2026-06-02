using RERPAPI.Model.DTO;

namespace RERPAPI.Model.Common
{
    public static class ApiResponseFactory
    {
        public static APIResponse Success(object? data = null, string? message = "")
        {
            return new APIResponse
            {
                status = 1,
                message = message ?? "",
                error = "",
                data = data
            };
        }

        public static APIResponse Fail(Exception? ex, string message, object? data = null)
        {
            return new APIResponse
            {
                status = 0,
                message = message,
                error = ex?.ToString(),
                data = data
            };
        }

        public static APIResponse Unauthorized(string message)
        {
            return new APIResponse
            {
                status = 403,
                message = message,
                //error = ex?.ToString(),
                //data = data
            };
        }
    }
}