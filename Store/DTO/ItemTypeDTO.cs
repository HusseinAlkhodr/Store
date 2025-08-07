namespace Store.DTO
{
    public class ItemTypeDTO
    {
        public required string Name { get; set; }
        public int QTY { get; set; }
    }
    public class GetItemTypeDTO : ItemTypeDTO
    {
        public long Id { get; set; }
        public string CreatedAt { get; set; }
    }
}
