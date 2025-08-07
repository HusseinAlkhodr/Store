namespace Store.Models
{
    public class Division : BaseModel
    {
        public required string Name { get; set; }
        public virtual ICollection<Item>? Items { get; set; }
    }
}
