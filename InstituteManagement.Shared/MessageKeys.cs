namespace InstituteManagement.Shared
{
    public static class MessageKeys
    {
        public static class Auth
        {
            public enum Keys
            {
                RecaptchaTokenRequired,
                InvalidCredentials,
                SignInSuccess,
                UsernameOrEmailRequired,
                UsernameOrEmailMaxLength,
                PasswordRequired,
                PasswordMinLength
            }
        }

        public static class Signup
        {
            public enum Keys
            {
                BirthdayMustBeInPast,
                CaptchaFailed,
                EmailFormatInvalid,
                EmailRequired,
                FirstNameMaxLength,
                FirstNameRequired,
                InvalidNationalId,
                LastNameMaxLength,
                LastNameRequired,
                NationalIdAlreadyExists,
                NationalIdMustBe10Digits,
                NationalIdRequired,
                NationalityCodeRequired,
                PersonSaveError,
                RecaptchaTokenRequired,
                SignupSuccess,
                UnexpectedError,
                UserCreationFailed,
                UsernameAlreadyExists,
                UsernameMinLength,
                UsernameMaxLength,
                UsernameRequired,
                DisplayNameRequired
            }
        }

        public static class UpdateAccount
        {
            public enum Keys
            {
                RecaptchaTokenRequired,
                EmailInvalid,
                PhoneInvalid,
                PasswordMinLength,
                PasswordUppercaseRequired,
                PasswordLowercaseRequired,
                PasswordDigitRequired,
                PasswordSpecialRequired
            }
        }
    }
}
