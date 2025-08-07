using Store.Models;

namespace Store.DTO
{
    public class PurchaseBillItemDTO
    {
        public long ItemId { get; set; }
        public long TypeId { get; set; }
        public int QTY { get; set; }
        public decimal ItemCost { get; set; }
    }
    public class AddPurchaseBillItemDTO : PurchaseBillItemDTO
    {

    }
    public class GetPurchaseBillItemDTO
    {
        public required string Code { get; set; }
        public required string ItemDescription { get; set; }
        public int QTY { get; set; }
        public decimal ItemCost { get; set; }
        public required string ItemType { get; set; }
        public decimal Total { get; set; }
    }
    public class GetPurchaseBillItemForUpdateDTO : PurchaseBillItemDTO
    {
        public long Id { get; set; }
        public required string CreatedAt { get; set; }
        public required string CreatedBy { get; set; }
        public required string Code { get; set; }
        public required string ItemDescription { get; set; }
        public ItemType ItemType { get; set; }

    }
    public class GetPurchasedItem
    {
        public required string Code { get; set; }
        public required string Description { get; set; }
        public required string Division { get; set; }
        public required string Vendor { get; set; }
        public decimal Cost { get; set; }
        public required string Type { get; set; }
        public int QTY { get; set; }
        public decimal Total { get; set; }
        public required string CreatedAt { get; set; }
        public long BillId { get; set; }
    }
}
