namespace Store.DTO
{
    public class SaleBillDTO
    {
        public string? CustomerName { get; set; }
        public ICollection<AddSaleBillItemDTO> Items { get; set; } = new List<AddSaleBillItemDTO>();
    }
    public class GetSaleBillDTO : SaleBillDTO
    {
        public long Id { get; set; }
        public required string CreatedAt { get; set; }
        public required string CreatedBy { get; set; }
        public long ExchangeRate { get; set; }
        public long Total { get; set; }
    }
    public class GetSaleBillDetails
    {
        public long Id { get; set; }
        public required string CreatedAt { get; set; }
        public required string CreatedBy { get; set; }
        public string? CustomerName { get; set; }
        public ICollection<GetSaleBillItemDTO> Items { get; set; } = new List<GetSaleBillItemDTO>();

    }
    public class GetSaleBillDetailsForView
    {
        public long Id { get; set; }
        public required string CreatedAt { get; set; }
        public required string CreatedBy { get; set; }
        public string? CustomerName { get; set; }
        public ICollection<GetSaleBillItemDetailsDTO> Items { get; set; } = new List<GetSaleBillItemDetailsDTO>();
        public decimal Total { get; set; }
        public decimal ExchangeRate { get; set; }
    }
    public class AddSaleBillDTO : SaleBillDTO
    {
        public int payType { get; set; } = 0;
    }
    public class UpdateSaleBillDTO : SaleBillDTO
    {
        public int payType { get; set; } = 0;
    }
}
