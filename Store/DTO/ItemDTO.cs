using System.ComponentModel.DataAnnotations;

namespace Store.DTO
{
    public class ItemDTO
    {
        [Required(ErrorMessage = "باركود المادة مطلوب")]
        public required string Code { get; set; }
        [Required(ErrorMessage = "اسم المادة مطلوب")]
        public required string Description { get; set; }
        public long Cost { get; set; } = 0;
        public long Price { get; set; } = 0;
        public int? Qty { get; set; } = 0;
        [Required(ErrorMessage = "يرجى اختيار المورد")]
        [Range(1, int.MaxValue, ErrorMessage = "يرجى اختيار مورد صالح")]
        public long VendorId { get; set; }
        [Required(ErrorMessage = "يرجى اختيار القسم")]
        [Range(1, int.MaxValue, ErrorMessage = "يرجى اختيار قسم صالح")]
        public long DivisionId { get; set; }
    }
    public class GetItemDTO : ItemDTO
    {
        public long Id { get; set; }
        public required string Vendor { get; set; }
        public required string Division { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string TypeName = "قطعة";
    }
    public class AddItemDTO : ItemDTO { }
    public class UpdateItemDTO : ItemDTO
    {
        public long Id { get; set; }
    }
    public class DeleteItemDTO : ItemDTO
    {
        public long Id { get; set; }
    }
    public class GetPurchaseItemDTO
    {
        public long Id { get; set; }
        public required string Code { get; set; }
        public required string Description { get; set; }
        public long Cost { get; set; }
    }
    public class GetSaleItemDTO
    {
        public long Id { get; set; }
        public required string Code { get; set; }
        public required string Description { get; set; }
        public long Price { get; set; }
        public int QTY { get; set; }
    }
}
