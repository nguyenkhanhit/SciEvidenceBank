using System.Collections.Generic;

namespace SciEvidenceBank.Models
{
    // partial type extension to avoid touching existing Evidence file structure
    public partial class Evidence
    {
        public virtual ICollection<EvidenceResearchField> EvidenceResearchFields { get; set; }
    }
}