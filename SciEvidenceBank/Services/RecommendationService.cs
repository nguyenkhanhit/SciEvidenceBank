//csharp SciEvidenceBank\Services\RecommendationService.cs
using System;
using System.Linq;
using System.Collections.Generic;
using SciEvidenceBank.Models;
using System.Data.Entity;

namespace SciEvidenceBank.Services
{
    // Lightweight, deterministic recommender: fast, explainable, suitable as a baseline
    public class RecommendationService
    {
        private readonly ApplicationDbContext db;

        public RecommendationService(ApplicationDbContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // Returns top N evidences with a score. You can call this from controllers or background jobs.
        public IList<(Evidence Evidence, double Score)> GetRecommendationsForUser(string userId, int top = 10)
        {
            if (string.IsNullOrEmpty(userId)) return new List<(Evidence, double)>();

            // User-selected categories (interests)
            var userCategoryIds = db.UserInterests
                                    .Where(ui => ui.UserId == userId)
                                    .Select(ui => ui.CategoryId)
                                    .ToHashSet();

            // Categories of evidences the user has cited (MyCitations -> Evidence -> Category)
            var citedEvidenceIds = db.MyCitations
                                     .Where(c => c.UserId == userId)
                                     .Select(c => c.EvidenceId)
                                     .ToList();

            var citedCategories = db.Evidences
                                   .Where(e => citedEvidenceIds.Contains(e.Id) && e.CategoryId.HasValue)
                                   .Select(e => e.CategoryId.Value)
                                   .ToHashSet();

            var evidences = db.Evidences
                  .Include(e => e.Category)
                  .ToList();

            // Normalizers to make popularity comparable
            var maxLikes = Math.Max(1, evidences.Max(e => e.LikesCount));
            var maxViews = Math.Max(1, evidences.Max(e => e.ViewsCount));
            var maxCitations = Math.Max(1, evidences.Max(e => e.CitationCount));

            var scored = evidences.Select(e =>
            {
                // Feature signals
                double categoryMatch = (e.CategoryId.HasValue && userCategoryIds.Contains(e.CategoryId.Value)) ? 1.0 : 0.0;
                double citedCategoryMatch = (e.CategoryId.HasValue && citedCategories.Contains(e.CategoryId.Value)) ? 1.0 : 0.0;

                // popularity normalized [0..1]
                double popularity = (0.5 * (double)e.LikesCount / maxLikes)
                                  + (0.3 * (double)e.ViewsCount / maxViews)
                                  + (0.2 * (double)e.CitationCount / maxCitations);

                // optional penalty if user already cited this exact evidence (promote novel suggestions)
                double citedPenalty = citedEvidenceIds.Contains(e.Id) ? -0.5 : 0.0;

                // Weighted linear combination (tune weights in config)
                double score = 3.0 * categoryMatch
                             + 2.0 * citedCategoryMatch
                             + 2.0 * popularity
                             + citedPenalty;

                return (Evidence: e, Score: score);
            });

            return scored.OrderByDescending(x => x.Score)
                         .Take(top)
                         .ToList();
        }
    }
}