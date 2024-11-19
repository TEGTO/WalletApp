namespace WalletApi.Features.AuthFeature.Dtos
{
    public class UserRegistrationRequest
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}