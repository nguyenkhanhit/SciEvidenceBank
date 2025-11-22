
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SciEvidenceBank.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public virtual ICollection<EvidenceTag> EvidenceTags { get; set; } = new List<EvidenceTag>();
    }
}