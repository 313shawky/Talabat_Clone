namespace Talabat.APIs.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        public ApiResponse(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(StatusCode);
        }

        private string? GetDefaultMessageForStatusCode(int? statusCode)
        {
            return StatusCode switch // switch expression C# 7
            {
                400 => "A Bad Request You Have Made",
                401 => "Authorized, You Are Not",
                404 => "Resources Not Found",
                500 => "There Is A Server Error",
                _ => null
            };
        }
    }
}
