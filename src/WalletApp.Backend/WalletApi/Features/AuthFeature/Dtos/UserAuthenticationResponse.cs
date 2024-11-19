namespace WalletApi.Features.AuthFeature.Dtos
{
    public class UserAuthenticationResponse
    {
        public AuthToken AuthToken { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
    }
}
