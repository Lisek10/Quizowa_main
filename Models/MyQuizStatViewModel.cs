using System.ComponentModel.DataAnnotations;

namespace Quizowa.Models
{
    public class MyQuizStatViewModel
    {
        
        public required Quiz Quiz { get; set; }
        public int TotalAttempts { get; set; }
        public double AverageScore { get; set; }
    }
}