namespace WalletApi
{
    public static class Configuration
    {
        public static string EF_CREATE_DATABASE { get; } = "EFCreateDatabase";
        public static string AUTH_REFRESH_TOKEN_EXPIRY_IN_DAYS { get; } = "AuthSettings:RefreshExpiryInDays";
        public static string AUTH_DATABASE_CONNECTION_STRING { get; } = "AuthenticationDb";
        public static string WALLET_DATABASE_CONNECTION_STRING { get; } = "WalletDb";
        public static string USE_CORS { get; } = "UseCORS";
        public static string LOGIN_PROVIDER { get; } = "LoginProvider";
    }
}