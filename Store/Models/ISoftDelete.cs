namespace Store.Models
{
    public interface ISoftDelete
    {
        public int IsArchived { get; set; }
        public DateTime? ArchiveDate { get; set; }
        public long? ArchivedById { get; set; }
    }
}
