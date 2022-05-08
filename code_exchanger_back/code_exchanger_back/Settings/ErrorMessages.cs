namespace code_exchanger_back.Settings
{
    public static class ErrorMessages
    {
        public const string WrongUserPassword = "wrong user password";
        public const string WrongContentPassword = "wrong content password";
        public const string ProhibitedSymbols = "login or password contains prohibited characters";
        public const string LongPassword = "password too long";
        public const string ShortPassword = "password too short";
        public const string BigCode = "your code is too big";
        public const string NoCode = "there's no code";
        public const string NoPermissionDelete = "you have no permission to delete this code";
        public const string NoPermissionChange = "you have no permission to change this code";
        public const string NoUser = "user with this login does not exist";
        public const string UserAlreadyExist = "user with same login exists";
        public const string InvalidLoginLength = "Length of login should be no less 2 and no more 20";
    }
}
