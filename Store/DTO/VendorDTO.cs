namespace Store.DTO
{
    public class VendorDTO
    {
        public required string Name { get; set; }
    }
    public class AddVendorDTO : VendorDTO { }
    public class UpdateVendorDTO : VendorDTO { }
    public class DeleteVendorDTO : VendorDTO
    {
        public long Id { get; set; }
    }
    public class GetVendorDTO : VendorDTO
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
