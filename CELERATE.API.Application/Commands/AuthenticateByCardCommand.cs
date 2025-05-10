namespace CELERATE.API.Application.Commands
{
    public class AuthenticateByCardCommand : IRequest<AuthenticationResult>
    {
        public string NfcCardId { get; set; }
    }
}
