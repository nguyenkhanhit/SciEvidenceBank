using System;
using System.ComponentModel.DataAnnotations;

namespace SciEvidenceBank.Models
{
    public class MyCitation
    {
        public int Id { get; set; }

        [Required, MaxLength(128)]
        [Display(Name = "Người dùng ID")]
        public string UserId { get; set; }
        [Display(Name = "Bằng chứng ID")]
        public int EvidenceId { get; set; }

        [Required, MaxLength(50)]
        [Display(Name = "Kiểu trích dẫn (APA|MLA|IEEE|Chicago|Harvard)")]
        public string Style { get; set; } // e.g. APA, MLA, Harvard, Chicago, IEEE
        [Display(Name = "Văn bản trích dẫn")]
        public string CitationText { get; set; }
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}