namespace SciEvidenceBank.Migrations
{
    using SciEvidenceBank.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<SciEvidenceBank.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "SciEvidenceBank.Models.ApplicationDbContext";
        }

        protected override void Seed(SciEvidenceBank.Models.ApplicationDbContext context)
        {
            // Categories
            context.Categories.AddOrUpdate(c => c.Name,
                new Category { Name = "Vật lý", Description = "Dẫn chứng và bài báo liên quan đến vật lý" },
                new Category { Name = "Sinh học", Description = "Dẫn chứng và bài báo liên quan đến sinh học" },
                new Category { Name = "Hóa học", Description = "Dẫn chứng và bài báo liên quan đến hóa học" },
                new Category { Name = "Khoa học Máy tính", Description = "Thuật toán, học máy và khoa học dữ liệu" },
                new Category { Name = "Khoa học Xã hội", Description = "Nghiên cứu xã hội và giáo dục" },
                new Category { Name = "Khác", Description = "Các lĩnh vực khác" }
            );
            context.SaveChanges();

            // Tags
            context.Tags.AddOrUpdate(t => t.Name,
                new Tag { Name = "vũ trụ học" },
                new Tag { Name = "năng lượng tối" },
                new Tag { Name = "thuyết tương đối" },
                new Tag { Name = "evolution" },
                new Tag { Name = "CRISPR" },
                new Tag { Name = "học máy" }
            );
            context.SaveChanges();

            // Helper: lookup category ids
            var physCat = context.Categories.FirstOrDefault(c => c.Name == "Vật lý");
            var bioCat = context.Categories.FirstOrDefault(c => c.Name == "Sinh học");
            var csCat = context.Categories.FirstOrDefault(c => c.Name == "Khoa học Máy tính");

            // Seed evidences (approved / published for demo)
            context.Evidences.AddOrUpdate(e => e.Title,
                new Evidence
                {
                    Title = "Sự giãn nở của vũ trụ đang tăng tốc (Riess et al., 1998)",
                    Authors = "Riess et al.",
                    Year = 1998,
                    Source = "The Astrophysical Journal",
                    AbstractText = "Quan sát các siêu tân tinh loại Ia cho thấy sự giãn nở của vũ trụ đang tăng tốc, gợi ý về sự tồn tại của năng lượng tối.",
                    Url = "https://doi.org/10.1086/300499",
                    FilePath = null,
                    IsPublished = true,
                    Status = EvidenceStatus.Approved,
                    CreatedAt = DateTime.UtcNow.AddYears(-10),
                    CreatedByName = "Hệ thống",
                    ApprovedAt = DateTime.UtcNow.AddYears(-9),
                    ApprovedByName = "Hệ thống",
                    LikesCount = 26,
                    BookmarksCount = 12,
                    ViewsCount = 420,
                    CategoryId = physCat?.Id
                },
                new Evidence
                {
                    Title = "Thuyết tương đối rộng mô tả lực hấp dẫn (Einstein, 1915)",
                    Authors = "Albert Einstein",
                    Year = 1915,
                    Source = "Annalen der Physik",
                    AbstractText = "Thuyết tương đối rộng mô tả lực hấp dẫn là sự uốn cong của không-thời gian gây ra bởi khối lượng và năng lượng. Lý thuyết giải thích quỹ đạo hành tinh và các hiệu ứng hấp dẫn khác.",
                    Url = "https://en.wikisource.org/wiki/The_Field_Equations_of_Gravity",
                    FilePath = null,
                    IsPublished = true,
                    Status = EvidenceStatus.Approved,
                    CreatedAt = DateTime.UtcNow.AddYears(-11),
                    CreatedByName = "Hệ thống",
                    ApprovedAt = DateTime.UtcNow.AddYears(-10),
                    ApprovedByName = "Hệ thống",
                    LikesCount = 35,
                    BookmarksCount = 45,
                    ViewsCount = 980,
                    CategoryId = physCat?.Id
                },
                new Evidence
                {
                    Title = "Sự chọn lọc tự nhiên và tiến hóa (Darwin, 1859)",
                    Authors = "Charles Darwin",
                    Year = 1859,
                    Source = "On the Origin of Species",
                    AbstractText = "Giải thích cách các loài thay đổi theo thời gian thông qua quá trình chọn lọc tự nhiên; các đặc điểm di truyền phù hợp với môi trường sẽ được duy trì.",
                    Url = "https://www.gutenberg.org/ebooks/1228",
                    FilePath = null,
                    IsPublished = true,
                    Status = EvidenceStatus.Approved,
                    CreatedAt = DateTime.UtcNow.AddYears(-15),
                    CreatedByName = "Hệ thống",
                    ApprovedAt = DateTime.UtcNow.AddYears(-14),
                    ApprovedByName = "Hệ thống",
                    LikesCount = 62,
                    BookmarksCount = 18,
                    ViewsCount = 760,
                    CategoryId = bioCat?.Id
                },
                new Evidence
                {
                    Title = "CRISPR–Cas9: công cụ chỉnh sửa gen (Jinek et al., 2012)",
                    Authors = "Jinek et al.",
                    Year = 2012,
                    Source = "Science",
                    AbstractText = "Mô tả cơ chế CRISPR–Cas9 và trình bày bằng chứng cho khả năng chỉnh sửa gene chính xác trong tế bào sống.",
                    Url = "https://doi.org/10.1126/science.1225829",
                    FilePath = null,
                    IsPublished = true,
                    Status = EvidenceStatus.Approved,
                    CreatedAt = DateTime.UtcNow.AddYears(-8),
                    CreatedByName = "Hệ thống",
                    ApprovedAt = DateTime.UtcNow.AddYears(-7),
                    ApprovedByName = "Hệ thống",
                    LikesCount = 48,
                    BookmarksCount = 21,
                    ViewsCount = 540,
                    CategoryId = bioCat?.Id
                },
                new Evidence
                {
                    Title = "ImageNet và bước tiến cho học sâu (Krizhevsky et al., 2012)",
                    Authors = "Alex Krizhevsky, Ilya Sutskever, Geoffrey Hinton",
                    Year = 2012,
                    Source = "NIPS",
                    AbstractText = "Mô tả kiến trúc mạng tích chập hiệu quả và trình bày kết quả vượt trội trên tập ImageNet, thúc đẩy sự phát triển của học sâu trong thị giác máy tính.",
                    Url = "http://papers.nips.cc/paper/4824-imagenet-classification-with-deep-convolutional-neural-networks.pdf",
                    FilePath = null,
                    IsPublished = true,
                    Status = EvidenceStatus.Approved,
                    CreatedAt = DateTime.UtcNow.AddYears(-6),
                    CreatedByName = "Hệ thống",
                    ApprovedAt = DateTime.UtcNow.AddYears(-5),
                    ApprovedByName = "Hệ thống",
                    LikesCount = 91,
                    BookmarksCount = 34,
                    ViewsCount = 1200,
                    CategoryId = csCat?.Id
                }
            );
            context.SaveChanges();

            // Link tags to evidences (if not already linked)
            // Helper to get tag id
            Func<string, Tag> findTag = name => context.Tags.FirstOrDefault(t => t.Name == name);
            Func<string, Evidence> findEvidence = title => context.Evidences.FirstOrDefault(e => e.Title == title);

            void Link(string evidenceTitle, params string[] tagNames)
            {
                var ev = findEvidence(evidenceTitle);
                if (ev == null) return;

                foreach (var tn in tagNames)
                {
                    var tag = findTag(tn);
                    if (tag == null) continue;

                    var exists = context.EvidenceTags.Any(et => et.EvidenceId == ev.Id && et.TagId == tag.Id);
                    if (!exists)
                    {
                        context.EvidenceTags.Add(new EvidenceTag { EvidenceId = ev.Id, TagId = tag.Id });
                    }
                }
            }

            Link("Sự giãn nở của vũ trụ đang tăng tốc (Riess et al., 1998)", "vũ trụ học", "năng lượng tối");
            Link("Thuyết tương đối rộng mô tả lực hấp dẫn (Einstein, 1915)", "thuyết tương đối");
            Link("Sự chọn lọc tự nhiên và tiến hóa (Darwin, 1859)", "evolution");
            Link("CRISPR–Cas9: công cụ chỉnh sửa gen (Jinek et al., 2012)", "CRISPR");
            Link("ImageNet và bước tiến cho học sâu (Krizhevsky et al., 2012)", "học máy");

            context.SaveChanges();
        }
    }
}
