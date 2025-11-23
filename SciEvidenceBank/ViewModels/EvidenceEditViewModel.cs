using System.Collections.Generic;

namespace SciEvidenceBank.ViewModels
{
    public class FieldItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
    }

    public class EvidenceEditViewModel
    {
        public SciEvidenceBank.Models.Evidence Evidence { get; set; }
        public List<FieldItem> AllFields { get; set; }
    }
}
