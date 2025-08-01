namespace InstituteManagement.Shared
{
    public class ApiError
    {
        public string Code { get; set; } = default!;
        public string Message { get; set; } = default!;
    }

    public static class ErrorCodes
    {
        public const string ValidationFailed = "ValidationFailed";
        public const string CaptchaFailed = "CaptchaFailed";
        public const string UserExists = "UserExists";
        public const string InvalidNationalId = "InvalidNationalId";
        public const string DatabaseError = "DatabaseError";
        public const string UnknownServerError = "UnknownServerError";
        public const string IdentityError = "IdentityError";
        // Add more as needed
    }

}
