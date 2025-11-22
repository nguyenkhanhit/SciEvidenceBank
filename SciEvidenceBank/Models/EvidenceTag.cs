//SciEvidenceBank\Models\EvidenceTag.cs
using System.ComponentModel.DataAnnotations.Schema;

namespace SciEvidenceBank.Models
{
    public class EvidenceTag
    {
        public int EvidenceId { get; set; }
        public virtual Evidence Evidence { get; set; }

        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }
    }
}