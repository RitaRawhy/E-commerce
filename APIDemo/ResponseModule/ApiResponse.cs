namespace APIDemo.ResponseModule
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = GetDefaultMessageForStatusCode(statusCode);
        }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode
                switch
            {
                400 => "Bad Request",
                401 => "You are Not Authorized",
                404 => "Resource Not Fount",
                500 => "Server Error",
                _ => null
            };
        }
    }
}
