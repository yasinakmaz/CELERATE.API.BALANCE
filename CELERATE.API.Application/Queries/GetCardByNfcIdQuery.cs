using CELERATE.API.Application.DTOs;
using MediatR;

namespace CELERATE.API.Application.Queries
{
    public class GetCardByNfcIdQuery : IRequest<CardDto>
    {
        public string? NfcId { get; set; }
    }
}
