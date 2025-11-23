//csharp SciEvidenceBank\Controllers\RecommendationController.cs
using Microsoft.AspNet.Identity;
using SciEvidenceBank.Models;
using SciEvidenceBank.Services;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace SciEvidenceBank.Controllers
{
    [Authorize]
    public class RecommendationController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Recommendation/UserRecommendations
        public ActionResult UserRecommendations(int top = 10)
        {
            var userId = User.Identity.GetUserId();
            var svc = new RecommendationService(db);
            var recs = svc.GetRecommendationsForUser(userId, top).Select(r => r.Evidence).ToList();
            return PartialView("_Recommendations", recs);
        }

        // GET: /Recommendation/ExportTrainingData
        // Restrict to Admin (adjust role as needed)
        [Authorize()]
        public ActionResult ExportTrainingData(int negativesPerPositive = 3)
        {
            // Prepare popularity normalizers
            var evidences = db.Evidences.ToList();
            var maxLikes = (double)(evidences.Any() ? evidences.Max(e => e.LikesCount) : 1);
            var maxViews = (double)(evidences.Any() ? evidences.Max(e => e.ViewsCount) : 1);
            var maxCitations = (double)(evidences.Any() ? evidences.Max(e => e.CitationCount) : 1);

            var allEvidenceIds = evidences.Select(e => e.Id).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("UserId,CategoryId,EvidenceId,EvidencePopularity,Label");

            // For each user who has at least one citation, create positive rows and sample negatives
            var usersWithCitations = db.MyCitations.Select(c => c.UserId).Distinct().ToList();

            foreach (var userId in usersWithCitations)
            {
                var positiveEvidenceIds = db.MyCitations
                    .Where(c => c.UserId == userId)
                    .Select(c => c.EvidenceId)
                    .Distinct()
                    .ToList();

                var positiveCategoryIds = db.Evidences
                    .Where(e => positiveEvidenceIds.Contains(e.Id) && e.CategoryId.HasValue)
                    .Select(e => e.CategoryId.Value)
                    .Distinct()
                    .ToList();

                // positive rows
                foreach (var eid in positiveEvidenceIds)
                {
                    var e = evidences.FirstOrDefault(x => x.Id == eid);
                    if (e == null) continue;
                    var popularity = ((double)e.LikesCount / maxLikes) * 0.5
                                   + ((double)e.ViewsCount / maxViews) * 0.3
                                   + ((double)e.CitationCount / maxCitations) * 0.2;
                    var catId = e.CategoryId ?? -1;
                    sb.AppendLine($"{Escape(userId)},{catId},{e.Id},{popularity:F6},1");
                }

                // negative sampling: pick evidences not cited by this user (deterministic top choices)
                var negativeCandidates = allEvidenceIds.Except(positiveEvidenceIds).Take(negativesPerPositive * positiveEvidenceIds.Count()).ToList();
                foreach (var eid in negativeCandidates)
                {
                    var e = evidences.FirstOrDefault(x => x.Id == eid);
                    if (e == null) continue;
                    var popularity = ((double)e.LikesCount / maxLikes) * 0.5
                                   + ((double)e.ViewsCount / maxViews) * 0.3
                                   + ((double)e.CitationCount / maxCitations) * 0.2;
                    var catId = e.CategoryId ?? -1;
                    sb.AppendLine($"{Escape(userId)},{catId},{e.Id},{popularity:F6},0");
                }
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "training_data.csv");
        }

        private string Escape(string s)
        {
            if (s == null) return "";
            return s.Replace("\"", "\"\"");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}