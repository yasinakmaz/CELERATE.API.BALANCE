namespace CELERATE.API.Application.DTOs.ReactOptimized
{
    public class CustomerListDto
    {
        public List<CustomerSummaryDto> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
    }
}
