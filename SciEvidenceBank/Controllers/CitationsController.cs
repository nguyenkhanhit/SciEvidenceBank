using SciEvidenceBank.Models;
using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace SciEvidenceBank.Controllers
{
    public class CitationsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Citations/Generate?id=5&style=APA
        [HttpGet]
        public ActionResult Generate(int id, string style)
        {
            var evidence = db.Evidences.Find(id);
            if (evidence == null) return Json(new { success = false, error = "Not found" }, JsonRequestBehavior.AllowGet);

            style = (style ?? "APA").Trim().ToUpperInvariant();
            var citation = BuildCitation(evidence, style);

            return Json(new { success = true, citation }, JsonRequestBehavior.AllowGet);
        }

        // POST: /Citations/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(int evidenceId, string style, string citationText)
        {
            if (string.IsNullOrWhiteSpace(citationText)) return Json(new { success = false });

            var userId = User.Identity.GetUserId();
            var c = new MyCitation
            {
                UserId = userId,
                EvidenceId = evidenceId,
                Style = style,
                CitationText = citationText,
                CreatedAt = DateTime.UtcNow
            };
            db.MyCitations.Add(c);

            // Increment citation counter on the Evidence
            var evidence = db.Evidences.Find(evidenceId);
            if (evidence != null)
            {
                evidence.CitationCount = (evidence.CitationCount < 0) ? 1 : evidence.CitationCount + 1;
            }

            db.SaveChanges();

            return Json(new { success = true, id = c.Id, citations = evidence?.CitationCount ?? 0 });
        }

        private string BuildCitation(Evidence e, string style)
        {
            // Basic heuristics. You can enhance formatting (italics, publishers, pages, etc).
            var authors = (e.Authors ?? "").Trim();
            var year = e.Year.HasValue ? e.Year.Value.ToString() : "n.d.";
            var title = (e.Title ?? "").Trim();
            var source = (e.Source ?? "").Trim();
            var url = (e.Url ?? "").Trim();

            switch (style)
            {
                case "MLA":
                    // MLA: Author. "Title." Source, Year. URL.
                    return $"{authors}. \"{title}.\" {source}{(string.IsNullOrEmpty(source) ? "" : ", ")}{(year == "n.d." ? "" : year + ".")} {(string.IsNullOrEmpty(url) ? "" : url)}".Trim();
                case "HARVARD":
                    // Harvard: Author, Year. Title. Source. Available at: URL.
                    return $"{authors}, {year}. {title}. {source} {(string.IsNullOrEmpty(url) ? "" : "Available at: " + url)}".Trim();
                case "CHICAGO":
                case "TURABIAN":
                    // Chicago Author-Date: Author. Year. "Title." Source. URL.
                    return $"{authors}. {year}. \"{title}.\" {source} {(string.IsNullOrEmpty(url) ? "" : url)}".Trim();
                case "IEEE":
                    // IEEE: [1] A. Author, "Title," Source, Year. URL.
                    var initials = ToInitials(authors);
                    return $"{initials} \"{title},\" {source}, {year}. {(string.IsNullOrEmpty(url) ? "" : url)}".Trim();
                case "APA":
                default:
                    // APA: Author (Year). Title. Source. URL
                    return $"{authors} ({year}). {title}. {source} {(string.IsNullOrEmpty(url) ? "" : url)}".Trim();
            }
        }

        private string ToInitials(string authors)
        {
            // Very simple conversion: "John Smith; Mary Jane" => "J. Smith, M. Jane"
            if (string.IsNullOrWhiteSpace(authors)) return authors;
            var parts = authors.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(p => p.Trim())
                               .Where(p => !string.IsNullOrEmpty(p))
                               .ToArray();
            var sb = new StringBuilder();
            for (int i = 0; i < parts.Length; i++)
            {
                var p = parts[i];
                var names = p.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (names.Length == 0) continue;
                var last = names[names.Length - 1];
                var first = names[0];
                var initial = first.Length > 0 ? first[0] + "." : "";
                if (sb.Length > 0) sb.Append(", ");
                sb.Append(initial).Append(" ").Append(last);
            }
            return sb.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}