using CELERATE.API.Application.Models;
using MediatR;

namespace CELERATE.API.Application.Commands
{
    public class QuickAuthCommand : IRequest<AuthenticationResult>
    {
        public string? NfcCardId { get; set; }
    }
}
