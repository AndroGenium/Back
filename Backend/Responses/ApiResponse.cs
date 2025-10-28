namespace Backend.Responses
{
    public class ApiResponse
    {

        public string Code { get; set; }
        public string Message { get; set; }

        public object? Data { get; set; }

        public ApiResponse(string code, string message , object? data) {
            Code = code;
            Message = message;
            Data = data;
        }
    }

    public static class ErrorCodes
    {
        public const string EmailInUse = "USR_001";
        public const string InvalidPassword = "AUTH_001";
        public const string ServerError = "SYS_001";
        public const string UserNotFound = "USR_002";
        public const string ProductNotFound = "PRO_001";
         
    }

    public static class SuccessCodes
    {
        public const string UserRegistered = "USR_101";
        public const string UserLoggedIn = "USR_102";
    }
    
        
    

}
