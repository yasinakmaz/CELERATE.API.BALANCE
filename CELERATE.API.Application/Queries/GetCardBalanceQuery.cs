namespace CELERATE.API.Application.Queries
{
    public class GetCardBalanceQuery : IRequest<CardBalanceDto>
    {
        public string? NfcId { get; set; }
    }

    public class CardBalanceDto
    {
        public string? CardId { get; set; }
        public decimal Balance { get; set; }
    }
}
