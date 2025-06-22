using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations; // Dodaj ten using, jeśli nie masz

namespace Quizowa.Models
{
   
    public class QuizStatsViewModel
    {
       
        public required Quiz Quiz { get; set; } // Zmieniono na required, aby upewnić się, że jest ustawiony
        public int AttemptsCount { get; set; }
        public double AverageScore { get; set; }
    }

    // ViewModel dla szczegółowych statystyk quizu (QuizDetails)
    public class DetailedQuizStatsViewModel
    {
        // Tutaj również Quiz powinien być zainicjowany istniejącym obiektem Quiz.
        public required Quiz Quiz { get; set; } // Zmieniono na required
        public int TotalAttempts { get; set; }
        public double AverageScore { get; set; }
        public List<QuestionStats> QuestionStatistics { get; set; } = new List<QuestionStats>();
    }

    public class QuestionStats
    {
        public required string QuestionText { get; set; } // Dodano 'required'
        public List<AnswerStats> AnswerStatistics { get; set; } = new List<AnswerStats>();
    }

    public class AnswerStats
    {
        public required string AnswerText { get; set; } // Dodano 'required'
        public double SelectedPercentage { get; set; }
        public bool IsCorrect { get; set; }
    }
}