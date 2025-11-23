using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SciEvidenceBank.Models
{
    public class EvidenceResearchField
    {
        [Key]
        public int Id { get; set; }

        [Index("IX_Evidence_Field", 1)]
        public int EvidenceId { get; set; }

        [Index("IX_Evidence_Field", 2)]
        public int ResearchFieldId { get; set; }

        public virtual Evidence Evidence { get; set; }
        public virtual ResearchField ResearchField { get; set; }
    }
}