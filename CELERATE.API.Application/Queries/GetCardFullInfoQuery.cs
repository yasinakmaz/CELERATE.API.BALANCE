using CELERATE.API.Application.DTOs;
using MediatR;

namespace CELERATE.API.Application.Queries
{
    public class GetCardFullInfoQuery : IRequest<CardFullInfoDto>
    {
        public string? NfcId { get; set; }
    }

    public class CardFullInfoDto
    {
        public CardDto? Card { get; set; }
        public UserDto? User { get; set; }
        public BranchDto? Branch { get; set; }
    }
}
