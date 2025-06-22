using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations; 

namespace Quizowa.Models
{
   
    public class QuizStatsViewModel
    {
       
        public required Quiz Quiz { get; set; } 
        public int AttemptsCount { get; set; }
        public double AverageScore { get; set; }
    }

    
    public class DetailedQuizStatsViewModel
    {
        
        public required Quiz Quiz { get; set; } 
        public int TotalAttempts { get; set; }
        public double AverageScore { get; set; }
        public List<QuestionStats> QuestionStatistics { get; set; } = new List<QuestionStats>();
    }

    public class QuestionStats
    {
        public required string QuestionText { get; set; } 
        public List<AnswerStats> AnswerStatistics { get; set; } = new List<AnswerStats>();
    }

    public class AnswerStats
    {
        public required string AnswerText { get; set; } 
        public double SelectedPercentage { get; set; }
        public bool IsCorrect { get; set; }
    }
}