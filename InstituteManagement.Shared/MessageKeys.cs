namespace InstituteManagement.Shared
{
    public static class MessageKeys
    {
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
                UsernameRequired
            }
        }
    }
}
