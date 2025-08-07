using Store.Models.Authenitication;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Models
{
    public class BaseModel
    {
        public long Id { get; set; }
        
        public long CreatedById { get; set; }
        [ForeignKey(nameof(CreatedById))]
        public Account CreatedBy { get; set; }
        public long? UpdatedById { get; set; }
        [ForeignKey(nameof(UpdatedById))]
        public Account UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
