namespace Store.Models
{
    public class Vendor : BaseModel
    {
        public required string Name { get; set; }
        public virtual ICollection<Item>? Items { get; set; }
    }
}
