using System.Security.Policy;

namespace SchoolProject1640.Models.ModelOfView
{
    public class AdminDashboard
    {
        public string? TotalArticle;
        public string? TotalStudent;
        public string? TotalFaculty;
        public string? ArticleAccept;
        public string? TotalContribution;
        public Dictionary<string, int>? UserPostArticleCount { get; set; }
    }
}
