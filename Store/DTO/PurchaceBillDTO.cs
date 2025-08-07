namespace Store.DTO
{
    public class PurchaceBillDTO
    {
        public string? CustomerName { get; set; }
        public ICollection<AddPurchaseBillItemDTO> Items { get; set; } = new List<AddPurchaseBillItemDTO>();

    }
    public class AddPurchaseBillDTO : PurchaceBillDTO
    {
        public int payType { get; set; } = 0;
    }
    public class GetPurchaseBillDTO
    {
        public long Id { get; set; }
        public required string CreatedAt { get; set; }
        public required string CreatedBy { get; set; }
        public required string SourceName { get; set; }
        public long ExchangeRate { get; set; }
        public long Total { get; set; }
    }
    public class GetPurchaseBillDetailsDTO : GetPurchaseBillDTO
    {
        public ICollection<GetPurchaseBillItemDTO> Items { get; set; } = new List<GetPurchaseBillItemDTO>();
    }
    public class GetPurchaseBillUpdateDTO : GetPurchaseBillDTO
    {
        public ICollection<GetPurchaseBillItemForUpdateDTO> Items { get; set; } = new List<GetPurchaseBillItemForUpdateDTO>();
    }
    public class GetDeletedDTO
    {
        public long Id { get; set; }
        public required string DeletedBy { get; set; }
        public required string DeletedAt { get; set; }
        public required string SourceName { get; set; }
        public long ExchangeRate { get; set; }
        public long Total { get; set; }
    }
}
