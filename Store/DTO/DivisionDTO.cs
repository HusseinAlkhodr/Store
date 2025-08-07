namespace Store.DTO
{
    public class DivisionDTO
    {
        public required string Name { get; set; }
    }
    public class AddDivisionDTO : DivisionDTO { }
    public class UpdateDivisionDTO : DivisionDTO { }
    public class  GetDivisionDTO : DivisionDTO
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
